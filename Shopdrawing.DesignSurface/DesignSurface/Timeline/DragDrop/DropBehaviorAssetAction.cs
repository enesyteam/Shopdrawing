// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Timeline.DragDrop.DropBehaviorAssetAction
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Tools.Assets;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.Timeline.DragDrop
{
  internal class DropBehaviorAssetAction : DropAssetAction
  {
    public DropBehaviorAssetAction(TypeAsset typeAsset, ISceneInsertionPoint insertionPoint)
      : base((Asset) typeAsset, insertionPoint)
    {
    }

    protected override bool OnQueryCanDrop(TimelineDragDescriptor descriptor)
    {
      this.CheckNullArgument((object) descriptor, "descriptor");
      if (PlatformTypes.FrameworkTemplate.IsAssignableFrom((ITypeId) this.TargetNode.Type) || PlatformTypes.Style.IsAssignableFrom((ITypeId) this.TargetNode.Type))
        return false;
      descriptor.DisableInBetween();
      return base.OnQueryCanDrop(descriptor);
    }
  }
}
