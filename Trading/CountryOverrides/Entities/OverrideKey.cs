namespace Trading.CountryOverrides.Entities
{
    /// <summary>
    /// Unique key of an override entry. Consists of the security's id
    /// and the type of override like PM or FILC.
    /// </summary>
    public struct OverrideKey
    {
        public OverrideKey(long securityId, OverrideType type, long portfolioId, long portfolioManagerId)
            : this()
        {
            SecurityId = securityId;
            Type = type;
            PortfolioId = portfolioId;
            PortfolioManagerId = portfolioManagerId;
        }

        public long SecurityId { get; set; }
        public OverrideType Type { get; set; }
        public long PortfolioId { get; set; }
        public long PortfolioManagerId { get; set; }

        #region equality members
        public bool Equals(OverrideKey other)
        {
            return SecurityId == other.SecurityId && Type == other.Type && PortfolioId == other.PortfolioId && PortfolioManagerId == other.PortfolioManagerId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is OverrideKey && Equals((OverrideKey)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = SecurityId.GetHashCode();
                hashCode = (hashCode * 397) ^ (int)Type;
                hashCode = (hashCode * 397) ^ PortfolioId.GetHashCode();
                hashCode = (hashCode * 397) ^ PortfolioManagerId.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(OverrideKey left, OverrideKey right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(OverrideKey left, OverrideKey right)
        {
            return !left.Equals(right);
        } 
        #endregion

        public override string ToString()
        {
            return string.Format("{0}-{1},PID:{2},PMID:{3}", SecurityId, Type, PortfolioId, PortfolioManagerId);
        }
    }
}