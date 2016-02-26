// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.WebServer.WebServerService
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Extensibility;
using Microsoft.Expression.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Microsoft.Expression.Framework.WebServer
{
  public class WebServerService : IWebServerService
  {
    private static List<IntPtr> g_FindWindowDataHwnds = new List<IntPtr>();
    private List<WebServerService.BrowsingSession> _listSessions = new List<WebServerService.BrowsingSession>();
    private IServices services;
    private static int g_Handle;

    public WebServerService(IServices services)
    {
      this.services = services;
    }

    public int StartServer(IWebServerSettings settings)
    {
      foreach (WebServerService.BrowsingSession browsingSession in this._listSessions)
      {
        if (string.CompareOrdinal(browsingSession.localPath, settings.LocalPath) == 0 && !browsingSession.process.HasExited)
          return browsingSession.Handle;
      }
      WebServerService.BrowsingSession browsingSession1 = new WebServerService.BrowsingSession();
      if (Microsoft.Expression.Framework.Documents.PathHelper.PathEndsInDirectorySeparator(settings.LocalPath))
        settings.LocalPath = Microsoft.Expression.Framework.Documents.PathHelper.TrimTrailingDirectorySeparators(settings.LocalPath);
      browsingSession1.localPath = settings.LocalPath;
      int num = settings.Port;
      if (num == 0)
        num = this.GetNextAvailPort();
      ProcessStartInfo startInfo = new ProcessStartInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Microsoft.Expression.WebServer.exe", " /port:" + (object) num + (" /path:\"" + settings.LocalPath + "\"") + (" /vpath:\"" + settings.VirtualPath + "\"") + (settings.ShowTrayIcon ? (string) null : " /nosystray") + (!string.IsNullOrEmpty(settings.PhpServerExe) ? " /php:\"" + settings.PhpServerExe + "\"" : (string) null) + (settings.Silent ? " /silent" : (string) null) + (settings.UseNtlmAuthentication ? " /ntlm" : (string) null) + (settings.ShowDirectoryListing ? (string) null : " /nodirlist"));
      try
      {
        browsingSession1.process = Process.Start(startInfo);
        browsingSession1.Uri = "http://localhost:" + (object) num + "/";
        browsingSession1.Handle = ++WebServerService.g_Handle;
        this._listSessions.Add(browsingSession1);
      }
      catch (Win32Exception ex)
      {
        IMessageDisplayService service = this.services.GetService<IMessageDisplayService>();
        if (service != null)
          service.ShowError(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.WebServerCannotLaunch, new object[1]
          {
            (object) settings.LocalPath
          }));
        return 0;
      }
      return browsingSession1.Handle;
    }

    public bool IsServerReachable(int handle, int timeout)
    {
      WebServerService.BrowsingSession browsingSession = this.GetBrowsingSession(handle);
      if (browsingSession != null && !browsingSession.process.HasExited)
        return this.CanConnectToPort(new Uri(browsingSession.Uri).Port, timeout);
      return false;
    }

    private bool CanConnectToPort(int port, int timeout)
    {
      bool flag = false;
      Socket socket = (Socket) null;
      try
      {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint ipEndPoint1 = new IPEndPoint(IPAddress.Any, 0);
        socket.Bind((EndPoint) ipEndPoint1);
        int tickCount = Environment.TickCount;
        IPEndPoint ipEndPoint2 = new IPEndPoint(IPAddress.Loopback, port);
        while (!flag)
        {
          try
          {
            socket.Blocking = false;
            socket.Connect((EndPoint) ipEndPoint2);
          }
          catch (SocketException ex)
          {
            ArrayList arrayList = new ArrayList();
            arrayList.Add((object) socket);
            Socket.Select((IList) null, (IList) arrayList, (IList) null, 1000);
            if (arrayList.Count == 1)
              flag = true;
          }
          socket.Blocking = true;
          if (Environment.TickCount - tickCount > timeout)
            break;
        }
      }
      finally
      {
        if (socket != null)
          socket.Close();
      }
      return flag;
    }

    public string GetSessionAddress(int handle)
    {
      return this.GetSessionAddress(handle, (string) null);
    }

    public string GetSessionAddress(int handle, string localPath)
    {
      WebServerService.BrowsingSession browsingSession = this.GetBrowsingSession(handle);
      if (browsingSession == null)
        return (string) null;
      if (localPath == null)
        return browsingSession.Uri;
      string originalString = new Uri(browsingSession.localPath + (object) Path.DirectorySeparatorChar, UriKind.Absolute).MakeRelativeUri(new Uri(localPath, UriKind.Absolute)).OriginalString;
      return new Uri(browsingSession.Uri + originalString, UriKind.Absolute).OriginalString;
    }

    public void StopAllSessions()
    {
      foreach (WebServerService.BrowsingSession browsingSession in this._listSessions)
        this.StopBrowsingSession(browsingSession.process);
      this._listSessions.Clear();
    }

    public void StopBrowsingSession(int handle)
    {
      foreach (WebServerService.BrowsingSession browsingSession in this._listSessions)
      {
        if (browsingSession.Handle == handle)
        {
          this.StopBrowsingSession(browsingSession.process);
          this._listSessions.Remove(browsingSession);
          break;
        }
      }
    }

    private WebServerService.BrowsingSession GetBrowsingSession(int handle)
    {
      foreach (WebServerService.BrowsingSession browsingSession in this._listSessions)
      {
        if (browsingSession.Handle == handle)
          return browsingSession;
      }
      return (WebServerService.BrowsingSession) null;
    }

    private void StopBrowsingSession(Process process)
    {
      if (process == null || process.HasExited)
        return;
      WebServerService.g_FindWindowDataHwnds.Clear();
      if (this.FindWindows(process.Id))
      {
        IntPtr pdwResult;
        foreach (IntPtr handle in WebServerService.g_FindWindowDataHwnds)
        {
          if (Microsoft.Expression.Framework.NativeMethods.IsWindow(new HandleRef((object) this, handle)) != 0)
            Microsoft.Expression.Framework.NativeMethods.SendMessageTimeout(new HandleRef((object) this, handle), 17, IntPtr.Zero, IntPtr.Zero, 2, 2000, out pdwResult);
        }
        foreach (IntPtr handle in WebServerService.g_FindWindowDataHwnds)
        {
          if (Microsoft.Expression.Framework.NativeMethods.IsWindow(new HandleRef((object) this, handle)) != 0)
            Microsoft.Expression.Framework.NativeMethods.SendMessageTimeout(new HandleRef((object) this, handle), 16, IntPtr.Zero, IntPtr.Zero, 2, 2000, out pdwResult);
        }
      }
      if (process != null && !process.HasExited && !process.WaitForExit(5000))
        process.Kill();
      process.Close();
    }

    public bool EnumWindowsCallback(IntPtr hwnd, IntPtr processId)
    {
      int lpdwProcessId = 0;
      if (Microsoft.Expression.Framework.NativeMethods.GetWindowThreadProcessId(new HandleRef((object) this, hwnd), out lpdwProcessId) != 0 && lpdwProcessId == (int) processId)
        WebServerService.g_FindWindowDataHwnds.Add(hwnd);
      return true;
    }

    private bool FindWindows(int processId)
    {
      if (processId == 0)
        return false;
      WebServerService.g_FindWindowDataHwnds.Clear();
      return Microsoft.Expression.Framework.NativeMethods.EnumWindows(new EnumWindowsCB(this.EnumWindowsCallback), (IntPtr) processId);
    }

    private int GetNextAvailPort()
    {
      int num = 0;
      Socket socket = (Socket) null;
      try
      {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 0);
        socket.Bind((EndPoint) ipEndPoint);
        num = ((IPEndPoint) socket.LocalEndPoint).Port;
      }
      catch (SocketException ex)
      {
      }
      finally
      {
        if (socket != null)
          socket.Close();
      }
      return num;
    }

    private class BrowsingSession
    {
      public int Handle { get; set; }

      public string localPath { get; set; }

      public Process process { get; set; }

      public string Uri { get; set; }
    }
  }
}
