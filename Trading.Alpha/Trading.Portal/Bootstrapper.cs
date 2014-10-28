using Trading.Common;
using Trading.Common.Data;
using Trading.Common.SharedSettings;
using Trading.Common.Utils;
using Trading.Portal.ViewModels;
using Ninject;

namespace Trading.Portal
{
    public class Bootstrapper : BootstrapperBase<IPortalViewModel>
    {
        public static new IKernel Kernel { get; private set; }

        protected override void Bind()
        {
            Kernel = base.Kernel;

            Bind<IPortalViewModel, PortalViewModel>();
            Bind<IOptionsFlyoutViewModel, OptionsFlyoutViewModel>();
            BindSingleton<ISettings, InfrastructureSettings>();
        }
    }
}