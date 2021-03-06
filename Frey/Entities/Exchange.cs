﻿namespace Automata.Entities
{
    public class Exchange
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Mic { get; set; }

        public Country Country { get; set; }

        #region Special Exchanges

        private static readonly Exchange ForexExchange = new Exchange
        {
            Code = "FOREX GLOBAL",
            Country = Country.Global,
            Id = -1,
            Mic = string.Empty,
            Name = "Forex Exchange"
        };
        public static Exchange Forex { get { return ForexExchange; } }

        #endregion

        protected bool Equals(Exchange other)
        {
            return string.Equals(Code, other.Code);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Exchange) obj);
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
