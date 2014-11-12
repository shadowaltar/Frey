using System.Threading;
using Caliburn.Micro;
using PropertyChanged;
using Trading.Common.Utils;
using Trading.Common.ViewModels;
using Trading.StrategyBuilder.Core;

namespace Trading.StrategyBuilder.ViewModels
{
    [ImplementPropertyChanged]
    public class EnterSetupViewModel : ViewModelBase, IEnterSetupViewModel
    {
        public ICreateConditionViewModel CreateCondition { get; set; }
        public IViewService ViewService { get; set; }

        public BindableCollection<RuleViewModel> Rules { get; private set; }

        private int ruleIndex;

        public EnterSetupViewModel(ICreateConditionViewModel createCondition)
        {
            CreateCondition = createCondition;
            Rules = new BindableCollection<RuleViewModel>();
        }

        public async void CreateSecurityRules()
        {
            await ViewService.ShowDialog(CreateCondition as ViewModelBase);
            if (isGood)
            {
                
            }
            var i = Interlocked.Increment(ref ruleIndex);
            var conds = new FilterSecurityConditions();
            Rules.Add(new RuleViewModel(conds));
        }
    }

    public interface IEnterSetupViewModel : IHasViewService
    {
    }
}