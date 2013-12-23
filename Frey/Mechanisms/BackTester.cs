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

namespace Automata.Mechanisms
{
    public class BackTester
    {
        public BackTester(ITradingScope testScope)
        {
            TestScope = testScope;
            PriceData = new ConcurrentQueue<HashSet<Price>>();
            DataSource = new DataSource(new HistoricalStaticDataFileAccess());
        }

        public ITradingScope TestScope { get; set; }

        public Strategy Strategy { get; set; }

        public ConcurrentQueue<HashSet<Price>> PriceData { get; protected set; }

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
            Strategy.TradingScope = TestScope;
            Strategy.Initialize();

            DataSource.TradingScope = TestScope;
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

        protected void Trade()
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
                var closePositionTask = Task.Factory.StartNew(() => ClosePositions(orders, Positions));

                // execute the entries, assuming all are executed immediately
                var orderExecutionTask = Task.Factory.StartNew(() => ExecuteOrders(orders));

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
                    Trades.AddRange(newClosedTrades);
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
        }

        private void ComputeRisks(List<Position> newPositions)
        {
        }

        private void ComputeProfits(List<Trade> trades)
        {
        }

        private List<Trade> ClosePositions(IEnumerable<Order> orders, List<Position> positions)
        {
            if (positions.IsNullOrEmpty() || orders.IsNullOrEmpty())
                return null;
            var exitOrders = orders.Where(o => o.IsClosingPosition);
            throw new NotImplementedException();
        }

        private List<Position> ExecuteOrders(IEnumerable<Order> orders)
        {
            if (orders.IsNullOrEmpty())
                return null;
            var entryOrders = orders.Where(o => !o.IsClosingPosition);
            var positions = new List<Position>();
            return positions;
        }

        private void CheckCrossTrades(List<Order> orders)
        {
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
        private void ProcessPriceData(HashSet<Price> price)
        {
            PriceData.Enqueue(price);
        }
    }
}