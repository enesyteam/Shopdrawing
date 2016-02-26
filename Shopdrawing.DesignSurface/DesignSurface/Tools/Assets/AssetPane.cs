// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Assets.AssetPane
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignSurface.Tools.Assets
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class AssetPane : Grid, IComponentConnector
  {
    private DesignerContext designerContext;
    private ToolContext toolContext;
    internal AssetView AssetView;
    private bool _contentLoaded;

    internal AssetPane(DesignerContext designerContext, ToolContext toolContext)
    {
      this.designerContext = designerContext;
      this.toolContext = toolContext;
      this.DataContext = (object) designerContext.AssetLibrary;
      this.InitializeComponent();
      this.AssetView.AssetSingleClicked += new EventHandler<AssetEventArgs>(this.AssetView_AssetSingleClicked);
      this.AssetView.AssetDoubleClicked += new EventHandler<AssetEventArgs>(this.AssetView_AssetDoubleClicked);
      this.AssetView.SelectedAssetChanged += new EventHandler<AssetEventArgs>(this.AssetView_SelectedAssetChanged);
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
      base.OnMouseDown(e);
      if (this.designerContext.ActiveView == null || this.AssetView.SearchBoxHasFocus)
        return;
      this.designerContext.ActiveView.ReturnFocus();
    }

    private void AssetView_SelectedAssetChanged(object sender, AssetEventArgs e)
    {
      this.toolContext.AssetMruList.OnSelectAsset(e.Asset);
    }

    private void AssetView_AssetSingleClicked(object sender, AssetEventArgs e)
    {
      this.toolContext.AssetMruList.ActivateAssetTool();
    }

    private void AssetView_AssetDoubleClicked(object sender, AssetEventArgs e)
    {
      SceneViewModel activeSceneViewModel = this.designerContext.ActiveSceneViewModel;
      if (activeSceneViewModel == null || !activeSceneViewModel.IsEditable || (activeSceneViewModel.DefaultView == null || !activeSceneViewModel.DefaultView.IsDesignSurfaceEnabled) || (e.Asset == null || !e.Asset.IsValid))
        return;
      bool flag = false;
      string description = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.UndoUnitCreateControlFormat, new object[1]
      {
        (object) e.Asset.Name
      });
      using (SceneEditTransaction editTransaction = activeSceneViewModel.CreateEditTransaction(description))
      {
        foreach (ISceneInsertionPoint insertionPoint in e.Asset.FindInsertionPoints(activeSceneViewModel))
        {
          if (e.Asset.CanCreateInstance(insertionPoint))
          {
            e.Asset.CreateInstance(this.designerContext.LicenseManager, insertionPoint, Rect.Empty, (OnCreateInstanceAction) null);
            flag = true;
          }
        }
        editTransaction.Commit();
        if (!flag || !e.Asset.SupportsTextEditing(activeSceneViewModel.ProjectContext))
          return;
        this.designerContext.ActiveView.TryEnterTextEditMode(true);
      }
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/tools/assettool/assetlibrary/assetpane.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    internal Delegate _CreateDelegate(Type delegateType, string handler)
    {
      return Delegate.CreateDelegate(delegateType, (object) this, handler);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerNonUserCode]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      if (connectionId == 1)
        this.AssetView = (AssetView) target;
      else
        this._contentLoaded = true;
    }
  }
}
