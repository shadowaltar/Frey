using System;

namespace Maintenance.Common.Entities
{
    public class AssetMapComponentProperty
    {
        public AssetMapComponentProperty(long id, string key, object value,
            DateTime updateTime, AssetMapComponent assetMapComponent = null)
        {
            Id = id;
            Key = key;
            Value = value;
            UpdateTime = updateTime;
            AssetMapComponent = assetMapComponent;
        }

        public long Id { get; set; }
        public string Key { get; set; }
        public object Value { get; set; }
        public DateTime UpdateTime { get; set; }
        public AssetMapComponent AssetMapComponent { get; set; }

        public AssetMapComponentProperty Copy()
        {
            return new AssetMapComponentProperty(Id, Key, Value, UpdateTime, AssetMapComponent);
        }

        #region equality members

        protected bool Equals(AssetMapComponentProperty other)
        {
            // AssetMapComponent's Equals() depends on Id and Code.
            return Id == other.Id && Equals(AssetMapComponent, other.AssetMapComponent);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((AssetMapComponentProperty)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Id.GetHashCode() * 397) ^ (AssetMapComponent != null ? AssetMapComponent.GetHashCode() : 0);
            }
        }

        public static bool operator ==(AssetMapComponentProperty left, AssetMapComponentProperty right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AssetMapComponentProperty left, AssetMapComponentProperty right)
        {
            return !Equals(left, right);
        }
        #endregion

        public override string ToString()
        {
            return string.Format(" Id: {3}, [{0}, {1}], ASM_COMP: {2}", Key, Value, AssetMapComponent, Id);
        }
    }
}