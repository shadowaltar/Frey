using MahApps.Metro.Controls.Dialogs;
using Maintenance.Common.Entities;
using Maintenance.Common.Utils;
using Maintenance.Portfolios.Entities;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Portfolio = Maintenance.Portfolios.Entities.Portfolio;

namespace Maintenance.Portfolios.ViewModels
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
            CanToggleEditPortfolio = false;
            CanToggleBenchmarkAssociation = false;
            CanToggleFilter = false;

            allPortfolios.Clear();
            allBenchmarks.Clear();
            allPortfolioManagers.Clear();
            allInstruments.Clear();
            allCountries.Clear();
            allSectorSchemes.Clear();
            assetClassFocusMapping.Clear();

            bool loadDataResult;
            progress = await ViewService.ShowLoading();
            try
            {
                await TaskEx.WhenAll(LoadPortfolios(), LoadInstrumentAssetClassFocuses(),
                    LoadIndexes(), LoadCountries(), LoadSectorSchemes());
                await TaskEx.WhenAll(LoadBenchmarks(), LoadPortfolioExtendedInfo());

                SortBenchmarks();
                AssociateAssetClassFocus();
                PopulatePortfolios(allPortfolios.Values);

                CanToggleFilter = true;
                // reset the ready flag for flyouts
                EditFlyout.IsReady = false;
                BenchmarkAssociationFlyout.IsReady = false;
                FilterFlyout.IsReady = false;

                loadDataResult = true;
            }
            catch (Exception e)
            {
                Log.Error("Error occurs when loading data from database.", e);
                loadDataResult = false;
            }
            await progress.Stop();
            if (loadDataResult)
            {
                IsPortfoliosEnabled = true;
            }
            else
            {
                IsPortfoliosEnabled = false;
                await ViewService.ShowError("Cannot read from database. You are not able to perform any action.");
            }
        }

        /// <summary>
        /// Only loads 'IMAP' benchmarks.
        /// </summary>
        /// <returns></returns>
        private Task LoadBenchmarks()
        {
            return TaskEx.Run(() =>
            {
                DataTable table;
                using (var access = DataAccessFactory.New())
                {
                    table = access.GetAllBenchmarks();
                }
                progress.AppendMessageForLoading("Benchmark relationships are loaded.");

                for (int i = 0; i < table.Rows.Count; i++)
                {
                    var row = table.Rows[i];
                    var type = row["TYPE"].ConvertString();

                    // ignore non IMAP bmks.
                    if (type != Common.Constants.Imap)
                    {
                        continue;
                    }

                    var fakeId = i + 1;
                    var bmk = new Benchmark
                    {
                        Id = fakeId,
                        EffectiveDate = row["EFFECTIVE"].ConvertDate(),
                        ExpiryDate = row["EXPIRY"].ConvertDate(),
                        Type = row["TYPE"].ConvertString(),
                    };

                    Portfolio ptf;
                    var pid = row["PID"].ConvertLong();
                    if (allPortfolios.TryGetValue(pid, out ptf))
                    {
                        ptf.Benchmarks.Add(bmk);

                        Index index;
                        long iid = row["IID"].ConvertLong();
                        if (allIndexes.TryGetValue(iid, out index))
                        {
                            bmk.Index = index;
                        }
                    }
                }
            });
        }

        private Task LoadInstrumentAssetClassFocuses()
        {
            return TaskEx.Run(() =>
            {
                DataTable table;
                using (var access = DataAccessFactory.New())
                {
                    table = access.GetInstrumentAssetClassFocuses();
                }
                progress.AppendMessageForLoading("Asset class focuses as of instruments are loaded.");

                foreach (DataRow row in table.Rows)
                {
                    var instrument = new Instrument
                    {
                        Id = row["ID"].ConvertLong(),
                        Name = row["NAME"].ConvertString(),
                        AssetClassFocus = row["CLASS"].ConvertString(),
                    };
                    allInstruments[instrument.Id] = instrument;
                    var pid = row["PID"].ConvertLong();
                    assetClassFocusMapping[instrument.Id] = pid;
                }
            });
        }

        private Task LoadPortfolios()
        {
            return TaskEx.Run(() =>
            {
                DataTable table;
                using (var access = DataAccessFactory.New())
                {
                    table = access.GetAllPortfoliosAndManagers();
                }
                progress.AppendMessageForLoading("Portfolios and managers are loaded.");

                foreach (DataRow row in table.Rows)
                {
                    var portfolio = new Portfolio
                    {
                        Id = row["PID"].ConvertLong(),
                        Code = row["CODE"].ConvertString(),
                        Name = row["NAME"].ConvertString(),
                    };

                    Portfolio p;
                    if (!allPortfolios.TryGetValue(portfolio.Id, out p))
                    {
                        allPortfolios[portfolio.Id] = portfolio;
                    }

                    // save the ptf-pm association; pm is not editable so it's loaded as a part of ptf data.
                    long pmid = row["PMID"].ConvertLong();
                    PortfolioManager pm;
                    if (!allPortfolioManagers.TryGetValue(pmid, out pm))
                    {
                        pm = new PortfolioManager
                        {
                            AId = row["PMAID"].ConvertString(),
                            Id = pmid,
                            Name = row["PMNAME"].ConvertString()
                        };
                        allPortfolioManagers[pmid] = pm;
                    }
                    portfolio.PortfolioManager = pm;
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
                    table = access.GetAllIndexes();
                }
                progress.AppendMessageForLoading("Indexes are loaded.");

                foreach (DataRow row in table.Rows)
                {
                    var index = new Index
                    {
                        Id = row["ID"].ConvertLong(),
                        Code = row["CODE"].ConvertString().Trim(),
                        Name = row["NAME"].ConvertString().Trim(),
                        ExpiryDate = row["EXPIRY"].ConvertDate(),
                    };
                    allIndexes[index.Id] = index;
                }
            });
        }

        private Task LoadCountries()
        {
            return TaskEx.Run(() =>
            {
                DataTable table;
                using (var access = DataAccessFactory.New())
                {
                    table = access.GetAllCountries();
                }
                progress.AppendMessageForLoading("Countries are loaded.");

                for (int i = 0; i < table.Rows.Count; i++)
                {
                    var row = table.Rows[i];
                    var fakeId = i + 1;
                    var location = new Location
                    {
                        Id = fakeId,
                        Code = row["CODE"].ConvertString().Trim(),
                        Name = row["NAME"].ConvertString().Trim(),
                        ExpiryDate = row["EXPIRY"].ConvertDate(),
                        IsCountry = true,
                    };
                    allCountries[location.Code] = location;
                }
            });
        }

        private Task LoadSectorSchemes()
        {
            return TaskEx.Run(() =>
            {
                DataTable table;
                using (var access = DataAccessFactory.New())
                {
                    table = access.GetAllSectorSchemes();
                }
                progress.AppendMessageForLoading("Sector schemes are loaded.");

                foreach (DataRow row in table.Rows)
                {
                    allSectorSchemes.Add(row[0].ConvertString());
                }
            });
        }

        private Task LoadPortfolioExtendedInfo()
        {
            return TaskEx.Run(() =>
            {
                DataTable table;
                using (var access = DataAccessFactory.New())
                {
                    table = access.GetAllPortfolioExtendedInfo();
                }
                progress.AppendMessageForLoading("Portfolio extension information is loaded.");

                foreach (DataRow row in table.Rows)
                {
                    var info = new PortfolioExtendedInfo
                    {
                        Id = row["ID"].ConvertLong(),
                        IsTopLevelFund = row["IS_TOP_LEVEL_FUND"].ConvertString() == "Y",
                        SectorScheme = row["SECTOR_SCHEME"].ConvertString(null),
                        AssetClassFocus = row["ASSET_CLASS_FOCUS"].ConvertString(null),
                    };
                    var locationCode = row["LOCATION"].ConvertString();
                    Location location;
                    if (allCountries.TryGetValue(locationCode, out location))
                    {
                        info.Location = location;
                    }
                    allExtendedInfo[info.Id] = info;
                }
            });
        }

        private void SortBenchmarks()
        {
            foreach (var pair in allPortfolios.Where(p => p.Value.Benchmarks.Count > 0))
            {
                Portfolio ptf = pair.Value;
                ptf.Benchmarks.Sort((b1, b2) => b2.EffectiveDate.CompareTo(b1.EffectiveDate));
            }
        }

        private void AssociateAssetClassFocus()
        {
            foreach (var pair in assetClassFocusMapping)
            {
                allPortfolios[pair.Value].AsOfInstruments.Add(allInstruments[pair.Key]);
            }

            foreach (var portfolio in allPortfolios.Values)
            {
                PortfolioExtendedInfo info;
                allExtendedInfo.TryGetValue(portfolio.Id, out info);
                portfolio.GenerateAssetClassFocuses(info);
            }
        }
    }
}