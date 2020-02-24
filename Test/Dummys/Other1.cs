using System;

namespace Test
{
    class Other1 : IOther1
    {
        private readonly IOther2 _other2;

        public Other1(IOther2 other2)
        {
            _other2 = other2;
        }
        public void DoStuff()
        {
            if (_other2 == null)
            {
            }
        }
    }
}