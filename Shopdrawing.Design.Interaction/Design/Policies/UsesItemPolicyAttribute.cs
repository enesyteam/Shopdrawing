// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Policies.UsesItemPolicyAttribute
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Services;
using MS.Internal;
using MS.Internal.Properties;
using System;
using System.Globalization;

namespace Microsoft.Windows.Design.Policies
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
  public sealed class UsesItemPolicyAttribute : RequirementAttribute
  {
    private Type _itemPolicyType;

    public override bool AllRequired
    {
      get
      {
        return false;
      }
    }

    public Type ItemPolicyType
    {
      get
      {
        return this._itemPolicyType;
      }
    }

    public override object TypeId
    {
      get
      {
        return (object) new EqualityArray(new object[2]
        {
          (object) typeof (UsesItemPolicyAttribute),
          (object) this._itemPolicyType
        });
      }
    }

    public UsesItemPolicyAttribute(Type itemPolicyType)
    {
      if (itemPolicyType == null)
        throw new ArgumentNullException("itemPolicyType");
      if (itemPolicyType.IsAssignableFrom(typeof (ItemPolicy)))
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_ArgIncorrectTypeValue, new object[2]
        {
          (object) "itemPolicyType",
          (object) typeof (ItemPolicy).Name
        }));
      this._itemPolicyType = itemPolicyType;
    }

    public override bool Equals(object obj)
    {
      UsesItemPolicyAttribute itemPolicyAttribute = obj as UsesItemPolicyAttribute;
      if (itemPolicyAttribute == null)
        return false;
      return itemPolicyAttribute.ItemPolicyType == this.ItemPolicyType;
    }

    public override int GetHashCode()
    {
      return this._itemPolicyType.GetHashCode();
    }

    public override RequirementSubscription CreateSubscription(EditingContext context)
    {
      if (context == null)
        throw new ArgumentNullException("context");
      return (RequirementSubscription) new UsesItemPolicyAttribute.RequirePolicySubscription(context, this);
    }

    public override bool MeetsRequirement(EditingContext context)
    {
      if (context == null)
        throw new ArgumentNullException("context");
      ItemPolicyService service = context.Services.GetService<ItemPolicyService>();
      if (service == null)
        return false;
      foreach (object obj in service.Policies)
      {
        if (obj.GetType() == this.ItemPolicyType)
          return true;
      }
      return false;
    }

    private class RequirePolicySubscription : RequirementSubscription
    {
      private EditingContext _context;
      private Type _policyType;
      private ItemPolicyService _policyService;

      internal RequirePolicySubscription(EditingContext context, UsesItemPolicyAttribute requirement)
        : base((RequirementAttribute) requirement)
      {
        this._context = context;
        this._policyType = requirement.ItemPolicyType;
      }

      private void OnPolicyAdded(object sender, PolicyAddedEventArgs e)
      {
        if (e.Policy.GetType() != this._policyType)
          return;
        this.OnRequirementChanged();
      }

      private void OnPolicyServiceAvailable(ItemPolicyService policyService)
      {
        this._context.Services.Unsubscribe<ItemPolicyService>(new SubscribeServiceCallback<ItemPolicyService>(this.OnPolicyServiceAvailable));
        this._policyService = policyService;
        this._policyService.PolicyAdded += new EventHandler<PolicyAddedEventArgs>(this.OnPolicyAdded);
      }

      protected override void Subscribe()
      {
        this._policyService = this._context.Services.GetService<ItemPolicyService>();
        if (this._policyService != null)
          this._policyService.PolicyAdded += new EventHandler<PolicyAddedEventArgs>(this.OnPolicyAdded);
        else
          this._context.Services.Subscribe<ItemPolicyService>(new SubscribeServiceCallback<ItemPolicyService>(this.OnPolicyServiceAvailable));
      }

      protected override void Unsubscribe()
      {
        if (this._policyService != null)
          this._policyService.PolicyAdded -= new EventHandler<PolicyAddedEventArgs>(this.OnPolicyAdded);
        else
          this._context.Services.Unsubscribe<ItemPolicyService>(new SubscribeServiceCallback<ItemPolicyService>(this.OnPolicyServiceAvailable));
      }
    }
  }
}
