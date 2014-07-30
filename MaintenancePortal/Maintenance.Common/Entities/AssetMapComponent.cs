using System.Collections.Generic;
using System.Linq;

namespace Maintenance.Common.Entities
{
    public class AssetMapComponent : Entity
    {
        public AssetMapComponent()
        {
            Children = new List<AssetMapComponent>();
            Properties = new List<AssetMapComponentProperty>();
        }

        protected AssetMapComponent(long id, string code, string name, int order, AssetMap assetMap, AssetMapComponent parent, List<AssetMapComponent> children, List<AssetMapComponentProperty> properties, bool isRoot)
            : base(id, code, name)
        {
            Order = order;
            AssetMap = assetMap;
            Parent = parent;
            Children = children;
            Properties = properties;
            IsRoot = isRoot;
        }

        public int Order { get; set; }

        public AssetMap AssetMap { get; set; }

        public AssetMapComponent Parent { get; set; }

        public List<AssetMapComponent> Children { get; private set; }

        public List<AssetMapComponentProperty> Properties { get; private set; }

        public bool IsRoot { get; set; }

        public long AsmId { get { return AssetMap.Id; } }
        public long ParentId { get { return Parent == null ? 0 : Parent.Id; } }

        /// <summary>
        /// Return a (shallow) copy of current asm component, which means the references
        /// of this instance's AssetMap and parent component are inact;
        /// however, all the Children and Properties will get their Copy() called.
        /// </summary>
        /// <returns></returns>
        public AssetMapComponent Copy()
        {
            var copy = new AssetMapComponent(Id, Code, Name, Order, AssetMap, Parent,
                Children.Select(c => c.Copy()).ToList(),
                Properties.Select(p => p.Copy()).ToList(), IsRoot);
            foreach (var child in copy.Children) // replace the cyclic reference by the new copy
            {
                child.Parent = copy;
            }
            return copy;
        }

        public static int GetLevel(AssetMapComponent component)
        {
            if (component.Parent == null)
            {
                return 0;
            }
            return GetLevel(component.Parent) + 1;
        }
    }
}