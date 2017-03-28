using System;
using System.Threading.Tasks;

namespace DeferredEvents
{
    public class DeferredEventArgs : EventArgs
    {
        public new static readonly DeferredEventArgs Empty = new DeferredEventArgs();

        private readonly object _eventDeferralLock = new object();

        private EventDeferral _eventDeferral;

        public EventDeferral GetDeferral()
        {
            lock (_eventDeferralLock)
            {
                return _eventDeferral ?? (_eventDeferral = new EventDeferral());
            }
        }

        internal Task GetTaskAndReset()
        {
            lock (_eventDeferralLock)
            {
                var eventDeferral = _eventDeferral;

                _eventDeferral = null;

                return eventDeferral?.GetTask();
            }
        }
    }
}