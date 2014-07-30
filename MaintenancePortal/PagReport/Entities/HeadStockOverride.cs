using System;
using Maintenance.Common.Entities;

namespace Maintenance.PagReport.Entities
{
    public class HeadStockOverride : Security
    {
        public HeadStockOverride(long id, HeadStockOverrideType type)
        {
            Key = new HeadStockOverrideKey(id, type);
            Id = id;
        }

        public HeadStockOverrideKey Key { get; private set; }
        public HeadStockOverrideType Type { get { return Key.Type; } }
        public Security NewSecurity { get; set; }

        #region equality members

        protected bool Equals(HeadStockOverride other)
        {
            return base.Equals(other) && Key.Equals(other.Key);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((HeadStockOverride)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ Key.GetHashCode();
            }
        }

        public static bool operator ==(HeadStockOverride left, HeadStockOverride right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(HeadStockOverride left, HeadStockOverride right)
        {
            return !Equals(left, right);
        }

        #endregion
    }

    /// <summary>
    /// Key of a <see cref="HeadStockOverride"/>.
    /// </summary>
    public struct HeadStockOverrideKey : IEquatable<HeadStockOverrideKey>
    {
        public HeadStockOverrideKey(long id, HeadStockOverrideType type)
            : this()
        {
            Id = id;
            Type = type;
        }

        public long Id { get; set; }
        public HeadStockOverrideType Type { get; set; }

        #region equality members

        public bool Equals(HeadStockOverrideKey other)
        {
            return Id == other.Id && Type == other.Type;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is HeadStockOverrideKey && Equals((HeadStockOverrideKey)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Id.GetHashCode() * 397) ^ (int)Type;
            }
        }

        public static bool operator ==(HeadStockOverrideKey left, HeadStockOverrideKey right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(HeadStockOverrideKey left, HeadStockOverrideKey right)
        {
            return !left.Equals(right);
        }

        #endregion
    }
}