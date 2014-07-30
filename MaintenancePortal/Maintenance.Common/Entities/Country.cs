namespace Maintenance.Common.Entities
{
    public class Country : Location
    {
        public Country()
        {
            IsCountry = true;
        }

        protected bool Equals(Country other)
        {
            return string.Equals(Code, other.Code);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Country)obj);
        }

        public override int GetHashCode()
        {
            return (Code != null ? Code.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}", Code, Name);
        }
    }
}