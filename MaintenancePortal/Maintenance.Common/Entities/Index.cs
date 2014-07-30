namespace Maintenance.Common.Entities
{
    public class Index : Entity
    {
        #region equality members

        protected bool Equals(Index other)
        {
            return Id == other.Id && string.Equals(Code, other.Code);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Index)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Id.GetHashCode() * 397) ^ (Code != null ? Code.GetHashCode() : 0);
            }
        }

        public static bool operator ==(Index left, Index right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Index left, Index right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}