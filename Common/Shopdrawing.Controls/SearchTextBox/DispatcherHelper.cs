using System;
using System.Windows.Threading;

namespace Shopdrawing.Controls
{
    public class DispatcherHelper
    {
        private DispatcherFrame Frame;

        public DispatcherHelper()
        {
            this.Frame = new DispatcherFrame();
        }

        public void ClearFrames(Dispatcher dispatcher)
        {
            dispatcher.BeginInvoke(DispatcherPriority.SystemIdle, new DispatcherOperationCallback(this.ExitFrame), this.Frame);
            Dispatcher.PushFrame(this.Frame);
        }

        private object ExitFrame(object frame)
        {
            ((DispatcherFrame)frame).Continue = false;
            this.Frame = null;
            return null;
        }

        public void ExitFrame()
        {
            if (this.Frame != null)
            {
                this.ExitFrame(this.Frame);
            }
        }
    }
}