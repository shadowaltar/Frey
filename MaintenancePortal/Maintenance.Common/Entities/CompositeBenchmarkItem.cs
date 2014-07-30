using System.Collections.Generic;

namespace Maintenance.Common.Entities
{
    public class CompositeBenchmarkItem : IHasChildCompositeBenchmarkItems
    {
        public CompositeBenchmarkItem()
        {
            Components = new List<CompositeBenchmarkItem>();
        }

        public long Id { get; set; }
        public Index Index { get; set; }
        public decimal Weight { get; set; }

        public bool IsReal { get; set; }

        public PortfolioAssetMapLink PortfolioAssetMapLink { get; set; }

        public AssetMapComponent AssetMapComponent { get; set; }

        public List<CompositeBenchmarkItem> Components { get; private set; }

        #region equality members

        protected bool Equals(CompositeBenchmarkItem other)
        {
            return Id == other.Id && Equals(Index, other.Index) && Weight == other.Weight && IsReal.Equals(other.IsReal) && Equals(AssetMapComponent, other.AssetMapComponent);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((CompositeBenchmarkItem)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Id.GetHashCode();
                hashCode = (hashCode * 397) ^ (Index != null ? Index.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Weight.GetHashCode();
                hashCode = (hashCode * 397) ^ IsReal.GetHashCode();
                hashCode = (hashCode * 397) ^ (AssetMapComponent != null ? AssetMapComponent.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(CompositeBenchmarkItem left, CompositeBenchmarkItem right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CompositeBenchmarkItem left, CompositeBenchmarkItem right)
        {
            return !Equals(left, right);
        }

        #endregion

        public override string ToString()
        {
            return string.Format("Id: {0}, Index: {1}, " +
                                 "Weight: {2}, AssetMapComponent: {3}, " +
                                 "Exists: {4}",
                                 Id, Index, Weight, AssetMapComponent, IsReal);
        }

        public static CompositeBenchmarkItem NewPlaceholder(AssetMapComponent assetMapComponent)
        {
            return new CompositeBenchmarkItem
            {
                Id = 0,
                AssetMapComponent = assetMapComponent,
                Index = null,
                IsReal = false,
                Weight = 0,
            };
        }
    }
}