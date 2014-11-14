using Ninject;
using Trading.Common;

namespace Trading.Portal
{
    public class Bootstrapper : BootstrapperBase<IPortalViewModel>
    {
        public static new IKernel Kernel { get; private set; }

        protected override void Bind()
        {
            Kernel = base.Kernel;

            Bind<IPortalViewModel, PortalViewModel>();
        }
    }
}