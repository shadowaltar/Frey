using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Maintenance.Benchmarks.Entities;
using Maintenance.Common.Entities;
using Maintenance.Common.Utils;

namespace Maintenance.Benchmarks.ViewModels
{
    public partial class MainViewModel
    {
        private async void Load()
        {
            allBenchmarks.Clear();
            allIndexes.Clear();
            benchmarkToIndexMappings.Clear();

            bool loadDataResult;
            var progress = await ViewService.ShowProgress("Loading",
                "Data is being loaded from the database.");
            try
            {
                await TaskEx.WhenAll(LoadIndexes(), LoadBenchmarks());

                MergeData();
                Benchmarks.ClearAndAddRange(allBenchmarks.Values.OrderBy(b => b.Code).ThenBy(b => b.Name));

                CanToggleFilter = true;
                CanToggleAdd = true;
                // reset the ready flag for flyouts
                AddFlyout.IsReady = false;
                EditFlyout.IsReady = false;
                FilterFlyout.IsReady = false;

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
                CanSelectBenchmarks = true;
            }
            else
            {
                CanSelectBenchmarks = true;
                await ViewService.ShowError("Cannot read from database. You are not able to perform any action.");
            }
        }

        private Task LoadBenchmarks()
        {
            return TaskEx.Run(() =>
            {
                DataTable table;
                using (var access = DataAccessFactory.New())
                {
                    table = access.GetBenchmarks();
                }

                foreach (DataRow row in table.Rows)
                {
                    var benchmark = new Benchmark
                    {
                        Id = row["ID"].ConvertLong(),
                        Code = row["CODE"].ConvertString(),
                        Name = row["CODE"].ConvertString(), // todo there is no "Name" column for bmks..
                        Type = row["TYPE"].ConvertString()
                    };
                    allBenchmarks[benchmark.Id] = benchmark;
                    var indexId = row["IID"].ConvertLong(-1);
                    if (indexId != -1)
                        benchmarkToIndexMappings[benchmark.Id] = indexId;
                }
            });
        }

        private Task LoadIndexes()
        {
            return TaskEx.Run(() =>
            {
                DataTable table;
                using (var access = DataAccessFactory.New())
                {
                    table = access.GetIndexes();
                }

                foreach (DataRow row in table.Rows)
                {
                    var index = new Index
                    {
                        Id = row["ID"].ConvertLong(),
                        Code = row["CODE"].ConvertString(),
                        Name = row["NAME"].ConvertString(),
                    };
                    allIndexes[index.Id] = index;
                }
            });
        }

        private Task<List<BenchmarkDependency>> LoadBenchmarkDependencies()
        {
            return TaskEx.Run(() =>
            {
                DataTable table;
                using (var access = DataAccessFactory.New())
                {
                    table = access.GetBenchmarkDependencies(SelectedBenchmark.Code);
                }
                var dependencies = new List<BenchmarkDependency>();
                foreach (DataRow row in table.Rows)
                {
                    var dependency = new BenchmarkDependency
                    {
                        Type = row["TYPE"].ParseEnum<BenchmarkDependencyType>(),
                        Code = row["CODE"].ConvertString(),
                        Name = row["NAME"].ConvertString(),
                    };
                    dependencies.Add(dependency);
                }
                return dependencies;
            });
        }

        /// <summary>
        /// Assign the indexes into benchmarks, after loaded from db.
        /// </summary>
        private void MergeData()
        {
            foreach (var mapping in benchmarkToIndexMappings)
            {
                var bmid = mapping.Key;
                var iid = mapping.Value;
                if (!allIndexes.ContainsKey(iid))
                {
                    RegisterMissingIndex(iid);
                }
                allBenchmarks[bmid].BasedOn = allIndexes[iid];
            }
        }
    }
}