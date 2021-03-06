using System;
using System.Threading;

namespace Ben.Diagnostics
{
    // Tips of the Toub
    internal sealed class DetectBlockingSynchronizationContext : SynchronizationContext
    {
        private readonly BlockingMonitor _monitor;
        private readonly SynchronizationContext _syncCtx;

        public DetectBlockingSynchronizationContext(BlockingMonitor monitor)
        {
            _monitor = monitor;

            SetWaitNotificationRequired();
        }

        public DetectBlockingSynchronizationContext(BlockingMonitor monitor, SynchronizationContext syncCtx) : this(monitor)
        {
            _syncCtx = syncCtx;
        }

        public override int Wait(IntPtr[] waitHandles, bool waitAll, int millisecondsTimeout)
        {
            _monitor.BlockingStart(DectectionSource.SynchronizationContext);

            try
            {
                if (_syncCtx != null)
                {
                    return _syncCtx.Wait(waitHandles, waitAll, millisecondsTimeout);
                }
                else
                {
                    return base.Wait(waitHandles, waitAll, millisecondsTimeout);
                }
            }
            finally
            {
                _monitor.BlockingEnd();
            }
        }
    }
}
