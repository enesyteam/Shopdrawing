using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Microsoft.Expression.Project
{
	internal class StructuredStorage : IDisposable
	{
		private Microsoft.Expression.Project.NativeMethods.IStorage iStorage;

		private StructuredStorage(Microsoft.Expression.Project.NativeMethods.IStorage iStorage)
		{
			if (iStorage == null)
			{
				throw new ArgumentNullException("iStorage");
			}
			this.iStorage = iStorage;
		}

		public static StructuredStorage CreateStorage(string fileName)
		{
			Microsoft.Expression.Project.NativeMethods.IStorage storage;
			int num = Microsoft.Expression.Project.NativeMethods.StgCreateDocfile(fileName, Microsoft.Expression.Project.NativeMethods.STGM.READWRITE | Microsoft.Expression.Project.NativeMethods.STGM.SHARE_EXCLUSIVE | Microsoft.Expression.Project.NativeMethods.STGM.CREATE, 0, out storage);
			if (num > -2147287007)
			{
				if (num == -2147286788)
				{
					return null;
				}
				if (num == 0)
				{
					return new StructuredStorage(storage);
				}
			}
			else
			{
				switch (num)
				{
					case -2147287036:
					case -2147287035:
					case -2147287032:
					{
						return null;
					}
					case -2147287034:
					case -2147287033:
					{
						break;
					}
					default:
					{
						switch (num)
						{
							case -2147287008:
							case -2147287007:
							{
								return null;
							}
						}
						break;
					}
				}
			}
			return null;
		}

		public Stream CreateStream(string streamName)
		{
			Microsoft.Expression.Project.NativeMethods.IStream stream;
			Microsoft.Expression.Project.NativeMethods.STGM sTGM = Microsoft.Expression.Project.NativeMethods.STGM.WRITE | Microsoft.Expression.Project.NativeMethods.STGM.SHARE_EXCLUSIVE | Microsoft.Expression.Project.NativeMethods.STGM.CREATE;
			int num = this.iStorage.CreateStream(streamName, sTGM, 0, 0, out stream);
			int num1 = num;
			if (num1 > -2147286788)
			{
				if (num1 == -2147286782)
				{
					return null;
				}
				if (num1 == 0)
				{
					return new ComStream(stream);
				}
			}
			else
			{
				switch (num1)
				{
					case -2147287036:
					case -2147287035:
					case -2147287032:
					{
						return null;
					}
					case -2147287034:
					case -2147287033:
					{
						break;
					}
					default:
					{
						if (num1 == -2147286788)
						{
							return null;
						}
						break;
					}
				}
			}
			return null;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			try
			{
				if (this.iStorage != null)
				{
					while (Marshal.ReleaseComObject(this.iStorage) > 0)
					{
					}
				}
			}
			finally
			{
				this.iStorage = null;
			}
		}

		~StructuredStorage()
		{
			this.Dispose(false);
		}

		public static bool HasStorage(string fileName)
		{
			return Microsoft.Expression.Project.NativeMethods.StgIsStorageFile(fileName) == 0;
		}

		public static StructuredStorage OpenStorage(string fileName, FileAccess accessType)
		{
			Microsoft.Expression.Project.NativeMethods.STGM sTGM;
			Microsoft.Expression.Project.NativeMethods.IStorage storage;
			switch (accessType)
			{
				case FileAccess.Read:
				{
					sTGM = Microsoft.Expression.Project.NativeMethods.STGM.SHARE_DENY_WRITE;
					break;
				}
				case FileAccess.Write:
				case FileAccess.ReadWrite:
				{
					sTGM = Microsoft.Expression.Project.NativeMethods.STGM.READWRITE | Microsoft.Expression.Project.NativeMethods.STGM.SHARE_EXCLUSIVE;
					break;
				}
				default:
				{
					return null;
				}
			}
			int num = Microsoft.Expression.Project.NativeMethods.StgOpenStorage(fileName, null, sTGM, IntPtr.Zero, 0, out storage);
			int num1 = num;
			if (num1 > -2147287007)
			{
				if (num1 == -2147286788)
				{
					return null;
				}
				if (num1 == 0)
				{
					return new StructuredStorage(storage);
				}
			}
			else
			{
				switch (num1)
				{
					case -2147287036:
					case -2147287035:
					case -2147287032:
					{
						return null;
					}
					case -2147287034:
					case -2147287033:
					{
						break;
					}
					default:
					{
						switch (num1)
						{
							case -2147287008:
							case -2147287007:
							{
								return null;
							}
						}
						break;
					}
				}
			}
			return null;
		}

		public Stream OpenStreamForRead(string streamName)
		{
			Microsoft.Expression.Project.NativeMethods.IStream stream;
			int num = this.iStorage.OpenStream(streamName, IntPtr.Zero, Microsoft.Expression.Project.NativeMethods.STGM.SHARE_EXCLUSIVE, 0, out stream);
			int num1 = num;
			switch (num1)
			{
				case -2147287038:
				case -2147287036:
				case -2147287035:
				{
					return null;
				}
				case -2147287037:
				{
					return null;
				}
				default:
				{
					if (num1 == -2147286782)
					{
						return null;
					}
					if (num1 != 0)
					{
						return null;
					}
					return new ComStream(stream);
				}
			}
		}
	}
}