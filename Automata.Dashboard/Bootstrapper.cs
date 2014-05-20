using System;
using System.Collections.Generic;
using Caliburn.Micro;
using Ninject;

namespace Automata.Dashboard
{
    public class Bootstrapper : Bootstrapper<IMainViewModel>
    {
        private IKernel kernel;

        protected override void Configure()
        {
            kernel = new StandardKernel();
            kernel.Bind<IWindowManager>().To<WindowManager>().InSingletonScope();
            kernel.Bind<IEventAggregator>().To<EventAggregator>().InSingletonScope();

            kernel.Bind<IMainViewModel>().To<MainViewModel>();
        }

        protected override object GetInstance(Type service, string key)
        {
            if (service != null)
            {
                return kernel.Get(service);
            }
            throw new ArgumentNullException("service");
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return kernel.GetAll(service);
        }

        protected override void BuildUp(object instance)
        {
            kernel.Inject(instance);
        }
    }
}