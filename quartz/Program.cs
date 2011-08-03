using System;
using Quartz;
using Quartz.Impl;

namespace quartz
{
    class Program
    {
        static void Main(string[] args)
        {
            ISchedulerFactory schedFact = new StdSchedulerFactory();

            IScheduler sched = schedFact.GetScheduler();
            sched.Start();

            Console.WriteLine("started");

            JobDetail jobDetail = new JobDetail("myJob", null, typeof(HelloJob));
            Trigger trigger = TriggerUtils.MakeSecondlyTrigger();
           
            trigger.StartTimeUtc = TriggerUtils.GetEvenHourDate(DateTime.UtcNow);
            trigger.Name = "myTrigger";
            sched.ScheduleJob(jobDetail, trigger);
            sched.Start();

            Console.WriteLine("started again");
        }
    }

    internal class HelloJob : IJob
    {
        public void Execute(JobExecutionContext context)
        {
            Console.WriteLine("Doing something now: {0}", DateTime.Now);
        }
    }
}
