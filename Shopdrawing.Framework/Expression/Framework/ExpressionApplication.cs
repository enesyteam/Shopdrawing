// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.ExpressionApplication
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Extensibility;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.Feedback;
using Microsoft.Expression.Framework.Licenses;
using Microsoft.Expression.Framework.UserInterface;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Security;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;

namespace Microsoft.Expression.Framework
{
  public abstract class ExpressionApplication : Application, IDisposable
  {
    private RunTimeTracker runTimetracker;
    private AutoResetEvent autoResetEvent;

    public static Version Version
    {
      get
      {
        return new Version(((AssemblyFileVersionAttribute) Assembly.GetEntryAssembly().GetCustomAttributes(typeof (AssemblyFileVersionAttribute), false)[0]).Version);
      }
    }

    public abstract string LongApplicationName { get; }

    public abstract string ShortApplicationName { get; }

    protected abstract string ProgramName { get; }

    public string RegistryPath
    {
      get
      {
        return "SOFTWARE\\Microsoft\\Expression\\" + this.ProgramName + "\\";
      }
    }

    public string VersionedRegistryPath
    {
      get
      {
        return this.RegistryPath + (object) this.ExpressionInformationService.Version.Major + "." + (string) (object) this.ExpressionInformationService.Version.Minor + "\\";
      }
    }

    public string PoliciesRegistryPath
    {
      get
      {
        return "SOFTWARE\\Policies\\Microsoft\\Expression\\" + this.ProgramName + "\\";
      }
    }

    public string VersionedPoliciesRegistryPath
    {
      get
      {
        return this.PoliciesRegistryPath + (object) this.ExpressionInformationService.Version.Major + "." + (string) (object) this.ExpressionInformationService.Version.Minor + "\\";
      }
    }

    public virtual bool IsReleaseVersion
    {
      get
      {
        return false;
      }
    }

    public virtual string StudioKeyRegistryPath
    {
      get
      {
        return "SOFTWARE\\Microsoft\\Expression\\Suite\\";
      }
    }

    public virtual int CustomerFeedbackApplicationIdentifier
    {
      get
      {
        return 4;
      }
    }

    public IExpressionInformationService ExpressionInformationService { get; private set; }

    public IServices Services { get; private set; }

    public WelcomeSplashScreen WelcomeSplashScreen { get; set; }

    public Microsoft.Expression.Framework.UserInterface.SplashScreen InitialSplashScreen { get; set; }

    public static string CommonFontsLocation
    {
      get
      {
        return Path.Combine(Path.GetDirectoryName(Assembly.GetAssembly(typeof (ExpressionApplication)).Location), "../Common/Fonts/");
      }
    }

    protected ExpressionApplication()
      : this((Microsoft.Expression.Framework.UserInterface.SplashScreen) null)
    {
    }

    protected ExpressionApplication(Microsoft.Expression.Framework.UserInterface.SplashScreen splashScreen)
    {
      this.InitialSplashScreen = splashScreen;
    }

    protected virtual bool ProcessGeneralCommandLineCommands()
    {
      string[] commandLineArgs = Environment.GetCommandLineArgs();
      for (int index = 1; index < commandLineArgs.Length; ++index)
      {
        if (string.Compare(commandLineArgs[index], "/EnableEventLogging", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(commandLineArgs[index], "-EnableEventLogging", StringComparison.OrdinalIgnoreCase) == 0 || (string.Compare(commandLineArgs[index], "/Diagnostics", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(commandLineArgs[index], "-Diagnostics", StringComparison.OrdinalIgnoreCase) == 0))
        {
          PerformanceUtility.LoggingEnabled = true;
          break;
        }
      }
      for (int index = 1; index < commandLineArgs.Length; ++index)
      {
        if (commandLineArgs[index].StartsWith("/PerformanceEvent:", StringComparison.OrdinalIgnoreCase) || commandLineArgs[index].StartsWith("-PerformanceEvent:", StringComparison.OrdinalIgnoreCase))
        {
          string str = commandLineArgs[index].Substring("PerformanceEvent:".Length + 1);
          try
          {
            PerformanceUtility.EnableEventProfile((PerformanceEvent) Enum.Parse(typeof (PerformanceEvent), str));
            PerformanceUtility.LoggingEnabled = true;
            break;
          }
          catch (ArgumentException ex)
          {
            break;
          }
        }
      }
      string[] arguments = new CommandLineService().GetArguments("SqmPipe", commandLineArgs);
      if (arguments != null && arguments.Length > 0)
      {
        int result;
        if (!int.TryParse(arguments[0], out result))
          result = 0;
        FeedbackService.Pipeline = result;
      }
      return true;
    }

    protected virtual void RunApplication()
    {
      ExpressionApplication.StartGlobalPerformanceInstrumentation();
      PerformanceUtility.MarkInterimStep(PerformanceEvent.ApplicationStartup, "Application Created");
      try
      {
        this.Run();
      }
      catch (TargetInvocationException ex)
      {
        if (ex.InnerException != null)
          this.ShowSafeMessageBox(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.ApplicationTargetInvocationDialogMessage, new object[1]
          {
            (object) ex.InnerException.ToString()
          }), StringTable.ApplicationTargetInvocationDialogTitle);
        throw;
      }
      catch (SecurityException ex)
      {
        this.ShowSafeMessageBox(ex.ToString(), StringTable.ApplicationSecurityExceptionDialogTitle);
      }
      catch (MissingSatelliteAssemblyException ex)
      {
        this.ShowSafeMessageBox(ex.Message, "Microsoft Expression");
      }
    }

    protected void CreateInitialServices(Version version)
    {
      this.Services = (IServices) new Microsoft.Expression.Framework.Services();
      this.ExpressionInformationService = (IExpressionInformationService) new Microsoft.Expression.Framework.ExpressionInformationService(this.Services, version, this);
      LowMemoryMessage.SetApplicationInformation(this.ExpressionInformationService);
      this.Services.AddService(typeof (IExpressionInformationService), (object) this.ExpressionInformationService);
    }

    public virtual void DoEvents()
    {
    }

    protected override void OnStartup(StartupEventArgs e)
    {
      this.runTimetracker = RunTimeTracker.GetInstance(this);
      base.OnStartup(e);
    }

    protected override void OnExit(ExitEventArgs e)
    {
      IFeedbackService service = this.Services.GetService<IFeedbackService>();
      if (service != null)
        service.Stop();
      base.OnExit(e);
    }

    protected void ReplaceWithWelcomeSplashScreen(string welcomeSplashScreenPath)
    {
      this.WelcomeSplashScreen = new WelcomeSplashScreen(welcomeSplashScreenPath)
      {
        DismissDelay = TimeSpan.FromSeconds(1.0)
      };
      this.WelcomeSplashScreen.Show();
      this.WelcomeSplashScreen.Activate();
      if (this.InitialSplashScreen != null)
        this.InitialSplashScreen.Close(new WindowInteropHelper((Window) this.WelcomeSplashScreen).Handle);
      this.MainWindow = (Window) null;
    }

    protected void CloseWelcomeSplashScreen()
    {
      if (this.WelcomeSplashScreen == null)
        return;
      this.WelcomeSplashScreen.CanClose = true;
      this.WelcomeSplashScreen.Close();
      this.WelcomeSplashScreen = (WelcomeSplashScreen) null;
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.ShowSplashScreen);
    }

    protected void CloseSplashScreens()
    {
      if (this.InitialSplashScreen != null)
      {
        try
        {
          this.InitialSplashScreen.Close();
        }
        catch
        {
        }
      }
      try
      {
        this.CloseWelcomeSplashScreen();
      }
      catch
      {
      }
    }

    [CLSCompliant(false)]
    protected ILicenseService InitializeLicenseService(string productSpecificationFeature, params ApplicationLicenses[] applicationLicenses)
    {
      ApplicationLicenses[] studioLicenses = new ApplicationLicenses[5]
      {
        (ApplicationLicenses) new StudioPremiumRtmV4Licenses(),
        (ApplicationLicenses) new StudioRtmV3ForV4Licenses(),
        (ApplicationLicenses) new StudioUltimateRtmV4Licenses(),
        (ApplicationLicenses) new WebStudioRtmV4Licenses(),
        (ApplicationLicenses) new XWebRtmV3ForV4Licenses()
      };
      ILicenseService licenseService = (ILicenseService) new LicenseService(this.Services, productSpecificationFeature, (ILicenseSkuFeatureMapper) new ExpressionRtmV4FeatureMapper(), this.StudioKeyRegistryPath, this.ExpressionInformationService.RegistryPath, studioLicenses, applicationLicenses);
      this.Services.AddService(typeof (ILicenseService), (object) licenseService);
      licenseService.Start();
      return licenseService;
    }

    protected void CreateFeedbackService(string customerFeedbackTemporaryFileName, Dictionary<string, int> commandStringToFeedbackValues)
    {
      FeedbackService feedbackService = new FeedbackService(this)
      {
        LoggingFileName = customerFeedbackTemporaryFileName,
        CustomerFeedbackRegistryPath = this.VersionedRegistryPath,
        GroupFeedbackRegistryPath = this.VersionedPoliciesRegistryPath
      };
      feedbackService.AddCommandStringToValueTable(CoreFeedbackValues.CommandToFeedbackValues);
      feedbackService.AddCommandStringToValueTable(commandStringToFeedbackValues);
      feedbackService.Start();
      this.Services.RegisterPackage((IPackage) feedbackService);
    }

    protected void FocusScopeManagerReturnFocusCallback()
    {
      if (this.Services != null)
      {
        IViewService service = this.Services.GetService<IViewService>();
        if (service == null || service.ActiveView == null)
          return;
        service.ActiveView.ReturnFocus();
      }
      else
        Keyboard.Focus((IInputElement) null);
    }

    protected static void StartGlobalPerformanceInstrumentation()
    {
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.ShowSplashScreen);
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.ApplicationStartup);
    }

    protected static void EndGlobalPerformanceInstrumentation(string performanceLogFile)
    {
    }

    protected void ShowSafeMessageBox(string message, string title)
    {
      this.CloseSplashScreens();
      UnsafeNativeMethods.MessageBox(IntPtr.Zero, message, title, 65600U);
    }

    private void UnhandledExceptionFilter(object sender, DispatcherUnhandledExceptionFilterEventArgs generatedException)
    {
      generatedException.RequestCatch = this.ShouldEatException(generatedException.Exception);
      int num = generatedException.RequestCatch ? 1 : 0;
    }

    private void UnhandledExceptionHandler(object sender, DispatcherUnhandledExceptionEventArgs generatedExceptionArguments)
    {
      if (generatedExceptionArguments.Handled || !this.ShouldEatException(generatedExceptionArguments.Exception))
        return;
      if (this.ShouldTerminateOnException(generatedExceptionArguments))
      {
        this.PrepareToTerminateOnUnrecoverableException();
      }
      else
      {
        generatedExceptionArguments.Handled = true;
        this.UnhandledExceptionPerApplicationHandler(generatedExceptionArguments);
      }
    }

    protected virtual void PrepareToTerminateOnUnrecoverableException()
    {
      MessageDisplayService messageDisplayService = this.ExpressionInformationService == null ? (MessageDisplayService) null : this.Services.GetService<IMessageDisplayService>() as MessageDisplayService;
      if (messageDisplayService != null)
        messageDisplayService.UseWin32MessageBox = true;
      using (Dispatcher.CurrentDispatcher.DisableProcessing())
        this.AttemptFinalActsOnUnrecoverableError();
    }

    protected virtual void UnhandledExceptionPerApplicationHandler(DispatcherUnhandledExceptionEventArgs generatedExceptionArguments)
    {
    }

    protected virtual void InitializeUnhandledExceptionHandlers()
    {
      Dispatcher.CurrentDispatcher.UnhandledExceptionFilter += new DispatcherUnhandledExceptionFilterEventHandler(this.UnhandledExceptionFilter);
      Dispatcher.CurrentDispatcher.UnhandledException += new DispatcherUnhandledExceptionEventHandler(this.UnhandledExceptionHandler);
    }

    private void AttemptFinalActsOnUnrecoverableError()
    {
      this.autoResetEvent = new AutoResetEvent(false);
      new Thread(new ThreadStart(this.AttemptFinalActsWorker)).Start();
      this.autoResetEvent.WaitOne();
    }

    private void AttemptFinalActsWorker()
    {
      try
      {
        this.PerformFinalActionsBeforeWatsonDump();
      }
      finally
      {
        this.autoResetEvent.Set();
      }
    }

    protected virtual void PerformFinalActionsBeforeWatsonDump()
    {
    }

    protected virtual bool ShouldEatException(Exception generatedException)
    {
      return false;
    }

    protected virtual bool ShouldTerminateOnException(DispatcherUnhandledExceptionEventArgs generatedExceptionArguments)
    {
      return generatedExceptionArguments.Exception is InvalidOperationException && generatedExceptionArguments.Exception.StackTrace.Contains("System.Windows.Media.MediaContext.NotifyPartitionIsZombie");
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing || this.autoResetEvent == null)
        return;
      this.autoResetEvent.Close();
      this.autoResetEvent = (AutoResetEvent) null;
    }
  }
}
