using Trading.Common;
using Trading.Common.SharedSettings;
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
            BindSingleton<ISettings, InfrastructureSettings>();
        }
    }
}