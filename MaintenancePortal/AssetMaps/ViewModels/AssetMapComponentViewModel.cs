using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using Maintenance.Common.Entities;
using Maintenance.Common.Entities.Decorators;
using Maintenance.Common.ViewModels;

namespace Maintenance.AssetMaps.ViewModels
{
    public class AssetMapComponentViewModel : ViewModelBaseSlim, ITreeNode,
        IHasChildNodes<AssetMapComponentViewModel>
    {
        private readonly BindableCollection<AssetMapComponentViewModel> children;
        private int propertyCount;
        private bool isExpanded;
        private bool isSelected;
        private bool isVisible;
        private bool willBeDeleted;
        private bool canEditName;

        public AssetMapComponentViewModel(AssetMapComponent component)
            : this(component, null)
        {
        }

        public AssetMapComponentViewModel(AssetMapComponent component, AssetMapComponentViewModel parent)
        {
            Component = component;
            Parent = parent;
            IEnumerable<AssetMapComponentViewModel> childrenVMs = from child in component.Children
                                                                  select new AssetMapComponentViewModel(child, this);
            children = new BindableCollection<AssetMapComponentViewModel>(childrenVMs.OrderBy(c => c.Order).ToList());
            CanEditName = true;
            PropertyCount = Component.Properties.Count;
        }

        public AssetMapComponent Component { get; private set; }

        public AssetMapComponentViewModel Parent { get; private set; }

        public string Code
        {
            get { return Component.Code; }
            set { Component.Code = value; }
        }

        public string Name
        {
            get
            {
                if (Component.IsRoot)
                    return "(Root)";
                return Component.Name;
            }
            set
            {
                if (Component.IsRoot) return;
                if (Name != value) Component.Name = value;
            }
        }

        public int Order
        {
            get { return Component.Order; }
            set { Component.Order = value; }
        }

        public bool WillBeDeleted
        {
            get { return willBeDeleted; }
            set
            {
                if (value.Equals(willBeDeleted)) return;
                willBeDeleted = value;
                Notify();
            }
        }

        public int Id { get; set; }

        public BindableCollection<AssetMapComponentViewModel> Children
        {
            get { return children; }
        }

        public bool IsSelected
        {
            get { return isSelected; }
            set { SetNotify(ref isSelected, value); }
        }

        public bool IsExpanded
        {
            get { return isExpanded; }
            set
            {
                if (SetNotify(ref isExpanded, value) && Parent != null)
                {
                    Parent.IsExpanded = true;
                }
            }
        }

        public bool IsVisible
        {
            get { return isVisible; }
            set { SetNotify(ref isVisible, value); }
        }

        public int PropertyCount
        {
            get { return propertyCount; }
            set { SetNotify(ref propertyCount, value); }
        }

        public int Level
        {
            get { return GetLevel(this); }
        }

        public bool IsLinkedToCompositeBenchmarkItem { get; set; }

        public bool CanEditName
        {
            get { return canEditName; }
            set { SetNotify(ref canEditName, value); }
        }

        public string CanDeleteMessage
        {
            get { return Component.IsRoot ? string.Empty : IsLinkedToCompositeBenchmarkItem ? "NO" : "YES"; }
        }

        public void SelectByEditor(AssetMapComponentViewModel vm, KeyboardFocusChangedEventArgs a)
        {
            if (a.OldFocus != null && a.NewFocus != a.OldFocus)
            {
                vm.IsSelected = true;
            }
        }

        private int GetLevel(AssetMapComponentViewModel parentComp)
        {
            if (parentComp.Parent != null)
            {
                return GetLevel(parentComp.Parent) + 1;
            }
            return 0;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}