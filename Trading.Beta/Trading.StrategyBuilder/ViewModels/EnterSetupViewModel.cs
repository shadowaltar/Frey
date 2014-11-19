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
            var isGood = await ViewService.ShowDialog(CreateCondition as ViewModelBase);
            if (isGood.HasValue && (bool)isGood)
            {
                var condition = new Condition(CreateCondition.SourceValue, CreateCondition.SelectedOperator.FromSymbol(),
                    CreateCondition.TargetValue);

                var i = Interlocked.Increment(ref ruleIndex);
                if (Rules.Count == 0)
                {
                    var conds = new FilterSecurityConditions { condition };
                    Rules.Add(new RuleViewModel(conds) { RuleIndex = i });
                }
                else
                {
                    Rules[0].Conditions.Add(condition);
                }
            }
        }
    }

    public interface IEnterSetupViewModel : IHasViewService
    {
    }
}