using Quartz;

namespace TestApp.Service
{
    public class SendEmailJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine("Hi Sample Job ");
          
            return Task.FromResult(true);
        }
    }
}
