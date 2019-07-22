using Lamar;
using NUnit.Framework;
using Topshelf.Common.Tests;

namespace Topshelf.Lamar.Tests
{
    [TestFixture]
    public class TopshelfLamarTests
    {
        private static Container _container;

        [SetUp]
        public void Setup()
        {
            _container = new Container(new SampleRegistry());
        }

        [Test]
        public void TestCanInstantiateServiceWithLamar()
        {
            var hasStarted = false;
            var hasStopped = false;

            var host = HostFactory.New(configurator =>
            {
                configurator.UseTestHost();
                configurator.UseLamar(_container);
                configurator.Service<SampleLamarService>(s =>
                {
                    s.ConstructUsingLamar();
                    s.WhenStarted((service, control) => hasStarted = service.Start());
                    s.WhenStopped((service, control) => hasStopped = service.Stop());
                });
            });

            var exitCode = host.Run();

            Assert.AreEqual(TopshelfExitCode.Ok, exitCode);
            Assert.IsTrue(hasStarted);
            Assert.IsTrue(hasStopped);
        }
    }
}