// Decompiled with JetBrains decompiler
// Type: Shopdrawing.App.BlendServer
// Assembly: Shopdrawing.App, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: DDD2F1CF-BB6D-4068-A4D9-DDBDD16D6739
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Shopdrawing.App.dll

using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Windows;
using System.Windows.Threading;

namespace Shopdrawing.App
{
  internal class BlendServer : MarshalByRefObject, IBlendServer
  {
    private static readonly string BlendServerPipe = "BlendLocalPipe";
    private static IpcServerChannel serverChannel;
    private static BlendServer remoteHelper;
    private IProjectManager projectManager;
    private Microsoft.Expression.Framework.UserInterface.IWindowService windowService;
    private ICommandLineService commandLineService;
    private TimeSpan defaultWait;

    public BlendServer(IServiceProvider serviceProvider)
    {
      this.windowService = (Microsoft.Expression.Framework.UserInterface.IWindowService) serviceProvider.GetService(typeof (Microsoft.Expression.Framework.UserInterface.IWindowService));
      this.projectManager = (IProjectManager) serviceProvider.GetService(typeof (IProjectManager));
      this.commandLineService = (ICommandLineService) serviceProvider.GetService(typeof (ICommandLineService));
      this.defaultWait = new TimeSpan(0, 0, 3);
    }

    internal static void StartRemoteService(IServiceProvider serviceProvider)
    {
      Process currentProcess = Process.GetCurrentProcess();
      BlendServer.remoteHelper = new BlendServer(serviceProvider);
      BlendServer.serverChannel = new IpcServerChannel(BlendServer.BlendServerPipe + currentProcess.Id.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      ChannelServices.RegisterChannel((IChannel) BlendServer.serverChannel, false);
      RemotingServices.Marshal((MarshalByRefObject) BlendServer.remoteHelper, typeof (IBlendServer).Name);
    }

    internal static void StopRemoteService()
    {
      RemotingServices.Disconnect((MarshalByRefObject) BlendServer.remoteHelper);
      BlendServer.serverChannel.StopListening((object) null);
      ChannelServices.UnregisterChannel((IChannel) BlendServer.serverChannel);
    }

    public override object InitializeLifetimeService()
    {
      return (object) null;
    }

    public bool CanProcessCommandLineArgs(string[] args)
    {
      DispatcherOperation dispatcherOperation = UIThreadDispatcher.Instance.BeginInvoke<string[], bool>(DispatcherPriority.Input, new Func<string[], bool>(this.HandleCanProcessCommandLineArgs), args);
      int num = (int) dispatcherOperation.Wait(this.defaultWait);
      if (dispatcherOperation.Status != DispatcherOperationStatus.Completed)
        return false;
      return (bool) dispatcherOperation.Result;
    }

    public void ProcessCommandLineArgs(string[] args)
    {
      UIThreadDispatcher.Instance.BeginInvoke<object>(DispatcherPriority.Input, (Func<object>) (() => this.HandleProcessCommandLineArgs((object) args)));
    }

    private bool HandleCanProcessCommandLineArgs(string[] args)
    {
      string[] arguments = this.commandLineService.GetArguments(string.Empty, args);
      bool flag = false;
      if (arguments != null)
      {
        if (arguments.Length == 1)
        {
          try
          {
            string fullPath = Path.GetFullPath(arguments[0]);
            if (Microsoft.Expression.Framework.Documents.PathHelper.FileOrDirectoryExists(fullPath))
            {
              DocumentReference documentReference = DocumentReference.Create(fullPath);
              if (this.projectManager.CurrentSolution == null || this.projectManager.CurrentSolution.DocumentReference.Path.Equals(fullPath, StringComparison.OrdinalIgnoreCase) || this.projectManager.CurrentSolution.FindProject(documentReference) != null)
              {
                flag = true;
              }
              else
              {
                foreach (IProject project in this.projectManager.CurrentSolution.Projects)
                {
                  if (project.FindItem(documentReference) != null)
                  {
                    flag = true;
                    break;
                  }
                }
              }
            }
          }
          catch (Exception ex)
          {
            if (!ErrorHandling.ShouldHandleExceptions(ex))
              throw;
            else
              flag = false;
          }
        }
      }
      return flag;
    }

    private object HandleProcessCommandLineArgs(object arg)
    {
      string[] args = (string[]) arg;
      Window mainWindow = this.windowService.MainWindow;
      if (mainWindow.WindowState == WindowState.Minimized)
        mainWindow.WindowState = WindowState.Normal;
      mainWindow.Activate();
      this.projectManager.InitializeFromKnownProjects(args);
      return (object) null;
    }
  }
}
