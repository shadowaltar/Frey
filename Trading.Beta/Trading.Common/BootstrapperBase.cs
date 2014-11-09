using Caliburn.Micro;
using Ninject;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Windows;
using Trading.Common.Utils;

namespace Trading.Common
{
    public class BootstrapperBase<T> : BootstrapperBase
    {
        public BootstrapperBase()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<T>();
        }

        protected IKernel Kernel { get; private set; }

        protected override void Configure()
        {
            // config log4net
            log4net.Config.XmlConfigurator.Configure();

            // config cm
            ConfigureCaliburnMicro();

            // IoC bindings
            Kernel = new StandardKernel();
            BindCaliburnMicro();
            BindMetroUi();

            // run user-defined Bootstrapper IoC bindings
            Bind();
        }

        protected virtual void ConfigureCaliburnMicro()
        {
            ConventionManager.AddElementConvention<FrameworkElement>(
                UIElement.IsEnabledProperty, "IsEnabled", "IsEnabledChanged");

            var baseBindProperties = ViewModelBinder.BindProperties;
            ViewModelBinder.BindProperties =
                (frameWorkElements, viewModels) =>
                {
                    foreach (var frameworkElement in frameWorkElements)
                    {
                        var propertyName = "Is" + frameworkElement.Name + "Enabled";
                        var property = viewModels.GetPropertyCaseInsensitive(propertyName);
                        if (property != null)
                        {
                            var convention = ConventionManager
                                .GetElementConvention(typeof(FrameworkElement));
                            ConventionManager.SetBindingWithoutBindingOverwrite(
                                viewModels, propertyName, property, frameworkElement, convention,
                                convention.GetBindableProperty(frameworkElement));
                        }
                    }
                    return baseBindProperties(frameWorkElements, viewModels);
                };
            ConventionManager.Singularize = Singularizer.Singularize;
        }

        protected virtual void BindCaliburnMicro()
        {
            BindSingleton<IWindowManager, WindowManager>();
            BindSingleton<IEventAggregator, EventAggregator>();
        }

        protected virtual void BindMetroUi()
        {
            Bind<IViewService, ViewService>();
        }

        protected virtual void Bind()
        {
            // Override and do your bindings here.
        }

        protected void Bind<TInterface, TImpl>() where TImpl : TInterface
        {
            Kernel.Bind<TInterface>().To<TImpl>();
        }

        protected void BindSingleton<TInterface, TImpl>() where TImpl : TInterface
        {
            Kernel.Bind<TInterface>().To<TImpl>().InSingletonScope();
        }

        protected void Load<TModule>() where TModule : NinjectModule
        {
            Kernel.Load((TModule)Activator.CreateInstance(typeof(TModule)));
        }

        protected override object GetInstance(Type service, string key)
        {
            if (service != null)
            {
                return Kernel.Get(service);
            }
            if (key != null)
            {
                return Kernel.Get<object>(key);
            }
            throw new ArgumentNullException("service");
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return Kernel.GetAll(service);
        }

        protected override void BuildUp(object instance)
        {
            Kernel.Inject(instance);
        }
    }
}