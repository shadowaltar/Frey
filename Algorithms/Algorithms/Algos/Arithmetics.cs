using System;
using System.Numerics;

namespace Algorithms.Algos
{
    public static class Arithmetics
    {
        public static int Absolute(this int x)
        {
            return x < 0 ? -x : x;
        }
        public static long Absolute(this long x)
        {
            return x < 0 ? -x : x;
        }
        public static double Absolute(this double x)
        {
            return x < 0 ? -x : x;
        }
        public static decimal Absolute(this decimal x)
        {
            return x < 0 ? -x : x;
        }

        public static long Floor(this double x)
        {
            return (long)x;
        }
        public static long Ceiling(this double x)
        {
            var y = (long)x;
            if (Absolute(y - x) < double.Epsilon)
                return y;
            return y + 1;
        }
        public static long Rounding(this double x)
        {
            return (long)(x + 0.5);
        }

        public static double Power(this double b, double power)
        {
            return Math.Pow(b, power);
        }

        public static BigInteger Factorial(this int x)
        {
            if (x < 0) throw new ArithmeticException();
            if (x == 0) return 1;
            var result = new BigInteger(1);
            for (int i = 1; i <= x; i++)
            {
                result = BigInteger.Multiply(result, i);
            }
            return result;
        }

        public static int GreatestCommonDivisor(int p, int q)
        {
            if (q == 0) return p;
            int r = p % q;
            return GreatestCommonDivisor(q, r);
        }

        public static bool IsPrime(this int x)
        {
            if (x < 2) return false;
            for (int i = 2; i * i <= x; i++)
            {
                if (x % i == 0) return false;
            }
            return true;
        }
        public static bool IsPrime(this long x)
        {
            if (x < 2) return false;
            for (int i = 2; i * i <= x; i++)
            {
                if (x % i == 0) return false;
            }
            return true;
        }

        public static double SquareRoot(this double c)
        {
            if (c < 0.0) return double.NaN;
            const double error = double.Epsilon;
            double t = c;
            var lastErr = 0.0;
            while (true)
            {
                var a = c / t;
                var b = Absolute(t - a);
                if (b > error * t)
                {
                    t = (a + t) / 2.0;
                    lastErr = Absolute(t - a);
                }
                Console.WriteLine("a:{0};b:{1};t:{2}", a, b, t);
                if (Math.Abs(lastErr - b) < error || b < error || double.IsNaN(b))
                    break;
            }
            return t;
        }

        public static double Hypotenuse(this double a, double b)
        {
            return SquareRoot(a * a + b * b);
        }

        public static double Harmonics(this int x)
        {
            var sum = 0.0;
            for (int i = 1; i <= x; i++)
                sum += 1.0 / i;
            return sum;
        }
    }
}