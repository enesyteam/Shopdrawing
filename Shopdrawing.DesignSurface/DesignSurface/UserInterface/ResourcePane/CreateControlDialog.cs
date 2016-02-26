// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.ResourcePane.CreateControlDialog
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Tools.Assets;
using Microsoft.Expression.Framework.Controls;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Expression.DesignSurface.UserInterface.ResourcePane
{
  internal sealed class CreateControlDialog : CreateResourceDialog
  {
    private Grid typeChooserContainer;
    private OnDemandControl assetViewContainer;
    private AssetView assetView;

    internal CreateControlDialog(DesignerContext designerContext, CreateResourceModel model)
      : base(designerContext, model)
    {
      this.Title = StringTable.MakeControlDialogTitle;
      this.typeChooserContainer = (Grid) LogicalTreeHelper.FindLogicalNode((DependencyObject) this, "TypeSelectorComponent");
      this.typeChooserContainer.Visibility = Visibility.Visible;
      this.assetViewContainer = (OnDemandControl) LogicalTreeHelper.FindLogicalNode((DependencyObject) this, "AssetViewContainer");
      this.assetViewContainer.DataContext = (object) designerContext.AssetLibrary;
      this.assetViewContainer.Visibility = Visibility.Visible;
    }

    protected override void OnActivated(EventArgs e)
    {
      base.OnActivated(e);
      this.assetView = (AssetView) this.assetViewContainer.Template.FindName("AssetView", (FrameworkElement) this.assetViewContainer);
      this.assetView.AssetFilter = (IAssetFilter) new CreateControlDialog.ControlsAssetFilter();
      this.assetView.CategoryFilter = (ICategoryFilter) new CreateControlDialog.ControlsCategoryFilter();
      this.assetView.SelectedAssetChanged += (EventHandler<AssetEventArgs>) ((s, args) => this.AssetSelected(args.Asset));
      this.assetView.AssetDoubleClicked += (EventHandler<AssetEventArgs>) ((s, args) => this.AssetDoubleClicked(args.Asset));
    }

    private void AssetDoubleClicked(Asset asset)
    {
      this.AssetSelected(asset);
      if (!this.Model.ResourceIsValid)
        return;
      this.OnAcceptButtonExecute();
    }

    private void AssetSelected(Asset asset)
    {
      this.Model.TargetType = asset.TargetType.RuntimeType;
      this.Model.TargetTypeAsset = asset as TypeAsset;
      this.Model.ResetResourceKey();
    }

    private class ControlsAssetFilter : IAssetFilter
    {
      public bool IsValid(Asset asset, AssetTypeHelper typeHelper)
      {
        TypeAsset typeAsset = asset as TypeAsset;
        if (typeAsset != null)
        {
          IType type = typeAsset.Type;
          if (type != null && PlatformTypes.Control.IsAssignableFrom((ITypeId) type) && !PlatformTypes.UserControl.IsAssignableFrom((ITypeId) type))
            return true;
        }
        return false;
      }
    }

    private class ControlsCategoryFilter : ICategoryFilter
    {
      public bool IsValid(AssetCategoryPath path, AssetTypeHelper typeHelper)
      {
        return PresetAssetCategoryPath.ControlsRoot.Contains(path);
      }
    }
  }
}
