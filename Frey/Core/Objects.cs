using Automata.Mechanisms;
using Ninject;
using Ninject.Modules;

namespace Automata.Core
{
    public class Objects : StandardKernel
    {
        private static Objects instance;
        public static Objects Instance
        {
            get { return instance ?? (instance = new Objects(new ObjectModule())); }
        }

        private Objects(NinjectModule module)
            : base(module)
        {
        }

        public StandardKernel Kernel { get; private set; }

        private class ObjectModule : NinjectModule
        {
            public override void Load()
            {
                Bind<IReferenceUniverse>().To<TestReferences>().InSingletonScope();
            }
        }
    }
}