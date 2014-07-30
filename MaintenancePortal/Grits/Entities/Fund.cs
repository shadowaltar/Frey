namespace Maintenance.Grits.Entities
{
    public class Fund
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string DisplayName { get { return Code + ", " + Name; } }

        public Benchmark Benchmark { get; set; }

        protected bool Equals(Fund other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Fund)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return Code;
        }
    }

    public class GritsFund : Fund
    {
        public GritsMode Mode { get; set; }
    }
}