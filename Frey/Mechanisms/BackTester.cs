using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Automata.Core.Extensions;
using Automata.Strategies;
using Automata.Entities;
using Automata.Core;
using System.Collections.Concurrent;
using Automata.Core.Exceptions;

namespace Automata.Mechanisms
{
    public class BackTester : TradingContext
    {
        public BackTester(ITradingScope testScope)
        {
            TradingScope = testScope;
            PriceData = new ConcurrentQueue<HashSet<Price>>();
            DataSource = new DataSource(new HistoricalStaticDataFileAccess());
            Equity = 100000;
            initEquity = Equity;
        }

        public ITradingScope TradingScope { get; set; }

        public Strategy Strategy { get; set; }

        public ConcurrentQueue<HashSet<Price>> PriceData { get; protected set; }

        public double Equity { get; set; }
        private double initEquity;

        public List<Position> Positions { get { return positions; } }
        public List<Order> ExecutedOrders { get { return executedOrders; } }
        public List<Trade> Trades { get { return trades; } }

        public DataSource DataSource { get; set; }

        private CancellationTokenSource cancellation;
        private readonly List<Order> executedOrders = new List<Order>();
        private readonly List<Position> positions = new List<Position>();
        private readonly List<Trade> trades = new List<Trade>();

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
            StopTrading();
            DataSource.Stop();
            UnsubscribeDataSource();
        }

        private void SubscribeDataSource()
        {
            DataSource.NotifyNewPriceData += ProcessPriceData;
        }

        private void UnsubscribeDataSource()
        {
            DataSource.NotifyNewPriceData -= ProcessPriceData;
        }

        #endregion

        protected override void Trade()
        {
            while (!Strategy.IsTimeToStop && !cancellation.Token.IsCancellationRequested)
            {
                // dequeue one day only
                HashSet<Price> prices = null;
                if (!PriceData.TryDequeue(out prices))
                {
                    continue;
                }

                // generate entries and exits
                var orders = Strategy.GenerateOrders(prices, Positions);
                CheckCrossTrades(orders);

                // execute the exits, assuming all are executed immediately
                var closePositionTask = Task.Factory.StartNew(() => ClosePositions(orders, Positions, prices));

                // execute the entries, assuming all are executed immediately
                var orderExecutionTask = Task.Factory.StartNew(() => ExecuteOrders(orders, prices));

                // wait for all the orders to be filled.
                closePositionTask.Wait();
                orderExecutionTask.Wait();

                var newClosedTrades = closePositionTask.Result;
                if (!newClosedTrades.IsNullOrEmpty())
                {
                    foreach (var trade in newClosedTrades)
                    {
                        Positions.Remove(trade.Position);
                    }
                    ComputeProfits(newClosedTrades);
                }

                var newPositions = orderExecutionTask.Result;
                if (!newPositions.IsNullOrEmpty())
                {
                    Positions.AddRange(newPositions);
                    ComputeRisks(newPositions);
                }
            }
            Console.WriteLine(Utilities.BracketNow + " Stopped trading.");
            Console.WriteLine(Utilities.BracketNow + " Equity: " + Equity);
            Console.WriteLine(Utilities.BracketNow + " Return: " + (Equity / initEquity - 1));
            TradeHistory.TrimExcess();
            Console.WriteLine(TradeHistory);
        }

        private readonly object equitySyncRoot = new object();
        private void ComputeProfits(List<Trade> newClosedTrades)
        {
            lock (equitySyncRoot)
            {
                foreach (var trade in newClosedTrades)
                {
                    Equity += trade.Profit;
                }
            }
        }

        private void ComputeRisks(List<Position> newPositions)
        {
        }

        private List<Trade> ClosePositions(IEnumerable<Order> orders,
            List<Position> positions, HashSet<Price> prices)
        {
            if (positions.IsNullOrEmpty() || orders.IsNullOrEmpty())
                return null;
            var exitOrders = orders.Where(o => o.IsClosingPosition);

            // save exit orders to history
            if (!exitOrders.IsNullOrEmpty())
                Task.Factory.StartNew(() => SaveOrdersToHistory(exitOrders));

            var trades = new List<Trade>();
            foreach (var order in exitOrders)
            {
                var position = positions.FirstOrDefault(p => p.Security == order.Security);
                var price = prices.FirstOrDefault(p => p.Security == position.Security);
                if (position == null || price == null)
                {
                    throw new InvalidStrategyBehaviorException();
                }
                var trade = new Trade();
                trade.Position = position;
                trade.ExitOrder = order;
                trade.ExecutionTime = price.Time;
                trade.ActuralExitPrice = price.AdjustedClose;
                trade.Return = price.AdjustedClose - position.ActualEntryPrice;
                trade.Profit = trade.Return * position.ActualQuantity;
                trades.Add(trade);
                Console.WriteLine(Utilities.BracketNow + " Close Position: " + trade);
            }

            // save trades to history
            if (!trades.IsNullOrEmpty())
                Task.Factory.StartNew(() => SaveTradesToHistory(trades));

            return trades;
        }

        private List<Position> ExecuteOrders(List<Order> orders, HashSet<Price> prices)
        {
            if (orders.IsNullOrEmpty())
                return null;
            var entryOrders = orders.Where(o => !o.IsClosingPosition);

            // save entry orders to history
            if (!entryOrders.IsNullOrEmpty())
                Task.Factory.StartNew(() => SaveOrdersToHistory(entryOrders));

            var positions = new List<Position>();
            foreach (var order in entryOrders)
            {
                var price = prices.FirstOrDefault(p => p.Security == order.Security);
                if (price == null)
                {
                    throw new InvalidStrategyBehaviorException();
                }
                order.Price = price.AdjustedClose;

                // not handling the "Side.Hold" at the moment.
                if (order.Side == Side.Hold)
                {
                    continue;
                }

                var position = new Position(order, order.Price, order.Quantity, price.Time);
                positions.Add(position);
                Console.WriteLine(Utilities.BracketNow + " New Position: " + position);
            }
            return positions;
        }

        private void CheckCrossTrades(List<Order> orders)
        {
            foreach (var order in orders)
            {
                var orderCount = orders.Count(o => o.Security == order.Security);
                if (orderCount > 1)
                {
                    throw new InvalidStrategyBehaviorException();
                }
            }
        }

        private void StopTrading()
        {
            Console.WriteLine(Utilities.BracketNow + " Stopping trading.");
            cancellation.Cancel();
        }

        /// <summary>
        /// Process the prices data come from the data source.
        /// Here we assumed that the data are causal.
        /// </summary>
        /// <param name="prices"></param>
        private void ProcessPriceData(HashSet<Price> prices)
        {
            PriceData.Enqueue(prices);
            Task.Factory.StartNew(() => SavePricesToHistory(prices));
        }
    }
}