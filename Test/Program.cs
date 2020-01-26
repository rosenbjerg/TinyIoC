using System;
using System.Threading.Tasks;
using Autofac;
using TinyInversionOfControl;

namespace Test
{
    class Program
    {
        private const int Times = 100000;
        
        static void Main(string[] args)
        {
            var (tinyResolved, tinyReg, tinyRes) = ContainerTest<TinyContainerWrapper>();
            var (autoResolved, autoReg, autoRes) = ContainerTest<AutofacContainerWrapper>();
            var (windResolved, windReg, windRes) = ContainerTest<WindsorContainerWrapper>();
            
            Console.WriteLine($"TinyIoC: Registration {tinyReg.Ticks / Times}ticks   Resolve: {tinyRes.Ticks / Times}ticks");
            Console.WriteLine($"Autofac: Registration {autoReg.Ticks / Times}ticks   Resolve: {autoRes.Ticks / Times}ticks");
            Console.WriteLine($"Windsor: Registration {windReg.Ticks / Times}ticks   Resolve: {windRes.Ticks / Times}ticks");
            
            tinyResolved.DoAllTheStuff();
            autoResolved.DoAllTheStuff();
            windResolved.DoAllTheStuff();
        }

        static (IInterface resolved, TimeSpan regTime, TimeSpan resolveTime) ContainerTest<TContainerWrapper>() where TContainerWrapper : ContainerWrapper, new()
        {
            var (container, regTime) = Test(Times, () =>
            {
                var c = new TContainerWrapper();
                c.Register<IOther2, Other2>();
                c.Register<IInterface, Classy>();
                c.Register<IInterface2, Class2>();
                c.Register<JustClass>();
                c.Register<IOther3, Other3>();
                c.Register<IInterface1, Class1>();
                c.Register<IOther1, Other1>();
                c.Build();
                return c;
            });
            var (resolved, resolveTime) = Test(Times, () => container.Resolve<IInterface>());
            return (resolved, regTime, resolveTime);
        }
        static (T obj, TimeSpan) Test<T>(int times, Func<T> action)
        {
            var started = DateTime.UtcNow;

            T obj = default;
            for (int i = 0; i < times; i++)
            {
                obj = action();
            }

            return (obj, DateTime.UtcNow.Subtract(started));
        }

        private static (TinyIoC ioc, TimeSpan registerTime) TinyIoC()
        {
            var started = DateTime.UtcNow;
            
            var ioc = new TinyInversionOfControl.TinyIoC();
            ioc.Register<IOther2, Other2>();
            ioc.Register<IInterface, Classy>();
            ioc.Register<IInterface2, Class2>();
            ioc.Register<JustClass>();
            ioc.Register<IOther3, Other3>();
            ioc.Register<IInterface1, Class1>();
            ioc.Register<IOther1, Other1>();

            var registerTime = DateTime.UtcNow.Subtract(started);

            return (ioc, registerTime);
        }
        
        private static (IContainer container, TimeSpan registerTime) Autofac()
        {
            var started = DateTime.UtcNow;
            var builder = new ContainerBuilder();
            
            builder.RegisterType<Other2>().As<IOther2>();
            builder.RegisterType<Classy>().As<IInterface>();
            builder.RegisterType<Class2>().As<IInterface2>();
            builder.RegisterType<JustClass>();
            builder.RegisterType<Other3>().As<IOther3>();
            builder.RegisterType<Class1>().As<IInterface1>();
            builder.RegisterType<Other1>().As<IOther1>();

            var container = builder.Build();
            var registerTime = DateTime.UtcNow.Subtract(started);

            return (container, registerTime);
        }
    }
}