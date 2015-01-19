using System.Linq;
using Caliburn.Micro;
using PropertyChanged;
using Trading.Common.ViewModels;
using Trading.StrategyBuilder.Core;
using Trading.StrategyBuilder.Utils;
using Trading.StrategyBuilder.Views.Items;

namespace Trading.StrategyBuilder.ViewModels
{
    [ImplementPropertyChanged]
    public class CreateDecisionViewModel : ViewModelBase, ICreateDecisionViewModel
    {
        public string DecisionName { get; set; }
        public BindableCollection<ConditionResult> ConditionResults { get; set; }
        public BindableCollection<ComboboxItem<DecisionType>> DecisionTypes { get; set; }
        public BindableCollection<ComboboxItem<DecisionTargetType>> DecisionTargetTypes { get; set; }
        public ConditionResult SelectedConditionResult { get; set; }
        public ComboboxItem<DecisionType> SelectedDecisionType { get; set; }
        public ComboboxItem<DecisionTargetType> SelectedDecisionTargetType { get; set; }

        internal DecisionType CurrentDecisionType { get { return SelectedDecisionType.Type; } }
        internal DecisionTargetType CurrentDecisionTargetType { get { return SelectedDecisionTargetType.Type; } }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            ConditionResults = new BindableCollection<ConditionResult>(ConditionManager.AvailableConditionResults);
            DecisionTypes = new BindableCollection<ComboboxItem<DecisionType>>(ComboboxItemManager.Load<DecisionType>());
            DecisionTargetTypes = new BindableCollection<ComboboxItem<DecisionTargetType>>(ComboboxItemManager.Load<DecisionTargetType>());

            // default selected combobox items
            SelectedConditionResult = null;
            SelectedDecisionType = DecisionTypes.First(d => d.Type == DecisionType.CloseAllPositions);
            SelectedDecisionTargetType = DecisionTargetTypes.FirstOrDefault(d => d.Type == DecisionTargetType.SelectedSecurities);
        }

        public Decision Generate()
        {
            return new Decision { Name = DecisionName, Type = SelectedDecisionType.Type };
        }

        public void Ok()
        {
            TryClose(true);
        }
    }

    public interface ICreateDecisionViewModel
    {
        string DecisionName { get; set; }
        BindableCollection<ConditionResult> ConditionResults { get; }
        Decision Generate();
    }
}