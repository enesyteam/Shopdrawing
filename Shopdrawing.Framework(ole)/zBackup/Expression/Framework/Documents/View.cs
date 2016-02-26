// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Documents.View
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.Commands;
using System;
using System.ComponentModel;

namespace Microsoft.Expression.Framework.Documents
{
  public class View : CommandTarget, IView, INotifyPropertyChanged, IDisposable
  {
    public virtual string Caption
    {
      get
      {
        return string.Empty;
      }
    }

    public virtual bool IsDirty
    {
      get
      {
        return false;
      }
    }

    public virtual string TabToolTip
    {
      get
      {
        return string.Empty;
      }
    }

    public virtual object ActiveEditor
    {
      get
      {
        return (object) null;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected View()
    {
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
    }

    public virtual void Initialize()
    {
    }

    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, e);
    }

    public virtual void Deactivating()
    {
    }

    public virtual void Activated()
    {
    }

    public virtual void Deactivated()
    {
    }

    public virtual void ReturnFocus()
    {
    }
  }
}
