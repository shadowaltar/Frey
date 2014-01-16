namespace Automata.Entities
{
    public class Country
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        #region Special Countries

        private static readonly Country GlobalCountry = new Country
        {
            Code = "GLOBAL",
            Id = -1,
            Name = "Global"
        };
        public static Country Global { get { return GlobalCountry; } }

        #endregion

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
            return Code;
        }
    }
}
