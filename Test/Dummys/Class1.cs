using System;

namespace Test
{
    class Class1 : IInterface1
    {
        private string _field1 = "1";
        public void Method1()
        {
            Console.WriteLine(_field1);
        }
    }
}