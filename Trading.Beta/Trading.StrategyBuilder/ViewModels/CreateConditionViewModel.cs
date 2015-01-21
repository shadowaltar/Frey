using System;
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
    public class CreateConditionViewModel : ViewModelBase, ICreateConditionViewModel
    {
        public BindableCollection<ComboboxItem<CriteriaType>> CriteriaTypes { get; private set; }
        public BindableCollection<ComboboxItem<CriteriaTargetType>> SourceItems { get; private set; }
        public BindableCollection<ComboboxItem<CriteriaTargetType>> TargetItems { get; private set; }
        public BindableCollection<ComboboxItem<Operator>> Operators { get; private set; }

        public ComboboxItem<CriteriaType> SelectedCriteriaType { get; set; }
        public string SourceValue { get; set; }
        public string TargetValue { get; set; }
        public ComboboxItem<CriteriaTargetType> SelectedSourceItem { get; set; }
        public ComboboxItem<CriteriaTargetType> SelectedTargetItem { get; set; }
        public ComboboxItem<Operator> SelectedOperator { get; set; }

        public bool IsUnary { get; set; }
        public bool IsBinary { get; set; }

        public Operator CurrentOperator { get { return SelectedOperator.Type; } }
        public CriteriaTargetType CurrentSourceItem { get { return SelectedSourceItem.Type; } }
        public CriteriaTargetType CurrentTargetItem { get { return SelectedTargetItem.Type; } }
        public CriteriaType CurrentCriteriaType { get { return SelectedCriteriaType.Type; } }

        public CreateConditionViewModel()
        {
            CriteriaTypes = ComboboxItemManager.NewBindables<CriteriaType>();
            SourceItems = ComboboxItemManager.NewBindables<CriteriaTargetType>();
            TargetItems = ComboboxItemManager.NewBindables<CriteriaTargetType>();
            Operators = ComboboxItemManager.NewBindables<Operator>();

            SelectedSourceItem = SourceItems[0];
            SelectedTargetItem = TargetItems[1];
            SelectedOperator = Operators[0];
        }

        public CreateConditionViewModel(string sourceValue, string @operator, string targetValue)
            : this()
        {
            SourceValue = sourceValue;
            SelectedOperator = new ComboboxItem<Operator>(@operator, @operator.FromSymbol());
            TargetValue = targetValue;
        }

        public void Add()
        {
            if (Verify())
            {
                TryClose(true);
            }
        }

        public void OnSelectedCriteriaTypeChanged()
        {
            if (CurrentCriteriaType == CriteriaType.Unary)
            {
                IsUnary = true;
                IsBinary = false;
            }
            else
            {
                IsBinary = true;
                IsUnary = false;
            }
        }

        private bool Verify()
        {
            return true;
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

        public Condition Generate()
        {
            return new Condition(SourceValue, SelectedOperator.Type, TargetValue);
        }
    }

    public interface ICreateConditionViewModel
    {
        string SourceValue { get; set; }
        ComboboxItem<Operator> SelectedOperator { get; set; }
        string TargetValue { get; set; }
        Condition Generate();
    }
}