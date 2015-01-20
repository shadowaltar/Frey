using System.Linq;
using Caliburn.Micro;
using Ninject;
using PropertyChanged;
using System.Collections.Generic;
using Trading.Common.Utils;
using Trading.Common.ViewModels;
using Trading.StrategyBuilder.Core;

namespace Trading.StrategyBuilder.ViewModels
{
    [ImplementPropertyChanged]
    public class CreateFilterViewModel : ViewModelBase, ICreateFilterViewModel
    {
        public BindableCollection<string> ConditionDescriptions { get; private set; }
        [Inject]
        public ICreateConditionViewModel CreateCondition { get; set; }
        public IViewService ViewService { get; set; }

        public string FilterName { get; set; }
        public string ResultName { get; set; }
        public List<Condition> Conditions { get; private set; }

        public string Message { get; set; }

        public CreateFilterViewModel()
        {
            ConditionDescriptions = new BindableCollection<string>();
            Conditions = new List<Condition>();
            Message = "Add at least one condition and fill in condition and result names.";
        }

        public async void AddCondition()
        {
            var r = await ViewService.ShowDialog(CreateCondition);
            if (r.HasValue && (bool)r)
            {
                var condition = CreateCondition.Generate();
                Conditions.Add(condition);
                ConditionDescriptions.Add(condition.ToString());
            }
        }

        public Filter Generate()
        {
            var result = new Filter();
            result.Conditions.AddRange(Conditions);
            result.DisplayName = FilterName;
            result.ConditionResult = new ConditionResult { DisplayName = ResultName };

            return result;
        }

        public void Save()
        {
            if (Verify())
            {
                TryClose(true);
            }
        }

        private bool Verify()
        {
            if (Conditions.IsNullOrEmpty())
            {
                Message = "At least one condition must be defined.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(ResultName))
            {
                Message = "A valid Result Name is required.";
                return false;
            }
            if (ConditionManager.AllConditionResults.Any(cr => cr.DisplayName == FilterName))
            {
                Message = "A Result Name which is not used already is required.";
                return false;
            }

            return true;
        }
    }

    public interface ICreateFilterViewModel : IHasViewService
    {
        Filter Generate();
    }
}