using System;

namespace Test
{
    class Classy : IInterface
    {
        private readonly JustClass _justClass;
        private readonly IOther1 _other1;
        private readonly IOther2 _other2;
        private readonly IOther3 _other3;

        public Classy(JustClass justClass, IOther1 other1, IOther2 other2, IOther3 other3)
        {
            _justClass = justClass;
            _other1 = other1;
            _other2 = other2;
            _other3 = other3;
        }

        public void DoAllTheStuff()
        {
            _other1.DoStuff();
            _other2.DoMoreStuff();
            _other3.DoFinalStuff();
            Console.WriteLine(_justClass);
        }
    }
}