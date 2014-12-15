using Caliburn.Micro;
using PropertyChanged;
using Trading.Common.ViewModels;
using Trading.StrategyBuilder.Core;

namespace Trading.StrategyBuilder.ViewModels
{
    [ImplementPropertyChanged]
    public class CreateConditionViewModel : ViewModelBase, ICreateConditionViewModel
    {
        public string SourceValue { get; set; }
        public string TargetValue { get; set; }
        public string SelectedOperator { get; set; }
        public BindableCollection<string> Operators { get; private set; }

        public CreateConditionViewModel()
        {
            Operators = new BindableCollection<string> { ">", "<", "=", ">=", "<=", "<>" };
            SelectedOperator = Operators[0];
        }

        public CreateConditionViewModel(string sourceValue, string @operator, string targetValue)
            : this()
        {
            SourceValue = sourceValue;
            SelectedOperator = @operator;
            TargetValue = targetValue;
        }

        public void Ok()
        {
            TryClose(true);
        }

        public void Reset()
        {
            SourceValue = "";
            SelectedOperator = Operators[0];
            TargetValue = "";
        }

        public static CreateConditionViewModel From(Condition condition)
        {
            return new CreateConditionViewModel(condition.LeftOperandValue,
                condition.Operator.ToSymbol(), condition.RightOperandValue);
        }

        public Condition Get()
        {
            return new Condition(SourceValue, SelectedOperator.FromSymbol(), TargetValue);
        }
    }

    public interface ICreateConditionViewModel
    {
        string SourceValue { get; set; }
        string SelectedOperator { get; set; }
        string TargetValue { get; set; }
        Condition Get();
    }
}