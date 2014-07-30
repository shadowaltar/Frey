using Maintenance.AssetMaps.Entities;
using Maintenance.Common.Entities;
using Maintenance.Common.Utils;
using System.Collections.Generic;

namespace Maintenance.AssetMaps.ViewModels
{
    public interface IEditorViewModel : IHasDataAccessFactory<AssetMapDataAccess>
    {
        EditorViewModel.EditorMode Mode { get; set; }
        AssetMap AssetMap { get; set; }
        List<MapsPortfolio> RelatedPortfolios { get; set; }
        IEnumerable<AssetMap> AllAssetMaps { get; set; }
        IEnumerable<AssetMapComponent> AllAssetMapComponents { get; set; }

        /// <summary>
        /// Initialize the editor. Necessary info includes the mode of editor,
        /// (optional) the selected asset map, and
        /// (optional) the list of portfolios already using selected asset map (if any).
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="selectedAssetMap"></param>
        void Initialize(EditorViewModel.EditorMode mode, AssetMap selectedAssetMap = null);

        void CheckNonDeletableAsmComponent(HashSet<long> nonDeletableAsmComponentIds);
    }
}