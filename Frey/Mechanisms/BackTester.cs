using Automata.Core;
using Automata.Core.Exceptions;
using Automata.Core.Extensions;
using Automata.Entities;
using Automata.Strategies;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

        private readonly List<Position> positions = new List<Position>();
        public List<Position> Positions { get { return positions; } }

        public DataSource DataSource { get; set; }

        private CancellationTokenSource cancellation;

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
                HashSet<Price> prices;
                if (!PriceData.TryDequeue(out prices))
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
            List<Position> existingPositions, HashSet<Price> prices)
        {
            if (existingPositions.IsNullOrEmpty())
                return null;

            var exitOrders = orders.Where(o => o.IsClosingPosition).ToList();
            if (exitOrders.Count == 0)
                return null;

            // save exit orders to history
            SaveOrdersToHistory(exitOrders);

            var results = new List<Trade>();
            foreach (var order in exitOrders)
            {
                var position = existingPositions.FirstOrDefault(p => p.Security == order.Security);
                if (position == null)
                    throw new InvalidStrategyBehaviorException();

                var price = prices.FirstOrDefault(p => p.Security == position.Security);
                if (price == null)
                    throw new InvalidStrategyBehaviorException();

                // the market price of order to close the position, is the stock's close price of the day
                order.Price = price.Close;
                var trade = new Trade
                {
                    Position = position,
                    ExitOrder = order,
                    ExecutionTime = price.Time,
                    ActuralExitPrice = order.Price,
                    Return = order.Price - position.ActualEntryPrice
                };
                trade.Profit = trade.Return * position.ActualQuantity;
                results.Add(trade);
                Console.WriteLine(Utilities.BracketNow + " Close Position: " + trade);
            }

            if (results.Count > 0)
                SaveTradesToHistory(results); // save trades to history

            return results;
        }

        private List<Position> ExecuteOrders(List<Order> orders, HashSet<Price> prices)
        {
            if (orders.IsNullOrEmpty())
                return null;

            var entryOrders = orders.Where(o => !o.IsClosingPosition).ToList();
            if (entryOrders.Count == 0)
                return null;

            // save entry orders to history
            SaveOrdersToHistory(entryOrders);

            var results = new List<Position>();
            foreach (var order in entryOrders)
            {
                var price = prices.FirstOrDefault(p => p.Security == order.Security);
                if (price == null)
                {
                    throw new InvalidStrategyBehaviorException();
                }

                // not handling the "Side.Hold" at the moment.
                if (order.Side == Side.Hold)
                {
                    continue;
                }

                var position = new Position(order, order.Price, order.Quantity, price.Time);
                results.Add(position);
                Console.WriteLine(Utilities.BracketNow + " New Position: " + position);
            }
            return results;
        }

        private void CheckCrossTrades(List<Order> orders)
        {
            foreach (var order in orders)
            {
                if (orders.Count(o => o.Security == order.Security) > 1)
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