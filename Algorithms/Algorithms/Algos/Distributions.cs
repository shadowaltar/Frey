using System.Numerics;

namespace Algorithms.Algos
{
    public class Distributions
    {
        public static long BinomialCoefficient(int n, int k)
        {
            long result = 1;
            if (k > n) return 0;

            for (long d = 1; d <= k; d++)
            {
                result *= n--;
                result /= d;
            }
            return result;
        }

        public static BigInteger BinomialCoefficientBigInteger(int n, int k)
        {
            BigInteger result = 1;
            if (k > n) return 0;

            for (long d = 1; d <= k; d++)
            {
                result *= n--;
                result /= d;
            }
            return result;
        }

        public static double GetBinomialAt(int n, int k, double p)
        {
            return BinomialCoefficient(n, k) * p.Power(k) * (1 - p).Power(n - k);
        }
    }
}