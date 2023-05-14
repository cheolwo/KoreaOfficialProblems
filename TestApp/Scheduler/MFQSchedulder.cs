using Quartz.Impl;
using Quartz;
using TestApp.Service;

namespace TestApp.Scheduler
{
    //public class MFQSchedulder
    //{
    //    private readonly IServiceProvider _serviceProvider;
    //    private StdSchedulerFactory schedulerFactory = new StdSchedulerFactory();
    //    public MFQSchedulder(IServiceProvider serviceProvider)
    //    {
    //        _serviceProvider = serviceProvider;
    //    }
    //    public async Task Start()
    //    {
    //        // 스케줄러 인스턴스 생성
    //        var scheduler = await schedulerFactory.GetScheduler();
    //        IExcuteJobMFQ? excuteJobMFQ = _serviceProvider.GetService<IExcuteJobMFQ>();
    //        IInsertJobMFQ? insertJobMFQ = _serviceProvider.GetService<IInsertJobMFQ>();
    //        ICordinateJobMFQ? cordinateJobMFQ = _serviceProvider.GetService<ICordinateJobMFQ>();

    //        if (excuteJobMFQ != null && insertJobMFQ != null && cordinateJobMFQ != null)
    //        {
    //            var ExcuteJob = JobBuilder.Create<ExcuteJob>()
    //            .WithIdentity("ExcuteJob", "group1")
    //            .SetJobData(new JobDataMap
    //            {
    //                    new KeyValuePair<string, object>("IExcuteJobMFQ", excuteJobMFQ)
    //            }).Build();

    //            var InsertJob  = JobBuilder.Create<InsertJob>()
    //            .WithIdentity("InsertJob", "group1")
    //            .SetJobData(new JobDataMap
    //            {
    //                    new KeyValuePair<string, object>("IInsertJobMFQ", insertJobMFQ)
    //            }).Build();

    //            var CordinateJob = JobBuilder.Create<CordinateJob>()
    //            .WithIdentity("CordinateJob", "group1")
    //            .SetJobData(new JobDataMap
    //            {
    //                    new KeyValuePair<string, object>("ICordinateJobMFQ", cordinateJobMFQ)
    //            }).Build();
    //            // 작업1(Job1)을 1초마다 실행하는 트리거(Trigger1) 생성
    //            var trigger1 = TriggerBuilder.Create()
    //                .WithIdentity("trigger1", "group1")
    //                .StartNow()
    //                .WithSimpleSchedule(x => x
    //                    .WithIntervalInSeconds(10)
    //                    .RepeatForever())
    //                .WithPriority(1)
    //                .Build();

    //            // 작업1(Job1)을 1초마다 실행하는 트리거(Trigger1) 생성
    //            var trigger2 = TriggerBuilder.Create()
    //                .WithIdentity("trigger2", "group1")
    //                .StartNow()
    //                .WithSimpleSchedule(x => x
    //                    .WithIntervalInSeconds(1)
    //                    .RepeatForever())
    //                .WithPriority(2)
    //                .Build();

    //            // 작업2(Job2)를 3초마다 실행하는 트리거(Trigger2) 생성
    //            var trigger3 = TriggerBuilder.Create()
    //                .WithIdentity("trigger3", "group1")
    //                .StartNow()
    //                .WithSimpleSchedule(x => x
    //                    .WithIntervalInSeconds(3)
    //                    .RepeatForever())
    //                .WithPriority(3)
    //                .Build();

    //            await scheduler.ScheduleJob(InsertJob, trigger1);
    //            await scheduler.ScheduleJob(ExcuteJob, trigger2);
    //            await scheduler.ScheduleJob(CordinateJob, trigger3);
    //            await scheduler.Start();
    //        }
    //        else throw new ArgumentNullException("Not Configuing Services");       
    //    }
    //}
}
