using Caliburn.Micro;
using Maintenance.AssetMaps.Entities;
using Maintenance.Common.Data;
using Maintenance.Common.Entities;
using Maintenance.Common.Utils;
using System.Collections.Generic;

namespace Maintenance.AssetMaps.ViewModels
{
    public partial class EditorViewModel
    {
        private long nextId;

        public IViewService ViewService { get; set; }
        public IDataAccessFactory<AssetMapDataAccess> DataAccessFactory { get; set; }

        public EditorMode Mode { get; set; }
        public AssetMap AssetMap { get; set; }
        public AssetMap OriginalAssetMap { get; set; }

        /// <summary>
        /// Get/set the list of portfolios attached to current asm.
        /// </summary>
        public List<MapsPortfolio> RelatedPortfolios { get; set; }
        /// <summary>
        /// Get/set the list of all asm.
        /// </summary>
        public IEnumerable<AssetMap> AllAssetMaps { get; set; }
        /// <summary>
        /// Get/set the list of all asm components.
        /// </summary>
        public IEnumerable<AssetMapComponent> AllAssetMapComponents { get; set; }

        private readonly BindableCollection<AssetMapComponentViewModel> assetMapComponents = new BindableCollection<AssetMapComponentViewModel>();
        public BindableCollection<AssetMapComponentViewModel> AssetMapComponents { get { return assetMapComponents; } }

        private readonly BindableCollection<ComponentPropertyViewModel> componentProperties = new BindableCollection<ComponentPropertyViewModel>();
        public BindableCollection<ComponentPropertyViewModel> ComponentProperties { get { return componentProperties; } }

        private AssetMapComponentViewModel selectedComponent;
        public AssetMapComponentViewModel SelectedComponent
        {
            get { return selectedComponent; }
            set
            {
                if (SetNotify(ref selectedComponent, value))
                {
                }
            }
        }

        public string AssetMapCode
        {
            get { return AssetMap == null ? string.Empty : AssetMap.Code; }
            set
            {
                if (AssetMap != null)
                {
                    AssetMap.Code = value;
                }
                NotifyOfPropertyChange("AssetMapCode");
            }
        }

        public string AssetMapName
        {
            get { return AssetMap == null ? string.Empty : AssetMap.Name; }
            set
            {
                if (AssetMap != null)
                {
                    AssetMap.Name = value;
                }
                NotifyOfPropertyChange("AssetMapName");
            }
        }

        private ComponentPropertyViewModel selectedComponentProperty;
        public ComponentPropertyViewModel SelectedComponentProperty
        {
            get { return selectedComponentProperty; }
            set
            {
                if (SetNotify(ref selectedComponentProperty, value))
                {
                    CanDeleteProperty = value != null;
                }
            }
        }

        private bool canAddChild;
        public bool CanAddChild
        {
            get { return canAddChild; }
            set { SetNotify(ref canAddChild, value); }
        }

        private bool canDelete;
        public bool CanDelete
        {
            get { return canDelete; }
            set { SetNotify(ref canDelete, value); }
        }

        private bool canMoveUp;
        public bool CanMoveUp
        {
            get { return canMoveUp; }
            set { SetNotify(ref canMoveUp, value); }
        }

        private bool canMoveDown;
        public bool CanMoveDown
        {
            get { return canMoveDown; }
            set { SetNotify(ref canMoveDown, value); }
        }

        private bool canDeleteAssetMap;
        public bool CanDeleteAssetMap
        {
            get { return canDeleteAssetMap; }
            set { SetNotify(ref canDeleteAssetMap, value); }
        }

        private bool canAddProperty;
        public bool CanAddProperty
        {
            get { return canAddProperty; }
            set { SetNotify(ref canAddProperty, value); }
        }

        private bool canDeleteProperty;
        public bool CanDeleteProperty
        {
            get { return canDeleteProperty; }
            set { SetNotify(ref canDeleteProperty, value); }
        }

        private bool canUndoAll;
        public bool CanUndoAll
        {
            get { return canUndoAll; }
            set { SetNotify(ref canUndoAll, value); }
        }

        public enum EditorMode
        {
            Create,
            Edit,
        }
    }
}