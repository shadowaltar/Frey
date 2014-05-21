using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Automata.Core;
using Automata.Quantitatives;

namespace Automata.Dashboard
{
    public class MainViewModel : ViewModelBase, IMainViewModel
    {
        public MainViewModel()
        {
            Volatility = 5.292302 / 100;
            Iterations = 500000;
        }

        private readonly Dictionary<double, double> resultChangingS0 = new Dictionary<double, double>();
        private readonly Dictionary<double, double> resultChangingVol = new Dictionary<double, double>();

        private double riskFreeRate = 0.005809777;

        private double volatility;
        public double Volatility { get { return volatility; } set { SetNotify(ref volatility, value); } }

        private long iterations;
        public long Iterations { get { return iterations; } set { SetNotify(ref iterations, value); } }

        public void CalculateWithChangingStockPrice()
        {
            var steps = 250;
            var tenor = 1d;
            var priceVol = Volatility;
            var strike = 100d;

            var capStrike = 130d;
            var barrier = 90d;
            var floor = 90d;

            var deltaS = 0.1;

            var trunkCount = 8;

            var startingS0 = 80;
            var endingS0 = 130;

            double timeUsed;
            using (var rt = ReportTime.StartWithMessage("TOTAL TIME ELAPSED {0} (Delta & Gamma)."))
            {
                for (int s0 = startingS0; s0 <= endingS0; s0++)
                {
                    var s0s = new List<double> { s0, s0 + deltaS, s0 + 2 * deltaS };

                    using (ReportTime.StartWithMessage("{0} -------- stock " + s0))
                    {
                        foreach (var s in s0s)
                        {
                            var trunks = new double[trunkCount];
                            var iteration = Iterations / trunkCount;

                            Parallel.For(0, trunkCount, j =>
                            {
                                var mc = new MonteCarloGenerator();
                                var result = 0d;
                                for (int i = 0; i < iteration; i++)
                                {
                                    result += CalculateTwinWinCollar(s, strike, capStrike, barrier, floor,
                                        riskFreeRate, priceVol, tenor, steps, mc);
                                }
                                trunks[j] = result / iteration;
                            });

                            resultChangingS0[s] = trunks.Average();
                        }
                    }
                }

                timeUsed = rt.Elapsed;
            }

            Report("ChangeStockPrice", resultChangingS0, timeUsed);
        }

        public void CalculateWithChangingVolatility()
        {
            var s0 = 100d;
            var steps = 250;
            var tenor = 1d;

            var strike = 100d;

            var capStrike = 130d;
            var barrier = 90d;
            var floor = 90d;

            var trunkCount = 8;

            var vols = new List<double> { 0.03,
                    3.388672/100, 3.388672/100+0.01,
                    0.04,
                    5.29230181692926/100, 5.29230181692926/100+0.01,
                    0.06, 0.07, 0.08, 0.09, 0.1, 0.11, 0.12, 0.13, 0.14, 0.15, 0.16, 0.17, 0.18, 0.19,
                    19.288638/100, 19.288638/100+0.01,
                    0.2, 0.21, 0.22, 0.23, 0.24, 0.25, 0.26, 0.27, 0.28, 0.29, 0.3, 0.31, 0.32, 0.33, 0.34, 0.35
                };

            double timeUsed;
            using (var rt = ReportTime.StartWithMessage("TOTAL TIME ELAPSED {0} (Vega)."))
            {
                foreach (var vol in vols)
                {
                    using (ReportTime.StartWithMessage("{0} -------- vol " + vol))
                    {
                        var trunks = new double[trunkCount];
                        var iteration = Iterations / trunkCount;

                        double vol1 = vol;
                        Parallel.For(0, trunkCount, j =>
                        {
                            var mc = new MonteCarloGenerator();
                            var result = 0d;
                            for (int i = 0; i < iteration; i++)
                            {
                                result += CalculateTwinWinCollar(s0, strike, capStrike, barrier, floor,
                                    riskFreeRate, vol1, tenor, steps, mc);
                            }
                            trunks[j] = result / iteration;
                        });

                        resultChangingVol[vol] = trunks.Average();
                    }
                }
                timeUsed = rt.Elapsed;
            }

            Report("ChangeVolatility", resultChangingVol, timeUsed);
        }

        internal double CalculateTwinWinCollar(double s0,
            double strike, double capStrike, double knockOutBarrier,
            double knockedOutFloor,
            double rf,
            double priceStdev, double tenor, int steps,
            MonteCarloGenerator mc)
        {
            var st = s0;
            var dt = tenor / steps;

            var stAnti = s0;
            var knockedOut1 = false;
            var knockedOut2 = false;
            for (int i = 0; i < steps; i++)
            {
                double drift;
                mc.NextPriceWithAntithetic(st, stAnti, rf, priceStdev, dt, out drift, out st, out stAnti);
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
                * Math.Exp(-rf * tenor);
        }

        private static void Report(string fileNameHead, Dictionary<double, double> data, double timeUsed)
        {
            var formattedResults = data.OrderBy(rv => rv.Key)
                .Select(rv => rv.Key + "," + rv.Value + Environment.NewLine);

            File.WriteAllLines(fileNameHead + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv"
                , formattedResults);

            MessageBox.Show("DONE! Time used: " + timeUsed);
        }

        //public double CalculateCall(double s0, double strike, double riskFreeRate,
        //    double priceStdev, double tenor, int steps)
        //{
        //    var st = s0;
        //    var dt = tenor / steps;

        //    var stAnti = s0;
        //    //var prices = new double[steps];
        //    //var anthitheticPrices = new double[steps];
        //    //var drifts = new double[steps];

        //    double drift;
        //    for (int i = 0; i < steps; i++)
        //    {
        //        mc.NextPriceWithAntithetic(st, stAnti, riskFreeRate, priceStdev, dt, out drift, out st, out stAnti);
        //        //drifts[i] = drift;
        //        //prices[i] = st;
        //        //anthitheticPrices[i] = stAnti;
        //    }

        //    return (Math.Max(0, st - strike) + Math.Max(0, stAnti - strike)) / 2
        //        * Math.Exp(-riskFreeRate * tenor);
        //}
    }
}