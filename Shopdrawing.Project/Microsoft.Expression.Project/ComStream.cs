using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Microsoft.Expression.Project
{
	internal class ComStream : Stream
	{
		private Microsoft.Expression.Project.NativeMethods.IStream iStream;

		public override bool CanRead
		{
			get
			{
				return this.iStream != null;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return this.iStream != null;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return this.iStream != null;
			}
		}

		public override long Length
		{
			get
			{
				return this.Status.cbSize;
			}
		}

		public override long Position
		{
			get
			{
				ulong num;
				this.iStream.Seek((long)0, Microsoft.Expression.Project.NativeMethods.STREAM_SEEK.STREAM_SEEK_CUR, out num);
				return (long)num;
			}
			set
			{
				ulong num;
				if (value < (long)0)
				{
					throw new ArgumentOutOfRangeException("value", "Position cannot be negative.");
				}
				this.iStream.Seek(value, Microsoft.Expression.Project.NativeMethods.STREAM_SEEK.STREAM_SEEK_SET, out num);
			}
		}

		private System.Runtime.InteropServices.ComTypes.STATSTG Status
		{
			get
			{
				System.Runtime.InteropServices.ComTypes.STATSTG sTATSTG;
				this.iStream.Stat(out sTATSTG, Microsoft.Expression.Project.NativeMethods.STATFLAG.STATFLAG_NONAME);
				return sTATSTG;
			}
		}

		public ComStream(Microsoft.Expression.Project.NativeMethods.IStream iStream)
		{
			if (iStream == null)
			{
				throw new ArgumentNullException("iStream");
			}
			this.iStream = iStream;
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing && this.iStream != null)
				{
					while (Marshal.ReleaseComObject(this.iStream) > 0)
					{
					}
				}
			}
			finally
			{
				this.iStream = null;
			}
			base.Dispose(disposing);
		}

		public override void Flush()
		{
			this.iStream.Commit(Microsoft.Expression.Project.NativeMethods.STGC.STGC_DEFAULT);
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			uint num;
			if (count <= 0)
			{
				throw new ArgumentOutOfRangeException("count", "Count must be greater than zero");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "Offset must not be negative.");
			}
			if ((long)(count + offset) > buffer.LongLength)
			{
				throw new ArgumentOutOfRangeException("count", "Count plus offset exceeds buffer size.");
			}
			if (offset <= 0)
			{
				this.iStream.Read(buffer, (uint)count, out num);
			}
			else
			{
				byte[] numArray = new byte[count];
				this.iStream.Read(numArray, (uint)count, out num);
				numArray.CopyTo(buffer, offset);
			}
			return (int)num;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			ulong num;
			Microsoft.Expression.Project.NativeMethods.STREAM_SEEK sTREAMSEEK = Microsoft.Expression.Project.NativeMethods.STREAM_SEEK.STREAM_SEEK_SET;
			switch (origin)
			{
				case SeekOrigin.Begin:
				{
					if (offset < (long)0)
					{
						throw new ArgumentOutOfRangeException("offset", "Value cannot be negative when the seek origin is Begin.");
					}
					sTREAMSEEK = Microsoft.Expression.Project.NativeMethods.STREAM_SEEK.STREAM_SEEK_SET;
					break;
				}
				case SeekOrigin.Current:
				{
					sTREAMSEEK = Microsoft.Expression.Project.NativeMethods.STREAM_SEEK.STREAM_SEEK_CUR;
					break;
				}
				case SeekOrigin.End:
				{
					if (offset > (long)0)
					{
						throw new ArgumentOutOfRangeException("offset", "Value cannot be positive when the seek orgin is End.");
					}
					sTREAMSEEK = Microsoft.Expression.Project.NativeMethods.STREAM_SEEK.STREAM_SEEK_END;
					break;
				}
			}
			this.iStream.Seek(offset, sTREAMSEEK, out num);
			return (long)num;
		}

		public override void SetLength(long value)
		{
			throw new NotImplementedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			uint num;
			if (count <= 0)
			{
				throw new ArgumentOutOfRangeException("count", "Count must be greater than zero");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "Offset must not be negative.");
			}
			if ((long)(count + offset) > buffer.LongLength)
			{
				throw new ArgumentOutOfRangeException("count", "Count plus offset exceeds buffer size.");
			}
			if (offset <= 0)
			{
				this.iStream.Write(buffer, (uint)count, out num);
				return;
			}
			byte[] numArray = new byte[count];
			buffer.CopyTo(numArray, offset);
			this.iStream.Write(numArray, (uint)count, out num);
		}
	}
}