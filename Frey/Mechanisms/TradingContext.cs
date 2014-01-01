using System.Diagnostics;
using System.IO;
using Automata.Entities;
using Automata.Core.Exceptions;
using Automata.Core.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System;
using Automata.Strategies;
using Automata.Core;

namespace Automata.Mechanisms
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

        protected double initEquity;

        public DataSource DataSource { get; set; }

        public Strategy Strategy { get; set; }

        public ITradingScope TradingScope { get; set; }

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
                foreach (var price in prices.OrderBy(p => p.Time).ThenBy(p => p.Security.Code))
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
            while (!Strategy.IsTimeToStop && !cancellation.Token.IsCancellationRequested)
            {
                // dequeue one day only
                var prices = RetrievePrices();

                if (prices == null)
                {
                    continue;
                }

                // use the timestamp of the price data to be the timestamp of order
                var orderTime = prices.First().Time;

                // generate entries and exits
                var orders = Strategy.GenerateOrders(prices, Portfolio, orderTime);
                CheckCrossTrades(orders);

                Task<List<Trade>> closePositionTask = null;
                Task<List<Position>> orderExecutionTask = null;
                if (orders.Any())
                {
                    // assuming all are executed immediately
                    closePositionTask = Task.Factory.StartNew(() =>
                    {
                        var trades = ClosePositions(orders, Portfolio, prices);
                        ComputeCashPosition(trades);
                        return trades;
                    });
                    orderExecutionTask = Task.Factory.StartNew(() =>
                    {
                        var positions = ExecuteOrders(orders, prices);
                        ComputeCashPosition(positions);
                        return positions;
                    });

                    // wait for all the orders to be filled.
                    closePositionTask.Wait();
                    orderExecutionTask.Wait();
                }

                if (closePositionTask != null)
                {
                    var newClosedTrades = closePositionTask.Result;
                    if (!newClosedTrades.IsNullOrEmpty())
                    {
                        foreach (var trade in newClosedTrades)
                        {
                            Portfolio.Remove(trade.Position);
                        }
                        // ComputeProfits(newClosedTrades);
                    }
                }

                if (orderExecutionTask != null)
                {
                    var newPositions = orderExecutionTask.Result;
                    if (!newPositions.IsNullOrEmpty())
                    {
                        Portfolio.AddRange(newPositions);
                        ComputeRisks(newPositions);
                    }
                }
            }
            TradeHistory.TrimExcess();

            Console.WriteLine();
            var reportFileName = ("Result_" + Utilities.Now + ".csv").Replace(":", string.Empty);
            using (var sw = new StreamWriter(new FileStream(reportFileName, FileMode.CreateNew)))
                foreach (var trade in TradeHistory)
                {
                    Console.WriteLine(trade);
                    sw.WriteLine(trade.PrintCSVFriendly());
                }
            Console.WriteLine(Utilities.BracketNow + " Stopped trading.");
            Console.WriteLine(Utilities.BracketNow + " Period From {0} To {1}", TradingScope.Start, TradingScope.End);
            Console.WriteLine(Utilities.BracketNow + " Equity: " + Portfolio.CashPosition.Equity);
            Console.WriteLine(Utilities.BracketNow + " Return: " + (Portfolio.CashPosition.Equity / initEquity - 1));
            Process.Start(reportFileName);
        }

        private void ComputeCashPosition(IEnumerable<Trade> trades)
        {
            if (trades == null) return;
            foreach (var trade in trades)
            {
                if (trade.ExitOrder.Side == Side.Long)
                    Portfolio.CashPosition.Add(-trade.ExitEquity);
                else if (trade.ExitOrder.Side == Side.Short)
                    Portfolio.CashPosition.Add(trade.ExitEquity);
            }
        }

        private void ComputeCashPosition(IEnumerable<Position> positions)
        {
            if (positions == null) return;
            foreach (var position in positions)
            {
                if (position.Order.Side == Side.Long)
                    Portfolio.CashPosition.Add(-position.Equity);
                else if (position.Order.Side == Side.Short)
                    Portfolio.CashPosition.Add(position.Equity);
            }
        }

        public virtual void AddCash(double quantity, Currency currency, DateTime contributeTime)
        {
            if (Portfolio.CashPosition == null)
            {
                initEquity = quantity;
                Portfolio.CashPosition = Position.NewCash(quantity, currency, contributeTime);
                SaveOrderToHistory(Portfolio.CashPosition.Order);
            }
            else
            {
                var cashOrder = CashOrder.NewContribute(Portfolio, quantity, contributeTime);
                Portfolio.CashPosition.Add(quantity);
                SaveOrderToHistory(cashOrder);
            }
        }

        public virtual void WithdrawCash(double quantity, DateTime withdrawTime)
        {
            var availableCash = CheckMarginRequirement();
            if (availableCash > 0)
            {
                var cashOrder = CashOrder.NewWithdrawal(Portfolio, quantity, withdrawTime);
                SaveOrderToHistory(cashOrder);
                Portfolio.CashPosition.Add(-quantity);
            }
        }

        protected abstract double CheckMarginRequirement();

        protected abstract void ComputeRisks(List<Position> newPositions);

        protected abstract List<Trade> ClosePositions(IEnumerable<Order> orders,
            List<Position> existingPositions, HashSet<Price> prices);

        protected abstract List<Position> ExecuteOrders(List<Order> orders, HashSet<Price> prices);

        protected abstract void CheckCrossTrades(List<Order> orders);

        protected abstract HashSet<Price> RetrievePrices();

        protected abstract void OnNewPriceData(HashSet<Price> prices);

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
        }

        private void UnsubscribeDataSource()
        {
            DataSource.NotifyNewPriceData -= OnNewPriceData;
        }

        #endregion
    }
}
