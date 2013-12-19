using Automata.Mechanisms;
using System.Threading;
namespace Automata
{
    public class Program
    {
        static void Main(string[] args)
        {
            var testScope = new TestScope();
            var tester = new BackTester(testScope);

            tester.Start();

            //while(true)
            Thread.Sleep(10000);

            tester.Stop();
        }
    }
}
