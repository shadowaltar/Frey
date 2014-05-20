using System;
using MathNet.Numerics.Distributions;

namespace Automata.Quantitatives
{
    public class MonteCarloGenerator
    {
        private static readonly Normal StandardNormal = new Normal();

        public double NextStandardNormalRandom()
        {
            return StandardNormal.Sample();
        }

        public double NextPrice(double lastPrice, double riskFreeRate, double priceStdev, double deltaT)
        {
            return lastPrice * Math.Exp((riskFreeRate - priceStdev * priceStdev / 2) * deltaT +
                            priceStdev * deltaT * NextStandardNormalRandom());
        }

        public double NextPrice(double lastPrice, double riskFreeRate, double priceStdev, double deltaT, out double drift)
        {
            drift = NextStandardNormalRandom();
            return lastPrice * Math.Exp((riskFreeRate - priceStdev * priceStdev / 2) * deltaT +
                            priceStdev * deltaT * drift);
        }

        public void NextPriceWithAntithetic(double lastPrice1, double lastPrice2,
            double riskFreeRate, double priceStdev, double deltaT, out double drift,
            out double nextPrice1, out double nextPrice2)
        {
            drift = NextStandardNormalRandom();
            var a = (riskFreeRate - priceStdev * priceStdev / 2) * deltaT;
            var factor = priceStdev * Math.Sqrt(deltaT) * drift;
            nextPrice1 = lastPrice1 * Math.Exp(a + factor);
            nextPrice2 = lastPrice2 * Math.Exp(a - factor);
        }
    }
}