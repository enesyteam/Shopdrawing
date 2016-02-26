using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Microsoft.Expression.SubsetFontTask.Zip
{
	internal sealed class ZipArchive
	{
		internal SortedDictionary<string, ZipArchiveFile> entries;

		internal Stream fromStream;

		private FileAccess access;

		private string archivePath;

		private bool closeCalled;

		public int Count
		{
			get
			{
				return this.entries.Count;
			}
		}

		public IEnumerable<ZipArchiveFile> Files
		{
			get
			{
				return this.entries.Values;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return this.access == FileAccess.Read;
			}
			set
			{
				if (this.fromStream == null)
				{
					this.access = (value ? FileAccess.Read : FileAccess.ReadWrite);
					return;
				}
				if (value)
				{
					if (!this.fromStream.CanRead)
					{
						throw new Exception("Can't read from stream");
					}
					this.access = FileAccess.Read;
					return;
				}
				if (!this.fromStream.CanWrite)
				{
					throw new ArgumentException("Can't reset IsReadOnly on a ZipArchive whose stream is ReadOnly.");
				}
				this.access = (this.fromStream.CanRead ? FileAccess.ReadWrite : FileAccess.Write);
			}
		}

		public ZipArchiveFile this[string archivePath]
		{
			get
			{
				ZipArchiveFile zipArchiveFile = null;
				this.entries.TryGetValue(archivePath, out zipArchiveFile);
				return zipArchiveFile;
			}
		}

		public ZipArchive(string archivePath) : this(archivePath, FileAccess.Read)
		{
		}

		public ZipArchive(string archivePath, FileAccess access)
		{
			this.entries = new SortedDictionary<string, ZipArchiveFile>(StringComparer.OrdinalIgnoreCase);
			this.archivePath = archivePath;
			this.access = access;
			if (access == FileAccess.Read)
			{
				this.fromStream = new FileStream(archivePath, FileMode.Open, access);
			}
			else if (access == FileAccess.ReadWrite)
			{
				this.fromStream = new FileStream(archivePath, FileMode.OpenOrCreate, access);
			}
			if (this.fromStream != null)
			{
				this.Read(this.fromStream);
			}
		}

		public ZipArchive(Stream fromStream, FileAccess desiredAccess)
		{
			this.entries = new SortedDictionary<string, ZipArchiveFile>(StringComparer.OrdinalIgnoreCase);
			this.access = desiredAccess;
			this.fromStream = fromStream;
			if ((int)(desiredAccess & FileAccess.Read) != 0)
			{
				if (!fromStream.CanRead)
				{
					throw new Exception("Error: Can't read from stream.");
				}
				this.Read(fromStream);
				return;
			}
			if ((int)(desiredAccess & FileAccess.Write) != 0 && !fromStream.CanWrite)
			{
				throw new Exception("Error: Can't write to stream.");
			}
		}

		public void Clear()
		{
			this.entries.Clear();
		}

		public void Close()
		{
			this.closeCalled = true;
			if (!this.IsReadOnly)
			{
				if (this.fromStream == null)
				{
					this.fromStream = new FileStream(this.archivePath, FileMode.Create);
				}
				this.fromStream.Position = (long)0;
				this.fromStream.SetLength((long)0);
				foreach (ZipArchiveFile value in this.entries.Values)
				{
					value.WriteToStream(this.fromStream);
				}
				this.WriteArchiveDirectoryToStream(this.fromStream);
			}
			this.fromStream.Close();
		}

		public void CopyFromDirectory(string sourceDirectory, string targetArchiveDirectory)
		{
			string[] files = Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories);
			for (int i = 0; i < (int)files.Length; i++)
			{
				string str = files[i];
				string relativePath = ZipArchive.GetRelativePath(str, sourceDirectory);
				this.CopyFromFile(str, Path.Combine(targetArchiveDirectory, relativePath));
			}
		}

		public void CopyFromFile(string sourceFilePath, string targetArchivePath)
		{
			using (Stream fileStream = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Write | FileShare.ReadWrite | FileShare.Delete))
			{
				using (Stream stream = this.Create(targetArchivePath))
				{
					ZipArchiveFile.CopyStream(fileStream, stream);
				}
			}
			this[targetArchivePath].LastWriteTime = File.GetLastWriteTime(sourceFilePath);
		}

		public void CopyToDirectory(string sourceArchiveDirectory, string targetDirectory)
		{
			foreach (ZipArchiveFile filesInDirectory in this.GetFilesInDirectory(sourceArchiveDirectory, SearchOption.AllDirectories))
			{
				string relativePath = ZipArchive.GetRelativePath(filesInDirectory.Name, sourceArchiveDirectory);
				filesInDirectory.CopyToFile(Path.Combine(targetDirectory, relativePath));
			}
		}

		public void CopyToFile(string sourceArchivePath, string targetFilePath)
		{
			this.entries[sourceArchivePath].CopyToFile(targetFilePath);
		}

		public Stream Create(string archivePath)
		{
			ZipArchiveFile zipArchiveFile;
			if (!this.entries.TryGetValue(archivePath, out zipArchiveFile))
			{
				zipArchiveFile = new ZipArchiveFile(this, archivePath);
			}
			return zipArchiveFile.Create();
		}

		public TextWriter CreateText(string archivePath)
		{
			ZipArchiveFile zipArchiveFile;
			if (!this.entries.TryGetValue(archivePath, out zipArchiveFile))
			{
				zipArchiveFile = new ZipArchiveFile(this, archivePath);
			}
			return zipArchiveFile.CreateText();
		}

		public bool Delete(string archivePath)
		{
			ZipArchiveFile zipArchiveFile;
			if (!this.entries.TryGetValue(archivePath, out zipArchiveFile))
			{
				return false;
			}
			zipArchiveFile.Delete();
			return true;
		}

		public int DeleteDirectory(string archivePath)
		{
			int num = 0;
			foreach (ZipArchiveFile zipArchiveFile in new List<ZipArchiveFile>(this.GetFilesInDirectory(archivePath, SearchOption.AllDirectories)))
			{
				zipArchiveFile.Delete();
				num++;
			}
			return num;
		}

		public bool Exists(string archivePath)
		{
			return this.entries.ContainsKey(archivePath);
		}

		~ZipArchive()
		{
		}

		public IEnumerable<ZipArchiveFile> GetFilesInDirectory(string archivePath, SearchOption searchOptions)
		{
			foreach (ZipArchiveFile value in this.entries.Values)
			{
				string str = value.Name;
				if (!str.StartsWith(archivePath, StringComparison.OrdinalIgnoreCase) || str.Length <= archivePath.Length || searchOptions == SearchOption.TopDirectoryOnly && str.IndexOf(Path.DirectorySeparatorChar, archivePath.Length + 1) >= 0)
				{
					continue;
				}
				yield return value;
			}
		}

		internal static string GetRelativePath(string fileName, string directory)
		{
			int length = directory.Length;
			if (length == 0)
			{
				return fileName;
			}
			while (length < fileName.Length && fileName[length] == Path.DirectorySeparatorChar)
			{
				length++;
			}
			return fileName.Substring(length);
		}

		public void Move(string sourceArchivePath, string destinationArchivePath)
		{
			this.entries[sourceArchivePath].MoveTo(destinationArchivePath);
		}

		public Stream OpenRead(string archivePath)
		{
			return this.entries[archivePath].OpenRead();
		}

		public StreamReader OpenText(string archivePath)
		{
			return this.entries[archivePath].OpenText();
		}

		private void Read(Stream archiveStream)
		{
			while (ZipArchiveFile.Read(this) != null)
			{
			}
		}

		public string ReadAllText(string archivePath)
		{
			return this.entries[archivePath].ReadAllText();
		}

		public override string ToString()
		{
			string str = this.archivePath;
			if (this.archivePath == null)
			{
				str = "<fromStream>";
			}
			object[] count = new object[] { "ZipArchive ", str, " FileCount = ", this.entries.Count };
			return string.Concat(count);
		}

		public void WriteAllText(string archivePath, string data)
		{
			ZipArchiveFile zipArchiveFile;
			if (!this.entries.TryGetValue(archivePath, out zipArchiveFile))
			{
				zipArchiveFile = new ZipArchiveFile(this, archivePath);
			}
			zipArchiveFile.WriteAllText(data);
		}

		private void WriteArchiveDirectoryToStream(Stream writer)
		{
			long position = writer.Position;
			foreach (ZipArchiveFile value in this.entries.Values)
			{
				value.WriteArchiveDirectoryEntryToStream(writer);
			}
			long num = writer.Position;
			ByteBuffer byteBuffer = new ByteBuffer(22);
			byteBuffer.WriteUInt32(101010256);
			byteBuffer.WriteUInt16(0);
			byteBuffer.WriteUInt16(0);
			byteBuffer.WriteUInt16((ushort)this.entries.Count);
			byteBuffer.WriteUInt16((ushort)this.entries.Count);
			byteBuffer.WriteUInt32((uint)(num - position));
			byteBuffer.WriteUInt32((uint)position);
			byteBuffer.WriteUInt16(0);
			byteBuffer.WriteContentsTo(writer);
		}
	}
}