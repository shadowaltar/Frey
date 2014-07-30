using Maintenance.Common.Entities;

namespace Maintenance.PagReport.Entities
{
    public class PagPortfolio : Portfolio
    {
        public bool IsInHeadStockReport { get; set; }
        public bool IsInBarraRegional { get; set; }
        public bool IsInBarraJapan { get; set; }
        public bool IsInBarraAustralia { get; set; }

        public override string ToString()
        {
            return string.Format("{0}, HeadStockReport: {1}, BarraRegional: {2}, BarraJapan: {3}, BarraAustralia: {4}", base.ToString(), IsInHeadStockReport, IsInBarraRegional, IsInBarraJapan, IsInBarraAustralia);
        }
    }
}