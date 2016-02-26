using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project.ServiceExtensions.Documents;
using Microsoft.Expression.Project.Templates;
using Microsoft.Expression.SubsetFontTask.Zip;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;

namespace Microsoft.Expression.Project
{
	public class TemplateBase : ITemplate
	{
		internal const string CSharpName = "Visual C#";

		internal const string VisualBasicName = "Visual Basic";

		internal const string JavaScriptName = "JavaScript";

		protected const string TemplateArgumentProjectName = "projectname";

		protected const string TemplateArgumentSafeProjectName = "safeprojectname";

		protected const string TemplateArgumentAssemblyName = "assemblyname";

		protected const string TemplateArgumentSafeAssemblyName = "safeassemblyname";

		protected const string TemplateArgumentSafeItemName = "safeitemname";

		protected const string TemplateArgumentSafeItemRootName = "safeitemrootname";

		protected const string TemplateArgumentCultureName = "culture";

		protected const string TemplateArgumentRootNamespaceName = "rootnamespace";

		protected const string TemplateArgumentFileInputName = "fileinputname";

		protected const string TemplateArgumentFileInputExtensionName = "fileinputextension";

		private static Dictionary<string, string> projectTypeConversions;

		private int? sortOrder = null;

		private FrameworkElement icon;

		public bool BuildOnLoad
		{
			get
			{
				XmlNode[] buildOnLoad = this.Template.TemplateData.BuildOnLoad as XmlNode[];
				if (buildOnLoad == null || (int)buildOnLoad.Length <= 0)
				{
					return false;
				}
				return buildOnLoad[0].Value.ToString().Equals("true", StringComparison.OrdinalIgnoreCase);
			}
		}

		public virtual string DefaultName
		{
			get
			{
				if (string.IsNullOrEmpty(this.Template.TemplateData.DefaultName))
				{
					return string.Empty;
				}
				return this.Template.TemplateData.DefaultName;
			}
		}

		public string Description
		{
			get
			{
				if (this.Template.TemplateData.Description == null || string.IsNullOrEmpty(this.Template.TemplateData.Description.Value))
				{
					return string.Empty;
				}
				return this.Template.TemplateData.Description.Value;
			}
		}

		public string DisplayName
		{
			get
			{
				if (this.Template.TemplateData.Name == null || string.IsNullOrEmpty(this.Template.TemplateData.Name.Value))
				{
					return string.Empty;
				}
				return this.Template.TemplateData.Name.Value;
			}
		}

		public bool Hidden
		{
			get
			{
				return this.Template.TemplateData.Hidden;
			}
		}

		public virtual FrameworkElement Icon
		{
			get
			{
				if (this.icon != null)
				{
					return this.icon;
				}
				if (!string.IsNullOrEmpty(this.Template.TemplateData.Icon.Value))
				{
					try
					{
						Stream stream = this.OpenRead(this.Template.TemplateData.Icon.Value, this.TemplateLocation);
						this.icon = new Microsoft.Expression.Framework.Controls.Icon();
						BitmapDecoder bitmapDecoder = BitmapDecoder.Create(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
						if (!bitmapDecoder.IsDownloading)
						{
							try
							{
								((Microsoft.Expression.Framework.Controls.Icon)this.icon).Source = bitmapDecoder.Frames[0];
							}
							finally
							{
								stream.Close();
							}
						}
						else
						{
							bitmapDecoder.DownloadCompleted += new EventHandler((object o, EventArgs e) => {
								try
								{
									ImageSource item = bitmapDecoder.Frames[0];
									((Microsoft.Expression.Framework.Controls.Icon)this.icon).Source = item;
								}
								finally
								{
									stream.Close();
								}
							});
							bitmapDecoder.DownloadFailed += new EventHandler<ExceptionEventArgs>((object o, ExceptionEventArgs e) => {
								try
								{
								}
								finally
								{
									stream.Close();
								}
							});
						}
					}
					catch (NotSupportedException notSupportedException)
					{
					}
					catch (FileFormatException fileFormatException)
					{
					}
					catch (IOException oException)
					{
					}
					catch (SecurityException securityException)
					{
					}
					catch (UnauthorizedAccessException unauthorizedAccessException)
					{
					}
					catch (ArgumentException argumentException)
					{
					}
				}
				if (this.icon == null)
				{
					this.icon = FileTable.GetElement("Resources\\ProjectIcon.xaml");
				}
				return this.icon;
			}
		}

		public string Identifier
		{
			get
			{
				return this.Template.TemplateData.TemplateID;
			}
		}

		protected bool IsZippedTemplate
		{
			get
			{
				return TemplateBase.IsZipArchive(this.TemplateLocation);
			}
		}

		public string MaximumFrameworkVersion
		{
			get
			{
				if (this.Template.TemplateData.MaxFrameworkVersionSpecified)
				{
					switch (this.Template.TemplateData.MaxFrameworkVersion)
					{
						case VSTemplateTemplateDataMaxFrameworkVersion.Item20:
						{
							return "2.0";
						}
						case VSTemplateTemplateDataMaxFrameworkVersion.Item30:
						{
							return "3.0";
						}
						case VSTemplateTemplateDataMaxFrameworkVersion.Item35:
						{
							return "3.5";
						}
						case VSTemplateTemplateDataMaxFrameworkVersion.Item40:
						{
							return "4.0";
						}
					}
				}
				return "4.0";
			}
		}

		public string MinimumFrameworkVersion
		{
			get
			{
				if (this.Template.TemplateData.RequiredFrameworkVersionSpecified)
				{
					switch (this.Template.TemplateData.RequiredFrameworkVersion)
					{
						case VSTemplateTemplateDataRequiredFrameworkVersion.Item20:
						{
							return "2.0";
						}
						case VSTemplateTemplateDataRequiredFrameworkVersion.Item30:
						{
							return "3.0";
						}
						case VSTemplateTemplateDataRequiredFrameworkVersion.Item35:
						{
							return "3.5";
						}
						case VSTemplateTemplateDataRequiredFrameworkVersion.Item40:
						{
							return "4.0";
						}
					}
				}
				return "2.0";
			}
		}

		public int NumberOfParentCategoriesToRollUp
		{
			get
			{
				int num;
				if (string.IsNullOrEmpty(this.Template.TemplateData.NumberOfParentCategoriesToRollUp))
				{
					return 0;
				}
				if (int.TryParse(this.Template.TemplateData.NumberOfParentCategoriesToRollUp, out num))
				{
					return num;
				}
				return 0;
			}
		}

		public string ProjectSubType
		{
			get
			{
				return this.Template.TemplateData.ProjectSubType;
			}
		}

		public string ProjectSubTypes
		{
			get
			{
				return this.Template.TemplateData.ProjectSubTypes;
			}
		}

		public string ProjectType
		{
			get
			{
				return TemplateBase.ConvertProjectType(this.Template.TemplateData.ProjectType);
			}
		}

		private static Dictionary<string, string> ProjectTypeConversions
		{
			get
			{
				if (TemplateBase.projectTypeConversions == null)
				{
					Dictionary<string, string> strs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
					{
						{ "C#", "Visual C#" },
						{ "CSharp", "Visual C#" },
						{ "VB", "Visual Basic" },
						{ "VisualBasic", "Visual Basic" },
						{ "Javascript", "JavaScript" }
					};
					TemplateBase.projectTypeConversions = strs;
				}
				return TemplateBase.projectTypeConversions;
			}
		}

		public bool ProvideDefaultName
		{
			get
			{
				if (!this.Template.TemplateData.ProvideDefaultNameSpecified)
				{
					return true;
				}
				return this.Template.TemplateData.ProvideDefaultName;
			}
		}

		public int SortOrder
		{
			get
			{
				if (!this.sortOrder.HasValue)
				{
					this.sortOrder = new int?(2147483647);
					if (!string.IsNullOrEmpty(this.Template.TemplateData.SortOrder))
					{
						try
						{
							this.sortOrder = new int?(Convert.ToInt32(this.Template.TemplateData.SortOrder, CultureInfo.InvariantCulture));
						}
						catch (FormatException formatException)
						{
						}
						catch (OverflowException overflowException)
						{
						}
					}
				}
				return this.sortOrder.Value;
			}
		}

		internal VSTemplate Template
		{
			get;
			private set;
		}

		public string TemplateGroupID
		{
			get
			{
				if (this.Template.TemplateData == null || string.IsNullOrEmpty(this.Template.TemplateData.TemplateGroupID))
				{
					return string.Empty;
				}
				return this.Template.TemplateData.TemplateGroupID;
			}
		}

		public string TemplateID
		{
			get
			{
				if (string.IsNullOrEmpty(this.Identifier))
				{
					return this.DisplayName;
				}
				return this.Identifier;
			}
		}

		protected Uri TemplateLocation
		{
			get;
			private set;
		}

		private Microsoft.Expression.SubsetFontTask.Zip.ZipArchive ZipArchive
		{
			get
			{
				Microsoft.Expression.SubsetFontTask.Zip.ZipArchive zipArchive = null;
				if (this.IsZippedTemplate)
				{
					zipArchive = ZipHelper.CreateZipArchive(this.TemplateLocation.LocalPath);
				}
				return zipArchive;
			}
		}

		protected TemplateBase(VSTemplate template, Uri templateLocation)
		{
			if (templateLocation == null)
			{
				throw new ArgumentNullException("templateLocation");
			}
			if (template == null)
			{
				throw new ArgumentNullException("template");
			}
			this.Template = template;
			this.TemplateLocation = templateLocation;
		}

		private static string ConvertProjectType(string projectType)
		{
			if (string.IsNullOrEmpty(projectType) || !TemplateBase.ProjectTypeConversions.ContainsKey(projectType))
			{
				return projectType;
			}
			return TemplateBase.ProjectTypeConversions[projectType];
		}

		protected bool CreateDirectory(Uri destinationPath, IEnumerable<TemplateArgument> templateArguments)
		{
			bool flag;
			string str = TemplateParser.ReplaceTemplateArguments(destinationPath.LocalPath, templateArguments);
			if (!string.IsNullOrEmpty(str) && !Microsoft.Expression.Framework.Documents.PathHelper.DirectoryExists(str))
			{
				try
				{
					Directory.CreateDirectory(str);
					return true;
				}
				catch (IOException oException)
				{
					flag = false;
				}
				catch (UnauthorizedAccessException unauthorizedAccessException)
				{
					flag = false;
				}
				return flag;
			}
			return true;
		}

		protected bool CreateFile(string source, Uri sourceLocation, Uri destination, bool replaceParameters, IEnumerable<TemplateArgument> templateArguments)
		{
			bool flag;
			string str = TemplateParser.ReplaceTemplateArguments(destination.LocalPath, templateArguments);
			string directoryName = Path.GetDirectoryName(str);
			if (!string.IsNullOrEmpty(directoryName) && !Microsoft.Expression.Framework.Documents.PathHelper.DirectoryExists(directoryName))
			{
				try
				{
					Directory.CreateDirectory(directoryName);
				}
				catch (IOException oException)
				{
					flag = false;
					return flag;
				}
				catch (UnauthorizedAccessException unauthorizedAccessException)
				{
					flag = false;
					return flag;
				}
			}
			if (!replaceParameters)
			{
				try
				{
					if (!TemplateBase.IsZipArchive(sourceLocation))
					{
						File.Copy(this.ResolveFileUri(source, sourceLocation).LocalPath, str, true);
					}
					else if (!this.ZipArchive.Exists(source))
					{
						flag = false;
						return flag;
					}
					else
					{
						this.ZipArchive.CopyToFile(source, str);
					}
					return true;
				}
				catch (IOException oException1)
				{
					flag = false;
				}
				catch (UnauthorizedAccessException unauthorizedAccessException1)
				{
					flag = false;
				}
			}
			else
			{
				StreamReader streamReader = this.OpenText(source, sourceLocation);
				try
				{
					File.WriteAllText(str, TemplateParser.ParseTemplate(streamReader, templateArguments), Encoding.UTF8);
					return true;
				}
				catch (IOException oException2)
				{
					flag = false;
				}
				catch (UnauthorizedAccessException unauthorizedAccessException2)
				{
					flag = false;
				}
				catch (SecurityException securityException)
				{
					flag = false;
				}
			}
			return flag;
		}

		protected bool FileExists(string file)
		{
			if (!this.IsZippedTemplate)
			{
				return Microsoft.Expression.Framework.Documents.PathHelper.FileExists(Microsoft.Expression.Framework.Documents.PathHelper.ResolveRelativePath(this.TemplateLocation.LocalPath, file));
			}
			Microsoft.Expression.SubsetFontTask.Zip.ZipArchive zipArchive = this.ZipArchive;
			if (this.ZipArchive[file] != null)
			{
				return true;
			}
			if (zipArchive[file.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar)] != null)
			{
				return true;
			}
			return false;
		}

		protected ICodeDocumentType GetCodeDocumentType(IServiceProvider serviceProvider)
		{
			string projectType = this.ProjectType;
			string str = projectType;
			if (projectType != null)
			{
				if (str == "Visual C#")
				{
					return serviceProvider.DocumentTypes().CSharpDocumentType();
				}
				if (str == "Visual Basic")
				{
					return serviceProvider.DocumentTypes().VisualBasicDocumentType();
				}
				if (str == "JavaScript")
				{
					return serviceProvider.DocumentTypes().JavaScriptDocumentType();
				}
			}
			return serviceProvider.DocumentTypes().DefaultCodeDocumentType();
		}

		protected bool IsDirectory(string path)
		{
			if (this.IsZippedTemplate)
			{
				return !this.FileExists(path);
			}
			return Microsoft.Expression.Framework.Documents.PathHelper.IsDirectory(this.ResolveFileUri(path, this.TemplateLocation).LocalPath);
		}

		internal static bool IsZipArchive(Uri location)
		{
			if (!location.IsFile)
			{
				return false;
			}
			return TemplateBase.IsZipArchive(location.LocalPath);
		}

		internal static bool IsZipArchive(string location)
		{
			if (Microsoft.Expression.Framework.Documents.PathHelper.IsDirectory(location) || !Microsoft.Expression.Framework.Documents.PathHelper.FileExists(location))
			{
				return false;
			}
			return Microsoft.Expression.Framework.Documents.PathHelper.GetSafeExtension(location).Equals(".zip", StringComparison.OrdinalIgnoreCase);
		}

		protected Stream OpenRead(string file, Uri root)
		{
			if (TemplateBase.IsZipArchive(root))
			{
				return this.ZipArchive.OpenRead(file);
			}
			Uri uri = this.ResolveFileUri(file, root);
			if (!uri.IsFile || !File.Exists(uri.LocalPath))
			{
				return null;
			}
			return File.OpenRead(uri.LocalPath);
		}

		protected StreamReader OpenText(string file, Uri root)
		{
			if (!TemplateBase.IsZipArchive(root))
			{
				return new StreamReader(this.ResolveFileUri(file, root).LocalPath);
			}
			Microsoft.Expression.SubsetFontTask.Zip.ZipArchive zipArchive = this.ZipArchive;
			if (zipArchive[file] != null)
			{
				return zipArchive.OpenText(file);
			}
			string str = file.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
			if (zipArchive[str] == null)
			{
				CultureInfo currentCulture = CultureInfo.CurrentCulture;
				string couldNotFindFile = ExceptionStringTable.CouldNotFindFile;
				object[] objArray = new object[] { string.Concat(this.TemplateLocation.LocalPath, "[", file, "]") };
				throw new FileNotFoundException(string.Format(currentCulture, couldNotFindFile, objArray));
			}
			return zipArchive.OpenText(str);
		}

		protected Uri ResolveFileUri(string file, Uri root)
		{
			Uri uri;
			if (!root.IsAbsoluteUri)
			{
				throw new ArgumentException("Root is not absolute", "root");
			}
			if (!Uri.TryCreate(file, UriKind.RelativeOrAbsolute, out uri))
			{
				Uri.TryCreate(root, file, out uri);
			}
			else if (!uri.IsAbsoluteUri)
			{
				try
				{
					string str = Microsoft.Expression.Framework.Documents.PathHelper.ResolveRelativePath(Microsoft.Expression.Framework.Documents.PathHelper.GetDirectoryNameOrRoot(root.LocalPath), uri.OriginalString);
					uri = new Uri(str, UriKind.Absolute);
				}
				catch (UriFormatException uriFormatException)
				{
				}
			}
			return uri;
		}
	}
}