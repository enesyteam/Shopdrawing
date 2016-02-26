// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.EffectInsertionPointCreator
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Tools.Assets;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface
{
  public class EffectInsertionPointCreator : IInsertionPointCreator
  {
    public SceneElement Element { get; private set; }

    public EffectInsertionPointCreator(SceneElement targetElement)
    {
      this.Element = targetElement;
    }

    public ISceneInsertionPoint Create(object data)
    {
      if (this.Element != null)
      {
        TypeAsset typeAsset = data as TypeAsset;
        if (typeAsset != null && PlatformTypes.IsEffectType((ITypeId) typeAsset.Type))
        {
          IProperty targetProperty = this.Element.ProjectContext.ResolveProperty(Base2DElement.EffectProperty);
          if (targetProperty != null)
            return (ISceneInsertionPoint) new PropertySceneInsertionPoint(this.Element, targetProperty);
        }
        SceneNode sceneNode = data as SceneNode;
        if (sceneNode != null && PlatformTypes.IsEffectType((ITypeId) sceneNode.Type))
          return (ISceneInsertionPoint) new PropertySceneInsertionPoint(this.Element, this.Element.ProjectContext.ResolveProperty(Base2DElement.EffectProperty));
      }
      return (ISceneInsertionPoint) null;
    }
  }
}
