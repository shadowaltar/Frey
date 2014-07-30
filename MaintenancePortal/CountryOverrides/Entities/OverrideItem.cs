using Maintenance.Common.Entities;

namespace Maintenance.CountryOverrides.Entities
{
    public class OverrideItem
    {
        public OverrideKey UniqueKey { get; set; }

        public string Name { get; set; }
        public string FullName { get; set; }
        public string Cusip { get; set; }
        public string Sedol { get; set; }

        public Country OldCountry { get; set; }
        public Country NewCountry { get; set; }
        public Portfolio Portfolio { get; set; }
        public PortfolioManager PortfolioManager { get; set; }

        public long Id { get { return UniqueKey.SecurityId; } }
        public OverrideType Type { get { return UniqueKey.Type; } }

        public string OldCountryName { get { return OldCountry == null ? string.Empty : OldCountry.Name; } }
        public string OldCountryCode { get { return OldCountry == null ? string.Empty : OldCountry.Code; } }
        public string NewCountryName { get { return NewCountry == null ? string.Empty : NewCountry.Name; } }
        public string NewCountryCode { get { return NewCountry == null ? string.Empty : NewCountry.Code; } }
        public long PortfolioManagerId { get { return PortfolioManager == null ? 0 : PortfolioManager.Id; } }
        public string PortfolioManagerName { get { return PortfolioManager == null ? string.Empty : PortfolioManager.Name; } }
        public long PortfolioId { get { return Portfolio == null ? 0 : Portfolio.Id; } }
        public string PortfolioName { get { return Portfolio == null ? string.Empty : Portfolio.DisplayName; } }

        public string AdditionalInfo
        {
            get
            {
                return Type == OverrideType.PM
                    ? PortfolioManager.Name
                    : Type == OverrideType.PORTFOLIO ? Portfolio.Code
                    : string.Empty;
            }
        }

        public ItemPrecedence ItemPrecedence { get; set; }

        #region equality members

        protected bool Equals(OverrideItem other)
        {
            return UniqueKey.Equals(other.UniqueKey) && Equals(OldCountry, other.OldCountry) && Equals(NewCountry, other.NewCountry) && Equals(Portfolio, other.Portfolio) && Equals(PortfolioManager, other.PortfolioManager);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((OverrideItem)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = UniqueKey.GetHashCode();
                hashCode = (hashCode * 397) ^ (OldCountry != null ? OldCountry.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (NewCountry != null ? NewCountry.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Portfolio != null ? Portfolio.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (PortfolioManager != null ? PortfolioManager.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(OverrideItem left, OverrideItem right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(OverrideItem left, OverrideItem right)
        {
            return !Equals(left, right);
        }

        #endregion

        public override string ToString()
        {
            return string.Format("{0} -> {1}, {2}, {3}", NewCountryCode, OldCountryCode, Type, Name);
        }
    }
}