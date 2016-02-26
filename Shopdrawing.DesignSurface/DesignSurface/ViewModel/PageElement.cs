// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.PageElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class PageElement : BaseFrameworkElement
  {
    public static readonly IPropertyId ContentProperty = (IPropertyId) PlatformTypes.Page.GetMember(MemberType.LocalProperty, "Content", MemberAccessTypes.Public);
    public static readonly IPropertyId TemplateProperty = (IPropertyId) PlatformTypes.Page.GetMember(MemberType.LocalProperty, "Template", MemberAccessTypes.Public);
    public static readonly PageElement.ConcretePageElementFactory Factory = new PageElement.ConcretePageElementFactory();

    public override bool CanCloneStyle(IPropertyId propertyKey)
    {
      return false;
    }

    public override StyleNode ExpandDefaultStyle(IPropertyId propertyKey)
    {
      return (StyleNode) null;
    }

    protected override object GetComputedValueInternal(PropertyReference propertyReference)
    {
      if (propertyReference.FirstStep.Equals((object) PageElement.ContentProperty))
        return this.GetRawComputedValue(propertyReference);
      return base.GetComputedValueInternal(propertyReference);
    }

    public class ConcretePageElementFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new PageElement();
      }
    }
  }
}
