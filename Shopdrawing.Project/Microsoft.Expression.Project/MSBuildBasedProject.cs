using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Exceptions;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Collections;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.Extensions.Enumerable;
using Microsoft.Expression.Framework.Interop;
using Microsoft.Expression.Project.Build;
using Microsoft.Expression.Project.ServiceExtensions;
using Microsoft.Expression.Project.ServiceExtensions.Documents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Text;
using System.Threading;

namespace Microsoft.Expression.Project
{
	public abstract class MSBuildBasedProject : KnownProjectBase, IMSBuildBasedProject
	{
		private const string ShouldAlreadyKnow = "SAK";

		private const string SccProjectName = "SccProjectName";

		private const string SccLocalPath = "SccLocalPath";

		private const string SccAuxPath = "SccAuxPath";

		private const string SccProvider = "SccProvider";

		private const string WindowsCEPropertyName = "WindowsCE";

		private const string SilverlightMobilePropertyName = "SilverlightMobile";

		private const string WindowsCESimplePlatformName = "WindowsCE";

		private const string MSBuildAssemblySearchPaths = "{{TargetFrameworkDirectory}};{{Registry:{0},{1},{2}{3}}};{{AssemblyFolders}};{{GAC}};";

		private const string MSBuildFrameworkRegistryBase = "FrameworkRegistryBase";

		private const string MSBuildTargetFrameworkVersionProperty = "TargetFrameworkVersion";

		private const string MSBuildAssemblyFoldersSuffixProperty = "AssemblyFoldersSuffix";

		private const string MSBuildAssemblyFoldersExConditionsProperty = "AssemblyFoldersExConditions";

		private const string MSBuildAssemblySearchPathsProperty = "AssemblySearchPaths";

		internal readonly static string BuildingInBlendPropertyName;

		internal readonly static string[] ResourceFolderWildcards;

		private bool referencePathsNeedRebuilding = true;

		private bool disableRebuildingReferencePath;

		private bool hasWildcardBasedItem;

		private bool currentlyRefreshing;

		private ICodeDocumentType codeDocumentType;

		private List<string> projectLoadErrors = new List<string>();

		private object msbuildProjectSyncLock = new object();

		private ProjectInstance msBuildProjectInstance;

		private IProjectType projectType;

		private FrameworkName targetFramework;

		private Dictionary<IProjectItem, int> danglingChildren = new Dictionary<IProjectItem, int>();

		private object implicitResolveSyncLock = new object();

		private bool isAlreadyImplicitlyResolving;

		private bool? isWindowsCEProject = null;

		public override ICodeDocumentType CodeDocumentType
		{
			get
			{
				return this.codeDocumentType;
			}
		}

		public override string DefaultNamespaceName
		{
			get
			{
				string evaluatedPropertyValue = this.GetEvaluatedPropertyValue("RootNamespace");
				if (!string.IsNullOrEmpty(evaluatedPropertyValue))
				{
					return evaluatedPropertyValue;
				}
				return CodeGenerator.GetApplicationNamespaceName(this.CodeDocumentType, this);
			}
		}

		protected virtual bool DefinesOwnMscorlib
		{
			get
			{
				return false;
			}
		}

		public string FullTargetFileName
		{
			get
			{
				string property = this.GetProperty("TargetPath");
				if (!Microsoft.Expression.Framework.Documents.PathHelper.IsValidPath(property))
				{
					return null;
				}
				property = Microsoft.Expression.Framework.Documents.PathHelper.ResolveRelativePath(Microsoft.Expression.Framework.Documents.PathHelper.GetDirectoryNameOrRoot(base.DocumentReference.Path), property);
				return property;
			}
		}

		public IList<string> GlobalImportsList
		{
			get;
			private set;
		}

		public override bool IsReadOnly
		{
			get
			{
				return (File.GetAttributes(base.DocumentReference.Path) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly;
			}
		}

		internal Microsoft.Expression.Project.ProjectBuildContext ProjectBuildContext
		{
			get;
			private set;
		}

		internal Guid ProjectFileProjectGuid
		{
			get
			{
				Guid guid;
				string evaluatedPropertyValue = this.GetEvaluatedPropertyValue("ProjectGuid");
				if (evaluatedPropertyValue != null)
				{
					try
					{
						guid = new Guid(evaluatedPropertyValue.ToString());
					}
					catch (FormatException formatException)
					{
						return Guid.Empty;
					}
					return guid;
				}
				return Guid.Empty;
			}
		}

		public override IProjectType ProjectType
		{
			get
			{
				return this.projectType;
			}
		}

		public string ProjectXml
		{
			get
			{
				StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
				((MSBuildBasedProjectStore)base.ProjectStore).MsBuildProject.Save(stringWriter);
				return stringWriter.ToString();
			}
		}

		public override string PropertiesPath
		{
			get
			{
				string evaluatedPropertyValue = this.GetEvaluatedPropertyValue("AppDesignerFolder");
				if (!string.IsNullOrEmpty(evaluatedPropertyValue))
				{
					return evaluatedPropertyValue;
				}
				return base.PropertiesPath;
			}
		}

		protected override IDocumentType StartupDocumentType
		{
			get
			{
				return base.Services.DocumentTypes()[DocumentTypeNamesHelper.Xaml];
			}
		}

		public override IProjectItem StartupItem
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public override FrameworkName TargetFramework
		{
			get
			{
				if (this.targetFramework == null)
				{
					this.targetFramework = ProjectStoreHelper.GetTargetFrameworkName(base.ProjectStore);
				}
				return this.targetFramework;
			}
		}

		public override string TemplateProjectType
		{
			get
			{
				ICodeDocumentType codeDocumentType = this.CodeDocumentType;
				if (codeDocumentType == null)
				{
					return null;
				}
				if (codeDocumentType.Name == base.Services.DocumentTypes().CSharpDocumentType().Name)
				{
					return "Visual C#";
				}
				if (codeDocumentType.Name == base.Services.DocumentTypes().VisualBasicDocumentType().Name)
				{
					return "Visual Basic";
				}
				return null;
			}
		}

		public override string UICulture
		{
			get
			{
				return base.ProjectStore.GetProperty("UICulture");
			}
			set
			{
				string str = value ?? string.Empty;
				base.ProjectStore.SetProperty("UICulture", str);
			}
		}

		static MSBuildBasedProject()
		{
			MSBuildBasedProject.BuildingInBlendPropertyName = "BuildingInsideExpressionBlend";
			string[] strArrays = new string[] { "*.jpg", "*.jpeg", "*.png", "*.bmp", "*.gif", "*.tif", "*.tiff" };
			MSBuildBasedProject.ResourceFolderWildcards = strArrays;
		}

		protected MSBuildBasedProject(IProjectStore projectStore, ICodeDocumentType codeDocumentType, IProjectType projectType, IServiceProvider serviceProvider) : base(projectStore, serviceProvider)
		{
			if (projectStore == null)
			{
				throw new ArgumentNullException("projectStore");
			}
			if (!(projectStore is MSBuildBasedProjectStore))
			{
				throw new ArgumentException("IProjectStore must be MSBuildBasedProjectStore", "projectStore");
			}
			this.codeDocumentType = codeDocumentType;
			this.projectType = projectType;
		}

		public override IProjectItem AddAssemblyReference(string path, bool verifyFileExists)
		{
			IProjectItem projectItem;
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException("path");
			}
			if (!Microsoft.Expression.Framework.Documents.PathHelper.IsValidPath(path))
			{
				return null;
			}
			string str = ProjectAssemblyHelper.TrimFiletypeFromAssemblyNameOrPath(path);
			ProjectAssembly projectAssembly = base.ReferencedAssemblies.Find(str);
			if (projectAssembly != null && projectAssembly.ProjectItem != null)
			{
				return null;
			}
			if (!base.AttemptToMakeProjectWritable())
			{
				return null;
			}
			AssemblyName assemblyName = ProjectAssemblyHelper.CachedGetAssemblyNameFromPath(path);
			string resolvedAssemblyReference = null;
			IProjectItemData projectItemDatum = null;
			try
			{
				if (string.IsNullOrEmpty(resolvedAssemblyReference))
				{
					projectItemDatum = (assemblyName == null || ProjectAssemblyHelper.CanAssemblyBeFoundInReferenceFolders(assemblyName, path, this.TargetFramework) ? base.ProjectStore.AddItem("Reference", str) : base.ProjectStore.AddItem("Reference", assemblyName.FullName));
					if (projectItemDatum != null)
					{
						this.referencePathsNeedRebuilding = true;
						resolvedAssemblyReference = this.GetResolvedAssemblyReference(str);
						if (string.IsNullOrEmpty(resolvedAssemblyReference) || this.IsInOutputDirectory(resolvedAssemblyReference))
						{
							string relativePath = base.DocumentReference.GetRelativePath(Microsoft.Expression.Framework.Documents.DocumentReference.Create(path));
							if (!Path.GetFileName(path).Equals(relativePath, StringComparison.OrdinalIgnoreCase))
							{
								projectItemDatum.SetItemMetadata("HintPath", relativePath);
								this.referencePathsNeedRebuilding = true;
								resolvedAssemblyReference = this.GetResolvedAssemblyReference(path);
							}
							if (string.IsNullOrEmpty(resolvedAssemblyReference))
							{
								resolvedAssemblyReference = path;
							}
						}
					}
					else
					{
						projectItem = null;
						return projectItem;
					}
				}
				if (this.IsValidAssemblyReference(resolvedAssemblyReference, verifyFileExists) || projectItemDatum == null)
				{
					IProjectItem projectItem1 = base.Services.DocumentTypes()[DocumentTypeNamesHelper.Assembly].CreateProjectItem(this, Microsoft.Expression.Framework.Documents.DocumentReference.CreateFromRelativePath(base.ProjectRoot.Path, resolvedAssemblyReference), base.Services);
					((IMSBuildItemInternal)projectItem1).BuildItem = ((MSBuildProjectItemData)projectItemDatum).ProjectItem;
					base.AddProjectItem(projectItem1);
					base.ImplicitSave();
					base.ProjectAssemblyReferencesDirty = true;
					projectItem = projectItem1;
				}
				else
				{
					base.ProjectStore.RemoveItem(projectItemDatum);
					projectItem = null;
				}
			}
			catch
			{
				if (projectItemDatum == null)
				{
					throw;
				}
				else
				{
					base.ProjectStore.RemoveItem(projectItemDatum);
					projectItem = null;
				}
			}
			return projectItem;
		}

		protected override IEnumerable<IProjectItem> AddAssetFolder(string sourceFolder, string targetFolder, bool link)
		{
			sourceFolder = Microsoft.Expression.Framework.Documents.PathHelper.EnsurePathEndsInDirectorySeparator(sourceFolder);
			targetFolder = Microsoft.Expression.Framework.Documents.PathHelper.EnsurePathEndsInDirectorySeparator(targetFolder);
			IEnumerable<IProjectItem> array = Enumerable.Empty<IProjectItem>();
			IProjectItem projectItem = base.Items.FindMatchByUrl<IProjectItem>(targetFolder);
			if (projectItem != null)
			{
				array = projectItem.Descendants.OfType<IProjectItem>().ToArray<IProjectItem>();
			}
			StringBuilder stringBuilder = new StringBuilder(512);
			if (!link && !sourceFolder.Equals(targetFolder, StringComparison.OrdinalIgnoreCase))
			{
				ProjectManager.CopyDirectory(sourceFolder, targetFolder, true);
			}
			string[] resourceFolderWildcards = MSBuildBasedProject.ResourceFolderWildcards;
			for (int i = 0; i < (int)resourceFolderWildcards.Length; i++)
			{
				string str = resourceFolderWildcards[i];
				string[] files = Directory.GetFiles(sourceFolder, str, SearchOption.AllDirectories);
				for (int j = 0; j < (int)files.Length; j++)
				{
					Microsoft.Expression.Framework.Documents.DocumentReference documentReference = Microsoft.Expression.Framework.Documents.DocumentReference.Create(files[j]);
					IProjectItem item = base.Items[documentReference.GetHashCode()];
					if (item != null)
					{
						IProjectItem[] projectItemArray = new IProjectItem[] { item };
						base.RemoveItems(false, projectItemArray);
					}
				}
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(';');
				}
				stringBuilder.Append(base.DocumentReference.GetRelativePath(Microsoft.Expression.Framework.Documents.DocumentReference.Create((link ? sourceFolder : targetFolder))));
				stringBuilder.Append("**\\");
				stringBuilder.Append(str);
			}
			string str1 = stringBuilder.ToString();
			ProjectItemElement projectItemElement = ((MSBuildBasedProjectStore)base.ProjectStore).MsBuildProject.Xml.AddItem("Resource", str1);
			if (link)
			{
				projectItemElement.AddMetadata("Link", string.Concat(base.DocumentReference.GetRelativePath(Microsoft.Expression.Framework.Documents.DocumentReference.Create(targetFolder)), Path.DirectorySeparatorChar));
			}
			this.ReevaluateIfNecessary();
			base.ImplicitSave();
			this.Refresh(true);
			if (projectItem == null)
			{
				projectItem = base.Items.FindMatchByUrl<IProjectItem>(targetFolder);
			}
			if (projectItem == null)
			{
				return Enumerable.Empty<IProjectItem>();
			}
			return projectItem.Descendants.OfType<IProjectItem>().Except<IProjectItem>(array);
		}

		public override void AddImport(string path)
		{
			this.ReevaluateIfNecessary();
			ICollection<Microsoft.Build.Evaluation.ProjectItem> items = ((MSBuildBasedProjectStore)base.ProjectStore).MsBuildProject.Items;
			foreach (ResolvedImport import in ((MSBuildBasedProjectStore)base.ProjectStore).MsBuildProject.Imports)
			{
				if (import.ImportingElement.Project != path)
				{
					continue;
				}
				return;
			}
			this.AddNewImport(path, null);
		}

		private IProjectItem AddItem(Microsoft.Build.Evaluation.ProjectItem item, IDocumentType documentType, Microsoft.Expression.Framework.Documents.DocumentReference documentReference)
		{
			string str;
			IProjectItem projectItem = base.FindItem(documentReference);
			if (projectItem == null)
			{
				PerformanceUtility.MarkInterimStep(PerformanceEvent.ProjectPopulate, "CreateProjectItem");
				IProjectItem projectItem1 = documentType.CreateProjectItem(this, documentReference, base.Services);
				IMSBuildItemInternal mSBuildItemInternal = projectItem1 as IMSBuildItemInternal;
				if (mSBuildItemInternal != null)
				{
					mSBuildItemInternal.BuildItem = item;
				}
				if (base.AddProjectItem(projectItem1))
				{
					return projectItem1;
				}
			}
			else
			{
				if (!projectItem.IsLinkedFile)
				{
					string addExistingItemFileExistsMessage = StringTable.AddExistingItemFileExistsMessage;
					CultureInfo currentCulture = CultureInfo.CurrentCulture;
					object[] displayName = new object[] { projectItem.DocumentReference.DisplayName };
					str = string.Format(currentCulture, addExistingItemFileExistsMessage, displayName);
				}
				else
				{
					string linkToExistingItemAlreadyExistsMessage = StringTable.LinkToExistingItemAlreadyExistsMessage;
					CultureInfo cultureInfo = CultureInfo.CurrentCulture;
					object[] path = new object[] { projectItem.DocumentReference.Path, projectItem.DocumentReference.DisplayName };
					str = string.Format(cultureInfo, linkToExistingItemAlreadyExistsMessage, path);
				}
				this.projectLoadErrors.Add(str);
			}
			return null;
		}

		protected void AddNewImport(string import, string condition)
		{
			if (import != null)
			{
				((MSBuildBasedProjectStore)base.ProjectStore).MsBuildProject.Xml.AddImport(import).Condition = condition;
				base.ImplicitSave();
			}
		}

		public override IProjectItem AddProjectReference(IProject project)
		{
			if (!base.AttemptToMakeProjectWritable())
			{
				return null;
			}
			string path = project.DocumentReference.Path;
			IDocumentType item = base.Services.DocumentTypes()[DocumentTypeNamesHelper.ProjectReference];
			DocumentCreationInfo documentCreationInfo = new DocumentCreationInfo()
			{
				DocumentType = item,
				TargetPath = path
			};
			IProjectItem projectItem = base.AddItem(documentCreationInfo);
			if (projectItem != null)
			{
				IMSBuildItem mSBuildItem = (IMSBuildItem)projectItem;
				Guid projectGuid = project.ProjectGuid;
				mSBuildItem.SetMetadata("Project", projectGuid.ToString("B").ToUpperInvariant());
				mSBuildItem.SetMetadata("Name", project.Name);
			}
			return projectItem;
		}

		private void AssociateDanglingChildren(IProjectItem projectItem)
		{
			foreach (KeyValuePair<IProjectItem, int> danglingChild in this.danglingChildren)
			{
				if (projectItem.DocumentReference.GetHashCode() != danglingChild.Value)
				{
					continue;
				}
				projectItem.AddChild(danglingChild.Key);
				this.danglingChildren.Remove(danglingChild.Key);
				return;
			}
		}

		private void AssociateParent(IProjectItem projectItem)
		{
			string parentDirectory;
			if (projectItem == null || projectItem is AssemblyReferenceProjectItem)
			{
				return;
			}
			IMSBuildItem mSBuildItem = (IMSBuildItem)projectItem;
			string metadata = mSBuildItem.GetMetadata(Microsoft.Expression.Project.ProjectItem.MSBuildDependentUponMetadata);
			if (!string.IsNullOrEmpty(metadata))
			{
				try
				{
					Microsoft.Expression.Framework.Documents.DocumentReference documentReference = Microsoft.Expression.Framework.Documents.DocumentReference.CreateFromRelativePath(Path.GetDirectoryName(projectItem.DocumentReference.Path), metadata);
					IProjectItem item = base.Items[documentReference.GetHashCode()];
					if (item == null)
					{
						this.danglingChildren.Add(projectItem, documentReference.GetHashCode());
					}
					else
					{
						item.AddChild(projectItem);
						return;
					}
				}
				catch (ArgumentException argumentException)
				{
				}
			}
			if (base.IsWithinProjectRoot(projectItem.DocumentReference))
			{
				parentDirectory = Microsoft.Expression.Framework.Documents.PathHelper.GetParentDirectory(projectItem.DocumentReference.Path);
			}
			else
			{
				string str = mSBuildItem.GetMetadata("Link");
				if (string.IsNullOrEmpty(str) || !Microsoft.Expression.Framework.Documents.PathHelper.IsPathRelative(str))
				{
					return;
				}
				parentDirectory = Microsoft.Expression.Framework.Documents.PathHelper.ResolveRelativePath(base.ProjectRoot.Path, Microsoft.Expression.Framework.Documents.PathHelper.GetParentDirectory(str));
			}
			if (!string.IsNullOrEmpty(parentDirectory))
			{
				Microsoft.Expression.Framework.Documents.DocumentReference documentReference1 = Microsoft.Expression.Framework.Documents.DocumentReference.Create(parentDirectory);
				if (documentReference1.Equals(base.ProjectRoot))
				{
					return;
				}
				IProjectItem folderProjectItem = base.Items[documentReference1.GetHashCode()];
				if (folderProjectItem == null)
				{
					folderProjectItem = new FolderProjectItem(this, documentReference1, base.Services.DocumentTypes()[DocumentTypeNamesHelper.Folder], base.Services);
					base.AddProjectItem(folderProjectItem);
				}
				if (folderProjectItem.CanAddChildren)
				{
					folderProjectItem.AddChild(projectItem);
					this.DisassociateFolderItemFromProject((IMSBuildItemInternal)folderProjectItem);
				}
			}
		}

		public void BuildCompleted(Microsoft.Expression.Project.Build.BuildResult buildResult)
		{
			if (!base.IsDisposed && buildResult == Microsoft.Expression.Project.Build.BuildResult.Succeeded)
			{
				base.CheckForUpdatedAssemblies();
			}
		}

		private void BuildCurrentReachableAssemblies(Assembly assembly, HashSet<string> reachableAssemblyNames, Assembly[] appDomainAssemblies)
		{
			if (assembly == null || reachableAssemblyNames.Contains(assembly.FullName))
			{
				return;
			}
			reachableAssemblyNames.Add(assembly.FullName);
			AssemblyName[] referencedAssemblies = assembly.GetReferencedAssemblies();
			for (int i = 0; i < (int)referencedAssemblies.Length; i++)
			{
				string fullName = referencedAssemblies[i].FullName;
				Assembly assembly1 = null;
				Assembly[] assemblyArray = appDomainAssemblies;
				int num = 0;
				while (num < (int)assemblyArray.Length)
				{
					Assembly assembly2 = assemblyArray[num];
					if (assembly2.FullName != fullName)
					{
						num++;
					}
					else
					{
						assembly1 = assembly2;
						break;
					}
				}
				if (assembly1 != null)
				{
					this.BuildCurrentReachableAssemblies(assembly1, reachableAssemblyNames, appDomainAssemblies);
				}
			}
		}

		private bool BuildItemIsInCurrentProject(Microsoft.Build.Evaluation.ProjectItem item)
		{
			return item.Project == ((MSBuildBasedProjectStore)base.ProjectStore).MsBuildProject;
		}

		internal static Microsoft.Expression.Project.Build.BuildResult BuildProject(ProjectInstance instance, IEnumerable<ILogger> loggers, params string[] targetNames)
		{
			IDictionary<string, TargetResult> strs = new Dictionary<string, TargetResult>();
			try
			{
				if (instance.Build(targetNames, loggers, null, out strs))
				{
					return Microsoft.Expression.Project.Build.BuildResult.Succeeded;
				}
			}
			catch (OutOfMemoryException outOfMemoryException)
			{
				LowMemoryMessage.Show();
			}
			return Microsoft.Expression.Project.Build.BuildResult.Failed;
		}

		private ProjectInstance CreateProjectInstance()
		{
			base.ProjectStore.SetUnpersistedProperty(MSBuildBasedProject.BuildingInBlendPropertyName, "true");
			base.ProjectStore.SetProperty("Utf8Output", "true");
			this.ReevaluateIfNecessary();
			return ((MSBuildBasedProjectStore)base.ProjectStore).MsBuildProject.CreateProjectInstance();
		}

		private void DisassociateFolderItemFromProject(IMSBuildItemInternal folderItem)
		{
			if (folderItem.BuildItem != null && this.BuildItemIsInCurrentProject(folderItem.BuildItem))
			{
				((MSBuildBasedProjectStore)base.ProjectStore).MsBuildProject.RemoveItem(folderItem.BuildItem);
				folderItem.BuildItem = null;
			}
		}

		private string FindImplicitAssemblyReference(AssemblyName assemblyName)
		{
			if (Microsoft.Expression.Project.Build.BuildManager.Building)
			{
				return null;
			}
			string fullName = assemblyName.FullName;
			string resolvedAssemblyReference = null;
			IProjectItemData projectItemDatum = null;
			try
			{
				projectItemDatum = base.ProjectStore.AddItem("Reference", fullName);
				this.referencePathsNeedRebuilding = true;
				resolvedAssemblyReference = this.GetResolvedAssemblyReference(fullName);
			}
			finally
			{
				if (projectItemDatum != null)
				{
					base.ProjectStore.RemoveItem(projectItemDatum);
				}
			}
			if (!string.IsNullOrEmpty(resolvedAssemblyReference) && Microsoft.Expression.Framework.Documents.PathHelper.FileExists(resolvedAssemblyReference) && !this.IsInOutputDirectory(resolvedAssemblyReference))
			{
				return resolvedAssemblyReference;
			}
			Microsoft.Expression.Framework.Documents.DocumentReference documentReference = base.ReferencedAssemblies.Where<ProjectAssembly>((ProjectAssembly projectAssembly) => {
				if (projectAssembly.RuntimeAssembly == null)
				{
					return false;
				}
				return projectAssembly.ProjectItem != null;
			}).SelectMany((ProjectAssembly projectAssembly) => projectAssembly.RuntimeAssembly.GetReferencedAssemblies(), (ProjectAssembly projectAssembly, AssemblyName referencedAssemblyName) => new { projectAssembly = projectAssembly, referencedAssemblyName = referencedAssemblyName }).Where((argument0) => argument0.referencedAssemblyName.FullName.Equals(fullName, StringComparison.OrdinalIgnoreCase)).Select((argument3) => argument3.projectAssembly.ProjectItem.DocumentReference).FirstOrDefault<Microsoft.Expression.Framework.Documents.DocumentReference>();
			if (documentReference == null)
			{
				IProjectManager projectManager = base.Services.ProjectManager();
				string str = (
					from assembly in projectManager.ImplicitAssemblyReferences.Skip<Assembly>(base.ImplicitReferencedAssembliesCount)
					from referencedAssemblyName in assembly.GetReferencedAssemblies()
					where referencedAssemblyName.FullName.Equals(fullName, StringComparison.OrdinalIgnoreCase)
					select projectManager.DetermineOriginalImplicitAssemblyLocation(assembly)).FirstOrDefault<string>();
				if (!string.IsNullOrEmpty(str))
				{
					documentReference = Microsoft.Expression.Framework.Documents.DocumentReference.Create(str);
				}
			}
			if (documentReference != null && documentReference.IsValidPathFormat)
			{
				string directoryNameOrRoot = Microsoft.Expression.Framework.Documents.PathHelper.GetDirectoryNameOrRoot(documentReference.Path);
				if (!string.IsNullOrEmpty(directoryNameOrRoot))
				{
					string str1 = Microsoft.Expression.Framework.Documents.PathHelper.ResolveCombinedPath(directoryNameOrRoot, string.Concat(assemblyName.Name, ".dll"));
					AssemblyName assemblyName1 = null;
					assemblyName1 = (!((AssemblyService)base.Services.AssemblyService()).IsInstalledAssembly(str1) ? ProjectAssemblyHelper.GetAssemblyNameFromPath(str1) : ProjectAssemblyHelper.CachedGetAssemblyNameFromPath(str1));
					if (assemblyName1 != null && assemblyName.FullName.Equals(assemblyName1.FullName, StringComparison.OrdinalIgnoreCase))
					{
						return str1;
					}
				}
			}
			return null;
		}

		private Microsoft.Build.Evaluation.ProjectItem FindMSBuildItem(Microsoft.Expression.Framework.Documents.DocumentReference documentReference)
		{
			Microsoft.Build.Evaluation.ProjectItem projectItem;
			this.ReevaluateIfNecessary();
			using (IEnumerator<Microsoft.Build.Evaluation.ProjectItem> enumerator = ((MSBuildBasedProjectStore)base.ProjectStore).MsBuildProject.Items.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Microsoft.Build.Evaluation.ProjectItem current = enumerator.Current;
					try
					{
						if (!current.IsImported && current.UnevaluatedInclude.Length != 0 && (documentReference.IsValidPathFormat && documentReference.Equals(current.GetMetadata("FullPath")) || documentReference.Equals(current.UnevaluatedInclude)))
						{
							projectItem = current;
							return projectItem;
						}
					}
					catch (InvalidOperationException invalidOperationException)
					{
					}
				}
				return null;
			}
			return projectItem;
		}

		protected override ProjectAssembly GetAssembly(IProjectItem projectItem)
		{
			string targetAssembly = this.GetTargetAssembly(projectItem);
			if (string.IsNullOrEmpty(targetAssembly))
			{
				return null;
			}
			ProjectAssembly assembly = base.GetAssembly(targetAssembly, projectItem);
			base.UpdateReferenceAssembly(assembly);
			return assembly;
		}

		public override T GetCapability<T>(string name)
		{
			object capability;
			string str = name;
			string str1 = str;
			if (str != null)
			{
				switch (str1)
				{
					case "ExpressionBlendPrototypeHarness":
					case "ExpressionBlendPrototypingEnabled":
					{
						return Microsoft.Expression.Framework.Interop.TypeHelper.ConvertType<T>(this.GetEvaluatedPropertyValue(name));
					}
					case "CanAddAssemblyReference":
					case "CanAddProjectReference":
					case "CanAddLinks":
					{
						return Microsoft.Expression.Framework.Interop.TypeHelper.ConvertType<T>(true);
					}
					case "SourceControlBound":
					{
						return Microsoft.Expression.Framework.Interop.TypeHelper.ConvertType<T>((!"SAK".Equals(this.GetEvaluatedPropertyValue("SccAuxPath")) || !"SAK".Equals(this.GetEvaluatedPropertyValue("SccLocalPath")) || !"SAK".Equals(this.GetEvaluatedPropertyValue("SccProjectName")) ? false : "SAK".Equals(this.GetEvaluatedPropertyValue("SccProvider"))));
					}
					case "SupportsDatabinding":
					case "SupportsMediaElementControl":
					case "SupportsHyperlinkButtonControl":
					case "SupportsUIElementEffectProperty":
					case "SupportsAssetLibraryBehaviorsItems":
					{
						bool? nullable = Microsoft.Expression.Framework.Interop.TypeHelper.ConvertType<bool?>(this.GetEvaluatedPropertyValue(name));
						if (nullable.HasValue)
						{
							return Microsoft.Expression.Framework.Interop.TypeHelper.ConvertType<T>(nullable);
						}
						return Microsoft.Expression.Framework.Interop.TypeHelper.ConvertType<T>(!this.IsWindowsCEProject());
					}
					case "PlatformSimpleName":
					{
						bool? nullable1 = Microsoft.Expression.Framework.Interop.TypeHelper.ConvertType<bool?>(this.GetEvaluatedPropertyValue(name));
						if (nullable1.HasValue)
						{
							return Microsoft.Expression.Framework.Interop.TypeHelper.ConvertType<T>(nullable1);
						}
						if (!this.IsWindowsCEProject())
						{
							capability = base.GetCapability<T>(name);
						}
						else
						{
							capability = "WindowsCE";
						}
						return Microsoft.Expression.Framework.Interop.TypeHelper.ConvertType<T>(capability);
					}
				}
			}
			return base.GetCapability<T>(name);
		}

		protected virtual IDocumentType GetDocumentTypeForBuildItem(string fileName, string buildItemType)
		{
			return this.GetDocumentType(fileName);
		}

		public string GetEvaluatedPropertyValue(string propertyName)
		{
			return base.ProjectStore.GetProperty(propertyName);
		}

		internal string GetMscorlibPath()
		{
			string value;
			foreach (IProjectItemData item in base.ProjectStore.GetItems("_ExplicitReference"))
			{
				if (!Microsoft.Expression.Framework.Documents.PathHelper.GetFileOrDirectoryName(item.Value).Equals("mscorlib.dll", StringComparison.OrdinalIgnoreCase))
				{
					continue;
				}
				value = item.Value;
				return value;
			}
			FrameworkName targetFramework = this.TargetFramework;
			using (IEnumerator<string> enumerator = ToolLocationHelper.GetPathToReferenceAssemblies(targetFramework.Identifier, targetFramework.Version.ToString(), targetFramework.Profile).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string str = Microsoft.Expression.Framework.Documents.PathHelper.ResolveCombinedPath(enumerator.Current, "mscorlib.dll");
					if (!Microsoft.Expression.Framework.Documents.PathHelper.FileExists(str))
					{
						continue;
					}
					value = str;
					return value;
				}
				return null;
			}
			return value;
		}

		protected string GetProjectExtension(string extension)
		{
			string item;
			using (IEnumerator<ProjectElement> enumerator = ((MSBuildBasedProjectStore)base.ProjectStore).MsBuildProject.Xml.ChildrenReversed.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ProjectExtensionsElement current = enumerator.Current as ProjectExtensionsElement;
					if (current == null)
					{
						continue;
					}
					item = current[extension];
					return item;
				}
				return string.Empty;
			}
			return item;
		}

		private string GetProperty(string propertyName)
		{
			this.ReevaluateIfNecessary();
			return base.ProjectStore.GetProperty(propertyName);
		}

		private IEnumerable<ProjectItemInstance> GetReferencePathBuildItemGroup()
		{
			IEnumerable<ProjectItemInstance> items = null;
			bool flag = this.referencePathsNeedRebuilding;
			if (!flag)
			{
				items = this.msBuildProjectInstance.GetItems("ReferencePath");
				flag = items.Count<ProjectItemInstance>() <= 0;
			}
			if (flag)
			{
				this.RebuildAssemblyReferencePaths();
				items = this.msBuildProjectInstance.GetItems("ReferencePath");
			}
			return items;
		}

		protected string GetResolvedAssemblyReference(string assemblyName)
		{
			string str;
			if (string.IsNullOrEmpty(assemblyName))
			{
				return null;
			}
			string str1 = null;
			IEnumerable<ProjectItemInstance> referencePathBuildItemGroup = this.GetReferencePathBuildItemGroup();
			if (referencePathBuildItemGroup != null)
			{
				int num = assemblyName.IndexOf(',');
				if (num >= 0)
				{
					assemblyName = assemblyName.Substring(0, num);
				}
				str = (".dll".Equals(Path.GetExtension(assemblyName), StringComparison.OrdinalIgnoreCase) || ".exe".Equals(Path.GetExtension(assemblyName), StringComparison.OrdinalIgnoreCase) ? Path.GetFileNameWithoutExtension(assemblyName) : Path.GetFileName(assemblyName));
				foreach (ProjectItemInstance projectItemInstance in referencePathBuildItemGroup)
				{
					string metadataValue = projectItemInstance.GetMetadataValue("FileName");
					if (StringComparer.OrdinalIgnoreCase.Compare(str, metadataValue) != 0)
					{
						continue;
					}
					string evaluatedInclude = projectItemInstance.EvaluatedInclude;
					if (!Microsoft.Expression.Framework.Documents.PathHelper.IsValidPath(evaluatedInclude))
					{
						break;
					}
					str1 = Microsoft.Expression.Framework.Documents.PathHelper.ResolveRelativePath(base.ProjectRoot.Path, evaluatedInclude);
					break;
				}
			}
			return str1;
		}

		private string GetTargetAssembly(IProjectItem projectItem)
		{
			string path = null;
			if (projectItem is AssemblyProjectItem || projectItem is ComReferenceProjectItem)
			{
				path = projectItem.DocumentReference.Path;
			}
			else if (projectItem is ProjectReferenceProjectItem && Microsoft.Expression.Framework.Documents.PathHelper.FileExists(projectItem.DocumentReference.Path))
			{
				try
				{
					Microsoft.Build.Evaluation.Project project = Microsoft.Expression.Project.Build.BuildManager.GetProject(projectItem.DocumentReference);
					project.ReevaluateIfNecessary();
					string propertyValue = project.GetPropertyValue("TargetFileName");
					string str = project.GetPropertyValue("OutputPath");
					if (!string.IsNullOrEmpty(propertyValue) && !string.IsNullOrEmpty(str))
					{
						path = Microsoft.Expression.Framework.Documents.PathHelper.ResolveRelativePath(Microsoft.Expression.Framework.Documents.PathHelper.ResolveRelativePath(Microsoft.Expression.Framework.Documents.PathHelper.GetDirectory(projectItem.DocumentReference.Path), str), propertyValue);
					}
				}
				catch (InvalidProjectFileException invalidProjectFileException)
				{
				}
			}
			return path;
		}

		public bool HasProperty(string propertyName)
		{
			return this.GetProperty(propertyName) != null;
		}

		protected override bool Initialize()
		{
			PerformanceUtility.StartPerformanceSequence(PerformanceEvent.ProjectPopulate, "InitializeExistingProject");
			this.Refresh(false);
			base.EnsureDesignMetadataLoaded(true);
			this.InitializeGlobalImportsList();
			PerformanceUtility.EndPerformanceSequence(PerformanceEvent.ProjectPopulate, "InitializeExistingProject");
			return true;
		}

		private void InitializeGlobalImportsList()
		{
			this.GlobalImportsList = new List<string>();
			foreach (IProjectItemData item in base.ProjectStore.GetItems("Import"))
			{
				if (string.IsNullOrEmpty(item.Value))
				{
					continue;
				}
				this.GlobalImportsList.Add(item.Value);
			}
		}

		private Microsoft.Build.Evaluation.ProjectItem InitializeMSBuildItem(IProjectItem projectItem, BuildTaskInfo buildTaskInfo)
		{
			string str = (projectItem.DocumentReference.IsValidPathFormat ? base.DocumentReference.GetRelativePath(projectItem.DocumentReference) : projectItem.DocumentReference.Path);
			Microsoft.Build.Evaluation.ProjectItem projectItem1 = ((MSBuildBasedProjectStore)base.ProjectStore).AddMsBuildItem(buildTaskInfo.BuildTask, str);
			if (projectItem1 != null)
			{
				((IMSBuildItemInternal)projectItem).BuildItem = projectItem1;
				if (buildTaskInfo.MetadataInfo != null)
				{
					foreach (KeyValuePair<string, string> metadataInfo in buildTaskInfo.MetadataInfo)
					{
						IMSBuildItem mSBuildItem = projectItem as IMSBuildItem;
						if (mSBuildItem == null)
						{
							continue;
						}
						mSBuildItem.SetMetadata(metadataInfo.Key, metadataInfo.Value);
					}
				}
			}
			return projectItem1;
		}

		private void InitializeNonPersistedProperties()
		{
			string evaluatedPropertyValue = this.GetEvaluatedPropertyValue("Configuration");
			if (evaluatedPropertyValue != null)
			{
				base.ProjectStore.SetUnpersistedProperty("Configuration", evaluatedPropertyValue);
			}
			base.ProjectStore.SetUnpersistedProperty(MSBuildBasedProject.BuildingInBlendPropertyName, "false");
			IProjectManager service = base.Services.GetService<IProjectManager>();
			if (service != null && service.CurrentSolution != null)
			{
				string path = service.CurrentSolution.DocumentReference.Path;
				base.ProjectStore.SetUnpersistedProperty("SolutionPath", path);
				base.ProjectStore.SetUnpersistedProperty("SolutionDir", Microsoft.Expression.Framework.Documents.PathHelper.EnsurePathEndsInDirectorySeparator(Path.GetDirectoryName(path)));
				base.ProjectStore.SetUnpersistedProperty("SolutionFileName", Path.GetFileName(path));
				base.ProjectStore.SetUnpersistedProperty("SolutionName", Path.GetFileNameWithoutExtension(path));
				base.ProjectStore.SetUnpersistedProperty("SolutionExt", Path.GetExtension(path));
			}
		}

		private bool IsFrameworkSupported(FrameworkName targetFramework)
		{
			if (targetFramework == null)
			{
				return false;
			}
			string identifier = targetFramework.Identifier;
			string str = identifier;
			if (identifier != null && (str == "Silverlight" || str == ".NETFramework"))
			{
				return true;
			}
			return false;
		}

		private bool IsInOutputDirectory(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return false;
			}
			string evaluatedPropertyValue = this.GetEvaluatedPropertyValue("OutputPath");
			if (evaluatedPropertyValue == null || !Microsoft.Expression.Framework.Documents.PathHelper.IsValidPath(evaluatedPropertyValue))
			{
				return false;
			}
			string str = Microsoft.Expression.Framework.Documents.PathHelper.ResolveRelativePath(base.ProjectRoot.Path, evaluatedPropertyValue);
			if (string.IsNullOrEmpty(str))
			{
				return false;
			}
			return Microsoft.Expression.Framework.Documents.PathHelper.GetDirectoryNameOrRoot(path).Equals(str, StringComparison.OrdinalIgnoreCase);
		}

		protected virtual bool IsValidAssemblyReference(string fullAssemblyPath, bool verifyFileExists)
		{
			if (!verifyFileExists)
			{
				return true;
			}
			return Microsoft.Expression.Framework.Documents.PathHelper.FileOrDirectoryExists(fullAssemblyPath);
		}

		private bool IsWindowsCEProject()
		{
			if (!this.isWindowsCEProject.HasValue)
			{
				this.isWindowsCEProject = new bool?(Microsoft.Expression.Framework.Interop.TypeHelper.ConvertType<bool>(this.GetEvaluatedPropertyValue("WindowsCE")));
			}
			return this.isWindowsCEProject.Value;
		}

		void Microsoft.Expression.Project.IMSBuildBasedProject.OnBuildItemChanged()
		{
			base.ImplicitSave();
		}

		protected virtual void OnProjectItemCreated(IProjectItem item)
		{
		}

		protected virtual void OnRefresh()
		{
		}

		protected override void ProcessNewlyAddedProjectItem(IProjectItem projectItem)
		{
			PerformanceUtility.MarkInterimStep(PerformanceEvent.ProjectAddProjectItem, "PlaceItemInFolder");
			this.AssociateParent(projectItem);
			PerformanceUtility.MarkInterimStep(PerformanceEvent.ProjectAddProjectItem, "base.AddProjectItem");
			if (projectItem is AssemblyProjectItem)
			{
				this.referencePathsNeedRebuilding = true;
				base.ImplicitReferencedAssembliesCount = 0;
			}
			base.InternalAddAssemblyReference(projectItem);
			PerformanceUtility.MarkInterimStep(PerformanceEvent.ProjectAddProjectItem, "Add reference");
			base.OnItemAdded(new ProjectItemEventArgs(projectItem));
			this.AssociateDanglingChildren(projectItem);
			PerformanceUtility.MarkInterimStep(PerformanceEvent.ProjectAddProjectItem, "OnItemAdded");
		}

		protected override void ProcessNewlyCreatedProjectItem(IProjectItem projectItem, DocumentCreationInfo creationInfo)
		{
			Microsoft.Build.Evaluation.ProjectItem projectItem1 = this.InitializeMSBuildItem(projectItem, creationInfo.BuildTaskInfo);
			if ((creationInfo.CreationOptions & CreationOptions.LinkSourceFile) == CreationOptions.LinkSourceFile)
			{
				projectItem1.SetMetadataValue("Link", base.DocumentReference.GetRelativePath(Microsoft.Expression.Framework.Documents.DocumentReference.Create(creationInfo.TargetPath)));
			}
			this.OnProjectItemCreated(projectItem);
			this.UpdateDependentUponForCodeBehind(projectItem);
		}

		private void RebuildAssemblyReferencePaths()
		{
			if (!this.disableRebuildingReferencePath)
			{
				this.msBuildProjectInstance = this.CreateProjectInstance();
				ProjectInstance projectInstance = this.msBuildProjectInstance;
				string[] strArrays = new string[] { "ResolveComReferences", "ResolveAssemblyReferences" };
				if (MSBuildBasedProject.BuildProject(projectInstance, null, strArrays) == Microsoft.Expression.Project.Build.BuildResult.Succeeded)
				{
					this.referencePathsNeedRebuilding = false;
				}
			}
		}

		private void ReevaluateIfNecessary()
		{
			lock (this.msbuildProjectSyncLock)
			{
				((MSBuildBasedProjectStore)base.ProjectStore).MsBuildProject.ReevaluateIfNecessary();
			}
		}

		public override void Refresh()
		{
			this.Refresh(true);
			if (base.Services.ProjectManager().CurrentSolution.IsSourceControlActive)
			{
				SourceControlStatusCache.UpdateStatus(this.Descendants.AppendItem<IDocumentItem>(this), base.Services.SourceControlProvider());
			}
		}

		private void Refresh(bool reloadProjectFile)
		{
			if (this.currentlyRefreshing)
			{
				return;
			}
			try
			{
				this.currentlyRefreshing = true;
				this.OnRefresh();
				if (reloadProjectFile)
				{
					base.ReloadProjectStore();
				}
				this.ReloadProjectBuildContext();
				base.ProjectAssemblyReferencesDirty = true;
				this.resetCapabilityCache();
				PerformanceUtility.MarkInterimStep(PerformanceEvent.ProjectPopulate, "InitializeNonPersistedProperties");
				this.InitializeNonPersistedProperties();
				this.ReevaluateIfNecessary();
				IEnumerable<Microsoft.Build.Evaluation.ProjectItem> projectItems = new List<Microsoft.Build.Evaluation.ProjectItem>(((MSBuildBasedProjectStore)base.ProjectStore).MsBuildProject.Items);
				string currentDirectory = Environment.CurrentDirectory;
				IDisposable disposable = null;
				try
				{
					disposable = CurrentDirectoryHelper.SetCurrentDirectory(base.ProjectRoot.Path);
					if (this.IsFrameworkSupported(this.TargetFramework))
					{
						base.NonIncrementalAssemblyUpdate(() => this.RefreshAssemblyReferences(projectItems));
					}
					this.RefreshNonAssemblyReferences(projectItems);
				}
				finally
				{
					if (disposable != null)
					{
						disposable.Dispose();
					}
					if (this.projectLoadErrors.Count > 0)
					{
						foreach (string projectLoadError in this.projectLoadErrors)
						{
							HostLogger.LogFormattedError(this.ProjectBuildContext, projectLoadError, base.Name, 0, 0, "");
						}
						IErrorService errorService = base.Services.ErrorService();
						if (errorService != null)
						{
							errorService.DisplayErrors();
						}
					}
				}
			}
			finally
			{
				this.currentlyRefreshing = false;
			}
		}

		private void RefreshAssemblyReferences(IEnumerable<Microsoft.Build.Evaluation.ProjectItem> evaluatedItems)
		{
			this.RebuildAssemblyReferencePaths();
			Dictionary<string, AssemblyReferenceProjectItem> strs = new Dictionary<string, AssemblyReferenceProjectItem>();
			foreach (IProjectItem item in (IEnumerable<IProjectItem>)base.Items)
			{
				AssemblyReferenceProjectItem assemblyReferenceProjectItem = item as AssemblyReferenceProjectItem;
				if (assemblyReferenceProjectItem == null)
				{
					continue;
				}
				strs.Add(assemblyReferenceProjectItem.DocumentReference.Path, assemblyReferenceProjectItem);
			}
			try
			{
				this.RefreshImplicitAssemblyReferences();
				this.disableRebuildingReferencePath = true;
				foreach (Microsoft.Build.Evaluation.ProjectItem evaluatedItem in evaluatedItems)
				{
					string evaluatedInclude = evaluatedItem.EvaluatedInclude;
					if (Microsoft.Expression.Framework.Documents.PathHelper.ValidateAndFixPathIfPossible(ref evaluatedInclude))
					{
						evaluatedInclude = Microsoft.Expression.Framework.Documents.PathHelper.ResolveRelativePath(base.ProjectRoot.Path, evaluatedInclude);
					}
					IDocumentType documentType = null;
					string str = null;
					PerformanceUtility.MarkInterimStep(PerformanceEvent.ProjectPopulate, string.Concat("Handling: ", evaluatedItem.ItemType));
					string itemType = evaluatedItem.ItemType;
					string str1 = itemType;
					if (itemType != null)
					{
						if (str1 == "Reference")
						{
							documentType = base.Services.DocumentTypes()[DocumentTypeNamesHelper.Assembly];
							if (StringComparer.InvariantCultureIgnoreCase.Compare(evaluatedItem.GetMetadataValue("Private"), "True") != 0)
							{
								str = evaluatedItem.EvaluatedInclude;
							}
							else
							{
								string metadataValue = evaluatedItem.GetMetadataValue("HintPath");
								str = (!string.IsNullOrEmpty(metadataValue) ? Microsoft.Expression.Framework.Documents.PathHelper.ResolveRelativePath(base.ProjectRoot.Path, metadataValue) : evaluatedInclude);
							}
						}
						else if (str1 != "ProjectReference")
						{
							if (str1 == "COMReference")
							{
								documentType = base.Services.DocumentTypes()[DocumentTypeNamesHelper.ComReference];
								str = evaluatedItem.EvaluatedInclude;
							}
						}
						else if (evaluatedInclude == null)
						{
							documentType = null;
							str = null;
						}
						else
						{
							documentType = base.Services.DocumentTypes()[DocumentTypeNamesHelper.ProjectReference];
							str = evaluatedInclude;
						}
					}
					if (documentType == null)
					{
						continue;
					}
					if (Microsoft.Expression.Framework.Documents.PathHelper.IsPathRelative(str) || !Microsoft.Expression.Framework.Documents.PathHelper.FileExists(str))
					{
						string resolvedAssemblyReference = this.GetResolvedAssemblyReference(str);
						if (!string.IsNullOrEmpty(resolvedAssemblyReference))
						{
							str = resolvedAssemblyReference;
						}
					}
					if (!strs.ContainsKey(str))
					{
						this.AddItem(evaluatedItem, documentType, Microsoft.Expression.Framework.Documents.DocumentReference.Create(str));
					}
					else
					{
						strs.Remove(str);
					}
				}
				base.RemoveProjectItems(false, strs.Values.ToArray<AssemblyReferenceProjectItem>());
			}
			finally
			{
				this.referencePathsNeedRebuilding = false;
				this.disableRebuildingReferencePath = false;
				this.UpdateAssemblyReferencesImmediately();
			}
		}

		private void RefreshImplicitAssemblyReferences()
		{
			string fullTargetFileName = this.FullTargetFileName;
			if (string.IsNullOrEmpty(fullTargetFileName))
			{
				base.TargetAssembly = null;
			}
			else
			{
				base.TargetAssembly = base.GetAssembly(fullTargetFileName, null);
			}
			if (!this.DefinesOwnMscorlib)
			{
				Assembly assembly = typeof(int).Assembly;
				base.InternalAddImplicitAssemblyReference(assembly, this.GetMscorlibPath(), false, true);
			}
		}

		private void RefreshNonAssemblyReferences(IEnumerable<Microsoft.Build.Evaluation.ProjectItem> evaluatedItems)
		{
			this.hasWildcardBasedItem = false;
			Dictionary<string, IProjectItem> strs = new Dictionary<string, IProjectItem>();
			foreach (IProjectItem item in (IEnumerable<IProjectItem>)base.Items)
			{
				if (item is AssemblyReferenceProjectItem)
				{
					continue;
				}
				strs.Add(item.DocumentReference.Path, item);
			}
			foreach (Microsoft.Build.Evaluation.ProjectItem evaluatedItem in evaluatedItems)
			{
				if (evaluatedItem.IsImported)
				{
					continue;
				}
				IDocumentType documentTypeForBuildItem = null;
				string evaluatedInclude = evaluatedItem.EvaluatedInclude;
				Microsoft.Expression.Framework.Documents.DocumentReference documentReference = null;
				documentReference = (!Microsoft.Expression.Framework.Documents.PathHelper.ValidateAndFixPathIfPossible(ref evaluatedInclude) ? Microsoft.Expression.Framework.Documents.DocumentReference.Create(evaluatedInclude) : Microsoft.Expression.Framework.Documents.DocumentReference.CreateFromRelativePath(base.ProjectRoot.Path, evaluatedInclude));
				PerformanceUtility.MarkInterimStep(PerformanceEvent.ProjectPopulate, string.Concat("Handling: ", evaluatedItem.ItemType));
				string itemType = evaluatedItem.ItemType;
				string str = itemType;
				if (itemType != null)
				{
					switch (str)
					{
						case "AppDesigner":
						case "Import":
						{
							documentTypeForBuildItem = null;
							documentReference = null;
							goto Label0;
						}
						case "Folder":
						{
							if (evaluatedInclude == null)
							{
								documentReference = null;
								documentTypeForBuildItem = null;
								goto Label0;
							}
							else
							{
								documentTypeForBuildItem = base.Services.DocumentTypes()[DocumentTypeNamesHelper.Folder];
								goto Label0;
							}
						}
						case "Reference":
						case "ProjectReference":
						case "COMReference":
						{
							goto Label0;
						}
					}
				}
				documentTypeForBuildItem = this.GetDocumentTypeForBuildItem(documentReference.Path, evaluatedItem.ItemType);
			Label0:
				if (documentTypeForBuildItem == null)
				{
					continue;
				}
				if (evaluatedItem.UnevaluatedInclude.ToString().IndexOf('*') != -1)
				{
					this.hasWildcardBasedItem = true;
				}
				if (!strs.ContainsKey(documentReference.Path))
				{
					this.AddItem(evaluatedItem, documentTypeForBuildItem, documentReference);
				}
				else
				{
					IDocumentType documentType = strs[documentReference.Path].DocumentType;
					IMSBuildItemInternal mSBuildItemInternal = strs[documentReference.Path] as IMSBuildItemInternal;
					if (mSBuildItemInternal != null)
					{
						mSBuildItemInternal.BuildItem = evaluatedItem;
					}
					strs.Remove(documentReference.Path);
				}
			}
			foreach (IProjectItem value in strs.Values)
			{
				if (value.IsVirtual || value is FolderProjectItem)
				{
					continue;
				}
				IMSBuildItemInternal mSBuildItemInternal1 = value as IMSBuildItemInternal;
				if (mSBuildItemInternal1 == null)
				{
					continue;
				}
				mSBuildItemInternal1.BuildItem = null;
			}
			KeyValuePair<string, IProjectItem>[] array = strs.ToArray<KeyValuePair<string, IProjectItem>>();
			for (int i = 0; i < (int)array.Length; i++)
			{
				KeyValuePair<string, IProjectItem> keyValuePair = array[i];
				IProjectItem projectItem = keyValuePair.Value;
				if (!projectItem.IsVirtual && !(projectItem is FolderProjectItem))
				{
					IMSBuildItemInternal mSBuildItemInternal2 = projectItem as IMSBuildItemInternal;
					if (mSBuildItemInternal2 != null)
					{
						mSBuildItemInternal2.BuildItem = null;
					}
					base.RemoveProjectItems(false, new IProjectItem[] { projectItem });
					strs.Remove(keyValuePair.Key);
				}
			}
			foreach (IProjectItem value1 in strs.Values)
			{
				if (value1.IsVirtual || !(value1 is FolderProjectItem) || value1.Children.Any<IProjectItem>())
				{
					continue;
				}
				IMSBuildItemInternal mSBuildItemInternal3 = value1 as IMSBuildItemInternal;
				if (mSBuildItemInternal3 != null)
				{
					mSBuildItemInternal3.BuildItem = null;
				}
				base.RemoveProjectItems(false, new IProjectItem[] { value1 });
			}
		}

		private void ReloadProjectBuildContext()
		{
			this.ProjectBuildContext = ((MSBuildBasedProjectStore)base.ProjectStore).GenerateNewBuildContext(new Action<Microsoft.Expression.Project.Build.BuildResult>(this.BuildCompleted));
		}

		private void RemoveItemFromFolder(IProjectItem projectItem)
		{
			IProjectItem parent = projectItem.Parent;
			if (parent != null && parent.CanAddChildren)
			{
				IMSBuildItemInternal mSBuildItemInternal = (IMSBuildItemInternal)parent;
				parent.RemoveChild(projectItem);
				if (mSBuildItemInternal.BuildItem == null && !parent.Children.Any<IProjectItem>())
				{
					mSBuildItemInternal.BuildItem = this.InitializeMSBuildItem(parent, parent.DocumentType.DefaultBuildTaskInfo);
					this.OnProjectItemCreated(parent);
				}
			}
		}

		protected override void RemoveProjectItem(IProjectItem projectItem, bool deleteFiles)
		{
			Microsoft.Build.Evaluation.ProjectItem buildItem;
			if (projectItem == null)
			{
				return;
			}
			try
			{
				if (projectItem.Children.Any<IProjectItem>())
				{
					base.RemoveProjectItems(deleteFiles, projectItem.Children.ToArray<IProjectItem>());
				}
				this.RemoveItemFromFolder(projectItem);
				base.RemoveProjectItem(projectItem, deleteFiles);
				base.InternalRemoveAssemblyReference(projectItem);
				if (projectItem is AssemblyProjectItem)
				{
					base.ProjectAssemblyReferencesDirty = true;
					this.referencePathsNeedRebuilding = true;
				}
				IMSBuildItemInternal mSBuildItemInternal = projectItem as IMSBuildItemInternal;
				if (mSBuildItemInternal == null)
				{
					buildItem = null;
				}
				else
				{
					buildItem = mSBuildItemInternal.BuildItem;
				}
				Microsoft.Build.Evaluation.ProjectItem projectItem1 = buildItem;
				if (projectItem1 == null || projectItem1.IsImported)
				{
					Microsoft.Build.Evaluation.ProjectItem projectItem2 = this.FindMSBuildItem(projectItem.DocumentReference);
					if (projectItem2 != null)
					{
						((MSBuildBasedProjectStore)base.ProjectStore).MsBuildProject.RemoveItem(projectItem2);
					}
				}
				else
				{
					bool flag = projectItem1.UnevaluatedInclude.IndexOf('*') != -1;
					try
					{
						if (Microsoft.Expression.Project.Build.BuildManager.ProjectCollection.LoadedProjects.Contains(projectItem1.Project))
						{
							((MSBuildBasedProjectStore)base.ProjectStore).MsBuildProject.RemoveItem(projectItem1);
						}
					}
					catch (NullReferenceException nullReferenceException)
					{
						if (!flag)
						{
							throw;
						}
						else
						{
							this.Refresh(false);
							this.RemoveProjectItem(base.Items[projectItem.DocumentReference.GetHashCode()], deleteFiles);
						}
					}
					mSBuildItemInternal.BuildItem = null;
					if (flag)
					{
						this.Refresh(false);
					}
				}
				if (projectItem.IsOpen)
				{
					projectItem.CloseDocument();
				}
				if (projectItem.IsOpen)
				{
					projectItem.CloseDocument();
				}
				if (deleteFiles)
				{
					base.DeleteProjectItem(projectItem);
				}
				base.OnItemRemoved(new ProjectItemEventArgs(projectItem));
			}
			finally
			{
				projectItem.Dispose();
			}
		}

		protected override void RenameProjectItemInternal(IProjectItem oldProjectItem, Microsoft.Expression.Framework.Documents.DocumentReference newDocumentReference)
		{
			((IMSBuildItem)oldProjectItem).Include = base.DocumentReference.GetRelativePath(newDocumentReference);
			this.UpdateDependentUponForCodeBehind(oldProjectItem);
		}

		public override void ReportChangedItem(IProjectItem projectItem, ProjectItemEventOptions options)
		{
			if (!this.hasWildcardBasedItem || !(Path.GetExtension(projectItem.DocumentReference.Path).ToUpperInvariant() == ".XAML"))
			{
				base.InternalRemoveAssemblyReference(projectItem);
				base.InternalAddAssemblyReference(projectItem);
			}
			else
			{
				this.Refresh(false);
			}
			base.ReportChangedItem(projectItem, options);
		}

		protected override void ReportDeletedItem(IProjectItem deletedItem)
		{
			base.InternalRemoveAssemblyReference(deletedItem);
			base.ReportDeletedItem(deletedItem);
		}

		private void resetCapabilityCache()
		{
			this.isWindowsCEProject = null;
		}

		protected override Assembly ResolveImplicitProjectAssembly(AssemblyName assemblyName)
		{
			Assembly assembly;
			Thread currentThread = Thread.CurrentThread;
			if (currentThread != null && currentThread.IsBackground)
			{
				return null;
			}
			if (assemblyName != null && assemblyName.FullName.StartsWith("Microsoft.Build", StringComparison.OrdinalIgnoreCase) && ProjectAssemblyHelper.ComparePublicKeyTokens(KnownProjectBase.GetMicrosoftPublicKey(), assemblyName.GetPublicKeyToken()))
			{
				return null;
			}
			lock (this.implicitResolveSyncLock)
			{
				if (!this.isAlreadyImplicitlyResolving)
				{
					this.isAlreadyImplicitlyResolving = true;
					goto Label0;
				}
				else
				{
					assembly = null;
				}
			}
			return assembly;
		Label0:
			Assembly assembly1 = null;
			try
			{
				string str = this.FindImplicitAssemblyReference(assemblyName);
				if (!string.IsNullOrEmpty(str))
				{
					AssemblyService assemblyService = (AssemblyService)base.Services.AssemblyService();
					assembly1 = assemblyService.ResolveAssembly(str).Assembly;
				}
				if (assembly1 != null)
				{
					base.Services.ProjectManager().AddImplicitAssemblyReference(assembly1, str);
				}
			}
			finally
			{
				lock (this.implicitResolveSyncLock)
				{
					this.isAlreadyImplicitlyResolving = false;
				}
			}
			return assembly1;
		}

		protected override void SaveProjectFileInternal()
		{
			lock (this.msbuildProjectSyncLock)
			{
				base.ProjectStore.Save();
			}
		}

		public override bool SetCapability<T>(string name, T value)
		{
			string str = name;
			string str1 = str;
			if (str != null)
			{
				if (str1 == "ExpressionBlendPrototypeHarness" || str1 == "ExpressionBlendPrototypingEnabled")
				{
					string str2 = value.ToString();
					if (object.Equals(str2, this.GetEvaluatedPropertyValue(name)))
					{
						return true;
					}
					if (!base.AttemptToMakeProjectWritable())
					{
						return false;
					}
					base.ProjectStore.SetProperty(name, str2);
					base.ImplicitSave();
					return true;
				}
				if (str1 == "SourceControlBound")
				{
					if (object.Equals(this.GetCapability<T>(name), value))
					{
						return true;
					}
					if (!base.AttemptToMakeProjectWritable())
					{
						return false;
					}
					if (!Microsoft.Expression.Framework.Interop.TypeHelper.ConvertType<bool>(value))
					{
						base.ProjectStore.SetProperty("SccAuxPath", null);
						base.ProjectStore.SetProperty("SccLocalPath", null);
						base.ProjectStore.SetProperty("SccProjectName", null);
						base.ProjectStore.SetProperty("SccProvider", null);
					}
					else
					{
						base.ProjectStore.SetProperty("SccAuxPath", "SAK");
						base.ProjectStore.SetProperty("SccLocalPath", "SAK");
						base.ProjectStore.SetProperty("SccProjectName", "SAK");
						base.ProjectStore.SetProperty("SccProvider", "SAK");
					}
					base.ImplicitSave();
					return true;
				}
			}
			return base.SetCapability<T>(name, value);
		}

		private bool ShouldUpdateAssemblyReferences(List<string> updatedAssemblies)
		{
			bool flag = null != updatedAssemblies.Find((string candidate) => string.Compare(this.FullTargetFileName, candidate, StringComparison.OrdinalIgnoreCase) == 0);
			if (!flag)
			{
				foreach (IProjectItem item in (IEnumerable<IProjectItem>)base.Items)
				{
					string targetAssembly = this.GetTargetAssembly(item);
					if (string.IsNullOrEmpty(targetAssembly))
					{
						continue;
					}
					if (updatedAssemblies.Find((string candidate) => string.Compare(targetAssembly, candidate, StringComparison.OrdinalIgnoreCase) == 0) == null)
					{
						continue;
					}
					flag = true;
					break;
				}
			}
			return flag;
		}

		internal void UpdateAssemblyReferences(List<string> updatedAssemblies)
		{
			if (base.IsDisposed)
			{
				return;
			}
			if (updatedAssemblies == null || this.ShouldUpdateAssemblyReferences(updatedAssemblies))
			{
				base.CheckForUpdatedAssemblies();
			}
		}

		protected override void UpdateAssemblyReferencesImmediately()
		{
			if (base.IsDisposed)
			{
				return;
			}
			PerformanceUtility.StartPerformanceSequence(PerformanceEvent.UpdateAssemblyReferences);
			base.NonIncrementalAssemblyUpdate(() => {
				this.RefreshImplicitAssemblyReferences();
				foreach (IProjectItem item in (IEnumerable<IProjectItem>)base.Items)
				{
					base.InternalAddAssemblyReference(item);
				}
				HashSet<string> strs = new HashSet<string>();
				foreach (ProjectAssembly referencedAssembly in base.ReferencedAssemblies)
				{
					this.BuildCurrentReachableAssemblies(referencedAssembly.RuntimeAssembly, strs, AppDomain.CurrentDomain.GetAssemblies());
				}
				IProjectManager projectManager = base.Services.ProjectManager();
				foreach (Assembly implicitAssemblyReference in projectManager.ImplicitAssemblyReferences)
				{
					if (!strs.Contains(implicitAssemblyReference.FullName))
					{
						continue;
					}
					base.InternalAddImplicitAssemblyReference(implicitAssemblyReference, null, true, false);
				}
				base.ImplicitReferencedAssembliesCount = projectManager.ImplicitAssemblyReferences.Count;
			});
			PerformanceUtility.EndPerformanceSequence(PerformanceEvent.UpdateAssemblyReferences);
		}

		private void UpdateDependentUponForCodeBehind(IProjectItem projectItem)
		{
			string path = projectItem.DocumentReference.Path;
			if (path.EndsWith(string.Concat(".xaml", this.codeDocumentType.DefaultFileExtension), StringComparison.OrdinalIgnoreCase))
			{
				((IMSBuildItem)projectItem).SetMetadata(Microsoft.Expression.Project.ProjectItem.MSBuildDependentUponMetadata, Path.GetFileNameWithoutExtension(path));
			}
		}
	}
}