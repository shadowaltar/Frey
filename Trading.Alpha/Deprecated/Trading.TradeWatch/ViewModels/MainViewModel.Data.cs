
using System.Collections.Generic;
using MahApps.Metro.Controls.Dialogs;
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
        private readonly Random rand = new Random();

        /// <summary>
        /// Progress controller for the progress indicator overlay on main UI.
        /// Don't use it unless for changing the message for the indicator already visible.
        /// </summary>
        private ProgressDialogController progress;

        public async void Load()
        {
            IsSecurityLoaded = false;

            allMarkets.Clear();
            Securities.Clear();

            bool loadDataResult;
            progress = await ViewService.ShowLoading();
            try
            {
                await Task.WhenAll(LoadMarkets(), LoadSecurities());
                LoadRandomPerformanceData();

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
            return Task.Run(() =>
            {
                DataTable table;
                using (var access = DataAccessFactory.New())
                {
                    table = access.GetAllMarkets();
                }
                progress.AppendMessageForLoading("Markets are loaded.");

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
            return Task.Run(() =>
            {
                using (var access = DataAccessFactory.New())
                {
                    foreach (var security in access.EnumerateAllSecurities(allMarkets))
                    {
                        var vm = new SecurityViewModel(security);

                        allSecurities[security.Code] = (security);
                        Securities.Add(vm);
                    }
                }
                progress.AppendMessageForLoading("Securities are loaded.");
            });
        }

        private async Task<List<Price>> LoadPrices(string code, DateTime from, DateTime to)
        {
            if (from > to)
            {
                return null;
            }

            Security security;
            if (!allSecurities.TryGetValue(code, out security))
            {
                return null;
            }
            var span = TimeSpan.FromDays(1);

            var results = await Task.Run(() =>
            {
                using (var access = DataAccessFactory.New())
                {
                    return access.GetPrices(security, span, @from, to);
                }
            });
            return results;
        }

        private void LoadRandomPerformanceData()
        {
            HoldingsPerformances.Clear();
            HoldingsPerformances.Add(new HoldingPerformance
            {
                Security = Securities.FirstOrDefault(s => s.Code == "AAPL").Security,
                Price = new PriceViewModel { At = DateTime.Now.Date, Open = 94.26m, High = 94.82m, Low = 93.28m, Close = 94.74m },
                Position = new Position { Price = 93.3m, Quantity = 500, Time = DateTime.Now.AddDays(-1) },
                BloombergCode = "AAPL",
                PriceChange = 2.9m,
                PriceChangePercentage = 0.28m,
                Volume = 47.35e+6m,
                Weight = 15,
            });
            HoldingsPerformances.Add(new HoldingPerformance
            {
                Security = Securities.FirstOrDefault(s => s.Code == "GOOG").Security,
                Price = new PriceViewModel { At = DateTime.Now.Date, Open = 563.56m, High = 570.25m, Low = 560.35m, Close = 568.77m },
                Position = new Position { Price = 560.22m, Quantity = 20, Time = DateTime.Now.AddDays(-0.5) },
                BloombergCode = "GOOG",
                PriceChange = 5.03m,
                PriceChangePercentage = 0.96m,
                Volume = 1.57e+6m,
                Weight = 10,
            });
            HoldingsPerformances.Add(new HoldingPerformance
            {
                Security = Securities.FirstOrDefault(s => s.Code == "FB").Security,
                BloombergCode = "FB",
                Price = new PriceViewModel { At = DateTime.Now.Date, Open = 73.40m, High = 73.43m, Low = 72.56m, Close = 73.06m },
                Position = new Position { Price = 74.1m, Quantity = 100, Time = DateTime.Now.AddDays(-0.5) },
                PriceChange = -0.7m,
                PriceChangePercentage = -0.15m,
                Volume = 43.59e+6m,
                Weight = 10,
            });
            HoldingsPerformances.Add(new HoldingPerformance
            {
                Security = Securities.FirstOrDefault(s => s.Code == "CSCO").Security,
                BloombergCode = "FB",
                Price = new PriceViewModel { At = DateTime.Now.Date, Open = 22.02m, High = 23.11m, Low = 21.90m, Close = 22.02m },
                Position = new Position { Price = 74.1m, Quantity = 100, Time = DateTime.Now.AddDays(-0.5) },
                PriceChange = -0.7m,
                PriceChangePercentage = -0.15m,
                Volume = 43.59e+6m,
                Weight = 10,
            });

            PeriodicTaskFactory.Start(RandomPrice, 500);

            //PortfolioPerformanceItems.Clear();
            //PortfolioPerformanceItems.Add(new KeyValuePair<string, decimal>("CASH Wt%", 65m));
            //PortfolioPerformanceItems.Add(new KeyValuePair<string, decimal>("YTD Return", 0.1424m));
            //PortfolioPerformanceItems.Add(new KeyValuePair<string, decimal>("YTD Stdev", 0.2627m));
            //PortfolioPerformanceItems.Add(new KeyValuePair<string, decimal>("YTD Sharpe Ratio", 0.5420m));
            //PortfolioPerformanceItems.Add(new KeyValuePair<string, decimal>("YTD Skew", 0.2m));
            //PortfolioPerformanceItems.Add(new KeyValuePair<string, decimal>("YTD Kurtosis", 4.2m));
        }

        public void RandomPrice()
        {
            foreach (var holding in HoldingsPerformances)
            {
                var change = Convert.ToDecimal((rand.NextDouble() - 0.5) / 50);
                var currentPrice = holding.Price.Close;

                currentPrice *= (1 + change);

                holding.Price.Close = Math.Truncate(100 * currentPrice) / 100;

            }
           
        }
    }
}