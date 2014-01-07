using Automata.Core;
using Automata.Entities;
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
            Context.Initialize(Objects.Instance);

            // sharpe ratio ranking:
            // var tradingContext = TestSharpeRanking();

            // pair trading:
            var tradingContext = TestPairTrading();

            tradingContext.Start();
            Thread.Sleep(600000);
            tradingContext.Stop();
        }

        private static TradingContext TestSharpeRanking()
        {
            var sharpeScope = ScopeFactory.DailyAllUnitedStatesStocks(10);
            sharpeScope.Securities.RemoveAll(s => s.Code == "NASDAQ:GOOG");
            var sharpeTester = new BackTester(sharpeScope)
            {
                Strategy = new SharpeRankingStrategy(20, 3)
            };
            sharpeTester.AddCash(100000, Currency.USD, sharpeScope.Start);
            return sharpeTester;
        }

        private static TradingContext TestPairTrading()
        {
            //YahooStockPriceDownloader.Download("EWC","NYSE",Context.StaticDataFileDirectory);
            //YahooStockPriceDownloader.Download("EWA","NYSE",Context.StaticDataFileDirectory);
            var pairScope = ScopeFactory.DailyPairStocks(10, "NYSE:EWC", "NYSE:EWA");
            var tester = new BackTester(pairScope)
            {
                Strategy = new PairTradingStrategy()
            };
            tester.AddCash(100000, Currency.USD, pairScope.Start);
            return tester;
        }
    }
}
