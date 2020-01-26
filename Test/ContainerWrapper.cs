namespace Test
{
    abstract class ContainerWrapper
    {
        public abstract void Register<T1, T2>() where T2 : T1;
        public abstract void Register<T>();
        public abstract T Resolve<T>();
        public abstract void Build();
    }
}