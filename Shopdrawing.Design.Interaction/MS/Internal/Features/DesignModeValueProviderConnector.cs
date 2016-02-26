// Decompiled with JetBrains decompiler
// Type: MS.Internal.Features.DesignModeValueProviderConnector
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MS.Internal.Features
{
  internal class DesignModeValueProviderConnector : FeatureConnector<DesignModeValueProvider>
  {
    private DesignModeValueProviderConnector.ValueTranslationServiceImpl _service;

    public DesignModeValueProviderConnector(FeatureManager manager)
      : base(manager)
    {
      this.Context.Services.Publish<ValueTranslationService>((PublishServiceCallback<ValueTranslationService>) (() =>
      {
        this._service = new DesignModeValueProviderConnector.ValueTranslationServiceImpl(this);
        return (ValueTranslationService) this._service;
      }));
      this.Context.Items.Subscribe<AssemblyReferences>((SubscribeContextCallback<AssemblyReferences>) (newReferences =>
      {
        if (this._service == null)
          return;
        this._service.Clear();
      }));
      TypeDescriptor.Refreshed += new RefreshEventHandler(this.TypeDescriptor_Refreshed);
    }

    private void TypeDescriptor_Refreshed(RefreshEventArgs e)
    {
      if (this._service == null)
        return;
      this._service.Clear();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        TypeDescriptor.Refreshed -= new RefreshEventHandler(this.TypeDescriptor_Refreshed);
        if (this._service != null)
        {
          this._service.Clear();
          ((IDisposable) this._service).Dispose();
          this._service = (DesignModeValueProviderConnector.ValueTranslationServiceImpl) null;
        }
      }
      base.Dispose(disposing);
    }

    internal class ValueTranslationServiceImpl : ValueTranslationService, IDisposable
    {
      private DesignModeValueProviderConnector _connector;
      private Dictionary<Type, List<DesignModeValueProvider>> _providers;
      private Dictionary<PropertyIdentifier, PropertyIdentifier> _seenPropertyPool;
      private bool _disposed;

      public override event EventHandler<PropertyInvalidatedEventArgs> PropertyInvalidated;

      internal ValueTranslationServiceImpl(DesignModeValueProviderConnector connector)
      {
        this._connector = connector;
      }

      internal void Clear()
      {
        if (this._providers == null)
          return;
        this._providers.Clear();
      }

      void IDisposable.Dispose()
      {
        this._disposed = true;
      }

      private IEnumerable<DesignModeValueProvider> GetFeatureProvidersInCallOrder(Type type)
      {
        if (this._providers == null)
          this._providers = new Dictionary<Type, List<DesignModeValueProvider>>();
        List<DesignModeValueProvider> list = (List<DesignModeValueProvider>) null;
        if (!this._providers.TryGetValue(type, out list))
        {
          foreach (DesignModeValueProvider modeValueProvider in this._connector.CreateFeatureProviders(type))
          {
            if (list == null)
              list = new List<DesignModeValueProvider>();
            list.Add(modeValueProvider);
          }
          if (list != null)
            list.Reverse();
          this._providers.Add(type, list);
        }
        return (IEnumerable<DesignModeValueProvider>) list;
      }

      public override IEnumerable<PropertyIdentifier> GetProperties(Type itemType)
      {
        if (itemType == null)
          throw new ArgumentNullException("itemType");
        IEnumerable<DesignModeValueProvider> providers = this.GetFeatureProvidersInCallOrder(itemType);
        if (providers != null)
        {
          Dictionary<PropertyIdentifier, PropertyIdentifier> seenProperties = this._seenPropertyPool;
          if (seenProperties == null)
            seenProperties = new Dictionary<PropertyIdentifier, PropertyIdentifier>();
          else
            this._seenPropertyPool = (Dictionary<PropertyIdentifier, PropertyIdentifier>) null;
          foreach (DesignModeValueProvider modeValueProvider in providers)
          {
            foreach (PropertyIdentifier key in (Collection<PropertyIdentifier>) modeValueProvider.Properties)
            {
              if (!seenProperties.ContainsKey(key))
              {
                seenProperties.Add(key, key);
                yield return key;
              }
            }
          }
          seenProperties.Clear();
          this._seenPropertyPool = seenProperties;
        }
      }

      public override bool HasValueTranslation(Type itemType, PropertyIdentifier property)
      {
        if (itemType == null)
          throw new ArgumentNullException("itemType");
        if (!this._disposed)
        {
          IEnumerable<DesignModeValueProvider> providersInCallOrder = this.GetFeatureProvidersInCallOrder(itemType);
          if (providersInCallOrder != null)
          {
            foreach (DesignModeValueProvider modeValueProvider in providersInCallOrder)
            {
              foreach (PropertyIdentifier id in (Collection<PropertyIdentifier>) modeValueProvider.Properties)
              {
                if (this.IdentifiersMatch(property, id))
                  return true;
              }
            }
          }
        }
        return false;
      }

      private bool IdentifiersMatch(PropertyIdentifier targetId, PropertyIdentifier id)
      {
        Type type1 = targetId.DeclaringType ?? ModelFactory.ResolveType(this._connector.Context, targetId.DeclaringTypeIdentifier);
        Type type2 = id.DeclaringType ?? ModelFactory.ResolveType(this._connector.Context, id.DeclaringTypeIdentifier);
        if (type2 != null && type1 == type2)
          return string.Equals(targetId.Name, id.Name, StringComparison.Ordinal);
        return false;
      }

      public override object TranslatePropertyValue(Type itemType, ModelItem item, PropertyIdentifier property, object value)
      {
        if (!this._disposed)
        {
          IEnumerable<DesignModeValueProvider> providersInCallOrder = this.GetFeatureProvidersInCallOrder(itemType);
          if (providersInCallOrder != null)
          {
            foreach (DesignModeValueProvider modeValueProvider in providersInCallOrder)
            {
              foreach (PropertyIdentifier propertyIdentifier in (Collection<PropertyIdentifier>) modeValueProvider.Properties)
              {
                if (this.IdentifiersMatch(property, propertyIdentifier))
                {
                  value = modeValueProvider.TranslatePropertyValue(item, propertyIdentifier, value);
                  break;
                }
              }
            }
          }
        }
        return value;
      }

      public override void InvalidateProperty(ModelItem item, PropertyIdentifier property)
      {
        if (item == null)
          throw new ArgumentNullException("item");
        if (this._disposed || this.PropertyInvalidated == null)
          return;
        this.PropertyInvalidated((object) this, new PropertyInvalidatedEventArgs(item, property));
      }
    }
  }
}
