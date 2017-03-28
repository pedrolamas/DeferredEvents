using System.Threading.Tasks;

namespace DeferredEvents
{
    public class EventDeferral
    {
        private readonly TaskCompletionSource<object> _taskCompletionSource = new TaskCompletionSource<object>();

        public void Complete() => _taskCompletionSource.TrySetResult(true);

        internal Task GetTask() => _taskCompletionSource.Task;
    }
}