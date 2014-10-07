using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Maintenance.Common.Entities;
using Maintenance.Common.Utils;

namespace Maintenance.CompositeBenchmarks.ViewModels
{
    public partial class EditorViewModel
    {
        private Task<SaveResult> InternalSaveNew()
        {
            return TaskEx.Run(() =>
            {
                lastEffectivePortfolioAssetMapLink = null;
                using (var access = DataAccessFactory.NewTransaction())
                {
                    try
                    {
                        // try to adjust last-time assigned cb's expiry date.
                        var lastCbTable = access.GetLastEffectivePortfolioAssetMapLink(SelectedPortfolio.Id,
                            SelectedAssetMap.AssetMap.Id);

                        if (!lastCbTable.IsNullOrEmpty())
                        {
                            lastEffectivePortfolioAssetMapLink = new PortfolioAssetMapLink
                            {
                                AssetMap = SelectedAssetMap.AssetMap,
                                Portfolio = SelectedPortfolio,
                                Id = lastCbTable.Rows[0]["ID"].ConvertLong(),
                                EffectiveDate = lastCbTable.Rows[0]["EFFECTIVE"].ConvertIsoDate(),
                                ExpiryDate = lastCbTable.Rows[0]["EXPIRY"].ConvertIsoDate(),
                            };
                        }

                        var lastNewExpiryDate = lastEffectivePortfolioAssetMapLink != null
                            ? EffectiveDate.AddDays(-1)
                            : DateTime.MinValue.Date;

                        if (lastEffectivePortfolioAssetMapLink != null
                            && lastEffectivePortfolioAssetMapLink.EffectiveDate > lastNewExpiryDate)
                        {
                            // stop immediately
                            Log.InfoFormat(@"
Attempt to update last composite benchmark which ends before it starts
: new expiry date is {0}, the effective date is {1}."
                                , lastNewExpiryDate, lastEffectivePortfolioAssetMapLink.EffectiveDate);
                            access.Rollback();
                            return SaveResult.ExpirySmallerThanEffective;
                        }

                        if (lastEffectivePortfolioAssetMapLink != null)
                        {
                            lastEffectivePortfolioAssetMapLink.ExpiryDate = lastNewExpiryDate;
                            var result = access.UpdatePortfolioAssetMapLink(lastEffectivePortfolioAssetMapLink);
                            if (!result)
                            {
                                Log.InfoFormat(@"Cannot modify last benchmark's expiry date to {0}!",
                                    lastNewExpiryDate);
                                access.Rollback();
                                return SaveResult.CannotModifyLastEntry;
                            }
                        }

                        if (!access.AddPortfolioAssetMapLink(portfolioAssetMapLink))
                        {
                            access.Rollback();
                            return SaveResult.CannotAddEntry;
                        }

                        foreach (var component in BenchmarkComponents.Flatten(bc => bc.Children)
                            .Where(bcvm => bcvm.Index != null)
                            .Select(bcvm => bcvm.CompositeBenchmarkItem))
                        {
                            if (!access.AddCompositeBenchmarkItem(component))
                            {
                                access.Rollback();
                                return SaveResult.CannotAddComponentEntry;
                            }
                        }
                        return SaveResult.Success;
                    }
                    catch (Exception e)
                    {
                        try
                        {
                            access.Rollback();
                        }
                        catch (Exception ex)
                        {
                            Log.Error("Unknown error occurred when rolling back the creation of composite benchmark in database.", ex);
                        }
                        Log.Error("Unknown error occurred when creating the composite benchmark in database.", e);
                        return SaveResult.Unknown;
                    }
                }
            });
        }

        private Task<SaveResult> InternalSaveEdit()
        {
            return TaskEx.Run(() =>
            {
                using (var access = DataAccessFactory.NewTransaction())
                {
                    try
                    {
                        List<CompositeBenchmarkItem> added;
                        List<CompositeBenchmarkItem> removed;
                        List<CompositeBenchmarkItem> modified;
                        FindModifiedComponents(out added, out removed, out modified);

                        foreach (var component in removed)
                        {
                            if (!access.DeleteCompositeBenchmarkItem(component))
                            {
                                access.Rollback();
                                return SaveResult.CannotRemoveComponentEntry;
                            }
                        }

                        foreach (var component in added)
                        {
                            if (!access.AddCompositeBenchmarkItem(component))
                            {
                                access.Rollback();
                                return SaveResult.CannotAddComponentEntry;
                            }
                        }

                        foreach (var component in modified)
                        {
                            if (!access.ModifyCompositeBenchmarkItem(component))
                            {
                                access.Rollback();
                                return SaveResult.CannotModifyComponentEntry;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        try
                        {
                            access.Rollback();
                        }
                        catch (Exception ex)
                        {
                            Log.Error("Unknown error occurred when rolling back the modification of composite benchmark in database.", ex);
                        }
                        Log.Error("Unknown error occurred when modifying the composite benchmark in database.", e);
                        return SaveResult.Unknown;
                    }
                }
                return SaveResult.Success;
            });
        }

        /// <summary>
        /// Find out newly created, just removed, and property being modified components,
        /// by comparing the old list to the new list.
        /// </summary>
        /// <param name="newComponents"></param>
        /// <param name="deletedComponents"></param>
        /// <param name="modifiedComponents"></param>
        private void FindModifiedComponents(out List<CompositeBenchmarkItem> newComponents,
            out List<CompositeBenchmarkItem> deletedComponents,
            out List<CompositeBenchmarkItem> modifiedComponents)
        {
            var currentComps = BenchmarkComponents.Flatten(bc => bc.Children)
                .Select(compVm => compVm.CompositeBenchmarkItem)
                .Where(comp => comp.Index != null).ToList();
            var originalComps = initialPortfolioAssetMapLink.Components
                .Flatten(c => c.Components)
                .Where(comp => comp.Index != null).ToList();

            // find out new components
            newComponents = currentComps.Where(cc
                => originalComps.FirstOrDefault(oc => oc.AssetMapComponent == cc.AssetMapComponent) == null)
                .ToList();
            // find out deleted components
            deletedComponents = originalComps.Where(oc
                => currentComps.FirstOrDefault(cc => oc.AssetMapComponent == cc.AssetMapComponent) == null)
                .ToList();

            // find out modified components
            modifiedComponents = new List<CompositeBenchmarkItem>();
            var commonAssetMapCompIds = originalComps.Where(oc =>
                currentComps.FirstOrDefault(cc => cc.AssetMapComponent.Id == oc.AssetMapComponent.Id) != null)
                .Select(c => c.AssetMapComponent.Id).ToList();

            foreach (var amcId in commonAssetMapCompIds)
            {
                var original = originalComps.FirstOrDefault(c => c.AssetMapComponent.Id == amcId);
                var current = currentComps.FirstOrDefault(c => c.AssetMapComponent.Id == amcId);
                if ((original != null && current != null)
                    &&
                    (original.Index != current.Index || original.Weight != current.Weight))
                {
                    // 'current' items never has cbi id (=0), so assign the 'original' ids into them.
                    current.Id = original.Id;
                    modifiedComponents.Add(current);
                }
            }
        }


        private enum SaveResult
        {
            Unknown,
            ExpirySmallerThanEffective,
            CannotModifyLastEntry,
            CannotAddEntry,
            CannotAddComponentEntry,
            CannotRemoveComponentEntry,
            CannotModifyComponentEntry,
            Success,
        }
    }
}