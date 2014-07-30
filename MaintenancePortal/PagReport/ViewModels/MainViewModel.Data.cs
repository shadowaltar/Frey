using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using MahApps.Metro.Controls.Dialogs;
using Maintenance.Common.Entities;
using Maintenance.Common.Utils;
using Maintenance.PagReport.Entities;

namespace Maintenance.PagReport.ViewModels
{
    public partial class MainViewModel
    {
        /// <summary>
        /// Progress controller for the progress indicator overlay on main UI.
        /// Don't use it unless for changing the message for the indicator already visible.
        /// </summary>
        private ProgressDialogController progress;

        public async void Load()
        {
            allPagIndexes.Clear();
            allPagPortfolios.Clear();
            allHeadStockOverrides.Clear();
            allBarraIdOverrides.Clear();

            bool loadDataResult;
            progress = await ViewService.ShowLoading();
            try
            {
                await TaskEx.WhenAll(LoadPagIndexes(), LoadPagPortfolios(),
                    LoadHeadStockOverrides(), LoadBarraOverrides());

                loadDataResult = true;

                Indexes.ClearAndAddRange(allPagIndexes.Values.OrderBy(i => i.Code));
                Portfolios.ClearAndAddRange(allPagPortfolios.Values.OrderBy(p => p.Code));
                HeadStockOverrides.ClearAndAddRange(allHeadStockOverrides.Values.OrderBy(h => h.Id).ThenBy(h => h.Type));
                BarraIdOverrides.ClearAndAddRange(allBarraIdOverrides.Values.OrderBy(h => h.Id).ThenBy(h => h.Type));
            }
            catch (Exception e)
            {
                Log.Error("Error occurs when loading data from database.", e);
                loadDataResult = false;
            }
            await progress.Stop();
            if (loadDataResult)
            {
                IsTabsEnabled = true;
            }
            else
            {
                IsTabsEnabled = false;
                await ViewService.ShowError("Cannot read from database. You are not able to perform any action.");
            }
        }

        private Task LoadPagIndexes()
        {
            return TaskEx.Run(() =>
            {
                DataTable table;
                using (var access = DataAccessFactory.New())
                {
                    table = access.GetAllPagIndexes();
                }
                progress.AppendMessageForLoading("PAG indexes are loaded.");

                foreach (DataRow row in table.Rows)
                {
                    var index = new PagIndex
                    {
                        Id = row["ID"].ConvertLong(),
                        Code = row["CODE"].ConvertString().Trim(),
                        Name = row["NAME"].ConvertString().Trim(),
                        IsInHeadStockReport = row["IS_HS_REPORT"].ConvertInt() == 1,
                        IsInBarraRegional = row["IS_BARRA_REGIONAL"].ConvertInt() == 1,
                        IsInBarraJapan = row["IS_BARRA_JAPAN"].ConvertInt() == 1,
                        IsInBarraAustralia = row["IS_BARRA_AUSTRALIA"].ConvertInt() == 1,
                    };
                    allPagIndexes[index.Id] = index;
                }
            });
        }

        private Task LoadPagPortfolios()
        {
            return TaskEx.Run(() =>
            {
                DataTable table;
                using (var access = DataAccessFactory.New())
                {
                    table = access.GetAllPagPortfolios();
                }
                progress.AppendMessageForLoading("PAG portfolios are loaded.");

                foreach (DataRow row in table.Rows)
                {
                    var ptf = new PagPortfolio
                    {
                        Id = row["ID"].ConvertLong(),
                        Code = row["CODE"].ConvertString().Trim(),
                        Name = row["NAME"].ConvertString().Trim(),
                        IsInHeadStockReport = row["IS_HS_REPORT"].ConvertInt() == 1,
                        IsInBarraRegional = row["IS_BARRA_REGIONAL"].ConvertInt() == 1,
                        IsInBarraJapan = row["IS_BARRA_JAPAN"].ConvertInt() == 1,
                        IsInBarraAustralia = row["IS_BARRA_AUSTRALIA"].ConvertInt() == 1,
                    };
                    allPagPortfolios[ptf.Id] = ptf;
                }
            });
        }

        private Task LoadHeadStockOverrides()
        {
            return TaskEx.Run(() =>
            {
                DataTable table;
                using (var access = DataAccessFactory.New())
                {
                    table = access.GetAllHeadStockOverrides();
                }
                progress.AppendMessageForLoading("HeadStock overrides are loaded.");

                foreach (DataRow row in table.Rows)
                {
                    var type = EnumMappings.HeadStockOverrideTypes[row["TYPE"].ConvertString()];
                    var id = row["ID1"].ConvertLong();
                    var hso = new HeadStockOverride(id, type)
                    {
                        Name = row["NAME1"].ConvertString().Trim(),
                        Cusip = row["CUSIP1"].ConvertString().Trim(),
                        Sedol = row["SEDOL1"].ConvertString().Trim(),
                        NewSecurity = new Security
                        {
                            Id = row["ID2"].ConvertLong(),
                            Name = row["NAME2"].ConvertString().Trim(),
                            Cusip = row["CUSIP2"].ConvertString().Trim(),
                            Sedol = row["SEDOL2"].ConvertString().Trim(),
                        },
                    };
                    allHeadStockOverrides[hso.Key] = hso;
                }
            });
        }

        private Task LoadBarraOverrides()
        {
            return TaskEx.Run(() =>
            {
                DataTable table;
                using (var access = DataAccessFactory.New())
                {
                    table = access.GetAllBarraOverrides();
                }
                progress.AppendMessageForLoading("BARRA Id overrides are loaded.");

                foreach (DataRow row in table.Rows)
                {
                    var type = EnumMappings.BarraIdOverrideTypes[row["TYPE"].ConvertString()];
                    var id = row["ID"].ConvertLong();
                    var bio = new BarraIdOverride(id, type)
                    {
                        Name = row["NAME"].ConvertString().Trim(),
                        Cusip = row["CUSIP"].ConvertString().Trim(),
                        Sedol = row["SEDOL"].ConvertString().Trim(),
                        BarraId = row["BARRA_ID"].ConvertString().Trim(),
                    };
                    allBarraIdOverrides[bio.Key] = bio;
                }
            });
        }
    }
}