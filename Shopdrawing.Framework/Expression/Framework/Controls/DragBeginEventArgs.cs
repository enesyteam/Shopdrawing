// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Controls.DragBeginEventArgs
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Windows;

namespace Microsoft.Expression.Framework.Controls
{
  public class DragBeginEventArgs : EventArgs
  {
    private UIElement dragSource;

    public UIElement DragSource
    {
      get
      {
        return this.dragSource;
      }
    }

    public bool Handled { get; set; }

    public DragBeginEventArgs(UIElement dragSource)
    {
      this.dragSource = dragSource;
    }
  }
}
