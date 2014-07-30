using System.Collections.Generic;
using Caliburn.Micro;
using MahApps.Metro.Controls.Dialogs;
using Maintenance.Common.Data;
using Maintenance.Common.Entities;
using Maintenance.Common.Utils;
using System;
using System.Linq;
using System.Threading.Tasks;
using Maintenance.Common.ViewModels;

namespace Maintenance.Benchmarks.ViewModels
{
    public class EditFlyoutViewModel : ViewModelBase, IEditFlyoutViewModel
    {
        public IViewService ViewService { get; set; }
        public IDataAccessFactory DataAccessFactory { get; set; }

        public IEnumerable<Benchmark> ExistingItems { get; set; }

        private readonly BindableCollection<Index> indexs = new BindableCollection<Index>();
        public BindableCollection<Index> Indexs { get { return indexs; } }

        private readonly BindableCollection<string> types = new BindableCollection<string>();
        public BindableCollection<string> Types { get { return types; } }

        private bool canSave;
        public bool CanSave
        {
            get { return canSave; }
            set { SetNotify(ref canSave, value); }
        }

        private Benchmark currentItem;

        public bool IsReady { get; set; }

        private string code;
        public string Code
        {
            get { return code; }
            set
            {
                if (SetNotify(ref code, value) && value != null)
                {
                    CheckCanSave();
                }
            }
        }

        private string selectedType;
        public string SelectedType
        {
            get { return selectedType; }
            set
            {
                if (SetNotify(ref selectedType, value) && value != null)
                {
                    CheckCanSave();
                }
            }
        }

        private Index selectedIndex;
        public Index SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                if (SetNotify(ref selectedIndex, value))
                {
                    CheckCanSave();
                }
            }
        }

        /// <summary>
        /// Let main vm sets the selected item to the edit vm.
        /// </summary>
        /// <param name="item"></param>
        public void SetItem(Benchmark item)
        {
            currentItem = item;
            Code = item.Code;
            SelectedIndex = Indexs.FirstOrDefault(c => c.Equals(currentItem.BasedOn));
            SelectedType = item.Type;
            CanSave = false;
        }

        /// <summary>
        /// Save the edited item to db. The method contains UI logics also.
        /// </summary>
        public async void Save()
        {
            var decision = await ViewService.AskSave();
            if (decision != MessageDialogResult.Affirmative)
                return;

            bool saveResult;
            try
            {
                saveResult = await InternalSave();
            }
            catch (Exception e)
            {
                Log.Error("Error occurs when editing data in database.", e);
                saveResult = false;
            }
            if (saveResult)
            {
                CanSave = false;

                await ViewService.ShowMessage("Changes saved",
                    "The changes of entry for \"" + Code + "\" is saved successfully.",
                    true, false, "ok");

                Publish<IMainViewModel>(ActivityType.Edit, currentItem);
            }
            else
            {
                await ViewService.ShowError("The changes of entry for \"" + Code + "\" cannot be saved.");
            }
        }

        private Task<bool> InternalSave()
        {
            return TaskEx.Run(() =>
            {
                Log.InfoFormat("Edit benchmark in database");

                using (var da = DataAccessFactory.New())
                {
                    return da.EditBenchmark(currentItem.Id, currentItem.Code, Code, SelectedIndex.Id);
                }
            });
        }

        public void UndoAll()
        {
            Code = currentItem.Code;
            if (currentItem.Type == "INDEX") // todo hardcoded benchmark Type.
                SelectedIndex = currentItem.BasedOn as Index;
            SelectedType = currentItem.Type;
            CanSave = false;
        }

        private void CheckCanSave()
        {
            CanSave = SelectedType != null
                      && (SelectedIndex != currentItem.BasedOn || Code != currentItem.Code);
        }
    }
}