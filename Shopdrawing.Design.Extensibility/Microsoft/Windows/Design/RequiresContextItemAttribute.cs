// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.RequiresContextItemAttribute
// Assembly: Microsoft.Windows.Design.Extensibility, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4673B7C2-4EF5-4715-85F2-D8E573468337
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Extensibility\Microsoft.Windows.Design.Extensibility.dll

using MS.Internal;
using System;

namespace Microsoft.Windows.Design
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
  public sealed class RequiresContextItemAttribute : RequirementAttribute
  {
    private Type _contextItemType;

    public Type ContextItemType
    {
      get
      {
        return this._contextItemType;
      }
    }

    public override object TypeId
    {
      get
      {
        return (object) new EqualityArray(new object[2]
        {
          (object) typeof (RequiresContextItemAttribute),
          (object) this._contextItemType
        });
      }
    }

    public RequiresContextItemAttribute(Type contextItemType)
    {
      if (contextItemType == null)
        throw new ArgumentNullException("contextItemType");
      this._contextItemType = contextItemType;
    }

    public override RequirementSubscription CreateSubscription(EditingContext context)
    {
      if (context == null)
        throw new ArgumentNullException("context");
      return (RequirementSubscription) new RequiresContextItemAttribute.RequireContextItemSubscription(context, this);
    }

    public override bool MeetsRequirement(EditingContext context)
    {
      if (context == null)
        throw new ArgumentNullException("context");
      return context.Items.Contains(this.ContextItemType);
    }

    private class RequireContextItemSubscription : RequirementSubscription
    {
      private EditingContext _context;
      private Type _contextItemType;

      internal RequireContextItemSubscription(EditingContext context, RequiresContextItemAttribute requirement)
        : base((RequirementAttribute) requirement)
      {
        this._context = context;
        this._contextItemType = requirement.ContextItemType;
      }

      private void OnContextItemChanged(ContextItem item)
      {
        this.OnRequirementChanged();
      }

      protected override void Subscribe()
      {
        this._context.Items.Subscribe(this._contextItemType, new SubscribeContextCallback(this.OnContextItemChanged));
      }

      protected override void Unsubscribe()
      {
        this._context.Items.Unsubscribe(this._contextItemType, new SubscribeContextCallback(this.OnContextItemChanged));
      }
    }
  }
}
