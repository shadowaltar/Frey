using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Automata.Core.Extensions;
using Automata.Strategies;
using Automata.Entities;
using Automata.Core;

namespace Automata.Mechanisms
{
    public class BackTester
    {
        public BackTester(ITradingScope testScope)
        {
            TestScope = testScope;

            DataSource = new DataSource(new HistoricalStaticDataFileAccess());
        }

        public ITradingScope TestScope { get; set; }

        public Strategy Strategy { get; set; }

        public PriceCache Data { get; protected set; }

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
                // generate entries and exits
                var exitOrders = Strategy.GenerateExits(Data, Positions);
                var entryOrders = Strategy.GenerateEntries(Data, Positions);
                CheckCrossTrades(exitOrders, entryOrders);

                // execute the exits, assuming all are executed immediately
                var closePositionTask = Task.Factory.StartNew(() => ClosePositions(exitOrders, Positions));
                closePositionTask.Wait();
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

                // execute the entries, assuming all are executed immediately
                var orderExecutionTask = Task.Factory.StartNew(() => ExecuteOrders(entryOrders));
                orderExecutionTask.Wait();
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

        private List<Trade> ClosePositions(List<Order> exitOrders, List<Position> positions)
        {
            return null;
        }

        private List<Position> ExecuteOrders(List<Order> entryOrders)
        {
            return null;
        }

        private void CheckCrossTrades(List<Order> exitOrders, List<Order> entryOrders)
        {
        }

        private void StopTrading()
        {
            Console.WriteLine(Utilities.BracketNow + " Stopping trading.");
            cancellation.Cancel();
        }

        private void ProcessPriceData(HashSet<Price> prices)
        {
            // save data to the cache.
        }
    }
}