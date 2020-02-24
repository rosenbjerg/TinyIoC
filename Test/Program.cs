using System;
using System.Threading.Tasks;
using Autofac;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using TinyInversionOfControl;

namespace Test
{
    class Program
    {
        private const int Times = 100000;
        
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<Bench>();
            Console.WriteLine(summary);
        }

        
    }
    [SimpleJob(RuntimeMoniker.NetCoreApp31, baseline: true)]
    public class Bench
    {
        [Benchmark(Baseline = true)]
        public void TinyBench()
        {
            ContainerTest<TinyContainerWrapper>();
        }
        [Benchmark]
        public void AutoBench()
        {
            ContainerTest<AutofacContainerWrapper>();
        }
        [Benchmark]
        public void CastleBench()
        {
            ContainerTest<WindsorContainerWrapper>();
        }

        static void ContainerTest<TContainerWrapper>() where TContainerWrapper : ContainerWrapper, new()
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
            var resolved = c.Resolve<IInterface>();
            resolved.DoAllTheStuff();
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
    }
}