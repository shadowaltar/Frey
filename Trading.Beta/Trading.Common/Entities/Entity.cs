using System;
using Trading.Common.Entities.Decorators;

namespace Trading.Common.Entities
{
    /// <summary>
    /// Basic entity across all objects to be manipulated on.
    /// Equality is based on the Id and Code.
    /// </summary>
    public class Entity : Entry, IEffectiveTimeRange
    {
        public virtual string Code { get; set; }
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the date that this entity becomes effective (inclusive).
        /// </summary>
        public virtual DateTime EffectiveDate { get; set; }

        /// <summary>
        /// Gets or sets the date that this entity becomes expired or deactivated (effectiveness exclusive).
        /// </summary>
        public virtual DateTime ExpiryDate { get; set; }

        public Entity() { }

        protected Entity(long id, string code, string name)
        {
            Id = id;
            Code = code;
            Name = name;
        }

        public virtual string DisplayName { get { return ToString(); } }

        #region equality members

        protected bool Equals(Entity other)
        {
            return Id == other.Id && string.Equals(Code, other.Code);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Entity)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Id.GetHashCode() * 397) ^ (Code != null ? Code.GetHashCode() : 0);
            }
        }

        public static bool operator ==(Entity left, Entity right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Entity left, Entity right)
        {
            return !Equals(left, right);
        }

        #endregion

        public override string ToString()
        {
            return Code + ", " + Name;
        }
    }
}