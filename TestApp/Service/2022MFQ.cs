using Microsoft.EntityFrameworkCore;
using Quartz;
using System.ComponentModel.DataAnnotations;

namespace MFQ2022
{
    public class Process
    {
        [Key]
        public int No { get; set; }
        public string? Name { get; set; }
        public int ArrivingTime { get; set; }
        public int RemainingTime { get; set; }
        public int ExcutingTime { get; set; }
        public int CompleteTime { get; set; }
    }
    public class ProcessDbContext : DbContext
    {
        public ProcessDbContext(DbContextOptions<ProcessDbContext> options)
            : base(options)
        {

        }

        public DbSet<Process> Processes { get; set; }
    }
    public class MFQService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<MFQService> _logger;
        private List<Process> Processes = new List<Process>();
        private PriorityQueue<Process, int> PriorityQueue1 = new PriorityQueue<Process, int>();
        private PriorityQueue<Process, int> PriorityQueue2 = new PriorityQueue<Process, int>();
        private int CurrentTime = 1;
        private int TimeSlice = 3;
        public MFQService(IServiceScopeFactory scopeFactory, ILogger<MFQService> logger)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }
        public async Task ExcuteServiceAsnyc()
        {
            _logger.LogInformation(nameof(MFQService.ExcuteServiceAsnyc));
            using (var scope = _scopeFactory.CreateScope())
            {
                var DbContext = scope.ServiceProvider.GetService<ProcessDbContext>();
                if (DbContext != null)
                {
                    while (PriorityQueue1.Count > 0 && TimeSlice > 0)
                    {
                        await Excute1(PriorityQueue1.Dequeue(), DbContext, _logger);
                    }
                    while (PriorityQueue2.Count > 0 && TimeSlice > 0)
                    {
                        await Excute2(PriorityQueue2.Dequeue(), DbContext, _logger);
                    }
                }
            }
            TimeSlice = 3;
        }
        public async Task InsertServiceAsync()
        {
            _logger.LogInformation(nameof(MFQService.InsertServiceAsync));
            
            using (var scope = _scopeFactory.CreateScope())
            {
                var DbContext = scope.ServiceProvider.GetService<ProcessDbContext>();
                if (DbContext != null)
                {
                    _logger.LogInformation("DbContext NULL 아닙니다");
                    var DbProcesses = await DbContext.Processes.Where(e => e.RemainingTime > 0).ToListAsync();

                    if (DbProcesses.Count > 0)
                    {
                        _logger.LogInformation("우선순위 큐에 프로세스를 Enque 합니다.");
                        foreach (var process in DbProcesses)
                        {
                            var Current = Processes.FirstOrDefault(e => e.No.Equals(process.No));
                            if (Current == null && process.ArrivingTime <= CurrentTime)
                            {
                                _logger.LogInformation($"Process Name : {process.Name}");
                                Processes.Add(process);
                                PriorityQueue1.Enqueue(process, process.ArrivingTime);
                            }
                        }
                    }
                }
            }
            CurrentTime++;
        }
        
        public Task CurrentTimeUpdate()
        {
            CurrentTime++;
            return Task.CompletedTask;
        }
        private async Task Excute1(Process Process, DbContext DbContext, ILogger<MFQService> _logger)
        {
            _logger.LogInformation("PriorityQue 1 동작합니다.");
            DbContext.Entry(Process).State = EntityState.Modified;
            while (TimeSlice > 0 && Process.RemainingTime > 0)
            {
                TimeSlice--;
                Process.RemainingTime--;
                Process.ExcutingTime++;
                CurrentTime++;
            }
          
            _logger.LogInformation($"Process Name : {Process.Name}");
            _logger.LogInformation($"ExcutingTime : {Process.ExcutingTime}");
            _logger.LogInformation($"Process ArrivingTime : {Process.ArrivingTime}");
            await DbContext.SaveChangesAsync();
          
            if (Process.RemainingTime > 0)
            {
                if (PriorityQueue1.Count > 0)
                {
                    PriorityQueue2.Enqueue(Process, PriorityQueue1.Count / (PriorityQueue1.Count + 1) * Process.ExcutingTime);
                }
                else
                {
                    PriorityQueue2.Enqueue(Process, 0);
                }
            }
            if(Process.RemainingTime == 0)
            {
                DbContext.Entry(Process).State = EntityState.Modified;
                Process.CompleteTime = CurrentTime;
                await DbContext.SaveChangesAsync();
            }
        }
        private async Task Excute2(Process Process, DbContext DbContext, ILogger<MFQService> _logger)
        {
            _logger.LogInformation("PriorityQue2 동작합니다.");
            DbContext.Entry(Process).State = EntityState.Modified;
            while (TimeSlice > 0 && Process.RemainingTime > 0)
            {
                TimeSlice--;
                Process.RemainingTime--;
                Process.ExcutingTime++;
                CurrentTime++;
            }
            _logger.LogInformation($"Process Name : {Process.Name}");
            _logger.LogInformation($"Process ExcutingTime : {Process.ExcutingTime}");
            _logger.LogInformation($"Process ArrivingTime : {Process.ArrivingTime}");
            await DbContext.SaveChangesAsync();
            if (Process.RemainingTime > 0)
            {
                if(PriorityQueue2.Count > 0) 
                {
                    PriorityQueue2.Enqueue(Process, PriorityQueue2.Count / (PriorityQueue2.Count + 1) * Process.ExcutingTime);
                }
                else
                {
                    PriorityQueue2.Enqueue(Process, 0);
                }
            }
            if (Process.RemainingTime == 0)
            {
                DbContext.Entry(Process).State = EntityState.Modified;
                Process.CompleteTime = CurrentTime;
                await DbContext.SaveChangesAsync();
            }
        }
    }
    public class InsertJob : IJob
    {
        private readonly MFQService _MFQService;
        public InsertJob(MFQService mFQService)
        {
            _MFQService = mFQService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await _MFQService.InsertServiceAsync();
        }
    }
    public class ExcuteJob : IJob
    {
        private readonly MFQService _MFQService;
        public ExcuteJob(MFQService mFQService)
        {
            _MFQService = mFQService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await _MFQService.ExcuteServiceAsnyc();
        }
    }
}
