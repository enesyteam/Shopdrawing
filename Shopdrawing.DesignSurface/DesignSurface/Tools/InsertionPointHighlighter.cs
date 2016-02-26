// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.InsertionPointHighlighter
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Tools.Transforms;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal class InsertionPointHighlighter
  {
    private SceneViewModel viewModel;
    private ISceneInsertionPoint insertionPointPreview;
    private PreviewElementHighlighter previewHighlighter;

    public HighlightOption Option { get; set; }

    public ISceneInsertionPoint InsertionPointPreview
    {
      get
      {
        return this.insertionPointPreview;
      }
      set
      {
        this.insertionPointPreview = value;
        if (this.insertionPointPreview != null && (this.viewModel.LockedInsertionPoint != null && this.insertionPointPreview.SceneElement == this.viewModel.LockedInsertionPoint.SceneElement && SceneInsertionPointHelper.IsDefaultContentProperty(this.insertionPointPreview)))
          this.insertionPointPreview = (ISceneInsertionPoint) null;
        SceneElement sceneElement = (SceneElement) null;
        if (this.insertionPointPreview != null)
        {
          sceneElement = this.insertionPointPreview.SceneElement;
          if (sceneElement == this.viewModel.FindPanelClosestToRoot() && SceneInsertionPointHelper.IsDefaultContentProperty(this.insertionPointPreview))
            sceneElement = (SceneElement) null;
        }
        this.previewHighlighter.PreviewElement = sceneElement;
        this.viewModel.TimelineItemManager.HoverOverrideInsertionPoint = this.insertionPointPreview;
      }
    }

    public InsertionPointHighlighter(ToolBehaviorContext toolContext)
    {
      InsertionPointHighlighter pointHighlighter = this;
      this.previewHighlighter = new PreviewElementHighlighter(toolContext.View.AdornerLayer, (PreviewElementHighlighter.CreateAdornerSet) (adornedElement => pointHighlighter.CreateAdornerSet(toolContext)), (PreviewElementHighlighter.VerifyIsEnabled) (() => toolContext.ToolManager.ShowActiveContainer));
      this.viewModel = toolContext.View.ViewModel;
    }

    private AnimatableAdornerSet CreateAdornerSet(ToolBehaviorContext toolContext)
    {
      HighlightOption highlightOption = this.Option;
      if (highlightOption == HighlightOption.Default)
        highlightOption = SceneInsertionPointHelper.IsDefaultContentProperty(this.insertionPointPreview) ? HighlightOption.Insert : HighlightOption.Preview;
      if (highlightOption == HighlightOption.Insert)
        return (AnimatableAdornerSet) new SceneInsertionPointAdornerSet(toolContext, this.insertionPointPreview.SceneElement);
      if (highlightOption == HighlightOption.Preview)
        return (AnimatableAdornerSet) new SelectionPreviewBoundingBoxAdornerSet(toolContext, this.insertionPointPreview.SceneElement);
      return (AnimatableAdornerSet) null;
    }
  }
}
