using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using MahApps.Metro.Controls.Dialogs;
using Maintenance.Common.Data;
using Maintenance.Common.Entities;
using Maintenance.Common.Utils;
using Maintenance.Common.ViewModels;

namespace Maintenance.Benchmarks.ViewModels
{
    public class AddFlyoutViewModel : ViewModelBase, IAddFlyoutViewModel
    {
        public IViewService ViewService { get; set; }
        public IDataAccessFactory DataAccessFactory { get; set; }

        public bool IsReady { get; set; }

        private readonly BindableCollection<Index> indexs = new BindableCollection<Index>();
        public BindableCollection<Index> Indexs { get { return indexs; } }

        private readonly BindableCollection<string> types = new BindableCollection<string>();
        public BindableCollection<string> Types { get { return types; } }

        public IEnumerable<Benchmark> ExistingItems { get; set; }

        private bool canSave;
        public bool CanSave
        {
            get { return canSave; }
            set { SetNotify(ref canSave, value); }
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
                if (SetNotify(ref selectedIndex, value) && value != null)
                {
                    CheckCanSave();
                    if (string.IsNullOrWhiteSpace(Code))
                    {
                        Code = value.Code;
                    }
                }
            }
        }

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
                Log.Error("Error occurs when adding data to database.", e);
                saveResult = false;
            }
            if (saveResult)
            {
                // disable save button if success
                CanSave = false;

                await ViewService.ShowMessage("New entry created",
                    "The new benchmark \"" + Code + "\" is created successfully.");

                ClearAll();
                Publish<IMainViewModel>(ActivityType.Add, "Benchmarks");
            }
            else
            {
                await ViewService.ShowMessage("Cannot create the entry!",
                    "The new benchmark \"" + Code + "\" cannot be created.");
            }
        }

        private Task<bool> InternalSave()
        {
            return TaskEx.Run(() =>
            {
                using (var access = DataAccessFactory.New())
                {
                    return access.AddIndexBenchmark(Code, SelectedIndex.Id);
                }
            });
        }

        public void ClearAll()
        {
            Code = null;
            SelectedIndex = null;
            SelectedType = null;
        }

        /// <summary>
        /// Check if can save to a new benchmark item.
        /// </summary>
        private void CheckCanSave()
        {
            CanSave = !string.IsNullOrEmpty(Code)
                && !Code.Contains(" ")
                && Code.Length <= 20
                && ExistingItems.All(b => b.Code != Code.Trim())
                && SelectedIndex != null
                && SelectedType != null;
        }
    }
}