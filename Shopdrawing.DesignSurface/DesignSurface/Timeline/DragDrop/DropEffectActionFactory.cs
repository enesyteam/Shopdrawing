// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Timeline.DragDrop.DropEffectActionFactory
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Tools.Assets;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.Timeline.DragDrop
{
  public class DropEffectActionFactory : ConcreteDropActionFactory
  {
    public override IDropAction CreateInstance(DragDropContext context)
    {
      this.CheckNullArgument((object) context, "context");
      TypeAsset result1 = (TypeAsset) null;
      if (DragSourceHelper.FirstDataOfType<TypeAsset>(context.Data, ref result1) && PlatformTypes.IsEffectType((ITypeId) result1.Type))
      {
        ISceneInsertionPoint insertionPoint = this.GetInsertionPoint((object) result1, context);
        if (insertionPoint != null)
          return (IDropAction) new DropEffectAssetAction(result1, insertionPoint);
      }
      DocumentNodeMarkerSortedList result2 = (DocumentNodeMarkerSortedList) null;
      if (DragSourceHelper.FirstDataOfType<DocumentNodeMarkerSortedList>(context.Data, ref result2) && result2.Count == 1)
      {
        DocumentNodeMarker marker = result2.MarkerAt(0);
        if (marker != null && marker.Node != null && PlatformTypes.IsEffectType((ITypeId) marker.Node.Type))
        {
          SceneViewModel viewModel = context.Target.TimelineItemManager.ViewModel;
          SceneNode sceneNode = SceneNode.FromMarker<SceneNode>(marker, viewModel);
          if (sceneNode != null)
          {
            ISceneInsertionPoint insertionPoint = this.GetInsertionPoint((object) sceneNode, context);
            if (insertionPoint != null)
              return (IDropAction) new DropEffectSceneNodeAction(sceneNode, insertionPoint);
          }
        }
      }
      return (IDropAction) null;
    }
  }
}
