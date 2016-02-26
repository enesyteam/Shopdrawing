using Microsoft.Expression.Framework.Documents;
using System;
using System.Threading;

namespace Microsoft.Expression.Project
{
	public static class CurrentDirectoryHelper
	{
		public static IDisposable SetCurrentDirectory(string path)
		{
			return new CurrentDirectoryHelper.CurrentDirectoryToken(path, true);
		}

		public static void SetWorkingDirectory(string workingDirectory)
		{
			using (CurrentDirectoryHelper.CurrentDirectoryToken currentDirectoryToken = new CurrentDirectoryHelper.CurrentDirectoryToken(workingDirectory, false))
			{
			}
		}

		private class CurrentDirectoryToken : MarshalByRefObject, IDisposable
		{
			private bool isDisposed;

			public static Mutex mutex;

			private string oldDirectory;

			private bool restoreOnDispose;

			static CurrentDirectoryToken()
			{
				CurrentDirectoryHelper.CurrentDirectoryToken.mutex = new Mutex();
			}

			public CurrentDirectoryToken(string path, bool restoreOnDispose)
			{
				this.restoreOnDispose = restoreOnDispose;
				CurrentDirectoryHelper.CurrentDirectoryToken.mutex.WaitOne();
				this.oldDirectory = Environment.CurrentDirectory;
				if (PathHelper.DirectoryExists(path))
				{
					Environment.CurrentDirectory = path;
				}
			}

			private void Dispose(bool disposing)
			{
				if (!this.isDisposed && disposing)
				{
					try
					{
						if (this.restoreOnDispose && !string.IsNullOrEmpty(this.oldDirectory) && PathHelper.DirectoryExists(this.oldDirectory))
						{
							Environment.CurrentDirectory = this.oldDirectory;
						}
					}
					finally
					{
						this.isDisposed = true;
						CurrentDirectoryHelper.CurrentDirectoryToken.mutex.ReleaseMutex();
					}
				}
			}

			public void Dispose()
			{
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}

			protected override void Finalize()
			{
				try
				{
					this.Dispose(false);
				}
				finally
				{
					base.Finalize();
				}
			}

			public override object InitializeLifetimeService()
			{
				return null;
			}
		}
	}
}