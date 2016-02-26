// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Features.FeatureConnector`1
// Assembly: Microsoft.Windows.Design.Extensibility, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4673B7C2-4EF5-4715-85F2-D8E573468337
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Extensibility\Microsoft.Windows.Design.Extensibility.dll

using Microsoft.Windows.Design;
using System;
using System.Collections.Generic;

namespace Microsoft.Windows.Design.Features
{
  public abstract class FeatureConnector<TFeatureProviderType> : IDisposable, IFeatureConnectorMarker where TFeatureProviderType : FeatureProvider
  {
    private EditingContext _context;
    private FeatureManager _manager;

    protected EditingContext Context
    {
      get
      {
        return this._context;
      }
    }

    protected FeatureManager Manager
    {
      get
      {
        return this._manager;
      }
    }

    protected FeatureConnector(FeatureManager manager)
    {
      if (manager == null)
        throw new ArgumentNullException("manager");
      this._manager = manager;
      this._context = manager.Context;
    }

    ~FeatureConnector()
    {
      this.Dispose(false);
    }

    protected IEnumerable<TFeatureProviderType> CreateFeatureProviders(Type type)
    {
      foreach (TFeatureProviderType featureProviderType in this._manager.CreateFeatureProviders(typeof (TFeatureProviderType), type))
        yield return featureProviderType;
    }

    protected IEnumerable<TSubtype> CreateFeatureProviders<TSubtype>(Type type) where TSubtype : TFeatureProviderType
    {
      foreach (TSubtype subtype in this._manager.CreateFeatureProviders(typeof (TSubtype), type))
        yield return subtype;
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
    }
  }
}
