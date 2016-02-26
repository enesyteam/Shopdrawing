// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Timeline.DragDrop.DropAssetActionFactory
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Tools.Assets;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.Timeline.DragDrop
{
  public class DropAssetActionFactory : ConcreteDropActionFactory
  {
    public override IDropAction CreateInstance(DragDropContext context)
    {
      this.CheckNullArgument((object) context, "context");
      Asset result = (Asset) null;
      if (DragSourceHelper.FirstDataOfType<Asset>(context.Data, ref result))
      {
        ISceneInsertionPoint insertionPoint = this.GetInsertionPoint((object) result, context);
        if (insertionPoint != null)
          return (IDropAction) new DropAssetAction(result, insertionPoint);
      }
      return (IDropAction) null;
    }
  }
}
