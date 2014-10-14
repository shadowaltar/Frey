using System;
using Algorithms.Apps.TexasHoldem;

namespace Algorithms.Tests
{
    public class Program
    {
        [STAThread]
        public static void Main(params string[] args)
        {
            GameTest.Start();
            Console.ReadLine();
        }
    }
}