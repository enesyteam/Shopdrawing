using Microsoft.Expression.Framework.Configuration;
using Microsoft.Expression.Project;
using System;

namespace Microsoft.Expression.Project.UserInterface
{
	internal sealed class ProjectSystemOptionsPage : IOptionsPage
	{
		private Microsoft.Expression.Project.UserInterface.ProjectSystemOptionsModel projectSystemOptionsModel;

		private ProjectSystemOptionsControl projectSystemOptionsControl;

		private IConfigurationObject configurationObject;

		private IProjectManager projectManager;

		private IAssemblyLoggingService assemblyLoggingService;

		public object Content
		{
			get
			{
				if (this.projectSystemOptionsControl == null)
				{
					this.projectSystemOptionsControl = new ProjectSystemOptionsControl(this.ProjectSystemOptionsModel);
				}
				return this.projectSystemOptionsControl;
			}
		}

		public string Name
		{
			get
			{
				return "ProjectSystemOptions";
			}
		}

		public Microsoft.Expression.Project.UserInterface.ProjectSystemOptionsModel ProjectSystemOptionsModel
		{
			get
			{
				if (this.projectSystemOptionsModel == null)
				{
					this.projectSystemOptionsModel = new Microsoft.Expression.Project.UserInterface.ProjectSystemOptionsModel();
					this.projectSystemOptionsModel.Load(this.configurationObject);
					this.projectSystemOptionsModel.LogAssemblyLoading = this.assemblyLoggingService.IsEnabled;
				}
				return this.projectSystemOptionsModel;
			}
		}

		public string Title
		{
			get
			{
				return StringTable.ProjectSystemOptionsPageTitle;
			}
		}

		public ProjectSystemOptionsPage(IProjectManager projectManager, IAssemblyLoggingService assemblyLoggingService)
		{
			this.projectManager = projectManager;
			this.assemblyLoggingService = assemblyLoggingService;
		}

		private void Apply()
		{
			this.projectManager.OptionsModel.Load(this.configurationObject);
		}

		public void Cancel()
		{
			this.projectSystemOptionsModel = null;
			this.projectSystemOptionsControl = null;
		}

		public void Commit()
		{
			if (this.ProjectSystemOptionsModel != null)
			{
				this.configurationObject.Clear();
				this.ProjectSystemOptionsModel.Save(this.configurationObject);
				this.assemblyLoggingService.IsEnabled = this.ProjectSystemOptionsModel.LogAssemblyLoading;
				this.projectSystemOptionsModel = null;
				this.projectSystemOptionsControl = null;
				this.Apply();
			}
		}

		public void Load(IConfigurationObject value)
		{
			this.configurationObject = value;
			this.Apply();
		}
	}
}