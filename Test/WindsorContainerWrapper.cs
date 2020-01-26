using Castle.MicroKernel.Registration;
using Castle.Windsor;
using TinyInversionOfControl;

namespace Test
{
    class WindsorContainerWrapper : ContainerWrapper
    {
        private readonly WindsorContainer _ioc = new WindsorContainer();
        
        public override void Register<T1, T2>()
        {
            _ioc.Register(Component.For<T1>().ImplementedBy<T2>().LifestyleTransient());
        }

        public override void Register<T>()
        {
            _ioc.Register(Component.For<T>().ImplementedBy<T>().LifestyleTransient());
        }

        public override T Resolve<T>()
        {
            return _ioc.Resolve<T>();
        }

        public override void Build()
        {
            
        }
    }
}