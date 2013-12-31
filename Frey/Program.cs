﻿using Automata.Core;
using Automata.Mechanisms;
using Automata.Mechanisms.Factories;
using Automata.Strategies;
using System.Threading;

namespace Automata
{
    public class Program
    {
        static void Main(string[] args)
        {
            var x = YahooStockPriceDownloader.GetUrl("SLY");
            YahooStockPriceDownloader.Download("SLY", "NYSE", Context.StaticDataFileDirectory);

            var testScope = ScopeFactory.DailyAllUnitedStatesStocks(10);
            var tester = new BackTester(testScope)
            {
                Strategy = new SharpeRankingStrategy(20, 1)
            };
            tester.Start();

            Thread.Sleep(600000);

            tester.Stop();
        }
    }
}
