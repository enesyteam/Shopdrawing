// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.DisposableObject
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.PlatformUI
{
  [ComVisible(true)]
  public class DisposableObject : IDisposable
  {
    private EventHandler _disposing;

    public bool IsDisposed { get; private set; }

    public event EventHandler Disposing
    {
      add
      {
        this.ThrowIfDisposed();
        this._disposing += value;
      }
      remove
      {
        this.ThrowIfDisposed();
        this._disposing -= value;
      }
    }

    ~DisposableObject()
    {
      this.Dispose(false);
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected void ThrowIfDisposed()
    {
      if (this.IsDisposed)
        throw new ObjectDisposedException(this.GetType().Name);
    }

    protected void Dispose(bool disposing)
    {
      if (!this.IsDisposed)
      {
        ExtensionMethods.RaiseEvent(this._disposing, (object) this);
        this._disposing = (EventHandler) null;
        if (disposing)
          this.DisposeManagedResources();
        this.DisposeNativeResources();
      }
      this.IsDisposed = true;
    }

    protected virtual void DisposeManagedResources()
    {
    }

    protected virtual void DisposeNativeResources()
    {
    }
  }
}
