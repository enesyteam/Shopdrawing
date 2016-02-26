// Decompiled with JetBrains decompiler
// Type: Shopdrawing.App.FrameworkPackage
// Assembly: Shopdrawing.App, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: DDD2F1CF-BB6D-4068-A4D9-DDBDD16D6739
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Shopdrawing.App.dll

using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.Extensibility;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.Configuration;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.Feedback;
using Microsoft.Expression.Framework.Importers;
using Microsoft.Expression.Framework.Scheduler;
using Microsoft.Expression.Framework.UserInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace Shopdrawing.App
{
  public class FrameworkPackage : CommandTarget, IPackage
  {
    private IServices services;
    private ShellOptionsPage shellOptionsPage;
    private ResourceDictionary frameworkIcons;
    private bool suppressViewUI;

    public FrameworkPackage()
      : this(false)
    {
    }

    public FrameworkPackage(bool suppressViewUI)
    {
      this.suppressViewUI = suppressViewUI;
    }

    public void Load(IServices services)
    {
      this.services = services;
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.FrameworkPackageLoad);
      UIThreadDispatcher.InitializeInstance();
      IExpressionMefHostingService mefHostingService = (IExpressionMefHostingService) new ExpressionMefHostingService(services);
      this.services.AddService(typeof (IExpressionMefHostingService), (object) mefHostingService);
      mefHostingService.AddInternalPart((object) mefHostingService);
      IMessageDisplayService messageDisplayService = (IMessageDisplayService) new MessageDisplayService(services.GetService<IExpressionInformationService>());
      this.services.AddService(typeof (IMessageDisplayService), (object) messageDisplayService);
      mefHostingService.AddInternalPart((object) this.services.GetService<IMessageDisplayService>());
      PerformanceUtility.MarkInterimStep(PerformanceEvent.FrameworkPackageLoad, "Create SchedulingService");
      this.services.AddService(typeof (ISchedulingService), (object) new SchedulingService());
      mefHostingService.AddInternalPart((object) this.services.GetService<ISchedulingService>());
      string str = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Microsoft\\Expression\\Blend 4");
      PerformanceUtility.MarkInterimStep(PerformanceEvent.FrameworkPackageLoad, "Create ConfigurationService");
      IConfigurationService configurationService = (IConfigurationService) new ConfigurationService(str);
      this.services.AddService(typeof (IConfigurationService), (object) configurationService);
      PerformanceUtility.MarkInterimStep(PerformanceEvent.FrameworkPackageLoad, "Create OptionsDialogService");
      IOptionsDialogService optionsDialogService = (IOptionsDialogService) new OptionsDialogService(configurationService);
      this.services.AddService(typeof (IOptionsDialogService), (object) optionsDialogService);
      PerformanceUtility.MarkInterimStep(PerformanceEvent.FrameworkPackageLoad, "Create CommandService");
      ICommandService commandService = (ICommandService) new CommandService(services.GetService<IFeedbackService>());
      this.services.AddService(typeof (ICommandService), (object) commandService);
      PerformanceUtility.MarkInterimStep(PerformanceEvent.FrameworkPackageLoad, "Add Commands");
      this.AddCommand("Application_Exit", (ICommand) new FrameworkPackage.ExitCommand());
      this.AddCommand("Application_Options", (ICommand) new FrameworkPackage.OptionsCommand(optionsDialogService));
      PerformanceUtility.MarkInterimStep(PerformanceEvent.FrameworkPackageLoad, "Create DocumentService");
      DocumentService documentService = (DocumentService) new BlendDocumentService(commandService, messageDisplayService);
      this.services.AddService(typeof (IDocumentService), (object) documentService);
      this.services.AddService(typeof (IViewService), (object) documentService);
      ICommandBarService commandBarService = (ICommandBarService) new CommandBarService(commandService);
      this.services.AddService(typeof (ICommandBarService), (object) commandBarService);
      PerformanceUtility.MarkInterimStep(PerformanceEvent.FrameworkPackageLoad, "Create WorkspaceService");
      WorkspaceService workspaceService = new WorkspaceService(configurationService["WorkspaceService"], Path.Combine(str, "Workspaces"), (IEnumerable<string>) new string[2]
      {
        StringTable.DesignWorkspaceName,
        StringTable.AnimationWorkspaceName
      }, (IEnumerable<Uri>) new Uri[2]
      {
        new Uri("pack://application:,,,/Shopdrawing.Application;Component/Resources/Workspaces/Design.xaml", UriKind.Absolute),
        new Uri("pack://application:,,,/Shopdrawing.Application;Component/Resources/Workspaces/Animation.xaml", UriKind.Absolute)
      }, messageDisplayService);
      commandService.AddTarget((ICommandTarget) workspaceService);
      this.services.AddService(typeof (IWorkspaceService), (object) workspaceService);
      this.frameworkIcons = new ResourceDictionary();
      this.frameworkIcons.Source = new Uri("pack://application:,,,/Microsoft.Expression.Framework;Component/Resources/Icons/FrameworkIcons.xaml", UriKind.Absolute);
      List<Theme> list = new List<Theme>(2);
      Uri resourceDictionaryUriToMerge1 = new Uri("pack://application:,,,/Shopdrawing.Application;Component/Resources/BlendDarkTheme.xaml", UriKind.Absolute);
      list.Add(new Theme(StringTable.ThemeNameExpressionDark, "Resources\\UserInterface\\ExpressionDark.xaml", resourceDictionaryUriToMerge1));
      Uri resourceDictionaryUriToMerge2 = new Uri("pack://application:,,,/Shopdrawing.Application;Component/Resources/BlendLightTheme.xaml", UriKind.Absolute);
      list.Add(new Theme(StringTable.ThemeNameExpressionLight, "Resources\\UserInterface\\ExpressionLight.xaml", resourceDictionaryUriToMerge2));
      PerformanceUtility.MarkInterimStep(PerformanceEvent.FrameworkPackageLoad, "Create WindowService");
      WindowService windowService = new WindowService(configurationService["WindowService"], commandBarService, commandService, (IViewService) documentService, (IWorkspaceService) workspaceService, messageDisplayService, services.GetService<IFeedbackService>(), services.GetService<IExpressionInformationService>().MainWindowRootElement, this.frameworkIcons, list.AsReadOnly(), this.suppressViewUI);
      this.services.AddService(typeof (Microsoft.Expression.Framework.UserInterface.IWindowService), (object) windowService);
      this.services.AddService(typeof (IOrderedViewProvider), (object) windowService);
      workspaceService.SetPaletteRegistry(windowService.PaletteRegistry);
      PerformanceUtility.MarkInterimStep(PerformanceEvent.FrameworkPackageLoad, "Register CommandTargets");
      commandService.AddTarget((ICommandTarget) this);
      commandService.AddTarget((ICommandTarget) documentService);
      this.services.AddService(typeof (IMessageLoggingService), (object) new FrameworkPackage.NullMessageLoggingService());
      this.services.AddService(typeof (IErrorService), (object) new FrameworkPackage.ErrorService());
      this.services.AddService(typeof (IImporterService), (object) new ImporterService(configurationService["ImporterService"], Path.Combine(str, "ImportersDataStore")));
      this.shellOptionsPage = new ShellOptionsPage((Microsoft.Expression.Framework.UserInterface.IWindowService) windowService);
      optionsDialogService.OptionsPages.Add((IOptionsPage) this.shellOptionsPage);
      Dialog.ServiceProvider = (IServiceProvider) services;
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.FrameworkPackageLoad);
    }

    public void Unload()
    {
      IOptionsDialogService service1 = this.services.GetService<IOptionsDialogService>();
      IConfigurationService service2 = this.services.GetService<IConfigurationService>();
      Microsoft.Expression.Framework.UserInterface.IWindowService service3 = this.services.GetService<Microsoft.Expression.Framework.UserInterface.IWindowService>();
      service1.OptionsPages.Remove((IOptionsPage) this.shellOptionsPage);
      service2["WindowService"].SetProperty("ActiveTheme", (object) service3.ActiveTheme, (object) service3.Themes[0].Name);
      service2.Save();
      this.services.RemoveService(typeof (Microsoft.Expression.Framework.UserInterface.IWindowService));
      this.services.RemoveService(typeof (IDocumentService));
      this.services.RemoveService(typeof (IViewService));
      this.services.RemoveService(typeof (ICommandService));
      this.services.RemoveService(typeof (IConfigurationService));
      this.services.RemoveService(typeof (IOptionsDialogService));
      this.services.RemoveService(typeof (IMessageDisplayService));
      this.services.RemoveService(typeof (IImporterService));
    }

    public static void RegisterCommandLineService(IServices services)
    {
      ICommandLineService commandLineService = (ICommandLineService) new CommandLineService();
      services.AddService(typeof (ICommandLineService), (object) commandLineService);
    }

    private sealed class NullMessageLoggingService : IMessageLoggingService
    {
      public void Clear()
      {
      }

      public void Write(string text)
      {
      }

      public void WriteLine(string text)
      {
      }
    }

    private sealed class ErrorService : IErrorService
    {
      private ErrorTaskCollection errors = new ErrorTaskCollection();

      public IErrorTaskCollection Errors
      {
        get
        {
          return (IErrorTaskCollection) this.errors;
        }
      }

      public void DisplayErrors()
      {
      }
    }

    private sealed class ExitCommand : Command
    {
      public override void Execute()
      {
          Window mainWindow = System.Windows.Application.Current.MainWindow;
        if (mainWindow == null)
          return;
        mainWindow.Close();
      }
    }

    private sealed class OptionsCommand : Command
    {
      private IOptionsDialogService optionsDialogService;

      public OptionsCommand(IOptionsDialogService optionsDialogService)
      {
        this.optionsDialogService = optionsDialogService;
      }

      public override void Execute()
      {
        bool? nullable = new OptionsDialog(this.optionsDialogService).ShowDialog();
        if (nullable.HasValue && nullable.Value)
          this.optionsDialogService.Commit();
        else
          this.optionsDialogService.Cancel();
      }
    }
  }
}
