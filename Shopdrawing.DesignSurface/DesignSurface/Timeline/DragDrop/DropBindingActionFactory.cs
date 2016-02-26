// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Timeline.DragDrop.DropBindingActionFactory
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Tools;

namespace Microsoft.Expression.DesignSurface.Timeline.DragDrop
{
  public class DropBindingActionFactory : ConcreteDropActionFactory
  {
    public override IDropAction CreateInstance(DragDropContext context)
    {
      this.CheckNullArgument((object) context, "context");
      DataSchemaNodePathCollection result = (DataSchemaNodePathCollection) null;
      if (DragSourceHelper.FirstDataOfType<DataSchemaNodePathCollection>(context.Data, ref result))
      {
        BindingSceneInsertionPoint insertionPoint = context.Target.GetInsertionPoint((object) result, context) as BindingSceneInsertionPoint;
        if (insertionPoint != null && insertionPoint.SceneNode != null)
          return (IDropAction) new DropBindingAction(result, insertionPoint);
      }
      return (IDropAction) null;
    }
  }
}
