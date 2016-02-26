// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Timeline.DragDrop.DropAssetAction
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.Tools.Assets;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.Timeline.DragDrop
{
  internal class DropAssetAction : DropAction<Asset>
  {
    public DropAssetAction(Asset asset, ISceneInsertionPoint insertionPoint)
      : base(asset, insertionPoint)
    {
    }

    protected override bool OnQueryCanDrop(TimelineDragDescriptor descriptor)
    {
      if (!this.SourceData.CanCreateInstance(this.InsertionPoint))
        return false;
      descriptor.DisableCopy();
      descriptor.SetCreateIn(this.InsertionPoint);
      descriptor.TryReplace((object) this.SourceData, SmartInsertionPoint.From(this.InsertionPoint), this.DestinationCollection);
      return descriptor.CanDrop;
    }

    protected override DragDropEffects OnHandleDrop(DragDropEffects dropEffects)
    {
      using (SceneEditTransaction editTransaction = this.ViewModel.CreateEditTransaction(StringTable.AssetAddAssetUndo))
      {
        if (this.SourceData.CreateInstance(this.ViewModel.DesignerContext.LicenseManager, this.InsertionPoint, Rect.Empty, (OnCreateInstanceAction) null) == null)
          dropEffects = DragDropEffects.None;
        editTransaction.Commit();
      }
      return dropEffects;
    }
  }
}
