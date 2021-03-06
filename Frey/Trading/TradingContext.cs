using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Automata.Core;
using Automata.Core.Extensions;
using Automata.Entities;
using Automata.Mechanisms;
using Automata.Quantitatives.Indicators;
using Automata.Strategies;

namespace Automata.Trading
{
    public abstract class TradingContext
    {
        protected readonly ReaderWriterLockSlim PriceHistoryDataLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        protected readonly ReaderWriterLockSlim OrderHistoryDataLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        protected readonly ReaderWriterLockSlim TradeHistoryDataLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        private readonly List<Price> priceHistory = new List<Price>(1000000);
        private readonly List<Order> orderHistory = new List<Order>(50000);
        private readonly List<Trade> tradeHistory = new List<Trade>(10000);

        private readonly Portfolio portfolio = new Portfolio();
        public Portfolio Portfolio { get { return portfolio; } }

        public List<Price> PriceHistory
        {
            get
            {
                try
                {
                    PriceHistoryDataLock.EnterReadLock();
                    return priceHistory;
                }
                finally
                {
                    PriceHistoryDataLock.ExitReadLock();
                }
            }
        }

        public List<Order> OrderHistory
        {
            get
            {
                try
                {
                    OrderHistoryDataLock.EnterReadLock();
                    return orderHistory;
                }
                finally
                {
                    OrderHistoryDataLock.ExitReadLock();
                }
            }
        }

        public List<Trade> TradeHistory
        {
            get
            {
                try
                {
                    TradeHistoryDataLock.EnterReadLock();
                    return tradeHistory;
                }
                finally
                {
                    TradeHistoryDataLock.ExitReadLock();
                }
            }
        }

        protected double InitEquity;

        public DataSource DataSource { get; set; }
        public Strategy Strategy { get; set; }
        public ITradingScope TradingScope { get; set; }
        public DataStatus DataStatus { get; set; }

        private CancellationTokenSource cancellation;

        protected void SavePriceToHistory(Price price)
        {
            try
            {
                PriceHistoryDataLock.EnterWriteLock();
                priceHistory.Add(price);
            }
            finally
            {
                PriceHistoryDataLock.ExitWriteLock();
            }
        }

        protected void SavePricesToHistory(IEnumerable<Price> prices)
        {
            try
            {
                PriceHistoryDataLock.EnterWriteLock();
                foreach (var price in prices.OrderBy(p => p.Start).ThenBy(p => p.Security.Code))
                {
                    priceHistory.Add(price);
                }
            }
            finally
            {
                PriceHistoryDataLock.ExitWriteLock();
            }
        }

        protected void SaveOrderToHistory(Order order)
        {
            try
            {
                OrderHistoryDataLock.EnterWriteLock();
                orderHistory.Add(order);
            }
            finally
            {
                OrderHistoryDataLock.ExitWriteLock();
            }
        }

        protected void SaveOrdersToHistory(IEnumerable<Order> orders)
        {
            try
            {
                OrderHistoryDataLock.EnterWriteLock();
                orderHistory.AddRange(orders);
            }
            finally
            {
                OrderHistoryDataLock.ExitWriteLock();
            }
        }

        protected void SaveTradeToHistory(Trade trade)
        {
            try
            {
                TradeHistoryDataLock.EnterWriteLock();
                tradeHistory.Add(trade);
            }
            finally
            {
                TradeHistoryDataLock.ExitWriteLock();
            }
        }

        protected void SaveTradesToHistory(IEnumerable<Trade> trades)
        {
            try
            {
                TradeHistoryDataLock.EnterWriteLock();
                tradeHistory.AddRange(trades);
            }
            finally
            {
                TradeHistoryDataLock.ExitWriteLock();
            }
        }

        protected virtual void Trade()
        {
            BeforeTrading();
            while (!Strategy.IsTimeToStop && !cancellation.Token.IsCancellationRequested)
            {
                // dequeue one day only
                var prices = RetrievePrices();

                if (prices == null && DataStatus == DataStatus.ReachTimeEnd)
                    break;

                if (prices == null)
                    continue;

                AfterNewPrices(prices);

                // use the timestamp of the price data to be the timestamp of order
                var orderTime = prices.First().Start;
                // generate entries and exits
                List<Order> orders;
                if (!Strategy.CheckIfStopTrading(prices, Portfolio, orderTime, out orders))
                {
                    orders = Strategy.GenerateOrders(prices, Portfolio, orderTime);

                    AfterIndicatorsComputed(Strategy.Indicators);
                    AfterOrdersGenerated(orders);
                }

                if (!orders.IsNullOrEmpty())
                {
                    CheckCrossTrades(orders);

                    var exitOrders = orders.Where(o => o.IsClosing).ToList();
                    var trades = ClosePositions(exitOrders, Portfolio, prices);
                    SaveOrdersToHistory(exitOrders);
                    ComputePortfolio(trades);

                    AfterPositionsClosed(trades);

                    var entryOrders = orders.Where(o => !o.IsClosing).ToList();
                    var positions = ExecuteOrders(entryOrders, prices);
                    SaveOrdersToHistory(entryOrders);
                    ComputePortfolio(positions);

                    AfterPositionsOpened(positions);
                }
            }

            AfterTrading();
        }

        protected virtual void BeforeTrading() { }

        protected virtual void AfterNewPrices(HashSet<Price> prices) { }

        protected virtual void AfterIndicatorsComputed(List<Indicator> indicators) { }

        protected virtual void AfterOrdersGenerated(List<Order> orders) { }

        protected virtual void AfterPositionsClosed(List<Trade> trades) { }

        protected virtual void AfterPositionsOpened(List<Position> positions) { }

        protected virtual void AfterTrading() { }

        private void ComputePortfolio(IEnumerable<Trade> trades)
        {
            if (trades == null) return;
            foreach (var trade in trades)
            {
                Portfolio.ClosePosition(trade);
                if (Portfolio.CashPosition.Value < 0)
                {
                    Utilities.WriteTimedLine("Exploded!");
                    Console.ReadLine();
                }
            }
        }

        private void ComputePortfolio(IEnumerable<Position> positions)
        {
            if (positions == null) return;
            foreach (var position in positions)
            {
                Portfolio.EnterPosition(position);
            }
        }

        protected abstract double CheckMarginRequirement();

        protected abstract void ComputeRisks(IEnumerable<Position> newPositions);

        protected abstract List<Trade> ClosePositions(IEnumerable<Order> orders,
            List<Position> existingPositions, HashSet<Price> prices);

        protected abstract List<Position> ExecuteOrders(List<Order> orders, HashSet<Price> prices);

        protected abstract void CheckCrossTrades(List<Order> orders);

        protected abstract HashSet<Price> RetrievePrices();

        protected abstract void OnNewPriceData(HashSet<Price> prices);

        protected abstract void OnDataStatusChanged(DataStatus status);

        public virtual void AddCash(double quantity, Currency currency, DateTime contributeTime)
        {
            if (Portfolio.CashPosition == null)
            {
                InitEquity = quantity;
                Portfolio.CashPosition = Position.NewCash(quantity, currency, contributeTime);
                SaveOrderToHistory(Portfolio.CashPosition.Order);
            }
            else
            {
                var cashOrder = CashOrder.NewContribute(Portfolio, quantity, contributeTime);
                Portfolio.CashPosition.Add(quantity);
                SaveOrderToHistory(cashOrder);
            }
            Portfolio.States.Add(new PortfolioState(0, quantity, contributeTime, PortfolioAction.ContributeCash));
        }

        public virtual void WithdrawCash(double quantity, DateTime withdrawTime)
        {
            var availableCash = CheckMarginRequirement();
            if (availableCash > 0)
            {
                var cashOrder = CashOrder.NewWithdrawal(Portfolio, quantity, withdrawTime);
                SaveOrderToHistory(cashOrder);
                Portfolio.CashPosition.Add(-quantity);
                // withdraw cash doesn't count as a 'drawdown'
                Portfolio.States.Add(new PortfolioState(0, -quantity, withdrawTime, PortfolioAction.WithdrawCash));
            }
        }

        #region lifecycle methods

        public void Start()
        {
            Strategy.TradingScope = TradingScope;
            Strategy.Initialize();

            DataSource.TradingScope = TradingScope;
            DataSource.Initialize();

            SubscribeDataSource();
            DataSource.Start();

            cancellation = new CancellationTokenSource();
            Task.Factory.StartNew(Trade, cancellation.Token);
        }

        public void Stop()
        {
            Console.WriteLine(Utilities.BracketNow + " Stopping trading.");
            cancellation.Cancel();
            DataSource.Stop();
            UnsubscribeDataSource();
        }

        private void SubscribeDataSource()
        {
            DataSource.NotifyNewPriceData += OnNewPriceData;
            DataSource.DataStatusChanged += OnDataStatusChanged;
        }

        private void UnsubscribeDataSource()
        {
            DataSource.NotifyNewPriceData -= OnNewPriceData;
            DataSource.DataStatusChanged -= OnDataStatusChanged;
        }

        #endregion
    }
}
