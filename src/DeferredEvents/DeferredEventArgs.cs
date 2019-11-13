using System;

namespace DeferredEvents
{
    public class DeferredEventArgs : EventArgs
    {
        [Obsolete("Please use `new DeferredEventArgs()` instead of this field. Check here for more information: https://github.com/PedroLamas/DeferredEvents/issues/1", true)]
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

        internal EventDeferral GetCurrentDeferralAndReset()
        {
            lock (_eventDeferralLock)
            {
                var eventDeferral = _eventDeferral;

                _eventDeferral = null;

                return eventDeferral;
            }
        }
    }
}