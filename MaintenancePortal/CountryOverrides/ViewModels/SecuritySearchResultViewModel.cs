using System.Windows.Input;
using Caliburn.Micro;
using Maintenance.Common.Entities;
using Maintenance.Common.ViewModels;
using Maintenance.CountryOverrides.Entities;
using System;

namespace Maintenance.CountryOverrides.ViewModels
{
    public class SecuritySearchResultViewModel : ViewModelBase, ISecuritySearchResultViewModel
    {
        public SecuritySearchResultViewModel()
        {
            DisplayName = "Securities Found";
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            SearchMessage = Securities.Count >= 100
                ? "Only the first 100 records are listed here. Please refine you criteria."
                : Securities.Count == 1
                ? "Exactly 1 security is found."
                : Securities.Count + " securities are found.";
        }

        private bool isSecuritySelected;

        private Security selectedSecurity;
        public Security SelectedSecurity
        {
            get { return selectedSecurity; }
            set { SetNotify(ref selectedSecurity, value); }
        }

        private string searchMessage;
        public string SearchMessage
        {
            get { return searchMessage; }
            set { SetNotify(ref searchMessage, value); }
        }

        private readonly BindableCollection<Security> securities = new BindableCollection<Security>();
        public BindableCollection<Security> Securities { get { return securities; } }

        public void Select()
        {
            if (SelectedSecurity != null)
                isSecuritySelected = true;
            TryClose(isSecuritySelected);
        }

        public override void CanClose(Action<bool> callback)
        {
            if (!isSecuritySelected)
                SelectedSecurity = null;
            callback(true);
        }

        public void HandleKeys(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                SelectedSecurity = null;
                TryClose(null);
                Securities.Clear();
            }
        }
    }
}