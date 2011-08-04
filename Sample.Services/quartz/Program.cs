using System;
using Quartz;
using Quartz.Impl;

namespace quartz
{
    class Program
    {
        static void Main()
        {
            // construct a scheduler factory
            var schedFact = new StdSchedulerFactory();

            // get a scheduler
            IScheduler sched = schedFact.GetScheduler();
            sched.Start();

            // construct job info
            var jobDetail = new JobDetail("myJob", null, typeof(DumbJob));
            // fire every 5 seconds.
            var trigger = TriggerUtils.MakeSecondlyTrigger(5);
            // start on the next even hour
            trigger.StartTimeUtc = TriggerUtils.GetEvenHourDate(DateTime.UtcNow);
            trigger.Name = "myTrigger";
            sched.ScheduleJob(jobDetail, trigger); 
        }
    }

    public class DumbJob : IJob
    {
        public DumbJob()
        {
        }

        public void Execute(JobExecutionContext context)
        {
            Console.WriteLine("DumbJob is executing.");
        }
    } 

}
