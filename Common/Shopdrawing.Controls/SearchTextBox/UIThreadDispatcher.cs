using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Threading;

namespace Shopdrawing.Controls
{
    public sealed class UIThreadDispatcher
    {
        private Dispatcher uiThreadDispatcher;

        private static UIThreadDispatcher dispatcher;

        public DispatcherHooks Hooks
        {
            get
            {
                if (this.uiThreadDispatcher == null)
                {
                    return null;
                }
                return this.uiThreadDispatcher.Hooks;
            }
        }

        public static UIThreadDispatcher Instance
        {
            get
            {
                return UIThreadDispatcher.dispatcher;
            }
            set
            {
                UIThreadDispatcher.dispatcher = value;
            }
        }

        public bool IsUIThread
        {
            get
            {
                return this.uiThreadDispatcher == Dispatcher.CurrentDispatcher;
            }
        }

        public UIThreadDispatcher()
        {
            this.uiThreadDispatcher = Dispatcher.CurrentDispatcher;
        }

        public DispatcherOperation BeginInvoke(DispatcherPriority priority, Action action)
        {
            if (this.uiThreadDispatcher.HasShutdownStarted)
            {
                return null;
            }
            return this.uiThreadDispatcher.BeginInvoke(priority, action);
        }

        public DispatcherOperation BeginInvoke<TResult>(DispatcherPriority priority, Func<TResult> func)
        {
            if (this.uiThreadDispatcher.HasShutdownStarted)
            {
                return null;
            }
            return this.uiThreadDispatcher.BeginInvoke(priority, func);
        }

        public DispatcherOperation BeginInvoke<T>(DispatcherPriority priority, Action<T> action, T arg)
        {
            if (this.uiThreadDispatcher.HasShutdownStarted)
            {
                return null;
            }
            return this.uiThreadDispatcher.BeginInvoke(priority, action, arg);
        }

        public DispatcherOperation BeginInvoke<T, TResult>(DispatcherPriority priority, Func<T, TResult> func, T arg)
        {
            if (this.uiThreadDispatcher.HasShutdownStarted)
            {
                return null;
            }
            return this.uiThreadDispatcher.BeginInvoke(priority, func, arg);
        }

        public DispatcherOperation BeginInvoke<T1, T2>(DispatcherPriority priority, Action<T1, T2> action, T1 arg1, T2 arg2)
        {
            if (this.uiThreadDispatcher.HasShutdownStarted)
            {
                return null;
            }
            Dispatcher dispatcher = this.uiThreadDispatcher;
            object obj = arg1;
            object[] objArray = new object[] { arg2 };
            return dispatcher.BeginInvoke(priority, action, obj, objArray);
        }

        public void DoEvents()
        {
            (new DispatcherHelper()).ClearFrames(this.uiThreadDispatcher);
        }

        public static void InitializeInstance()
        {
            UIThreadDispatcher.Instance = new UIThreadDispatcher();
        }

        public void Invoke(DispatcherPriority priority, Action action)
        {
            if (!this.uiThreadDispatcher.HasShutdownStarted)
            {
                this.uiThreadDispatcher.Invoke(priority, action);
            }
        }

        public void Invoke<T>(DispatcherPriority priority, Action<T> action, T arg)
        {
            if (!this.uiThreadDispatcher.HasShutdownStarted)
            {
                this.uiThreadDispatcher.Invoke(priority, action, arg);
            }
        }

        public void Invoke<T1, T2>(DispatcherPriority priority, Action<T1, T2> action, T1 arg1, T2 arg2)
        {
            if (!this.uiThreadDispatcher.HasShutdownStarted)
            {
                Dispatcher dispatcher = this.uiThreadDispatcher;
                object obj = arg1;
                object[] objArray = new object[] { arg2 };
                dispatcher.Invoke(priority, action, obj, objArray);
            }
        }

        public TResult Invoke<TResult>(DispatcherPriority priority, Func<TResult> func)
        {
            if (this.uiThreadDispatcher.HasShutdownStarted)
            {
                return default(TResult);
            }
            return (TResult)this.uiThreadDispatcher.Invoke(priority, func);
        }

        public void InvokeAfter(TimeSpan delay, DispatcherPriority priority, Action action)
		{
			Timer timer = null;
            //timer = new Timer((object o) => {
            //    this.<>4__this.Invoke(this.priority, this.action);
            //    if (this.startTimer != null)
            //    {
            //        this.startTimer.Dispose();
            //    }
            //}, null, delay, new TimeSpan((long)0));
		}
    }
}