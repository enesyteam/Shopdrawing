// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Policies.PolicyDrivenFeatureConnector`1
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Services;
using MS.Internal.Features;
using System;
using System.Collections.Generic;

namespace Microsoft.Windows.Design.Policies
{
  public abstract class PolicyDrivenFeatureConnector<TFeatureProviderType> : FeatureConnector<TFeatureProviderType> where TFeatureProviderType : FeatureProvider
  {
    private ItemPolicyConnector _policyServer;
    private ItemPolicyService _policyService;
    private Dictionary<ModelItem, List<PolicyDrivenFeatureConnector<TFeatureProviderType>.ProviderData>> _featureProviders;
    private Dictionary<ModelItem, List<PolicyDrivenFeatureConnector<TFeatureProviderType>.ProviderData>> _removes;
    private Dictionary<ModelItem, List<PolicyDrivenFeatureConnector<TFeatureProviderType>.ProviderData>> _adds;

    protected IEnumerable<PolicyDrivenFeatureConnector<TFeatureProviderType>.ItemFeatureProvider> FeatureProviders
    {
      get
      {
        foreach (KeyValuePair<ModelItem, List<PolicyDrivenFeatureConnector<TFeatureProviderType>.ProviderData>> keyValuePair in this._featureProviders)
        {
          foreach (PolicyDrivenFeatureConnector<TFeatureProviderType>.ProviderData providerData in keyValuePair.Value)
          {
            if (providerData.IsValid)
              yield return new PolicyDrivenFeatureConnector<TFeatureProviderType>.ItemFeatureProvider(keyValuePair.Key, providerData.Provider);
          }
        }
      }
    }

    protected PolicyDrivenFeatureConnector(FeatureManager manager)
      : base(manager)
    {
      this._featureProviders = new Dictionary<ModelItem, List<PolicyDrivenFeatureConnector<TFeatureProviderType>.ProviderData>>();
      this._policyService = this.Context.Services.GetService<ItemPolicyService>();
      if (this._policyService == null)
      {
        this._policyServer = new ItemPolicyConnector(manager);
        this._policyService = this.Context.Services.GetService<ItemPolicyService>();
      }
      else
        this._policyServer = ((ItemPolicyConnector.ItemPolicyServiceImpl) this._policyService).Server;
      foreach (TFeatureProviderType featureProviderType in this.Manager.CreateFeatureProviders(typeof (TFeatureProviderType), (Predicate<Type>) (featureProviderType =>
      {
        this._policyServer.OnExtensionAvailable(featureProviderType);
        return false;
      })))
        ;
      this._policyService.PolicyAdded += new EventHandler<PolicyAddedEventArgs>(this.OnPolicyAdded);
      foreach (ItemPolicy policy in this._policyService.Policies)
      {
        this.OnPolicyItemsChanged((object) this, new PolicyItemsChangedEventArgs(policy, policy.PolicyItems, (IEnumerable<ModelItem>) null));
        policy.PolicyItemsChanged += new EventHandler<PolicyItemsChangedEventArgs>(this.OnPolicyItemsChanged);
      }
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this._policyServer != null)
        this._policyServer.Dispose();
      base.Dispose(disposing);
    }

    private void OnPolicyAdded(object sender, PolicyAddedEventArgs e)
    {
      this.OnPolicyItemsChanged((object) this, new PolicyItemsChangedEventArgs(e.Policy, e.Policy.PolicyItems, (IEnumerable<ModelItem>) null));
      e.Policy.PolicyItemsChanged += new EventHandler<PolicyItemsChangedEventArgs>(this.OnPolicyItemsChanged);
    }

    private void OnPolicyItemsChanged(object sender, PolicyItemsChangedEventArgs e)
    {
      Dictionary<ModelItem, List<PolicyDrivenFeatureConnector<TFeatureProviderType>.ProviderData>> dictionary1 = this._removes;
      Dictionary<ModelItem, List<PolicyDrivenFeatureConnector<TFeatureProviderType>.ProviderData>> dictionary2 = this._adds;
      this._removes = this._adds = (Dictionary<ModelItem, List<PolicyDrivenFeatureConnector<TFeatureProviderType>.ProviderData>>) null;
      if (dictionary1 == null)
        dictionary1 = new Dictionary<ModelItem, List<PolicyDrivenFeatureConnector<TFeatureProviderType>.ProviderData>>();
      else
        dictionary1.Clear();
      if (dictionary2 == null)
        dictionary2 = new Dictionary<ModelItem, List<PolicyDrivenFeatureConnector<TFeatureProviderType>.ProviderData>>();
      else
        dictionary2.Clear();
      foreach (ModelItem key in e.ItemsRemoved)
      {
        List<PolicyDrivenFeatureConnector<TFeatureProviderType>.ProviderData> list;
        if (!this._featureProviders.TryGetValue(key, out list))
          list = new List<PolicyDrivenFeatureConnector<TFeatureProviderType>.ProviderData>();
        bool flag = false;
        foreach (PolicyDrivenFeatureConnector<TFeatureProviderType>.ProviderData providerData in list)
        {
          if (PolicyDrivenFeatureConnector<TFeatureProviderType>.PolicyNode.Remove(ref providerData.AssociatedPolicies, e.Policy))
            flag = true;
        }
        if (flag)
          dictionary1[key] = list;
      }
      foreach (ModelItem key in e.ItemsAdded)
      {
        List<PolicyDrivenFeatureConnector<TFeatureProviderType>.ProviderData> dataList;
        if (!this._featureProviders.TryGetValue(key, out dataList))
          dataList = new List<PolicyDrivenFeatureConnector<TFeatureProviderType>.ProviderData>();
        bool flag = false;
        foreach (TFeatureProviderType featureProviderType in (IEnumerable<TFeatureProviderType>) this.CreateFeatureProviders(key, e.Policy, dataList))
        {
          PolicyDrivenFeatureConnector<TFeatureProviderType>.ProviderData providerData = new PolicyDrivenFeatureConnector<TFeatureProviderType>.ProviderData();
          providerData.Provider = featureProviderType;
          providerData.AssociatedPolicies = new PolicyDrivenFeatureConnector<TFeatureProviderType>.PolicyNode(e.Policy, providerData.AssociatedPolicies);
          providerData.IsValid = this.IsValidProvider((FeatureProvider) featureProviderType);
          dataList.Add(providerData);
          flag = true;
        }
        if (flag)
          dictionary2[key] = dataList;
      }
      List<TFeatureProviderType> list1 = (List<TFeatureProviderType>) null;
      foreach (KeyValuePair<ModelItem, List<PolicyDrivenFeatureConnector<TFeatureProviderType>.ProviderData>> keyValuePair in dictionary1)
      {
        ModelItem key = keyValuePair.Key;
        if (list1 != null)
          list1.Clear();
        for (int index = 0; index < keyValuePair.Value.Count; ++index)
        {
          PolicyDrivenFeatureConnector<TFeatureProviderType>.ProviderData providerData = keyValuePair.Value[index];
          if (providerData.AssociatedPolicies == null)
          {
            if (providerData.IsValid)
            {
              if (list1 == null)
                list1 = new List<TFeatureProviderType>();
              list1.Add(providerData.Provider);
            }
            keyValuePair.Value.RemoveAt(index);
            --index;
          }
        }
        if (keyValuePair.Value.Count == 0)
          this._featureProviders.Remove(key);
        if (list1 != null && list1.Count > 0)
          this.FeatureProvidersRemoved(key, (IEnumerable<TFeatureProviderType>) list1);
      }
      foreach (KeyValuePair<ModelItem, List<PolicyDrivenFeatureConnector<TFeatureProviderType>.ProviderData>> keyValuePair in dictionary2)
      {
        ModelItem key = keyValuePair.Key;
        if (list1 != null)
          list1.Clear();
        for (int index = 0; index < keyValuePair.Value.Count; ++index)
        {
          PolicyDrivenFeatureConnector<TFeatureProviderType>.ProviderData providerData = keyValuePair.Value[index];
          if (providerData.AssociatedPolicies != null && !providerData.AssociatedPolicies.HasMultiple && (providerData.AssociatedPolicies.Contains(e.Policy) && providerData.IsValid))
          {
            if (list1 == null)
              list1 = new List<TFeatureProviderType>();
            list1.Add(providerData.Provider);
          }
        }
        this._featureProviders[key] = keyValuePair.Value;
        if (list1 != null && list1.Count > 0)
          this.FeatureProvidersAdded(key, (IEnumerable<TFeatureProviderType>) list1);
      }
      dictionary1.Clear();
      dictionary2.Clear();
      this._removes = dictionary1;
      this._adds = dictionary2;
    }

    protected void UpdateFeatureProviders()
    {
      List<TFeatureProviderType> list1 = (List<TFeatureProviderType>) null;
      List<TFeatureProviderType> list2 = (List<TFeatureProviderType>) null;
      foreach (ModelItem key in new List<ModelItem>((IEnumerable<ModelItem>) this._featureProviders.Keys))
      {
        if (list1 != null)
          list1.Clear();
        if (list2 != null)
          list2.Clear();
        List<PolicyDrivenFeatureConnector<TFeatureProviderType>.ProviderData> list3;
        if (this._featureProviders.TryGetValue(key, out list3) && list3 != null)
        {
          foreach (PolicyDrivenFeatureConnector<TFeatureProviderType>.ProviderData providerData in list3)
          {
            bool flag = this.IsValidProvider((FeatureProvider) providerData.Provider);
            if (flag != providerData.IsValid)
            {
              providerData.IsValid = flag;
              if (flag)
              {
                if (list1 == null)
                  list1 = new List<TFeatureProviderType>();
                list1.Add(providerData.Provider);
              }
              else
              {
                if (list2 == null)
                  list2 = new List<TFeatureProviderType>();
                list2.Add(providerData.Provider);
              }
            }
          }
          if (list2 != null)
            this.FeatureProvidersRemoved(key, (IEnumerable<TFeatureProviderType>) list2);
          if (list1 != null)
            this.FeatureProvidersAdded(key, (IEnumerable<TFeatureProviderType>) list1);
        }
      }
    }

    protected virtual bool IsValidProvider(FeatureProvider featureProvider)
    {
      return true;
    }

    protected abstract void FeatureProvidersAdded(ModelItem item, IEnumerable<TFeatureProviderType> featureProviders);

    protected abstract void FeatureProvidersRemoved(ModelItem item, IEnumerable<TFeatureProviderType> featureProviders);

    private IList<TFeatureProviderType> CreateFeatureProviders(ModelItem item, ItemPolicy policy, List<PolicyDrivenFeatureConnector<TFeatureProviderType>.ProviderData> dataList)
    {
      UsesItemPolicyAttribute requiredPolicy = new UsesItemPolicyAttribute(policy.GetType());
      Predicate<Type> match = (Predicate<Type>) (featureProviderType =>
      {
        foreach (UsesItemPolicyAttribute itemPolicyAttribute in this.Manager.GetCustomAttributes(featureProviderType, typeof (UsesItemPolicyAttribute)))
        {
          if (itemPolicyAttribute.Equals((object) requiredPolicy) && new RequirementValidator(this.Manager, featureProviderType).MeetsRequirements)
          {
            if (dataList != null)
            {
              foreach (PolicyDrivenFeatureConnector<TFeatureProviderType>.ProviderData providerData in dataList)
              {
                if (providerData.Provider.GetType() == featureProviderType)
                {
                  if (providerData.AssociatedPolicies == null || !providerData.AssociatedPolicies.Contains(policy))
                    providerData.AssociatedPolicies = new PolicyDrivenFeatureConnector<TFeatureProviderType>.PolicyNode(policy, providerData.AssociatedPolicies);
                  return false;
                }
              }
            }
            return true;
          }
        }
        return false;
      });
      List<TFeatureProviderType> list = new List<TFeatureProviderType>();
      if (!policy.IsSurrogate)
      {
        foreach (TFeatureProviderType featureProviderType in FeatureExtensions.CreateFeatureProviders(this.Manager, typeof (TFeatureProviderType), item, match))
          list.Add(featureProviderType);
      }
      else
      {
        foreach (ModelItem modelItem in policy.GetSurrogateItems(item))
        {
          foreach (TFeatureProviderType featureProviderType in FeatureExtensions.CreateFeatureProviders(this.Manager, typeof (TFeatureProviderType), modelItem, match))
            list.Add(featureProviderType);
        }
      }
      return (IList<TFeatureProviderType>) list;
    }

    protected sealed class ItemFeatureProvider
    {
      private ModelItem _item;
      private TFeatureProviderType _featureProvider;

      public ModelItem Item
      {
        get
        {
          return this._item;
        }
      }

      public TFeatureProviderType FeatureProvider
      {
        get
        {
          return this._featureProvider;
        }
      }

      internal ItemFeatureProvider(ModelItem item, TFeatureProviderType featureProvider)
      {
        this._item = item;
        this._featureProvider = featureProvider;
      }
    }

    private class PolicyNode
    {
      private PolicyDrivenFeatureConnector<TFeatureProviderType>.PolicyNode _next;
      private ItemPolicy _policy;

      internal bool HasMultiple
      {
        get
        {
          return this._next != null;
        }
      }

      internal PolicyNode(ItemPolicy policy, PolicyDrivenFeatureConnector<TFeatureProviderType>.PolicyNode next)
      {
        this._policy = policy;
        this._next = next;
      }

      internal bool Contains(ItemPolicy policy)
      {
        for (PolicyDrivenFeatureConnector<TFeatureProviderType>.PolicyNode policyNode = this; policyNode != null; policyNode = policyNode._next)
        {
          if (policyNode._policy == policy)
            return true;
        }
        return false;
      }

      internal static bool Remove(ref PolicyDrivenFeatureConnector<TFeatureProviderType>.PolicyNode root, ItemPolicy policy)
      {
        PolicyDrivenFeatureConnector<TFeatureProviderType>.PolicyNode policyNode1 = (PolicyDrivenFeatureConnector<TFeatureProviderType>.PolicyNode) null;
        for (PolicyDrivenFeatureConnector<TFeatureProviderType>.PolicyNode policyNode2 = root; policyNode2 != null; policyNode2 = policyNode2._next)
        {
          if (policyNode2._policy == policy)
          {
            if (policyNode1 != null)
            {
              policyNode1._next = policyNode2._next;
              return true;
            }
            root = policyNode2._next;
            return true;
          }
          policyNode1 = policyNode2;
        }
        return false;
      }
    }

    private class ProviderData
    {
      internal TFeatureProviderType Provider;
      internal PolicyDrivenFeatureConnector<TFeatureProviderType>.PolicyNode AssociatedPolicies;
      internal bool IsValid;
    }
  }
}
