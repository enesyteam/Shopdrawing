using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.Reflection.Adds
{
	[SecurityPermission(SecurityAction.Demand, UnmanagedCode=true)]
	internal class MetadataFile : IDisposable
	{
		private IntPtr m_rawPointer;

		public virtual string FilePath
		{
			get
			{
				return null;
			}
		}

		public IntPtr RawPtr
		{
			get
			{
				this.EnsureNotDispose();
				return this.m_rawPointer;
			}
		}

		public MetadataFile(object importer)
		{
			if (importer == null)
			{
				throw new ArgumentNullException("importer");
			}
			this.m_rawPointer = Marshal.GetIUnknownForObject(importer);
		}

		internal MetadataFile(IntPtr rawImporter)
		{
			if (rawImporter == IntPtr.Zero)
			{
				throw new ArgumentNullException("rawImporter");
			}
			Marshal.AddRef(rawImporter);
			this.m_rawPointer = rawImporter;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this.m_rawPointer != IntPtr.Zero)
			{
				Marshal.Release(this.m_rawPointer);
			}
			this.m_rawPointer = IntPtr.Zero;
		}

		protected void EnsureNotDispose()
		{
			if (this.m_rawPointer == IntPtr.Zero)
			{
				throw new ObjectDisposedException(this.GetType().FullName);
			}
		}

		~MetadataFile()
		{
			this.Dispose(false);
		}

		public byte[] ReadEmbeddedBlob(EmbeddedBlobPointer pointer, int countBytes)
		{
			this.EnsureNotDispose();
			if (countBytes == 0)
			{
				return new byte[0];
			}
			IntPtr getDangerousLivePointer = pointer.GetDangerousLivePointer;
			this.ValidateRange(getDangerousLivePointer, countBytes);
			byte[] numArray = new byte[countBytes];
			Marshal.Copy(getDangerousLivePointer, numArray, 0, countBytes);
			return numArray;
		}

		public virtual Token ReadEntryPointToken()
		{
			throw new NotSupportedException(MetadataStringTable.RVAUnsupported);
		}

		public virtual byte[] ReadResource(long offset)
		{
			throw new NotSupportedException(MetadataStringTable.RVAUnsupported);
		}

		public virtual byte[] ReadRva(long rva, int countBytes)
		{
			throw new NotSupportedException(MetadataStringTable.RVAUnsupported);
		}

		public virtual T ReadRvaStruct<T>(long rva)
		where T : new()
		{
			T structure;
			this.EnsureNotDispose();
			int num = Marshal.SizeOf(typeof(T));
			byte[] numArray = this.ReadRva(rva, num);
			GCHandle gCHandle = GCHandle.Alloc(numArray, GCHandleType.Pinned);
			try
			{
				IntPtr intPtr = gCHandle.AddrOfPinnedObject();
				structure = (T)Marshal.PtrToStructure(intPtr, typeof(T));
			}
			finally
			{
				gCHandle.Free();
			}
			return structure;
		}

		protected virtual void ValidateRange(IntPtr ptr, int countBytes)
		{
		}
	}
}