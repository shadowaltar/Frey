namespace Maintenance.Benchmarks.Entities
{
    public class BenchmarkDependency
    {
        public BenchmarkDependencyType Type { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        #region equality members

        protected bool Equals(BenchmarkDependency other)
        {
            return Type == other.Type && string.Equals(Code, other.Code) && string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((BenchmarkDependency)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)Type;
                hashCode = (hashCode * 397) ^ (Code != null ? Code.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(BenchmarkDependency left, BenchmarkDependency right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(BenchmarkDependency left, BenchmarkDependency right)
        {
            return !Equals(left, right);
        }

        #endregion

        public override string ToString()
        {
            return string.Format("{0}: Code: {1}, Name: {2}", Type, Code, Name);
        }
    }
}