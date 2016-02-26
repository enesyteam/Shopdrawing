using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.Configuration;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.Build;
using Microsoft.Expression.Project.Licensing;
using Microsoft.Expression.Project.ServiceExtensions;
using Microsoft.Expression.Project.UserInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Expression.Project.Commands
{
	internal abstract class BaseNewProjectCommand : ProjectCommand
	{
		private IConfigurationObject configurationObject;

		private readonly static string LastProjectTemplateCreated;

		private readonly static string LastProjectLanguageCreated;

		private readonly static string LastProjectTargetFrameworkCreated;

		private readonly static string LastProjectCategoryTreeVisible;

		private readonly static string LastProjectCategoryTreeItem;

		private string projectName;

		private string projectFolder;

		private string projectFilter;

		private IProjectTemplate projectTemplate;

		private ProjectPropertyValue targetFrameworkVersion;

		protected abstract bool CreateNewSolution
		{
			get;
		}

		public override bool IsEnabled
		{
			get
			{
				if (!base.IsEnabled)
				{
					return false;
				}
				return !BuildManager.Building;
			}
		}

		protected abstract string NewProjectPath
		{
			get;
		}

		static BaseNewProjectCommand()
		{
			BaseNewProjectCommand.LastProjectTemplateCreated = "LastProjectTemplateCreated";
			BaseNewProjectCommand.LastProjectLanguageCreated = "LastProjectFilterCreated";
			BaseNewProjectCommand.LastProjectTargetFrameworkCreated = "LastProjectTargetFrameworkCreated";
			BaseNewProjectCommand.LastProjectCategoryTreeVisible = "LastProjectCategoryTreeVisible";
			BaseNewProjectCommand.LastProjectCategoryTreeItem = "LastProjectCategoryTreeItem";
		}

		public BaseNewProjectCommand(IServiceProvider serviceProvider, IConfigurationObject configurationObject) : base(serviceProvider)
		{
			this.configurationObject = configurationObject;
		}

		protected IEnumerable<INamedProject> CreateNewProject()
		{
			IEnumerable<INamedProject> namedProjects;
			LicensingHelper.SuppressDialogForSession();
			using (IDisposable disposable = TemporaryCursor.SetWaitCursor())
			{
				List<TemplateArgument> templateArguments = new List<TemplateArgument>();
				if (this.targetFrameworkVersion != null)
				{
					templateArguments.Add(new TemplateArgument("targetframeworkversion", this.targetFrameworkVersion.Value));
					templateArguments.Add(new TemplateArgument("clientprofile", "True"));
				}
				IEnumerable<INamedProject> namedProjects1 = this.ProjectManager().CreateProjectTemplate(this.projectFolder, this.projectName, this.projectTemplate, templateArguments);
				if (this.targetFrameworkVersion != null)
				{
					this.configurationObject.SetProperty(BaseNewProjectCommand.LastProjectTargetFrameworkCreated, this.targetFrameworkVersion.Value);
				}
				if (this.projectTemplate != null)
				{
					this.configurationObject.SetProperty(BaseNewProjectCommand.LastProjectTemplateCreated, this.projectTemplate.Identifier);
				}
				this.configurationObject.SetProperty(BaseNewProjectCommand.LastProjectLanguageCreated, this.projectFilter);
				namedProjects = namedProjects1;
			}
			return namedProjects;
		}

		protected bool GetNewProjectData()
		{
			IProjectManager projectManager = this.ProjectManager();
			CreateProjectDialog createProjectDialog = new CreateProjectDialog(projectManager.TemplateManager.ProjectTemplates, projectManager.TemplateManager, this.DisplayName, this.NewProjectPath, this.CreateNewSolution, base.Services)
			{
				IsCategoryTreeVisible = (bool)this.configurationObject.GetProperty(BaseNewProjectCommand.LastProjectCategoryTreeVisible, true),
				SelectedCategoryFullName = (string)this.configurationObject.GetProperty(BaseNewProjectCommand.LastProjectCategoryTreeItem, "")
			};
			string property = (string)this.configurationObject.GetProperty(BaseNewProjectCommand.LastProjectLanguageCreated, "");
			if (!string.IsNullOrEmpty(property))
			{
				createProjectDialog.Filter = property;
			}
			IProjectTemplate projectTemplate = projectManager.TemplateManager.ProjectTemplates.FirstOrDefault<IProjectTemplate>((IProjectTemplate template) => template.Identifier == (string)this.configurationObject.GetProperty(BaseNewProjectCommand.LastProjectTemplateCreated, ""));
			if (projectTemplate == null)
			{
				createProjectDialog.ProjectTemplateListBox.Items.MoveCurrentToFirst();
			}
			else
			{
				createProjectDialog.ProjectTemplate = projectTemplate;
			}
			string str = (string)this.configurationObject.GetProperty(BaseNewProjectCommand.LastProjectTargetFrameworkCreated, "");
			if (!string.IsNullOrEmpty(str))
			{
				createProjectDialog.TargetFrameworkVersion = ProjectPropertyInfo.CreatePropertyValue("TargetFrameworkVersion", str);
			}
			bool? nullable = createProjectDialog.ShowDialog();
			this.configurationObject.SetProperty(BaseNewProjectCommand.LastProjectCategoryTreeVisible, createProjectDialog.IsCategoryTreeVisible);
			this.configurationObject.SetProperty(BaseNewProjectCommand.LastProjectCategoryTreeItem, createProjectDialog.SelectedCategoryFullName);
			if (!nullable.GetValueOrDefault(false))
			{
				return false;
			}
			string str1 = null;
			if (createProjectDialog.ProjectTemplate == null || createProjectDialog.ProjectTemplate.IsPlatformSupported(out str1))
			{
				this.projectTemplate = createProjectDialog.ProjectTemplate;
				this.targetFrameworkVersion = createProjectDialog.TargetFrameworkVersion;
				this.projectFolder = createProjectDialog.ProjectPath;
				this.projectName = createProjectDialog.ProjectName;
				this.projectFilter = createProjectDialog.Filter;
				return true;
			}
			MessageBoxArgs messageBoxArg = new MessageBoxArgs()
			{
				Message = str1,
				Button = MessageBoxButton.OK,
				Image = MessageBoxImage.Hand,
				AutomationId = "UnknownProjectTypeErrorDialog"
			};
			MessageBoxArgs messageBoxArg1 = messageBoxArg;
			base.Services.MessageDisplayService().ShowMessage(messageBoxArg1);
			return false;
		}
	}
}