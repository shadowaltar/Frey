using System;
using System.Collections.Generic;
using Automata.Core;

namespace Automata.Entities
{
    public class TimeValue
    {
        public DateTime Time { get; set; }
        public double Value { get; set; }

        public TimeValue(DateTime time, double value)
        {
            Time = time;
            Value = value;
        }

        #region equality related

        protected bool Equals(TimeValue other)
        {
            return Time.Equals(other.Time) && Value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((TimeValue)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Time.GetHashCode() * 397) ^ Value.GetHashCode();
            }
        }

        private sealed class TimeValueEqualityComparer : IEqualityComparer<TimeValue>
        {
            public bool Equals(TimeValue x, TimeValue y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.Time.Equals(y.Time) && x.Value.Equals(y.Value);
            }

            public int GetHashCode(TimeValue obj)
            {
                unchecked
                {
                    return (obj.Time.GetHashCode() * 397) ^ obj.Value.GetHashCode();
                }
            }
        }

        private static readonly IEqualityComparer<TimeValue> TimeValueComparerInstance = new TimeValueEqualityComparer();

        public static IEqualityComparer<TimeValue> TimeValueComparer
        {
            get { return TimeValueComparerInstance; }
        }

        #endregion

        public override string ToString()
        {
            return "[" + Time.Print() + "] " + Value;
        }
    }
}