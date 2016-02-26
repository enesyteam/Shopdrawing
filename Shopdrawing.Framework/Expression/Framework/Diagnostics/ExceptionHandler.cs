// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Diagnostics.ExceptionHandler
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

namespace Microsoft.Expression.Framework.Diagnostics
{
  public static class ExceptionHandler
  {
    private static bool isEnabled = true;
    private static bool waitingForLayoutArrange;

    public static bool IsEnabled
    {
      get
      {
        return ExceptionHandler.isEnabled;
      }
      set
      {
        ExceptionHandler.isEnabled = value;
      }
    }

    public static void SafelyForceLayoutArrange()
    {
      if (ExceptionHandler.waitingForLayoutArrange)
        return;
      Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (Delegate) new DispatcherOperationCallback(ExceptionHandler.ForceLayoutArrangeWorker), (object) null);
      ExceptionHandler.waitingForLayoutArrange = true;
    }

    private static object ForceLayoutArrangeWorker(object obj)
    {
      Window mainWindow = Application.Current.MainWindow;
      if (mainWindow != null)
        mainWindow.Arrange(new Rect(0.0, 0.0, mainWindow.ActualWidth, mainWindow.ActualHeight));
      ExceptionHandler.waitingForLayoutArrange = false;
      return (object) null;
    }

    public static void Attach(AppDomain domain)
    {
      if (domain == null)
        throw new ArgumentNullException("domain");
      domain.UnhandledException += new UnhandledExceptionEventHandler(ExceptionHandler.AppDomain_UnhandledException);
    }

    public static void Detach(AppDomain domain)
    {
      if (domain == null)
        throw new ArgumentNullException("domain");
      domain.UnhandledException -= new UnhandledExceptionEventHandler(ExceptionHandler.AppDomain_UnhandledException);
    }

    public static void Report(Exception exception)
    {
      if (!ExceptionHandler.IsEnabled)
        return;
      ExceptionHandler.DumpReport(exception);
    }

    private static void AppDomain_UnhandledException(object sender, UnhandledExceptionEventArgs args)
    {
      if (!ExceptionHandler.IsEnabled)
        return;
      ExceptionHandler.DumpReport((Exception) args.ExceptionObject);
    }

    private static void DumpReport(Exception exception)
    {
      StringWriter stringWriter = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture);
      stringWriter.WriteLine("Microsoft® Expression® crash report:");
      stringWriter.WriteLine();
      DateTime now = DateTime.Now;
      stringWriter.WriteLine("Date: " + now.Year.ToString("D4", (IFormatProvider) CultureInfo.InvariantCulture) + "-" + now.Month.ToString("D2", (IFormatProvider) CultureInfo.InvariantCulture) + "-" + now.Day.ToString("D2", (IFormatProvider) CultureInfo.InvariantCulture) + " " + now.Hour.ToString("D2", (IFormatProvider) CultureInfo.InvariantCulture) + "-" + now.Minute.ToString("D2", (IFormatProvider) CultureInfo.InvariantCulture) + "-" + now.Second.ToString("D2", (IFormatProvider) CultureInfo.InvariantCulture));
      stringWriter.WriteLine("User: " + Environment.UserName + "@" + Environment.MachineName);
      stringWriter.WriteLine();
      stringWriter.WriteLine("Application Version: " + typeof (ExceptionHandler).Assembly.GetName().Version.ToString());
      stringWriter.WriteLine("Operating System Version: " + Environment.OSVersion.ToString());
      stringWriter.WriteLine("Common Language Runtime Version: " + Environment.Version.ToString());
      stringWriter.WriteLine();
      stringWriter.WriteLine(exception.ToString());
      stringWriter.WriteLine();
      Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
      foreach (Assembly assembly in assemblies)
        stringWriter.WriteLine(assembly.GetName().FullName);
      stringWriter.WriteLine();
      foreach (Assembly assembly in assemblies)
      {
        bool flag = false;
        try
        {
          if (assembly.Location.Length != 0)
          {
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            stringWriter.Write(versionInfo.ToString());
            flag = true;
          }
        }
        catch
        {
        }
        if (!flag)
          stringWriter.WriteLine(assembly.ToString());
        stringWriter.WriteLine();
      }
      string text = stringWriter.ToString();
      stringWriter.Close();
      Dump.Write(text);
    }
  }
}
