// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.WindowElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class WindowElement : ContentControlElement
  {
    public static readonly IPropertyId WindowStyleProperty = (IPropertyId) PlatformTypes.Window.GetMember(MemberType.LocalProperty, "WindowStyle", MemberAccessTypes.Public);
    public static readonly IPropertyId AllowsTransparencyProperty = (IPropertyId) PlatformTypes.Window.GetMember(MemberType.LocalProperty, "AllowsTransparency", MemberAccessTypes.Public);
    public static readonly WindowElement.ConcreteWindowElementFactory Factory = new WindowElement.ConcreteWindowElementFactory();
    private bool isChangingWindowStyleOrAllowsTransparency;

    public override bool CanCloneStyle(IPropertyId propertyKey)
    {
      return false;
    }

    public override StyleNode ExpandDefaultStyle(IPropertyId propertyKey)
    {
      return (StyleNode) null;
    }

    public override void SetValue(PropertyReference propertyReference, object valueToSet)
    {
      if (!this.isChangingWindowStyleOrAllowsTransparency)
      {
        this.isChangingWindowStyleOrAllowsTransparency = true;
        if (propertyReference.LastStep.Equals((object) WindowElement.AllowsTransparencyProperty))
        {
          if (object.Equals(valueToSet, (object) true))
            this.SetValue(WindowElement.WindowStyleProperty, (object) WindowStyle.None);
          else if (object.Equals(valueToSet, (object) false))
            this.ClearValue(WindowElement.WindowStyleProperty);
        }
        else if (propertyReference.LastStep.Equals((object) WindowElement.WindowStyleProperty) && !object.Equals(valueToSet, (object) WindowStyle.None))
          this.ClearValue(WindowElement.AllowsTransparencyProperty);
        this.isChangingWindowStyleOrAllowsTransparency = false;
      }
      base.SetValue(propertyReference, valueToSet);
    }

    public override void ClearValue(PropertyReference propertyReference)
    {
      if (!this.isChangingWindowStyleOrAllowsTransparency)
      {
        this.isChangingWindowStyleOrAllowsTransparency = true;
        if (propertyReference.LastStep.Equals((object) WindowElement.AllowsTransparencyProperty))
          this.ClearValue(WindowElement.WindowStyleProperty);
        else if (propertyReference.LastStep.Equals((object) WindowElement.WindowStyleProperty))
          this.ClearValue(WindowElement.AllowsTransparencyProperty);
        this.isChangingWindowStyleOrAllowsTransparency = false;
      }
      base.ClearValue(propertyReference);
    }

    protected override object GetComputedValueInternal(PropertyReference propertyReference)
    {
      if (propertyReference.FirstStep.Equals((object) ContentControlElement.ContentProperty))
        return this.GetRawComputedValue(propertyReference);
      return base.GetComputedValueInternal(propertyReference);
    }

    public class ConcreteWindowElementFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new WindowElement();
      }
    }
  }
}
