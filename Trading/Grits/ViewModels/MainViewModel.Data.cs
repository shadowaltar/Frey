using Maintenance.Common.Data;
using Maintenance.Common.Utils;
using Maintenance.Grits.Entities;
using Maintenance.Grits.Utils;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Maintenance.Grits.ViewModels
{
    public partial class MainViewModel
    {
        private async void Load()
        {
            CanToggleAdd = false;
            CanToggleEdit = false;

            var progress = await ViewService.ShowLoading();

            if (!FmsDataAccess.IsAuthenticated)
            {
                await TaskEx.Run(() => DataAccessFactory.Login());
            }
            else if (!FmsDataAccess.CanLogin)
            {
                await ViewService.ShowError("You are not allowed to perform anything about GRITS configurations.");
                await progress.Stop();
                return;
            }

            bool loadDataResult;
            try
            {
                await TaskEx.WhenAll(LoadGritsFunds(), LoadFunds(), LoadGritsBenchmarks(), LoadBenchmarks());

                MergeData();

                allGritsFunds = allGritsFunds.OrderBy(f => f.Code).ToList();
                allGritsBenchmarks = allGritsBenchmarks.OrderBy(b => b.Code).ToList();
                allFunds = allFunds.OrderBy(f => f.Code).ToList();
                allBenchmarks = allBenchmarks.OrderBy(b => b.Code).ToList();

                Funds.ClearAndAddRange(allGritsFunds);
                Benchmarks.ClearAndAddRange(allGritsBenchmarks);

                CanToggleAdd = true;
                // reset the ready flag for flyouts
                AddFlyout.IsReady = false;
                EditFundFlyout.IsReady = false;
                EditBenchmarkFlyout.IsReady = false;
                loadDataResult = true;
            }
            catch (Exception e)
            {
                Log.Error("Error occurs when loading data from database.", e);
                loadDataResult = false;
            }
            await progress.CloseAsync();

            if (loadDataResult)
            {
                CanSelectItems = true;
            }
            else
            {
                CanSelectItems = false;
                await ViewService.ShowError("Cannot read from database. You are not able to perform any action.");
            }
        }

        private Task LoadGritsFunds()
        {
            return TaskEx.Run(() =>
            {
                allGritsFunds.Clear();
                using (var da = DataAccessFactory.New())
                {
                    var result = da.GetGritsFunds();
                    if (result == null)
                        return;

                    foreach (DataRow row in result.Rows)
                    {
                        var f = new GritsFund
                        {
                            Id = row["FundId"].ConvertInt(),
                            Code = row["FundCode"].ConvertString().Trim(),
                            Mode = row["Mode"].Process(Helper.GetModeEnum),
                            Name = row["FundName"].ConvertString().Trim(),
                        };
                        allGritsFunds.AddIfNotExist(f);
                        fundToBenchmark[f.Id] = row["BmId"].ConvertInt();
                    }
                }
            });
        }

        private Task LoadFunds()
        {
            return TaskEx.Run(() =>
            {
                allFunds.Clear();
                using (var da = DataAccessFactory.New())
                {
                    var result = da.GetAllFunds();
                    if (result == null)
                        return;

                    foreach (DataRow row in result.Rows)
                    {
                        var f = new Fund
                        {
                            Id = row["FundId"].ConvertInt(),
                            Code = row["FundCode"].ConvertString().Trim(),
                            Name = row["FundName"].ConvertString().Trim(),
                        };
                        allFunds.AddIfNotExist(f);
                        fundToBenchmark[f.Id] = row["BmId"].ConvertInt();
                    }
                }
            });
        }

        private Task LoadGritsBenchmarks()
        {
            return TaskEx.Run(() =>
            {
                allGritsBenchmarks.Clear();
                using (var da = DataAccessFactory.New())
                {
                    var result = da.GetGritsBenchmarks();
                    if (result == null)
                        return;

                    foreach (DataRow row in result.Rows)
                    {
                        var b = new GritsBenchmark
                        {
                            Id = row["BmId"].ConvertInt(),
                            Code = row["BmCode"].ConvertString().Trim(),
                            Mode = row["Mode"].Process(Helper.GetModeEnum),
                            Name = row["BmName"].ConvertString().Trim(),
                            IsLoadedAtNight = row["IsLate"].ConvertInt() == 1,
                        };
                        allGritsBenchmarks.AddIfNotExist(b);
                    }
                }
            });
        }

        private Task LoadBenchmarks()
        {
            return TaskEx.Run(() =>
            {
                allBenchmarks.Clear();
                using (var da = DataAccessFactory.New())
                {
                    var result = da.GetAllBenchmarks();
                    if (result == null)
                        return;

                    foreach (DataRow row in result.Rows)
                    {
                        var b = new Benchmark
                        {
                            Id = row["BmId"].ConvertInt(),
                            Code = row["BmCode"].ConvertString().Trim(),
                            Name = row["BmName"].ConvertString().Trim(),
                        };
                        allBenchmarks.AddIfNotExist(b);
                    }
                }
            });
        }

        /// <summary>
        /// Assign the benchmark to the fund.
        /// </summary>
        private void MergeData()
        {
            // allBenchmarks contains only Active bmks, if already deactivated, find it in the GritsBenchmarks list.
            foreach (var fund in allFunds)
            {
                var benchmark = allBenchmarks.FirstOrDefault(b => b.Id == fundToBenchmark[fund.Id]) ??
                                allGritsBenchmarks.FirstOrDefault(b => b.Id == fundToBenchmark[fund.Id]);
                fund.Benchmark = benchmark;
            }
            foreach (var fund in allGritsFunds)
            {
                var benchmark = allBenchmarks.FirstOrDefault(b => b.Id == fundToBenchmark[fund.Id]) ??
                                allGritsBenchmarks.FirstOrDefault(b => b.Id == fundToBenchmark[fund.Id]);
                fund.Benchmark = benchmark;
            }
        }
    }
}