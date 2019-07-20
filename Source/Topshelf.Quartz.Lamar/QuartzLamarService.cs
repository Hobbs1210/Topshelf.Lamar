using Lamar;
using System;
using System.Collections.Generic;
using System.Text;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using Topshelf.Lamar;

namespace Topshelf.Quartz.Lamar
{
    internal class QuartzLamarService : ServiceRegistry
    {
        public QuartzLamarService()
        {
            For<IJobFactory>().Use<LamarJobFactory>();
            For<ISchedulerFactory>().Use(new StdSchedulerFactory()).Singleton();
            For<IScheduler>().Use((service) =>
            {
                var scheduler = service.GetInstance<ISchedulerFactory>().GetScheduler().Result;
                scheduler.JobFactory = service.GetInstance<IJobFactory>();

                return scheduler;
            });
        }
    }
}