// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Timeline.DragDrop.DropMarkerListActionFactory
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.Timeline.DragDrop
{
  public class DropMarkerListActionFactory : ConcreteDropActionFactory
  {
    public override IDropAction CreateInstance(DragDropContext context)
    {
      this.CheckNullArgument((object) context, "context");
      DocumentNodeMarkerSortedList result = (DocumentNodeMarkerSortedList) null;
      if (DragSourceHelper.FirstDataOfType<DocumentNodeMarkerSortedList>(context.Data, ref result))
      {
        ISceneInsertionPoint insertionPoint = this.GetInsertionPoint((object) result, context);
        if (insertionPoint != null)
        {
          foreach (DocumentNodeMarker documentNodeMarker in result.Markers)
          {
            if (documentNodeMarker.Node != null && documentNodeMarker.Node.Type != null && !insertionPoint.CanInsert((ITypeId) documentNodeMarker.Node.Type))
              return (IDropAction) null;
          }
          return (IDropAction) new DropMarkerListAction(result, insertionPoint);
        }
      }
      return (IDropAction) null;
    }
  }
}
