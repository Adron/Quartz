using System;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

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

    public class IoCJobFactory : IJobFactory
    {
        // Castle Windsor conatainer locator
        readonly IServiceLocator locator;

        public IoCJobFactory(IServiceLocator locator)
        {
            this.locator = locator;
        }

        public IJob NewJob(TriggerFiredBundle bundle)
        {
            try
            {
                JobDetail jobDetail = bundle.JobDetail;
                Type jobType = jobDetail.JobType;

                // Return job that is registrated in container
                return (IJob)locator.GetInstance(jobType);
            }
            catch (Exception e)
            {
                SchedulerException se = new SchedulerException(
                                        "Problem instantiating class", e);
                throw se;
            }
        }
    }

    public class WindsorServiceLocator : IServiceLocator
    {
        private readonly IWindsorContainer container;

        public WindsorServiceLocator(IWindsorContainer container)
        {
            this.container = container;
        }

        public object GetService(Type serviceType)
        {
            return container.GetService(serviceType);
        }

        public object GetInstance(Type serviceType)
        {
            return container.Resolve(serviceType);
        }

        public object GetInstance(Type serviceType, string key)
        {
            if (key != null)
                return container.Resolve(key, serviceType);
            return container.Resolve(serviceType);
        }

        public IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return (object[])container.ResolveAll(serviceType);
        }

        public TService GetInstance<TService>()
        {
            return container.Resolve<TService>();
        }

        public TService GetInstance<TService>(string key)
        {
            return container.Resolve<TService>(key);
        }

        public IEnumerable<TService> GetAllInstances<TService>()
        {
            return container.ResolveAll<TService>();
        }
    }
}
