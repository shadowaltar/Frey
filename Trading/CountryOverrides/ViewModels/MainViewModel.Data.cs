using MahApps.Metro.Controls.Dialogs;
using Trading.Common.Entities;
using Trading.Common.Utils;
using Trading.CountryOverrides.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Trading.CountryOverrides.ViewModels
{
    public partial class MainViewModel
    {
        /// <summary>
        /// Progress controller for the progress indicator overlay on main UI.
        /// Don't use it unless for changing the message for the indicator already visible.
        /// </summary>
        private ProgressDialogController progress;

        private async void Load()
        {
            CanSelectOverrideItems = false;
            selectedItems = null;
            SelectedOverrideItem = null;

            CanToggleAdd = false;
            CanToggleEdit = false;
            CanToggleFilter = false;
            CanDelete = false;

            bool result;
            progress = await ViewService.ShowLoading();

            try
            {
                await TaskEx.WhenAll(LoadCountries(), LoadOverrides(), LoadPortfoliosAndManagers());
                MergeData();
                OverrideItems.ClearAndAddRange(allItems);
                orderedCountries = countries.Values.OrderBy(c => c.Code).ToList();
                orderedPortfolios = portfolios.Values.OrderBy(c => c.Code).ToList();
                orderedPortfolioManagers = portfolioManagers.Values.OrderBy(c => c.Name).ToList();

                CanToggleFilter = true;
                CanToggleAdd = true;
                // reset the ready flag for flyouts
                AddFlyout.IsReady = false;
                EditFlyout.IsReady = false;
                FilterFlyout.IsReady = false;

                result = true;
            }
            catch (Exception e)
            {
                Log.Error("Error occurs when loading data from database.", e);
                result = false;
            }

            await progress.Stop();
            if (result)
            {
                CanSelectOverrideItems = true;
            }
            else
            {
                CanSelectOverrideItems = false;
                await ViewService.ShowError("Cannot read from database. You are not able to perform any action.");
            }
        }

        private Task LoadOverrides()
        {
            return TaskEx.Run(() =>
            {
                var items = new List<OverrideItem>();
                DataTable result;
                using (var da = DataAccessFactory.New())
                {
                    result = da.GetOverrides();
                }
                progress.AppendMessageForLoading("Country override entries are loaded.");

                foreach (DataRow row in result.Rows)
                {
                    var item = new OverrideItem
                    {
                        Name = row["NAME"].ConvertString(),
                        FullName = row["FULLNAME"].ConvertString(),
                        Cusip = row["CUSIP"].ConvertString(),
                        Sedol = row["SEDOL"].ConvertString(),
                    };
                    // assign unique key to entry; it combines the TE id and override type.
                    var id = row["ID"].ConvertLong();
                    var type = row["TYPE"].ParseEnum<OverrideType>();
                    var pid = row["PORTFOLIOID"].ConvertLong();
                    var pmid = row["PMID"].ConvertLong();
                    var uid = new OverrideKey(id, type, pid, pmid);
                    item.UniqueKey = uid;

                    // right now the override items doesn't have country assigned yet;
                    // will be done after the country info is also loaded from db.

                    // only save the ctry codes into a hashmap now
                    oldCountryCodes[uid] = row["OLDCOUNTRYCODE"].ConvertString();
                    newCountryCodes[uid] = row["NEWCOUNTRYCODE"].ConvertString();
                    // similar to the pid and pmid but only those != 0
                    if (pid != 0)
                        portfolioMapping[uid] = pid;
                    if (pmid != 0)
                        portfolioManagerMapping[uid] = pmid;

                    if (!items.Contains(item))
                    {
                        items.Add(item);
                    }
                }

                SetPrecedences(items);
                allItems = items;
            });
        }

        private Task LoadCountries()
        {
            return TaskEx.Run(() =>
            {
                DataTable result;
                using (var da = DataAccessFactory.New())
                {
                    result = da.GetAllCountries();
                }
                progress.AppendMessageForLoading("Countries are loaded.");

                if (result != null)
                {
                    foreach (DataRow row in result.Rows)
                    {
                        var country = new Country
                        {
                            Code = row["CODE"].ConvertString(),
                            Name = row["NAME"].ConvertString(),
                        };
                        countries[country.Code] = country;
                    }
                }
            });
        }

        private Task LoadPortfoliosAndManagers()
        {
            return TaskEx.Run(() =>
            {
                DataTable table;
                using (var access = DataAccessFactory.New())
                {
                    table = access.GetAllPortfoliosAndManagers();
                }
                progress.AppendMessageForLoading("Portfolio and managers are loaded.");

                foreach (DataRow row in table.Rows)
                {
                    var portfolio = new Portfolio
                    {
                        Id = row["PID"].ConvertLong(),
                        Name = row["NAME"].ConvertString(),
                        Code = row["CODE"].ConvertString(),
                    };
                    portfolios[portfolio.Id] = portfolio;

                    var pmid = row["PMID"].ConvertLong();
                    if (pmid != 0)
                    {
                        PortfolioManager pm;
                        if (!portfolioManagers.TryGetValue(pmid, out pm))
                        {
                            pm = new PortfolioManager
                            {
                                Id = row["PMID"].ConvertLong(),
                                Name = row["PMNAME"].ConvertString(),
                                AId = row["PMAID"].ConvertString(),
                            };
                            portfolioManagers[pm.Id] = pm;
                        }
                        portfolio.PortfolioManager = pm;
                    }
                }
            });
        }

        /// <summary>
        /// Create temporary country generated from a code which is not
        /// defined in database.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private Country RegisterMissingCountry(string code)
        {
            var country = new Country { Code = code, Name = code + " (Custom)" };
            countries[code] = country;
            return country;
        }

        /// <summary>
        /// Create temporary pm generated from the person's id which is not
        /// defined in database.
        /// </summary>
        /// <param name="pId"></param>
        /// <returns></returns>
        private Portfolio RegisterMissingPortfolio(long pId)
        {
            var pm = new Portfolio { Id = pId, Name = pId + " (Missing)" };
            portfolios[pId] = pm;
            return pm;
        }

        /// <summary>
        /// Create temporary pm generated from the person's id which is not
        /// defined in database.
        /// </summary>
        /// <param name="pmId"></param>
        /// <returns></returns>
        private PortfolioManager RegisterMissingPortfolioManager(long pmId)
        {
            var pm = new PortfolioManager { Id = pmId, Name = pmId + " (Missing)" };
            portfolioManagers[pmId] = pm;
            return pm;
        }

        /// <summary>
        /// Merge the country, pm and porfolio data into override entries.
        /// Also handles any missing country entries.
        /// </summary>
        private void MergeData()
        {
            // merge into 'allItems' but not 'OverrideItems'; this is the backup of
            // original db data; 'OverrideItems' is affected by filtering.
            foreach (var item in allItems)
            {
                var key = item.UniqueKey;

                // assign old & new country
                var code = oldCountryCodes[key];
                Country c;
                if (!countries.TryGetValue(code, out c))
                {
                    c = RegisterMissingCountry(code);
                }
                code = newCountryCodes[key];
                item.OldCountry = c;
                if (!countries.TryGetValue(code, out c))
                {
                    c = RegisterMissingCountry(code);
                }
                item.NewCountry = c;

                // assign pm if any; create if not defined in db.
                long pmId;
                if (portfolioManagerMapping.TryGetValue(key, out pmId))
                {
                    PortfolioManager pm;
                    if (!portfolioManagers.TryGetValue(pmId, out pm))
                    {
                        pm = RegisterMissingPortfolioManager(pmId);
                    }
                    item.PortfolioManager = pm;
                }

                // assign ptf if any; create if not defined in db.
                long pId;
                if (portfolioMapping.TryGetValue(key, out pId))
                {
                    Portfolio p;
                    if (!portfolios.TryGetValue(pId, out p))
                    {
                        p = RegisterMissingPortfolio(pmId);
                    }
                    item.Portfolio = p;
                }
            }
        }

        /// <summary>
        /// Set the precedence of items for different levels.
        /// Level precedence is FILC - ALL - PM - PORTFOLIO (low to high).
        /// If ALL exists, FILC = Overridden;
        /// if PORTFOLIO exists, PORTFOLIO = Final, PM = Ambiguous, ALL = Ambiguous, FILC = Overridden;
        /// if PORTFOLIO doesn't exists, PM = Final, ALL = Ambiguous, FILC = Overridden;
        /// if PM doesn't exists, ALL = Final, FILC = Overridden;
        /// if PORTFOLIO/PM exists but ALL doesn't exists, PORTFOLIO/PM = Final/Ambigous, FILC = Overridden.
        /// </summary>
        /// <param name="items"></param>
        private static void SetPrecedences(IEnumerable<OverrideItem> items)
        {
            var groupOfItems = from item in items
                               group item by item.Id into groups
                               select new { groups.Key, Value = groups };

            foreach (var p in groupOfItems)
            {
                if (p.Value.Count() == 1)
                    continue;

                // any at ptf-lv would be the 'final' item
                var ptfLevel = p.Value.FirstOrDefault(i => i.Type == OverrideType.PORTFOLIO);
                if (ptfLevel != null)
                    ptfLevel.ItemPrecedence = ItemPrecedence.Final;

                // any at pm-lv (no ptf) would be the 'final' item
                var pmLevel = p.Value.FirstOrDefault(i => i.Type == OverrideType.PM);
                if (ptfLevel == null && pmLevel != null)
                    pmLevel.ItemPrecedence = ItemPrecedence.Final;
                if (ptfLevel != null && pmLevel != null)
                    pmLevel.ItemPrecedence = ItemPrecedence.Ambiguous;

                // only allow one at all-lv to be the 'final', if pm/ptf-lv = null
                var allLevel = p.Value.FirstOrDefault(i => i.Type == OverrideType.ALL);
                if (ptfLevel == null && pmLevel == null && allLevel != null)
                    allLevel.ItemPrecedence = ItemPrecedence.Final;
                if ((ptfLevel != null || pmLevel != null) && allLevel != null)
                    allLevel.ItemPrecedence = ItemPrecedence.Ambiguous;

                var filcLevel = p.Value.FirstOrDefault(i => i.Type == OverrideType.FILC);
                // must be overridden in filc-lv if all-lv != null
                // if all-lv is null but pm/ptf-lv != null, ambiguous
                if (filcLevel != null)
                {
                    if (allLevel != null)
                        filcLevel.ItemPrecedence = ItemPrecedence.Overridden;
                    else if (ptfLevel != null || pmLevel != null)
                        filcLevel.ItemPrecedence = ItemPrecedence.Ambiguous;
                    else
                        filcLevel.ItemPrecedence = ItemPrecedence.Final;
                }
            }
        }
    }
}