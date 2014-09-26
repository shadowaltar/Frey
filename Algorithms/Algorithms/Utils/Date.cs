using System;

namespace Algorithms.Utils
{
    public class Date
    {
        private readonly int d;
        private readonly int m;
        private readonly int y;
        private readonly long ticks;
        //private DayOfWeek 
        public const long TicksPerDay = 864000000000L;
        public const long TicksPerYear = 864000000000L * 365;
        public const long TicksPerLeapYear = 864000000000L * 366;

        public Date(int y, int m, int d)
        {
            this.y = y;
            this.m = m;
            this.d = d;
            Check();
        }

        private void Check()
        {
            if (m < 1 || d < 1)
                throw new InvalidOperationException();
            if (m > 12)
                throw new InvalidOperationException();
            if (d > 31)
                throw new InvalidOperationException();
            if (m == 4 || m == 6 || m == 9 || m == 11)
                if (d > 30)
                    throw new InvalidOperationException();
            if (IsLeapYear)
            {
                if (d > 29)
                    throw new InvalidOperationException();
            }
            else
            {
                if (d > 28)
                    throw new InvalidOperationException();
            }
        }

        public int Year
        {
            get { return y; }
        }

        public int Month
        {
            get { return m; }
        }

        public int Day
        {
            get { return d; }
        }

        public bool IsLeapYear
        {
            get
            {
                if (y % 100 == 0)
                {
                    return y % 400 == 0;
                }
                return y % 4 == 0;
            }
        }
    }
}