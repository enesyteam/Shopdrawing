using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignModel.Core
{
	public static class DispatcherHelper
	{
		private static Stopwatch stopwatch;

		private static long lastElapsedMilliseconds;

		private static bool doEventsDeferred;

		public static IDisposable DeferDoEvents()
		{
			return new DispatcherHelper.DeferDoEventsToken();
		}

		public static void DoEvents()
		{
			if (DispatcherHelper.doEventsDeferred)
			{
				return;
			}
			if (DispatcherHelper.stopwatch == null)
			{
				DispatcherHelper.stopwatch = new Stopwatch();
				DispatcherHelper.stopwatch.Reset();
				DispatcherHelper.stopwatch.Start();
				DispatcherHelper.lastElapsedMilliseconds = DispatcherHelper.stopwatch.ElapsedMilliseconds;
			}
			long elapsedMilliseconds = DispatcherHelper.stopwatch.ElapsedMilliseconds;
			if (elapsedMilliseconds - DispatcherHelper.lastElapsedMilliseconds > (long)1000)
			{
				DispatcherFrame dispatcherFrame = new DispatcherFrame();
				Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Send, new DispatcherOperationCallback((object arg) => {
					dispatcherFrame.Continue = false;
					return null;
				}), null);
				try
				{
					Dispatcher.PushFrame(dispatcherFrame);
				}
				catch (InvalidOperationException invalidOperationException)
				{
				}
				DispatcherHelper.lastElapsedMilliseconds = elapsedMilliseconds;
			}
		}

		private sealed class DeferDoEventsToken : IDisposable
		{
			private bool disposed;

			public DeferDoEventsToken()
			{
				DispatcherHelper.doEventsDeferred = true;
			}

			public void Dispose()
			{
				if (!this.disposed)
				{
					DispatcherHelper.doEventsDeferred = false;
					this.disposed = true;
				}
			}
		}
	}
}