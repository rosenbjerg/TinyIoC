using Autofac;

namespace Test
{
    class AutofacContainerWrapper : ContainerWrapper
    {
        private readonly ContainerBuilder _builder = new ContainerBuilder();
        private IContainer _container;
        
        public override void Register<T1, T2>()
        {
            _builder.RegisterType<T2>().As<T1>();
        }

        public override void Register<T>()
        {
            _builder.RegisterType<T>();
        }

        public override T Resolve<T>()
        {
            return _container.Resolve<T>();
        }

        public override void Build()
        {
            _container = _builder.Build();
        }
    }
}