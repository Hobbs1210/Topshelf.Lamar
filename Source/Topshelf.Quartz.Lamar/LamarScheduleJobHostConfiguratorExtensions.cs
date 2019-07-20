using System;
using Lamar;
using Topshelf.HostConfigurators;
using Topshelf.Lamar;

namespace Topshelf.Quartz.Lamar
{
    public static class LamarScheduleJobHostConfiguratorExtensions
    {
        public static HostConfigurator UseQuartzLamar(this HostConfigurator configurator, Container container)
        {
            configurator.UseLamar(container);

            LamarScheduleJobServiceConfiguratorExtensions.SetupLamar();

            return configurator;
        }

        public static HostConfigurator UseQuartzLamar(this HostConfigurator configurator)
        {
            var container = LamarBuilderConfigurator.Container;

            if (container == null) throw new ArgumentNullException(nameof(container));

            configurator.UseLamar(container);

            LamarScheduleJobServiceConfiguratorExtensions.SetupLamar();

            return configurator;
        }
    }
}
