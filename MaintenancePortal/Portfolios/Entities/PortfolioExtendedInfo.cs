using Maintenance.Common.Entities;

namespace Maintenance.Portfolios.Entities
{
    /// <summary>
    /// Extended information for a portfolio. Notice that the <see cref="PortfolioExtendedInfo.Id"/>
    /// here is the portfolio's id, not the extended info's id.
    /// </summary>
    public class PortfolioExtendedInfo : Entry
    {
        public bool IsTopLevelFund { get; set; }
        public string SectorScheme { get; set; }
        public string AssetClassFocus { get; set; }
        public Location Location { get; set; }

        #region equality members

        protected bool Equals(PortfolioExtendedInfo other)
        {
            return Id == other.Id
                && IsTopLevelFund.Equals(other.IsTopLevelFund)
                && string.Equals(SectorScheme, other.SectorScheme)
                && string.Equals(AssetClassFocus, other.AssetClassFocus)
                && Equals(Location, other.Location);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((PortfolioExtendedInfo)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = IsTopLevelFund.GetHashCode();
                hashCode = (hashCode * 397) ^ (SectorScheme != null ? SectorScheme.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (AssetClassFocus != null ? AssetClassFocus.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Location != null ? Location.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(PortfolioExtendedInfo left, PortfolioExtendedInfo right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PortfolioExtendedInfo left, PortfolioExtendedInfo right)
        {
            return !Equals(left, right);
        }

        #endregion

        public PortfolioExtendedInfo Copy()
        {
            return new PortfolioExtendedInfo
            {
                Id = Id,
                IsTopLevelFund = IsTopLevelFund, 
                SectorScheme = SectorScheme,
                AssetClassFocus = AssetClassFocus,
                Location = Location,
            };
        }

        public override string ToString()
        {
            return string.Format("Id: {0}, IsTopLevelFund: {1}, SectorScheme: {2}, AssetClassFocus: {3}, Location: {4}", Id, IsTopLevelFund, SectorScheme, AssetClassFocus, Location);
        }
    }
}