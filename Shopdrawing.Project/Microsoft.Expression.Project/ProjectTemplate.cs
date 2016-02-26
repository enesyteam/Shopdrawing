using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project.Build;
using Microsoft.Expression.Project.ServiceExtensions;
using Microsoft.Expression.Project.Templates;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project
{
	public class ProjectTemplate : TemplateBase, IProjectTemplate, ITemplate
	{
		public bool CreateNewFolder
		{
			get
			{
				if (!base.Template.TemplateData.CreateNewFolderSpecified)
				{
					return false;
				}
				return base.Template.TemplateData.CreateNewFolder;
			}
		}

		public override string DefaultName
		{
			get
			{
				string defaultName = base.DefaultName;
				if (string.IsNullOrEmpty(defaultName))
				{
					return StringTable.ProjectDefaultName;
				}
				return defaultName;
			}
		}

		public bool HasMultipleProjects
		{
			get
			{
				return this.TemplateProjects.Count<VSTemplateTemplateContentProject>() > 1;
			}
		}

		public bool HasProjectFile
		{
			get
			{
				return this.TemplateProjects.Any<VSTemplateTemplateContentProject>((VSTemplateTemplateContentProject project) => {
					if (string.IsNullOrEmpty(project.File))
					{
						return false;
					}
					return base.FileExists(project.File);
				});
			}
		}

		public bool IsUserCreatable
		{
			get
			{
				return true;
			}
		}

		public string ProjectFilename
		{
			get
			{
				return this.TemplateProjects.Where<VSTemplateTemplateContentProject>((VSTemplateTemplateContentProject project) => {
					if (string.IsNullOrEmpty(project.File))
					{
						return false;
					}
					return base.FileExists(project.File);
				}).Select<VSTemplateTemplateContentProject, string>((VSTemplateTemplateContentProject project) => project.File).FirstOrDefault<string>();
			}
		}

		private IEnumerable<VSTemplateTemplateContentProject> TemplateProjects
		{
			get
			{
				return base.Template.TemplateContent.Items.OfType<VSTemplateTemplateContentProject>();
			}
		}

		public ProjectTemplate(VSTemplate projectTemplate, Uri templateLocation) : base(projectTemplate, templateLocation)
		{
			if (string.Compare(projectTemplate.Type, "Project", StringComparison.OrdinalIgnoreCase) != 0)
			{
				throw new ArgumentException("Not a valid project template.", "projectTemplate");
			}
		}

		public IEnumerable<INamedProject> CreateProjects(string name, string path, IEnumerable<TemplateArgument> templateArguments, IServiceProvider serviceProvider)
		{
			Uri uri;
			IEnumerable<INamedProject> namedProjects;
			ICodeDocumentType codeDocumentType = base.GetCodeDocumentType(serviceProvider);
			if (templateArguments != null)
			{
				templateArguments = templateArguments.Concat<TemplateArgument>(TemplateManager.GetDefaultArguments(serviceProvider.ExpressionInformationService()));
			}
			else
			{
				templateArguments = TemplateManager.GetDefaultArguments(serviceProvider.ExpressionInformationService());
			}
			TemplateArgument templateArgument = new TemplateArgument("projectname", name);
			if (!base.Template.TemplateData.CreateNewFolderSpecified || base.Template.TemplateData.CreateNewFolder)
			{
				path = Microsoft.Expression.Framework.Documents.PathHelper.ResolveCombinedPath(path, name);
				Directory.CreateDirectory(path);
			}
			List<INamedProject> namedProjects1 = new List<INamedProject>();
			Uri uri1 = null;
			try
			{
				uri1 = new Uri(Microsoft.Expression.Framework.Documents.PathHelper.EnsurePathEndsInDirectorySeparator(path));
			}
			catch (UriFormatException uriFormatException)
			{
				namedProjects = namedProjects1;
				return namedProjects;
			}
			List<Uri> uris = new List<Uri>();
			List<ProjectTemplate.FileToOpen> fileToOpens = new List<ProjectTemplate.FileToOpen>();
			List<ProjectTemplate.SourceDestination> sourceDestinations = new List<ProjectTemplate.SourceDestination>();
			string str = CodeGenerator.MakeSafeIdentifier(codeDocumentType, name, false);
			string str1 = CodeGenerator.MakeSafeIdentifier(codeDocumentType, name, true);
			TemplateArgument templateArgument1 = new TemplateArgument("safeprojectname", str);
			TemplateArgument templateArgument2 = new TemplateArgument("safeprojectname", str1);
			TemplateArgument templateArgument3 = new TemplateArgument("assemblyname", name);
			TemplateArgument templateArgument4 = new TemplateArgument("safeassemblyname", name.Replace(' ', '\u005F'));
			using (ProjectPathHelper.TemporaryDirectory temporaryDirectory = new ProjectPathHelper.TemporaryDirectory(true))
			{
				foreach (VSTemplateTemplateContentProject templateProject in this.TemplateProjects)
				{
					if (!string.IsNullOrEmpty(templateProject.File))
					{
						string targetFileName = templateProject.TargetFileName;
						if (string.IsNullOrEmpty(targetFileName))
						{
							targetFileName = string.Concat(name, Path.GetExtension(templateProject.File));
						}
						TemplateArgument[] templateArgumentArray = new TemplateArgument[] { templateArgument, templateArgument1, templateArgument3, templateArgument4 };
						IEnumerable<TemplateArgument> templateArguments1 = templateArguments.Concat<TemplateArgument>(templateArgumentArray);
						uri = base.ResolveFileUri(targetFileName, uri1);
						string str2 = TemplateParser.ReplaceTemplateArguments(uri.LocalPath, templateArguments1);
						uri = new Uri(str2);
						if (!string.IsNullOrEmpty(templateProject.File) && !base.IsDirectory(templateProject.File))
						{
							Uri uri2 = new Uri(temporaryDirectory.GenerateTemporaryFileName());
							base.CreateFile(templateProject.File, base.TemplateLocation, uri2, templateProject.ReplaceParameters, templateArguments1);
							ProjectTemplate.SourceDestination sourceDestination = new ProjectTemplate.SourceDestination()
							{
								Source = uri2,
								Destination = uri
							};
							sourceDestinations.Add(sourceDestination);
						}
					}
					else
					{
						uri = uri1;
					}
					foreach (Microsoft.Expression.Project.Templates.ProjectItem projectItem in templateProject.Items.OfType<Microsoft.Expression.Project.Templates.ProjectItem>())
					{
						string value = projectItem.TargetFileName;
						if (string.IsNullOrEmpty(value))
						{
							value = projectItem.Value;
						}
						int num = projectItem.Value.IndexOf('.');
						string str3 = (num != -1 ? projectItem.Value.Remove(num) : projectItem.Value);
						bool flag = Path.GetExtension(projectItem.Value).Equals(codeDocumentType.DefaultFileExtension, StringComparison.OrdinalIgnoreCase);
						string str4 = CodeGenerator.MakeSafeIdentifier(codeDocumentType, str3, flag);
						TemplateArgument[] templateArgumentArray1 = new TemplateArgument[] { templateArgument, templateArgument3, templateArgument4, null, null, null };
						templateArgumentArray1[3] = (flag ? templateArgument2 : templateArgument1);
						templateArgumentArray1[4] = new TemplateArgument("safeitemname", str4);
						templateArgumentArray1[5] = new TemplateArgument("safeitemrootname", str4);
						TemplateArgument[] templateArgumentArray2 = templateArgumentArray1;
						string str5 = TemplateParser.ReplaceTemplateArguments(value, templateArguments.Concat<TemplateArgument>(templateArgumentArray2));
						Uri uri3 = base.ResolveFileUri(str5, uri1);
						Uri uri4 = new Uri(temporaryDirectory.GenerateTemporaryFileName());
						if (!base.CreateFile(projectItem.Value, base.TemplateLocation, uri4, projectItem.ReplaceParameters, templateArguments.Concat<TemplateArgument>(templateArgumentArray2)))
						{
							continue;
						}
						if (projectItem.OpenInEditorSpecified && projectItem.OpenInEditor)
						{
							ProjectTemplate.FileToOpen fileToOpen = new ProjectTemplate.FileToOpen()
							{
								ProjectFile = uri,
								ProjectItem = uri3
							};
							fileToOpens.Add(fileToOpen);
						}
						ProjectTemplate.SourceDestination sourceDestination1 = new ProjectTemplate.SourceDestination()
						{
							Source = uri4,
							Destination = uri3
						};
						sourceDestinations.Add(sourceDestination1);
					}
					uris.Add(uri);
				}
				foreach (ProjectTemplate.SourceDestination sourceDestination2 in sourceDestinations)
				{
					string localPath = sourceDestination2.Destination.LocalPath;
					if (!Microsoft.Expression.Framework.Documents.PathHelper.FileOrDirectoryExists(localPath))
					{
						continue;
					}
					IMessageDisplayService messageDisplayService = serviceProvider.MessageDisplayService();
					CultureInfo currentUICulture = CultureInfo.CurrentUICulture;
					string cannotCreateTemplateDirectoryOrFileExistsMessage = StringTable.CannotCreateTemplateDirectoryOrFileExistsMessage;
					object[] objArray = new object[] { localPath };
					messageDisplayService.ShowError(string.Format(currentUICulture, cannotCreateTemplateDirectoryOrFileExistsMessage, objArray));
					namedProjects = namedProjects1;
					return namedProjects;
				}
				foreach (ProjectTemplate.SourceDestination sourceDestination3 in sourceDestinations)
				{
					base.CreateFile(sourceDestination3.Source.LocalPath, new Uri(Path.GetDirectoryName(sourceDestination3.Source.LocalPath)), sourceDestination3.Destination, false, Enumerable.Empty<TemplateArgument>());
				}
				foreach (Uri uri5 in uris)
				{
					DocumentReference documentReference = DocumentReference.Create(uri5.LocalPath);
					IProjectStore projectStore = null;
					INamedProject namedProject = null;
					try
					{
						projectStore = ProjectStoreHelper.CreateProjectStore(documentReference, serviceProvider, ProjectStoreHelper.DefaultProjectCreationChain);
						namedProject = serviceProvider.ProjectManager().AddProject(projectStore);
					}
					finally
					{
						if (namedProject == null && projectStore != null)
						{
							projectStore.Dispose();
						}
					}
					if (namedProject == null)
					{
						continue;
					}
					namedProjects1.Add(namedProject);
				}
				foreach (ProjectTemplate.FileToOpen fileToOpen1 in fileToOpens)
				{
					ISolution currentSolution = serviceProvider.ProjectManager().CurrentSolution;
					if (currentSolution == null)
					{
						continue;
					}
					IProject project = currentSolution.Projects.FindMatchByUrl<IProject>(fileToOpen1.ProjectFile.LocalPath);
					if (project == null)
					{
						continue;
					}
					IProjectItem projectItem1 = project.Items.FindMatchByUrl<IProjectItem>(fileToOpen1.ProjectItem.LocalPath);
					if (projectItem1 == null)
					{
						continue;
					}
					projectItem1.OpenView(true);
				}
				return namedProjects1;
			}
			return namedProjects;
		}

		public virtual bool IsPlatformSupported(out string warningMessage)
		{
			warningMessage = null;
			return true;
		}

		public ProjectPropertyValue PreferredPropertyValue(string property)
		{
			if (property != "TargetFrameworkVersion")
			{
				return null;
			}
			return this.ValidPropertyValues(property).LastOrDefault<ProjectPropertyValue>();
		}

		public List<ProjectPropertyValue> ValidPropertyValues(string property)
		{
			List<ProjectPropertyValue> projectPropertyValues = new List<ProjectPropertyValue>();
			if (property == "TargetFrameworkVersion")
			{
				string projectSubType = base.ProjectSubType;
				if (projectSubType == null || !(projectSubType == "Silverlight"))
				{
					if (base.MinimumFrameworkVersion != "4.0")
					{
						projectPropertyValues.Add(ProjectPropertyInfo.CreatePropertyValue(property, "3.5"));
					}
					if (base.MaximumFrameworkVersion == "4.0" || projectPropertyValues.Count == 0)
					{
						projectPropertyValues.Add(ProjectPropertyInfo.CreatePropertyValue(property, "4.0"));
					}
				}
				else
				{
					if (base.MinimumFrameworkVersion != "4.0" && base.MaximumFrameworkVersion != "2.0")
					{
						projectPropertyValues.Add(ProjectPropertyInfo.CreatePropertyValue(property, "3.0"));
					}
					if (base.MaximumFrameworkVersion == "4.0" || projectPropertyValues.Count == 0)
					{
						projectPropertyValues.Add(ProjectPropertyInfo.CreatePropertyValue(property, "4.0"));
					}
				}
			}
			return projectPropertyValues;
		}

		private struct FileToOpen
		{
			public Uri ProjectFile;

			public Uri ProjectItem;
		}

		private struct SourceDestination
		{
			public Uri Source;

			public Uri Destination;
		}
	}
}