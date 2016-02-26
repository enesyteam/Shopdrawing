// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Timeline.DragDrop.DropBindingAction
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.Timeline.DragDrop
{
  internal class DropBindingAction : DropAction<DataSchemaNodePathCollection>
  {
    private BindingSceneInsertionPoint BindingInsertionPoint
    {
      get
      {
        return (BindingSceneInsertionPoint) this.InsertionPoint;
      }
    }

    private DataBindingDragDropModel DragFeedback
    {
      get
      {
        return DataBindingDragDropManager.GetDragFeedback(this.SourceData, this.BindingInsertionPoint, this.Flags, Keyboard.Modifiers);
      }
    }

    private DataBindingDragDropFlags Flags
    {
      get
      {
        BindingSceneInsertionPoint bindingInsertionPoint = this.BindingInsertionPoint;
        if (bindingInsertionPoint == null || bindingInsertionPoint.SceneNode == null || bindingInsertionPoint.SceneNode is StyleNode)
          return DataBindingDragDropFlags.None;
        DataBindingDragDropFlags bindingDragDropFlags = DataBindingDragDropFlags.ObjectTreeDefault;
        if (bindingInsertionPoint.InsertIndex >= 0)
          bindingDragDropFlags &= ~DataBindingDragDropFlags.SetBinding;
        return bindingDragDropFlags;
      }
    }

    public DropBindingAction(DataSchemaNodePathCollection dataSourceInfo, BindingSceneInsertionPoint insertionPoint)
      : base(dataSourceInfo, (ISceneInsertionPoint) insertionPoint)
    {
    }

    protected override bool OnQueryCanDrop(TimelineDragDescriptor descriptor)
    {
      DataBindingDragDropModel dragFeedback = this.DragFeedback;
      if (dragFeedback == null || dragFeedback.DropFlags == DataBindingDragDropFlags.None)
        return false;
      descriptor.DisableCopy();
      descriptor.SetDataBindingTo((ISceneInsertionPoint) this.BindingInsertionPoint);
      descriptor.UserData = (object) dragFeedback;
      return true;
    }

    protected override DragDropEffects OnHandleDrop(DragDropEffects dropEffects)
    {
      DataBindingDragDropManager.Drop(this.DragFeedback, new Point());
      return dropEffects;
    }
  }
}
