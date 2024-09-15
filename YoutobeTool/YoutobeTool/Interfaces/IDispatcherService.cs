using System;
using System.Threading.Tasks;

namespace YoutobeTool.Interfaces
{
    public interface IDispatcherService
    {
        void Enqueue(Action action);
        Task EnqueueAsync(Func<Task> asyncAction);
    }
}
