using System.Collections.Generic;
using Caliburn.Micro;
using Maintenance.Common.Entities;
using Maintenance.Common.Utils;
using Maintenance.CompositeBenchmarks.Utils;

namespace Maintenance.CompositeBenchmarks.ViewModels
{
    public interface IEditorViewModel : IHasViewService,
        IHasDataAccessFactory<CompositeBenchmarkDataAccess>
    {
        bool IsReady { get; set; }
        BindableCollection<AssetMapViewModel> AssetMaps { get; }
        Dictionary<long, AssetMapComponent> AllComponents { get; set; }
        BindableCollection<Index> Indexes { get; }
        AssetMapViewModel SelectedAssetMap { get; set; }
        Portfolio SelectedPortfolio { get; set; }
        BindableCollection<Portfolio> Portfolios { get; }
        List<PortfolioAssetMapLink> AllPortfolioAssetMapLinks { get; }

        /// <summary>
        /// Sets an item to the editor dialog. It is called by main UI, or by this UI for a reset.
        /// If the link and ptf are provided, cb item values will be filled in.
        /// </summary>
        /// <param name="link"></param>
        /// <param name="portfolio"></param>
        void SetItem(PortfolioAssetMapLink link, Portfolio portfolio);

        /// <summary>
        /// Set the mode of editor. Without calling this, by default it is <see cref="EditorMode.Add"/>.
        /// </summary>
        /// <param name="mode"></param>
        void InitializeMode(EditorMode mode);
    }
}