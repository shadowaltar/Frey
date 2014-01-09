using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using Automata.Core;
using Automata.Core.Exceptions;
using Automata.Core.Extensions;
using Automata.Entities;
using Automata.Mechanisms;
using Automata.Mechanisms.Utils;
using Automata.Quantitatives.Indicators;

namespace Automata.Strategies
{
    public class ForexMACDCrossingStrategy : Strategy
    {
        public override bool IsTimeToStop { get; protected set; }

        public Dictionary<Security, List<Price>> PriceHistory { get; private set; }

        private MACD macd;

        private int tickCounter = 0;
        private DateTime lastPriceTime = DateTime.MinValue;

        public override void Initialize()
        {
            macd = Indicators[0] as MACD;
            if (macd == null)
                throw new InvalidStrategyBehaviorException();
        }

        private HashSet<Price> lastPrices = new HashSet<Price>();

        public override List<Order> GenerateOrders(HashSet<Price> prices, Portfolio portfolio, DateTime orderTime)
        {
            if (prices.IsNullOrEmpty())
                return null;

            tickCounter++;

            foreach (var price in prices)
            {
                macd.Compute(price);

                if (tickCounter < 50)
                    continue;

                var macdHist = macd.HistogramValues.LastOrDefault();
                var macdSig = macd.SignalValues.LastOrDefault();
                var macdBody = macd.MACDValues.LastOrDefault();

                Console.WriteLine("MACD values: HIST:{0},SIG:{1},MACD:{2}", 
                    macdHist.Value.ToString("#0.########"),
                    macdSig.Value.ToString("#0.########"),
                    macdBody.Value.ToString("#0.########"));
            }

            lastPriceTime = orderTime;
            return null;
        }
    }
}