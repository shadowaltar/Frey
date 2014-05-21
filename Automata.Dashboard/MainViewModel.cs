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
            RiskFreeRate = 0.0364927 / 100;
            RemainingSteps = 250; // = 1year
        }

        private readonly Dictionary<double, double> resultChangingUnderlying = new Dictionary<double, double>();
        private readonly Dictionary<double, double> resultChangingVol = new Dictionary<double, double>();
        private readonly Dictionary<double, double> resultChangingTime = new Dictionary<double, double>();
        private readonly Dictionary<double, double> resultChangingRate = new Dictionary<double, double>();


        private double riskFreeRate;
        public double RiskFreeRate { get { return riskFreeRate; } set { SetNotify(ref riskFreeRate, value); } }

        private double volatility;
        public double Volatility { get { return volatility; } set { SetNotify(ref volatility, value); } }

        private long iterations;
        public long Iterations { get { return iterations; } set { SetNotify(ref iterations, value); } }

        private int remainingSteps;
        public int RemainingSteps { get { return remainingSteps; } set { SetNotify(ref remainingSteps, value); } }

        public void CalculateAll()
        {
            CalculateWithChangingInterestRate();
            CalculateWithChangingStockPrice();
            CalculateWithChangingTime();
            CalculateWithChangingVolatility();
        }

        public void CalculateWithChangingStockPrice()
        {
            var tenor = RemainingSteps * 0.004;
            var priceVol = Volatility;
            var strike = 100d;

            var capStrike = 130d;
            var barrier = 90d;
            var floor = 90d;

            var trunkCount = 8;

            var startingS0 = 80;
            var endingS0 = 136;

            double timeUsed;
            using (var rt = ReportTime.StartWithMessage("TOTAL TIME ELAPSED {0} (Delta & Gamma)."))
            {
                for (double s = startingS0; s <= endingS0; s += 0.5)
                {
                    using (ReportTime.StartWithMessage("{0} -------- stock " + s))
                    {
                        var trunks = new double[trunkCount];
                        var iteration = Iterations / trunkCount;

                        double s1 = s;
                        Parallel.For(0, trunkCount, j =>
                        {
                            var mc = new MonteCarloGenerator();
                            var result = 0d;
                            for (int i = 0; i < iteration; i++)
                            {
                                result += CalculateTwinWinCollar(s1, strike, capStrike, barrier, floor,
                                    riskFreeRate, priceVol, tenor, RemainingSteps, mc);
                            }
                            trunks[j] = result / iteration;
                        });

                        resultChangingUnderlying[s] = trunks.Average();
                    }
                }

                timeUsed = rt.Elapsed;
            }

            Report("ChangeStockPrice", resultChangingUnderlying, timeUsed);
        }

        public void CalculateWithChangingVolatility()
        {
            var s0 = 100d;
            var tenor = RemainingSteps * 0.004;

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
                                    riskFreeRate, vol1, tenor, RemainingSteps, mc);
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

        public void CalculateWithChangingTime()
        {
            var s0 = 100d;
            var steps = RemainingSteps;
            //   var tenor = 1d;

            var strike = 100d;

            var capStrike = 130d;
            var barrier = 90d;
            var floor = 90d;

            var trunkCount = 8;

            double timeUsed;
            using (var rt = ReportTime.StartWithMessage("TOTAL TIME ELAPSED {0} (Theta)."))
            {
                for (double ttm = RemainingSteps * 0.004; ttm > 0.0001; ttm -= 0.02)
                {
                    using (ReportTime.StartWithMessage("{0} -------- ttm " + ttm))
                    {
                        var trunks = new double[trunkCount];
                        var iteration = Iterations / trunkCount;

                        double ttm1 = ttm;
                        int steps1 = steps;
                        Parallel.For(0, trunkCount, j =>
                        {
                            var mc = new MonteCarloGenerator();
                            var result = 0d;
                            for (int i = 0; i < iteration; i++)
                            {
                                result += CalculateTwinWinCollar(s0, strike, capStrike, barrier, floor,
                                    riskFreeRate, Volatility, ttm1, steps1, mc);
                            }
                            trunks[j] = result / iteration;
                        });

                        resultChangingTime[ttm] = trunks.Average();
                        steps -= 5;
                    }
                }
                timeUsed = rt.Elapsed;
            }

            Report("ChangeTime", resultChangingTime, timeUsed);
        }

        public void CalculateWithChangingInterestRate()
        {
            var s0 = 100d;
            var steps = RemainingSteps;
            var tenor = RemainingSteps * 0.004;

            var strike = 100d;

            var capStrike = 130d;
            var barrier = 90d;
            var floor = 90d;

            var trunkCount = 8;

            var rates = new List<double> { 0.0364927 / 100 };
            for (double rate = 0.01 / 100; rate < 5d / 100; rate += 0.08/100)
            {
                rates.Add(rate);
            }
            double timeUsed;
            using (var rt = ReportTime.StartWithMessage("TOTAL TIME ELAPSED {0} (Theta)."))
            {
                foreach (var rate in rates.OrderBy(r => r))
                {
                    using (ReportTime.StartWithMessage("{0} -------- rate " + rate))
                    {
                        var trunks = new double[trunkCount];
                        var iteration = Iterations / trunkCount;
                        
                        double rate1 = rate;
                        Parallel.For(0, trunkCount, j =>
                        {
                            var mc = new MonteCarloGenerator();
                            var result = 0d;
                            for (int i = 0; i < iteration; i++)
                            {
                                result += CalculateTwinWinCollar(s0, strike, capStrike, barrier, floor,
                                    rate1, Volatility, tenor, steps, mc);
                            }
                            trunks[j] = result / iteration;
                        });

                        resultChangingRate[rate] = trunks.Average();
                    }
                }
                timeUsed = rt.Elapsed;
            }

            Report("ChangeRate", resultChangingRate, timeUsed);
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
                .Select(rv => rv.Key + "," + rv.Value);

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