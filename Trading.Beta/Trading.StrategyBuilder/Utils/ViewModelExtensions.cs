using System.Linq;
using Trading.StrategyBuilder.Core;
using Trading.StrategyBuilder.ViewModels;
using Trading.StrategyBuilder.ViewModels.Entities;

namespace Trading.StrategyBuilder.Utils
{
    internal static class ViewModelExtensions
    {
        internal static FilterViewModel CreateViewModel(this Filter filter)
        {
            // todo we need to think of a way to decouple the actual condition instance from those in vm.
            var vm = new FilterViewModel();
            vm.Conditions.AddRange(filter.Conditions.Select(c => c.ToString()));
            vm.DisplayName = filter.DisplayName;
            vm.ConditionResultName = filter.ConditionResult.DisplayName;
            return vm;
        }
    }
}