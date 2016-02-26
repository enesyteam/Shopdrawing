// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Feedback.FeedbackService
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Extensibility;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Interop;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using System.Windows.Media;

namespace Microsoft.Expression.Framework.Feedback
{
  public class FeedbackService : IFeedbackService, IPackage
  {
    private static string feedbackPath = string.Empty;
    private static int pipeline = -1;
    private string customerFeedbackRegistryPath = string.Empty;
    private string groupFeedbackRegistryPath = string.Empty;
    private string customerFeedbackPolicyRegistryKeyName = "QMEnable";
    private string feedbackPromptRegistryKeyName = "QMPrompt";
    private int pauseTimeBeforeUploading = 10000;
    private bool canSetFeedbackPolicy = true;
    private bool? forceFeedback = new bool?();
    private const string sqmClientRegistryPath = "SOFTWARE\\Microsoft\\SQMClient\\";
    private const string sqmClientPoliciesRegistryPath = "SOFTWARE\\Policies\\Microsoft\\SQMClient\\";
    private const string msftInternalValueName = "MSFTInternal";
    private const string isTestValueName = "IsTest";
    private const string redmondSqmPipelineUrl = "http://expsqmtest/sqmtest/sqmserver.dll";
    private const string chinaSqmPipelineUrl = "http://expsqmvmsh/sqmtest/sqmserver.dll";
    private const string productionSqmPipelineUrl = "http://sqm.microsoft.com/sqm/expressionsuite/sqmserver.dll";
    private IServices services;
    private ExpressionApplication parentApplication;
    private HSQMSESSION sqmSessionHandle;
    private Dictionary<string, int> commandFeedbackValueTable;
    private Guid sharedMachineId;
    private Guid sharedUserId;
    private uint filesToBeUploaded;
    private bool programaticClose;

    private static string FeedbackPath
    {
      get
      {
        if (string.IsNullOrEmpty(FeedbackService.feedbackPath))
        {
          try
          {
            FeedbackService.feedbackPath = Path.GetTempPath();
          }
          catch (SecurityException ex)
          {
          }
        }
        return FeedbackService.feedbackPath;
      }
    }

    private ExpressionApplication ParentApplication
    {
      get
      {
        return this.parentApplication;
      }
    }

    public bool IsLogging { get; private set; }

    public string CustomerFeedbackRegistryPath
    {
      get
      {
        return this.customerFeedbackRegistryPath;
      }
      set
      {
        if (!string.IsNullOrEmpty(this.customerFeedbackRegistryPath))
          return;
        this.customerFeedbackRegistryPath = value;
      }
    }

    public string GroupFeedbackRegistryPath
    {
      get
      {
        return this.groupFeedbackRegistryPath;
      }
      set
      {
        if (!string.IsNullOrEmpty(this.groupFeedbackRegistryPath))
          return;
        this.groupFeedbackRegistryPath = value;
      }
    }

    public string LoggingFileName { get; set; }

    public string UploadFileNamePattern
    {
      get
      {
        return Path.Combine(FeedbackService.FeedbackPath, this.LoggingFileName + "*.sqm");
      }
    }

    public string EndSessionFileNamePattern
    {
      get
      {
        return Path.Combine(FeedbackService.FeedbackPath, this.LoggingFileName + "%02d.sqm");
      }
    }

    private int Timeout { get; set; }

    private FeedbackService.UserType CurrentUserType
    {
      get
      {
        bool? nullable1 = this.RetrieveRegistryBoolValue("SOFTWARE\\Policies\\Microsoft\\SQMClient\\", "IsTest");
        if (!nullable1.HasValue)
          return FeedbackService.UserType.Unavailable;
        bool? nullable2 = nullable1;
        if ((!nullable2.GetValueOrDefault() ? false : (nullable2.HasValue ? true : false)) != false)
          return FeedbackService.UserType.TestLab;
        bool? nullable3 = this.RetrieveRegistryBoolValue("SOFTWARE\\Microsoft\\SQMClient\\", "IsTest");
        if (!nullable3.HasValue)
          return FeedbackService.UserType.Unavailable;
        bool? nullable4 = nullable3;
        if ((!nullable4.GetValueOrDefault() ? false : (nullable4.HasValue ? true : false)) != false)
          return FeedbackService.UserType.TestLab;
        bool? nullable5 = this.RetrieveRegistryBoolValue("SOFTWARE\\Policies\\Microsoft\\SQMClient\\", "MSFTInternal");
        if (!nullable5.HasValue)
          return FeedbackService.UserType.Unavailable;
        bool? nullable6 = nullable5;
        if ((!nullable6.GetValueOrDefault() ? false : (nullable6.HasValue ? true : false)) != false)
          return FeedbackService.UserType.Internal;
        bool? nullable7 = this.RetrieveRegistryBoolValue("SOFTWARE\\Microsoft\\SQMClient\\", "MSFTInternal");
        if (!nullable7.HasValue)
          return FeedbackService.UserType.Unavailable;
        bool? nullable8 = nullable7;
        return (!nullable8.GetValueOrDefault() ? false : (nullable8.HasValue ? true : false)) != false ? FeedbackService.UserType.Internal : FeedbackService.UserType.External;
      }
    }

    public static int Pipeline
    {
      get
      {
        return FeedbackService.pipeline;
      }
      set
      {
        FeedbackService.pipeline = value;
      }
    }

    public string CustomerFeedbackPolicyRegistryKeyName
    {
      get
      {
        return this.customerFeedbackPolicyRegistryKeyName;
      }
    }

    public string FeedbackPromptRegistryKeyName
    {
      get
      {
        return this.feedbackPromptRegistryKeyName;
      }
    }

    public bool ShouldPromptAtStartup
    {
      get
      {
        if (string.IsNullOrEmpty(this.CustomerFeedbackRegistryPath) || RegistryHelper.RetrieveCurrentUserRegistryValue<bool>(this.CustomerFeedbackRegistryPath, this.FeedbackPromptRegistryKeyName))
          return false;
        RegistryHelper.SetCurrentUserRegistryValue<bool>(this.CustomerFeedbackRegistryPath, this.FeedbackPromptRegistryKeyName, true);
        return !this.AggregateFeedbackPolicy.HasValue;
      }
    }

    public bool? AggregateFeedbackPolicy
    {
      get
      {
        bool? groupFeedbackPolicy = this.GroupFeedbackPolicy;
        if (groupFeedbackPolicy.HasValue)
        {
          this.canSetFeedbackPolicy = false;
          return groupFeedbackPolicy;
        }
        this.canSetFeedbackPolicy = true;
        bool? customerFeedbackPolicy = this.CustomerFeedbackPolicy;
        if (customerFeedbackPolicy.HasValue)
          return customerFeedbackPolicy;
        if (this.forceFeedback.HasValue)
          return this.forceFeedback;
        return new bool?();
      }
    }

    public bool CanSetFeedbackPolicy
    {
      get
      {
        return this.canSetFeedbackPolicy;
      }
    }

    public bool? CustomerFeedbackPolicy
    {
      get
      {
        return this.RetrieveExpressionCustomerFeedbackRecordingPolicy();
      }
      set
      {
        this.SetExpressionCustomerFeedbackDataRecordingPolicy(value);
      }
    }

    private bool? GroupFeedbackPolicy
    {
      get
      {
        return this.RetrieveExpressionGroupFeedbackRecordingPolicy();
      }
    }

    public FeedbackService(ExpressionApplication application)
    {
      this.services = application.Services;
      this.parentApplication = application;
      this.Timeout = 30000;
    }

    public void Start()
    {
      bool? aggregateFeedbackPolicy = this.AggregateFeedbackPolicy;
      this.IsLogging = aggregateFeedbackPolicy.GetValueOrDefault() && aggregateFeedbackPolicy.HasValue;
      if (!this.IsLogging)
        return;
      this.StartFeedbackRecording();
      ThreadPool.QueueUserWorkItem(new WaitCallback(this.UploadWorker), (object) null);
    }

    private void StartFeedbackRecording()
    {
      if (!this.CreateFeedbackSession(this.ParentApplication))
        return;
      this.RecordStartupInformation();
    }

    public void AddCommandStringToValueTable(Dictionary<string, int> commandStringToFeedbackValue)
    {
      if (this.commandFeedbackValueTable == null)
        this.commandFeedbackValueTable = new Dictionary<string, int>();
      if (commandStringToFeedbackValue == null)
        return;
      foreach (KeyValuePair<string, int> keyValuePair in commandStringToFeedbackValue)
      {
        this.commandFeedbackValueTable.ContainsKey(keyValuePair.Key);
        this.commandFeedbackValueTable[keyValuePair.Key] = keyValuePair.Value;
      }
    }

    public void Load(IServices services)
    {
      this.services.AddService(typeof (IFeedbackService), (object) this);
    }

    public void Unload()
    {
    }

    public int GetFeedbackValue(string commandName)
    {
      int num;
      if (!this.commandFeedbackValueTable.TryGetValue(commandName, out num))
        num = 0;
      return num;
    }

    public void SetData(int dataId, int value)
    {
      if (!this.IsLogging || UnsafeNativeMethods.Set(this.sqmSessionHandle, (uint) dataId, (uint) value))
        return;
      Marshal.GetLastWin32Error();
    }

    public void AddDataToStream(int dataId, int value)
    {
      if (!this.IsLogging || UnsafeNativeMethods.AddToStream(this.sqmSessionHandle, (uint) dataId, 1, value, 0, 0, 0, 0, 0, 0, 0, 0))
        return;
      Marshal.GetLastWin32Error();
    }

    public void AddDataToStream2(int dataId, int value1, int value2)
    {
      if (!this.IsLogging || UnsafeNativeMethods.AddToStream(this.sqmSessionHandle, (uint) dataId, 2, value1, value2, 0, 0, 0, 0, 0, 0, 0))
        return;
      Marshal.GetLastWin32Error();
    }

    private bool CreateFeedbackSession(ExpressionApplication application)
    {
      if (string.IsNullOrEmpty(this.LoggingFileName))
        this.LoggingFileName = "MySqmSession";
      this.sqmSessionHandle = UnsafeNativeMethods.GetSession(this.LoggingFileName, 65536U, 1U);
      if (this.sqmSessionHandle == null)
      {
        Marshal.GetLastWin32Error();
        return false;
      }
      if (this.sqmSessionHandle.IsInvalid)
        return false;
      if (!UnsafeNativeMethods.SetAppId(this.sqmSessionHandle, (uint) application.CustomerFeedbackApplicationIdentifier))
      {
        Marshal.GetLastWin32Error();
        return false;
      }
      Version version = ExpressionApplication.Version;
      if (!UnsafeNativeMethods.SetAppVersion(this.sqmSessionHandle, (uint) ((version.Major << 16) + version.Minor), (uint) ((version.Build << 16) + version.Revision)))
      {
        Marshal.GetLastWin32Error();
        return false;
      }
      if (!UnsafeNativeMethods.ReadSharedMachineId(ref this.sharedMachineId) && !UnsafeNativeMethods.CreateNewId(ref this.sharedMachineId))
        UnsafeNativeMethods.WriteSharedMachineId(ref this.sharedMachineId);
      UnsafeNativeMethods.SetMachineId(this.sqmSessionHandle, ref this.sharedMachineId);
      if (!UnsafeNativeMethods.ReadSharedUserId(ref this.sharedUserId) && !UnsafeNativeMethods.CreateNewId(ref this.sharedUserId))
        UnsafeNativeMethods.WriteSharedUserId(ref this.sharedUserId);
      UnsafeNativeMethods.SetUserId(this.sqmSessionHandle, ref this.sharedUserId);
      return true;
    }

    private void RecordStartupInformation()
    {
      UnsafeNativeMethods.Set(this.sqmSessionHandle, 5U, (uint) (Environment.OSVersion.Version.Major * 65536 + Environment.OSVersion.Version.Minor));
      UnsafeNativeMethods.Set(this.sqmSessionHandle, 6U, (uint) CultureInfo.InstalledUICulture.LCID);
      UnsafeNativeMethods.Set(this.sqmSessionHandle, 7U, (uint) Thread.CurrentThread.CurrentUICulture.LCID);
      UnsafeNativeMethods.Set(this.sqmSessionHandle, 8U, (uint) this.CurrentUserType);
      this.RecordDisplayInformation();
      this.RecordRenderCapabilities();
      Microsoft.Expression.Framework.NativeMethods.MEMORYSTATUSEX lpBuffer = new Microsoft.Expression.Framework.NativeMethods.MEMORYSTATUSEX();
      if (Microsoft.Expression.Framework.NativeMethods.GlobalMemoryStatusEx(lpBuffer))
        UnsafeNativeMethods.Set(this.sqmSessionHandle, 40U, (uint) (lpBuffer.ullTotalPhys >> 10));
      this.RecordWinSatInformation();
    }

    private void RecordRenderCapabilities()
    {
      UnsafeNativeMethods.Set(this.sqmSessionHandle, 33U, (uint) (RenderCapability.Tier >> 16));
      short majorVersionRequested = (short) 3;
      short minorVersionRequested = (short) 0;
      if (!RenderCapability.IsPixelShaderVersionSupported(majorVersionRequested, minorVersionRequested))
      {
        majorVersionRequested = (short) 2;
        if (!RenderCapability.IsPixelShaderVersionSupported(majorVersionRequested, minorVersionRequested))
          majorVersionRequested = (short) 0;
      }
      UnsafeNativeMethods.Set(this.sqmSessionHandle, 34U, ((uint) majorVersionRequested << 16) + (uint) minorVersionRequested);
    }

    private void RecordDisplayInformation()
    {
      UnsafeNativeMethods.Set(this.sqmSessionHandle, 36U, (uint) Microsoft.Expression.Framework.NativeMethods.GetSystemMetrics(80));
      IntPtr dc = Microsoft.Expression.Framework.NativeMethods.GetDC(IntPtr.Zero);
      if (!(dc != IntPtr.Zero))
        return;
      try
      {
        int deviceCaps = Microsoft.Expression.Framework.NativeMethods.GetDeviceCaps(dc, 88);
        FeedbackService.SqmDpi sqmDpi;
        switch (deviceCaps)
        {
          case 72:
            sqmDpi = FeedbackService.SqmDpi.Dpi72;
            break;
          case 96:
            sqmDpi = FeedbackService.SqmDpi.Dpi96;
            break;
          case 120:
            sqmDpi = FeedbackService.SqmDpi.Dpi120;
            break;
          case 144:
            sqmDpi = FeedbackService.SqmDpi.Dpi144;
            break;
          case 192:
            sqmDpi = FeedbackService.SqmDpi.Dpi192;
            break;
          default:
            sqmDpi = deviceCaps >= 192 ? FeedbackService.SqmDpi.CustomDpiGreaterThan192 : FeedbackService.SqmDpi.CustomDpiLessThan192;
            break;
        }
        UnsafeNativeMethods.Set(this.sqmSessionHandle, 39U, (uint) sqmDpi);
      }
      finally
      {
        Microsoft.Expression.Framework.NativeMethods.ReleaseDC(IntPtr.Zero, dc);
      }
    }

    private void RecordWinSatInformation()
    {
      Type typeFromProgId = Type.GetTypeFromProgID("QueryWinSAT");
      if (typeFromProgId == (Type) null)
      {
        UnsafeNativeMethods.Set(this.sqmSessionHandle, 26U, 0U);
      }
      else
      {
        try
        {
          Microsoft.Expression.Framework.NativeMethods.IProvideWinSATResultsInfo info = (Activator.CreateInstance(typeFromProgId) as Microsoft.Expression.Framework.NativeMethods.IQueryRecentWinSATAssessment).Info;
          switch (info.AssessmentState)
          {
            case Microsoft.Expression.Framework.NativeMethods.WINSAT_ASSESSMENT_STATE.WINSAT_ASSESSMENT_STATE_VALID:
              UnsafeNativeMethods.Set(this.sqmSessionHandle, 26U, 1U);
              break;
            case Microsoft.Expression.Framework.NativeMethods.WINSAT_ASSESSMENT_STATE.WINSAT_ASSESSMENT_STATE_INCOHERENT_WITH_HARDWARE:
              UnsafeNativeMethods.Set(this.sqmSessionHandle, 26U, 2U);
              break;
            default:
              UnsafeNativeMethods.Set(this.sqmSessionHandle, 26U, 0U);
              break;
          }
          if (info.AssessmentState != Microsoft.Expression.Framework.NativeMethods.WINSAT_ASSESSMENT_STATE.WINSAT_ASSESSMENT_STATE_VALID && info.AssessmentState != Microsoft.Expression.Framework.NativeMethods.WINSAT_ASSESSMENT_STATE.WINSAT_ASSESSMENT_STATE_INCOHERENT_WITH_HARDWARE)
            return;
          UnsafeNativeMethods.Set(this.sqmSessionHandle, 28U, (uint) ((double) info.SystemRating * 10.0));
          UnsafeNativeMethods.Set(this.sqmSessionHandle, 27U, (uint) ((double) info.GetAssessmentInfo(Microsoft.Expression.Framework.NativeMethods.WINSAT_ASSESSMENT_TYPE.WINSAT_ASSESSMENT_CPU).Score * 10.0));
          UnsafeNativeMethods.Set(this.sqmSessionHandle, 29U, (uint) ((double) info.GetAssessmentInfo(Microsoft.Expression.Framework.NativeMethods.WINSAT_ASSESSMENT_TYPE.WINSAT_ASSESSMENT_D3D).Score * 10.0));
          UnsafeNativeMethods.Set(this.sqmSessionHandle, 30U, (uint) ((double) info.GetAssessmentInfo(Microsoft.Expression.Framework.NativeMethods.WINSAT_ASSESSMENT_TYPE.WINSAT_ASSESSMENT_DISK).Score * 10.0));
          UnsafeNativeMethods.Set(this.sqmSessionHandle, 31U, (uint) ((double) info.GetAssessmentInfo(Microsoft.Expression.Framework.NativeMethods.WINSAT_ASSESSMENT_TYPE.WINSAT_ASSESSMENT_GRAPHICS).Score * 10.0));
          UnsafeNativeMethods.Set(this.sqmSessionHandle, 32U, (uint) ((double) info.GetAssessmentInfo(Microsoft.Expression.Framework.NativeMethods.WINSAT_ASSESSMENT_TYPE.WINSAT_ASSESSMENT_MEMORY).Score * 10.0));
        }
        catch (DirectoryNotFoundException ex)
        {
          UnsafeNativeMethods.Set(this.sqmSessionHandle, 26U, 0U);
        }
        catch (FileNotFoundException ex)
        {
          UnsafeNativeMethods.Set(this.sqmSessionHandle, 26U, 0U);
        }
        catch (Exception ex)
        {
          UnsafeNativeMethods.Set(this.sqmSessionHandle, 26U, 3U);
        }
      }
    }

    private bool? RetrieveRegistryBoolValue(string keyName, string valueName)
    {
      try
      {
        using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(keyName))
        {
          if (registryKey == null)
            return new bool?(false);
          object obj = registryKey.GetValue(valueName);
          if (obj == null)
            return new bool?(false);
          if (obj is int)
            return new bool?((int) obj != 0);
          return new bool?();
        }
      }
      catch (Exception ex)
      {
        if (ex is ArgumentException || ex is IOException || (ex is ObjectDisposedException || ex is SecurityException) || ex is UnauthorizedAccessException)
          return new bool?();
        throw;
      }
    }

    public void Stop()
    {
      if (this.programaticClose)
        return;
      this.EndFeedbackSession(true);
    }

    private void EndFeedbackSession(bool normalExit)
    {
      if (this.sqmSessionHandle == null && (this.sqmSessionHandle == null || !this.sqmSessionHandle.IsInvalid && !this.sqmSessionHandle.IsClosed))
        return;
      if (!UnsafeNativeMethods.Set(this.sqmSessionHandle, 4U, normalExit ? 0U : 1U))
        Marshal.GetLastWin32Error();
      string feedbackPath = FeedbackService.FeedbackPath;
      try
      {
        this.sqmSessionHandle.FilePattern = this.EndSessionFileNamePattern;
      }
      catch (ArgumentException ex)
      {
      }
      try
      {
        if (!UnsafeNativeMethods.EndSession(this.sqmSessionHandle, this.EndSessionFileNamePattern, HSQMSESSION.MaxFilesToQueue, 10U))
          Marshal.GetLastWin32Error();
      }
      catch (Exception ex)
      {
      }
      this.sqmSessionHandle = (HSQMSESSION) null;
    }

    internal static string SqmErrorCodeToString(int errorCode)
    {
      switch (errorCode)
      {
        case 6:
          return "Invalid Handle";
        case 87:
          return "Invalid Parameter";
        case 268435713:
          return "Session Not Initialized";
        case 268435714:
          return "Session Out Of Context";
        case 268435715:
          return "Session Disabled";
        case 268435716:
          return "Data Type Mismatch";
        case 268435717:
          return "Timer Not Started";
        case 268435718:
          return "Session Not FOund";
        case 268435719:
          return "Data Not Found";
        case 268435720:
          return "File Not Found";
        case 268435721:
          return "Upload Timeout";
        case 268435722:
          return "Initialization Failed";
        case 268435723:
          return "Session Full";
        case 268435724:
          return "Network Not Available";
        case 268435725:
          return "Stream Full";
        default:
          return "Unknow SQM Error";
      }
    }

    private void ProcessFeedbackFiles()
    {
      bool? aggregateFeedbackPolicy = this.AggregateFeedbackPolicy;
      if ((!aggregateFeedbackPolicy.GetValueOrDefault() ? false : (aggregateFeedbackPolicy.HasValue ? true : false)) != false)
        this.UploadSqmFiles();
      else
        this.DeleteFeedbackFiles();
    }

    private void UploadSqmFiles()
    {
      UnsafeNativeMethods.UploadCallBack pfnCallback = new UnsafeNativeMethods.UploadCallBack(this.CheckCurrentUploadCallback);
      string szUrl = "http://sqm.microsoft.com/sqm/expressionsuite/sqmserver.dll";
      string szSecureUrl = "http://sqm.microsoft.com/sqm/expressionsuite/sqmserver.dll";
      switch (FeedbackService.Pipeline)
      {
        case 0:
          szUrl = "http://sqm.microsoft.com/sqm/expressionsuite/sqmserver.dll";
          szSecureUrl = "http://sqm.microsoft.com/sqm/expressionsuite/sqmserver.dll";
          break;
        case 1:
          szUrl = "http://expsqmvmsh/sqmtest/sqmserver.dll";
          szSecureUrl = "http://expsqmvmsh/sqmtest/sqmserver.dll";
          break;
        case 2:
          szUrl = "http://expsqmtest/sqmtest/sqmserver.dll";
          szSecureUrl = "http://expsqmtest/sqmtest/sqmserver.dll";
          break;
      }
      this.filesToBeUploaded = UnsafeNativeMethods.StartUpload(Path.Combine(FeedbackService.FeedbackPath, this.UploadFileNamePattern), szUrl, szSecureUrl, 6U, pfnCallback);
      UnsafeNativeMethods.WaitForUploadComplete((uint) this.Timeout, 2U);
      GC.KeepAlive((object) pfnCallback);
    }

    private void DeleteFeedbackFiles()
    {
      foreach (string path in Directory.GetFiles(FeedbackService.FeedbackPath, this.LoggingFileName + "*.sqm"))
      {
        try
        {
          File.Delete(path);
        }
        catch (Exception ex)
        {
        }
      }
    }

    private uint CheckCurrentUploadCallback(uint hResult, [MarshalAs(UnmanagedType.LPWStr)] string filePath, uint dwHttpResponse)
    {
      if ((int) hResult == 0)
      {
        if ((int) dwHttpResponse == 200)
        {
          --this.filesToBeUploaded;
          return 0U;
        }
        if ((int) dwHttpResponse == 403)
        {
          --this.filesToBeUploaded;
          return 1U;
        }
        if ((int) dwHttpResponse != 0 || (int) this.filesToBeUploaded != 0)
          return 1U;
        this.programaticClose = true;
        return 1U;
      }
      ErrorHandler.HResultToString((int) hResult);
      return 1U;
    }

    private void UploadWorker(object parameter)
    {
      Thread.Sleep(this.pauseTimeBeforeUploading);
      this.ProcessFeedbackFiles();
    }

    private bool? RetrieveExpressionCustomerFeedbackRecordingPolicy()
    {
      return RegistryHelper.RetrieveCurrentUserRegistryValue<bool?>(this.CustomerFeedbackRegistryPath, this.CustomerFeedbackPolicyRegistryKeyName);
    }

    private bool? RetrieveExpressionGroupFeedbackRecordingPolicy()
    {
      return RegistryHelper.RetrieveCurrentUserRegistryValue<bool?>(this.GroupFeedbackRegistryPath, this.CustomerFeedbackPolicyRegistryKeyName);
    }

    private void SetExpressionCustomerFeedbackDataRecordingPolicy(bool? value)
    {
      RegistryHelper.SetCurrentUserRegistryValue<bool?>(this.CustomerFeedbackRegistryPath, this.CustomerFeedbackPolicyRegistryKeyName, value);
      bool? nullable = value;
      this.IsLogging = nullable.GetValueOrDefault() && nullable.HasValue;
      if (this.IsLogging)
      {
        this.DeleteFeedbackFiles();
        this.StartFeedbackRecording();
      }
      else
        this.Stop();
    }

    private enum ExitType
    {
      NormalExit,
      CrashExit,
    }

    private enum UserType
    {
      None,
      Error,
      Unknown,
      Unavailable,
      External,
      Internal,
      TestLab,
    }

    private enum WinSatState
    {
      Unavailable,
      Valid,
      IncoherentWithHardware,
      Error,
    }

    private enum SqmDpi
    {
      CustomDpiLessThan192 = 0,
      CustomDpiGreaterThan192 = 1,
      Dpi72 = 72,
      Dpi96 = 96,
      Dpi120 = 120,
      Dpi144 = 144,
      Dpi192 = 192,
    }
  }
}
