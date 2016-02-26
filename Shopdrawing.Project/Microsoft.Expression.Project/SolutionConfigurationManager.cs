using Microsoft.Expression.Framework.Configuration;
using Microsoft.Expression.Framework.Documents;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Xml;

namespace Microsoft.Expression.Project
{
	internal sealed class SolutionConfigurationManager : ConfigurationServiceBase
	{
		private const string BlendSettingsSection = "BlendSettings";

		private VisualStudioSolution solution;

		private string SuoPath
		{
			get
			{
				return Path.ChangeExtension(this.solution.DocumentReference.Path, ".suo");
			}
		}

		public SolutionConfigurationManager(VisualStudioSolution solution)
		{
			this.solution = solution;
		}

		private bool AttemptSave()
		{
			bool flag;
			StructuredStorage structuredStorage = this.OpenOrCreateStorage(FileAccess.ReadWrite);
			if (structuredStorage != null)
			{
				using (structuredStorage)
				{
					Stream stream = structuredStorage.CreateStream("BlendSettings");
					if (stream == null)
					{
						flag = false;
					}
					else
					{
						using (stream)
						{
							XmlWriterSettings xmlWriterSetting = new XmlWriterSettings()
							{
								Indent = true,
								Encoding = Encoding.Unicode
							};
							XmlWriter xmlWriter = null;
							try
							{
								xmlWriter = XmlWriter.Create(stream, xmlWriterSetting);
							}
							catch (COMException cOMException)
							{
								flag = false;
								return flag;
							}
							base.SaveInternal(xmlWriter);
						}
						goto Label0;
					}
				}
				return flag;
			}
			return true;
		Label0:
			string suoPath = this.SuoPath;
			if (Microsoft.Expression.Framework.Documents.PathHelper.FileExists(suoPath))
			{
				FileAttributes attributes = File.GetAttributes(suoPath);
				FileAttributes fileAttribute = attributes;
				if ((attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
				{
					fileAttribute = fileAttribute | FileAttributes.Hidden;
				}
				if ((attributes & FileAttributes.ReadOnly) != FileAttributes.ReadOnly)
				{
					fileAttribute = fileAttribute | FileAttributes.ReadOnly;
				}
				if (fileAttribute != attributes)
				{
					try
					{
						File.SetAttributes(suoPath, attributes | FileAttributes.Hidden);
						return true;
					}
					catch (UnauthorizedAccessException unauthorizedAccessException)
					{
						return true;
					}
					catch (IOException oException)
					{
						return true;
					}
				}
				else
				{
					return true;
				}
			}
			else
			{
				return true;
			}
		}

		public override void Load()
		{
			StructuredStorage structuredStorage = this.OpenOrCreateStorage(FileAccess.Read);
			if (structuredStorage != null)
			{
				using (structuredStorage)
				{
					Stream stream = structuredStorage.OpenStreamForRead("BlendSettings");
					if (stream != null)
					{
						using (stream)
						{
							XmlReader xmlReader = null;
							try
							{
								xmlReader = XmlReader.Create(stream);
							}
							catch (SecurityException securityException)
							{
								return;
							}
							catch (COMException cOMException)
							{
								return;
							}
							base.LoadInternal(xmlReader);
						}
					}
				}
			}
		}

		private StructuredStorage OpenOrCreateStorage(FileAccess fileAccess)
		{
			StructuredStorage structuredStorage;
			string suoPath = this.SuoPath;
			StructuredStorage structuredStorage1 = null;
			if (Microsoft.Expression.Framework.Documents.PathHelper.FileExists(suoPath))
			{
				if (fileAccess == FileAccess.ReadWrite || fileAccess == FileAccess.Write)
				{
					FileAttributes attributes = File.GetAttributes(suoPath);
					if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
					{
						try
						{
							File.SetAttributes(suoPath, attributes & (FileAttributes.Hidden | FileAttributes.System | FileAttributes.Directory | FileAttributes.Archive | FileAttributes.Device | FileAttributes.Normal | FileAttributes.Temporary | FileAttributes.SparseFile | FileAttributes.ReparsePoint | FileAttributes.Compressed | FileAttributes.Offline | FileAttributes.NotContentIndexed | FileAttributes.Encrypted | FileAttributes.IntegrityStream | FileAttributes.NoScrubData));
							goto Label0;
						}
						catch (UnauthorizedAccessException unauthorizedAccessException)
						{
							structuredStorage = null;
						}
						catch (IOException oException)
						{
							structuredStorage = null;
						}
						return structuredStorage;
					}
				}
			Label0:
				if (StructuredStorage.HasStorage(suoPath))
				{
					structuredStorage1 = StructuredStorage.OpenStorage(suoPath, fileAccess);
				}
			}
			if (structuredStorage1 == null && fileAccess != FileAccess.Read)
			{
				structuredStorage1 = StructuredStorage.CreateStorage(suoPath);
			}
			return structuredStorage1;
		}

		public override void Save()
		{
			if (!this.AttemptSave())
			{
				if (Microsoft.Expression.Framework.Documents.PathHelper.FileExists(this.SuoPath))
				{
					try
					{
						File.Delete(this.SuoPath);
					}
					catch (IOException oException)
					{
					}
					catch (UnauthorizedAccessException unauthorizedAccessException)
					{
					}
				}
				this.AttemptSave();
			}
		}
	}
}