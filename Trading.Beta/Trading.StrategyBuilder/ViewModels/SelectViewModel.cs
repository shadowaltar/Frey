using Caliburn.Micro;
using PropertyChanged;
using Trading.Common.Entities;
using Trading.Common.ViewModels;

namespace Trading.StrategyBuilder.ViewModels
{
    [ImplementPropertyChanged]
    public class SelectViewModel : ViewModelBase
    {
        public BindableCollection<SecurityViewModel> Securities { get; private set; }

        public SelectViewModel()
        {
            Securities = new BindableCollection<SecurityViewModel>();
            DisplayName = "Pick one security";
        }

        public void Add(params Security[] securities)
        {
            foreach (var security in securities)
            {
                Securities.Add(new SecurityViewModel(security));
            }
        }
    }

    [ImplementPropertyChanged]
    public class SecurityViewModel : Security
    {
        public SecurityViewModel()
        {

        }

        public SecurityViewModel(Security security)
        {
            Code = security.Code;
            Name = security.Name;
            Id = security.Id;
        }
    }
}