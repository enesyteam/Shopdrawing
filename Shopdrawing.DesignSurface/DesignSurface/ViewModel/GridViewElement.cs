// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.GridViewElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public sealed class GridViewElement : SceneElement
  {
    public static readonly IPropertyId ColumnsProperty = (IPropertyId) PlatformTypes.GridView.GetMember(MemberType.LocalProperty, "Columns", MemberAccessTypes.Public);
    public static readonly IPropertyId ColumnHeaderTemplateProperty = (IPropertyId) PlatformTypes.GridView.GetMember(MemberType.Property, "ColumnHeaderTemplate", MemberAccessTypes.Public);
    public static readonly GridViewElement.ConcreteGridViewElementFactory Factory = new GridViewElement.ConcreteGridViewElementFactory();

    public IList<SceneNode> Columns
    {
      get
      {
        return (IList<SceneNode>) this.GetCollectionForProperty(GridViewElement.ColumnsProperty);
      }
    }

    public class ConcreteGridViewElementFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new GridViewElement();
      }
    }
  }
}
