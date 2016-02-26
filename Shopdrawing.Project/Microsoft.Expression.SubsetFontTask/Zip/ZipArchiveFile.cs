using Microsoft.Expression.SubsetFontTask;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Microsoft.Expression.SubsetFontTask.Zip
{
	public sealed class ZipArchiveFile
	{
		internal const uint SignatureFileEntry = 67324752;

		internal const uint SignatureArchiveDirectory = 33639248;

		internal const uint SignatureArchiveDirectoryEnd = 101010256;

		internal const ushort VersionNeededToExtract = 256;

		internal const ushort MaximumVersionExtractable = 256;

		internal const ushort VersionMadeBy = 0;

		internal const ushort GeneralPurposeBitFlag = 0;

		internal const ushort ExtraFieldLength = 0;

		internal const ushort FileCommentLength = 0;

		internal const ushort DiskNumberStart = 0;

		internal const ushort InternalFileAttributes = 0;

		internal const uint ExternalFileAttributes = 0;

		private uint? crc32;

		private string name;

		private DateTime lastWriteTime;

		private long length;

		private int compressedLength;

		private ZipArchiveFile.CompressionMethod compressionMethod;

		private ZipArchive archive;

		private uint headerOffset;

		private MemoryStream uncompressedData;

		private byte[] compressedData;

		private long positionOfCompressedDataInArchive;

		private static char[] invalidPathChars;

		internal ZipArchive Archive
		{
			get
			{
				return this.archive;
			}
		}

		public uint CheckSum
		{
			get
			{
				if (!this.crc32.HasValue)
				{
					this.crc32 = new uint?(Crc32.Calculate(0, this.uncompressedData.GetBuffer(), 0, (int)this.uncompressedData.Length));
				}
				return this.crc32.Value;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return this.archive.IsReadOnly;
			}
		}

		public DateTime LastWriteTime
		{
			get
			{
				return this.lastWriteTime;
			}
			set
			{
				if (this.IsReadOnly)
				{
					throw new ApplicationException("Archive is ReadOnly");
				}
				this.lastWriteTime = value;
			}
		}

		public long Length
		{
			get
			{
				if (this.uncompressedData == null)
				{
					return this.length;
				}
				return this.uncompressedData.Length;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.MoveTo(value);
			}
		}

		static ZipArchiveFile()
		{
			ZipArchiveFile.invalidPathChars = Path.GetInvalidPathChars();
		}

		internal ZipArchiveFile(ZipArchive archive, string archiveName)
		{
			this.archive = archive;
			this.name = archiveName;
			if (this.name != null)
			{
				archive.entries[this.name] = this;
			}
			this.lastWriteTime = DateTime.Now;
		}

		internal static int CopyStream(Stream fromStream, Stream toStream)
		{
			byte[] numArray = new byte[8192];
			int num = 0;
			while (true)
			{
				int num1 = fromStream.Read(numArray, 0, (int)numArray.Length);
				if (num1 == 0)
				{
					break;
				}
				toStream.Write(numArray, 0, num1);
				num = num + num1;
			}
			return num;
		}

		public void CopyTo(string outputArchivePath)
		{
			using (Stream stream = this.archive.Create(outputArchivePath))
			{
				using (Stream stream1 = this.OpenRead())
				{
					ZipArchiveFile.CopyStream(stream1, stream);
				}
			}
			this.archive[outputArchivePath].LastWriteTime = this.LastWriteTime;
		}

		public void CopyToFile(string outputFilePath)
		{
			string directoryName = Path.GetDirectoryName(outputFilePath);
			if (directoryName.Length > 0)
			{
				Directory.CreateDirectory(directoryName);
			}
			using (Stream fileStream = new FileStream(outputFilePath, FileMode.Create))
			{
				using (Stream stream = this.OpenRead())
				{
					ZipArchiveFile.CopyStream(stream, fileStream);
				}
			}
			File.SetLastWriteTime(outputFilePath, this.LastWriteTime);
		}

		public Stream Create()
		{
			if (this.IsReadOnly)
			{
				throw new ApplicationException("Archive is ReadOnly");
			}
			if (this.uncompressedData != null && (this.uncompressedData.CanWrite || this.uncompressedData.CanRead))
			{
				throw new ApplicationException("ZipArchiveFile already open.");
			}
			this.compressedData = null;
			this.positionOfCompressedDataInArchive = (long)0;
			this.compressedLength = 0;
			this.uncompressedData = new RepairedMemoryStream(256);
			return this.uncompressedData;
		}

		public StreamWriter CreateText()
		{
			return new StreamWriter(this.Create());
		}

		private static uint DateTimeToDosTime(DateTime dateTime)
		{
			int year = dateTime.Year - 1980 & 127;
			year = (year << 4) + dateTime.Month;
			year = (year << 5) + dateTime.Day;
			year = (year << 5) + dateTime.Hour;
			year = (year << 6) + dateTime.Minute;
			year = (year << 5) + dateTime.Second / 2;
			return (uint)year;
		}

		public void Delete()
		{
			if (this.IsReadOnly)
			{
				throw new ApplicationException("Archive is ReadOnly");
			}
			this.archive.entries.Remove(this.name);
			this.name = null;
			this.archive = null;
			this.uncompressedData = null;
			this.compressedData = null;
		}

		private static DateTime DosTimeToDateTime(uint dateTime)
		{
			int num = (int)dateTime;
			int num1 = 1980 + (num >> 25);
			int num2 = num >> 21 & 15;
			int num3 = num >> 16 & 31;
			int num4 = num >> 11 & 31;
			int num5 = num >> 5 & 63;
			int num6 = (num & 31) * 2;
			if (num6 >= 60)
			{
				num6 = 0;
			}
			DateTime dateTime1 = new DateTime();
			try
			{
				dateTime1 = new DateTime(num1, num2, num3, num4, num5, num6, 0);
			}
			catch
			{
			}
			return dateTime1;
		}

		public void MoveTo(string newArchivePath)
		{
			if (this.IsReadOnly)
			{
				throw new ApplicationException("Archive is ReadOnly");
			}
			this.archive.entries.Remove(this.name);
			this.name = newArchivePath;
			this.archive.entries[newArchivePath] = this;
		}

		public Stream OpenRead()
		{
			if (this.uncompressedData != null)
			{
				if (this.uncompressedData.CanWrite)
				{
					throw new ApplicationException("ZipArchiveFile still open for writing.");
				}
				return new MemoryStream(this.uncompressedData.GetBuffer(), 0, (int)this.uncompressedData.Length, false);
			}
			if (this.compressedData == null)
			{
				this.compressedData = new byte[this.compressedLength];
				this.archive.fromStream.Seek(this.positionOfCompressedDataInArchive, SeekOrigin.Begin);
				this.archive.fromStream.Read(this.compressedData, 0, this.compressedLength);
			}
			MemoryStream memoryStream = new MemoryStream(this.compressedData);
			if (this.compressionMethod == ZipArchiveFile.CompressionMethod.None)
			{
				return memoryStream;
			}
			return new DeflateStream(memoryStream, CompressionMode.Decompress);
		}

		public StreamReader OpenText()
		{
			return new StreamReader(this.OpenRead());
		}

		internal static ZipArchiveFile Read(ZipArchive archive)
		{
			Stream stream = archive.fromStream;
			ByteBuffer byteBuffer = new ByteBuffer(30);
			int num = byteBuffer.ReadContentsFrom(stream);
			if (num == 0)
			{
				return null;
			}
			uint num1 = byteBuffer.ReadUInt32();
			if (num1 != 67324752)
			{
				if (num1 != 33639248)
				{
					throw new ApplicationException("Bad ZipFile Header");
				}
				return null;
			}
			if (byteBuffer.ReadUInt16() > 256)
			{
				throw new ApplicationException("Zip file requires unsupported features");
			}
			byteBuffer.SkipBytes(2);
			ZipArchiveFile zipArchiveFile = new ZipArchiveFile(archive, null)
			{
				compressionMethod = (ZipArchiveFile.CompressionMethod)byteBuffer.ReadUInt16(),
				lastWriteTime = ZipArchiveFile.DosTimeToDateTime(byteBuffer.ReadUInt32()),
				crc32 = new uint?(byteBuffer.ReadUInt32()),
				compressedLength = (void*)(checked((int)byteBuffer.ReadUInt32())),
				length = (long)byteBuffer.ReadUInt32()
			};
			int num2 = byteBuffer.ReadUInt16();
			byte[] numArray = new byte[num2];
			int num3 = stream.Read(numArray, 0, num2);
			zipArchiveFile.name = Encoding.UTF8.GetString(numArray).Replace('/', Path.DirectorySeparatorChar);
			archive.entries[zipArchiveFile.name] = zipArchiveFile;
			if (num != byteBuffer.Length || num3 != num2 || num2 == 0 || zipArchiveFile.LastWriteTime.Ticks == (long)0)
			{
				throw new ApplicationException("Bad Zip File Header");
			}
			if (zipArchiveFile.Name.IndexOfAny(ZipArchiveFile.invalidPathChars) >= 0)
			{
				throw new ApplicationException("Invalid File Name");
			}
			if (zipArchiveFile.compressionMethod != ZipArchiveFile.CompressionMethod.None && zipArchiveFile.compressionMethod != ZipArchiveFile.CompressionMethod.Deflate)
			{
				throw new ApplicationException(string.Concat("Unsupported compression mode ", zipArchiveFile.compressionMethod));
			}
			if (!archive.IsReadOnly || !stream.CanSeek)
			{
				zipArchiveFile.compressedData = new byte[zipArchiveFile.compressedLength];
				stream.Read(zipArchiveFile.compressedData, 0, zipArchiveFile.compressedLength);
			}
			else
			{
				zipArchiveFile.positionOfCompressedDataInArchive = archive.fromStream.Position;
				stream.Seek((long)zipArchiveFile.compressedLength, SeekOrigin.Current);
			}
			return zipArchiveFile;
		}

		public string ReadAllText()
		{
			TextReader textReader = this.OpenText();
			string end = textReader.ReadToEnd();
			textReader.Close();
			return end;
		}

		public override string ToString()
		{
			object[] name = new object[] { "ZipArchiveEntry ", this.Name, " length ", this.Length };
			return string.Concat(name);
		}

		internal void Validate()
		{
			Stream stream = this.OpenRead();
			uint num = 0;
			byte[] numArray = new byte[655536];
			while (true)
			{
				int num1 = stream.Read(numArray, 0, (int)numArray.Length);
				if (num1 == 0)
				{
					break;
				}
				num = Crc32.Calculate(num, numArray, 0, num1);
			}
			stream.Close();
			if (num != this.CheckSum)
			{
				throw new ApplicationException(string.Concat("Error: data checksum failed for ", this.Name));
			}
		}

		public void WriteAllText(string data)
		{
			TextWriter textWriter = this.CreateText();
			textWriter.Write(data);
			textWriter.Close();
		}

		internal void WriteArchiveDirectoryEntryToStream(Stream writer)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(this.name);
			ByteBuffer byteBuffer = new ByteBuffer(46);
			byteBuffer.WriteUInt32(33639248);
			byteBuffer.WriteUInt16(0);
			byteBuffer.WriteUInt16(256);
			byteBuffer.WriteUInt16(0);
			byteBuffer.WriteUInt16((ushort)this.compressionMethod);
			byteBuffer.WriteUInt32(ZipArchiveFile.DateTimeToDosTime(this.lastWriteTime));
			byteBuffer.WriteUInt32(this.CheckSum);
			byteBuffer.WriteUInt32((uint)this.compressedLength);
			byteBuffer.WriteUInt32((uint)this.Length);
			byteBuffer.WriteUInt16((ushort)((int)bytes.Length));
			byteBuffer.WriteUInt16(0);
			byteBuffer.WriteUInt16(0);
			byteBuffer.WriteUInt16(0);
			byteBuffer.WriteUInt16(0);
			byteBuffer.WriteUInt32(0);
			byteBuffer.WriteUInt32(this.headerOffset);
			byteBuffer.WriteContentsTo(writer);
			writer.Write(bytes, 0, (int)bytes.Length);
		}

		internal void WriteToStream(Stream writer)
		{
			if (this.uncompressedData != null)
			{
				if (this.uncompressedData.CanWrite)
				{
					throw new Exception(string.Concat("Unclosed writable handle to ", this.Name, " still exists at Save time"));
				}
				MemoryStream repairedMemoryStream = new RepairedMemoryStream((int)(this.uncompressedData.Length * (long)5 / (long)8));
				Stream deflateStream = new DeflateStream(repairedMemoryStream, CompressionMode.Compress);
				deflateStream.Write(this.uncompressedData.GetBuffer(), 0, (int)this.uncompressedData.Length);
				deflateStream.Close();
				this.compressionMethod = ZipArchiveFile.CompressionMethod.Deflate;
				this.compressedLength = (int)repairedMemoryStream.Length;
				this.compressedData = repairedMemoryStream.GetBuffer();
			}
			this.WriteZipFileHeader(writer);
			writer.Write(this.compressedData, 0, this.compressedLength);
		}

		private void WriteZipFileHeader(Stream writer)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(this.name.Replace(Path.DirectorySeparatorChar, '/'));
			if ((ulong)((uint)this.length) != this.length)
			{
				throw new ApplicationException("File length too long.");
			}
			this.headerOffset = (uint)writer.Position;
			ByteBuffer byteBuffer = new ByteBuffer(30);
			byteBuffer.WriteUInt32(67324752);
			byteBuffer.WriteUInt16(256);
			byteBuffer.WriteUInt16(0);
			byteBuffer.WriteUInt16((ushort)this.compressionMethod);
			byteBuffer.WriteUInt32(ZipArchiveFile.DateTimeToDosTime(this.lastWriteTime));
			byteBuffer.WriteUInt32(this.CheckSum);
			byteBuffer.WriteUInt32((uint)this.compressedLength);
			byteBuffer.WriteUInt32((uint)this.Length);
			byteBuffer.WriteUInt16((ushort)((int)bytes.Length));
			byteBuffer.WriteUInt16(0);
			byteBuffer.WriteContentsTo(writer);
			writer.Write(bytes, 0, (int)bytes.Length);
		}

		internal enum CompressionMethod : ushort
		{
			None = 0,
			Deflate = 8
		}
	}
}