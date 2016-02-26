using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project
{
	public sealed class ProjectFileInformation
	{
		public DateTime CreationTime
		{
			get;
			private set;
		}

		private FileInfo DocumentFileInfo
		{
			get
			{
				FileInfo fileInfo = null;
				try
				{
					fileInfo = new FileInfo(this.DocumentPath);
				}
				catch (NotSupportedException notSupportedException)
				{
				}
				catch (ArgumentException argumentException)
				{
				}
				return fileInfo;
			}
		}

		public string DocumentPath
		{
			get;
			private set;
		}

		public bool Exists
		{
			get;
			private set;
		}

		public System.IO.FileAttributes FileAttributes
		{
			get;
			private set;
		}

		public DateTime LastWriteTime
		{
			get;
			private set;
		}

		public long Length
		{
			get;
			private set;
		}

		public ProjectFileInformation(string documentPath)
		{
			this.DocumentPath = documentPath;
			FileInfo documentFileInfo = null;
			if (documentPath != null)
			{
				documentFileInfo = this.DocumentFileInfo;
			}
			if (documentFileInfo == null || !documentFileInfo.Exists)
			{
				this.CreationTime = new DateTime((long)0);
				this.LastWriteTime = new DateTime((long)0);
				return;
			}
			this.Exists = true;
			this.FileAttributes = documentFileInfo.Attributes;
			this.LastWriteTime = documentFileInfo.LastWriteTime;
			this.CreationTime = documentFileInfo.CreationTime;
			this.Length = documentFileInfo.Length;
		}

		public bool HasChanged()
		{
			bool flag;
			if (string.IsNullOrEmpty(this.DocumentPath))
			{
				return false;
			}
			FileInfo documentFileInfo = this.DocumentFileInfo;
			if (documentFileInfo == null)
			{
				return false;
			}
			if (this.Exists != documentFileInfo.Exists)
			{
				flag = true;
			}
			else if (!documentFileInfo.Exists)
			{
				flag = false;
			}
			else
			{
				flag = (this.LastWriteTime != documentFileInfo.LastWriteTime || this.CreationTime != documentFileInfo.CreationTime ? true : this.Length != documentFileInfo.Length);
			}
			bool flag1 = flag;
			bool flag2 = (!documentFileInfo.Exists ? false : documentFileInfo.Attributes != this.FileAttributes);
			if (!flag1 && !flag2)
			{
				return false;
			}
			if (flag2)
			{
				this.FileAttributes = documentFileInfo.Attributes;
			}
			return flag1;
		}

		public bool HasComeIntoExistence()
		{
			if (string.IsNullOrEmpty(this.DocumentPath))
			{
				return false;
			}
			FileInfo documentFileInfo = this.DocumentFileInfo;
			if (documentFileInfo == null)
			{
				return false;
			}
			if (this.Exists)
			{
				return false;
			}
			return this.Exists != documentFileInfo.Exists;
		}
	}
}