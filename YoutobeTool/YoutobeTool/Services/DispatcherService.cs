using Microsoft.UI.Dispatching;
using System;
using System.Threading.Tasks;
using YoutobeTool.Interfaces;

namespace YoutobeTool.Services
{
    public class DispatcherService : IDispatcherService
    {
        private readonly DispatcherQueue _dispatcherQueue;

        public DispatcherService()
        {
            _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        }

        public void Enqueue(Action action)
        {
            _dispatcherQueue.TryEnqueue(() => action());
        }
        public Task EnqueueAsync(Func<Task> asyncAction)
        {
            var tcs = new TaskCompletionSource<object>();

            _dispatcherQueue.TryEnqueue(async () =>
            {
                try
                {
                    await asyncAction();
                    tcs.SetResult(null);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });

            return tcs.Task;
        }
    }
}
