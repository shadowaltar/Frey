using Caliburn.Micro;
using PropertyChanged;
using Trading.Common.ViewModels;
using Trading.StrategyBuilder.Core;

namespace Trading.StrategyBuilder.ViewModels
{
    [ImplementPropertyChanged]
    public class RuleViewModel : ViewModelBaseSlim, IRuleViewModel
    {
        public int RuleIndex { get; set; }
        public BindableCollection<Condition> Conditions { get; private set; }
        public string For { get; set; }

        public RuleViewModel(IConditions conditions)
        {
            Conditions = new BindableCollection<Condition>();
            Conditions.AddRange(conditions);
        }
    }

    public interface IRuleViewModel
    {
    }
}