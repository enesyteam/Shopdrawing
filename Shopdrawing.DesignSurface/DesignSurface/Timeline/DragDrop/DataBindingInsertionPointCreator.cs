// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Timeline.DragDrop.DataBindingInsertionPointCreator
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface.Timeline;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.Timeline.DragDrop
{
  internal class DataBindingInsertionPointCreator : DefaultTimelineItemInsertionPointCreator
  {
    public IProperty TargetProperty { get; private set; }

    public DataBindingInsertionPointCreator(TimelineItem targetItem, SceneNode node, IProperty targetProperty, DragDropContext context)
      : base(targetItem, node, context)
    {
      this.TargetProperty = targetProperty;
    }

    public override ISceneInsertionPoint Create(object data)
    {
      if (data is DataSchemaNodePathCollection)
        return (ISceneInsertionPoint) new BindingSceneInsertionPoint((SceneNode) this.GetDropTargetInfo(this.Context.AllowedEffects, this.Context.Descriptor) ?? this.Node, this.TargetProperty, this.Context.Descriptor.DropIndex);
      return (ISceneInsertionPoint) null;
    }
  }
}
