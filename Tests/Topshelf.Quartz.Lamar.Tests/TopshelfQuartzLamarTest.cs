using System;
using Lamar;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using NUnit.Framework;
using Quartz;
using Topshelf;
using Topshelf.Common.Tests;
using Topshelf.Lamar;

namespace Topshelf.Quartz.Lamar.Tests
{
    [TestFixture]
    public class TopshelfQuartzLamarTest
    {
        private static Container _container;
        private static SampleRegistry _registry;

        [SetUp]
        public void Setup()
        {
            _registry = new SampleRegistry();
            _container = new Container(_registry);
        }

        [Test]
        public void QuartzJobIsExecutedSuccessfullyTest()
        {
            var testJobMock = new Mock<IJob>();
            _container.Configure(new MoqRegistry(testJobMock.Object));

            var host = HostFactory.New(configurator =>
            {
                configurator.UseTestHost();

                configurator.UseQuartzLamar(_container);
                configurator.Service<SampleLamarService>(s =>
                {
                    s.ScheduleQuartzJob(q =>
                        q.WithJob(() => JobBuilder.Create<IJob>().WithIdentity("SampleJob").Build())
                            .AddTrigger(() => TriggerBuilder.Create().WithSimpleSchedule(builder => builder.WithInterval(TimeSpan.FromMilliseconds(1)).RepeatForever()).Build())
                    );

                    s.ConstructUsingLamar();
                    s.WhenStarted((service, control) => service.Start());
                    s.WhenStopped((service, control) => service.Stop());
                });
            });

            var exitCode = host.Run();

            Assert.AreEqual(TopshelfExitCode.Ok, exitCode);
            testJobMock.Verify(job => job.Execute(It.IsAny<IJobExecutionContext>()), Times.AtLeastOnce);
        }
    }

    public class MoqRegistry : ServiceRegistry
    {
        public MoqRegistry(IJob instance)
        {
            For<IJob>().Use(instance);
        }
    }
}