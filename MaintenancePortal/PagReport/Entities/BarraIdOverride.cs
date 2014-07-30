using System;
using Maintenance.Common.Entities;

namespace Maintenance.PagReport.Entities
{
    public class BarraIdOverride : Security
    {
        public BarraIdOverride(long id, BarraIdOverrideType type)
        {
            Key = new BarraIdOverrideKey(id, type);
            Id = id;
        }

        public BarraIdOverrideKey Key { get; private set; }
        public BarraIdOverrideType Type { get { return Key.Type; } }

        public string BarraId { get; set; }

        #region equality members

        protected bool Equals(BarraIdOverride other)
        {
            return Key.Equals(other.Key);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BarraIdOverride)obj);
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }

        public static bool operator ==(BarraIdOverride left, BarraIdOverride right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(BarraIdOverride left, BarraIdOverride right)
        {
            return !Equals(left, right);
        }

        #endregion
    }

    /// <summary>
    /// Key of a <see cref="BarraIdOverride"/>.
    /// </summary>
    public struct BarraIdOverrideKey : IEquatable<BarraIdOverrideKey>
    {
        public BarraIdOverrideKey(long id, BarraIdOverrideType type)
            : this()
        {
            Id = id;
            Type = type;
        }

        public long Id { get; set; }
        public BarraIdOverrideType Type { get; set; }

        #region equality members

        public bool Equals(BarraIdOverrideKey other)
        {
            return Id == other.Id && Type == other.Type;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is BarraIdOverrideKey && Equals((BarraIdOverrideKey)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Id.GetHashCode() * 397) ^ (int)Type;
            }
        }

        public static bool operator ==(BarraIdOverrideKey left, BarraIdOverrideKey right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(BarraIdOverrideKey left, BarraIdOverrideKey right)
        {
            return !left.Equals(right);
        }

        #endregion
    }
}