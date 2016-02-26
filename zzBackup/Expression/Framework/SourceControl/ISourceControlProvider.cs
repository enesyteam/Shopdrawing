// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.SourceControl.ISourceControlProvider
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Expression.Framework.SourceControl
{
  public interface ISourceControlProvider
  {
    string Identifier { get; }

    object NativeWorkspaceInfo { get; }

    event EventHandler<SourceControlOnlineEventArgs> OnlineStatusChanged;

    void Initialize();

    void Uninitialize();

    SourceControlSuccess OpenProject(string localPath, bool silent);

    SourceControlSuccess GetFiles(string[] fileNames, ref string version, SourceControlGetOption options);

    SourceControlSuccess Checkout(string[] fileNames);

    SourceControlSuccess RevertChange(string[] fileNames, out Dictionary<string, string> changedFilenames);

    SourceControlSuccess CheckIn(string[] fileNames);

    SourceControlSuccess Rename(string[] fileNames, string[] listOfNewNames, bool moveFiles);

    SourceControlSuccess Rename(string[] fileNames, string[] listOfNewNames, bool moveFiles, out bool[] results);

    SourceControlSuccess Add(string[] fileNames);

    SourceControlSuccess Remove(string[] fileNames);

    SourceControlSuccess QueryInfo(string[] fileNames, SourceControlStatus[] status);

    SourceControlSuccess QueryInfoRecursive(string path, out string[] fileNames, out SourceControlStatus[] status);

    SourceControlSuccess Diff(string[] fileNames);

    SourceControlSuccess History(string[] fileNames);

    SourceControlOnlineStatus GetOnlineStatus();

    SourceControlSuccess SetOnlineStatus(SourceControlOnlineStatus online);

    bool ExistsOnServer(string fileName);

    SourceControlSuccess ResolveConflicts(string[] fileNames);
  }
}
