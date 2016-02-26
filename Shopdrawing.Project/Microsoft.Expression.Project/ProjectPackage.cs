using Microsoft.Expression.Extensibility;
using Microsoft.Expression.Extensibility.Project;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.Configuration;
using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Expression.Project.ServiceExtensions;
using Microsoft.Expression.Project.UserInterface;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;

namespace Microsoft.Expression.Project
{
	public sealed class ProjectPackage : IPackage
	{
		private IServices services;

		private IDocumentType assemblyDocumentType;

		private IDocumentType projectReferenceDocumentType;

		private IDocumentType folderDocumentType;

		private IDocumentType comReferenceDocumentType;

		private IDocumentType cursorDocumentType;

		private IDocumentType deepZoomDocumentType;

		private AssemblyService assemblyService;

		private AssemblyLoggingService assemblyLoggingService;

		private IProjectType websiteProjectType;

		private IProjectType webApplicationProjectType;

		private ProjectSystemOptionsPage projectSystemOptionsPage;

		private BlendAssemblyResolver blendAssemblyResolver;

		private BlendSdkAssemblyResolver blendSdkAssemblyResolver;

		private SolutionService solutionService;

		public ProjectPackage()
		{
		}

		private void Current_SessionEnding(object sender, SessionEndingCancelEventArgs args)
		{
			if (!this.services.GetService<IProjectManager>().CloseSolution(true))
			{
				args.Cancel = true;
			}
		}

		public void Load(IServices services)
		{
			this.services = services;
			ICommandService service = (ICommandService)this.services.GetService(typeof(ICommandService));
			IExpressionMefHostingService expressionMefHostingService = this.services.GetService<IExpressionMefHostingService>();
			IDocumentTypeManager documentTypeManager = new DocumentTypeManager(new UnknownDocumentType());
			this.services.AddService(typeof(IDocumentTypeManager), documentTypeManager);
			IProjectTypeManager projectTypeManager = new ProjectTypeManager();
			this.services.AddService(typeof(IProjectTypeManager), projectTypeManager);
			IConfigurationService configurationService = this.services.GetService<IConfigurationService>();
			ProjectManager projectManager = new ProjectManager(this.services, configurationService["ProjectManager"]);
			this.services.AddService(typeof(IProjectManager), projectManager);
			service.AddTarget(projectManager);
			this.services.AddService(typeof(IExternalChanges), projectManager);
			this.solutionService = new SolutionService(projectManager);
			this.services.AddService(typeof(ISolutionService), this.solutionService);
			this.assemblyLoggingService = new AssemblyLoggingService(configurationService.ConfigurationDirectoryPath);
			this.services.AddService(typeof(IAssemblyLoggingService), this.assemblyLoggingService);
			IProjectAdapterService projectAdapterService = new ProjectAdapterService(this.services);
			this.services.AddService(typeof(IProjectAdapterService), projectAdapterService);
			if (expressionMefHostingService != null)
			{
				expressionMefHostingService.AddInternalPart(projectAdapterService);
				expressionMefHostingService.AddInternalPart(this.solutionService);
			}
			IOptionsDialogService optionsDialogService = this.services.GetService<IOptionsDialogService>();
			this.projectSystemOptionsPage = new ProjectSystemOptionsPage(projectManager, this.assemblyLoggingService);
			optionsDialogService.OptionsPages.Add(this.projectSystemOptionsPage);
			this.assemblyDocumentType = new AssemblyDocumentType();
			documentTypeManager.Register(this.assemblyDocumentType);
			this.projectReferenceDocumentType = new ProjectReferenceDocumentType();
			documentTypeManager.Register(this.projectReferenceDocumentType);
			this.folderDocumentType = new FolderDocumentType();
			documentTypeManager.Register(this.folderDocumentType);
			this.comReferenceDocumentType = new ComReferenceDocumentType();
			documentTypeManager.Register(this.comReferenceDocumentType);
			this.cursorDocumentType = new CursorDocumentType();
			documentTypeManager.Register(this.cursorDocumentType);
			this.deepZoomDocumentType = new DeepZoomDocumentType();
			documentTypeManager.Register(this.deepZoomDocumentType);
			this.websiteProjectType = new WebsiteProjectType();
			projectTypeManager.Register(this.websiteProjectType);
			this.webApplicationProjectType = new WebApplicationProjectType();
			projectTypeManager.Register(this.webApplicationProjectType);
			this.assemblyService = new AssemblyService(this.services);
			this.services.AddService(typeof(IAssemblyService), this.assemblyService);
			this.services.AddService(typeof(ISatelliteAssemblyResolver), this.assemblyService);
			this.blendSdkAssemblyResolver = new BlendSdkAssemblyResolver();
			this.assemblyService.RegisterLibraryResolver(this.blendSdkAssemblyResolver);
			this.blendAssemblyResolver = new BlendAssemblyResolver();
			this.assemblyService.RegisterLibraryResolver(this.blendAssemblyResolver);
			Microsoft.Expression.Framework.UserInterface.IWindowService windowService = this.services.GetService<Microsoft.Expression.Framework.UserInterface.IWindowService>();
			windowService.Closing += new CancelEventHandler(this.WindowManager_Closing);
			Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new DispatcherOperationCallback((object o) => {
				Application.Current.SessionEnding += new SessionEndingCancelEventHandler(this.Current_SessionEnding);
				return null;
			}), null);
			UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.SystemIdle, new Action(this.assemblyService.AssemblyCache.Clean));
		}

		public void Unload()
		{
			Microsoft.Expression.Framework.UserInterface.IWindowService service = this.services.GetService<Microsoft.Expression.Framework.UserInterface.IWindowService>();
			service.Closing -= new CancelEventHandler(this.WindowManager_Closing);
			Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new DispatcherOperationCallback((object o) => {
				Application.Current.SessionEnding -= new SessionEndingCancelEventHandler(this.Current_SessionEnding);
				return null;
			}), null);
			ICommandService commandService = this.services.GetService<ICommandService>();
			IProjectManager projectManager = this.services.GetService<IProjectManager>();
			projectManager.CloseSolution();
			ProjectManager projectManager1 = (ProjectManager)projectManager;
			projectManager1.UpdateConfiguration();
			commandService.RemoveTarget(projectManager1);
			IDocumentTypeManager documentTypeManager = this.services.GetService<IDocumentTypeManager>();
			documentTypeManager.Unregister(this.assemblyDocumentType);
			documentTypeManager.Unregister(this.projectReferenceDocumentType);
			documentTypeManager.Unregister(this.folderDocumentType);
			documentTypeManager.Unregister(this.comReferenceDocumentType);
			documentTypeManager.Unregister(this.cursorDocumentType);
			documentTypeManager.Unregister(this.deepZoomDocumentType);
			IProjectTypeManager projectTypeManager = this.services.GetService<IProjectTypeManager>();
			projectTypeManager.Unregister(this.websiteProjectType);
			projectTypeManager.Unregister(this.webApplicationProjectType);
			this.services.GetService<IOptionsDialogService>().OptionsPages.Remove(this.projectSystemOptionsPage);
			this.assemblyLoggingService.Unload();
			this.services.RemoveService(typeof(IAssemblyLoggingService));
			this.services.RemoveService(typeof(IProjectTypeManager));
			this.services.RemoveService(typeof(IDocumentTypeManager));
			this.services.RemoveService(typeof(IProjectManager));
			this.services.RemoveService(typeof(ISolutionService));
			this.services.AssemblyService().UnregisterLibraryResolver(this.blendSdkAssemblyResolver);
			this.services.AssemblyService().UnregisterLibraryResolver(this.blendAssemblyResolver);
			this.services.RemoveService(typeof(IAssemblyService));
			this.services.RemoveService(typeof(ISatelliteAssemblyResolver));
			this.assemblyService.Dispose();
		}

		private void WindowManager_Closing(object sender, CancelEventArgs args)
		{
			if (!this.services.GetService<IProjectManager>().CloseSolution(true))
			{
				args.Cancel = true;
			}
		}
	}
}