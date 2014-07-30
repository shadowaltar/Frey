using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using Maintenance.AssetMaps.Utils;
using Maintenance.Common.Entities;
using Maintenance.Common.Utils;
using Maintenance.Common.ViewModels;
using MetroWindow = MahApps.Metro.Controls.MetroWindow;

namespace Maintenance.AssetMaps.ViewModels
{
    public partial class EditorViewModel : ViewModelBase, IEditorViewModel
    {
        public EditorViewModel(IViewService viewService)
        {
            DisplayName = "Asset Map Editor"; // default title, which won't be visible unless error.
            ViewService = viewService; // don't use parent's viewService; since we want to show dialogs on top of this window directly
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            ViewService.Window = (MetroWindow)view;
        }

        public void OnClosing()
        {
            // because this vm is injected, have to reset everything for next time reuse
            AssetMap = null;
            OriginalAssetMap = null;
            RelatedPortfolios = null;
        }

        public void Initialize(EditorMode mode, AssetMap selectedAssetMap = null)
        {
            AssetMap = selectedAssetMap;
            AssetMapComponents.Clear();
            Mode = mode;

            switch (mode)
            {
                case EditorMode.Create:
                    DisplayName = "Create a New Asset Map";
                    AssetMap = new AssetMap { Code = "ASM", ComponentPrefix = "ASM_", Id = 0, Name = "ASM" };
                    OriginalAssetMap = AssetMap;
                    AssetMapComponents.Add(new AssetMapComponentViewModel(CreateNewRoot()));

                    CanUndoAll = false;
                    CanDeleteAssetMap = false;
                    break;
                case EditorMode.Edit:
                    if (selectedAssetMap == null)
                    {
                        RelatedPortfolios = null;
                        break;
                    }

                    DisplayName = "Edit Asset Map \"" + AssetMap.Name + "\"";
                    AssetMapCode = selectedAssetMap.Code;
                    AssetMapName = selectedAssetMap.Name;

                    if (OriginalAssetMap == null || OriginalAssetMap.Id != AssetMap.Id)
                    {
                        OriginalAssetMap = AssetMap.Copy();
                    }
                    else
                    {
                        AssetMap = OriginalAssetMap.Copy();
                    }

                    AssetMapComponents.Add(new AssetMapComponentViewModel(AssetMap.RootComponent));

                    ExpandAllNodes();

                    CanUndoAll = true;
                    CanDeleteAssetMap = true;
                    break;
            }
            AssetMapComponents[0].CanEditName = false; // cannot edit root.

            foreach (var comp in AssetMapComponents.Flatten(f => f.Children))
            {
                comp.PropertyCount = comp.Component.Properties.Count;
                comp.Refresh();
            }

            CanDeleteProperty = false;
            CanAddProperty = false;
            CanAddChild = false;
            CanDelete = false;
        }

        public void CheckNonDeletableAsmComponent(HashSet<long> compIds)
        {
            // leaves first, then level by level back to root.
            foreach (var vm in AssetMapComponents.Flatten(c => c.Children).OrderByDescending(c => c.Level))
            {
                if (compIds.Contains(vm.Component.Id))
                {
                    vm.IsLinkedToCompositeBenchmarkItem = true;
                }
            }
        }

        public void ExpandAllNodes()
        {
            var allComps = AssetMapComponents.Flatten(comp => comp.Children);
            foreach (var comp in allComps)
            {
                comp.IsExpanded = true;
            }
        }

        public void CollapseAllNodes()
        {
            var allComps = AssetMapComponents.Flatten(comp => comp.Children);
            foreach (var comp in allComps)
            {
                comp.IsExpanded = false;
            }
        }

        public void SelectComponent(AssetMapComponentViewModel selected)
        {
            SelectedComponent = selected;

            ComponentProperties.Clear();
            if (SelectedComponent != null)
            {
                ComponentProperties.AddRange(SelectedComponent.Component.Properties
                    .Select(p => new ComponentPropertyViewModel(p)).OrderBy(p => p.Key));
            }

            // add allowed list of property keys (different kinds of properties), and the already
            // selected key's available options 
            foreach (var pvm in ComponentProperties)
            {
                AssetMapComponentPropertyHelper.AddAllowedProperties(selected.Component, pvm);
                pvm.Options.AddRange(AssetMapComponentPropertyHelper.GetOptions(pvm.Key));
            }

            SelectedComponentProperty = null;

            // check all buttons after selection.

            CanAddChild = SelectedComponent != null && SelectedComponent.Level <= 5;

            // cannot add property if all of them are already added to this component.
            CanAddProperty = SelectedComponent != null
                && ComponentProperties.Count < AssetMapComponentPropertyHelper.MaxPropertyPerComponent;

            CanMoveUp = SelectedComponent != null && !SelectedComponent.Component.IsRoot;
            CanMoveDown = SelectedComponent != null && !SelectedComponent.Component.IsRoot;
            CanDeleteProperty = false;
            // not allow delete, if any composite benchmark item attached to it or its descendants
            CanDelete = SelectedComponent != null && !SelectedComponent.Component.IsRoot
                && !SelectedComponent.IsLinkedToCompositeBenchmarkItem;
        }

        public void AddChild()
        {
            if (SelectedComponent != null)
            {
                if (SelectedComponent.Level == 5)
                {
                    ViewService.ShowWarning("You cannot create a node that the level is larger than 5.");
                    return;
                }
                var order = SelectedComponent.Children.Count == 0
                    ? 0 : SelectedComponent.Children.Max(c => c.Order) + 1;
                var comp = new AssetMapComponent
                {
                    AssetMap = AssetMap,
                    Id = NextFakeId(),
                    Order = order,
                    IsRoot = false,
                    Code = AssetMap.Code + "_",
                    Name = string.Empty,
                    Parent = SelectedComponent.Component,
                };
                SelectedComponent.Component.Children.Add(comp);

                SelectedComponent.IsExpanded = true;
                var vm = new AssetMapComponentViewModel(comp, SelectedComponent);

                SelectedComponent.Children.Add(vm);
                SelectedComponent.IsExpanded = true;

                SelectedComponent.IsSelected = false;
                vm.IsSelected = true;
                SelectComponent(vm);
            }
        }

        public async void Delete()
        {
            if (SelectedComponent == null)
                return;

            if (SelectedComponent.IsLinkedToCompositeBenchmarkItem)
            {
                await ViewService.ShowWarning("You cannot delete a component/asset map when one or more composite benchmark items are attached to it, or to its descendants.");
                return;
            }

            if (SelectedComponent.Component.IsRoot)
            {
                await ViewService.ShowWarning("The root component of an asset map cannot be deleted.");
            }
            else
            {
                SelectedComponent.Parent.Component.Children.Remove(SelectedComponent.Component); // remove the comp
                SelectedComponent.Parent.Children.Remove(SelectedComponent); // remove the viewmodel of comp
            }
        }

        public async void DeleteAssetMap()
        {
            MessageDialogResult answer;

            // check if linked to other composite benchmark items.
            if (AssetMapComponents[0].IsLinkedToCompositeBenchmarkItem)
            {
                await ViewService.ShowError("The asset map components are linked to one or more composite " +
                                            "benchmark items. It cannot be deleted.");
                answer = MessageDialogResult.Negative;
            }
            // check if trying to delete an asm which has portfolio linked to it.
            else if (!RelatedPortfolios.IsNullOrEmpty()) // prevent deletion if ptf attached
            {
                await ViewService.ShowError("The asset map cannot be deleted as there " +
                                            "are portfolios attached to it.");
                answer = MessageDialogResult.Negative;
            }
            else
            {
                answer = await ViewService.AskDelete();
            }

            // stop if no good.
            if (answer != MessageDialogResult.Affirmative)
                return;

            var worker = new EditorSaveWorker(this);
            var progress = await ViewService.ShowProgress("Deleting", "Deleting the asset map...");
            var result = await worker.Delete(AssetMap);
            await progress.Stop();

            if (!result)
            {
                await ViewService.ShowError("There is one or more errors thrown in the delete process. " +
                                            "Please look at the log file for more information.");
                answer = MessageDialogResult.Negative;
            }
            else
            {
                answer = await ViewService.ShowMessage("Deleted successfully!",
                    "Do you want to close the dialog?", false, false, "Yes", "No");
            }

            if (result) // only if save successfully, will the main vm be notified.
            {
                Publish<IMainViewModel>(ActivityType.Edit, true);
                if (answer == MessageDialogResult.Affirmative)
                    TryClose();
            }
        }

        public void AddProperty()
        {
            var property = new AssetMapComponentProperty(NextFakeId(), null, null,
                DateTime.MinValue, SelectedComponent.Component);

            SelectedComponent.Component.Properties.Add(property);
            SelectedComponent.PropertyCount++;

            var pvm = new ComponentPropertyViewModel(property);
            AssetMapComponentPropertyHelper.AddAllowedProperties(SelectedComponent.Component, pvm);
            ComponentProperties.Add(pvm);
        }

        public void DeleteProperty()
        {
            if (SelectedComponentProperty != null)
            {
                var property = SelectedComponentProperty.Property;
                SelectedComponent.Component.Properties.Remove(property); // remove the property
                ComponentProperties.Remove(SelectedComponentProperty); // remove the property's vm from ui
                SelectedComponent.PropertyCount--; // recalculate the property count
                SelectedComponentProperty = null;
            }
        }

        public void MoveUp()
        {
            if (SelectedComponent == null || SelectedComponent.Parent == null)
                return;
            var allSiblings = SelectedComponent.Parent.Children;
            var currentPosition = allSiblings.IndexOf(SelectedComponent);
            if (currentPosition != 0) // test if head
                allSiblings.Move(currentPosition, currentPosition - 1);
        }

        public void MoveDown()
        {
            if (SelectedComponent == null || SelectedComponent.Parent == null)
                return;
            var allSiblings = SelectedComponent.Parent.Children;
            var currentPosition = allSiblings.IndexOf(SelectedComponent);
            if (currentPosition + 1 != allSiblings.Count) // test if tail
                allSiblings.Move(currentPosition, currentPosition + 1);
        }

        public void UndoAll()
        {
            Initialize(EditorMode.Edit, OriginalAssetMap.Copy());
        }

        public void HandleShortcutKeys(KeyEventArgs e)
        {
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                {
                    switch (e.Key)
                    {
                        case Key.D:
                            if (SelectedComponent != null && SelectedComponentProperty != null)
                                DeleteProperty();
                            break;
                        case Key.N:
                            if (SelectedComponent != null)
                                AddProperty();
                            break;
                    }
                }
                else
                {
                    switch (e.Key)
                    {
                        case Key.N:
                            if (SelectedComponent != null)
                                AddChild();
                            break;
                        case Key.Up:
                            if (SelectedComponent != null)
                                MoveUp();
                            break;
                        case Key.Down:
                            if (SelectedComponent != null)
                                MoveDown();
                            break;
                        case Key.D:
                            if (SelectedComponent != null)
                                Delete();
                            break;
                        case Key.Z:
                            UndoAll();
                            break;
                        case Key.S:
                            Save();
                            break;
                    }
                }
            }
        }

        private AssetMapComponent CreateNewRoot()
        {
            var comp = new AssetMapComponent
            {
                AssetMap = AssetMap,
                Id = 0,
                Order = 0,
                IsRoot = true,
                Code = AssetMap.ComponentPrefix + "ROOT",
                Name = AssetMap.ComponentPrefix + "ROOT",
            };
            AssetMap.RootComponent = comp;
            return comp;
        }

        /// <summary>
        /// Get the next fake id for a newly created component.
        /// Notice that they are all negative to avoid any conflictions.
        /// </summary>
        /// <returns></returns>
        private long NextFakeId()
        {
            nextId--;
            return nextId;
        }
    }
}