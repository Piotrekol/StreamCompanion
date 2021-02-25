using System;
using System.Threading;
using System.Threading.Tasks;

namespace StreamCompanion.Common
{
    public static class TaskExtensions
    {
        public static Action<Exception> GlobalExceptionHandler { get; set; }

        public static Task HandleExceptions(this Task task)
        {
            return task.ContinueWith((task, state) =>
            {
                GlobalExceptionHandler(task.Exception);
            }, null, CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted,TaskScheduler.Default);
        }
    }
}