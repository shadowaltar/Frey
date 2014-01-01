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
            var testScope = ScopeFactory.DailyAllUnitedStatesStocks(10);
            testScope.Securities.RemoveAll(s => s.Code == "NASDAQ:GOOG");
            var tester = new BackTester(testScope)
            {
                Strategy = new SharpeRankingStrategy(20, 1)
            };
            tester.AddCash(100000, Currency.USD, testScope.Start);
            tester.Start();

            Thread.Sleep(600000);

            tester.Stop();
        }
    }
}
