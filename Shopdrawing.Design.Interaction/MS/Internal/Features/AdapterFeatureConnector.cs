// Decompiled with JetBrains decompiler
// Type: MS.Internal.Features.AdapterFeatureConnector
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Services;
using System;

namespace MS.Internal.Features
{
  internal class AdapterFeatureConnector : FeatureConnector<Adapter>
  {
    public AdapterFeatureConnector(FeatureManager manager)
      : base(manager)
    {
      this.Context.Services.Publish<AdapterService>((PublishServiceCallback<AdapterService>) (() => (AdapterService) new AdapterFeatureConnector.AdapterServiceImpl(this)));
    }

    private class AdapterServiceImpl : AdapterService
    {
      private AdapterFeatureConnector _server;

      internal AdapterServiceImpl(AdapterFeatureConnector server)
      {
        this._server = server;
      }

      private bool FilterExtension(Type extensionType)
      {
        return new RequirementValidator(this._server.Manager, extensionType).MeetsRequirements;
      }

      public override Adapter GetAdapter(Type adapterType, Type itemType)
      {
        foreach (Adapter adapter in this._server.Manager.CreateFeatureProviders(adapterType, itemType, new Predicate<Type>(this.FilterExtension)))
        {
          if (adapter.AdapterType == adapterType)
            return adapter;
        }
        return (Adapter) null;
      }
    }
  }
}
