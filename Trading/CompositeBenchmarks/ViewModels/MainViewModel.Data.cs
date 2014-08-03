using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using MahApps.Metro.Controls.Dialogs;
using Maintenance.Common.Entities;
using Maintenance.Common.Utils;

namespace Maintenance.CompositeBenchmarks.ViewModels
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
            // reset selected items
            SelectedCompositeBenchmarkItem = null;
            SelectedPortfolio = null;
            SelectedPortfolioAssetMapLink = null;

            // reset entity caches
            allAssetMapComponents.Clear();
            allAssetMaps.Clear();
            allPortfolios.Clear();
            allIndexes.Clear();
            allLinks.Clear();
            assetMapComponentParents.Clear();
            links.Clear();

            // reset UI collections
            Portfolios.Clear();
            CompositeBenchmarkItems.Clear();


            bool loadDataResult;
            progress = await ViewService.ShowLoading();
            try
            {
                await TaskEx.WhenAll(LoadAssetMaps(), LoadLinks(), LoadPortfolios(), LoadIndexes());
                await LoadAssetMapComponents();
                await LoadCompositeBenchmarkItems();

                // merge the entities loaded from db to meaningful tree structures
                MergeStructures();

                Editor.IsReady = false;
                loadDataResult = true;
            }
            catch (Exception e)
            {
                Log.Error("Error occurs when loading data from database.", e);
                loadDataResult = false;
            }
            await progress.Stop();
            await ViewService.LightUp();
            if (loadDataResult)
            {
                Portfolios.Clear();
                var groupedLinks = allLinks.Values.GroupBy(l => l.Portfolio);
                foreach (var group in groupedLinks.OrderBy(g => g.Key.Code))
                {
                    var portfolio = group.Key;
                    Portfolios.Add(new PortfolioViewModel(portfolio, group));
                }
                ToggleShowCompositeBenchmarks();
            }
            else
            {
                await ViewService.ShowError("Cannot read from database. You are not able to perform any action.");
            }
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

                foreach (DataRow row in table.Rows)
                {
                    var index = new Index
                    {
                        Id = row["ID"].ConvertLong(),
                        Code = row["CODE"].Trim(),
                        Name = row["NAME"].Trim(),
                        ExpiryDate = row["EXPIRY"].ConvertDate(DateTime.MaxValue),
                    };
                    allIndexes[index.Id] = index;
                }
            });
        }

        private Task LoadAssetMaps()
        {
            return TaskEx.Run(() =>
            {
                DataTable table;
                using (var access = DataAccessFactory.New())
                {
                    table = access.GetAllAssetMaps();
                }
                progress.AppendMessageForLoading("Asset maps are loaded.");

                foreach (DataRow row in table.Rows)
                {
                    var asm = new AssetMap
                    {
                        Id = row["ID"].ConvertLong(),
                        Name = row["NAME"].ConvertString(),
                        Code = row["CODE"].ConvertString(),
                    };
                    allAssetMaps[asm.Id] = asm;
                }
            });
        }

        /// <summary>
        /// Load all portfolios, and find out the asm id plus the link id if any asm
        /// is attached to a portfolio.
        /// </summary>
        /// <returns></returns>
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
                    var ptf = new Portfolio
                    {
                        Id = row["PID"].ConvertLong(),
                        Name = row["NAME"].ToString(),
                        Code = row["CODE"].ToString(),
                        PortfolioManager = new PortfolioManager
                        {
                            Id = row["PMID"].ConvertLong(),
                            Name = row["PMNAME"].ToString(),
                            AId = row["PMAID"].ToString(),
                        }
                    };
                    allPortfolios[ptf.Id] = ptf;
                }
            });
        }

        private Task LoadLinks()
        {
            return TaskEx.Run(() =>
            {
                DataTable table;
                using (var access = DataAccessFactory.New())
                {
                    table = access.GetAllPortfolioAssetMapLinks();
                }
                progress.AppendMessageForLoading("Portfolio to asset map links are loaded.");

                foreach (DataRow row in table.Rows)
                {
                    // save the triple ids to be merged later.
                    var asmId = row["ASMID"].ConvertLong();
                    var linkId = row["LINKID"].ConvertLong();
                    var ptfId = row["PID"].ConvertLong();
                    if (asmId != 0 && linkId != 0)
                    {
                        links[linkId] = new Tuple<long, long>(ptfId, asmId);
                    }

                    allLinks[linkId] = new PortfolioAssetMapLink
                    {
                        Id = linkId,
                        AssetMap = null, // merge later
                        Portfolio = null, // merge later
                        IsDefault = row["ISDEFAULT"].ConvertString("N") == "Y",
                        CreateTime = row["CREATETIME"].ConvertIsoDateTime(),
                        Creator = row["CREATOR"].ToString(),
                        UpdateTime = row["UPDATETIME"].ConvertIsoDateTime(),
                        Updater = row["UPDATER"].ToString(),
                        EffectiveDate = row["EFFECTIVE"].ConvertIsoDate(),
                        ExpiryDate = row["EXPIRY"].ConvertIsoDate()
                    };
                }
            });
        }

        private Task LoadAssetMapComponents()
        {
            return TaskEx.Run(() =>
            {
                DataTable table;
                using (var access = DataAccessFactory.New())
                {
                    table = access.GetAllAssetMapComponents();
                }
                progress.AppendMessageForLoading("Asset map components are loaded.");

                foreach (DataRow row in table.Rows)
                {
                    var com = new AssetMapComponent
                    {
                        Id = row["ID"].ConvertLong(),
                        Code = row["CODE"].ToString(),
                        Name = row["NAME"].ToString(),
                        AssetMap = allAssetMaps[row["AID"].ConvertLong()],
                        // cannot parse the parent here.
                        Order = row["ORDER"].ConvertInt(),
                        IsRoot = row["PARENTID"].IsNullOrDBNull(),
                    };

                    allAssetMapComponents[com.Id] = com;

                    // save the parent-child relationship; merge them later.
                    assetMapComponentParents[com.Id] = row["PARENTID"].ConvertLong();
                }
            });
        }

        private Task LoadCompositeBenchmarkItems()
        {
            return TaskEx.Run(() =>
            {
                DataTable table;
                using (var access = DataAccessFactory.New())
                {
                    table = access.GetCompositeBenchmarkItems();
                }
                progress.AppendMessageForLoading("Composite benchmark items are loaded.");

                foreach (DataRow row in table.Rows)
                {
                    var linkId = row["LINKID"].ConvertLong();
                    PortfolioAssetMapLink link;
                    if (!allLinks.TryGetValue(linkId, out link))
                    {
                        continue;
                    }
                    var comp = new CompositeBenchmarkItem
                    {
                        Id = row["ID"].ConvertLong(),
                        AssetMapComponent = allAssetMapComponents[row["COMPID"].ConvertLong()],
                        PortfolioAssetMapLink = link,
                        Index = allIndexes[row["IID"].ConvertLong()],
                        Weight = row["WEIGHT"].ConvertDecimal(),
                        IsReal = true,
                        // Components are not handled here; it is happened after Merge(); bind parent and children together.
                    };
                    allBenchmarkComponents[comp.Id] = comp;
                }
            });
        }

        private void MergeStructures()
        {
            // construct the asm tree.
            foreach (var pair in assetMapComponentParents)
            {
                var childId = pair.Key;
                var parentId = pair.Value;
                AssetMapComponent parent;
                // here, child id's asmc is guaranteed to exist, but parent id's asmc is not
                // in db, an asm's root asmc has parent id = null.
                if (allAssetMapComponents.TryGetValue(parentId, out parent))
                {
                    allAssetMapComponents[childId].Parent = parent;
                    parent.Children.Add(allAssetMapComponents[childId]);
                }
            }

            // bind the tree to the asm instance.
            var roots = allAssetMapComponents.Values.Where(asmc => asmc.Parent == null);
            foreach (var root in roots)
            {
                var asm = allAssetMaps.Values.FirstOrDefault(a => a == root.AssetMap);
                if (asm != null)
                {
                    asm.RootComponent = root;
                }
            }

            // complete the ptf, asm and 'links' relationship.
            foreach (var pair in links)
            {
                var linkId = pair.Key;
                var ptfId = pair.Value.Item1;
                var asmId = pair.Value.Item2;
                var link = allLinks[linkId];
                link.AssetMap = allAssetMaps[asmId];
                link.Portfolio = allPortfolios[ptfId];
                // link.Components are handled below
            }

            ConstructBenchmarkComponentTrees();
        }

        private void ConstructBenchmarkComponentTrees()
        {
            foreach (var compositeBenchmark in allLinks.Values)
            {
                var firstLevelNodes = compositeBenchmark.AssetMap.RootComponent.Children;
                RecursiveAddBenchmarkComponent(firstLevelNodes, compositeBenchmark, compositeBenchmark);
            }
        }

        private void RecursiveAddBenchmarkComponent(IEnumerable<AssetMapComponent> asmComps,
            IHasChildCompositeBenchmarkItems parent, PortfolioAssetMapLink theComposite)
        {
            foreach (var assetMapComponent in asmComps)
            {
                var component = allBenchmarkComponents.Values
                    .FirstOrDefault(o => o.AssetMapComponent == assetMapComponent && o.PortfolioAssetMapLink == theComposite)
                    ?? CompositeBenchmarkItem.NewPlaceholder(assetMapComponent);

                parent.Components.Add(component);
                if (assetMapComponent.Children.Count > 0)
                {
                    RecursiveAddBenchmarkComponent(assetMapComponent.Children, component, theComposite);
                }
            }
        }
    }
}