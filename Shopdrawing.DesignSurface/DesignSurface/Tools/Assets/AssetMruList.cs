// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Assets.AssetMruList
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using System;
using System.Collections.Generic;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.Tools.Assets
{
  internal sealed class AssetMruList
  {
    private List<Asset> mruAssetList = new List<Asset>();
    private DesignerContext designerContext;
    private ToolContext toolContext;
    private AssetLibrary assetLibrary;
    private Asset activeAsset;
    private bool updateMruValidStateScheduled;

    public AssetMruList(DesignerContext designerContext, ToolContext toolContext)
    {
      this.designerContext = designerContext;
      this.toolContext = toolContext;
      this.assetLibrary = (AssetLibrary) this.designerContext.AssetLibrary;
      this.assetLibrary.AssetLibraryChanged += new Action<AssetLibraryDamages>(this.OnAssetLibraryChanged);
      this.designerContext.SelectionManager.LateActiveSceneUpdatePhase += new SceneUpdatePhaseEventHandler(this.SelectionManager_LateActiveSceneUpdatePhase);
    }

    public void OnSelectAsset(Asset asset)
    {
      this.AssetSelected(asset);
    }

    private static bool IsMruSupported(Asset asset)
    {
      if (asset == null)
        return false;
      TypeAsset typeAsset = asset as TypeAsset;
      return (typeAsset == null || !PlatformTypes.Effect.IsAssignableFrom((ITypeId) typeAsset.Type)) && (!(asset is BehaviorAsset) && !(asset is TriggerActionAsset));
    }

    private void BeginInvokeUpdateMruValidState()
    {
      if (this.updateMruValidStateScheduled)
        return;
      this.updateMruValidStateScheduled = true;
      UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(this.UpdateMruValidState));
    }

    private void OnAssetLibraryChanged(AssetLibraryDamages damages)
    {
      if ((damages & AssetLibraryDamages.Assets) == AssetLibraryDamages.None)
        return;
      this.BeginInvokeUpdateMruValidState();
    }

    private void SelectionManager_LateActiveSceneUpdatePhase(object sender, SceneUpdatePhaseEventArgs args)
    {
      if (args.Document != null && args.Document.HasOpenTransaction || !args.IsRadicalChange && !args.SceneSwitched && !this.assetLibrary.NeedsUpdate)
        return;
      this.BeginInvokeUpdateMruValidState();
    }

    private void UpdateMruValidState()
    {
      this.updateMruValidStateScheduled = false;
      if (this.toolContext.ActiveView != null && this.toolContext.ActiveView.EventRouter.IsButtonDown)
        this.BeginInvokeUpdateMruValidState();
      else if (Microsoft.Expression.DesignSurface.UnsafeNativeMethods.GetCapture() != IntPtr.Zero)
      {
        this.BeginInvokeUpdateMruValidState();
      }
      else
      {
        bool flag1 = false;
        for (int index = 0; index < this.mruAssetList.Count; ++index)
        {
          Asset asset1 = this.mruAssetList[index];
          bool flag2 = false;
          Asset asset2 = this.assetLibrary.FindAsset(asset1);
          if (asset2 != null)
          {
            flag2 = true;
            this.mruAssetList[index] = asset2;
            if (this.activeAsset == asset1)
              this.activeAsset = asset2;
          }
          if (asset1 != this.mruAssetList[index] || asset1.IsValid != flag2)
            flag1 = true;
          asset1.IsValid = flag2;
        }
        if (!flag1)
          return;
        this.OnMruGroupChanged();
      }
    }

    private void OnMruGroupChanged()
    {
      ToolManager toolManager = this.designerContext.ToolManager;
      if (toolManager == null)
        return;
      AssetTool assetTool = toolManager.ActiveTool as AssetTool;
      for (int index = toolManager.Tools.Count - 1; index >= 0; --index)
      {
        Tool tool = toolManager.Tools[index];
        if (tool.Category == ToolCategory.Asset)
          toolManager.Remove(tool);
      }
      foreach (Asset asset in this.mruAssetList)
      {
        Tool tool = (Tool) new AssetTool(this.toolContext, asset);
        toolManager.Add(tool);
        if (assetTool != null && this.activeAsset == asset && (asset.IsValid && tool.IsEnabled))
          toolManager.ActiveTool = tool;
      }
    }

    private void AssetSelected(Asset asset)
    {
      this.activeAsset = asset;
      if (AssetMruList.IsMruSupported(this.activeAsset))
      {
        for (int index = 0; index < this.mruAssetList.Count; ++index)
        {
          if (this.mruAssetList[index].CompareTo(this.activeAsset) == 0)
          {
            this.mruAssetList.RemoveAt(index);
            break;
          }
        }
        this.mruAssetList.Insert(0, this.activeAsset);
        if (this.mruAssetList.Count > 10)
          this.mruAssetList.RemoveAt(10);
        this.OnMruGroupChanged();
      }
      this.ActivateAssetTool();
    }

    internal void ActivateAssetTool()
    {
      foreach (Tool tool in this.designerContext.ToolManager.Tools)
      {
        AssetTool assetTool = tool as AssetTool;
        if (assetTool != null && assetTool.Asset == this.activeAsset && assetTool.IsEnabled)
        {
          this.designerContext.ToolManager.ActiveTool = (Tool) assetTool;
          break;
        }
      }
    }
  }
}
