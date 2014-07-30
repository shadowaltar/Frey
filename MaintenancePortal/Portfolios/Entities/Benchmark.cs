using System;
using Maintenance.Common.Entities;
using Maintenance.Common.Entities.Decorators;
using Maintenance.Common.Utils;

namespace Maintenance.Portfolios.Entities
{
    public class Benchmark : Entry, IEffectiveTimeRange
    {
        // public long Id { get; set; } is a fake id; db table has no id field.

        public Index Index { get; set; }

        public string Code { get { return Index == null ? string.Empty : Index.Code; } }
        public string Name { get { return Index == null ? string.Empty : Index.Name; } }
        public string Type { get; set; }

        public Benchmark Clone()
        {
            return new Benchmark
            {
                Id = Id,
                Index = Index,
                EffectiveDate = EffectiveDate,
                ExpiryDate = ExpiryDate,
                Type = Type,
            };
        }

        #region equality members

        protected bool Equals(Benchmark other)
        {
            return Id == other.Id
                && Equals(Index, other.Index)
                && EffectiveDate.Equals(other.EffectiveDate)
                && ExpiryDate.Equals(other.ExpiryDate);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Benchmark)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Id.GetHashCode();
                hashCode = (hashCode * 397) ^ (Index != null ? Index.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ EffectiveDate.GetHashCode();
                hashCode = (hashCode * 397) ^ ExpiryDate.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(Benchmark left, Benchmark right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Benchmark left, Benchmark right)
        {
            return !Equals(left, right);
        }

        #endregion

        public override string ToString()
        {
            return Index + ", On:" + EffectiveDate.IsoFormat();
        }

        public DateTime EffectiveDate { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
