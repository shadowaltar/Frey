using Maintenance.Common.Entities;
using Maintenance.Common.Entities.Decorators;
using Maintenance.Common.ViewModels;

namespace Maintenance.CompositeBenchmarks.ViewModels
{
    public class AssetMapViewModel : ViewModelBaseSlim, ITreeNode
    {
        public AssetMapViewModel(AssetMap assetMap)
        {
            AssetMap = assetMap;
            IsVisible = true;
        }

        public AssetMap AssetMap { get; private set; }

        public string Name { get { return AssetMap.Name; } }
        public string Code { get { return AssetMap.Code; } }

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
    }
}