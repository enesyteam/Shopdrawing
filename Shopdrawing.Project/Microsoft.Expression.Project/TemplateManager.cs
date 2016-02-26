using Microsoft.Expression.Extensibility.Project;
using Microsoft.Expression.Extensibility.Project.Templates;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project.Licensing;
using Microsoft.Expression.Project.ServiceExtensions;
using Microsoft.Expression.Project.Templates;
using Microsoft.Expression.SubsetFontTask.Zip;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace Microsoft.Expression.Project
{
	public class TemplateManager : ITemplateManager
	{
		private List<IProjectTemplate> projectTemplates;

		private ReadOnlyCollection<IProjectTemplate> readOnlyProjectTemplates;

		private bool templatesLoaded;

		private List<IProjectTemplate> sampleProjectTemplates;

		private ReadOnlyCollection<IProjectTemplate> readOnlySampleProjectTemplates;

		private bool sampleTemplatesLoaded;

		private List<IProjectItemTemplate> projectItemTemplates;

		private ReadOnlyCollection<IProjectItemTemplate> readOnlyProjectItemTemplates;

		private bool projectItemTemplatesLoaded;

		private static string baseItemTemplateFolder;

		private static string baseProjectTemplateFolder;

		private static string baseSampleTemplateFolder;

		private static string userItemTemplateFolder;

		private static string userProjectTemplateFolder;

		private static IEnumerable<string> blendSdkItemTemplateFolders;

		private readonly static string ItemTemplateFolderName;

		private readonly static string UserItemTemplateFolderName;

		private readonly static string ProjectTemplateFolderName;

		private readonly static string SampleTemplateFolderName;

		private IServiceProvider serviceProvider;

		private static XmlSerializer templateXmlSerializer;

		public static string BaseItemTemplateFolder
		{
			get
			{
				if (TemplateManager.baseItemTemplateFolder == null)
				{
					TemplateManager.baseItemTemplateFolder = TemplateManager.TranslatedFolder(TemplateManager.ItemTemplateFolderName);
				}
				return TemplateManager.baseItemTemplateFolder;
			}
		}

		public static string BaseProjectTemplateFolder
		{
			get
			{
				if (TemplateManager.baseProjectTemplateFolder == null)
				{
					TemplateManager.baseProjectTemplateFolder = TemplateManager.TranslatedFolder(TemplateManager.ProjectTemplateFolderName);
				}
				return TemplateManager.baseProjectTemplateFolder;
			}
		}

		public static string BaseSampleTemplateFolder
		{
			get
			{
				if (TemplateManager.baseSampleTemplateFolder == null)
				{
					TemplateManager.baseSampleTemplateFolder = TemplateManager.TranslatedFolder(TemplateManager.SampleTemplateFolderName);
				}
				return TemplateManager.baseSampleTemplateFolder;
			}
		}

		public static IEnumerable<string> BlendSdkItemTemplateFolders
		{
			get
			{
				if (TemplateManager.blendSdkItemTemplateFolders == null && BlendSdkHelper.IsAnySdkInstalled)
				{
					TemplateManager.blendSdkItemTemplateFolders = BlendSdkHelper.TemplatePaths;
				}
				return TemplateManager.blendSdkItemTemplateFolders;
			}
		}

		private bool IsPrototypingEnabled
		{
			get
			{
				ILicenseService service = this.Services.GetService<ILicenseService>();
				if (service == null || !LicensingHelper.IsSketchFlowLicensed(service))
				{
					return false;
				}
				return this.Services.PrototypingProjectService() != null;
			}
		}

		public IEnumerable<IProjectItemTemplate> ProjectItemTemplates
		{
			get
			{
				if (!this.projectItemTemplatesLoaded)
				{
					this.GetAllProjectItemTemplatesInSubfolder(TemplateManager.BaseItemTemplateFolder, this.projectItemTemplates, new TemplateManager.TemplateValidator(this.ValidateItemTemplate));
					this.GetAllProjectItemTemplatesInSubfolder(TemplateManager.UserItemTemplateFolder, this.projectItemTemplates, new TemplateManager.TemplateValidator(this.ValidateItemTemplate));
					if (BlendSdkHelper.IsAnySdkInstalled)
					{
						foreach (string blendSdkItemTemplateFolder in TemplateManager.BlendSdkItemTemplateFolders)
						{
							this.GetAllProjectItemTemplatesInSubfolder(blendSdkItemTemplateFolder, this.projectItemTemplates, new TemplateManager.TemplateValidator(this.ValidateSdkItemTemplate));
						}
					}
				}
				this.projectItemTemplatesLoaded = true;
				return this.readOnlyProjectItemTemplates;
			}
		}

		public IEnumerable<IProjectTemplate> ProjectTemplates
		{
			get
			{
				if (!this.templatesLoaded)
				{
					this.GetAllProjectTemplatesInSubfolder(TemplateManager.BaseProjectTemplateFolder, this.projectTemplates, new TemplateManager.TemplateValidator(this.ValidateProjectTemplate));
					this.GetAllProjectTemplatesInSubfolder(TemplateManager.UserProjectTemplateFolder, this.projectTemplates, new TemplateManager.TemplateValidator(this.ValidateProjectTemplate));
				}
				this.templatesLoaded = true;
				return this.readOnlyProjectTemplates;
			}
		}

		public IEnumerable<IProjectTemplate> SampleProjectTemplates
		{
			get
			{
				if (!this.sampleTemplatesLoaded)
				{
					this.GetAllProjectTemplatesInSubfolder(TemplateManager.BaseSampleTemplateFolder, this.sampleProjectTemplates, new TemplateManager.TemplateValidator(this.ValidateSampleTemplate));
				}
				this.sampleTemplatesLoaded = true;
				return this.readOnlySampleProjectTemplates;
			}
		}

		internal IServiceProvider Services
		{
			get
			{
				return this.serviceProvider;
			}
		}

		[ImportMany]
		internal IEnumerable<ITemplateCategoryInformation> TemplateCategoryInformation
		{
			get;
			set;
		}

		[ImportMany]
		internal IEnumerable<ITemplateFilter> TemplateFilters
		{
			get;
			set;
		}

		internal static XmlSerializer TemplateXmlSerializer
		{
			get
			{
				if (TemplateManager.templateXmlSerializer == null)
				{
					TemplateManager.templateXmlSerializer = new XmlSerializer(typeof(Microsoft.Expression.Project.Templates.VSTemplate));
				}
				return TemplateManager.templateXmlSerializer;
			}
		}

		public static string UserItemTemplateFolder
		{
			get
			{
				if (TemplateManager.userItemTemplateFolder == null)
				{
					TemplateManager.userItemTemplateFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), Path.Combine("Expression\\Blend 4", TemplateManager.UserItemTemplateFolderName));
				}
				return TemplateManager.userItemTemplateFolder;
			}
		}

		public static string UserProjectTemplateFolder
		{
			get
			{
				if (TemplateManager.userProjectTemplateFolder == null)
				{
					TemplateManager.userProjectTemplateFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), Path.Combine("Expression\\Blend 4", TemplateManager.ProjectTemplateFolderName));
				}
				return TemplateManager.userProjectTemplateFolder;
			}
		}

		static TemplateManager()
		{
			TemplateManager.ItemTemplateFolderName = "Templates";
			TemplateManager.UserItemTemplateFolderName = "ItemTemplates";
			TemplateManager.ProjectTemplateFolderName = "ProjectTemplates";
			TemplateManager.SampleTemplateFolderName = "Samples";
			TemplateManager.templateXmlSerializer = null;
			try
			{
				TemplateManager.templateXmlSerializer = new XmlSerializer(typeof(Microsoft.Expression.Project.Templates.VSTemplate));
			}
			catch (Exception exception)
			{
			}
		}

		public TemplateManager(IServiceProvider serviceProvider)
		{
			this.serviceProvider = serviceProvider;
			this.projectTemplates = new List<IProjectTemplate>();
			this.readOnlyProjectTemplates = new ReadOnlyCollection<IProjectTemplate>(this.projectTemplates);
			this.sampleProjectTemplates = new List<IProjectTemplate>();
			this.readOnlySampleProjectTemplates = new ReadOnlyCollection<IProjectTemplate>(this.sampleProjectTemplates);
			this.projectItemTemplates = new List<IProjectItemTemplate>();
			this.readOnlyProjectItemTemplates = new ReadOnlyCollection<IProjectItemTemplate>(this.projectItemTemplates);
		}

		public Icon FindCategoryIcon(string name)
		{
			Icon icon;
			if (this.TemplateCategoryInformation != null)
			{
				using (IEnumerator<ITemplateCategoryInformation> enumerator = this.TemplateCategoryInformation.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ITemplateCategoryInformation current = enumerator.Current;
						try
						{
							if (name.Equals(current.Name, StringComparison.OrdinalIgnoreCase) && current.Image != null)
							{
								icon = new Icon()
								{
									Source = current.Image.Source
								};
								return icon;
							}
						}
						catch (Exception exception)
						{
						}
					}
					return null;
				}
				return icon;
			}
			return null;
		}

		private void GetAllProjectItemTemplatesInSubfolder(string templatePath, IList<IProjectItemTemplate> templateCollection, TemplateManager.TemplateValidator filter)
		{
			this.GetAllVSTemplatesInSubfolder(templatePath, filter, (Microsoft.Expression.Project.Templates.VSTemplate vsTemplate, string vsTemplatePath) => {
				IProjectItemTemplate projectItemTemplate = new ProjectItemTemplate(vsTemplate, new Uri(vsTemplatePath));
				templateCollection.Add(projectItemTemplate);
			});
		}

		private void GetAllProjectTemplatesInSubfolder(string templatePath, IList<IProjectTemplate> templateCollection, TemplateManager.TemplateValidator filter)
		{
			this.GetAllVSTemplatesInSubfolder(templatePath, filter, (Microsoft.Expression.Project.Templates.VSTemplate vsTemplate, string vsTemplatePath) => {
				IProjectTemplate projectTemplate = new ProjectTemplate(vsTemplate, new Uri(vsTemplatePath));
				templateCollection.Add(projectTemplate);
			});
		}

		private void GetAllVSTemplatesInSubfolder(string templatePath, TemplateManager.TemplateValidator filter, Action<Microsoft.Expression.Project.Templates.VSTemplate, string> acceptor)
		{
			if (!Microsoft.Expression.Framework.Documents.PathHelper.DirectoryExists(templatePath))
			{
				return;
			}
			XmlSerializer templateXmlSerializer = null;
			try
			{
				templateXmlSerializer = TemplateManager.TemplateXmlSerializer;
			}
			catch (TypeInitializationException typeInitializationException)
			{
				this.LogTemplateLoadError(templatePath, typeInitializationException.Message);
			}
			catch (ExternalException externalException)
			{
				this.LogTemplateLoadError(templatePath, externalException.Message);
			}
			if (templateXmlSerializer == null)
			{
				return;
			}
			foreach (string str in new List<string>(Directory.GetFiles(templatePath, "*.vstemplate", SearchOption.AllDirectories)))
			{
				using (FileStream fileStream = new FileStream(str, FileMode.Open, FileAccess.Read))
				{
					this.readTemplate(Microsoft.Expression.Framework.Documents.PathHelper.GetDirectoryNameOrRoot(str), str, fileStream, templateXmlSerializer, filter, acceptor);
				}
			}
			foreach (string str1 in new List<string>(Directory.GetFiles(templatePath, "*.zip", SearchOption.AllDirectories)))
			{
				try
				{
					foreach (ZipArchiveFile file in ZipHelper.CreateZipArchive(str1).Files)
					{
						try
						{
							if (Path.GetExtension(file.Name).Equals(".vstemplate", StringComparison.OrdinalIgnoreCase) && string.IsNullOrEmpty(Path.GetDirectoryName(file.Name)))
							{
								using (Stream stream = file.OpenRead())
								{
									this.readTemplate(str1, string.Concat(str1, "[", file.Name, "]"), stream, templateXmlSerializer, filter, acceptor);
								}
							}
						}
						catch (Exception exception1)
						{
							Exception exception = exception1;
							this.LogTemplateLoadError(string.Concat(str1, "[", file.Name, "]"), exception.Message);
						}
					}
				}
				catch (Exception exception2)
				{
					this.LogTemplateLoadError(str1, exception2.Message);
				}
			}
		}

		public static IEnumerable<TemplateArgument> GetDefaultArguments(IExpressionInformationService expressionInformationService)
		{
			List<TemplateArgument> templateArguments = new List<TemplateArgument>(22);
			DateTime now = DateTime.Now;
			templateArguments.Add(new TemplateArgument("time", now.ToString(CultureInfo.CurrentUICulture)));
			int year = DateTime.Now.Year;
			templateArguments.Add(new TemplateArgument("year", year.ToString(CultureInfo.CurrentUICulture)));
			templateArguments.Add(new TemplateArgument("clrversion", Environment.Version.ToString()));
			templateArguments.Add(new TemplateArgument("machinename", Environment.MachineName));
			templateArguments.Add(new TemplateArgument("userdomain", Environment.UserDomainName));
			templateArguments.Add(new TemplateArgument("username", Environment.UserName));
			templateArguments.Add(new TemplateArgument("expressionblendversion", (expressionInformationService == null ? "0.0.0.0" : expressionInformationService.Version.ToString())));
			List<TemplateArgument> templateArguments1 = templateArguments;
			for (int i = 1; i <= 10; i++)
			{
				Guid guid = Guid.NewGuid();
				CultureInfo invariantCulture = CultureInfo.InvariantCulture;
				object[] objArray = new object[] { i };
				templateArguments1.Add(new TemplateArgument(string.Format(invariantCulture, "guid{0}", objArray), guid.ToString("D")));
			}
			string str = RegistryHelper.RetrieveRegistryValue<string>(Registry.LocalMachine, "Software\\Microsoft\\Windows NT\\CurrentVersion", "RegisteredOrganization");
			if (!string.IsNullOrEmpty(str))
			{
				templateArguments1.Add(new TemplateArgument("registeredorganization", str));
			}
			string str1 = RegistryHelper.RetrieveRegistryValue<string>(Registry.LocalMachine, "Software\\Microsoft\\Microsoft SDKs\\Silverlight\\v3.0\\ReferenceAssemblies", "SLRuntimeInstallVersion") ?? "0.0.0.0";
			string str2 = RegistryHelper.RetrieveRegistryValue<string>(Registry.LocalMachine, "Software\\Microsoft\\Microsoft SDKs\\Silverlight\\v4.0\\ReferenceAssemblies", "SLRuntimeInstallVersion") ?? "0.0.0.0";
			templateArguments1.Add(new TemplateArgument("silverlight3sdkversion", str1));
			templateArguments1.Add(new TemplateArgument("silverlight4sdkversion", str2));
			return templateArguments1;
		}

		internal static bool IsPathFromSDK(string path, FrameworkName frameworkName)
		{
			string templatePath = BlendSdkHelper.GetTemplatePath(frameworkName);
			if (string.IsNullOrWhiteSpace(templatePath))
			{
				return false;
			}
			return path.StartsWith(templatePath, StringComparison.OrdinalIgnoreCase);
		}

		internal void LogTemplateLoadError(string vsTemplatePath, string message)
		{
			IMessageLoggingService messageLoggingService = this.Services.MessageLoggingService();
			CultureInfo currentCulture = CultureInfo.CurrentCulture;
			string errorLoadingTemplate = StringTable.ErrorLoadingTemplate;
			object[] objArray = new object[] { vsTemplatePath, message };
			messageLoggingService.WriteLine(string.Format(currentCulture, errorLoadingTemplate, objArray));
		}

		private void readTemplate(string vsTemplatePath, string vsTemplateLocation, Stream fs, XmlSerializer serializer, TemplateManager.TemplateValidator filter, Action<Microsoft.Expression.Project.Templates.VSTemplate, string> acceptor)
		{
			Microsoft.Expression.Project.Templates.VSTemplate vSTemplate = null;
			try
			{
				vSTemplate = (Microsoft.Expression.Project.Templates.VSTemplate)serializer.Deserialize(fs);
			}
			catch (InvalidOperationException invalidOperationException)
			{
				this.LogTemplateLoadError(vsTemplateLocation, invalidOperationException.Message);
			}
			if (vSTemplate != null && (filter == null || filter(vsTemplateLocation, vSTemplate)) && this.ShouldLoad(vSTemplate))
			{
				acceptor(vSTemplate, vsTemplatePath);
			}
		}

		private bool ShouldLoad(Microsoft.Expression.Project.Templates.VSTemplate vsTemplate)
		{
			bool flag;
			if (this.TemplateFilters != null)
			{
				using (IEnumerator<ITemplateFilter> enumerator = this.TemplateFilters.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ITemplateFilter current = enumerator.Current;
						try
						{
							if (!current.ShouldLoad(vsTemplate.TemplateExtensibilityWrapper))
							{
								flag = false;
								return flag;
							}
						}
						catch (Exception exception)
						{
						}
					}
					return true;
				}
				return flag;
			}
			return true;
		}

		public bool ShouldUseTemplate(Microsoft.Expression.Project.IProject project, ITemplate templateToTest)
		{
			bool flag;
			Microsoft.Expression.Project.Templates.VSTemplate template;
			if (this.TemplateFilters != null)
			{
				Microsoft.Expression.Extensibility.Project.IProject project1 = null;
				TemplateBase templateBase = templateToTest as TemplateBase;
				if (templateBase != null)
				{
					template = templateBase.Template;
				}
				else
				{
					template = null;
				}
				Microsoft.Expression.Project.Templates.VSTemplate vSTemplate = template;
				if (vSTemplate != null)
				{
					using (IEnumerator<ITemplateFilter> enumerator = this.TemplateFilters.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							ITemplateFilter current = enumerator.Current;
							try
							{
								if (project1 == null)
								{
									project1 = new Microsoft.Expression.Extensibility.Project.Project(project);
								}
								if (!current.ShouldUseTemplate(project1, vSTemplate.TemplateExtensibilityWrapper))
								{
									flag = false;
									return flag;
								}
							}
							catch (Exception exception)
							{
							}
						}
						return true;
					}
					return flag;
				}
			}
			return true;
		}

		public static string TranslatedFolder(string rootFolderName)
		{
			string directoryName = Path.GetDirectoryName(Assembly.GetAssembly(typeof(TemplateManager)).Location);
			string str = Path.Combine(directoryName, rootFolderName);
			for (CultureInfo i = CultureInfo.CurrentUICulture; i != null && !i.Equals(CultureInfo.InvariantCulture); i = i.Parent)
			{
				string str1 = Path.Combine(str, i.Name);
				if (Microsoft.Expression.Framework.Documents.PathHelper.DirectoryExists(str1))
				{
					return str1;
				}
			}
			return Path.Combine(str, "en");
		}

		private bool ValidateAnyTemplate(string vsTemplatePath, Microsoft.Expression.Project.Templates.VSTemplate vsTemplate)
		{
			bool flag;
			if (vsTemplate.TemplateContent == null)
			{
				CultureInfo currentCulture = CultureInfo.CurrentCulture;
				string missingTemplateElement = StringTable.MissingTemplateElement;
				object[] objArray = new object[] { "TemplateContent" };
				this.LogTemplateLoadError(vsTemplatePath, string.Format(currentCulture, missingTemplateElement, objArray));
				return false;
			}
			if (vsTemplate.TemplateContent.Items == null)
			{
				CultureInfo cultureInfo = CultureInfo.CurrentCulture;
				string invalidTemplateElement = StringTable.InvalidTemplateElement;
				object[] objArray1 = new object[] { "TemplateContent" };
				this.LogTemplateLoadError(vsTemplatePath, string.Format(cultureInfo, invalidTemplateElement, objArray1));
				return false;
			}
			if (vsTemplate.TemplateContent.Items != null)
			{
				object[] items = vsTemplate.TemplateContent.Items;
				int num = 0;
				while (true)
				{
					if (num >= (int)items.Length)
					{
						goto Label0;
					}
					object obj = items[num];
					if (!(obj is VSTemplateTemplateContentProjectCollection))
					{
						VSTemplateTemplateContentReferences vSTemplateTemplateContentReference = obj as VSTemplateTemplateContentReferences;
						if (vSTemplateTemplateContentReference != null)
						{
							if (vSTemplateTemplateContentReference.References != null)
							{
								bool flag1 = false;
								VSTemplateTemplateContentReferencesReference[] references = vSTemplateTemplateContentReference.References;
								int num1 = 0;
								while (num1 < (int)references.Length)
								{
									VSTemplateTemplateContentReferencesReference vSTemplateTemplateContentReferencesReference = references[num1];
									if (vSTemplateTemplateContentReferencesReference.Assembly == null)
									{
										CultureInfo currentCulture1 = CultureInfo.CurrentCulture;
										string str = StringTable.InvalidTemplateElement;
										object[] objArray2 = new object[] { "Reference" };
										this.LogTemplateLoadError(vsTemplatePath, string.Format(currentCulture1, str, objArray2));
										flag = false;
										return flag;
									}
									else if (!string.IsNullOrEmpty(vSTemplateTemplateContentReferencesReference.Assembly))
									{
										flag1 = true;
										num1++;
									}
									else
									{
										CultureInfo cultureInfo1 = CultureInfo.CurrentCulture;
										string invalidTemplateElement1 = StringTable.InvalidTemplateElement;
										object[] objArray3 = new object[] { "Assembly" };
										this.LogTemplateLoadError(vsTemplatePath, string.Format(cultureInfo1, invalidTemplateElement1, objArray3));
										flag = false;
										return flag;
									}
								}
								if (!flag1)
								{
									CultureInfo currentCulture2 = CultureInfo.CurrentCulture;
									string str1 = StringTable.InvalidTemplateElement;
									object[] objArray4 = new object[] { "References" };
									this.LogTemplateLoadError(vsTemplatePath, string.Format(currentCulture2, str1, objArray4));
									flag = false;
									break;
								}
							}
							else
							{
								CultureInfo cultureInfo2 = CultureInfo.CurrentCulture;
								string invalidTemplateElement2 = StringTable.InvalidTemplateElement;
								object[] objArray5 = new object[] { "References" };
								this.LogTemplateLoadError(vsTemplatePath, string.Format(cultureInfo2, invalidTemplateElement2, objArray5));
								flag = false;
								break;
							}
						}
						num++;
					}
					else
					{
						CultureInfo currentCulture3 = CultureInfo.CurrentCulture;
						string unsupportedTemplateElement = StringTable.UnsupportedTemplateElement;
						object[] objArray6 = new object[] { "ProjectCollection" };
						this.LogTemplateLoadError(vsTemplatePath, string.Format(currentCulture3, unsupportedTemplateElement, objArray6));
						flag = false;
						break;
					}
				}
				return flag;
			}
		Label0:
			if (vsTemplate.TemplateData == null)
			{
				CultureInfo cultureInfo3 = CultureInfo.CurrentCulture;
				string missingTemplateElement1 = StringTable.MissingTemplateElement;
				object[] objArray7 = new object[] { "TemplateData" };
				this.LogTemplateLoadError(vsTemplatePath, string.Format(cultureInfo3, missingTemplateElement1, objArray7));
				return false;
			}
			if (!this.ValidateTemplateNameDescriptionIcon(vsTemplatePath, vsTemplate.TemplateData.Name, "Name") || !this.ValidateTemplateNameDescriptionIcon(vsTemplatePath, vsTemplate.TemplateData.Description, "Description"))
			{
				return false;
			}
			if (vsTemplate.TemplateData.MaxFrameworkVersionSpecified && (vsTemplate.TemplateData.MaxFrameworkVersion == VSTemplateTemplateDataMaxFrameworkVersion.Item20 || vsTemplate.TemplateData.MaxFrameworkVersion == VSTemplateTemplateDataMaxFrameworkVersion.Item30 && vsTemplate.TemplateData.ProjectSubType != "Silverlight"))
			{
				CultureInfo currentCulture4 = CultureInfo.CurrentCulture;
				string str2 = StringTable.InvalidTemplateElement;
				object[] objArray8 = new object[] { "MaxFrameworkVersion" };
				this.LogTemplateLoadError(vsTemplatePath, string.Format(currentCulture4, str2, objArray8));
				return false;
			}
			if (vsTemplate.TemplateData.DefaultName != null && !Microsoft.Expression.Framework.Documents.PathHelper.IsValidFileOrDirectoryName(vsTemplate.TemplateData.DefaultName))
			{
				CultureInfo cultureInfo4 = CultureInfo.CurrentCulture;
				string invalidTemplateElement3 = StringTable.InvalidTemplateElement;
				object[] objArray9 = new object[] { "DefaultName" };
				this.LogTemplateLoadError(vsTemplatePath, string.Format(cultureInfo4, invalidTemplateElement3, objArray9));
				return false;
			}
			if (vsTemplate.WizardData != null)
			{
				CultureInfo currentCulture5 = CultureInfo.CurrentCulture;
				string unsupportedTemplateElement1 = StringTable.UnsupportedTemplateElement;
				object[] objArray10 = new object[] { "WizardData" };
				this.LogTemplateLoadError(vsTemplatePath, string.Format(currentCulture5, unsupportedTemplateElement1, objArray10));
				return false;
			}
			if (vsTemplate.WizardExtension == null)
			{
				return true;
			}
			CultureInfo cultureInfo5 = CultureInfo.CurrentCulture;
			string unsupportedTemplateElement2 = StringTable.UnsupportedTemplateElement;
			object[] objArray11 = new object[] { "WizardExtension" };
			this.LogTemplateLoadError(vsTemplatePath, string.Format(cultureInfo5, unsupportedTemplateElement2, objArray11));
			return false;
		}

		internal bool ValidateItemTemplate(string vsTemplatePath, Microsoft.Expression.Project.Templates.VSTemplate vsTemplate)
		{
			if (vsTemplatePath == null)
			{
				throw new ArgumentNullException("vsTemplatePath");
			}
			if (vsTemplate == null)
			{
				throw new ArgumentNullException("vsTemplate");
			}
			if (!this.ValidateTemplateType(vsTemplatePath, vsTemplate, "Item"))
			{
				return false;
			}
			if (!this.ValidateAnyTemplate(vsTemplatePath, vsTemplate))
			{
				return false;
			}
			if (!this.ValidateTemplateNameDescriptionIcon(vsTemplatePath, vsTemplate.TemplateData.Icon, "Icon"))
			{
				return false;
			}
			bool flag = false;
			if (vsTemplate.TemplateContent.Items != null)
			{
				object[] items = vsTemplate.TemplateContent.Items;
				for (int i = 0; i < (int)items.Length; i++)
				{
					object obj = items[i];
					if (obj is VSTemplateTemplateContentProjectItem)
					{
						flag = true;
					}
					if (obj is VSTemplateTemplateContentReferences)
					{
						flag = true;
					}
					if (obj is VSTemplateTemplateContentProject)
					{
						CultureInfo currentCulture = CultureInfo.CurrentCulture;
						string unsupportedTemplateElement = StringTable.UnsupportedTemplateElement;
						object[] objArray = new object[] { "Project" };
						this.LogTemplateLoadError(vsTemplatePath, string.Format(currentCulture, unsupportedTemplateElement, objArray));
						return false;
					}
				}
			}
			if (flag)
			{
				return true;
			}
			CultureInfo cultureInfo = CultureInfo.CurrentCulture;
			string invalidTemplateElement = StringTable.InvalidTemplateElement;
			object[] objArray1 = new object[] { "TemplateContent" };
			this.LogTemplateLoadError(vsTemplatePath, string.Format(cultureInfo, invalidTemplateElement, objArray1));
			return false;
		}

		private bool ValidateProjectOrSampleTemplate(string vsTemplatePath, Microsoft.Expression.Project.Templates.VSTemplate vsTemplate)
		{
			bool flag;
			object[] objArray;
			CultureInfo currentCulture;
			string invalidTemplateElement;
			if (vsTemplate.TemplateData != null && vsTemplate.TemplateData.ExpressionBlendPrototypingEnabled && (!this.IsPrototypingEnabled || vsTemplate.TemplateData.ProjectSubType == "Silverlight" && !BlendSdkHelper.IsSdkInstalled(BlendSdkHelper.CurrentSilverlightVersion) || vsTemplate.TemplateData.ProjectSubType == "WPF" && !BlendSdkHelper.IsSdkInstalled(BlendSdkHelper.CurrentWpfVersion)))
			{
				return false;
			}
			if (!this.ValidateTemplateType(vsTemplatePath, vsTemplate, "Project"))
			{
				return false;
			}
			if (!this.ValidateAnyTemplate(vsTemplatePath, vsTemplate))
			{
				return false;
			}
			bool flag1 = false;
			if (vsTemplate.TemplateContent.Items != null)
			{
				object[] items = vsTemplate.TemplateContent.Items;
				int num = 0;
				while (true)
				{
					if (num >= (int)items.Length)
					{
						if (flag1)
						{
							return true;
						}
						currentCulture = CultureInfo.CurrentCulture;
						invalidTemplateElement = StringTable.InvalidTemplateElement;
						objArray = new object[] { "TemplateContent" };
						this.LogTemplateLoadError(vsTemplatePath, string.Format(currentCulture, invalidTemplateElement, objArray));
						return false;
					}
					object obj = items[num];
					VSTemplateTemplateContentProject vSTemplateTemplateContentProject = obj as VSTemplateTemplateContentProject;
					if (vSTemplateTemplateContentProject != null)
					{
						if (vSTemplateTemplateContentProject.File == null)
						{
							CultureInfo cultureInfo = CultureInfo.CurrentCulture;
							string missingTemplateAttribute = StringTable.MissingTemplateAttribute;
							object[] objArray1 = new object[] { "Project", "File" };
							this.LogTemplateLoadError(vsTemplatePath, string.Format(cultureInfo, missingTemplateAttribute, objArray1));
							flag = false;
							break;
						}
						else if (Microsoft.Expression.Framework.Documents.PathHelper.ValidatePath(vSTemplateTemplateContentProject.File) == null)
						{
							flag1 = true;
						}
						else
						{
							CultureInfo currentCulture1 = CultureInfo.CurrentCulture;
							string invalidTemplateValue = StringTable.InvalidTemplateValue;
							object[] file = new object[] { "File", vSTemplateTemplateContentProject.File };
							this.LogTemplateLoadError(vsTemplatePath, string.Format(currentCulture1, invalidTemplateValue, file));
							flag = false;
							break;
						}
					}
					if (obj is VSTemplateTemplateContentProjectItem)
					{
						CultureInfo cultureInfo1 = CultureInfo.CurrentCulture;
						string unsupportedTemplateElement = StringTable.UnsupportedTemplateElement;
						object[] objArray2 = new object[] { "ProjectItem" };
						this.LogTemplateLoadError(vsTemplatePath, string.Format(cultureInfo1, unsupportedTemplateElement, objArray2));
						flag = false;
						break;
					}
					else if (!(obj is VSTemplateTemplateContentReferences))
					{
						num++;
					}
					else
					{
						CultureInfo currentCulture2 = CultureInfo.CurrentCulture;
						string str = StringTable.UnsupportedTemplateElement;
						object[] objArray3 = new object[] { "References" };
						this.LogTemplateLoadError(vsTemplatePath, string.Format(currentCulture2, str, objArray3));
						flag = false;
						break;
					}
				}
				return flag;
			}
			if (flag1)
			{
				return true;
			}
			currentCulture = CultureInfo.CurrentCulture;
			invalidTemplateElement = StringTable.InvalidTemplateElement;
			objArray = new object[] { "TemplateContent" };
			this.LogTemplateLoadError(vsTemplatePath, string.Format(currentCulture, invalidTemplateElement, objArray));
			return false;
		}

		internal bool ValidateProjectTemplate(string vsTemplatePath, Microsoft.Expression.Project.Templates.VSTemplate vsTemplate)
		{
			if (vsTemplatePath == null)
			{
				throw new ArgumentNullException("vsTemplatePath");
			}
			if (vsTemplate == null)
			{
				throw new ArgumentNullException("vsTemplate");
			}
			if (!this.ValidateProjectOrSampleTemplate(vsTemplatePath, vsTemplate))
			{
				return false;
			}
			if (!this.ValidateTemplateNameDescriptionIcon(vsTemplatePath, vsTemplate.TemplateData.Icon, "Icon"))
			{
				return false;
			}
			return true;
		}

		internal bool ValidateSampleTemplate(string vsTemplatePath, Microsoft.Expression.Project.Templates.VSTemplate vsTemplate)
		{
			if (vsTemplatePath == null)
			{
				throw new ArgumentNullException("vsTemplatePath");
			}
			if (vsTemplate == null)
			{
				throw new ArgumentNullException("vsTemplate");
			}
			if (!this.ValidateProjectOrSampleTemplate(vsTemplatePath, vsTemplate))
			{
				return false;
			}
			return true;
		}

		internal bool ValidateSdkItemTemplate(string vsTemplatePath, Microsoft.Expression.Project.Templates.VSTemplate vsTemplate)
		{
			if (!this.ValidateItemTemplate(vsTemplatePath, vsTemplate))
			{
				return false;
			}
			if (TemplateManager.IsPathFromSDK(vsTemplatePath, BlendSdkHelper.Wpf35) || TemplateManager.IsPathFromSDK(vsTemplatePath, BlendSdkHelper.Silverlight3))
			{
				vsTemplate.TemplateData.MaxFrameworkVersionSpecified = true;
				vsTemplate.TemplateData.MaxFrameworkVersion = VSTemplateTemplateDataMaxFrameworkVersion.Item35;
				vsTemplate.TemplateData.RequiredFrameworkVersionSpecified = true;
				vsTemplate.TemplateData.RequiredFrameworkVersion = VSTemplateTemplateDataRequiredFrameworkVersion.Item30;
			}
			return true;
		}

		private bool ValidateTemplateNameDescriptionIcon(string vsTemplatePath, NameDescriptionIcon value, string name)
		{
			if (value == null)
			{
				CultureInfo currentCulture = CultureInfo.CurrentCulture;
				string missingTemplateElement = StringTable.MissingTemplateElement;
				object[] objArray = new object[] { name };
				this.LogTemplateLoadError(vsTemplatePath, string.Format(currentCulture, missingTemplateElement, objArray));
				return false;
			}
			if (!string.IsNullOrEmpty(value.Value))
			{
				return true;
			}
			CultureInfo cultureInfo = CultureInfo.CurrentCulture;
			string invalidTemplateElement = StringTable.InvalidTemplateElement;
			object[] objArray1 = new object[] { name };
			this.LogTemplateLoadError(vsTemplatePath, string.Format(cultureInfo, invalidTemplateElement, objArray1));
			return false;
		}

		private bool ValidateTemplateType(string vsTemplatePath, Microsoft.Expression.Project.Templates.VSTemplate vsTemplate, string validType)
		{
			if (vsTemplate.Type == null)
			{
				CultureInfo currentCulture = CultureInfo.CurrentCulture;
				string missingTemplateAttribute = StringTable.MissingTemplateAttribute;
				object[] objArray = new object[] { "VSTemplate", "Type" };
				this.LogTemplateLoadError(vsTemplatePath, string.Format(currentCulture, missingTemplateAttribute, objArray));
				return false;
			}
			if (vsTemplate.Type.Equals(validType, StringComparison.Ordinal))
			{
				return true;
			}
			CultureInfo cultureInfo = CultureInfo.CurrentCulture;
			string invalidTemplateValue = StringTable.InvalidTemplateValue;
			object[] type = new object[] { "Type", vsTemplate.Type };
			this.LogTemplateLoadError(vsTemplatePath, string.Format(cultureInfo, invalidTemplateValue, type));
			return false;
		}

		internal delegate bool TemplateValidator(string vsTemplatePath, Microsoft.Expression.Project.Templates.VSTemplate vsTemplate);
	}
}