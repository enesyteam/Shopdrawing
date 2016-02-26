// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.Controls.FloatingOperationFinishedEventArgs
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI.Shell;
using System;
using System.Windows;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
  public abstract class FloatingOperationFinishedEventArgs : RoutedEventArgs
  {
    public ViewElement Content { get; private set; }

    protected FloatingOperationFinishedEventArgs(RoutedEvent routedEvent, ViewElement content)
      : base(routedEvent)
    {
      if (content == null)
        throw new ArgumentNullException("content");
      this.Content = content;
    }
  }
}
