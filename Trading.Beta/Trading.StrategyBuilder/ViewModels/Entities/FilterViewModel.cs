using Caliburn.Micro;
using PropertyChanged;
using Trading.Common.ViewModels;

namespace Trading.StrategyBuilder.ViewModels.Entities
{
    [ImplementPropertyChanged]
    public class FilterViewModel : ViewModelBaseSlim
    {
        public string DisplayName { get; set; }
        public BindableCollection<string> Conditions { get; set; } 
        public string ConditionResultName { get; set; }
    }
}