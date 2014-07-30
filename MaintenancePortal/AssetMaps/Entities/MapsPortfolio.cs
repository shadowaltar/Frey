using Maintenance.Common.Entities;

namespace Maintenance.AssetMaps.Entities
{
    /// <summary>
    /// Portfolio class dedicated for MAPS.
    /// </summary>
    public class MapsPortfolio : Portfolio
    {
        public PortfolioAssetMapLink PortfolioAssetMapLink { get; set; }

        public AssetMap AssetMap
        {
            get
            {
                return PortfolioAssetMapLink == null
                    ? null : PortfolioAssetMapLink.AssetMap;
            }
        }
    }
}
