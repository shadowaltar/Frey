namespace Automata.Entities
{
    public class Exchange
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Mic { get; set; }

        public Country Country { get; set; }

        private static readonly Exchange otc = new Exchange { Code = "OTC", Id = -1, Name = "Over The Counter", Mic = "N/A" };
        public static Exchange OTC { get { return otc; } }

        protected bool Equals(Exchange other)
        {
            return string.Equals(Code, other.Code);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Exchange)obj);
        }

        public override int GetHashCode()
        {
            return (Code != null ? Code.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", Code, Country);
        }
    }
}
