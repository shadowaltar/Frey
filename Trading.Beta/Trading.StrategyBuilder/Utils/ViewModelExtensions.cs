using Trading.StrategyBuilder.Core;
using Trading.StrategyBuilder.ViewModels;

namespace Trading.StrategyBuilder.Utils
{
    internal static class ViewModelExtensions
    {
        internal static StageViewModel CreateViewModel(this Stage stage)
        {
            // todo we need to think of a way to decouple the actual condition instance from those in vm.
            var vm = new StageViewModel();
            vm.Conditions.AddRange(stage.Conditions);
            vm.StageName = stage.Name;
            return vm;
        }
    }
}