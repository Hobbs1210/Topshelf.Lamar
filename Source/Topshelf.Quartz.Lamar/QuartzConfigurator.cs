using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Quartz;

namespace Topshelf.Quartz.Lamar
{
    public class QuartzConfigurator
    {
        public Func<IJobDetail> Job { get; set; }
        public ICollection<Func<ITrigger>> Triggers { get; set; }
        public ICollection<Tuple<Func<IJobListener>, IMatcher<JobKey>[]>> JobListeners { get; set; }
        public ICollection<Tuple<Func<ITriggerListener>, IMatcher<TriggerKey>[]>> TriggerListeners { get; set; }
        public ICollection<Func<ISchedulerListener>> ScheduleListeners { get; set; }

        public Func<bool> JobEnabled { get; private set; }

        public QuartzConfigurator()
        {
            Triggers = new Collection<Func<ITrigger>>();
            JobListeners = new Collection<Tuple<Func<IJobListener>, IMatcher<JobKey>[]>>();
            TriggerListeners = new Collection<Tuple<Func<ITriggerListener>, IMatcher<TriggerKey>[]>>();
            ScheduleListeners = new Collection<Func<ISchedulerListener>>();
        }

        public QuartzConfigurator WithJob(Func<IJobDetail> jobDetail)
        {
            Job = jobDetail;
            return this;
        }

        public QuartzConfigurator AddTrigger(Func<ITrigger> jobTrigger)
        {
            Triggers.Add(jobTrigger);
            return this;
        }

        public QuartzConfigurator WithTriggers(IEnumerable<ITrigger> jobTriggers)
        {
            foreach (var jobTrigger in jobTriggers)
            {
                var trigger = jobTrigger;
                AddTrigger(() => trigger);
            }
            return this;
        }

        public QuartzConfigurator EnableJobWhen(Func<bool> jobEnabled)
        {
            JobEnabled = jobEnabled;
            return this;
        }

        public QuartzConfigurator WithJobListener(Func<IJobListener> jobListener, params IMatcher<JobKey>[] keyEquals)
        {
            JobListeners.Add(Tuple.Create(jobListener, keyEquals));
            return this;
        }

        public QuartzConfigurator WithTriggerListener(Func<ITriggerListener> triggerListener, params IMatcher<TriggerKey>[] keyEquals)
        {
            TriggerListeners.Add(Tuple.Create(triggerListener, keyEquals));
            return this;
        }

        public QuartzConfigurator WithScheduleListener(Func<ISchedulerListener> scheduleListener)
        {
            ScheduleListeners.Add(scheduleListener);
            return this;
        }
    }
}