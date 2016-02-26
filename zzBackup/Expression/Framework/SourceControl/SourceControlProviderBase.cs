// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.SourceControl.SourceControlProviderBase
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.Expression.Framework.SourceControl
{
  public abstract class SourceControlProviderBase : ISourceControlProvider
  {
    protected IServiceProvider serviceProvider;
    protected ISourceControlService sourceControlService;
    private SourceControlOnlineStatus onlineStatus;
    protected string activeProjectPath;

    public abstract object NativeWorkspaceInfo { get; }

    protected SourceControlOnlineStatus OnlineStatus
    {
      get
      {
        return this.onlineStatus;
      }
      set
      {
        if (this.onlineStatus == value)
          return;
        this.onlineStatus = value;
        this.OnOnlineStatusChanged();
      }
    }

    public abstract string Identifier { get; }

    public event EventHandler<SourceControlOnlineEventArgs> OnlineStatusChanged;

    protected SourceControlProviderBase(IServiceProvider serviceProvider)
    {
      this.serviceProvider = serviceProvider;
      this.sourceControlService = (ISourceControlService) serviceProvider.GetService(typeof (ISourceControlService));
    }

    private void OnOnlineStatusChanged()
    {
      if (this.OnlineStatusChanged == null)
        return;
      this.OnlineStatusChanged((object) this, new SourceControlOnlineEventArgs(this.OnlineStatus, (ISourceControlProvider) this));
    }

    public SourceControlOnlineStatus GetOnlineStatus()
    {
      return this.OnlineStatus;
    }

    public SourceControlSuccess SetOnlineStatus(SourceControlOnlineStatus status)
    {
      if (this.OnlineStatus == status)
        return SourceControlSuccess.Success;
      if (status == SourceControlOnlineStatus.Online)
        return this.OpenProject(this.activeProjectPath, false);
      this.OnlineStatus = status;
      return SourceControlSuccess.Success;
    }

    public abstract void Initialize();

    public abstract void Uninitialize();

    public abstract SourceControlSuccess OpenProject(string localPath, bool silent);

    public abstract SourceControlSuccess GetFiles(string[] fileNames, ref string version, SourceControlGetOption options);

    public abstract SourceControlSuccess Checkout(string[] fileNames);

    public abstract SourceControlSuccess RevertChange(string[] fileNames, out Dictionary<string, string> changedFilenames);

    public abstract SourceControlSuccess CheckIn(string[] fileNames);

    public abstract SourceControlSuccess Rename(string[] fileNames, string[] listOfNewNames, bool moveFiles);

    public abstract SourceControlSuccess Rename(string[] fileNames, string[] listOfNewNames, bool moveFiles, out bool[] results);

    public abstract SourceControlSuccess Add(string[] fileNames);

    public abstract SourceControlSuccess Remove(string[] fileNames);

    public abstract SourceControlSuccess QueryInfo(string[] fileNames, SourceControlStatus[] status);

    public abstract SourceControlSuccess QueryInfoRecursive(string path, out string[] fileNames, out SourceControlStatus[] status);

    public abstract SourceControlSuccess Diff(string[] fileNames);

    public abstract SourceControlSuccess History(string[] fileNames);

    public abstract SourceControlSuccess ResolveConflicts(string[] fileNames);

    public abstract bool ExistsOnServer(string fileName);

    protected void LogMessage(string message)
    {
      this.sourceControlService.LogMessage(message);
    }

    [Conditional("DEBUG")]
    protected void LogDebugMessage(string message)
    {
    }
  }
}
