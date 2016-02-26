// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Assets.AssetDropToolBehavior
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.Tools.Assets
{
  internal sealed class AssetDropToolBehavior : ElementCreateBehavior
  {
    private ITypeId instanceType;

    protected override ITypeId InstanceType
    {
      get
      {
        return this.instanceType;
      }
    }

    internal AssetDropToolBehavior(ToolBehaviorContext toolContext)
      : base(toolContext)
    {
    }

    internal static bool CanHandleDropData(IDataObject dataObj)
    {
      return AssetDropToolBehavior.GetAssetFromDropData(dataObj) != null;
    }

    private static Asset GetAssetFromDropData(IDataObject dataObj)
    {
      Asset result = (Asset) null;
      if (DragSourceHelper.FirstDataOfType<Asset>(dataObj, ref result))
        return result;
      return (Asset) null;
    }

    protected override bool OnDrop(DragEventArgs args)
    {
      Asset assetFromDropData = AssetDropToolBehavior.GetAssetFromDropData(args.Data);
      if (assetFromDropData != null)
      {
        Point position = args.GetPosition((IInputElement) this.ActiveView.ViewRootContainer);
        ISceneInsertionPoint insertionPoint = this.GetInsertionPoint(position);
        this.ToolBehaviorContext.SnappingEngine.Start(this.ToolBehaviorContext, (BaseFrameworkElement) null, (IList<BaseFrameworkElement>) null);
        Point point = this.ToolBehaviorContext.SnappingEngine.SnapPoint(position, EdgeFlags.All);
        this.ToolBehaviorContext.SnappingEngine.Stop();
        if (assetFromDropData != null && assetFromDropData.CanCreateInstance(insertionPoint))
        {
          SceneView activeView = this.ActiveView;
          Rect rect = new Rect(activeView.TransformPoint((IViewObject) activeView.HitTestRoot, insertionPoint.SceneElement.Visual, point), new Size(double.PositiveInfinity, double.PositiveInfinity));
          assetFromDropData.CreateInstance(this.ActiveView.DesignerContext.LicenseManager, insertionPoint, rect, (OnCreateInstanceAction) null);
          activeView.TryEnterTextEditMode(true);
        }
      }
      this.OnDragDropFinished();
      return true;
    }

    protected override bool OnDragOver(DragEventArgs args)
    {
      bool flag = false;
      Point position = args.GetPosition((IInputElement) this.ActiveView.ViewRootContainer);
      this.UpdatePreviewElement(position);
      Asset assetFromDropData = AssetDropToolBehavior.GetAssetFromDropData(args.Data);
      if (assetFromDropData != null)
      {
        this.instanceType = this.ComputeInstanceType(args.Data);
        ISceneInsertionPoint insertionPoint = this.GetInsertionPoint(position);
        flag = assetFromDropData.CanCreateInstance(insertionPoint);
      }
      args.Effects = flag ? DragDropEffects.Copy : DragDropEffects.None;
      args.Handled = true;
      return true;
    }

    protected override bool OnDragEnter(DragEventArgs args)
    {
      this.instanceType = this.ComputeInstanceType(args.Data);
      return true;
    }

    protected override bool OnDragLeave(DragEventArgs args)
    {
      this.OnDragDropFinished();
      return true;
    }

    private ISceneInsertionPoint GetInsertionPoint(Point dropPoint)
    {
      return this.ActiveSceneViewModel.GetActiveSceneInsertionPointFromPosition(new InsertionPointContext(dropPoint, this.InstanceType));
    }

    private ITypeId ComputeInstanceType(IDataObject dataObject)
    {
      TypeAsset typeAsset = AssetDropToolBehavior.GetAssetFromDropData(dataObject) as TypeAsset;
      if (typeAsset != null)
        return (ITypeId) typeAsset.Type;
      return (ITypeId) null;
    }

    private void OnDragDropFinished()
    {
      this.instanceType = (ITypeId) null;
      if (this.IsSuspended)
        return;
      this.PopSelf();
    }
  }
}
