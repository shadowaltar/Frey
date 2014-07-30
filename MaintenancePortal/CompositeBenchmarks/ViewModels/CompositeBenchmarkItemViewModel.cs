using System.ComponentModel;
using System.Linq;
using Maintenance.Common.Entities;
using Maintenance.Common.Entities.Decorators;
using Maintenance.Common.Utils;
using Maintenance.Common.ViewModels;

namespace Maintenance.CompositeBenchmarks.ViewModels
{
    /// <summary>
    /// ViewModel for CompositeBenchmarkItemViewModel. It provides function IsSelected and IsExpanded
    /// </summary>
    public class CompositeBenchmarkItemViewModel : ViewModelBaseSlim, ITreeNode, IHasBindingChildNodes<CompositeBenchmarkItemViewModel>
    {
        public CompositeBenchmarkItemViewModel(CompositeBenchmarkItem compositeBenchmarkItem,
            CompositeBenchmarkItemViewModel parent)
        {
            compositeBenchmarkItem.ThrowIfNull("component", "Must provide a Component.");

            CanChangeWeight = false;
            CompositeBenchmarkItem = compositeBenchmarkItem;
            Parent = parent;
            Children = new BindingList<CompositeBenchmarkItemViewModel>();
            if (CompositeBenchmarkItem.Components.Count > 0)
            {
                foreach (var result in CompositeBenchmarkItem.Components
                    .Select(c => new CompositeBenchmarkItemViewModel(c, this)))
                {
                    Children.Add(result);
                }
            }
            IsVisible = true;
        }

        public CompositeBenchmarkItemViewModel Parent { get; private set; }
        public CompositeBenchmarkItem CompositeBenchmarkItem { get; private set; }
        public AssetMapComponent AssetMapComponent { get { return CompositeBenchmarkItem.AssetMapComponent; } }

        private bool isAggregated;
        /// <summary>
        /// Get/set for a component aggregation status. If it is true, it means as a parent node
        /// it has aggregated weight, instead of that this component itself has weight.
        /// </summary>
        public bool IsAggregated
        {
            get { return isAggregated; }
            set { SetNotify(ref isAggregated, value); }
        }

        private bool canChangeWeight;
        public bool CanChangeWeight
        {
            get { return canChangeWeight; }
            set { SetNotify(ref canChangeWeight, value); }
        }

        public bool IsReal
        {
            get { return CompositeBenchmarkItem.IsReal; }
            set
            {
                if (CompositeBenchmarkItem.IsReal != value)
                {
                    CompositeBenchmarkItem.IsReal = value;
                    Notify();
                }
            }
        }

        public Index Index
        {
            get { return CompositeBenchmarkItem.Index; }
            set
            {
                if (CompositeBenchmarkItem.Index != value)
                {
                    CanChangeWeight = !IsAggregated && value != null;
                    CompositeBenchmarkItem.Index = value;
                    Notify();
                }
            }
        }

        public decimal Weight
        {
            get
            {
                return IsAggregated ? ChildrenWeight : CompositeBenchmarkItem.Weight;
            }
            set
            {
                if (CompositeBenchmarkItem.Weight != value)
                {
                    CompositeBenchmarkItem.Weight = value;
                    Notify();
                }
            }
        }

        public decimal ChildrenWeight
        {
            get
            {
                if (Children.IsNullOrEmpty())
                {
                    return Weight;
                }
                return Children.Sum(c => c.IsAggregated ? c.ChildrenWeight : c.Weight);
            }
        }

        public BindingList<CompositeBenchmarkItemViewModel> Children { get; private set; }

        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set { SetNotify(ref isSelected, value); }
        }

        private bool isExpanded;
        public bool IsExpanded
        {
            get { return isExpanded; }
            set { SetNotify(ref isExpanded, value); }
        }

        private bool isVisible;
        public bool IsVisible
        {
            get { return isVisible; }
            set { SetNotify(ref isVisible, value); }
        }

        #region item changed event handler related

        public void BindListChangedForChildren(ListChangedEventHandler benchmarkComponentsListChanged)
        {
            Children.ListChanged += benchmarkComponentsListChanged;
            foreach (var bcvm in Children)
            {
                bcvm.BindListChangedForChildren(benchmarkComponentsListChanged);
            }
        }

        public void UnbindListChangedForChildren(ListChangedEventHandler benchmarkComponentsListChanged)
        {
            foreach (var bcvm in Children)
            {
                bcvm.UnbindListChangedForChildren(benchmarkComponentsListChanged);
            }
            Children.ListChanged -= benchmarkComponentsListChanged;
        }

        #endregion

        public void CheckParentAggregated()
        {
            if (Parent != null && !Parent.IsAggregated)
            {
                Parent.IsAggregated = Index != null;
            }
        }

        public override string ToString()
        {
            if (!IsAggregated)
                return AssetMapComponent.Name + ": "
                    + (Index != null ? Index.Code + " " + Weight : string.Empty);
            return AssetMapComponent.Name + ": "
                   + ("(Aggregated) " + Weight);
        }

        /// <summary>
        /// Check (and update the weight of) a node's parent, see whether it is "aggregated":
        /// either any of the children have benchmark assigned or is also "aggregated".
        /// </summary>
        public void CheckAncestorAggregation()
        {
            if (Parent != null)
            {
                var siblings = Parent.Children;
                if (siblings.Any(s => s.Index != null || s.IsAggregated))
                {
                    Parent.IsAggregated = true;
                    Parent.Weight = Parent.ChildrenWeight;
                }
                else // now its parent should not be isAggregated
                {
                    Parent.IsAggregated = false;
                    if (Parent.Index == null)
                    {
                        Parent.Weight = 0;
                    }
                }
                Parent.CheckAncestorAggregation();
            }
        }
    }
}