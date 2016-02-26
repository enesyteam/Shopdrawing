using System;
using System.IO;

namespace Microsoft.Expression.SubsetFontTask.Zip
{
	internal struct ByteBuffer
	{
		private byte[] buffer;

		private int offset;

		public int Length
		{
			get
			{
				return (int)this.buffer.Length;
			}
		}

		public ByteBuffer(int size)
		{
			this.buffer = new byte[size];
			this.offset = 0;
		}

		public int ReadContentsFrom(Stream reader)
		{
			return reader.Read(this.buffer, 0, (int)this.buffer.Length);
		}

		public ushort ReadUInt16()
		{
			byte[] numArray = this.buffer;
			ByteBuffer byteBuffer = this;
			int num = byteBuffer.offset;
			int num1 = num;
			byteBuffer.offset = num + 1;
			byte num2 = numArray[num1];
			byte[] numArray1 = this.buffer;
			ByteBuffer byteBuffer1 = this;
			int num3 = byteBuffer1.offset;
			int num4 = num3;
			byteBuffer1.offset = num3 + 1;
			return (ushort)(num2 | numArray1[num4] << 8);
		}

		public uint ReadUInt32()
		{
			byte[] numArray = this.buffer;
			ByteBuffer byteBuffer = this;
			int num = byteBuffer.offset;
			int num1 = num;
			byteBuffer.offset = num + 1;
			byte num2 = numArray[num1];
			byte[] numArray1 = this.buffer;
			ByteBuffer byteBuffer1 = this;
			int num3 = byteBuffer1.offset;
			int num4 = num3;
			byteBuffer1.offset = num3 + 1;
			byte num5 = numArray1[num4];
			byte[] numArray2 = this.buffer;
			ByteBuffer byteBuffer2 = this;
			int num6 = byteBuffer2.offset;
			int num7 = num6;
			byteBuffer2.offset = num6 + 1;
			byte num8 = numArray2[num7];
			byte[] numArray3 = this.buffer;
			ByteBuffer byteBuffer3 = this;
			int num9 = byteBuffer3.offset;
			int num10 = num9;
			byteBuffer3.offset = num9 + 1;
			return (uint)(num2 | (num5 | (num8 | numArray3[num10] << 8) << 8) << 8);
		}

		public void SkipBytes(int count)
		{
			ByteBuffer byteBuffer = this;
			byteBuffer.offset = byteBuffer.offset + count;
		}

		public void WriteContentsTo(Stream writer)
		{
			writer.Write(this.buffer, 0, (int)this.buffer.Length);
		}

		public void WriteUInt16(ushort value)
		{
			byte[] numArray = this.buffer;
			ByteBuffer byteBuffer = this;
			int num = byteBuffer.offset;
			int num1 = num;
			byteBuffer.offset = num + 1;
			numArray[num1] = (byte)value;
			byte[] numArray1 = this.buffer;
			ByteBuffer byteBuffer1 = this;
			int num2 = byteBuffer1.offset;
			int num3 = num2;
			byteBuffer1.offset = num2 + 1;
			numArray1[num3] = (byte)(value >> 8);
		}

		public void WriteUInt32(uint value)
		{
			byte[] numArray = this.buffer;
			ByteBuffer byteBuffer = this;
			int num = byteBuffer.offset;
			int num1 = num;
			byteBuffer.offset = num + 1;
			numArray[num1] = (byte)value;
			byte[] numArray1 = this.buffer;
			ByteBuffer byteBuffer1 = this;
			int num2 = byteBuffer1.offset;
			int num3 = num2;
			byteBuffer1.offset = num2 + 1;
			numArray1[num3] = (byte)(value >> 8);
			byte[] numArray2 = this.buffer;
			ByteBuffer byteBuffer2 = this;
			int num4 = byteBuffer2.offset;
			int num5 = num4;
			byteBuffer2.offset = num4 + 1;
			numArray2[num5] = (byte)(value >> 16);
			byte[] numArray3 = this.buffer;
			ByteBuffer byteBuffer3 = this;
			int num6 = byteBuffer3.offset;
			int num7 = num6;
			byteBuffer3.offset = num6 + 1;
			numArray3[num7] = (byte)(value >> 24);
		}
	}
}