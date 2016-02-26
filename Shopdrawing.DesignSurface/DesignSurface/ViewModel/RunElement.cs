// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.RunElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class RunElement : TextElementSceneElement
  {
    public static readonly IPropertyId TextProperty = (IPropertyId) PlatformTypes.Run.GetMember(MemberType.LocalProperty, "Text", MemberAccessTypes.Public);
    public new static readonly SceneNode.ConcreteSceneNodeFactory Factory = (SceneNode.ConcreteSceneNodeFactory) new RunElement.ConcreteRunElementFactory();

    private class ConcreteRunElementFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new RunElement();
      }
    }
  }
}
