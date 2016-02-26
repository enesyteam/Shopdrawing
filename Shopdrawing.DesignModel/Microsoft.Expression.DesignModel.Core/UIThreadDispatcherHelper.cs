using System;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignModel.Core
{
	public static class UIThreadDispatcherHelper
	{
		private static Dispatcher uiThreadDispatcher;

		public static bool IsUIThread
		{
			get
			{
				return UIThreadDispatcherHelper.uiThreadDispatcher == Dispatcher.CurrentDispatcher;
			}
		}

		public static void BeginInvoke(DispatcherPriority priority, Delegate method, object arg)
		{
			Dispatcher currentDispatcher = UIThreadDispatcherHelper.uiThreadDispatcher ?? Dispatcher.CurrentDispatcher;
			if (!currentDispatcher.HasShutdownStarted)
			{
				currentDispatcher.BeginInvoke(priority, method, arg);
			}
		}

		public static void Capture()
		{
			UIThreadDispatcherHelper.uiThreadDispatcher = Dispatcher.CurrentDispatcher;
		}

		public static void Invoke(DispatcherPriority priority, Delegate method, object arg)
		{
			Dispatcher dispatcher = (UIThreadDispatcherHelper.uiThreadDispatcher != null ? UIThreadDispatcherHelper.uiThreadDispatcher : Dispatcher.CurrentDispatcher);
			if (!dispatcher.HasShutdownStarted)
			{
				dispatcher.Invoke(priority, method, arg);
			}
		}

		public static void Invoke(DispatcherPriority dispatcherPriority, Action action)
		{
			Dispatcher dispatcher = (UIThreadDispatcherHelper.uiThreadDispatcher != null ? UIThreadDispatcherHelper.uiThreadDispatcher : Dispatcher.CurrentDispatcher);
			if (!dispatcher.HasShutdownStarted)
			{
				dispatcher.Invoke(action, dispatcherPriority, new object[0]);
			}
		}
	}
}