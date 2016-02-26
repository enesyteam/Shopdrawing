using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.Build;
using Microsoft.Expression.Project.ServiceExtensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project.Templates
{
	public class ProjectItemTemplate : TemplateBase, IProjectItemTemplate, ITemplate
	{
		public override string DefaultName
		{
			get
			{
				string defaultName = base.DefaultName;
				if (string.IsNullOrEmpty(defaultName))
				{
					return StringTable.ItemDefaultName;
				}
				return defaultName;
			}
		}

		public bool IsUserCreatable
		{
			get
			{
				return true;
			}
		}

		private IEnumerable<VSTemplateTemplateContentProjectItem> TemplateProjectItems
		{
			get
			{
				return base.Template.TemplateContent.Items.OfType<VSTemplateTemplateContentProjectItem>();
			}
		}

		private IEnumerable<VSTemplateTemplateContentReferencesReference> TemplateReferences
		{
			get
			{
				return base.Template.TemplateContent.Items.OfType<VSTemplateTemplateContentReferences>().SelectMany<VSTemplateTemplateContentReferences, VSTemplateTemplateContentReferencesReference>((VSTemplateTemplateContentReferences item) => item.References);
			}
		}

		public ProjectItemTemplate(VSTemplate projectItemTemplate, Uri templateLocation) : base(projectItemTemplate, templateLocation)
		{
			if (string.Compare(projectItemTemplate.Type, "Item", StringComparison.OrdinalIgnoreCase) != 0)
			{
				throw new ArgumentException("Not a valid item template.", "projectItemTemplate");
			}
		}

		private IEnumerable<IProjectItem> AddAssemblies(IProject project)
		{
			List<IProjectItem> projectItems = new List<IProjectItem>();
			foreach (VSTemplateTemplateContentReferencesReference templateReference in this.TemplateReferences)
			{
				IProjectItem projectItem = project.AddAssemblyReference(templateReference.Assembly.Trim(), false);
				if (projectItem == null)
				{
					continue;
				}
				projectItems.Add(projectItem);
			}
			return projectItems;
		}

		private static string AdjustTargetFolder(string targetFolder, string rootSourcePath, string fullSourcePath)
		{
			string directoryNameOrRoot = Microsoft.Expression.Framework.Documents.PathHelper.GetDirectoryNameOrRoot(fullSourcePath);
			DocumentReference documentReference = DocumentReference.Create(rootSourcePath);
			DocumentReference documentReference1 = DocumentReference.Create(directoryNameOrRoot);
			return Microsoft.Expression.Framework.Documents.PathHelper.ResolveCombinedPath(targetFolder, documentReference.GetRelativePath(documentReference1));
		}

		public IEnumerable<IProjectItem> CreateProjectItems(string name, string targetFolder, IProject project, IEnumerable<TemplateArgument> templateArguments, CreationOptions creationOptions, out List<IProjectItem> itemsToOpen, IServiceProvider serviceProvider)
		{
			Uri uri;
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			ICodeDocumentType codeDocumentType = base.GetCodeDocumentType(serviceProvider);
			if (templateArguments != null)
			{
				templateArguments = templateArguments.Concat<TemplateArgument>(TemplateManager.GetDefaultArguments(serviceProvider.ExpressionInformationService()));
			}
			else
			{
				templateArguments = TemplateManager.GetDefaultArguments(serviceProvider.ExpressionInformationService());
			}
			IEnumerable<IProjectItem> projectItems = Enumerable.Empty<IProjectItem>();
			itemsToOpen = new List<IProjectItem>();
			using (ProjectPathHelper.TemporaryDirectory temporaryDirectory = new ProjectPathHelper.TemporaryDirectory(true))
			{
				Uri uri1 = new Uri(Microsoft.Expression.Framework.Documents.PathHelper.EnsurePathEndsInDirectorySeparator(temporaryDirectory.Path));
				List<DocumentCreationInfo> documentCreationInfos = new List<DocumentCreationInfo>();
				List<string> strs = new List<string>();
				bool flag = false;
				foreach (VSTemplateTemplateContentProjectItem templateProjectItem in this.TemplateProjectItems)
				{
					if (!templateProjectItem.OpenInEditorSpecified)
					{
						continue;
					}
					flag = true;
					break;
				}
				bool flag1 = false;
				string str = CodeGenerator.MakeSafeIdentifier(codeDocumentType, project.DocumentReference.DisplayNameShort, flag1);
				bool flag2 = true;
				string str1 = CodeGenerator.MakeSafeIdentifier(codeDocumentType, project.DocumentReference.DisplayNameShort, flag2);
				TemplateArgument templateArgument = new TemplateArgument("safeprojectname", str);
				TemplateArgument templateArgument1 = new TemplateArgument("safeprojectname", str1);
				TemplateArgument templateArgument2 = new TemplateArgument("assemblyname", project.DocumentReference.DisplayNameShort);
				TemplateArgument templateArgument3 = new TemplateArgument("safeassemblyname", project.DocumentReference.DisplayNameShort.Replace(' ', '\u005F'));
				foreach (VSTemplateTemplateContentProjectItem vSTemplateTemplateContentProjectItem in this.TemplateProjectItems)
				{
					string targetFileName = vSTemplateTemplateContentProjectItem.TargetFileName;
					if (string.IsNullOrEmpty(targetFileName))
					{
						targetFileName = vSTemplateTemplateContentProjectItem.Value;
					}
					TemplateArgument[] templateArgumentArray = new TemplateArgument[] { new TemplateArgument("fileinputname", Path.GetFileNameWithoutExtension(name)), new TemplateArgument("fileinputextension", Path.GetExtension(name)) };
					IEnumerable<TemplateArgument> templateArguments1 = templateArgumentArray;
					targetFileName = TemplateParser.ReplaceTemplateArguments(targetFileName, templateArguments1);
					templateArguments1 = templateArguments1.Concat<TemplateArgument>(templateArguments);
					bool flag3 = Path.GetExtension(vSTemplateTemplateContentProjectItem.Value).Equals(codeDocumentType.DefaultFileExtension, StringComparison.OrdinalIgnoreCase);
					string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(Microsoft.Expression.Framework.Documents.PathHelper.GetFileOrDirectoryName(targetFileName));
					if (serviceProvider.DocumentTypeManager().DocumentTypes[DocumentTypeNamesHelper.Xaml].IsDocumentTypeOf(fileNameWithoutExtension))
					{
						fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileNameWithoutExtension);
					}
					string str2 = CodeGenerator.MakeSafeIdentifier(codeDocumentType, fileNameWithoutExtension, flag3);
					IEnumerable<TemplateArgument> templateArguments2 = templateArguments1;
					TemplateArgument[] templateArgumentArray1 = new TemplateArgument[] { new TemplateArgument("rootnamespace", project.DefaultNamespaceName), new TemplateArgument("projectname", project.DocumentReference.DisplayNameShort), templateArgument2, templateArgument3, null, null, null, null };
					templateArgumentArray1[4] = (flag3 ? templateArgument1 : templateArgument);
					templateArgumentArray1[5] = new TemplateArgument("safeitemname", str2);
					templateArgumentArray1[6] = new TemplateArgument("safeitemrootname", str2);
					templateArgumentArray1[7] = new TemplateArgument("culture", (string.IsNullOrEmpty(project.UICulture) ? CultureInfo.CurrentUICulture.ToString() : project.UICulture));
					templateArguments1 = templateArguments2.Concat<TemplateArgument>(templateArgumentArray1);
					try
					{
						uri = base.ResolveFileUri(targetFileName, uri1);
					}
					catch (UriFormatException uriFormatException)
					{
						continue;
					}
					IDocumentType documentType = project.GetDocumentType(targetFileName);
					foreach (IDocumentType documentType1 in serviceProvider.DocumentTypeManager().DocumentTypes)
					{
						if (vSTemplateTemplateContentProjectItem.SubType != documentType1.Name)
						{
							continue;
						}
						documentType = documentType1;
					}
					if (!base.CreateFile(vSTemplateTemplateContentProjectItem.Value, base.TemplateLocation, uri, vSTemplateTemplateContentProjectItem.ReplaceParameters, templateArguments1))
					{
						continue;
					}
					string str3 = ProjectItemTemplate.AdjustTargetFolder(targetFolder, uri1.LocalPath, uri.LocalPath);
					DocumentCreationInfo documentCreationInfo = new DocumentCreationInfo()
					{
						SourcePath = uri.LocalPath,
						TargetFolder = str3,
						DocumentType = documentType,
						CreationOptions = creationOptions
					};
					documentCreationInfos.Add(documentCreationInfo);
					if (!flag)
					{
						if (strs.Count > 0)
						{
							continue;
						}
						strs.Add(Path.Combine(str3, Path.GetFileName(uri.LocalPath)));
					}
					else
					{
						if (!vSTemplateTemplateContentProjectItem.OpenInEditor)
						{
							continue;
						}
						strs.Add(Path.Combine(str3, Path.GetFileName(uri.LocalPath)));
					}
				}
				if (documentCreationInfos.Count > 0)
				{
					projectItems = project.AddItems(documentCreationInfos);
				}
				if (projectItems.Any<IProjectItem>())
				{
					for (int i = 0; i < strs.Count; i++)
					{
						IProjectItem projectItem = projectItems.FirstOrDefault<IProjectItem>((IProjectItem item) => item.DocumentReference.Path.Equals(strs[i], StringComparison.OrdinalIgnoreCase));
						if (projectItem != null)
						{
							itemsToOpen.Add(projectItem);
						}
					}
					projectItems = projectItems.Concat<IProjectItem>(this.AddAssemblies(project));
				}
			}
			if (base.BuildOnLoad)
			{
				IProjectManager projectManager = serviceProvider.ProjectManager();
				IProjectBuildContext activeBuildTarget = projectManager.ActiveBuildTarget;
				projectManager.BuildManager.Build(activeBuildTarget, null, true);
				KnownProjectBase knownProjectBase = project as KnownProjectBase;
				if (knownProjectBase != null)
				{
					knownProjectBase.CheckForChangedOrDeletedItems();
				}
			}
			return projectItems;
		}

		public string FindAvailableDefaultName(string targetFolder, IExpressionInformationService expressionInformationService)
		{
			string str = string.Concat(Path.GetFileNameWithoutExtension(this.DefaultName), "{0}");
			string safeExtension = Microsoft.Expression.Framework.Documents.PathHelper.GetSafeExtension(this.DefaultName);
			targetFolder = Microsoft.Expression.Framework.Documents.PathHelper.EnsurePathEndsInDirectorySeparator(targetFolder);
			IEnumerable<TemplateArgument> defaultArguments = TemplateManager.GetDefaultArguments(expressionInformationService);
			TemplateArgument[] templateArgument = new TemplateArgument[] { new TemplateArgument("fileinputname", str), new TemplateArgument("fileinputextension", safeExtension) };
			defaultArguments = defaultArguments.Concat<TemplateArgument>(templateArgument);
			List<string> strs = new List<string>();
			foreach (VSTemplateTemplateContentProjectItem templateProjectItem in this.TemplateProjectItems)
			{
				if (string.IsNullOrEmpty(templateProjectItem.TargetFileName) || !templateProjectItem.TargetFileName.Contains("$fileinputname$"))
				{
					continue;
				}
				strs.Add(string.Concat(targetFolder, TemplateParser.ReplaceTemplateArguments(templateProjectItem.TargetFileName, defaultArguments)));
			}
			if (strs.Count == 0)
			{
				return this.DefaultName;
			}
			string str1 = Microsoft.Expression.Framework.Documents.PathHelper.FindAvailablePaths(strs, true);
			CultureInfo invariantCulture = CultureInfo.InvariantCulture;
			object[] objArray = new object[] { str1 };
			return string.Concat(string.Format(invariantCulture, str, objArray), safeExtension);
		}

		public bool IsPlatformSupported(out string warningMessage)
		{
			warningMessage = null;
			return true;
		}
	}
}