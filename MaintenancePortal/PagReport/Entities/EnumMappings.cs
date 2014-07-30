using System.Collections.Generic;

namespace Maintenance.PagReport.Entities
{
    internal static class EnumMappings
    {
        private static Dictionary<string, HeadStockOverrideType> headStockOverrideTypes
            = new Dictionary<string, HeadStockOverrideType>
            {
                {"NONE", HeadStockOverrideType.None},
                {"QFR", HeadStockOverrideType.QFR},
                {"BARRA_REGIONAL", HeadStockOverrideType.BarraRegional},
                {"BARRA_JAPAN", HeadStockOverrideType.BarraJapan},
                {"BARRA_AUSTRALIA", HeadStockOverrideType.BarraAustralia},
            };

        private static Dictionary<string, BarraIdOverrideType> barraIdOverrideTypes
            = new Dictionary<string, BarraIdOverrideType>
            {
                {"BARRA_REGIONAL", BarraIdOverrideType.BarraRegional},
                {"BARRA_JAPAN", BarraIdOverrideType.BarraJapan},
                {"BARRA_AUSTRALIA", BarraIdOverrideType.BarraAustralia},
            };

        internal static Dictionary<string, HeadStockOverrideType> HeadStockOverrideTypes
        {
            get
            {
                return headStockOverrideTypes;
            }
        }

        internal static Dictionary<string, BarraIdOverrideType> BarraIdOverrideTypes
        {
            get
            {
                return barraIdOverrideTypes;
            }
        }
    }
}