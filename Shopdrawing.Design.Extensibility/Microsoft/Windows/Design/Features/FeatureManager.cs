// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Features.FeatureManager
// Assembly: Microsoft.Windows.Design.Extensibility, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4673B7C2-4EF5-4715-85F2-D8E573468337
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Extensibility\Microsoft.Windows.Design.Extensibility.dll

using Microsoft.Windows.Design;
using MS.Internal.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Microsoft.Windows.Design.Features
{
  public class FeatureManager : IDisposable
  {
    private static readonly BindingFlags _createBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance;
    private EditingContext _context;
    private Dictionary<Type, FeatureManager.FeatureConnectorEntry> _featureConnectors;
    private HashSet<Type> _knownFeatureProviders;
    private Predicate<Type> _defaultFilter;
    private MetadataProviderCallback _metadataProvider;
    private Dictionary<Type, IEnumerable<object>> _featureAttributeCache;
    private Dictionary<Type, IEnumerable<object>> _featureConnectorAttributeCache;
    private HashSet<Type> _initializedTypes;

    public EditingContext Context
    {
      get
      {
        return this._context;
      }
    }

    public MetadataProviderCallback MetadataProvider
    {
      get
      {
        if (this._metadataProvider == null)
          this._metadataProvider = new MetadataProviderCallback(this.GetCustomAttributesDefault);
        return this._metadataProvider;
      }
      set
      {
        this._metadataProvider = value;
        this.ClearCaches();
        if (this._featureConnectorAttributeCache == null)
          return;
        this._featureConnectorAttributeCache.Clear();
      }
    }

    public IEnumerable<FeatureConnectorInformation> PendingConnectors
    {
      get
      {
        foreach (FeatureManager.FeatureConnectorEntry featureConnectorEntry in this._featureConnectors.Values)
        {
          if (!featureConnectorEntry.IsActivated)
            yield return (FeatureConnectorInformation) featureConnectorEntry;
        }
      }
    }

    public IEnumerable<FeatureConnectorInformation> RunningConnectors
    {
      get
      {
        foreach (FeatureManager.FeatureConnectorEntry featureConnectorEntry in this._featureConnectors.Values)
        {
          if (featureConnectorEntry.IsActivated)
            yield return (FeatureConnectorInformation) featureConnectorEntry;
        }
      }
    }

    public event EventHandler<FeatureAvailableEventArgs> FeatureAvailable;

    public FeatureManager(EditingContext context)
    {
      if (context == null)
        throw new ArgumentNullException("context");
      this._context = context;
      this._featureConnectors = new Dictionary<Type, FeatureManager.FeatureConnectorEntry>();
      this._defaultFilter = new Predicate<Type>(this.OnDefaultCallbackFilter);
      this._context.Items.Subscribe<AssemblyReferences>((SubscribeContextCallback<AssemblyReferences>) (newReferences => this.ClearCaches()));
    }

    ~FeatureManager()
    {
      this.Dispose(false);
    }

    private void ClearCaches()
    {
      if (this._featureAttributeCache != null)
        this._featureAttributeCache.Clear();
      if (this._initializedTypes == null)
        return;
      this._initializedTypes.Clear();
    }

    public IEnumerable<FeatureProvider> CreateFeatureProviders(Type featureProviderType)
    {
      return this.CreateFeatureProviders(featureProviderType, this._defaultFilter);
    }

    public virtual IEnumerable<FeatureProvider> CreateFeatureProviders(Type featureProviderType, Predicate<Type> match)
    {
      if (featureProviderType == null)
        throw new ArgumentNullException("featureProviderType");
      if (match == null)
        throw new ArgumentNullException("match");
      if (!typeof (FeatureProvider).IsAssignableFrom(featureProviderType))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_IncorrectTypePassed, new object[1]
        {
          (object) typeof (FeatureProvider).Name
        }));
      if (this._knownFeatureProviders != null)
      {
        foreach (Type type in this._knownFeatureProviders)
        {
          if (featureProviderType.IsAssignableFrom(type) && match(type))
          {
            FeatureProvider featureProvider = FeatureManager.CreateFeatureProvider(type);
            if (featureProvider != null)
              yield return featureProvider;
          }
        }
      }
    }

    public IEnumerable<FeatureProvider> CreateFeatureProviders(Type featureProviderType, Type type)
    {
      return this.CreateFeatureProviders(featureProviderType, type, this._defaultFilter);
    }

    public virtual IEnumerable<FeatureProvider> CreateFeatureProviders(Type featureProviderType, Type type, Predicate<Type> match)
    {
      if (featureProviderType == null)
        throw new ArgumentNullException("featureProviderType");
      if (type == null)
        throw new ArgumentNullException("type");
      if (match == null)
        throw new ArgumentNullException("match");
      if (!typeof (FeatureProvider).IsAssignableFrom(featureProviderType))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_IncorrectTypePassed, new object[1]
        {
          (object) typeof (FeatureProvider).Name
        }));
      IEnumerable<object> attrs = this.GetFeatureAttributesForType(type);
      foreach (FeatureProvider featureProvider in FeatureManager.CreateFeatureProviders(featureProviderType, attrs, match))
        yield return featureProvider;
    }

    private static FeatureProvider CreateFeatureProvider(Type featureProviderType)
    {
      ConstructorInfo constructor = featureProviderType.GetConstructor(FeatureManager._createBindingFlags, (Binder) null, Type.EmptyTypes, (ParameterModifier[]) null);
      if (constructor != null)
        return constructor.Invoke((object[]) null) as FeatureProvider;
      return (FeatureProvider) null;
    }

    private static IEnumerable<FeatureProvider> CreateFeatureProviders(Type featureProviderType, IEnumerable<object> attrs, Predicate<Type> match)
    {
      foreach (Attribute attribute in attrs)
      {
        FeatureAttribute fAttrib = attribute as FeatureAttribute;
        if (fAttrib != null)
        {
          Type fType = fAttrib.FeatureProviderType;
          if (featureProviderType.IsAssignableFrom(fType) && match(fType))
          {
            FeatureProvider featureProvider = FeatureManager.CreateFeatureProvider(fType);
            if (featureProvider != null)
              yield return featureProvider;
          }
        }
      }
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      foreach (FeatureManager.FeatureConnectorEntry featureConnectorEntry in this._featureConnectors.Values)
        featureConnectorEntry.Dispose();
      this._featureConnectors.Clear();
    }

    public IEnumerable<object> GetCustomAttributes(Type type, Type attributeType)
    {
      if (type == null)
        throw new ArgumentNullException("type");
      if (attributeType == null)
        throw new ArgumentNullException("attributeType");
      return this.MetadataProvider(type, attributeType);
    }

    private IEnumerable<object> GetCustomAttributesDefault(Type type, Type attributeType)
    {
      return (IEnumerable<object>) type.GetCustomAttributes(attributeType, true);
    }

    private IEnumerable<object> GetFeatureAttributesForType(Type type)
    {
      if (this._featureAttributeCache == null)
        this._featureAttributeCache = new Dictionary<Type, IEnumerable<object>>();
      IEnumerable<object> enumerable;
      if (!this._featureAttributeCache.TryGetValue(type, out enumerable))
      {
        enumerable = (IEnumerable<object>) Enumerable.ToArray<object>(this.GetCustomAttributes(type, typeof (FeatureAttribute)));
        this._featureAttributeCache[type] = enumerable;
      }
      return enumerable;
    }

    private IEnumerable<object> GetFeatureConnectorAttributesForType(Type type)
    {
      if (this._featureConnectorAttributeCache == null)
        this._featureConnectorAttributeCache = new Dictionary<Type, IEnumerable<object>>();
      IEnumerable<object> attributesDefault;
      if (!this._featureConnectorAttributeCache.TryGetValue(type, out attributesDefault))
      {
        attributesDefault = this.GetCustomAttributesDefault(type, typeof (FeatureConnectorAttribute));
        this._featureConnectorAttributeCache[type] = attributesDefault;
      }
      return attributesDefault;
    }

    public void InitializeFeatures(Type type)
    {
      if (type == null)
        throw new ArgumentNullException("type");
      if (type.IsPrimitive || type == typeof (string))
        return;
      if (this._initializedTypes == null)
        this._initializedTypes = new HashSet<Type>();
      if (this._initializedTypes.Contains(type))
        return;
      this._initializedTypes.Add(type);
      foreach (Attribute attribute1 in this.GetFeatureAttributesForType(type))
      {
        FeatureAttribute featureAttribute = attribute1 as FeatureAttribute;
        if (featureAttribute != null)
        {
          Type featureProviderType = featureAttribute.FeatureProviderType;
          if (!this._initializedTypes.Contains(featureProviderType))
          {
            this._initializedTypes.Add(featureProviderType);
            foreach (Attribute attribute2 in this.GetFeatureConnectorAttributesForType(featureProviderType))
            {
              FeatureConnectorAttribute connectorAttribute = attribute2 as FeatureConnectorAttribute;
              if (connectorAttribute != null)
              {
                Type featureConnectorType = connectorAttribute.FeatureConnectorType;
                if (!this._featureConnectors.ContainsKey(featureConnectorType))
                {
                  FeatureManager.FeatureConnectorEntry featureConnectorEntry = new FeatureManager.FeatureConnectorEntry(featureConnectorType, this);
                  this._featureConnectors.Add(featureConnectorType, featureConnectorEntry);
                  featureConnectorEntry.AttemptActivate();
                }
              }
            }
          }
          if (this._knownFeatureProviders == null)
            this._knownFeatureProviders = new HashSet<Type>();
          if (!this._knownFeatureProviders.Contains(featureProviderType))
          {
            this._knownFeatureProviders.Add(featureProviderType);
            if (this.FeatureAvailable != null)
              this.OnFeatureAvailable(new FeatureAvailableEventArgs(featureProviderType));
          }
        }
      }
    }

    private bool OnDefaultCallbackFilter(Type featureProviderType)
    {
      return new RequirementValidator(this, featureProviderType).MeetsRequirements;
    }

    protected virtual void OnFeatureAvailable(FeatureAvailableEventArgs e)
    {
      if (this.FeatureAvailable == null)
        return;
      this.FeatureAvailable((object) this, e);
    }

    private class FeatureConnectorEntry : FeatureConnectorInformation
    {
      private Type _featureConnectorType;
      private FeatureManager _manager;
      private IDisposable _featureConnector;
      private RequirementValidator _requirements;

      public override Type FeatureConnectorType
      {
        get
        {
          return this._featureConnectorType;
        }
      }

      internal bool IsActivated
      {
        get
        {
          return this._featureConnector != null;
        }
      }

      public override IEnumerable<Type> RequiredServices
      {
        get
        {
          foreach (RequirementAttribute requirementAttribute in this._requirements.Requirements)
          {
            RequiresServiceAttribute attr = requirementAttribute as RequiresServiceAttribute;
            if (attr != null)
              yield return attr.ServiceType;
          }
        }
      }

      public override IEnumerable<Type> RequiredItems
      {
        get
        {
          foreach (RequirementAttribute requirementAttribute in this._requirements.Requirements)
          {
            RequiresContextItemAttribute attr = requirementAttribute as RequiresContextItemAttribute;
            if (attr != null)
              yield return attr.ContextItemType;
          }
        }
      }

      public override IEnumerable<Type> PendingServices
      {
        get
        {
          foreach (RequirementAttribute requirementAttribute in this._requirements.PendingRequirements)
          {
            RequiresServiceAttribute attr = requirementAttribute as RequiresServiceAttribute;
            if (attr != null)
              yield return attr.ServiceType;
          }
        }
      }

      public override IEnumerable<Type> PendingItems
      {
        get
        {
          foreach (RequirementAttribute requirementAttribute in this._requirements.PendingRequirements)
          {
            RequiresContextItemAttribute attr = requirementAttribute as RequiresContextItemAttribute;
            if (attr != null)
              yield return attr.ContextItemType;
          }
        }
      }

      internal FeatureConnectorEntry(Type featureConnectorType, FeatureManager manager)
      {
        this._featureConnectorType = featureConnectorType;
        this._manager = manager;
      }

      internal void AttemptActivate()
      {
        bool flag = false;
        if (this._requirements == null)
        {
          this._requirements = new RequirementValidator(this._manager, this._featureConnectorType);
          flag = true;
        }
        if (this._requirements.MeetsRequirements)
        {
          this._requirements.RequirementsChanged -= new EventHandler(this.OnRequirementsChanged);
          this._featureConnector = Activator.CreateInstance(this._featureConnectorType, new object[1]
          {
            (object) this._manager
          }) as IDisposable;
        }
        else
        {
          if (!flag)
            return;
          this._requirements.RequirementsChanged += new EventHandler(this.OnRequirementsChanged);
        }
      }

      internal void Dispose()
      {
        if (this._featureConnector != null)
        {
          this._featureConnector.Dispose();
          this._featureConnector = (IDisposable) null;
        }
        else
          this._requirements.RequirementsChanged -= new EventHandler(this.OnRequirementsChanged);
      }

      private void OnRequirementsChanged(object sender, EventArgs e)
      {
        this.AttemptActivate();
      }
    }
  }
}
