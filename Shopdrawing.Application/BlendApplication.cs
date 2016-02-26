// Decompiled with JetBrains decompiler
// Type: Shopdrawing.App.BlendApplication
// Assembly: Shopdrawing.App, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: DDD2F1CF-BB6D-4068-A4D9-DDBDD16D6739
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Shopdrawing.App.dll

using Microsoft.Expression.Code;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.Extensibility;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.Configuration;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.Feedback;
using Microsoft.Expression.Framework.Globalization;
using Microsoft.Expression.Framework.Licenses;
using Microsoft.Expression.Framework.SourceControl;
using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Expression.Framework.WebServer;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Shopdrawing.App
{
  public class BlendApplication : ExpressionApplication
  {
    private const string DoNotWarnAboutMefCompositionException = "DoNotWarnAboutMefCompositionException";
    private static bool disableWhitecap;
    private Exception mefExceptionToShow;

    private Dispatcher UIDispatcher { get; set; }

    public override string LongApplicationName
    {
      get
      {
          return "Microsoft Expression Shopdrawing";
      }
    }

    public override string ShortApplicationName
    {
      get
      {
          return "Expression Shopdrawing";
      }
    }

    protected override string ProgramName
    {
      get
      {
        return "Shopdrawing";
      }
    }

    public override bool IsReleaseVersion
    {
      get
      {
        return true;
      }
    }

    public override int CustomerFeedbackApplicationIdentifier
    {
      get
      {
        return 5;
      }
    }

    private bool TestingStartup
    {
      get
      {
        return this.Services.GetService<ICommandLineService>().GetArguments("TestStartup") != null;
      }
    }

    public BlendApplication(Microsoft.Expression.Framework.UserInterface.SplashScreen splashScreen)
      : base(splashScreen)
    {
      if (!this.ProcessGeneralCommandLineCommands())
        return;
      this.RunApplication();
    }

    protected override bool ProcessGeneralCommandLineCommands()
    {
      string[] commandLineArgs = Environment.GetCommandLineArgs();
      for (int index = 1; index < commandLineArgs.Length; ++index)
      {
        if (string.Compare(commandLineArgs[index], "/SynchPropertyCache", StringComparison.OrdinalIgnoreCase) == 0)
        {
          PropertyCacheHelper.Synchronous = true;
          break;
        }
      }
      return base.ProcessGeneralCommandLineCommands();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
      this.InitializeUnhandledExceptionHandlers();
      CultureManager.ForceCulture(Path.GetDirectoryName(this.GetType().Module.FullyQualifiedName), "en");
      SplashService splashService = new SplashService("{5d76ab22-cd7a-42ea-9756-629f133abd8ex}", this.RegistryPath);
      this.ReplaceWithWelcomeSplashScreen(splashService.GetSplashVersion() == 1 ? "pack://application:,,,/Shopdrawing.Application;Component/licensing/SplashScreenSketchFlow.png" : "pack://application:,,,/Shopdrawing.Application;Component/licensing/SplashScreen.png");
      this.DoEvents();
      PerformanceUtility.MarkInterimStep(PerformanceEvent.ApplicationStartup, "Creating Services and ExpressionInformationService");
      this.CreateInitialServices(ExpressionApplication.Version);
      this.ExpressionInformationService.MainWindowRootElement = FileTable.GetElement("MainWindow.xaml");
      //this.InitializeLicenseService(ExpressionFeatureMapper.Blend, (ApplicationLicenses) new BlendTrialRtmV4Licenses(), (ApplicationLicenses) new BlendMobileRtmV4Licenses());
      FrameworkPackage.RegisterCommandLineService(this.Services);
      ICommandLineService service1 = this.Services.GetService<ICommandLineService>();
      this.CreateFeedbackService("Shopdrawing", BlendFeedbackValues.CommandToFeedbackValues);
      this.Services.AddService(typeof (SplashService), (object) splashService);
      string name = service1.GetArgument("culture");
      if (!string.IsNullOrEmpty(name))
      {
        Thread.CurrentThread.CurrentCulture = new CultureInfo(name);
        Thread.CurrentThread.CurrentUICulture = new CultureInfo(name);
      }
      PerformanceUtility.MarkInterimStep(PerformanceEvent.ApplicationStartup, "Load resources");
      PerformanceUtility.MarkInterimStep(PerformanceEvent.ApplicationStartup, "Register FrameworkPackage");
      this.Services.RegisterPackage((IPackage) new FrameworkPackage());
      IExpressionMefHostingService service2 = this.Services.GetService<IExpressionMefHostingService>();
      Microsoft.Expression.Framework.UserInterface.IWindowService service3 = this.Services.GetService<Microsoft.Expression.Framework.UserInterface.IWindowService>();
      service3.Title = StringTable.ApplicationTitle;
      FocusScopeManager.SetFocusScopePriority((DependencyObject) System.Windows.Application.Current.MainWindow, 0);
      //FocusScopeManager.Instance.ReturnFocusCallback = new ReturnFocusCallback(((ExpressionApplication)this).FocusScopeManagerReturnFocusCallback);
      this.Services.RegisterPackage((IPackage) new WebServerPackage());
      this.Services.RegisterPackage((IPackage) new SourceControlPackage());
      this.Services.RegisterPackage((IPackage) new ProjectPackage());
      this.Services.RegisterPackage((IPackage) new CodePackage());
      PlatformPackage platformPackage = new PlatformPackage();
      this.Services.RegisterPackage((IPackage) platformPackage);
      if (service2 != null)
        service2.AddInternalPart((object) platformPackage);
      this.Services.RegisterPackage((IPackage) new DesignerPackage());
      this.Services.GetService<IHelpService>().RegisterHelpProvider((IHelpProvider) new BlendSDKHelpProvider());
      PerformanceUtility.MarkInterimStep(PerformanceEvent.ApplicationStartup, "Discovering external packages");
      BlendApplication.disableWhitecap = service1.GetArgument("DisableWhitecap") != null;
      if (BlendApplication.disableWhitecap)
        this.Services.ExcludeAddIn("Microsoft.Expression.PrototypeHostEnvironment.dll");
      this.Services.LoadAddIns("Microsoft.Expression.*.addin");
      this.Services.LoadAddIns("AddIns\\*.addin");
      this.InitializeMefHostingService(service1);
      ICommandService service4 = this.Services.GetService<ICommandService>();
      service4.AddTarget((ICommandTarget) new ApplicationCommandTarget(this.Services));
      PerformanceUtility.MarkInterimStep(PerformanceEvent.ApplicationStartup, "Creating Menu");
      ICommandBar menuBar = this.Services.GetService<ICommandBarService>().CommandBars.AddMenuBar("MainMenu");
      MenuBar.Create(menuBar, this.Services);
      DebugCommands.CreateDebugMenu(menuBar, this.Services);
      this.DoEvents();
      PerformanceUtility.MarkInterimStep(PerformanceEvent.ApplicationStartup, "Show ApplicationWindow");
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.ShowMainWindow);
      service3.Initialized += new EventHandler(this.MainWindow_SourceInitialized);
      service3.IsVisible = true;
      this.MainWindow = service3.MainWindow;
      this.MainWindow.IsEnabled = false;
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.ShowMainWindow);
      this.Services.GetService<IWorkspaceService>().LoadConfiguration(service1.GetArgument("DefaultWorkspace") != null);
      service4.AddTarget((ICommandTarget) new DebugCommands(this.Services));
      PerformanceUtility.MarkInterimStep(PerformanceEvent.ApplicationStartup, "Initializing Project System");
      IProjectManager service5 = this.Services.GetService<IProjectManager>();
      service5.SolutionOpened += new EventHandler<SolutionEventArgs>(this.ProjectManager_SolutionOpened);
      service5.SolutionClosed += new EventHandler<SolutionEventArgs>(this.ProjectManager_SolutionClosed);
      service5.SolutionMigrated += new EventHandler<SolutionEventArgs>(this.ProjectManager_SolutionMigrated);
      this.DoEvents();
      BlendServer.StartRemoteService((IServiceProvider) this.Services);
      if (service1.GetArgument("ExceptionLog") != null)
      {
        ExceptionHandler.Attach(AppDomain.CurrentDomain);
        DebugVariables.Instance.ExceptionHandlerEnabled = true;
      }
      string[] arguments = service1.GetArguments("addin");
      if (arguments != null)
      {
        foreach (string fileName in arguments)
        {
          try
          {
            this.Services.LoadAddIn(fileName);
          }
          catch (Exception ex)
          {
            IMessageDisplayService service6 = this.Services.GetService<IMessageDisplayService>();
            if (service6 != null)
              service6.ShowError(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.ApplicationAssemblyLoadErrorDialogMessage, new object[2]
              {
                (object) fileName,
                (object) ex.Message
              }));
          }
        }
      }
      this.OnStartupIdleProcessing();
      base.OnStartup(e);
    }

    private static bool InitializeLicensingDialogResource(ILicenseService licenseService)
    {
      bool flag = licenseService.IsAnySkuEnabled(licenseService.SkusFromFeature(ExpressionFeatureMapper.SketchFlowFeature)) | licenseService.IsAnySkuEnabled(licenseService.SkusFromFeature(ExpressionFeatureMapper.HobbledSketchFlowFeature));
      string name1 = "Licensing\\AboutDialogResources.xaml";
      if (flag)
        name1 = "Licensing\\AboutDialogResourcesSketchFlow.xaml";
      string name2 = "Licensing\\LicensingDialogResources.xaml";
      if (flag)
        name2 = "Licensing\\LicensingDialogResourcesSketchFlow.xaml";
      licenseService.EstablishLicensingDialogs(FileTable.GetResourceDictionary(name2), FileTable.GetResourceDictionary(name1));
      licenseService.LicensingDialogResources[(object) "GetHelpLinkCommand"] = (object) new DelegateCommand((DelegateCommand.SimpleEventHandler) (() => BlendHelp.Instance.ShowHelpTopic("/html/9c3e6f16-85f3-4ca8-a372-a53dcbf014cb.htm")));
      return flag;
    }

    private void InitializeMefHostingService(ICommandLineService commandLineService)
    {
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.MefInitialization);
      IExpressionMefHostingService service = this.Services.GetService<IExpressionMefHostingService>();
      string[] arguments = commandLineService.GetArguments("extension");
      if (arguments != null)
      {
        foreach (string assembly in arguments)
          service.AddAssembly(assembly);
      }
      string folder = Path.Combine(Path.GetDirectoryName(this.GetType().Module.FullyQualifiedName), "extensions");
      service.AddFolder(folder);
      try
      {
        service.Compose();
      }
      catch (Exception ex)
      {
        this.mefExceptionToShow = ex;
      }
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.MefInitialization);
    }

    private void ShowMefComposeError()
    {
      IConfigurationService service1 = this.Services.GetService<IConfigurationService>();
      IMessageDisplayService service2 = this.Services.GetService<IMessageDisplayService>();
      IMessageLoggingService service3 = this.Services.GetService<IMessageLoggingService>();
      IExpressionMefHostingService service4 = this.Services.GetService<IExpressionMefHostingService>();
      if (this.mefExceptionToShow == null && Enumerable.Count<Exception>(service4.CompositionExceptions) <= 0)
        return;
      bool doNotAskAgain = service1 != null && (bool) service1["MEFHosting"].GetProperty("DoNotWarnAboutMefCompositionException", (object) false);
      if (!doNotAskAgain && service3 != null && (service4 != null && Enumerable.Count<Exception>(service4.CompositionExceptions) > 0))
      {
        foreach (Exception exception in service4.CompositionExceptions)
          service3.WriteLine(exception.Message);
      }
      service4.ClearCompositionExceptions();
      if (!doNotAskAgain && service3 != null && (this.mefExceptionToShow != null && !string.IsNullOrEmpty(this.mefExceptionToShow.Message)))
        service3.WriteLine(this.mefExceptionToShow.Message);
      this.mefExceptionToShow = (Exception) null;
      if (doNotAskAgain || service2 == null)
        return;
      string str = Path.Combine(Path.GetDirectoryName(this.GetType().Module.FullyQualifiedName), "extensions");
      MessageBoxArgs args = new MessageBoxArgs()
      {
        Message = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.MefCompositionException, new object[1]
        {
          (object) str
        }),
        Button = MessageBoxButton.OK,
        Image = MessageBoxImage.Exclamation
      };
      int num = (int) service2.ShowMessage(args, out doNotAskAgain);
      if (!doNotAskAgain || service1 == null)
        return;
      service1["MEFHosting"].SetProperty("DoNotWarnAboutMefCompositionException", (object) true);
    }

    private void OnStartupIdleProcessing()
    {
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.PostingFirstIdleandCallingApplicationRun);
      UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(this.OnFirstIdle));
      PropertyCacheHelper.PopulateStartupReflectionCache();
      UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(FontEmbedder.CreateSystemFontFamiliesCache));
      UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(this.OnLastIdle));
    }

    private void MainWindow_SourceInitialized(object sender, EventArgs e)
    {
      this.WelcomeSplashScreen.Owner = this.Services.GetService<Microsoft.Expression.Framework.UserInterface.IWindowService>().MainWindow;
    }

    private void ProjectManager_SolutionOpened(object sender, SolutionEventArgs e)
    {
      this.SetTitleWithSolutionName(e.Solution.DocumentReference.DisplayName);
    }

    private void ProjectManager_SolutionMigrated(object sender, SolutionEventArgs e)
    {
      this.SetTitleWithSolutionName(e.Solution.DocumentReference.DisplayName);
    }

    private void SetTitleWithSolutionName(string solutionName)
    {
      this.Services.GetService<Microsoft.Expression.Framework.UserInterface.IWindowService>().Title = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.ApplicationTitleFormatWithProjectName, new object[2]
      {
        (object) solutionName,
        (object) StringTable.ApplicationTitle
      });
    }

    private void ProjectManager_SolutionClosed(object sender, SolutionEventArgs e)
    {
      this.Services.GetService<Microsoft.Expression.Framework.UserInterface.IWindowService>().Title = StringTable.ApplicationTitle;
    }

    private void OnFirstIdle()
    {
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.StartupCacheWarmUp);
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.PostingFirstIdleandCallingApplicationRun);
      ICommandLineService service1 = this.Services.GetService<ICommandLineService>();
      this.CloseWelcomeSplashScreen();
      ILicenseService service2 = this.Services.GetService<ILicenseService>();
      BlendApplication.InitializeLicensingDialogResource(service2);
      if (!(service2 is LicenseService) || !LicensingDialogHelper.EnsureProductIsLicensed(this.Services, 0))
      {
        this.Shutdown(-1);
      }
      else
      {
        this.MainWindow.IsEnabled = true;
        this.MainWindow.Focus();
        if (service1.GetArguments("PerfTest") != null)
        {
          string[] arguments = service1.GetArguments("PerfTest");
          if (arguments.Length == 1)
            new PerformanceTester(this.Services).RunTestFile(arguments[0]);
        }
        else
        {
          IProjectManager service3 = this.Services.GetService<IProjectManager>();
          service3.InitializeRecentProjects();
          if (this.Services.GetService<IFeedbackService>().ShouldPromptAtStartup)
            new FeedbackOptionsCommand(this.Services).Execute();
          BlendSdkHelper.PromptUserForMissingSdk(this.Services);
          this.ShowMefComposeError();
          bool flag = true;
          string[] arguments = service1.GetArguments(string.Empty);
          if (arguments == null || arguments.Length != 1)
          {
            IConfigurationObject configurationObject = this.Services.GetService<IConfigurationService>()["WindowService"];
            if (configurationObject != null)
            {
              object property = configurationObject.GetProperty("SkipWelcomeScreen");
              if (property != null && property is bool && (bool) property)
                flag = false;
            }
          }
          if (service1.GetArgument("SkipWelcomeScreen") != null || this.TestingStartup)
            flag = false;
          service3.InitializeFromKnownProjects((string[]) null);
          if (flag && service3.CurrentSolution == null)
            WelcomeScreen.Show(this.Services);
        }
        string[] arguments1 = service1.GetArguments("actions");
        if (arguments1 != null && arguments1.Length == 1)
          new ActionProcessor().ProcessActionFile(arguments1[0], this.Services);
        this.Services.GetService<SplashService>().UpdateSplashVersion(service2);
      }
    }

    private void OnLastIdle()
    {
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.StartupCacheWarmUp);
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.ApplicationStartup);
      if (!this.TestingStartup)
        return;
      this.Shutdown();
    }

    protected override void OnExit(ExitEventArgs e)
    {
      if (DebugVariables.Instance.ExceptionHandlerEnabled)
        ExceptionHandler.Detach(AppDomain.CurrentDomain);
      BlendServer.StopRemoteService();
      base.OnExit(e);
    }

    public override void DoEvents()
    {
      Microsoft.Expression.DesignModel.Core.DispatcherHelper.DoEvents();
    }

    protected override void InitializeUnhandledExceptionHandlers()
    {
      base.InitializeUnhandledExceptionHandlers();
      this.UIDispatcher = Dispatcher.CurrentDispatcher;
      AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(this.AppDomainUnhandledExceptionHandler);
    }

    internal void UnregisterAppDomainUnhandledExceptionHandler()
    {
      AppDomain.CurrentDomain.UnhandledException -= new UnhandledExceptionEventHandler(this.AppDomainUnhandledExceptionHandler);
    }

    private void AppDomainUnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
    {
      //if (!e.IsTerminating || this.UIDispatcher == null || this.UIDispatcher.Thread == Thread.CurrentThread)
      //  return;
      //this.UIDispatcher.Invoke(DispatcherPriority.Send, (Delegate) (o =>
      //{
      //  this.PrepareToTerminateOnUnrecoverableException();
      //  if (!DebugVariables.Instance.ExceptionHandlerEnabled)
      //  {
      //    Exception exception = e.ExceptionObject as Exception;
      //    if (exception != null)
      //    {
      //      ExceptionHandler.IsEnabled = true;
      //      ExceptionHandler.Report(exception);
      //    }
      //  }
      //  return (object) null;
      //}), (object) null);
    }

    protected override void PrepareToTerminateOnUnrecoverableException()
    {
      if (this.UIDispatcher != null)
      {
        AppDomain.CurrentDomain.UnhandledException -= new UnhandledExceptionEventHandler(this.AppDomainUnhandledExceptionHandler);
        this.UIDispatcher = (Dispatcher) null;
      }
      base.PrepareToTerminateOnUnrecoverableException();
    }

    protected override bool ShouldEatException(Exception generatedException)
    {
      this.CloseSplashScreens();
      if (generatedException.StackTrace.Contains("Microsoft.Expression.Framework.FileResourceManager.LoadObject"))
        return false;
      InvalidOperationException operationException = generatedException as InvalidOperationException;
      InvalidCastException invalidCastException = generatedException as InvalidCastException;
      if (generatedException is NotSupportedException || generatedException.StackTrace.Contains("System.Windows.Media.Imaging.BitmapSource.UpdateBitmapSourceResource") || generatedException is AnimationException || (generatedException is DivideByZeroException && generatedException.StackTrace.Contains("MS.Internal.PtsHost.PtsPage.UpdateBottomlessPage") || (generatedException.StackTrace.Contains("System.Windows.UIElement.Measure") || generatedException.StackTrace.Contains("System.Windows.UIElement.Arrange"))) || (generatedException.StackTrace.Contains("System.Windows.UIElement.OnRender") || generatedException.StackTrace.Contains("System.Windows.Media.Animation.TimeManager.Tick") || generatedException.StackTrace.Contains("System.Windows.BroadcastEventHelper.BroadcastLoadedEvent") || (operationException != null && generatedException.StackTrace.Contains("System.Windows.FrameworkElement.ChangeLogicalParent") || operationException != null && generatedException.StackTrace.Contains("System.Windows.Media.MediaContext.RenderMessageHandler"))) || (operationException != null && generatedException.StackTrace.Contains("System.Windows.Media.MediaContext.NotifyPartitionIsZombie") || (generatedException.StackTrace.Contains("System.Windows.Media.Imaging.BitmapDecoder.SetupDecoderFromUriOrStream") || generatedException is XamlParseException) || (this.IsDataException(generatedException) || generatedException is WebException) || (generatedException is IOException && generatedException.StackTrace.Contains("System.Windows.Navigation.NavigationService.DoNavigate") && generatedException.StackTrace.Contains("MS.Internal.AppModel.ResourcePart.GetStreamCore") || operationException != null && generatedException.StackTrace.Contains("MS.Internal.Documents.UndoService.Clear") && generatedException.StackTrace.Contains("System.Windows.Controls.TextBox.OnTextPropertyChanged"))) || (generatedException is ResourceReferenceKeyNotFoundException && generatedException.StackTrace.Contains("MS.Internal.Helper.FindResourceHelper.DoTryCatchWhen") || generatedException is NullReferenceException && generatedException.StackTrace.Contains("System.Windows.Data.BindingExpression.get_DynamicConverter") || operationException != null && generatedException.StackTrace.Contains("System.Windows.Markup.Primitives.MarkupWriter.VerifyTypeIsSerializable") && generatedException.StackTrace.Contains("System.Windows.Documents.TextTreeDeleteContentUndoUnit.CopyObjectNode") || (invalidCastException != null && generatedException.StackTrace.Contains("System.Windows.Controls.TextBlock.CoerceText") || generatedException.StackTrace.Contains("MS.Internal.Text.DynamicPropertyReader.GetCultureInfo") && generatedException.StackTrace.Contains("MS.Internal.PtsHost.PtsHost.FormatLine") || operationException != null && generatedException.StackTrace.Contains("System.Windows.Data.ListCollectionView.PrepareLocalArray") && generatedException.StackTrace.Contains("System.Array.SorterObjectArray.SwapIfGreaterWithItems")) || (operationException != null && generatedException.StackTrace.Contains("System.Windows.Controls.ItemsControl.OnItemsSourceChanged") && generatedException.StackTrace.Contains("System.Windows.Controls.ItemCollection.SetItemsSource") || invalidCastException != null && generatedException.StackTrace.Contains("System.Windows.Navigation.JournalEntryListConverter.Convert") || (operationException != null && generatedException.StackTrace.Contains("System.Windows.WeakEventManager.Purge") && generatedException.StackTrace.Contains("System.Windows.LostFocusEventManager.StopListening") || (BlendApplication.IsDispatcherException(generatedException) || BlendApplication.IsSilverlightObjectDisposedException(generatedException) || this.IsUserException(generatedException))))) || generatedException is FileNotFoundException && generatedException.StackTrace.Contains("System.Windows.Controls.SoundPlayerAction"))
        return true;
      if (generatedException.InnerException != null)
        return this.ShouldEatException(generatedException.InnerException);
      return false;
    }

    protected override bool ShouldTerminateOnException(DispatcherUnhandledExceptionEventArgs generatedExceptionArguments)
    {
      return base.ShouldTerminateOnException(generatedExceptionArguments) || BlendApplication.IsSilverlightObjectDisposedException(generatedExceptionArguments.Exception);
    }

    private static bool IsSilverlightObjectDisposedException(Exception generatedException)
    {
      ObjectDisposedException disposedException = generatedException as ObjectDisposedException;
      return disposedException != null && string.Equals(disposedException.ObjectName, "NativeObject", StringComparison.Ordinal);
    }

    private bool IsDataException(Exception generatedException)
    {
      if (generatedException != null && generatedException.StackTrace.Contains("System.Windows.Data.MultiBindingExpression.Transfer"))
        return true;
      if (generatedException is InvalidOperationException)
        return generatedException.StackTrace.Contains("MS.Internal.Data.PropertyPathWorker.CheckReadOnly");
      return false;
    }

    private static bool IsDispatcherException(Exception generatedException)
    {
      if (!(generatedException is TargetParameterCountException))
        return false;
      string str = generatedException.StackTrace;
      int length = str.IndexOf(Environment.NewLine);
      if (length != -1)
        str = str.Substring(0, length);
      return str.Contains("System.Reflection.RuntimeMethodInfo.Invoke");
    }

    private bool IsUserException(Exception generatedException)
    {
      StackTrace stackTrace = new StackTrace(generatedException);
      for (int index = 0; index < stackTrace.FrameCount; ++index)
      {
        if (this.IsUserAssembly(stackTrace.GetFrame(index).GetMethod().Module.Assembly))
          return true;
      }
      if (generatedException.InnerException != null)
        return this.IsUserException(generatedException.InnerException);
      return false;
    }

    private bool IsUserAssembly(Assembly assembly)
    {
      foreach (IAssembly assembly1 in this.Services.GetService<IDesignerDefaultPlatformService>().DefaultPlatform.Metadata.DefaultAssemblies)
      {
        if (assembly1.CompareTo(assembly))
          return false;
      }
      if (!PlatformTypes.DesignToolAssemblies.Contains(assembly))
        return !assembly.FullName.Contains("Microsoft");
      return false;
    }

    protected override void UnhandledExceptionPerApplicationHandler(DispatcherUnhandledExceptionEventArgs generatedExceptionArguments)
    {
      generatedExceptionArguments.Handled = true;
      if (this.ExpressionInformationService == null)
        return;
      IViewService service = this.Services.GetService<IViewService>();
      if (service == null)
        return;
      SceneView sceneView = service.ActiveView as SceneView;
      if (sceneView == null || this.IsDataException(generatedExceptionArguments.Exception) || (sceneView.MessageContent != null || sceneView.InstanceBuilderContext == null) || (sceneView.InstanceBuilderContext.ViewNodeManager == null || sceneView.InstanceBuilderContext.ViewNodeManager.IsUpdating))
        return;
      sceneView.ViewModel.AnimationEditor.Invalidate();
      sceneView.OnExceptionWithUnknownSource(generatedExceptionArguments.Exception);
      sceneView.ViewModel.RefreshCurrentValues();
      ExceptionHandler.SafelyForceLayoutArrange();
    }

    protected override void PerformFinalActionsBeforeWatsonDump()
    {
      if (this.ExpressionInformationService == null)
        return;
      MessageBoxArgs args = new MessageBoxArgs()
      {
        Message = StringTable.ApplicationUnrecoverableErrorDialogMessage,
        Button = MessageBoxButton.YesNo,
        Image = MessageBoxImage.Hand
      };
      MessageBoxResult messageBoxResult = MessageBoxResult.No;
      IMessageDisplayService service1 = this.Services.GetService<IMessageDisplayService>();
      if (service1 != null)
        messageBoxResult = service1.ShowMessage(args);
      IProjectManager service2 = this.Services.GetService<IProjectManager>();
      IViewService service3 = this.Services.GetService<IViewService>();
      if (service3 != null)
      {
        foreach (IView view in (IEnumerable<IView>) service3.Views)
        {
          SilverlightSceneView silverlightSceneView = view as SilverlightSceneView;
          if (silverlightSceneView != null)
            silverlightSceneView.SuspendUpdatesForViewShutdown();
        }
      }
      if (service2 == null || service2.CurrentSolution == null)
        return;
      bool flag = false;
      foreach (IProject project in service2.CurrentSolution.Projects)
      {
        foreach (IProjectItem projectItem in (IEnumerable<IProjectItem>) project.Items)
        {
          if (projectItem.IsDirty)
          {
            flag = true;
            break;
          }
        }
        if (flag)
          break;
      }
      if (!flag || messageBoxResult != MessageBoxResult.Yes)
        return;
      service2.CurrentSolution.Save(false);
    }
  }
}
