using System;
using System.IO;

namespace Microsoft.Expression.SubsetFontTask.Zip
{
	internal class RepairedMemoryStream : MemoryStream
	{
		private long myLength;

		private bool isClosed;

		public override long Length
		{
			get
			{
				if (!this.isClosed)
				{
					return base.Length;
				}
				return this.myLength;
			}
		}

		public RepairedMemoryStream(int size) : base(size)
		{
		}

		public override void Close()
		{
			this.myLength = this.Length;
			this.isClosed = true;
			base.Close();
		}
	}
}