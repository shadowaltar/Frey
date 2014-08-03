using System.Collections.Generic;

namespace Trading.Common.Entities
{
    public class PortfolioManager
    {
        private static readonly PortfolioManager AnyPortfolioManager = new PortfolioManager { Id = -1, Name = "Any" };

        public PortfolioManager()
        {
            PortfolioIds = new List<long>();
        }

        public long Id { get; set; }
        public string AId { get; set; }
        public string Name { get; set; }

        public static PortfolioManager Any
        {
            get { return AnyPortfolioManager; }
        }

        public List<long> PortfolioIds { get; private set; }

        #region equality members

        protected bool Equals(PortfolioManager other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((PortfolioManager) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(PortfolioManager left, PortfolioManager right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PortfolioManager left, PortfolioManager right)
        {
            return !Equals(left, right);
        }

        #endregion

        public override string ToString()
        {
            return Name;
        }
    }
}