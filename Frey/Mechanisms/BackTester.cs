using System.Diagnostics;
using System.IO;
using Automata.Core;
using Automata.Core.Exceptions;
using Automata.Core.Extensions;
using Automata.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Automata.Mechanisms
{
    public class BackTester : TradingContext
    {
        private CsvFileAccess portfolioDataWriter;

        public BackTester(ITradingScope testScope)
        {
            TradingScope = testScope;
            PriceData = new ConcurrentQueue<HashSet<Price>>();
            DataSource = new DataSource(new HistoricalStaticDataFileAccess());
        }

        public ConcurrentQueue<HashSet<Price>> PriceData { get; protected set; }

        protected override double CheckMarginRequirement()
        {
            return Portfolio.CashPosition.Value;
        }

        protected override void ComputeRisks(IEnumerable<Position> newPositions)
        {
        }

        protected override List<Trade> ClosePositions(IEnumerable<Order> orders,
            List<Position> existingPositions, HashSet<Price> prices)
        {
            if (existingPositions.IsNullOrEmpty())
                return null;

            var results = new List<Trade>();
            foreach (var order in orders)
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
                Utilities.WriteTimedLine("Close Position: " + trade);
            }

            if (results.Count > 0)
                SaveTradesToHistory(results); // save trades to history

            return results;
        }

        protected override List<Position> ExecuteOrders(List<Order> orders, HashSet<Price> prices)
        {
            if (orders.IsNullOrEmpty())
                return null;

            var results = new List<Position>();
            foreach (var order in orders)
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

        protected override void CheckCrossTrades(List<Order> orders)
        {
            foreach (var order in orders)
            {
                if (orders.Any(o => o.Side != order.Side && o.Security == order.Security))
                {
                    throw new InvalidStrategyBehaviorException();
                }
            }
        }

        /// <summary>
        /// Process the prices data come from the data source.
        /// Here we assumed that the data are causal.
        /// </summary>
        /// <param name="prices"></param>
        protected override void OnNewPriceData(HashSet<Price> prices)
        {
            PriceData.Enqueue(prices);
            Task.Factory.StartNew(() => SavePricesToHistory(prices));
        }

        protected override void OnDataStatusChanged(DataStatus status)
        {
            DataStatus = status;
        }

        protected override HashSet<Price> RetrievePrices()
        {
            HashSet<Price> prices;
            if (!PriceData.TryDequeue(out prices))
            {
                return null;
            }
            return prices;
        }

        protected override void BeforeTrading()
        {
            Utilities.WriteTimedLine("BackTester Trading Starts.");
            portfolioDataWriter = CsvFileAccess.GetWriter(Path.Combine(Context.LocalTempFileDirectory, "Test.csv"));

        }

        protected override void AfterTrading()
        {
            TradeHistory.TrimExcess();

            // reporting #1
            if (portfolioDataWriter != null)
                portfolioDataWriter.Dispose();

            // reporting #2, for trade records.
            Console.WriteLine();
            var equity = InitEquity;
            var reportFileName = ("Result_" + Utilities.Now + ".csv").Replace(":", string.Empty);
            using (var sw = new StreamWriter(new FileStream(reportFileName, FileMode.CreateNew)))
            {
                foreach (var trade in TradeHistory)
                {
                    Console.WriteLine(trade);
                    sw.WriteLine(trade.PrintCSVFriendly(equity));
                    equity += trade.Profit;
                }
            }
            Utilities.WriteTimedLine("Stopped trading.");
            Utilities.WriteTimedLine("Period From {0} To {1}", TradingScope.Start, TradingScope.End);
            Utilities.WriteTimedLine("Equity: " + Portfolio.CashPosition.Value);
            Utilities.WriteTimedLine("Return: " + (Portfolio.CashPosition.Value / InitEquity - 1));
            Process.Start(reportFileName);
        }
    }
}