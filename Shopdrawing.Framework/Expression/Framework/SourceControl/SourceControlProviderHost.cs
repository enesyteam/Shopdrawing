// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.SourceControl.SourceControlProviderHost
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Expression.Framework.SourceControl
{
  internal class SourceControlProviderHost : ISourceControlProvider
  {
    private ISourceControlProvider hostedProvider;

    public string Identifier
    {
      get
      {
        return this.hostedProvider.Identifier;
      }
    }

    public object NativeWorkspaceInfo
    {
      get
      {
        return this.hostedProvider.NativeWorkspaceInfo;
      }
    }

    public event EventHandler<SourceControlOnlineEventArgs> OnlineStatusChanged;

    public SourceControlProviderHost(ISourceControlProvider sourceControlProvider)
    {
      if (sourceControlProvider == null)
        throw new ArgumentNullException("sourceControlProvider");
      this.hostedProvider = sourceControlProvider;
      int num = (int) this.hostedProvider.SetOnlineStatus(SourceControlOnlineStatus.None);
      this.hostedProvider.OnlineStatusChanged += new EventHandler<SourceControlOnlineEventArgs>(this.Provider_OnlineStatusChanged);
    }

    private void Provider_OnlineStatusChanged(object sender, SourceControlOnlineEventArgs args)
    {
      if (this.OnlineStatusChanged == null)
        return;
      this.OnlineStatusChanged((object) this, args);
    }

    public void Initialize()
    {
      this.hostedProvider.Initialize();
    }

    public void Uninitialize()
    {
      this.hostedProvider.Uninitialize();
    }

    public SourceControlOnlineStatus GetOnlineStatus()
    {
      return this.hostedProvider.GetOnlineStatus();
    }

    public SourceControlSuccess SetOnlineStatus(SourceControlOnlineStatus online)
    {
      return this.hostedProvider.SetOnlineStatus(online);
    }

    public SourceControlSuccess OpenProject(string localPath, bool silent)
    {
      return this.hostedProvider.OpenProject(localPath, silent);
    }

    public SourceControlSuccess GetFiles(string[] fileNames, ref string version, SourceControlGetOption options)
    {
      if ((fileNames == null || fileNames.Length == 0) && (options & SourceControlGetOption.All) != SourceControlGetOption.All)
        return SourceControlSuccess.Success;
      switch (this.hostedProvider.GetOnlineStatus())
      {
        case SourceControlOnlineStatus.None:
          return SourceControlSuccess.Failed;
        case SourceControlOnlineStatus.Offline:
          return SourceControlSuccess.Offline;
        default:
          return this.hostedProvider.GetFiles(fileNames, ref version, options);
      }
    }

    public SourceControlSuccess Checkout(string[] fileNames)
    {
      if (fileNames == null)
        throw new ArgumentNullException("fileNames");
      if (fileNames.Length == 0)
        return SourceControlSuccess.Success;
      switch (this.hostedProvider.GetOnlineStatus())
      {
        case SourceControlOnlineStatus.None:
          return SourceControlSuccess.Failed;
        case SourceControlOnlineStatus.Offline:
          return SourceControlSuccess.Offline;
        default:
          return this.hostedProvider.Checkout(fileNames);
      }
    }

    public SourceControlSuccess RevertChange(string[] fileNames, out Dictionary<string, string> changedFilenames)
    {
      changedFilenames = new Dictionary<string, string>();
      if (fileNames == null)
        throw new ArgumentNullException("fileNames");
      switch (this.hostedProvider.GetOnlineStatus())
      {
        case SourceControlOnlineStatus.None:
          return SourceControlSuccess.Failed;
        case SourceControlOnlineStatus.Offline:
          return SourceControlSuccess.Offline;
        default:
          return this.hostedProvider.RevertChange(fileNames, out changedFilenames);
      }
    }

    public SourceControlSuccess CheckIn(string[] fileNames)
    {
      if (fileNames == null)
        throw new ArgumentNullException("fileNames");
      switch (this.hostedProvider.GetOnlineStatus())
      {
        case SourceControlOnlineStatus.None:
          return SourceControlSuccess.Failed;
        case SourceControlOnlineStatus.Offline:
          return SourceControlSuccess.Offline;
        default:
          return this.hostedProvider.CheckIn(fileNames);
      }
    }

    public SourceControlSuccess Rename(string[] fileNames, string[] listOfNewNames, bool moveFiles)
    {
      bool[] results;
      return this.Rename(fileNames, listOfNewNames, moveFiles, out results);
    }

    public SourceControlSuccess Rename(string[] fileNames, string[] listOfNewNames, bool moveFiles, out bool[] results)
    {
      if (fileNames == null)
        throw new ArgumentNullException("fileNames");
      if (listOfNewNames == null)
        throw new ArgumentNullException("listOfNewNames");
      if (fileNames.Length != listOfNewNames.Length)
        throw new ArgumentException("fileNames length is different than listOfNewNames length");
      if (fileNames.Length == 0)
      {
        results = new bool[0];
        return SourceControlSuccess.Success;
      }
      switch (this.hostedProvider.GetOnlineStatus())
      {
        case SourceControlOnlineStatus.None:
          results = new bool[fileNames.Length];
          return SourceControlSuccess.Failed;
        case SourceControlOnlineStatus.Offline:
          results = new bool[fileNames.Length];
          return SourceControlSuccess.Offline;
        default:
          return this.hostedProvider.Rename(fileNames, listOfNewNames, moveFiles, out results);
      }
    }

    public SourceControlSuccess Add(string[] fileNames)
    {
      if (fileNames == null)
        throw new ArgumentNullException("fileNames");
      if (fileNames.Length == 0)
        return SourceControlSuccess.Success;
      switch (this.hostedProvider.GetOnlineStatus())
      {
        case SourceControlOnlineStatus.None:
          return SourceControlSuccess.Failed;
        case SourceControlOnlineStatus.Offline:
          return SourceControlSuccess.Offline;
        default:
          return this.hostedProvider.Add(fileNames);
      }
    }

    public SourceControlSuccess Remove(string[] fileNames)
    {
      if (fileNames == null)
        throw new ArgumentNullException("fileNames");
      if (fileNames.Length == 0)
        return SourceControlSuccess.Success;
      switch (this.hostedProvider.GetOnlineStatus())
      {
        case SourceControlOnlineStatus.None:
          return SourceControlSuccess.Failed;
        case SourceControlOnlineStatus.Offline:
          return SourceControlSuccess.Offline;
        default:
          return this.hostedProvider.Remove(fileNames);
      }
    }

    public SourceControlSuccess QueryInfo(string[] fileNames, SourceControlStatus[] status)
    {
      if (fileNames == null)
        throw new ArgumentNullException("fileNames");
      if (status == null)
        throw new ArgumentNullException("status");
      if (fileNames.Length != status.Length)
        throw new ArgumentException("fileNames length is different than status length");
      if (fileNames.Length == 0)
        return SourceControlSuccess.Success;
      for (int index = 0; index < status.GetLength(0); ++index)
        status[index] = SourceControlStatus.None;
      switch (this.hostedProvider.GetOnlineStatus())
      {
        case SourceControlOnlineStatus.None:
          return SourceControlSuccess.Failed;
        case SourceControlOnlineStatus.Offline:
          return SourceControlSuccess.Offline;
        default:
          return this.hostedProvider.QueryInfo(fileNames, status);
      }
    }

    public SourceControlSuccess QueryInfoRecursive(string path, out string[] fileNames, out SourceControlStatus[] status)
    {
      if (path == null)
        throw new ArgumentNullException("path");
      switch (this.hostedProvider.GetOnlineStatus())
      {
        case SourceControlOnlineStatus.None:
          fileNames = (string[]) null;
          status = (SourceControlStatus[]) null;
          return SourceControlSuccess.Failed;
        case SourceControlOnlineStatus.Offline:
          fileNames = (string[]) null;
          status = (SourceControlStatus[]) null;
          return SourceControlSuccess.Offline;
        default:
          return this.hostedProvider.QueryInfoRecursive(path, out fileNames, out status);
      }
    }

    public SourceControlSuccess Diff(string[] fileNames)
    {
      if (fileNames == null)
        throw new ArgumentNullException("fileNames");
      switch (this.hostedProvider.GetOnlineStatus())
      {
        case SourceControlOnlineStatus.None:
          return SourceControlSuccess.Failed;
        case SourceControlOnlineStatus.Offline:
          return SourceControlSuccess.Offline;
        default:
          return this.hostedProvider.Diff(fileNames);
      }
    }

    public SourceControlSuccess History(string[] fileNames)
    {
      if (fileNames == null)
        throw new ArgumentNullException("fileNames");
      switch (this.hostedProvider.GetOnlineStatus())
      {
        case SourceControlOnlineStatus.None:
          return SourceControlSuccess.Failed;
        case SourceControlOnlineStatus.Offline:
          return SourceControlSuccess.Offline;
        default:
          return this.hostedProvider.History(fileNames);
      }
    }

    public SourceControlSuccess ResolveConflicts(string[] fileNames)
    {
      switch (this.hostedProvider.GetOnlineStatus())
      {
        case SourceControlOnlineStatus.None:
          return SourceControlSuccess.Failed;
        case SourceControlOnlineStatus.Offline:
          return SourceControlSuccess.Offline;
        default:
          return this.hostedProvider.ResolveConflicts(fileNames);
      }
    }

    public bool ExistsOnServer(string fileName)
    {
      if (string.IsNullOrEmpty(fileName))
        throw new ArgumentNullException("fileName");
      if (this.hostedProvider.GetOnlineStatus() != SourceControlOnlineStatus.Online)
        return false;
      return this.hostedProvider.ExistsOnServer(fileName);
    }
  }
}
