using System;
using System.Collections.Generic;
using Maintenance.Common.Entities.Decorators;
using Maintenance.Common.Utils;

namespace Maintenance.Common.Entities
{
    public class PortfolioAssetMapLink : Entry, IEffectiveTimeRange, IHasChildCompositeBenchmarkItems
    {
        private readonly List<CompositeBenchmarkItem> components = new List<CompositeBenchmarkItem>();

        public Portfolio Portfolio { get; set; }
        public AssetMap AssetMap { get; set; }
        public bool IsDefault { get; set; }
        public List<CompositeBenchmarkItem> Components { get { return components; } }

        /// <summary>
        /// Gets or sets the date that this entity becomes effective (inclusive).
        /// </summary>
        public DateTime EffectiveDate { get; set; }

        /// <summary>
        /// Gets or sets the date that this entity becomes expired or deactivated (effectiveness exclusive).
        /// </summary>
        public DateTime ExpiryDate { get; set; }

        public bool IsExpired { get { return ExpiryDate < DateTime.Today; } }
        public bool IsCompositeBenchmarkExist { get { return !Components.IsNullOrEmpty(); } }

        #region equality members

        protected bool Equals(PortfolioAssetMapLink other)
        {
            return Id.Equals(other.Id)
                && Equals(Portfolio, other.Portfolio)
                && Equals(AssetMap, other.AssetMap)
                && ExpiryDate.Equals(other.ExpiryDate)
                && EffectiveDate.Equals(other.EffectiveDate);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((PortfolioAssetMapLink)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ (Portfolio != null ? Portfolio.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (AssetMap != null ? AssetMap.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ ExpiryDate.GetHashCode();
                hashCode = (hashCode * 397) ^ EffectiveDate.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(PortfolioAssetMapLink left, PortfolioAssetMapLink right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PortfolioAssetMapLink left, PortfolioAssetMapLink right)
        {
            return !Equals(left, right);
        }

        #endregion

        public override string ToString()
        {
            return string.Format("Portfolio: {0}, AssetMap: {1}, IsExpired: {2}", Portfolio, AssetMap, IsExpired);
        }
    }

    /// <summary>
    /// Interface that indicates the implemented object is the parent of a 
    /// list of <see cref="CompositeBenchmarkItem"/>s.
    /// </summary>
    public interface IHasChildCompositeBenchmarkItems
    {
        List<CompositeBenchmarkItem> Components { get; }
    }
}