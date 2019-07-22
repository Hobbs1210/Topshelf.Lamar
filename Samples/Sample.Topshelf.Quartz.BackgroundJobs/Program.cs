using Lamar;
using Quartz;
using System;
using System.Threading.Tasks;
using Topshelf;
using Topshelf.Lamar;
using Topshelf.Quartz.Lamar;

namespace Sample.Topshelf.Quartz.BackgroundJobs
{
    internal class Program
    {
        private static void Main()
        {
            HostFactory.Run(c =>
            {
                c.UseLamar(new Container(new SampleRegistry()));

                c.Service<SampleService>(s =>
                {
                    s.ConstructUsingLamar();

                    s.WhenStarted((service, control) => service.Start());
                    s.WhenStopped((service, control) => service.Stop());

                    s.UseQuartzLamar();

                    s.ScheduleQuartzJob(q =>
                        q.WithJob(() => JobBuilder.Create<SampleJob>().WithIdentity("SampleJob").Build())
                            .AddTrigger(() => TriggerBuilder.Create().WithSimpleSchedule(builder => builder.WithIntervalInSeconds(5).RepeatForever()).Build())
                        );

                    s.ScheduleQuartzJob(q =>
                        q.WithJob(() => JobBuilder.Create<WithInjectedDependenciesJob>().WithIdentity("WithInjectedDependenciesJob").Build())
                            .AddTrigger(() => TriggerBuilder.Create().WithSimpleSchedule(builder => builder.WithIntervalInSeconds(5).RepeatForever()).Build())
                    );
                });
            });
        }
    }

    public class SampleRegistry : ServiceRegistry
    {
        public SampleRegistry()
        {
            For<ISampleDependency>().Use<SampleDependency>();
            For<IDependencyInjected>().Use<DependencyInjected>();
            For<IJob>().Use<WithInjectedDependenciesJob>();
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
            Console.WriteLine("Sample Service Started...");
            return true;
        }

        public bool Stop()
        {
            return true;
        }
    }

    public interface ISampleDependency
    {
    }

    public class SampleDependency : ISampleDependency
    {
    }

    public interface IDependencyInjected
    {
        Task Execute();
    }

    public class DependencyInjected : IDependencyInjected
    {
        public Task Execute()
        {
            Console.WriteLine("[" + typeof(DependencyInjected).Name + "] Triggered " + DateTime.Now.ToLongTimeString());

            return Task.CompletedTask;
        }
    }

    public class SampleJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine("The current time is: {0}", DateTime.Now);

            return Task.CompletedTask;
        }
    }

    public class WithInjectedDependenciesJob : IJob
    {
        private readonly IDependencyInjected _dependencyInjected;

        public WithInjectedDependenciesJob(IDependencyInjected dependencyInjected)
        {
            _dependencyInjected = dependencyInjected;
        }

        public Task Execute(IJobExecutionContext context)
        {
            return _dependencyInjected.Execute();
        }
    }
}