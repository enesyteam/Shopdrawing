// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Extensions.ServiceProvider.ServiceProviderExtensions
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Configuration;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.Feedback;
using Microsoft.Expression.Framework.Scheduler;
using Microsoft.Expression.Framework.SourceControl;
using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Expression.Framework.WebServer;
using System;

namespace Microsoft.Expression.Framework.Extensions.ServiceProvider
{
  public static class ServiceProviderExtensions
  {
    public static TService GetService<TService>(this IServiceProvider source) where TService : class
    {
      return source.GetService(typeof (TService)) as TService;
    }

    public static IConfigurationService ConfigurationService(this IServiceProvider source)
    {
      return ServiceProviderExtensions.GetService<IConfigurationService>(source);
    }

    public static ICommandBarService CommandBarService(this IServiceProvider source)
    {
      return ServiceProviderExtensions.GetService<ICommandBarService>(source);
    }

    public static ICommandLineService CommandLineService(this IServiceProvider source)
    {
      return ServiceProviderExtensions.GetService<ICommandLineService>(source);
    }

    public static IDocumentService DocumentService(this IServiceProvider source)
    {
      return ServiceProviderExtensions.GetService<IDocumentService>(source);
    }

    public static IWindowService WindowService(this IServiceProvider source)
    {
      return ServiceProviderExtensions.GetService<IWindowService>(source);
    }

    public static IWorkspaceService WorkspaceService(this IServiceProvider source)
    {
      return ServiceProviderExtensions.GetService<IWorkspaceService>(source);
    }

    public static IViewService ViewService(this IServiceProvider source)
    {
      return ServiceProviderExtensions.GetService<IViewService>(source);
    }

    public static ISchedulingService SchedulingService(this IServiceProvider source)
    {
      return ServiceProviderExtensions.GetService<ISchedulingService>(source);
    }

    public static IMessageDisplayService MessageDisplayService(this IServiceProvider source)
    {
      return ServiceProviderExtensions.GetService<IMessageDisplayService>(source);
    }

    public static IMessageLoggingService MessageLoggingService(this IServiceProvider source)
    {
      return ServiceProviderExtensions.GetService<IMessageLoggingService>(source);
    }

    public static IErrorService ErrorService(this IServiceProvider source)
    {
      return ServiceProviderExtensions.GetService<IErrorService>(source);
    }

    [CLSCompliant(false)]
    public static ILicenseService LicenseService(this IServiceProvider source)
    {
      return ServiceProviderExtensions.GetService<ILicenseService>(source);
    }

    public static IFeedbackService FeedbackService(this IServiceProvider source)
    {
      return ServiceProviderExtensions.GetService<IFeedbackService>(source);
    }

    public static ISourceControlService SourceControlService(this IServiceProvider source)
    {
      return ServiceProviderExtensions.GetService<ISourceControlService>(source);
    }

    public static IWebServerService WebServerService(this IServiceProvider source)
    {
      return ServiceProviderExtensions.GetService<IWebServerService>(source);
    }
  }
}
