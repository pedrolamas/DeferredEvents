using System.Threading.Tasks;

namespace DeferredEvents
{
    public class EventDeferral
    {
        private readonly TaskCompletionSource<object> _taskCompletionSource = new TaskCompletionSource<object>();

        internal EventDeferral()
        {
        }

        public void Complete() => _taskCompletionSource.TrySetResult(true);

        internal void Cancel() => _taskCompletionSource.TrySetCanceled();

        internal Task GetTask() => _taskCompletionSource.Task;
    }
}