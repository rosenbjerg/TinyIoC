using TinyInversionOfControl;

namespace Test
{
    class TinyContainerWrapper : ContainerWrapper
    {
        private readonly TinyIoC _ioc = new TinyIoC();
        private TinyIoCContainer _container;
        
        public override void Register<T1, T2>()
        {
            _ioc.Register<T1, T2>();
        }

        public override void Register<T>()
        {
            _ioc.Register<T>();
        }

        public override T Resolve<T>()
        {
            return _container.Resolve<T>();
        }

        public override void Build()
        {
            _container = _ioc.Build();
        }
    }
}