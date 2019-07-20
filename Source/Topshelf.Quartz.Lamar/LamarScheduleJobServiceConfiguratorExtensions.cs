using Quartz;
using System;
using Quartz.Impl;
using Quartz.Spi;
using Topshelf.Lamar;
using Topshelf.Logging;
using Topshelf.ServiceConfigurators;

namespace Topshelf.Quartz.Lamar
{
    public static class LamarScheduleJobServiceConfiguratorExtensions
    {
        public static ServiceConfigurator<T> UseQuartzLamar<T>(this ServiceConfigurator<T> configurator)  where T : class
        {
            SetupLamar();

            return configurator;
        }

        internal static void SetupLamar()
        {
            var log = HostLogger.Get(typeof(LamarScheduleJobServiceConfiguratorExtensions));


            var container = LamarBuilderConfigurator.Container;
            if (container == null) throw new ArgumentNullException(nameof(container));

            container.Configure(new QuartzLamarService());

            IScheduler SchedulerFunc() => container.GetInstance<IScheduler>();

            ScheduleJobServiceConfiguratorExtensions.SchedulerFactory = SchedulerFunc;

            log.Info("[Topshelf.Quartz.Lamar] Quartz configured to construct jobs with Lamar.");
        }

    }
}
