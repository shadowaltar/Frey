using Maintenance.Benchmarks.ViewModels;
using Ninject.Modules;

namespace Maintenance.Benchmarks
{
    public class Module : NinjectModule
    {
        public override void Load()
        {
            Bind<IMainViewModel>().To<MainViewModel>();
            Bind<IAddFlyoutViewModel>().To<AddFlyoutViewModel>();
            Bind<IEditFlyoutViewModel>().To<EditFlyoutViewModel>();
            Bind<IFilterFlyoutViewModel>().To<FilterFlyoutViewModel>();
            Bind<IOptionsFlyoutViewModel>().To<OptionsFlyoutViewModel>();
            Bind<IBenchmarkDependencyReportViewModel>().To<BenchmarkDependencyReportViewModel>();
        }
    }
}