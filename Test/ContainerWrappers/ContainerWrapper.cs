namespace Test
{
    abstract class ContainerWrapper
    {
        public abstract void Register<T1, T2>()
            where T1 : class
            where T2 : T1;

        public abstract void Register<T>()
            where T : class;

        public abstract T Resolve<T>();

        public abstract void Build();
    }
}