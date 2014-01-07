using System;
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
            //DownloadFxFiles();
            // sharpe ratio ranking:
            // var tradingContext = TestSharpeRanking();

            // pair trading:
            //var tradingContext = TestPairTrading();

            // eurusd macd crossing
            var tradingContext = TestForexMACDCrossing();

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

        private static void DownloadFxFiles()
        {
            var urls = new[]
            {
                "https://s3.amazonaws.com/STPriceData/AUDCAD+ask.rar","https://s3.amazonaws.com/STPriceData/AUDCHF+ask.rar","https://s3.amazonaws.com/STPriceData/AUDJPY+ask.rar","https://s3.amazonaws.com/STPriceData/AUDNZD+ask.rar","https://s3.amazonaws.com/STPriceData/AUDUSD+ask.rar","https://s3.amazonaws.com/STPriceData/AUS200+ask.rar","https://s3.amazonaws.com/STPriceData/CADCHF+ask.rar","https://s3.amazonaws.com/STPriceData/CADJPY+ask.rar","https://s3.amazonaws.com/STPriceData/CHFJPY+ask.rar","https://s3.amazonaws.com/STPriceData/CHFNOK+ask.rar","https://s3.amazonaws.com/STPriceData/CHFSEK+ask.rar","https://s3.amazonaws.com/STPriceData/ESP35+ask.rar","https://s3.amazonaws.com/STPriceData/EURAUD+ask.rar","https://s3.amazonaws.com/STPriceData/EURCAD+ask.rar","https://s3.amazonaws.com/STPriceData/EURCHF+ask.rar","https://s3.amazonaws.com/STPriceData/EURGBP+ask.rar","https://s3.amazonaws.com/STPriceData/EURJPY+ask.rar","https://s3.amazonaws.com/STPriceData/EURNOK+ask.rar","https://s3.amazonaws.com/STPriceData/EURNZD+ask.rar","https://s3.amazonaws.com/STPriceData/EURSEK+ask.rar","https://s3.amazonaws.com/STPriceData/EURTRY+ask.rar","https://s3.amazonaws.com/STPriceData/EURUSD+ask.rar","https://s3.amazonaws.com/STPriceData/FRA40+ask.rar","https://s3.amazonaws.com/STPriceData/GBPAUD+ask.rar","https://s3.amazonaws.com/STPriceData/GBPCAD+ask.rar","https://s3.amazonaws.com/STPriceData/GBPCHF+ask.rar","https://s3.amazonaws.com/STPriceData/GBPJPY+ask.rar","https://s3.amazonaws.com/STPriceData/GBPNZD+ask.rar","https://s3.amazonaws.com/STPriceData/GBPSEK+ask.rar","https://s3.amazonaws.com/STPriceData/GBPUSD+ask.rar","https://s3.amazonaws.com/STPriceData/GER30+ask.rar","https://s3.amazonaws.com/STPriceData/HKDJPY+ask.rar","https://s3.amazonaws.com/STPriceData/HKG33+ask.rar","https://s3.amazonaws.com/STPriceData/ITA40+ask.rar","https://s3.amazonaws.com/STPriceData/JPN225+ask.rar","https://s3.amazonaws.com/STPriceData/NAS100+ask.rar","https://s3.amazonaws.com/STPriceData/NOKJPY+ask.rar","https://s3.amazonaws.com/STPriceData/NZDCAD+ask.rar","https://s3.amazonaws.com/STPriceData/NZDCHF+ask.rar","https://s3.amazonaws.com/STPriceData/NZDJPY+ask.rar","https://s3.amazonaws.com/STPriceData/NZDUSD+ask.rar","https://s3.amazonaws.com/STPriceData/SEKJPY+ask.rar","https://s3.amazonaws.com/STPriceData/SGDJPY+ask.rar","https://s3.amazonaws.com/STPriceData/SPX500+ask.rar","https://s3.amazonaws.com/STPriceData/UK100+ask.rar","https://s3.amazonaws.com/STPriceData/US30+ask.rar","https://s3.amazonaws.com/STPriceData/USDCAD+ask.rar","https://s3.amazonaws.com/STPriceData/USDCHF+ask.rar","https://s3.amazonaws.com/STPriceData/USDDKK+ask.rar","https://s3.amazonaws.com/STPriceData/USDHKD+ask.rar","https://s3.amazonaws.com/STPriceData/USDJPY+ask.rar","https://s3.amazonaws.com/STPriceData/USDMXN+ask.rar","https://s3.amazonaws.com/STPriceData/USDNOK+ask.rar","https://s3.amazonaws.com/STPriceData/USDSEK+ask.rar","https://s3.amazonaws.com/STPriceData/USDTRY+ask.rar","https://s3.amazonaws.com/STPriceData/USDZAR+ask.rar","https://s3.amazonaws.com/STPriceData/USOil+ask.rar","https://s3.amazonaws.com/STPriceData/XAGUSD+ask.rar","https://s3.amazonaws.com/STPriceData/XAUUSD+ask.rar","https://s3.amazonaws.com/STPriceData/ZARJPY+ask.rar","https://s3.amazonaws.com/STPriceData/AUDCAD+bid.rar","https://s3.amazonaws.com/STPriceData/AUDCHF+bid.rar","https://s3.amazonaws.com/STPriceData/AUDJPY+bid.rar","https://s3.amazonaws.com/STPriceData/AUDNZD+bid.rar","https://s3.amazonaws.com/STPriceData/AUDUSD+bid.rar","https://s3.amazonaws.com/STPriceData/AUS200+bid.rar","https://s3.amazonaws.com/STPriceData/CADCHF+bid.rar","https://s3.amazonaws.com/STPriceData/CADJPY+bid.rar","https://s3.amazonaws.com/STPriceData/CHFJPY+bid.rar","https://s3.amazonaws.com/STPriceData/CHFNOK+bid.rar","https://s3.amazonaws.com/STPriceData/CHFSEK+bid.rar","https://s3.amazonaws.com/STPriceData/ESP35+bid.rar","https://s3.amazonaws.com/STPriceData/EURAUD+bid.rar","https://s3.amazonaws.com/STPriceData/EURCAD+bid.rar","https://s3.amazonaws.com/STPriceData/EURCHF+bid.rar","https://s3.amazonaws.com/STPriceData/EURGBP+bid.rar","https://s3.amazonaws.com/STPriceData/EURJPY+bid.rar","https://s3.amazonaws.com/STPriceData/EURNOK+bid.rar","https://s3.amazonaws.com/STPriceData/EURNZD+bid.rar","https://s3.amazonaws.com/STPriceData/EURSEK+bid.rar","https://s3.amazonaws.com/STPriceData/EURTRY+bid.rar","https://s3.amazonaws.com/STPriceData/EURUSD+bid.rar","https://s3.amazonaws.com/STPriceData/FRA40+bid.rar","https://s3.amazonaws.com/STPriceData/GBPAUD+bid.rar","https://s3.amazonaws.com/STPriceData/GBPCAD+bid.rar","https://s3.amazonaws.com/STPriceData/GBPCHF+bid.rar","https://s3.amazonaws.com/STPriceData/GBPJPY+bid.rar","https://s3.amazonaws.com/STPriceData/GBPNZD+bid.rar","https://s3.amazonaws.com/STPriceData/GBPSEK+bid.rar","https://s3.amazonaws.com/STPriceData/GBPUSD+bid.rar","https://s3.amazonaws.com/STPriceData/GER30+bid.rar","https://s3.amazonaws.com/STPriceData/HKDJPY+bid.rar","https://s3.amazonaws.com/STPriceData/HKG33+bid.rar","https://s3.amazonaws.com/STPriceData/ITA40+bid.rar","https://s3.amazonaws.com/STPriceData/JPN225+bid.rar","https://s3.amazonaws.com/STPriceData/NAS100+bid.rar","https://s3.amazonaws.com/STPriceData/NOKJPY+bid.rar","https://s3.amazonaws.com/STPriceData/NZDCAD+bid.rar","https://s3.amazonaws.com/STPriceData/NZDCHF+bid.rar","https://s3.amazonaws.com/STPriceData/NZDJPY+bid.rar","https://s3.amazonaws.com/STPriceData/NZDUSD+bid.rar","https://s3.amazonaws.com/STPriceData/SEKJPY+bid.rar","https://s3.amazonaws.com/STPriceData/SGDJPY+bid.rar","https://s3.amazonaws.com/STPriceData/SPX500+bid.rar","https://s3.amazonaws.com/STPriceData/UK100+bid.rar","https://s3.amazonaws.com/STPriceData/US30+bid.rar","https://s3.amazonaws.com/STPriceData/USDCAD+bid.rar","https://s3.amazonaws.com/STPriceData/USDCHF+bid.rar","https://s3.amazonaws.com/STPriceData/USDDKK+bid.rar","https://s3.amazonaws.com/STPriceData/USDHKD+bid.rar","https://s3.amazonaws.com/STPriceData/USDJPY+bid.rar","https://s3.amazonaws.com/STPriceData/USDMXN+bid.rar","https://s3.amazonaws.com/STPriceData/USDNOK+bid.rar","https://s3.amazonaws.com/STPriceData/USDSEK+bid.rar","https://s3.amazonaws.com/STPriceData/USDSGD+bid.rar","https://s3.amazonaws.com/STPriceData/USDTRY+bid.rar","https://s3.amazonaws.com/STPriceData/USDZAR+bid.rar","https://s3.amazonaws.com/STPriceData/USOil+bid.rar","https://s3.amazonaws.com/STPriceData/XAGUSD+bid.rar","https://s3.amazonaws.com/STPriceData/XAUUSD+bid.rar","https://s3.amazonaws.com/STPriceData/ZARJPY+bid.rar"
            };
            MiscDownloader.Download(urls);
        }

        private static TradingContext TestForexMACDCrossing()
        {
            var currencyScope = ScopeFactory.DailyForex(1, "EURUSD");
            var tester = new BackTester(currencyScope)
            {
                Strategy = new ForexMACDCrossingStrategy(),
            };
            tester.AddCash(10000, Currency.USD, currencyScope.Start);
            return tester;
        }
    }
}
