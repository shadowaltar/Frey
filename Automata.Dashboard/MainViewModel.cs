using System;
using System.Linq;
using System.Threading.Tasks;
using Automata.Core;
using Automata.Quantitatives;

namespace Automata.Dashboard
{
    public class MainViewModel : IMainViewModel
    {
        private MonteCarloGenerator mc = new MonteCarloGenerator();

        public void Go()
        {
            var s0s = new[] { 88, 90, 92, 94, 96, 98, 100, 102, 104, 106, 108, 110, 112, 114, 116, 118, 120 };
            foreach (var s0 in s0s)
            {
                GoMultiple(s0);
            }
        }

        public void GoMultiple(double s0)
        {
            var loops = 100000;

            //   var s0 = 115d;
            var steps = 250;
            var tenor = 1d;
            var rf = 0.0112;
            var priceVol = 0.0551084226;

            var strike = 100d;

            var capStrike = 130d;
            var barrier = 90d;
            var floor = 90d;

            var value1 = 0d;
            using (ReportTime.StartWithMessage("-------- s0:" + s0))
            {
                for (int i = 0; i < loops; i++)
                {
                    //sum += CalculateCall(s0, strike, rf, priceVol, tenor, steps);
                    value1 += CalculateTwinWinCollar(s0, strike, capStrike, barrier, floor,
                        rf, priceVol, tenor, steps);
                }
                value1 /= loops;
                Console.WriteLine("VALUE1: " + value1);

                s0 += 0.1;
                var value2 = 0d;

                for (int i = 0; i < loops; i++)
                {
                    //sum += CalculateCall(s0, strike, rf, priceVol, tenor, steps);
                    value2 += CalculateTwinWinCollar(s0, strike, capStrike, barrier, floor,
                        rf, priceVol, tenor, steps);
                }
                value2 /= loops;
                Console.WriteLine("VALUE2: " + value2);

                s0 += 0.1;
                var value3 = 0d;

                for (int i = 0; i < loops; i++)
                {
                    //sum += CalculateCall(s0, strike, rf, priceVol, tenor, steps);
                    value3 += CalculateTwinWinCollar(s0, strike, capStrike, barrier, floor,
                        rf, priceVol, tenor, steps);
                }
                value3 /= loops;
                Console.WriteLine("VALUE3: " + value3);


                var delta1 = (value2 - value1) / 0.1;
                var delta2 = (value3 - value2) / 0.1;
                var gamma = (delta2 - delta1) / 0.1;

                Console.WriteLine("DELTA1: " + delta1);
                Console.WriteLine("DELTA2: " + delta2);
                Console.WriteLine("GAMMA1: " + gamma);
            }

            //var fourTrunks = new double[4];
            //var subLoops = loops / 4;
            //Parallel.ForEach(fourTrunks, trunk =>
            //{
            //    var newMc = new MonteCarloGenerator();
            //    var s = 0d;
            //    for (int i = 0; i < subLoops; i++)
            //    {
            //        //sum += CalculateCall(s0, strike, rf, priceVol, tenor, steps);
            //        s += CalculateTwinWinCollar(s0, strike, capStrike, barrier, floor,
            //            rf, priceVol, tenor, steps, newMc);
            //    }
            //    s /= subLoops;
            //    Console.WriteLine(s);
            //});
        }

        public double CalculateTwinWinCollar(double s0,
            double strike, double capStrike, double knockOutBarrier,
            double knockedOutFloor,
            double riskFreeRate,
            double priceStdev, double tenor, int steps)
        {
            var st = s0;
            var dt = tenor / steps;

            var stAnti = s0;
            var knockedOut1 = false;
            var knockedOut2 = false;
            for (int i = 0; i < steps; i++)
            {
                double drift;
                mc.NextPriceWithAntithetic(st, stAnti, riskFreeRate, priceStdev, dt, out drift, out st, out stAnti);
                if (!knockedOut1 && st <= knockOutBarrier)
                {
                    knockedOut1 = true;
                }
                if (!knockedOut2 && stAnti <= knockOutBarrier)
                {
                    knockedOut2 = true;
                }
            }

            var price1 = knockedOut1
                ? Math.Max(knockedOutFloor, st) - strike
                : Math.Min(Math.Abs(st - strike), capStrike - strike);
            var price2 = knockedOut2
                ? Math.Max(knockedOutFloor, stAnti) - strike
                : Math.Min(Math.Abs(stAnti - strike), capStrike - strike);

            return (price1 + price2) / 2
                * Math.Exp(-riskFreeRate * tenor);
        }

        public double CalculateCall(double s0, double strike, double riskFreeRate,
            double priceStdev, double tenor, int steps)
        {
            var st = s0;
            var dt = tenor / steps;

            var stAnti = s0;
            //var prices = new double[steps];
            //var anthitheticPrices = new double[steps];
            //var drifts = new double[steps];

            double drift;
            for (int i = 0; i < steps; i++)
            {
                mc.NextPriceWithAntithetic(st, stAnti, riskFreeRate, priceStdev, dt, out drift, out st, out stAnti);
                //drifts[i] = drift;
                //prices[i] = st;
                //anthitheticPrices[i] = stAnti;
            }

            return (Math.Max(0, st - strike) + Math.Max(0, stAnti - strike)) / 2
                * Math.Exp(-riskFreeRate * tenor);
        }
    }
}