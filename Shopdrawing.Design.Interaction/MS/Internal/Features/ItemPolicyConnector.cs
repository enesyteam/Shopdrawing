// Decompiled with JetBrains decompiler
// Type: MS.Internal.Features.ItemPolicyConnector
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Policies;
using Microsoft.Windows.Design.Services;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MS.Internal.Features
{
  internal class ItemPolicyConnector : IDisposable
  {
    private FeatureManager _manager;
    private Dictionary<Type, ItemPolicyConnector.PolicyData> _policies;
    private HashSet<Type> _seenFeatureProviderTypes;
    private EventHandler<PolicyAddedEventArgs> _policyAdded;

    public ItemPolicyConnector(FeatureManager manager)
    {
      this._manager = manager;
      manager.Context.Services.Publish<ItemPolicyService>((PublishServiceCallback<ItemPolicyService>) (() => (ItemPolicyService) new ItemPolicyConnector.ItemPolicyServiceImpl(this)));
      this._policies = new Dictionary<Type, ItemPolicyConnector.PolicyData>();
      manager.FeatureAvailable += new EventHandler<FeatureAvailableEventArgs>(this.OnFeatureAvailable);
    }

    private void AttemptActivatePolicy(ItemPolicy policy)
    {
      if (this._policies.ContainsKey(policy.GetType()))
        return;
      ItemPolicyConnector.PolicyData policyData = new ItemPolicyConnector.PolicyData();
      policyData.Validator = new RequirementValidator(this._manager, policy.GetType());
      policyData.Policy = policy;
      this._policies[policy.GetType()] = policyData;
      if (policyData.Validator.MeetsRequirements)
        this.ActivatePolicy(policyData);
      else
        policyData.Validator.RequirementsChanged += new EventHandler(this.OnPolicyRequirementsChanged);
    }

    private void ActivatePolicy(ItemPolicyConnector.PolicyData policyData)
    {
      policyData.Validator.RequirementsChanged -= new EventHandler(this.OnPolicyRequirementsChanged);
      policyData.Validator = (RequirementValidator) null;
      policyData.Policy.Activate(this._manager.Context);
      if (this._policyAdded == null)
        return;
      this._policyAdded((object) this, new PolicyAddedEventArgs(policyData.Policy));
    }

    private void Dispose(bool disposing)
    {
      if (!disposing || this._policies == null)
        return;
      foreach (ItemPolicyConnector.PolicyData policyData in this._policies.Values)
      {
        if (policyData.Validator != null)
          policyData.Validator.RequirementsChanged -= new EventHandler(this.OnPolicyRequirementsChanged);
        else
          policyData.Policy.Deactivate();
      }
      this._manager.FeatureAvailable -= new EventHandler<FeatureAvailableEventArgs>(this.OnFeatureAvailable);
      this._policies = (Dictionary<Type, ItemPolicyConnector.PolicyData>) null;
    }

    public void OnExtensionAvailable(Type featureProviderType)
    {
      if (this._seenFeatureProviderTypes == null)
        this._seenFeatureProviderTypes = new HashSet<Type>();
      if (this._seenFeatureProviderTypes.Contains(featureProviderType))
        return;
      this._seenFeatureProviderTypes.Add(featureProviderType);
      foreach (UsesItemPolicyAttribute itemPolicyAttribute in featureProviderType.GetCustomAttributes(typeof (UsesItemPolicyAttribute), true))
      {
        Type itemPolicyType = itemPolicyAttribute.ItemPolicyType;
        if (typeof (ItemPolicy).IsAssignableFrom(itemPolicyType) && !this._policies.ContainsKey(itemPolicyType))
        {
          ConstructorInfo constructor = itemPolicyType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, (Binder) null, Type.EmptyTypes, (ParameterModifier[]) null);
          if (constructor != null)
          {
            ItemPolicy policy = constructor.Invoke((object[]) null) as ItemPolicy;
            if (policy != null)
              this.AttemptActivatePolicy(policy);
          }
        }
      }
    }

    private void OnFeatureAvailable(object sender, FeatureAvailableEventArgs e)
    {
      this.OnExtensionAvailable(e.FeatureProviderType);
    }

    private void OnPolicyRequirementsChanged(object sender, EventArgs e)
    {
      RequirementValidator requirementValidator = (RequirementValidator) sender;
      if (!requirementValidator.MeetsRequirements)
        return;
      this.ActivatePolicy(this._policies[requirementValidator.Type]);
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    internal class ItemPolicyServiceImpl : ItemPolicyService
    {
      private ItemPolicyConnector _server;

      public ItemPolicyConnector Server
      {
        get
        {
          return this._server;
        }
      }

      public override IEnumerable<ItemPolicy> Policies
      {
        get
        {
          if (this._server._policies != null)
          {
            foreach (ItemPolicyConnector.PolicyData policyData in this._server._policies.Values)
            {
              if (policyData.Validator == null)
                yield return policyData.Policy;
            }
          }
        }
      }

      public override event EventHandler<PolicyAddedEventArgs> PolicyAdded
      {
        add
        {
          this._server._policyAdded += value;
        }
        remove
        {
          this._server._policyAdded -= value;
        }
      }

      internal ItemPolicyServiceImpl(ItemPolicyConnector server)
      {
        this._server = server;
      }
    }

    private class PolicyData
    {
      internal ItemPolicy Policy;
      internal RequirementValidator Validator;
    }
  }
}
