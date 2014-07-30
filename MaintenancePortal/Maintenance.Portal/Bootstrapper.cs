using Maintenance.Common;
using Maintenance.Common.SharedSettings;
using Maintenance.Common.Utils;
using Maintenance.Portal.ViewModels;
using Ninject;

namespace Maintenance.Portal
{
    public class Bootstrapper : BootstrapperBase<IPortalViewModel>
    {
        public static new IKernel Kernel { get; private set; }

        protected override void Bind()
        {
            Kernel = base.Kernel;

            Bind<IPortalViewModel, PortalViewModel>();
            Bind<IOptionsFlyoutViewModel, OptionsFlyoutViewModel>();
            BindSingleton<ImapSettings, ImapSettings>();
            BindSingleton<FmsSettings, FmsSettings>();
        }
    }
}