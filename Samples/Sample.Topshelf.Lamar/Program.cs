using Lamar;
using System;
using Topshelf;
using Topshelf.Lamar;

namespace Sample.Topshelf.Lamar
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            var exitCode = HostFactory.Run(c =>
            {
                c.UseLamar(new Container(new SampleRegistry()));

                c.Service<SampleService>(s =>
                {
                    s.ConstructUsingLamar();
                    s.WhenStarted((service, control) => service.Start());
                    s.WhenStopped((service, control) => service.Stop());
                });
            });

            return (int)exitCode;
        }
    }

    public class SampleRegistry : ServiceRegistry
    {
        public SampleRegistry()
        {
            For<ISampleDependency>().Use<SampleDependency>();
        }
    }

    public class SampleService
    {
        private readonly ISampleDependency _sample;

        public SampleService(ISampleDependency sample)
        {
            _sample = sample;
        }

        public bool Start()
        {
            Console.WriteLine("Sample Service Started.");
            Console.WriteLine($"Sample Dependency: {_sample}");
            return _sample != null;
        }

        public bool Stop()
        {
            return _sample != null;
        }
    }

    public interface ISampleDependency
    {
    }

    public class SampleDependency : ISampleDependency
    {
    }
}