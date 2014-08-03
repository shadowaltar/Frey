using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Automata.Core;
using Automata.Quantitatives;
using Ninject.Infrastructure.Language;

namespace Automata.Dashboard
{
    public class AutocallMainViewModel : ViewModelBase, IAutocallMainViewModel
    {
        public AutocallMainViewModel()
        {
            Volatility = 25 / 100;
            Iterations = 500000;
            RiskFreeRate = 5 / 100;
            RemainingSteps = 250; // = 1year

            DisplayName = "Autocall Monte Carlo Simulator (8 Core)";
        }

        private readonly Dictionary<double, double> resultChangingUnderlyingWithdVol = new Dictionary<double, double>();

        private readonly Dictionary<double, double> resultChangingUnderlying = new Dictionary<double, double>();

        private readonly object syncRoot = new object();
        private List<string> cache = new List<string>();

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
            var tenor = 3;
            var priceVol = Volatility;
            var strike = 100d;
            var barrier = 100d;
           
            var coupon = 10d;


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

        public void CalculateWithChangingStockPriceForVega()
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

            var dVol = 0.01;
            var dRate = 0.08 / 100;

            double timeUsed;
            using (var rt = ReportTime.StartWithMessage("TOTAL TIME ELAPSED {0} (Delta & Gamma)."))
            {
                for (double s = startingS0; s <= endingS0; s += 0.5)
                {
                    using (ReportTime.StartWithMessage("{0} -------- stock " + s))
                    {
                        var trunkBases = new double[trunkCount];
                        var trunkdVols = new double[trunkCount];
                        var trunkdRates = new double[trunkCount];
                        var iteration = Iterations / trunkCount;

                        double s1 = s;
                        Parallel.For(0, trunkCount, j =>
                        {
                            var mc = new MonteCarloGenerator();
                            var resultBase = 0d;
                            var resultdVol = 0d;
                            var resultdRate = 0d;
                            for (int i = 0; i < iteration; i++)
                            {
                                resultBase += CalculateTwinWinCollar(s1, strike, capStrike, barrier, floor,
                                    riskFreeRate, priceVol, tenor, RemainingSteps, mc);
                                resultdVol += CalculateTwinWinCollar(s1, strike, capStrike, barrier, floor,
                                    riskFreeRate, priceVol + dVol, tenor, RemainingSteps, mc);
                                resultdRate += CalculateTwinWinCollar(s1, strike, capStrike, barrier, floor,
                                    riskFreeRate + dRate, priceVol, tenor, RemainingSteps, mc);
                            }
                            trunkBases[j] = resultBase / iteration;
                            trunkdVols[j] = resultdVol / iteration;
                            trunkdRates[j] = resultdRate / iteration;
                        });

                        resultChangingUnderlying[s] = trunkBases.Average();
                        resultChangingUnderlyingWithdVol[s] = trunkdVols.Average();
                        resultChangingUnderlyingWithdRate[s] = trunkdRates.Average();
                    }
                }

                timeUsed = rt.Elapsed;
            }

            Report("ChangeStockPrice+VegaRho(BASE)", resultChangingUnderlying, timeUsed);
            Report("ChangeStockPrice+VegaRho(VEGA)", resultChangingUnderlyingWithdVol, timeUsed);
            Report("ChangeStockPrice+VegaRho(RHO)", resultChangingUnderlyingWithdRate, timeUsed);
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
            price1 = Math.Min(price1, capStrike - strike);
            var price2 = knockedOut2
                ? Math.Max(knockedOutFloor, stAnti) - strike
                : Math.Min(Math.Abs(stAnti - strike), capStrike - strike);
            price2 = Math.Min(price2, capStrike - strike);

            return (price1 + price2) / 2
                * Math.Exp(-rf * tenor);
        }

        internal double CalculateTwinWinCollar(double s0,
            double strike, double capStrike, double knockOutBarrier,
            double knockedOutFloor,
            double rf,
            double priceStdev, double tenor, int steps,
            MonteCarloGenerator mc, out double terminalUnderlyingPrice)
        {
            var st = s0;
            var dt = tenor / steps;

            var stAnti = s0;
            var knockedOut1 = false;
            for (int i = 0; i < steps; i++)
            {
                double drift;
                mc.NextPriceWithAntithetic(st, stAnti, rf, priceStdev, dt, out drift, out st, out stAnti);
                if (!knockedOut1 && st <= knockOutBarrier)
                {
                    knockedOut1 = true;
                }
            }

            var price1 = knockedOut1
                ? Math.Max(knockedOutFloor, st) - strike
                : Math.Min(Math.Abs(st - strike), capStrike - strike);
            price1 = Math.Min(price1, capStrike - strike);
            terminalUnderlyingPrice = st;

            return price1 * Math.Exp(-rf * tenor);
        }

        private static void Report(string fileNameHead, Dictionary<double, double> data, double timeUsed)
        {
            var formattedResults = data.OrderBy(rv => rv.Key)
                .Select(rv => rv.Key + "," + rv.Value);

            File.WriteAllLines(fileNameHead + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv"
                , formattedResults);

            MessageBox.Show("DONE! Time used: " + timeUsed);
        }

        private static void Report(string fileNameHead, List<string> data, double timeUsed)
        {
            File.WriteAllLines(fileNameHead + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv"
                , data);

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