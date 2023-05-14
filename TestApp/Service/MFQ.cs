namespace TestApp.Service
{
    //public interface IProcessRepository
    //{
    //    List<Process> GetProcesses();
    //}
    //public class ProcessRepostitory : IProcessRepository
    //{
    //    List<Process> _processes;
    //    public ProcessRepostitory()
    //    {
    //        _processes = new List<Process>
    //    {
    //        new Process("A", 0, 6, 6),
    //        new Process("B", 2, 4, 4),
    //        new Process("C", 4, 2, 2),
    //        new Process("D", 5, 2, 2)
    //    };
    //    }
    //    public List<Process> GetProcesses()
    //    {
    //        return _processes;
    //    }
    //}
    //public class Process
    //{
    //    public int ArrivingTime;
    //    public int ExcutingTime;
    //    public int RemainingTime;
    //    public string Name;
    //    public int PriorityValue = 0;
    //    public int Level = 0;
    //    public Process(string name, int arrivingTime, int excutingTime, int remainingTime)
    //    {
    //        Name = name;
    //        ArrivingTime = arrivingTime;
    //        ExcutingTime = excutingTime;
    //        RemainingTime = remainingTime;
    //    }
    //}
    //public class Que1Comparer : IComparer<int>
    //{
    //    public int Compare(int x, int y)
    //    {
    //        return x.CompareTo(y);
    //    }
    //}
    //public class Que2Comparer : IComparer<int>
    //{
    //    public int Compare(int x, int y)
    //    {
    //        return x.CompareTo(y);
    //    }
    //}
    //public interface IInsertJobMFQ
    //{
    //    Task InsertJob();
    //}
    //public interface IExcuteJobMFQ
    //{
    //    Task ExcuteJob();
    //}
    //public interface ICordinateJobMFQ
    //{
    //    Task CordinateJob();
    //}
    //public class MFQService : IInsertJobMFQ, IExcuteJobMFQ, ICordinateJobMFQ
    //{
    //    private readonly IProcessRepository ProcessRepository;
    //    private readonly ILogger<MFQService> _logger;
    //    private List<Process> processes;
    //    private PriorityQueue<Process, int> priorityQueue = new PriorityQueue<Process, int>(new Que1Comparer());
    //    private PriorityQueue<Process, int> priorityQueue2 = new PriorityQueue<Process, int>(new Que2Comparer());
    //    private PriorityQueue<Process, int> priorityQueue3 = new PriorityQueue<Process, int>(new Que2Comparer());
    //    private Stack<int> stack = new Stack<int>();
    //    private Process? CurrentProcess = null;
    //    private int CurrentTime = 0;

    //    public MFQService(IProcessRepository processRepository, ILogger<MFQService> logger)
    //    {
    //        this.ProcessRepository = processRepository;
    //        processes = ProcessRepository.GetProcesses();
    //        _logger = logger;
    //    }
    //    public Task ExcuteJob()
    //    {
    //        _logger.LogInformation("ExcuteJon 실행");
    //        if (CurrentProcess == null)
    //        {
    //            CurrentProcess = priorityQueue.Dequeue();
    //        }
    //        CurrentTime++;
    //        stack.Push(CurrentTime);
    //        return Task.CompletedTask;
    //    }
    //    public Task CordinateJob()
    //    {
    //        _logger.LogInformation("CordinateJob 실행");
    //        if (CurrentProcess != null)
    //        {
    //            while (stack.Count > 0)
    //            {
    //                CurrentProcess.RemainingTime--;
    //                stack.Pop();
    //            }
    //            CurrentProcess.Level++;
    //            CurrentProcess.PriorityValue = (priorityQueue.Count + priorityQueue2.Count) * 2 / ((priorityQueue.Count + priorityQueue2.Count) * 2 + 1);
    //            if (CurrentProcess.Level == 1)
    //            {
    //                priorityQueue2.Enqueue(CurrentProcess, CurrentProcess.PriorityValue);
    //            }
    //            if (CurrentProcess.Level >= 2)
    //            {
    //                priorityQueue3.Enqueue(CurrentProcess, CurrentProcess.PriorityValue);
    //            }
    //            if (priorityQueue.Count > 0) { CurrentProcess = priorityQueue.Dequeue(); return Task.CompletedTask; }
    //            else if (priorityQueue2.Count > 0) { CurrentProcess = priorityQueue2.Dequeue(); return Task.CompletedTask; }
    //            else { CurrentProcess = priorityQueue3.Dequeue(); return Task.CompletedTask; }
    //        }
    //        return Task.CompletedTask;
    //    }
    //    public Task InsertJob()
    //    {
    //        _logger.LogInformation("InsertJob 실행");
    //        var FilterProcess = processes.Where(e => e.ArrivingTime <= CurrentTime).ToList();
    //        foreach (var process in FilterProcess)
    //        {
    //            _logger.LogInformation($"Enque {process.Name}");
    //            priorityQueue.Enqueue(process, process.ArrivingTime);
    //        }
    //        return Task.CompletedTask;
    //    }
    //}

}
