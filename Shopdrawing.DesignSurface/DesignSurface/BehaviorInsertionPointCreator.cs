// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.BehaviorInsertionPointCreator
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Tools.Assets;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface
{
  internal class BehaviorInsertionPointCreator : IInsertionPointCreator
  {
    public SceneElement Element { get; private set; }

    public BehaviorInsertionPointCreator(SceneElement targetElement)
    {
      this.Element = targetElement;
    }

    public ISceneInsertionPoint Create(object data)
    {
      if (this.Element != null)
      {
        IType type = (IType) null;
        TypeAsset typeAsset = data as TypeAsset;
        BehaviorBaseNode behaviorBaseNode = data as BehaviorBaseNode;
        if (typeAsset != null)
          type = typeAsset.Type;
        if (typeof (DocumentNodeMarkerSortedList).IsAssignableFrom(data.GetType()))
          type = ((DocumentNodeMarkerSortedListBase) data).MarkerAt(0).Node.Type;
        else if (behaviorBaseNode != null)
          type = behaviorBaseNode.Type;
        IProperty targetProperty = (IProperty) null;
        if (ProjectNeutralTypes.BehaviorTriggerAction.IsAssignableFrom((ITypeId) type))
          targetProperty = this.Element.ProjectContext.ResolveProperty(BehaviorHelper.BehaviorTriggersProperty);
        else if (ProjectNeutralTypes.Behavior.IsAssignableFrom((ITypeId) type))
          targetProperty = this.Element.ProjectContext.ResolveProperty(BehaviorHelper.BehaviorsProperty);
        if (targetProperty != null)
          return (ISceneInsertionPoint) new PropertySceneInsertionPoint(this.Element, targetProperty);
      }
      return (ISceneInsertionPoint) null;
    }
  }
}
