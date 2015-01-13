using Trading.Common;
using Trading.Common.Data;
using Trading.StrategyBuilder.Data;
using Trading.StrategyBuilder.ViewModels;

namespace Trading.StrategyBuilder
{
    public class Module : ModuleBase
    {
        public override void Load()
        {
            TryBind<IDataAccessFactory<Access>, AccessFactory<Access>>();

            Bind<IMainViewModel>().To<MainViewModel>();
            Bind<IEnterSetupViewModel>().To<EnterSetupViewModel>();
            Bind<ICreateConditionViewModel>().To<CreateConditionViewModel>();
            Bind<ICreateStageViewModel>().To<CreateStageViewModel>();
            Bind<IRunTestViewModel>().To<RunTestViewModel>();
            Bind<IStageViewModel>().To<StageViewModel>();

            Constants.InitializeDirectories();
        }
    }
}