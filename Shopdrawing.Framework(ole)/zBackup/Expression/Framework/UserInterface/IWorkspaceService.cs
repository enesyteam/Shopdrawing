// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.UserInterface.IWorkspaceService
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.Commands;
using System;
using System.ComponentModel;
using System.Windows;

namespace Microsoft.Expression.Framework.UserInterface
{
  public interface IWorkspaceService : ICommandTarget
  {
    FrameworkElement Content { get; set; }

    IWorkspace ActiveWorkspace { get; }

    event CancelEventHandler ActiveWorkspaceChanging;

    event EventHandler ActiveWorkspaceChangingCanceled;

    event EventHandler ActiveWorkspaceChanged;

    void LoadConfiguration();

    void LoadConfiguration(bool useDefaultWorkspace);

    void SaveConfiguration();

    void SaveConfiguration(bool displayErrorMessages);
  }
}
