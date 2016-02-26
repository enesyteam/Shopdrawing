// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.DataTemplateElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using System;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public sealed class DataTemplateElement : FrameworkTemplateElement
  {
    public static readonly IPropertyId DataTypeProperty = (IPropertyId) PlatformTypes.DataTemplate.GetMember(MemberType.LocalProperty, "DataType", MemberAccessTypes.Public);
    private static readonly IPropertyId triggersProperty = (IPropertyId) PlatformTypes.DataTemplate.GetMember(MemberType.LocalProperty, "Triggers", MemberAccessTypes.Public);
    public static readonly DataTemplateElement.ConcreteDataTemplateElementFactory Factory = new DataTemplateElement.ConcreteDataTemplateElementFactory();

    public Type DataType
    {
      get
      {
        return (Type) this.GetLocalValue(DataTemplateElement.DataTypeProperty) ?? typeof (object);
      }
      set
      {
        this.SetLocalValue(DataTemplateElement.DataTypeProperty, (object) value);
      }
    }

    protected override IProperty TriggersProperty
    {
      get
      {
        return this.ProjectContext.ResolveProperty(DataTemplateElement.triggersProperty);
      }
    }

    public class ConcreteDataTemplateElementFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new DataTemplateElement();
      }
    }
  }
}
