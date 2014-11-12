using PropertyChanged;
using Trading.Common.ViewModels;
using Trading.StrategyBuilder.Core;

namespace Trading.StrategyBuilder.ViewModels
{
    [ImplementPropertyChanged]
    public class CreateConditionViewModel : ViewModelBase, ICreateConditionViewModel
    {
        public string Field { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }

        public CreateConditionViewModel(string field, string @operator, string value)
        {
            Field = field;
            Operator = @operator;
            Value = value;
        }

        public static CreateConditionViewModel From(Condition condition)
        {
            return new CreateConditionViewModel(condition.SourceValue, condition.Operator, condition.TargetValue);
        }

        public Condition To()
        {
            return new Condition(Field, Operator, Value);
        }
    }

    public interface ICreateConditionViewModel
    {
    }
}