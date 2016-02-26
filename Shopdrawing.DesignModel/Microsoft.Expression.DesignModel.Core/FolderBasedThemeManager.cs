using Microsoft.Expression.DesignModel.Markup;
using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;

namespace Microsoft.Expression.DesignModel.Core
{
	public abstract class FolderBasedThemeManager : ThemeManager
	{
		public virtual string ThemeFolder
		{
			get
			{
				return this.GetThemeFolder(base.Platform.Metadata.TargetFramework);
			}
		}

		public virtual IList<IDocumentLocator> Themes
		{
			get
			{
				string str;
				List<IDocumentLocator> documentLocators = new List<IDocumentLocator>();
				if (Directory.Exists(this.ThemeFolder))
				{
					string[] files = Directory.GetFiles(this.ThemeFolder);
					for (int i = 0; i < (int)files.Length; i++)
					{
						string str1 = files[i];
						using (FileStream fileStream = new FileStream(str1, FileMode.Open, FileAccess.Read))
						{
							IDocumentLocator documentLocator = new DocumentLocator(str1);
							ITypeId typeId = XamlRootNodeSniffer.SniffRootNodeType(fileStream, this.ProvideDocumentContext(documentLocator), out str);
							if (PlatformTypes.ResourceDictionary.IsAssignableFrom(typeId))
							{
								documentLocators.Add(documentLocator);
							}
						}
					}
				}
				return documentLocators;
			}
		}

		protected FolderBasedThemeManager(IPlatform platform) : base(platform)
		{
		}

		protected string GetAvailableSystemThemeFolder(params string[] subfolderNames)
		{
			string directoryName = Path.GetDirectoryName(typeof(ThemeManager).Assembly.Location);
			directoryName = Path.Combine(directoryName, "SystemThemes");
			string str = Path.Combine(directoryName, subfolderNames[0]);
			string str1 = str;
			for (int i = 1; i < (int)subfolderNames.Length && !string.IsNullOrEmpty(subfolderNames[i]); i++)
			{
				string str2 = Path.Combine(str1, subfolderNames[i]);
				if (!Directory.Exists(str2))
				{
					break;
				}
				str1 = str2;
				if ((new DirectoryInfo(str1)).EnumerateFileSystemInfos("*.xaml", SearchOption.TopDirectoryOnly).FirstOrDefault<FileSystemInfo>((FileSystemInfo info) => (int)(info.Attributes & FileAttributes.Directory) == 0) != null)
				{
					str = str1;
				}
			}
			return str;
		}

		public virtual string GetThemeFolder(FrameworkName targetFramework)
		{
			return null;
		}

		public virtual bool IsKnownStyleType(Type type)
		{
			return true;
		}
	}
}