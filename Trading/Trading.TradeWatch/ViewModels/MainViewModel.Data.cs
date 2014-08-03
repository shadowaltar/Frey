
using System.Collections.Generic;
using System.Windows.Documents;
using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using Trading.Common.Entities;
using Trading.Common.Utils;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Trading.TradeWatch.ViewModels.Entities;

namespace Trading.TradeWatch.ViewModels
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
            IsSecurityLoaded = false;

            allSecurityViewModels = new HashSet<SecurityViewModel>();
            allMarkets.Clear();
            Securities.Clear();

            bool loadDataResult;
            progress = await ViewService.ShowLoading();
            try
            {
                await LoadMarkets();
                await LoadSecurities();

                FilterFlyout.IsReady = false;

                loadDataResult = true;
            }
            catch (Exception e)
            {
                Log.Error("Error occurs when loading data from database.", e);
                loadDataResult = false;
            }
            await progress.Stop();
            if (!loadDataResult)
            {
                await ViewService.ShowError("Cannot read from database. You are not able to perform any action.");
            }
        }

        private Task LoadMarkets()
        {
            return TaskEx.Run(() =>
               {
                   DataTable table;
                   using (var access = DataAccessFactory.New())
                   {
                       table = access.GetAllMarkets();
                   }
                   progress.AppendMessageForLoading("Data are loaded.");

                   foreach (DataRow row in table.Rows)
                   {
                       var market = new Market
                       {
                           Id = row["Id"].ConvertLong(),
                           Code = row["Acronym"].ConvertString(),
                           Name = row["Name"].ConvertString(),
                       };
                       allMarkets[market.Id] = market;
                   }
               });
        }

        /// <summary>
        /// Only loads 'IMAP' benchmarks.
        /// </summary>
        /// <returns></returns>
        private Task LoadSecurities()
        {
            return TaskEx.Run(() =>
            {
                using (var access = DataAccessFactory.New())
                {
                    foreach (var security in access.EnumerateAllSecurities(allMarkets))
                    {
                        var vm = new SecurityViewModel(security);

                        allSecurities[security(security);
                        Securities.Add(vm);
                    }
                }
                progress.AppendMessageForLoading("Data are loaded.");
            });
        }

        private async  void LoadPrices(string hk)
        {
          await  TaskEx.Run(() =>
            {
                using (var access = DataAccessFactory.New())
                {
                    foreach (var security in access.EnumeratePrices(allMarkets))
                    {
                        var vm = new SecurityViewModel(security);
                        allSecurityViewModels.Add(vm);
                        Securities.Add(vm);
                    }
                }
                progress.AppendMessageForLoading("Data are loaded.");
            });
            IsSecurityLoaded = true;
        }
    }
}