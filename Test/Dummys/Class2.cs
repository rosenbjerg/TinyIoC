namespace Test
{
    class Class2 : IInterface2
    {
        private readonly IInterface1 _interface1;

        public Class2(IInterface1 interface1)
        {
            _interface1 = interface1;
        }

        public int Sideeffect()
        {
            _interface1.Method1();
            return 1;
        }
    }
}