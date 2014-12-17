using Caliburn.Micro;
using PropertyChanged;
using Trading.Common.Entities;
using Trading.Common.ViewModels;

namespace Trading.StrategyBuilder.ViewModels
{
    [ImplementPropertyChanged]
    public class SelectViewModel : ViewModelBase
    {
        public BindableCollection<Security> Securities { get; private set; }
        public Security SelectedSecurity { get; set; }

        public SelectViewModel()
        {
            Securities = new BindableCollection<Security>();
            DisplayName = "Pick one security";
        }

        public void Initialize(params Security[] securities)
        {
            foreach (var security in securities)
            {
                Securities.Add(security);
            }
        }

        public void OnDoubleClick()
        {
            if (SelectedSecurity != null)
            {
                TryClose(true);
            }
        }
    }
}