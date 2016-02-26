// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.RequiresServiceAttribute
// Assembly: Microsoft.Windows.Design.Extensibility, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4673B7C2-4EF5-4715-85F2-D8E573468337
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Extensibility\Microsoft.Windows.Design.Extensibility.dll

using MS.Internal;
using System;

namespace Microsoft.Windows.Design
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
  public sealed class RequiresServiceAttribute : RequirementAttribute
  {
    private Type _serviceType;

    public Type ServiceType
    {
      get
      {
        return this._serviceType;
      }
    }

    public override object TypeId
    {
      get
      {
        return (object) new EqualityArray(new object[2]
        {
          (object) typeof (RequiresServiceAttribute),
          (object) this._serviceType
        });
      }
    }

    public RequiresServiceAttribute(Type serviceType)
    {
      if (serviceType == null)
        throw new ArgumentNullException("serviceType");
      this._serviceType = serviceType;
    }

    public override RequirementSubscription CreateSubscription(EditingContext context)
    {
      if (context == null)
        throw new ArgumentNullException("context");
      return (RequirementSubscription) new RequiresServiceAttribute.RequireServiceSubscription(context, this);
    }

    public override bool MeetsRequirement(EditingContext context)
    {
      if (context == null)
        throw new ArgumentNullException("context");
      return context.Services.Contains(this.ServiceType);
    }

    private class RequireServiceSubscription : RequirementSubscription
    {
      private EditingContext _context;
      private Type _serviceType;

      internal RequireServiceSubscription(EditingContext context, RequiresServiceAttribute requirement)
        : base((RequirementAttribute) requirement)
      {
        this._context = context;
        this._serviceType = requirement.ServiceType;
      }

      private void OnServiceAvailable(Type serviceType, object serviceInstance)
      {
        this.OnRequirementChanged();
      }

      protected override void Subscribe()
      {
        this._context.Services.Subscribe(this._serviceType, new SubscribeServiceCallback(this.OnServiceAvailable));
      }

      protected override void Unsubscribe()
      {
        this._context.Services.Unsubscribe(this._serviceType, new SubscribeServiceCallback(this.OnServiceAvailable));
      }
    }
  }
}
