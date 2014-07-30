namespace Maintenance.Common.Entities
{
    public class AssetMap : Entity
    {
        public string ComponentPrefix { get; set; }

        public AssetMapComponent RootComponent { get; set; }

        public AssetMap() { }

        private AssetMap(long id, string name, string code, string componentPrefix, AssetMapComponent rootComponent)
            : base(id, code, name)
        {
            ComponentPrefix = componentPrefix;
            RootComponent = rootComponent;
        }

        #region equality members

        protected bool Equals(AssetMap other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((AssetMap)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(AssetMap left, AssetMap right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AssetMap left, AssetMap right)
        {
            return !Equals(left, right);
        }

        #endregion

        public override string ToString()
        {
            return Id + ", " + Name + ", " + Code;
        }

        public AssetMap Copy()
        {
            var assetMap = new AssetMap(Id, Name, Code, ComponentPrefix, RootComponent.Copy());
            assetMap.RootComponent.AssetMap = assetMap; // replace the cyclic reference by the new copy
            return assetMap;
        }
    }
}