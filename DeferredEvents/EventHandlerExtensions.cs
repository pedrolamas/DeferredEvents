using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DeferredEvents
{
    public static class EventHandlerExtensions
    {
        private static readonly Task CompletedTask = Task.FromResult(0);

        public static Task InvokeAsync<T>(this EventHandler<T> eventHandler, object sender, T eventArgs)
            where T : DeferredEventArgs
        {
            return InvokeAsync(eventHandler, sender, eventArgs, CancellationToken.None);
        }

        public static Task InvokeAsync<T>(this EventHandler<T> eventHandler, object sender, T eventArgs, CancellationToken cancellationToken)
            where T : DeferredEventArgs
        {
            if (eventHandler == null)
            {
                return CompletedTask;
            }

            var tasks = eventHandler.GetInvocationList()
                .OfType<EventHandler<T>>()
                .Select(invocationDelegate =>
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    invocationDelegate(sender, eventArgs);

                    var deferral = eventArgs.GetCurrentDeferralAndReset();

                    return deferral?.WaitForCompletion(cancellationToken) ?? CompletedTask;
                })
                .ToArray();

            return Task.WhenAll(tasks);
        }
    }
}