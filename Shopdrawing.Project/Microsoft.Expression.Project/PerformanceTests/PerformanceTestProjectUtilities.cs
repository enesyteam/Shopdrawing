using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.ServiceExtensions;
using Microsoft.Expression.Project.ServiceExtensions.Documents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project.PerformanceTests
{
	public sealed class PerformanceTestProjectUtilities
	{
		private IServiceProvider serviceProvider;

		public PerformanceTestProjectUtilities(IServiceProvider serviceProvider)
		{
			this.serviceProvider = serviceProvider;
		}

		public void CloseAll()
		{
			if ((ISolutionManagement)this.serviceProvider.ProjectManager().CurrentSolution != null)
			{
				((ISolutionManagement)this.serviceProvider.ProjectManager().CurrentSolution).CloseAllProjects();
			}
		}

		public void CreateProject(string projectPath, string projectName)
		{
			IProjectTemplate projectTemplate = (
				from template in this.serviceProvider.ProjectManager().TemplateManager.ProjectTemplates
				where template.Identifier.Equals("Microsoft.Blend.WPFApplication")
				select template).First<IProjectTemplate>();
			IProject project = this.serviceProvider.ProjectManager().CreateProjectTemplate(projectPath, projectName, projectTemplate, null).FirstOrDefault<INamedProject>() as IProject;
			if (project != null)
			{
				IDocumentType item = this.serviceProvider.DocumentTypes()[DocumentTypeNamesHelper.Xaml];
				ICodeDocumentType codeDocumentType = this.serviceProvider.DocumentTypes().CSharpDocumentType();
				List<DocumentCreationInfo> documentCreationInfos = new List<DocumentCreationInfo>();
				DocumentCreationInfo documentCreationInfo = new DocumentCreationInfo()
				{
					DocumentType = item,
					TargetPath = Path.Combine(project.ProjectRoot.Path, "TestUserControl.xaml")
				};
				documentCreationInfos.Add(documentCreationInfo);
				DocumentCreationInfo documentCreationInfo1 = new DocumentCreationInfo()
				{
					DocumentType = codeDocumentType,
					TargetPath = Path.Combine(project.ProjectRoot.Path, "TestUserControl.xaml.cs")
				};
				documentCreationInfos.Add(documentCreationInfo1);
				project.AddItems(documentCreationInfos);
				project.StartupItem.OpenView(true);
			}
		}

		public void OpenProject(string path)
		{
			this.serviceProvider.ProjectManager().OpenSolution(DocumentReference.Create(path), true, true);
		}

		public void SaveActiveProjectItems()
		{
			((ISolutionManagement)this.serviceProvider.ProjectManager().CurrentSolution).Save(false);
		}
	}
}