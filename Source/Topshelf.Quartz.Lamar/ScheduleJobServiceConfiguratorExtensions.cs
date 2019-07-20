using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quartz;
using Quartz.Spi;
using Topshelf.Logging;
using Topshelf.ServiceConfigurators;

namespace Topshelf.Quartz.Lamar
{
    public static class ScheduleJobServiceConfiguratorExtensions
    {
        private static IScheduler _scheduler;
        internal static IJobFactory JobFactory;

        internal static Func<IScheduler> SchedulerFactory { get; set; }

        private static IScheduler GetScheduler()
        {
            var scheduler = SchedulerFactory();

            if (JobFactory != null)
                scheduler.JobFactory = JobFactory;

            return scheduler;
        }

        public static ServiceConfigurator<T> UsingQuartzJobFactory<T, TJobFactory>(this ServiceConfigurator<T> configurator, Func<TJobFactory> jobFactory)
            where T : class
            where TJobFactory : IJobFactory
        {
            JobFactory = jobFactory();
            return configurator;
        }

        public static ServiceConfigurator<T> ScheduleQuartzJob<T>(this ServiceConfigurator<T> configurator, Action<QuartzConfigurator> jobConfigurator) where T : class
        {
            ConfigureJob(configurator, jobConfigurator);
            return configurator;
        }

        private static void ConfigureJob<T>(ServiceConfigurator<T> configurator,
            Action<QuartzConfigurator> jobConfigurator, bool replaceJob = false) where T : class
        {
            var log = HostLogger.Get(typeof(ScheduleJobServiceConfiguratorExtensions));

            var jobConfig = new QuartzConfigurator();

            jobConfigurator(jobConfig);

            if (jobConfig.JobEnabled == null || jobConfig.JobEnabled() || jobConfig.Job == null ||
                jobConfig.Triggers == null)
            {
                var jobDetail = jobConfig.Job();
                var jobTriggers = jobConfig.Triggers.Select(triggerFactory => triggerFactory())
                    .Where(trigger => trigger != null);
                var jobListeners = jobConfig.JobListeners;
                var triggerListeners = jobConfig.TriggerListeners;
                var scheduleListeners = jobConfig.ScheduleListeners;

                configurator.BeforeStartingService(() =>
                {
                    log.Debug("[Topshelf.Lamar] Scheduler starting up...");

                    if (_scheduler == null)
                        _scheduler = GetScheduler();

                    if (_scheduler != null && jobDetail != null && jobTriggers.Any())
                    {
                        var triggersForJob = new HashSet<ITrigger>(jobTriggers);
                        _scheduler.ScheduleJob(jobDetail, triggersForJob, false);
                        log.Info($"[Topshelf.Quartz] Scheduled Job: {jobDetail.Key}");

                        foreach (var trigger in triggersForJob)
                        {
                            log.Info($"[Topshelf.Quartz] Job Schedule: {trigger} - Next Fire Time (local): {(trigger.GetNextFireTimeUtc().HasValue ? trigger.GetNextFireTimeUtc().Value.ToLocalTime().ToString() : "none")}");
                        }

                        foreach (var jobListenerTuple in jobListeners)
                        {
                            var jobListener = jobListenerTuple.Item1();
                            var keyEquals = jobListenerTuple.Item2;

                            if (jobListener != null)
                            {
                                _scheduler.ListenerManager.AddJobListener(jobListener, keyEquals);
                            }
                        }

                        foreach (var triggerListenerTuple in jobConfig.TriggerListeners)
                        {
                            var triggerListener = triggerListenerTuple.Item1();
                            var keyEquals = triggerListenerTuple.Item2;

                            if (triggerListener != null)
                            {
                                _scheduler.ListenerManager.AddTriggerListener(triggerListener, keyEquals);
                            }
                        }

                        foreach (var scheduleListener in scheduleListeners)
                        {
                            if (scheduleListener != null)
                            {
                                var listener = scheduleListener();
                                _scheduler.ListenerManager.AddSchedulerListener(listener);
                            }
                        }

                        if (!_scheduler.IsStarted)
                        {
                            _scheduler.Start();
                            log.Info("[Topshelf.Quartz] Scheduler started...");
                        }
                    }
                });

                configurator.BeforeStoppingService(() =>
                {
                    log.Debug("[Topshelf.Quartz] Scheduler shutting down...");
                    _scheduler.Shutdown(true);
                    log.Info("[Topshelf.Quartz] Scheduler shut down...");
                });
            }
        }
    }
}