using PropertyChanged;
using Trading.Common.ViewModels;
using Trading.StrategyBuilder.Core;

namespace Trading.StrategyBuilder.ViewModels
{
    [ImplementPropertyChanged]
    public class CreateConditionViewModel : ViewModelBase, ICreateConditionViewModel
    {
        public string SourceValue { get; set; }
        public string Operator { get; set; }
        public string TargetValue { get; set; }

        public CreateConditionViewModel()
        {

        }

        public CreateConditionViewModel(string sourceValue, string @operator, string targetValue)
        {
            SourceValue = sourceValue;
            Operator = @operator;
            TargetValue = targetValue;
        }

        public void Ok()
        {
            TryClose(true);
        }

        public static CreateConditionViewModel From(Condition condition)
        {
            return new CreateConditionViewModel(condition.SourceValue, condition.Operator, condition.TargetValue);
        }

        public Condition To()
        {
            return new Condition(SourceValue, Operator, TargetValue);
        }
    }

    public interface ICreateConditionViewModel
    {
        string SourceValue { get; set; }
        string Operator { get; set; }
        string TargetValue { get; set; }
        Condition To();
    }
}