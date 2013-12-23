using Automata.Mechanisms;
using Automata.Mechanisms.Factories;
using System.Threading;
using Automata.Strategies;

namespace Automata
{
    public class Program
    {
        static void Main(string[] args)
        {
            var testScope = ScopeFactory.DailyAllUnitedStatesStocks(10);
            var tester = new BackTester(testScope)
            {
                Strategy = new SharpeRankingStrategy()
            };
            tester.Start();

            //while(true)
            Thread.Sleep(600000);

            tester.Stop();
        }
    }
}
