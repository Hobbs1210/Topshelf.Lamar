using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lamar;
using Quartz;
using Quartz.Spi;

namespace Topshelf.Lamar
{
    public class LamarJobFactory : IJobFactory
    {
        private readonly IContainer _container;

        public LamarJobFactory(IContainer container)
        {
            _container = container;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var jobType = bundle.JobDetail.JobType;

            try
            {
                return _container.GetInstance(jobType) as IJob;
            }
            catch (Exception ex)
            {
                throw new SchedulerConfigException(string.Format(CultureInfo.InvariantCulture, $"Failed to instantiate Job '{bundle.JobDetail.Key}' of type '{bundle.JobDetail.JobType}'"), ex);
            }
        }

        public void ReturnJob(IJob job)
        {
        }
    }
}