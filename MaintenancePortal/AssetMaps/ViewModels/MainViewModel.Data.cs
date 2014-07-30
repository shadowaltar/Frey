using MahApps.Metro.Controls.Dialogs;
using Maintenance.AssetMaps.Entities;
using Maintenance.Common.Entities;
using Maintenance.Common.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Maintenance.AssetMaps.ViewModels
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
            ClearEverything();

            bool loadDataResult;
            progress = await ViewService.ShowLoading();
            try
            {
                await TaskEx.WhenAll(LoadPortfoliosAndManagers(), LoadAssetMaps(),
                    LoadAssetMapComponents(), LoadAssetMapComponentProperty());
                await TaskEx.WhenAll(LoadPorfolioAssetMapLinks(), LoadNotDeletableAssetMapComponentIds());

                MergeEverything();
                uiAssetMaps.AddRange(allAssetMaps.Values.OrderBy(asm => asm.Name));
                loadDataResult = true;

                IsPortfolioListVisible = true;
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

        private Task LoadPortfoliosAndManagers()
        {
            return TaskEx.Run(() =>
            {
                DataTable portfolioTable;
                using (var access = DataAccessFactory.New())
                {
                    portfolioTable = access.GetAllPortfoliosAndManagers();
                }
                progress.SetMessage("Data is being loaded from the database."
                    + System.Environment.NewLine + "Portfolios and managers are loaded.");

                foreach (DataRow row in portfolioTable.AsEnumerable())
                {
                    PortfolioManager pm;
                    var pmid = row["PMID"].ConvertLong();
                    if (pmid == 0)
                    {
                        pm = null;
                    }
                    else
                    {
                        pm = new PortfolioManager
                        {
                            Id = row["PMID"].ConvertLong(),
                            Name = row["PMNAME"].Trim(),
                            AId = row["PMAID"].Trim(),
                        };
                    }

                    var portfolio = new MapsPortfolio
                    {
                        Code = row["CODE"].ToString(),
                        Name = row["NAME"].ToString(),
                        Id = row["PID"].ConvertLong(),
                        PortfolioManager = pm
                    };
                    allPortfolios[portfolio.Id] = portfolio;
                }
            });
        }

        private Task LoadPorfolioAssetMapLinks()
        {
            return TaskEx.Run(() =>
            {
                DataTable table;
                using (var access = DataAccessFactory.New())
                {
                    table = access.GetAllPortfolioAssetMapLinks();
                }
                progress.SetMessage("Data is being loaded from the database."
                      + System.Environment.NewLine + "Portfolio-Asset Map links are loaded.");

                foreach (DataRow row in table.Rows)
                {
                    AssetMap assetMap;
                    var asmId = row["ASMID"].ConvertLong();
                    allAssetMaps.TryGetValue(asmId, out assetMap);

                    MapsPortfolio portfolio;
                    var pid = row["PID"].ConvertLong();
                    allPortfolios.TryGetValue(pid, out portfolio);

                    var linkId = row["LINKID"].ConvertLong();
                    allLinks[linkId] = new PortfolioAssetMapLink
                    {
                        Id = linkId,
                        AssetMap = assetMap,
                        Portfolio = portfolio,
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

        private Task LoadAssetMaps()
        {
            return TaskEx.Run(() =>
            {
                DataTable result;
                using (var access = DataAccessFactory.New())
                {
                    result = access.GetAllAssetMaps();
                }
                progress.SetMessage("Data is being loaded from the database."
                        + System.Environment.NewLine + "Asset Maps are loaded.");

                if (result == null || result.Rows.Count == 0)
                {
                    ViewService.ShowWarning("No asset map is defined.");
                    return;
                }

                foreach (DataRow row in result.AsEnumerable())
                {
                    var id = row["ID"].ConvertLong();
                    var name = row["NAME"].ConvertString();
                    var code = row["CODE"].ConvertString();
                    allAssetMaps[id] = new AssetMap { Id = id, Code = code, Name = name };
                }
            });
        }

        private Task LoadAssetMapComponents()
        {
            return TaskEx.Run(() =>
            {
                var childToParents = new Dictionary<long, long>();

                DataTable componentTable;
                using (var access = DataAccessFactory.New())
                {
                    componentTable = access.GetAllAssetMapComponents();
                }
                progress.SetMessage("Data is being loaded from the database."
                    + System.Environment.NewLine + "Asset Map components are loaded.");

                // create nodes
                foreach (DataRow row in componentTable.AsEnumerable())
                {
                    var component = new AssetMapComponent
                    {
                        Id = row["ID"].ConvertLong(),
                        Code = row["CODE"].ToString(),
                        IsRoot = row.IsNull("PARENTID"),
                        Name = row["NAME"].ToString(),
                        Order = row["ORDER"].ConvertInt(),
                    };
                    allComponents[component.Id] = component;

                    // save the child-parent link here
                    childToParents[component.Id] = row["PARENTID"].ConvertLong();
                    componentToAsms[component.Id] = row["AID"].ConvertLong();
                    // notice that root comps are not set in this table.
                }

                // link the parent and children here
                foreach (var childToParent in childToParents)
                {
                    if (!allComponents.ContainsKey(childToParent.Value))
                    {
                        Log.ErrorFormat("The component identified by an expected parent id {0} (for child {1}) doesn't exist.",
                            childToParent.Value, childToParent.Key);
                        continue;
                    }
                    var child = allComponents[childToParent.Key];
                    var parent = allComponents[childToParent.Value];
                    child.Parent = parent;
                    parent.Children.AddIfNotExist(child);
                }
            });
        }

        private Task LoadAssetMapComponentProperty()
        {
            return TaskEx.Run(() =>
            {
                DataTable propertiesTable;
                using (var access = DataAccessFactory.New())
                {
                    propertiesTable = access.GetAllAssetMapComponentProperties();
                }
                progress.SetMessage("Data is being loaded from the database."
                        + System.Environment.NewLine + "Asset Map component properties are loaded.");

                if (propertiesTable == null || propertiesTable.Rows.Count == 0)
                    return;

                foreach (DataRow row in propertiesTable.Rows)
                {
                    var prop = new AssetMapComponentProperty(
                        row["ASM_COMP_PROPERTY_ID"].ConvertLong(),
                        row["PROP_KEY"].ConvertString(),
                        row["PROP_VALUE"].ConvertString(),
                        row["UPDATE_DATETIME"].ConvertDate());
                    allProperties[prop.Id] = prop;
                    propertyToComponents[prop.Id] = row["ASM_COMP_ID"].ConvertLong();
                }
            });
        }

        private Task LoadNotDeletableAssetMapComponentIds()
        {
            return TaskEx.Run(() =>
            {
                DataTable table;
                using (var access = DataAccessFactory.New())
                {
                    table = access.GetCbItemAffectedAsmCompIds();
                }
                progress.SetMessage("Data is being loaded from the database."
                    + System.Environment.NewLine + "Composite benchmark items are loaded.");

                if (table == null || table.Rows.Count == 0)
                    return;

                foreach (DataRow row in table.Rows)
                {
                    AssetMapComponent comp;
                    var id = row["ACID"].ConvertLong();
                    if (allComponents.TryGetValue(id, out comp))
                    {
                        nonDeletableAsmComponentIds.Add(id);
                    }
                }
            });
        }

        private void MergeEverything()
        {
            if (allComponents.Count == 0)
                throw new Exception("No data exists!");

            // bind links to ptfs
            foreach (var link in allLinks.Values)
            {
                MapsPortfolio ptf;
                if (allPortfolios.TryGetValue(link.Portfolio.Id, out ptf))
                {
                    ptf.PortfolioAssetMapLink = link;
                }
            }

            // group ptfs by their pms
            var groupOfPortfoliosByPm = allPortfolios.Values
                .Where(p => p.PortfolioManager != null && p.AssetMap != null)
                .GroupBy(p => p.PortfolioManager);
            foreach (var group in groupOfPortfoliosByPm) // key is the pm
            {
                PortfolioManagers.Add(new PortfolioManagerViewModel(group.Key, group.OrderBy(p => p.Code)));
            }

            // assign asm to all asm components; assign root asm comps to asms
            foreach (var componentToAsm in componentToAsms)
            {
                var comp = allComponents[componentToAsm.Key];
                var asm = allAssetMaps[componentToAsm.Value];
                comp.AssetMap = asm;
                if (comp.IsRoot)
                {
                    asm.ComponentPrefix = comp.Code.Replace("ROOT", "");
                    asm.RootComponent = comp;
                }
            }

            // assign properties to asm comps
            foreach (var propertyToComponent in propertyToComponents)
            {
                var comp = allComponents[propertyToComponent.Value];
                var prop = allProperties[propertyToComponent.Key];
                comp.Properties.AddIfNotExist(prop);
                prop.AssetMapComponent = comp;
            }

            // determine if any children has composite benchmark item attached.
            foreach (var comp in allComponents.Values)
            {
                RecursiveAddNonDeletableAsmComponentId(comp);
            }
        }

        private void RecursiveAddNonDeletableAsmComponentId(AssetMapComponent comp)
        {
            if (nonDeletableAsmComponentIds.Contains(comp.Id) && comp.Parent != null)
            {
                nonDeletableAsmComponentIds.Add(comp.ParentId);
                RecursiveAddNonDeletableAsmComponentId(comp.Parent);
            }
        }

        private void ClearEverything()
        {
            PortfolioManagers.Clear();
            AssetMaps.Clear();
            ComponentProperties.Clear();
            AssetMapComponents.Clear();
            uiAssetMaps.Clear();
            SelectedComponent = null;
            SelectedPortfolio = null;
            SelectedPortfolioManager = null;
            SelectedAssetMap = null;

            allComponents = new Dictionary<long, AssetMapComponent>();
            allProperties = new Dictionary<long, AssetMapComponentProperty>();
            allLinks = new Dictionary<long, PortfolioAssetMapLink>();
            componentToAsms = new Dictionary<long, long>();
            propertyToComponents = new Dictionary<long, long>();
            allAssetMaps = new Dictionary<long, AssetMap>();
            allPortfolios = new Dictionary<long, MapsPortfolio>();
            nonDeletableAsmComponentIds = new HashSet<long>();
            CanEdit = false;
        }
    }
}