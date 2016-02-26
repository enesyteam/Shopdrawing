using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.Configuration;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.IssueTracking;
using Microsoft.Expression.Framework.SourceControl;
using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Expression.Project;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project.ServiceExtensions
{
	public static class ServiceExtensions
	{
		public static IAssemblyLoggingService AssemblyLoggingService(this IServiceProvider source)
		{
			return source.GetService<IAssemblyLoggingService>();
		}

		public static IAssemblyService AssemblyService(this IServiceProvider source)
		{
			return source.GetService<IAssemblyService>();
		}

		public static ICommandBarService CommandBarService(this IServiceProvider source)
		{
			return source.GetService<ICommandBarService>();
		}

		public static ICommandLineService CommandLineService(this IServiceProvider source)
		{
			return source.GetService<ICommandLineService>();
		}

		public static ICommandService CommandService(this IServiceProvider source)
		{
			return source.GetService<ICommandService>();
		}

		public static IDocumentService DocumentService(this IServiceProvider source)
		{
			return source.GetService<IDocumentService>();
		}

		public static IDocumentTypeManager DocumentTypeManager(this IServiceProvider source)
		{
			return source.GetService<IDocumentTypeManager>();
		}

		public static IErrorService ErrorService(this IServiceProvider source)
		{
			return source.GetService<IErrorService>();
		}

		public static IExpressionInformationService ExpressionInformationService(this IServiceProvider source)
		{
			return source.GetService<IExpressionInformationService>();
		}

		public static IExternalChanges ExternalChanges(this IServiceProvider source)
		{
			return source.GetService<IExternalChanges>();
		}

		public static TService GetService<TService>(this IServiceProvider source)
		where TService : class
		{
			return (TService)(source.GetService(typeof(TService)) as TService);
		}

		public static IIssueTrackingProvider IssueTrackingProvider(this IServiceProvider source)
		{
			if (source.IssueTrackingService() == null)
			{
				return null;
			}
			return source.IssueTrackingService().ActiveProvider;
		}

		public static IIssueTrackingService IssueTrackingService(this IServiceProvider source)
		{
			return source.GetService<IIssueTrackingService>();
		}

		public static IExpressionMefHostingService MefHostingService(this IServiceProvider source)
		{
			return source.GetService<IExpressionMefHostingService>();
		}

		public static IMessageDisplayService MessageDisplayService(this IServiceProvider source)
		{
			return source.GetService<IMessageDisplayService>();
		}

		public static IMessageLoggingService MessageLoggingService(this IServiceProvider source)
		{
			return source.GetService<IMessageLoggingService>();
		}

		public static IOrderedViewProvider OrderedViewProvider(this IServiceProvider source)
		{
			return source.GetService<IOrderedViewProvider>();
		}

		public static IOutOfBrowserDeploymentService OutOfBrowserDeploymentService(this IServiceProvider source)
		{
			return source.GetService<IOutOfBrowserDeploymentService>();
		}

		internal static IProjectAdapterService ProjectAdapterService(this IServiceProvider source)
		{
			return source.GetService<IProjectAdapterService>();
		}

		public static IConfigurationObject ProjectConfigurationObject(this IServiceProvider source)
		{
			return source.GetService<IConfigurationService>()["Project Manager"];
		}

		public static IProjectManager ProjectManager(this IServiceProvider source)
		{
			return source.GetService<IProjectManager>();
		}

		public static IProjectTypeManager ProjectTypeManager(this IServiceProvider source)
		{
			return source.GetService<IProjectTypeManager>();
		}

		public static IPrototypingProjectService PrototypingProjectService(this IServiceProvider source)
		{
			return source.GetService<IPrototypingProjectService>();
		}

		public static ISourceControlProvider SourceControlProvider(this IServiceProvider source)
		{
			if (source.SourceControlService() == null)
			{
				return null;
			}
			return source.SourceControlService().ActiveProvider;
		}

		public static ISourceControlService SourceControlService(this IServiceProvider source)
		{
			return source.GetService<ISourceControlService>();
		}

		public static IViewService ViewService(this IServiceProvider source)
		{
			return source.GetService<IViewService>();
		}

		public static IWindowService WindowService(this IServiceProvider source)
		{
			return source.GetService<IWindowService>();
		}
	}
}