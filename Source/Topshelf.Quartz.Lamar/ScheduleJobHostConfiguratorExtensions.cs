using System;
using System.Collections.Generic;
using System.Text;
using Quartz.Spi;
using Topshelf.HostConfigurators;

namespace Topshelf.Quartz.Lamar
{
    public static class ScheduleJobHostConfiguratorExtensions
    {
        public static HostConfigurator UsingQuartzJobFactory<TJobFactory>(this HostConfigurator configurator, Func<TJobFactory> jobFactory)
            where TJobFactory : IJobFactory
        {
            ScheduleJobServiceConfiguratorExtensions.JobFactory = jobFactory();
            return configurator;
        }

        public static HostConfigurator ScheduleQuartzJobAsService(this HostConfigurator configurator, Action<QuartzConfigurator> jobConfigurator)
        {
            configurator.Service<NullService>(s => s
                .ScheduleQuartzJob(jobConfigurator)
                .WhenStarted(p => p.Start())
                .WhenStopped(p => p.Stop())
                .ConstructUsing(settings => new NullService())
            );

            return configurator;
        }
    }
}
