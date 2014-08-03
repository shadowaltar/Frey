using Maintenance.Common;
using Maintenance.Common.ViewModels;

namespace Maintenance.Benchmarks.ViewModels
{
    public class OptionsFlyoutViewModel : OptionsViewModelBase<IMainViewModel>, IOptionsFlyoutViewModel
    {
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            Environments.AddRange(Settings.Environments.Keys);
        }
    }
}