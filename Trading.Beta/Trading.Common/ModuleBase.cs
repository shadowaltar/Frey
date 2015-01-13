using System.Linq;
using Ninject;
using Ninject.Modules;

namespace Trading.Common
{
    public abstract class ModuleBase : NinjectModule
    {
        public T Create<T>()
        {
            return Kernel.Get<T>();
        }

        /// <summary>
        /// Try to bind an interface to one of its implementation.
        /// It would avoid duplicated binding error.
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <typeparam name="TImpl"></typeparam>
        protected void TryBind<TInterface, TImpl>() where TImpl : TInterface
        {
            if (!Kernel.GetBindings(typeof(TInterface)).Any())
            {
                Bind<TInterface>().To<TImpl>();
            }
        }

        /// <summary>
        /// Try to bind an interface to one of its implementation in Singleton scope.
        /// It would avoid duplicated binding error.
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <typeparam name="TImpl"></typeparam>
        protected void TryBindSingleton<TInterface, TImpl>() where TImpl : TInterface
        {
            if (!Kernel.GetBindings(typeof(TInterface)).Any())
            {
                Bind<TInterface>().To<TImpl>().InSingletonScope();
            }
        }
    }
}