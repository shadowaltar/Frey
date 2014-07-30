using Caliburn.Micro;
using MahApps.Metro.Controls.Dialogs;
using Maintenance.Common.Data;
using Maintenance.Common.Entities;
using Maintenance.Common.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ILog = log4net.ILog;
using LogManager = log4net.LogManager;

namespace Maintenance.AssetMaps.ViewModels
{
    public partial class EditorViewModel
    {
        public async void Save()
        {
            string message;
            var isOk = Check(out message);
            if (!isOk)
            {
                await ViewService.ShowError(message);
                return;
            }
            if (await ViewService.AskSave() != MessageDialogResult.Affirmative)
            {
                return;
            }

            var worker = new EditorSaveWorker(this);
            var result = await worker.Save();
            if (result)
            {
                OriginalAssetMap = AssetMap.Copy();

                if (await ViewService.ShowMessage("Saved successfully!", "Do you want to close the dialog and reload from the database?",
                    false, false, "Yes", "No") == MessageDialogResult.Affirmative)
                {
                    Publish<IMainViewModel>(ActivityType.Edit, true);
                    TryClose();
                }
            }
            else
            {
                await ViewService.ShowError("There is one or more errors thrown in the saving process. " +
                                            "Please look at the log file for more information.");
            }
        }

        private bool Check(out string message)
        {
            var isOk = true;
            var allCurrentComponents = AssetMapComponents.Flatten(c => c.Children);
            message = null;

            if (string.IsNullOrWhiteSpace(AssetMapName) || string.IsNullOrWhiteSpace(AssetMapCode))
            {
                message = "You must provide a code / name for this asset map. Please correct the error(s) and try to save again.";
                isOk = false;
            }
            // check if crash of asm code/name.
            if (isOk && Mode == EditorMode.Create)
            {
                var isCodeExists = AllAssetMaps.FirstOrDefault(asm => asm.Code == AssetMapCode) != null;
                var isNameExists = AllAssetMaps.FirstOrDefault(asm => asm.Name == AssetMapName) != null;
                if (isCodeExists)
                {
                    message = "The asset map cannot be created as there is another asset" +
                        " map already using the code \"" + AssetMapCode + "\".";
                    isOk = false;
                }
                else if (isNameExists)
                {
                    message = "The asset map cannot be created as there is another asset" +
                                                " map already using the name \"" + AssetMapName + "\".";
                    isOk = false;
                }
            }

            // check if root asm component's code is ended with "_ROOT"
            if (isOk && (AssetMapComponents.Count == 0 || AssetMapComponents[0].Children.Count == 0))
            {
                message = "One or more nodes under the root node is required for an asset map.";
                isOk = false;
            }


            // check if crash of asm component code (for those new comps)
            if (isOk)
            {
                foreach (var comp in allCurrentComponents.Where(vm => vm.Id < 0).Select(vm => vm.Component))
                {
                    var existCompCode = AllAssetMapComponents.FirstOrDefault(c => c.Code == comp.Code);
                    if (existCompCode != null)
                    {
                        message = "Unique asset map component code is required: the code \""
                            + existCompCode + "\" already exists.";
                        break;
                    }
                }
            }

            if (isOk && allCurrentComponents.Any(c => string.IsNullOrWhiteSpace(c.Name)))
            {
                message = "One or more components have no name assigned. Please correct the error(s) " +
                          "and try to save again.";
                isOk = false;
            }
            if (isOk && allCurrentComponents.Any(c => string.IsNullOrWhiteSpace(c.Code)))
            {
                message = "One or more components have no code assigned. Please correct the error(s) " +
                          "and try to save again.";
                isOk = false;
            }
            if (isOk && allCurrentComponents.Select(c => c.Code).ContainsDuplicates()) // can only detect two new node code duplication
            {
                message = "One or more components which the codes are the same. Please correct the " +
                         "duplications and try to save again.";
                isOk = false;
            }

            // check if root asm component's code is ended with "_ROOT"
            if (isOk && !AssetMapComponents[0].Code.EndsWith("_ROOT"))
            {
                message = "Suffix \"_ROOT\" is required for the root node of an asset map.";
                isOk = false;
            }

            return isOk;
        }

        internal class EditorSaveWorker
        {
            private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            private readonly List<AssetMapComponent> allCurrentComponents;
            private readonly Dictionary<long, AssetMapComponentProperty> allCurrentProperties; // Value is component where the prop is attached.
            private readonly List<AssetMapComponent> allOriginalComponents;
            private readonly List<AssetMapComponentProperty> allOriginalProperties;

            private readonly BindableCollection<AssetMapComponentViewModel> currentComponentTree;
            private readonly EditorMode mode;
            private readonly AssetMap currentAssetMap;
            private readonly AssetMap originalAssetMap;

            private readonly IDataAccessFactory<AssetMapDataAccess> dataAccessFactory;
            private readonly IViewService viewService;

            private bool willUpdateAssetMap;
            private List<AssetMapComponent> componentsToAdd;
            private List<AssetMapComponent> componentsToRemove;
            private List<AssetMapComponent> componentsToUpdate;
            private Dictionary<string, AssetMapComponent> componentsToUpdateCode;
            private Dictionary<AssetMapComponentProperty, AssetMapComponent> propertiesToAdd;
            private List<AssetMapComponentProperty> propertiesToRemove;
            private List<AssetMapComponentProperty> propertiesToUpdate;

            private ProgressDialogController progress;

            internal EditorSaveWorker(EditorViewModel editor)
            {
                mode = editor.Mode;

                dataAccessFactory = editor.DataAccessFactory;
                viewService = editor.ViewService;

                currentComponentTree = editor.AssetMapComponents;
                currentAssetMap = editor.AssetMap;
                originalAssetMap = editor.OriginalAssetMap;

                if (editor.AssetMap.Name != editor.OriginalAssetMap.Name ||
                    editor.AssetMap.Code != editor.OriginalAssetMap.Code)
                {
                    willUpdateAssetMap = true;
                }

                var currentRoot = editor.AssetMap.RootComponent.Copy(); // so won't link to the editor
                allCurrentComponents = currentRoot.Children.Flatten(c => c.Children).ToList();
                allCurrentComponents.Add(currentRoot);

                var originalRoot = editor.OriginalAssetMap.RootComponent.Copy(); // so won't link to the editor
                allOriginalComponents = originalRoot.Children.Flatten(c => c.Children).ToList();
                allOriginalComponents.Add(originalRoot);

                allCurrentProperties = new Dictionary<long, AssetMapComponentProperty>();
                foreach (var comp in allCurrentComponents)
                {
                    foreach (var prop in comp.Properties)
                    {
                        allCurrentProperties[prop.Id] = prop;
                    }
                }
                allOriginalProperties = allOriginalComponents.SelectMany(c => c.Properties).ToList();
            }

            internal async Task<bool> Save()
            {
                componentsToAdd = new List<AssetMapComponent>();
                componentsToUpdate = new List<AssetMapComponent>();
                componentsToRemove = new List<AssetMapComponent>();
                propertiesToAdd = new Dictionary<AssetMapComponentProperty, AssetMapComponent>();
                propertiesToUpdate = new List<AssetMapComponentProperty>();
                componentsToUpdateCode = new Dictionary<string, AssetMapComponent>();
                propertiesToRemove = new List<AssetMapComponentProperty>();

                if (!Check() || !CheckPropertyValues())
                    return false;

                progress = await viewService.ShowProgress("Saving", "Saving the asset map changes...");

                DefineOrders(currentComponentTree);
                CompareComponents();
                AssignNewIdsToComponents();

                CompareProperties();
                AssignNewIdsToProperties();
                AssignNewUpdateTimesToProperties();

                CompareAssetMap();

                var result = await InternalSave();
                await progress.Stop();
                return result;
            }

            private bool CheckPropertyValues()
            {
                string message = null;
                foreach (var property in allCurrentProperties.Values)
                {
                    switch (property.Key)
                    {
                        case "LEVEL_COUNTRY":
                            if (property.Value.ConvertInt(-1) < 0 ||
                                AssetMapComponent.GetLevel(property.AssetMapComponent) != 0)
                            {
                                message =
                                    "For property \"LEVEL_COUNTRY\", it must be assigned at root level, and its value must be a positive integer.";
                            }
                            goto BreakLoop;
                        case "LEVEL_REGION":
                            if (property.Value.ConvertInt(-1) < 0 ||
                                AssetMapComponent.GetLevel(property.AssetMapComponent) != 0)
                            {
                                message =
                                    "For property \"LEVEL_REGION\", it must be assigned at root level, and its value must be a positive integer.";
                            }
                            goto BreakLoop;
                        case "INCLUDE_IN_FI_DATA":
                            if (property.Value.ConvertBoolean() == null)
                            {
                                message =
                                    "For property \"INCLUDE_IN_FI_DATA\", you must provide \"true\" or \"false\" as its value.";
                            }
                            goto BreakLoop;
                        case "INCLUDE_IN_MATRIX":
                            if (property.Value.ConvertBoolean() == null)
                            {
                                message =
                                    "For property \"INCLUDE_IN_FI_DATA\", you must provide \"true\" or \"false\" as its value.";
                            }
                            goto BreakLoop;
                    }
                }

            BreakLoop:
                if (message == null)
                    return true;
                viewService.ShowWarning(message);
                return false;
            }

            private void CompareAssetMap()
            {
                if (originalAssetMap.Code != currentAssetMap.Code
                    || originalAssetMap.Name != currentAssetMap.Name)
                {
                    willUpdateAssetMap = true;
                }
            }

            private bool Check()
            {
                string message = null;
                if (allCurrentProperties.Values.Any(prop => prop.Key == null))
                {
                    message = "One or more component properties have no key selected. " +
                              "Please correct the error(s) and try to save again.";
                }
                else if (allCurrentProperties.Values.Any(prop => prop.Value == null))
                {
                    message = "One or more component properties have no value defined. " +
                              "Please correct the error(s) and try to save again.";
                }
                else if (CheckPropertyDuplicates()) // it should not happen as usually the property key combobox disallow duplicated properties
                {
                    message = "Some component properties are assigned under the same component " +
                              "for more than once. Please correct the error(s) and try to save again.";
                }
                if (message == null)
                {
                    return true;
                }
                viewService.ShowError(message);
                return false;
            }

            /// <summary>
            /// Check if there are duplicated property keys exist (existing plus newly added)
            /// under each of the asm component.
            /// </summary>
            /// <returns></returns>
            private bool CheckPropertyDuplicates()
            {
                var componentPropertyKeys = new Dictionary<long, List<string>>();
                foreach (var prop in allCurrentProperties.Values)
                {
                    var comp = prop.AssetMapComponent;
                    if (!componentPropertyKeys.ContainsKey(comp.Id))
                    {
                        componentPropertyKeys[comp.Id] = new List<string>();
                    }
                    if (componentPropertyKeys[comp.Id].Contains(prop.Key))
                    {
                        return true; // there is duplication.
                    }
                    componentPropertyKeys[comp.Id].Add(prop.Key);
                }
                return false;
            }

            private void DefineOrders(BindableCollection<AssetMapComponentViewModel> componentViewModels)
            {
                foreach (AssetMapComponentViewModel componentViewModel in componentViewModels)
                {
                    BindableCollection<AssetMapComponentViewModel> children = componentViewModel.Children;
                    for (int i = 0; i < children.Count; i++)
                    {
                        if (children[i].Order != i)
                        {
                            var comp = allCurrentComponents.FirstOrDefault(c => c.Id == children[i].Component.Id);
                            if (comp != null)
                            {
                                comp.Order = i;
                            }
                        }
                    }
                    DefineOrders(children);
                }
            }

            private void CompareComponents()
            {
                // Create ASM
                if (mode == EditorMode.Create)
                {
                    foreach (var newComp in allCurrentComponents)
                    {
                        componentsToAdd.AddIfNotExist(newComp);
                    }
                    return;
                }

                // Edit ASM

                // find out all comps to be inserted
                foreach (var newComp in allCurrentComponents)
                {
                    if (newComp.Id < 0) // all new comp fake ids are negative
                    {
                        componentsToAdd.AddIfNotExist(newComp);
                    }
                }

                // find out all comps to be deleted
                foreach (var originalComp in allOriginalComponents)
                {
                    if (allCurrentComponents.All(c => c.Id != originalComp.Id))
                    {
                        componentsToRemove.AddIfNotExist(originalComp);
                    }
                }

                // find out all comps to be updated
                foreach (var component in allCurrentComponents
                    .Where(c => !componentsToAdd.Contains(c) && !componentsToRemove.Contains(c)))
                {
                    var comp = component;
                    var originalComp = allOriginalComponents.FirstOrDefault(c => c.Id == comp.Id);
                    if (originalComp != null) // we assume always exist, as already filtered out new and removed comps.
                    {
                        if (component.Name != originalComp.Name
                            || component.Code != originalComp.Code
                            || component.Order != originalComp.Order)
                        {
                            componentsToUpdate.AddIfNotExist(component);
                        }

                        // tackle an update case such that, comp of code A change to code B
                        // and code B change to code C.
                        if (component.Code != originalComp.Code)
                        {
                            componentsToUpdateCode[component.Code + "?"] = component;
                        }
                    }
                }

            }

            private void AssignNewIdsToComponents()
            {
                if (componentsToAdd.Count > 0)
                {
                    using (var access = dataAccessFactory.New())
                    {
                        var ids = access.GetNewComponentIds(componentsToAdd.Count + 1)
                            .AsEnumerable().Select(r => r["NEXTVAL"].ConvertLong()).ToList();
                        for (int i = 0; i < componentsToAdd.Count; i++)
                        {
                            componentsToAdd[i].Id = ids[i];
                        }
                    }
                }
            }

            private void CompareProperties()
            {
                // Create ASM
                if (mode == EditorMode.Create)
                {
                    foreach (var newProp in allCurrentProperties.Values)
                    {
                        propertiesToAdd[newProp] = newProp.AssetMapComponent;
                    }
                    return;
                }

                // Edit ASM

                // find out all props to be inserted
                foreach (var pair in allCurrentProperties)
                {
                    if (pair.Key < 0) // all new prop fake ids are negative
                    {
                        propertiesToAdd[pair.Value] = pair.Value.AssetMapComponent;
                    }
                }

                // find out all props to be deleted
                foreach (var originalProp in allOriginalProperties)
                {
                    if (allCurrentProperties.All(pair => pair.Key != originalProp.Id)) // pair key is the id
                    {
                        propertiesToRemove.AddIfNotExist(originalProp);
                    }
                }

                // find out all props to be updated
                foreach (var property in allCurrentProperties.Values
                    .Where(p => !propertiesToAdd.ContainsKey(p) && !propertiesToRemove.Contains(p)))
                {
                    var prop = property;
                    var originalProp = allOriginalProperties.FirstOrDefault(c => c.Id == prop.Id);
                    if (originalProp != null) // we assume always exist, as already filtered out new and removed comps.
                    {
                        if (prop.Key != originalProp.Key
                            || prop.Value != originalProp.Value)
                        {
                            prop.UpdateTime = DateTime.Now;
                            propertiesToUpdate.AddIfNotExist(prop);
                        }
                    }
                }
            }

            private void AssignNewIdsToProperties()
            {
                if (propertiesToAdd.Count > 0)
                {
                    Queue<long> ids;
                    using (var access = dataAccessFactory.New())
                    {
                        ids = new Queue<long>(access.GetNewPropertyIds(propertiesToAdd.Count + 1)
                             .AsEnumerable().Select(r => r["NEXTVAL"].ConvertLong()));
                    }
                    foreach (var prop in propertiesToAdd.Keys)
                    {
                        prop.Id = ids.Dequeue();

                        // the property's component's id will be 0 if the component
                        // is newly added. Assign the comp's id here.
                        if (prop.AssetMapComponent.Id <= 0)
                        {
                            var code = prop.AssetMapComponent.Code; // use code to lookup, as id is not working
                            var comp = componentsToAdd.FirstOrDefault(c => c.Code == code);
                            if (comp != null)
                            {
                                prop.AssetMapComponent.Id = comp.Id;
                            }
                            else
                            {
                                throw new InvalidOperationException(
                                    "Property's ASM component has invalid Id, which is not expected to happen.");
                            }
                        }
                    }
                }
            }

            private void AssignNewUpdateTimesToProperties()
            {
                var updateTime = DateTime.Now;
                foreach (var prop in propertiesToAdd.Keys)
                {
                    prop.UpdateTime = updateTime;
                }
                foreach (var prop in propertiesToUpdate)
                {
                    prop.UpdateTime = updateTime;
                }
            }

            public async Task<bool> Delete(AssetMap assetMap)
            {
                var result = await TaskEx.Run(() =>
                {
                    using (var access = dataAccessFactory.NewTransaction())
                    {
                        try
                        {
                            foreach (var prop in allOriginalProperties)
                            {
                                if (!access.DeleteAssetMapComponentProperty(prop))
                                {
                                    access.Rollback();
                                    return false;
                                }
                            }
                            return access.DeleteAssetMap(assetMap);
                        }
                        catch (Exception e)
                        {
                            access.Rollback();
                            log.Error("Cannot delete the asset map.", e);
                            return false;
                        }
                    }
                });
                return result;
            }

            private Task<bool> InternalSave()
            {
                return TaskEx.Run(() =>
                {
                    using (var access = dataAccessFactory.NewTransaction())
                    {
                        try
                        {
                            if (mode == EditorMode.Create)
                            {
                                progress.AppendMessageForLoading("Inserting asset map...");

                                // setup the new asm's id
                                var newAsmId = access.GetNewAssetMapId()
                                    .AsEnumerable().Select(r => r["NEXTVAL"].ConvertLong()).FirstOrDefault();
                                currentAssetMap.Id = newAsmId;

                                if (!access.InsertAssetMap(currentAssetMap))
                                {
                                    access.Rollback();
                                    return false;
                                }
                            }
                            else
                            {
                                progress.AppendMessageForLoading("Updating asset map...");
                                if (willUpdateAssetMap && !access.UpdateAssetMap(currentAssetMap))
                                {
                                    access.Rollback();
                                    return false;
                                }
                            }

                            progress.AppendMessageForLoading("Inserting new components...");
                            Sort(componentsToAdd);
                            if (componentsToAdd.Any(component => !access.InsertAssetMapComponent(component)))
                            {
                                access.Rollback();
                                return false;
                            }

                            progress.AppendMessageForLoading("Updating components...");
                            foreach (var pair in componentsToUpdateCode)
                            {
                                var tempCode = pair.Key;
                                var comp = pair.Value;
                                var tempComp = new AssetMapComponent
                                {
                                    Id = comp.Id,
                                    Code = tempCode,
                                    Name = comp.Name,
                                    Order = comp.Order
                                };
                                if (!access.UpdateAssetMapComponent(tempComp))
                                {
                                    access.Rollback();
                                    return false;
                                }
                            }

                            if (componentsToUpdate.Any(component => !access.UpdateAssetMapComponent(component)))
                            {
                                access.Rollback();
                                return false;
                            }

                            progress.AppendMessageForLoading("Removing components...");
                            if (componentsToRemove.Any(component => !access.DeleteAssetMapComponent(component)))
                            {
                                access.Rollback();
                                return false;
                            }

                            progress.AppendMessageForLoading("Inserting component properties...");
                            if (propertiesToAdd.Any(pair => !access.InsertAssetMapComponentProperty(pair.Value.Id, pair.Key)))
                            {
                                access.Rollback();
                                return false;
                            }

                            progress.AppendMessageForLoading("Updating component properties...");
                            if (propertiesToUpdate.Any(property => !access.UpdateAssetMapComponentProperty(property)))
                            {
                                access.Rollback();
                                return false;
                            }

                            progress.AppendMessageForLoading("Removing component properties...");
                            if (propertiesToRemove.Any(property => !access.DeleteAssetMapComponentProperty(property)))
                            {
                                access.Rollback();
                                return false;
                            }

                        }
                        catch (Exception e)
                        {
                            access.Rollback();
                            log.Error("Failed to save the changes of asset map.", e);
                            return false;
                        }
                        return true;
                    }
                });
            }

            /// <summary>
            /// Sort the components gonna add to db: smaller level (closer to root) will be
            /// inserted first.
            /// </summary>
            /// <param name="components"></param>
            private static void Sort(List<AssetMapComponent> components)
            {
                var componentLevels = new Dictionary<AssetMapComponent, int>();
                foreach (var component in components)
                {
                    componentLevels[component] = AssetMapComponent.GetLevel(component);
                }
                components.Sort((one, two) => componentLevels[one].CompareTo(componentLevels[two]));
            }
        }
    }
}