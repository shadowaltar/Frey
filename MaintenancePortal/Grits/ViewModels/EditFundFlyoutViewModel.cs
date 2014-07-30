using Caliburn.Micro;
using MahApps.Metro.Controls.Dialogs;
using Maintenance.Common.Data;
using Maintenance.Common.Utils;
using Maintenance.Common.ViewModels;
using Maintenance.Grits.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Maintenance.Grits.ViewModels
{
    public class EditFundFlyoutViewModel : ViewModelBase, IEditFundFlyoutViewModel
    {
        public EditFundFlyoutViewModel()
        {
            Modes.AddRange(Enum.GetValues(typeof(GritsMode)).OfType<GritsMode>());
        }

        public IDataAccessFactory<GritsDataAccess> DataAccessFactory { get; set; }
        public IViewService ViewService { get; set; }

        private GritsFund currentItem;

        public bool IsReady { get; set; }

        private bool canSave;
        public bool CanSave
        {
            get { return canSave; }
            set { SetNotify(ref canSave, value); }
        }

        private bool canDisable;
        public bool CanDisable
        {
            get { return canDisable; }
            set { SetNotify(ref canDisable, value); }
        }

        private string code;
        public string Code
        {
            get { return code; }
            set { SetNotify(ref code, value); }
        }

        private GritsMode selectedMode;
        public GritsMode SelectedMode
        {
            get { return selectedMode; }
            set
            {
                if (SetNotify(ref selectedMode, value))
                {
                    CanSave = CheckCanSave();
                }
            }
        }

        private readonly BindableCollection<GritsMode> modes = new BindableCollection<GritsMode>();
        public BindableCollection<GritsMode> Modes { get { return modes; } }

        /// <summary>
        /// Let main vm sets the selected item to the edit vm.
        /// </summary>
        /// <param name="item"></param>
        public void SetItem(GritsFund item)
        {
            currentItem = item;
            Code = item.Code;
            SelectedMode = item.Mode;
            CanSave = false;
            CanDisable = SelectedMode != GritsMode.Disabled;
        }

        /// <summary>
        /// Save the edited item to db. The method contains UI logics also.
        /// </summary>
        public async void Save()
        {
            if (!CanSave)
                return;

            if (SelectedMode == GritsMode.Disabled)
            {
                Disable();
                return;
            }

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
                Log.Error("Error occurs when editing data to database.", e);
                saveResult = false;
            }
            if (saveResult)
            {
                CanSave = false;

                await ViewService.ShowMessage("Changes saved",
                    "The change for \"" + Code + "\" is saved successfully.");

                Publish<IMainViewModel>(ActivityType.Edit, currentItem);
            }
            else
            {
                await ViewService.ShowError("The change for \"" + Code + "\" cannot be saved.");
            }
        }

        public async void Disable()
        {
            var decision = await ViewService.AskSave();

            if (decision != MessageDialogResult.Affirmative)
                return;

            var saveResult = await InternalDisable();
            if (saveResult)
            {
                CanSave = false;

                await ViewService.ShowMessage("Fund disabled",
                    "The fund \"" + Code + "\" will no longer be processed by GRITS loader.",
                    true, false, "ok");

                Publish<IMainViewModel>(ActivityType.Edit, currentItem);
            }
            else
            {
                await ViewService.ShowError();
            }
        }

        private Task<bool> InternalDisable()
        {
            return TaskEx.Run(() =>
            {
                try
                {
                    using (var da = DataAccessFactory.New())
                    {
                        return da.DisableFund(currentItem.Code);
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            });
        }

        private Task<bool> InternalSave()
        {
            return TaskEx.Run(() =>
            {
                using (var da = DataAccessFactory.New())
                {
                    return da.ChangeFundMode(currentItem.Code, SelectedMode);
                }
            });
        }

        public void UndoAll()
        {
            SelectedMode = currentItem.Mode;
            CanSave = false;
        }

        private bool CheckCanSave()
        {
            return SelectedMode != currentItem.Mode;
        }
    }
}