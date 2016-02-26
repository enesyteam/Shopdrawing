// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.Extensibility.SceneNodeViewService
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Services;
using System;

namespace Microsoft.Expression.DesignSurface.ViewModel.Extensibility
{
  public class SceneNodeViewService : ViewService
  {
    private SceneViewModel viewModel;
    private bool selectionSetDirty;

    private event EventHandler LayoutUpdatedInternal;

    public override event EventHandler LayoutUpdated
    {
      add
      {
        this.LayoutUpdatedInternal += value;
      }
      remove
      {
        this.LayoutUpdatedInternal -= value;
      }
    }

    public SceneNodeViewService(SceneViewModel viewModel)
    {
      this.viewModel = viewModel;
    }

    public void Initialize()
    {
      this.viewModel.DefaultView.Artboard.LayoutUpdated += new EventHandler(this.OnArtboardLayoutUpdated);
      this.viewModel.ElementSelectionSet.Changed += new EventHandler(this.OnElementSelectionSetChanged);
    }

    public void OnLayoutUpdated()
    {
      this.selectionSetDirty = false;
      if (this.LayoutUpdatedInternal == null)
        return;
      this.LayoutUpdatedInternal((object) this, EventArgs.Empty);
    }

    private void OnElementSelectionSetChanged(object sender, EventArgs e)
    {
      this.selectionSetDirty = true;
    }

    private void OnArtboardLayoutUpdated(object sender, EventArgs e)
    {
      if (this.viewModel.DefaultView == null || this.viewModel.DefaultView.IsUpdating || (this.viewModel.Document.HasOpenTransaction || this.selectionSetDirty))
        return;
      this.OnLayoutUpdated();
    }

    public override ModelItem GetModel(ViewItem view)
    {
      SceneNodeViewItem sceneNodeViewItem = view as SceneNodeViewItem;
      IViewObject instance = (IViewObject) null;
      if ((ViewItem) sceneNodeViewItem != (ViewItem) null)
        instance = (IViewObject) sceneNodeViewItem.ViewVisual;
      if (instance == null)
        instance = this.viewModel.ProjectContext.Platform.ViewObjectFactory.Instantiate(view.PlatformObject);
      DocumentNode correspondingDocumentNode = this.viewModel.DefaultView.GetCorrespondingDocumentNode(instance, false);
      if (correspondingDocumentNode == null)
        return (ModelItem) null;
      return (ModelItem) this.viewModel.GetSceneNode(correspondingDocumentNode).ModelItem;
    }
  }
}
