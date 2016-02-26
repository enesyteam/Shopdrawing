using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Threading;

namespace Microsoft.Expression.Project.Build
{
	public sealed class PartialXamlBuilder : IProjectBuildContext
	{
		private static string DefaultXamlCopyDirectory;

		private IErrorTaskCollection taskCollection = new ErrorTaskCollection();

		private IProject project;

		private IProjectItem projectItem;

		private string xamlText;

		private string fileName;

		private Microsoft.Expression.Project.Build.BuildManager buildManager;

		private PartialXamlBuilder.XamlBuildWorker buildWorker;

		private string tempFilePath;

		public IErrorTaskCollection BuildErrors
		{
			get
			{
				return this.taskCollection;
			}
		}

		public IBuildWorker BuildWorker
		{
			get
			{
				return this.buildWorker;
			}
		}

		public string Configuration
		{
			get
			{
				return "Debug";
			}
		}

		public string DisplayName
		{
			get
			{
				return string.Empty;
			}
		}

		public string FullTargetPath
		{
			get
			{
				return this.fileName;
			}
		}

		public bool NeedsBuilding
		{
			get
			{
				return true;
			}
		}

		public Microsoft.Build.Execution.ProjectInstance ProjectInstance
		{
			get
			{
				return null;
			}
		}

		static PartialXamlBuilder()
		{
			PartialXamlBuilder.DefaultXamlCopyDirectory = "Microsoft\\Expression\\Blend\\Partial Xaml\\";
		}

		public PartialXamlBuilder(IProject project, IProjectItem projectItem, string xamlText, string fileName, Microsoft.Expression.Project.Build.BuildManager buildManager)
		{
			this.project = project;
			this.projectItem = projectItem;
			this.xamlText = xamlText;
			this.fileName = fileName;
			this.buildManager = buildManager;
		}

		public bool Build()
		{
			if (this.PersistXamlToTempFile())
			{
				this.buildWorker = new PartialXamlBuilder.XamlBuildWorker(this.project.DocumentReference.Path, this.tempFilePath);
				this.buildManager.Build(this, null, false, false);
			}
			return true;
		}

		public void BuildCompleted(Microsoft.Expression.Project.Build.BuildResult buildResult)
		{
			if (this.BuildFinished != null)
			{
				PartialXamlBuildFinishedEventArgs partialXamlBuildFinishedEventArg = new PartialXamlBuildFinishedEventArgs(this.projectItem, buildResult, this.tempFilePath, this.xamlText);
				this.BuildFinished(this, partialXamlBuildFinishedEventArg);
			}
		}

		public static object CleanPartialCompileDirectory()
		{
			try
			{
				string str = Microsoft.Expression.Framework.Documents.PathHelper.ResolveCombinedPath(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), PartialXamlBuilder.DefaultXamlCopyDirectory);
				ProjectPathHelper.CleanDirectory(str, true);
			}
			catch (IOException oException)
			{
			}
			return null;
		}

		private bool PersistXamlToTempFile()
		{
			bool flag;
			string str = Microsoft.Expression.Framework.Documents.PathHelper.ResolveCombinedPath(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), PartialXamlBuilder.DefaultXamlCopyDirectory);
			try
			{
				if (!Microsoft.Expression.Framework.Documents.PathHelper.DirectoryExists(str))
				{
					Directory.CreateDirectory(str);
				}
				this.tempFilePath = Microsoft.Expression.Framework.Documents.PathHelper.ResolveCombinedPath(str, this.fileName);
				using (TextWriter streamWriter = new StreamWriter(new FileStream(this.tempFilePath, FileMode.Create)))
				{
					streamWriter.Write(this.xamlText);
				}
				return true;
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				if (!(exception is ArgumentException) && !(exception is IOException) && !(exception is UnauthorizedAccessException) && !(exception is SecurityException))
				{
					throw;
				}
				flag = false;
			}
			return flag;
		}

		public event EventHandler<PartialXamlBuildFinishedEventArgs> BuildFinished;

		private class XamlBuildWorker : IBuildWorker
		{
			private const string MSBuildToolsVersion = "4.0";

			private string originalProjectFilePath;

			private string xamlPath;

			public XamlBuildWorker(string originalProjectFilePath, string xamlPath)
			{
				this.originalProjectFilePath = originalProjectFilePath;
				this.xamlPath = xamlPath;
			}

			private bool AddAssemblyReferences(ProjectRootElement inMemoryProject, Microsoft.Build.Evaluation.Project originalProject)
			{
				foreach (Microsoft.Build.Evaluation.ProjectItem item in originalProject.GetItems("Reference"))
				{
					ProjectItemElement condition = inMemoryProject.AddItem(item.ItemType, item.UnevaluatedInclude);
					condition.Condition = item.Xml.Condition;
					foreach (ProjectMetadata metadatum in item.Metadata)
					{
						condition.AddMetadata(metadatum.Name, metadatum.EvaluatedValue);
					}
				}
				return true;
			}

			private bool AddBuildProperties(ProjectRootElement inMemoryProject, Microsoft.Build.Evaluation.Project originalProject)
			{
				foreach (ProjectPropertyGroupElement propertyGroup in originalProject.Xml.PropertyGroups)
				{
					ProjectPropertyGroupElement condition = inMemoryProject.AddPropertyGroup();
					condition.Condition = propertyGroup.Condition;
					foreach (ProjectPropertyElement property in propertyGroup.Properties)
					{
						ProjectPropertyElement projectPropertyElement = condition.AddProperty(property.Name, property.Value);
						projectPropertyElement.Condition = property.Condition;
					}
				}
				return true;
			}

			private bool AddImports(ProjectRootElement inMemoryProject, Microsoft.Build.Evaluation.Project originalProject)
			{
				foreach (ResolvedImport import in originalProject.Imports)
				{
					inMemoryProject.AddImport(import.ImportedProject.FullPath).Condition = import.ImportingElement.Condition;
				}
				return true;
			}

			private void AddWpfImports(ProjectRootElement currentProject)
			{
				currentProject.AddImport("$(MSBuildToolsPath)\\Microsoft.WinFX.targets");
			}

			public Microsoft.Expression.Project.Build.BuildResult Build(IEnumerable<ILogger> loggers, params string[] targetNames)
			{
				Microsoft.Build.Execution.ProjectInstance projectInstance = new Microsoft.Build.Execution.ProjectInstance(this.CreateProject());
				string[] strArrays = new string[] { "ResolveReferences", "MarkupCompilePass1" };
				if (projectInstance.Build(strArrays, loggers))
				{
					return Microsoft.Expression.Project.Build.BuildResult.Succeeded;
				}
				return Microsoft.Expression.Project.Build.BuildResult.Failed;
			}

			private ProjectRootElement CreateProject()
			{
				Microsoft.Build.Evaluation.Project project = null;
				try
				{
					project = Microsoft.Expression.Project.Build.BuildManager.GetProject(DocumentReference.Create(this.originalProjectFilePath));
				}
				catch (ArgumentException argumentException)
				{
				}
				ProjectRootElement projectRootElement = ProjectRootElement.Create();
				projectRootElement.ToolsVersion = "4.0";
				this.AddBuildProperties(projectRootElement, project);
				projectRootElement.AddItemGroup();
				projectRootElement.AddPropertyGroup();
				projectRootElement.AddProperty("IntermediateOutputPath", Microsoft.Expression.Framework.Documents.PathHelper.GetDirectoryNameOrRoot(this.xamlPath));
				this.AddAssemblyReferences(projectRootElement, project);
				this.AddImports(projectRootElement, project);
				if (!this.IsSilverlightApplication(project))
				{
					this.AddWpfImports(projectRootElement);
				}
				projectRootElement.AddItem("Page", this.xamlPath);
				string str = Microsoft.Expression.Framework.Documents.PathHelper.ResolveCombinedPath(Microsoft.Expression.Framework.Documents.PathHelper.GetDirectoryNameOrRoot(this.xamlPath), "MarkupCompileAssembly.proj");
				projectRootElement.Save(str);
				return projectRootElement;
			}

			private bool IsSilverlightApplication(Microsoft.Build.Evaluation.Project project)
			{
				if (project == null)
				{
					return false;
				}
				string propertyValue = project.GetPropertyValue("SilverlightApplication");
				if (propertyValue == null)
				{
					return false;
				}
				return propertyValue.Equals("true", StringComparison.OrdinalIgnoreCase);
			}
		}
	}
}