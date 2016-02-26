using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.Documents;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Threading;

namespace Microsoft.Expression.Project
{
	internal abstract class FileWatcherBase : IDisposable
	{
		private FileSystemWatcher watcher;

		private bool hasBeenDisposed;

		public FileSystemWatcher Watcher
		{
			get
			{
				return this.watcher;
			}
		}

		public FileWatcherBase()
		{
		}

		private void CheckForChangedOrDeletedFilesCore()
		{
			if (!this.hasBeenDisposed)
			{
				this.CheckForChangedOrDeletedItems();
			}
		}

		protected abstract void CheckForChangedOrDeletedItems();

		internal void CheckForChangesAndReenable()
		{
			if (!this.hasBeenDisposed)
			{
				this.CheckForChangedOrDeletedItems();
				this.EnableWatchingForChanges();
			}
		}

		protected void CreateFileWatcher(string path)
		{
			if (!Microsoft.Expression.Framework.Documents.PathHelper.DirectoryExists(path))
			{
				return;
			}
			PerformanceUtility.StartPerformanceSequence(PerformanceEvent.CreateFileWatcher);
			this.watcher = new FileSystemWatcher(path)
			{
				NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Attributes | NotifyFilters.Size | NotifyFilters.LastWrite | NotifyFilters.CreationTime
			};
			PerformanceUtility.MarkInterimStep(PerformanceEvent.CreateFileWatcher, "Add EventHandlers to FileSystemWatcher");
			this.watcher.Deleted += new FileSystemEventHandler(this.Watcher_Changed);
			this.watcher.Renamed += new RenamedEventHandler(this.Watcher_Changed);
			this.watcher.Changed += new FileSystemEventHandler(this.Watcher_Changed);
			this.UpdateFileInformation();
			this.EnableWatchingForChanges();
			PerformanceUtility.EndPerformanceSequence(PerformanceEvent.CreateFileWatcher);
		}

		public void DisableWatchingForChanges()
		{
			if (this.watcher != null)
			{
				this.watcher.EnableRaisingEvents = false;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.DisableWatchingForChanges();
				if (this.watcher != null)
				{
					this.watcher.Deleted -= new FileSystemEventHandler(this.Watcher_Changed);
					this.watcher.Renamed -= new RenamedEventHandler(this.Watcher_Changed);
					this.watcher.Changed -= new FileSystemEventHandler(this.Watcher_Changed);
					this.watcher.Dispose();
					this.watcher = null;
				}
				this.hasBeenDisposed = true;
			}
		}

		public void EnableWatchingForChanges()
		{
			if (this.watcher != null)
			{
				try
				{
					this.watcher.EnableRaisingEvents = true;
				}
				catch (IOException oException)
				{
				}
			}
		}

		public abstract void UpdateFileInformation();

		private void Watcher_Changed(object sender, FileSystemEventArgs e)
		{
			if (!this.hasBeenDisposed && (e.ChangeType == WatcherChangeTypes.Changed || e.ChangeType == WatcherChangeTypes.Deleted))
			{
				UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(this.CheckForChangedOrDeletedFilesCore));
			}
		}
	}
}