// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.BitmapImageNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class BitmapImageNode : SceneNode
  {
    public static readonly IPropertyId UriSourceProperty = (IPropertyId) PlatformTypes.BitmapImage.GetMember(MemberType.LocalProperty, "UriSource", MemberAccessTypes.Public);
    public static readonly BitmapImageNode.ConcreteBitmapImageNodeFactory Factory = new BitmapImageNode.ConcreteBitmapImageNodeFactory();

    public UriNode UriSource
    {
      get
      {
        return (UriNode) this.GetLocalValueAsSceneNode(BitmapImageNode.UriSourceProperty);
      }
      set
      {
        this.SetValueAsSceneNode(BitmapImageNode.UriSourceProperty, (SceneNode) value);
      }
    }

    public class ConcreteBitmapImageNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new BitmapImageNode();
      }
    }
  }
}
