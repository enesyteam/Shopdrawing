// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Assets.AssetProvider
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Data;
using System;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.Tools.Assets
{
  internal abstract class AssetProvider : IDisposable
  {
    private ObservableCollectionWorkaround<Asset> assets = new ObservableCollectionWorkaround<Asset>();
    private bool needsUpdate = true;
    private bool updateLock;
    private bool disposed;

    public ObservableCollectionWorkaround<Asset> Assets
    {
      get
      {
        return this.assets;
      }
    }

    public bool NeedsUpdate
    {
      get
      {
        return this.needsUpdate;
      }
      protected internal set
      {
        if (this.needsUpdate == value)
          return;
        this.needsUpdate = value;
        if (this.NeedsUpdateChanged == null)
          return;
        this.NeedsUpdateChanged((object) this, EventArgs.Empty);
      }
    }

    public event EventHandler AssetsChanged;

    public event EventHandler NeedsUpdateChanged;

    ~AssetProvider()
    {
      this.Dispose(false);
    }

    internal bool Update()
    {
      if (this.updateLock)
        return false;
      this.updateLock = true;
      try
      {
        return this.UpdateAssets();
      }
      finally
      {
        this.updateLock = false;
      }
    }

    protected virtual bool UpdateAssets()
    {
      this.NeedsUpdate = false;
      return false;
    }

    protected void NotifyAssetsChanged()
    {
      if (this.AssetsChanged == null)
        return;
      UIThreadDispatcher.Instance.Invoke(DispatcherPriority.Send, (Action) (() => this.AssetsChanged((object) this, EventArgs.Empty)));
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    private void Dispose(bool disposing)
    {
      if (this.disposed)
        return;
      this.disposed = true;
      this.InternalDispose(disposing);
    }

    protected virtual void InternalDispose(bool disposing)
    {
    }
  }
}
