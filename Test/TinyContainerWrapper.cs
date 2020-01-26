using TinyInversionOfControl;

namespace Test
{
    class TinyContainerWrapper : ContainerWrapper
    {
        private readonly TinyIoC _ioc = new TinyIoC();
        
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
            return _ioc.Resolve<T>();
        }

        public override void Build()
        {
            _ioc.Build();
        }
    }
}