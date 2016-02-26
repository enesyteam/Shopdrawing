// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Timeline.DragDrop.DropActionFactory
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.Timeline.DragDrop
{
  public static class DropActionFactory
  {
    private static IList<IDropActionFactory> factoryList = (IList<IDropActionFactory>) new List<IDropActionFactory>();

    static DropActionFactory()
    {
      DropActionFactory.factoryList.Add((IDropActionFactory) new DropEffectActionFactory());
      DropActionFactory.factoryList.Add((IDropActionFactory) new DropBehaviorActionFactory());
      DropActionFactory.factoryList.Add((IDropActionFactory) new DropAssetActionFactory());
      DropActionFactory.factoryList.Add((IDropActionFactory) new DropMarkerListActionFactory());
      DropActionFactory.factoryList.Add((IDropActionFactory) new DropBindingActionFactory());
    }

    public static IDropAction CreateInstance(DragDropContext context)
    {
      TimelineDragDescriptor timelineDragDescriptor = context.Descriptor.Clone();
      foreach (IDropActionFactory dropActionFactory in (IEnumerable<IDropActionFactory>) DropActionFactory.factoryList)
      {
        IDropAction instance = dropActionFactory.CreateInstance(context);
        if (instance != null)
          return instance;
        context.Descriptor = timelineDragDescriptor.Clone();
      }
      context.Descriptor.DisableDrop();
      return (IDropAction) null;
    }
  }
}
