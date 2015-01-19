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
        public List<Condition> Conditions { get; private set; }
        public string ConditionResult { get; set; }

        public CreateFilterViewModel()
        {
            ConditionDescriptions = new BindableCollection<string>();
            Conditions = new List<Condition>();
        }

        public async void AddCondition()
        {
            var r = await ViewService.ShowDialog(CreateCondition);
            if (r.HasValue && (bool)r)
            {
                var condition = CreateCondition.Yield();
                Conditions.Add(condition);
                ConditionDescriptions.Add(condition.ToString());
            }
        }

        public Filter Generate()
        {
            if (ConditionManager.AllConditionResults.Any(cr => cr.DisplayName == FilterName))
                return null;

            var result = new Filter();
            result.Conditions.AddRange(Conditions);
            result.DisplayName = FilterName;
            result.ConditionResult = new ConditionResult { DisplayName = ConditionResult };
            return result;
        }

        public void Ok()
        {
            TryClose(true);
        }
    }

    public interface ICreateFilterViewModel : IHasViewService
    {
        Filter Generate();
    }
}