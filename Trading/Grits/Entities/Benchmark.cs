namespace Maintenance.Grits.Entities
{
    public class Benchmark
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string DisplayName { get { return Code + ", " + Name; } }

        protected bool Equals(Benchmark other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Benchmark)obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public static bool operator ==(Benchmark left, Benchmark right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Benchmark left, Benchmark right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return Code + ", " + Name;
        }
    }

    public class GritsBenchmark : Benchmark
    {
        public GritsMode Mode { get; set; }
        public bool IsLoadedAtNight { get; set; }
    }
}