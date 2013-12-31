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

        private readonly List<Position> positions = new List<Position>();
        public List<Position> Positions { get { return positions; } }

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

        public double Equity { get; set; }

        protected double initEquity;

        public DataSource DataSource { get; set; }

        public Strategy Strategy { get; set; }

        public ITradingScope TradingScope { get; set; }

        private CancellationTokenSource cancellation;

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

                // generate entries and exits
                var orders = Strategy.GenerateOrders(prices, Positions);
                CheckCrossTrades(orders);

                Task<List<Trade>> closePositionTask = null;
                Task<List<Position>> orderExecutionTask = null;
                if (orders.Any())
                {
                    // assuming all are executed immediately
                    closePositionTask = Task.Factory.StartNew(() => ClosePositions(orders, Positions, prices));
                    orderExecutionTask = Task.Factory.StartNew(() => ExecuteOrders(orders, prices));

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
                            Positions.Remove(trade.Position);
                        }
                        ComputeProfits(newClosedTrades);
                    }
                }

                if (orderExecutionTask != null)
                {
                    var newPositions = orderExecutionTask.Result;
                    if (!newPositions.IsNullOrEmpty())
                    {
                        Positions.AddRange(newPositions);
                        ComputeRisks(newPositions);
                    }
                }
            }
            Console.WriteLine(Utilities.BracketNow + " Stopped trading.");
            Console.WriteLine(Utilities.BracketNow + " Equity: " + Equity);
            Console.WriteLine(Utilities.BracketNow + " Return: " + (Equity / initEquity - 1));
            TradeHistory.TrimExcess();
            Console.WriteLine(TradeHistory);
        }

        protected abstract void ComputeProfits(List<Trade> newClosedTrades);

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
