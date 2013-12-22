using Automata.Mechanisms;
using Automata.Mechanisms.Factories;
using System.Threading;
namespace Automata
{
    public class Program
    {
        static void Main(string[] args)
        {
            var testScope = ScopeFactory.DailyAllUnitedStatesStocks(10);

            var tester = new BackTester(testScope);

            tester.Start();

            //while(true)
            Thread.Sleep(20000);

            tester.Stop();
        }
    }
}
