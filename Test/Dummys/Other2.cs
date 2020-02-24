using System;

namespace Test
{
    class Other2 : IOther2
    {
        private readonly IInterface2 _interface2;
        private readonly IInterface1 _interface1;

        public Other2(IInterface1 interface1, IInterface2 interface2)
        {
            _interface1 = interface1;
            _interface2 = interface2;
        }
        public void DoMoreStuff()
        {
            if (_interface1 != null)
                _interface2.Sideeffect();
        }
    }
}