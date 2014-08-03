using Caliburn.Micro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Trading.Common.Data;
using Trading.Common.Entities;
using Trading.Common.Utils;
using Trading.Common.ViewModels;
using Trading.CountryOverrides.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace Trading.CountryOverrides.ViewModels
{
    public class CustomCountryEditorViewModel : ViewModelBase, ICustomCountryEditorViewModel
    {
        public CustomCountryEditorViewModel(IViewService viewService)
        {
            DisplayName = "Custom Country Editor";
            ViewService = viewService; // don't use parent's viewService; since we want to show dialogs on top of this window directly
        }

        private DataGrid grid;
        public IViewService ViewService { get; set; }
        public IDataAccessFactory<CountryOverrideDataAccess> DataAccessFactory { get; set; }

        private readonly BindableCollection<CustomCountryViewModel> countries = new BindableCollection<CustomCountryViewModel>();
        public BindableCollection<CustomCountryViewModel> Countries { get { return countries; } }

        private List<CustomCountry> allCountries;

        private CustomCountryViewModel selectedCountry;

        public CustomCountryViewModel SelectedCountry
        {
            get { return selectedCountry; }
            set { SetNotify(ref selectedCountry, value); }
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            ViewService.Window = (MetroWindow)view;
            grid = ((MetroWindow)view).FindChild<DataGrid>("Countries");
            Load();
        }

        public void HandleKeys(KeyEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                switch (e.Key)
                {
                    case Key.N: Create(); break;
                    case Key.R: Load(); break;
                    case Key.S: Save(); break;
                }
            }
        }

        public async void Save()
        {
            var message = Check();
            if (message != null)
            {
                await ViewService.ShowError(message);
                return;
            }

            var newCountries = (from countryViewModel in Countries
                                where !countryViewModel.IsExist
                                select countryViewModel.Country).ToList();
            var currentAllCountries = Countries.Select(vm => vm.Country.Copy());
            var currentExistCountries = currentAllCountries.Except(newCountries).OrderBy(c => c.Code).ToList();
            var originalCountries = allCountries.OrderBy(c => c.Code).ToList();

            var modifiedCountries = new List<CustomCountry>();

            // always expect 1-to-1 relationship, because no delete function.
            for (int i = 0; i < currentExistCountries.Count; i++)
            {
                var current = currentExistCountries[i];
                var original = originalCountries[i];
                if (current.Name != original.Name || current.FmrCode != original.FmrCode)
                {
                    modifiedCountries.Add(current);
                }
            }

            if (newCountries.Count > 0 || modifiedCountries.Count > 0)
            {
                var result = await InternalSave(newCountries, modifiedCountries);
                if (!result)
                {
                    await ViewService.ShowError("Error occurred when saving the changes of custom countries.");
                }
                else
                {
                    allCountries.ClearAndAddRange(allCountries.Select(c => c.Copy()).OrderBy(c => c.Code));

                    if (await ViewService.ShowMessage("Saved successfully!",
                        "Do you want to close the dialog and refresh the main window?",
                        false, false, "Yes", "No") == MessageDialogResult.Affirmative)
                    {
                        Publish<IMainViewModel>(ActivityType.Edit, true);
                        TryClose();
                    }
                }
            }
        }

        private string Check()
        {
            string message = null;
            var codeDuplicationSet = new HashSet<string>();
            var fmrCodeDuplicationSet = new HashSet<string>();
            foreach (var c in Countries)
            {
                if (string.IsNullOrWhiteSpace(c.Code)
                    || string.IsNullOrWhiteSpace(c.FmrCode)
                    || string.IsNullOrWhiteSpace(c.Name))
                {
                    message = "All the fields of a custom country code must not be empty, or white spaces only.";
                    break;
                }
                if (!codeDuplicationSet.Add(c.Code))
                {
                    message = "A custom country code must be unique.";
                    break;
                }
                if (!fmrCodeDuplicationSet.Add(c.FmrCode))
                {
                    message = "A custom country FMR code must be unique.";
                    break;
                }
                if (c.Code.Length > 15)
                {
                    message = "The length of a custom country code must be shorter or equal to 15 characters.";
                    break;
                }
                if (c.FmrCode.Length > 15)
                {
                    message = "The length of a custom country FMR code must be shorter or equal to 15 characters.";
                    break;
                }
                if (c.Name.Length > 255)
                {
                    message = "The length of a custom country name must be shorter or equal to 255 characters.";
                    break;
                }
            }
            return message;
        }

        private Task<bool> InternalSave(IEnumerable<CustomCountry> newCountries,
            IEnumerable<CustomCountry> modifiedCountries)
        {
            return TaskEx.Run(() =>
            {
                Log.InfoFormat("Modify custom countries in database");

                using (var access = DataAccessFactory.NewTransaction())
                {
                    try
                    {
                        foreach (var country in newCountries)
                        {
                            if (!access.InsertCustomCountry(country))
                            {
                                Log.InfoFormat("Custom countries failed to be inserted: code {0}, fmrcode {1}, name {2}", country.Code, country.FmrCode, country.Name);
                                access.Rollback();
                                return false;
                            }
                        }
                        foreach (var country in modifiedCountries)
                        {
                            if (!access.UpdateCustomCountry(country))
                            {
                                Log.InfoFormat("Custom countries failed to be updated: code {0}, fmrcode {1}, name {2}", country.Code, country.FmrCode, country.Name);
                                access.Rollback();
                                return false;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        access.Rollback();
                        Log.Error("Cannot modify custom country table.", e);
                        return false;
                    }
                }
                return true;
            });
        }

        /// <summary>
        /// Create a new row at the start of list.
        /// </summary>
        public void Create()
        {
            var newCountry = new CustomCountryViewModel();
            Countries.Insert(0, newCountry);
            SelectedCountry = newCountry;
            grid.UpdateLayout(); // must update, or else the Items list won't have the new row.

            grid.ScrollIntoView(grid.Items[0]); // scroll new item into view.
            var cell = grid.FindChildren<DataGridCell>().FirstOrDefault();
            if (cell != null)
            {
                cell.Focus();
                grid.BeginEdit(); // enter edit mode (it would be the "Code" cell)
            }
        }

        public async void Load()
        {
            var progress = await ViewService.ShowLoading("Custom countries are being loaded from database.");
            Countries.Clear();
            var result = true;
            try
            {
                await InternalLoad();
            }
            catch (Exception e)
            {
                Log.Error("Cannot load custom countries.", e);
                result = false;
            }
            await progress.Stop();

            if (!result)
            {
                var r = await ViewService.ShowError("Failed to load custom countries.");
                if (r == MessageDialogResult.Affirmative)
                {
                    TryClose();
                }
            }
            else
            {
                Countries.AddRange(allCountries.Select(c => new CustomCountryViewModel(c.Copy()))
                    .OrderBy(c => c.Code));

                foreach (var vm in Countries)
                {
                    vm.IsExist = true;
                }
            }
        }

        internal Task InternalLoad()
        {
            return TaskEx.Run(() =>
            {
                DataTable table;
                using (var access = DataAccessFactory.New())
                {
                    table = access.GetAllLocationExtensions();
                }

                allCountries = new List<CustomCountry>();
                foreach (DataRow row in table.AsEnumerable().OrderBy(r => r["CODE"].Trim()))
                {
                    var ctry = new CustomCountry
                    {
                        FmrCode = row["CODE_FMR"].Trim(),
                        Code = row["CODE"].Trim(),
                        Name = row["NAME"].Trim(),
                    };
                    allCountries.Add(ctry);
                }
            });
        }
    }
}