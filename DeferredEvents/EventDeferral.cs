using System;
using System.Threading;
using System.Threading.Tasks;

namespace DeferredEvents
{
    public class EventDeferral : IDisposable
    {
        private readonly TaskCompletionSource<object> _taskCompletionSource = new TaskCompletionSource<object>();

        internal EventDeferral()
        {
        }

        public void Complete() => _taskCompletionSource.TrySetResult(null);

        internal async Task WaitForCompletion(CancellationToken cancellationToken)
        {
            using (cancellationToken.Register(() => _taskCompletionSource.TrySetCanceled()))
            {
                await _taskCompletionSource.Task;
            }
        }

        public void Dispose()
        {
            Complete();
        }
    }
}