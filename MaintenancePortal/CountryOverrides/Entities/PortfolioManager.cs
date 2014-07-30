using System.Collections.Generic;

namespace Maintenance.CountryOverrides.Entities
{
    public class PortfolioManager
    {
        public long Id { get; set; }
        public string Name { get; set; }
        
        public PortfolioManager()
        {
            PortfolioIds = new List<long>();
        }

        private static readonly PortfolioManager AnyPortfolioManager = new PortfolioManager { Id = -1, Name = "Any" };
        public static PortfolioManager Any { get { return AnyPortfolioManager; } }

        public List<long> PortfolioIds { get; private set; }

        protected bool Equals(PortfolioManager other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((PortfolioManager)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}