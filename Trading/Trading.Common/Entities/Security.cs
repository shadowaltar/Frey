using System;

namespace Trading.Common.Entities
{
    public class Security : Entity
    {
        public string Type { get; set; }
        public DateTime Inception { get; set; }
        public int LotSize { get; set; }
        public string Currency { get; set; }
        public bool IsShortSellable { get; set; }

        public Market Market { get; set; }

        public string CurrencySymbol
        {
            get
            {
                return Currency == "HKD" ? "HK$" : "$";
            }
        }

        protected bool Equals(Security other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Security)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}