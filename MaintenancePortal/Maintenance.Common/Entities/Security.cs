using System;

namespace Maintenance.Common.Entities
{
    public class Security : Entity
    {
        public string FullName { get; set; }
        public Country Country { get; set; }
        public string Cusip { get; set; }
        public string Sedol { get; set; }

        public override string Code
        {
            get { throw new NotSupportedException("Security does not support an explicit Code field."); }
            set { throw new NotSupportedException("Security does not support an explicit Code field."); }
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

        public override string ToString()
        {
            return string.Format("{0}, {1}", Cusip, Name);
        }
    }
}