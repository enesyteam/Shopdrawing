// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Documents.ViewChangedEventArgs
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;

namespace Microsoft.Expression.Framework.Documents
{
  public sealed class ViewChangedEventArgs : EventArgs
  {
    private IView oldView;
    private IView newView;

    public IView OldView
    {
      get
      {
        return this.oldView;
      }
    }

    public IView NewView
    {
      get
      {
        return this.newView;
      }
    }

    public ViewChangedEventArgs(IView oldView, IView newView)
    {
      this.oldView = oldView;
      this.newView = newView;
    }
  }
}
