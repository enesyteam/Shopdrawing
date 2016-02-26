// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.Extensibility.SceneNodeViewItem
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Windows.Design.Interaction;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.ViewModel.Extensibility
{
  public class SceneNodeViewItem : ViewItem
  {
    private SceneView view;
    private IViewVisual viewVisual;

    public IViewVisual ViewVisual
    {
      get
      {
        return this.viewVisual;
      }
    }

    public override FlowDirection FlowDirection
    {
      get
      {
        return FlowDirection.LeftToRight;
      }
    }

    public override bool IsOffscreen
    {
      get
      {
        return false;
      }
    }

    public override bool IsVisible
    {
      get
      {
        DocumentNode correspondingDocumentNode = this.view.GetCorrespondingDocumentNode((IViewObject) this.viewVisual, false);
        if (correspondingDocumentNode != null)
        {
          SceneElement sceneElement = this.view.ViewModel.GetSceneNode(correspondingDocumentNode) as SceneElement;
          if (sceneElement != null)
            return !sceneElement.IsHiddenOrCollapsedOrAncestorHiddenOrCollapsed;
        }
        return true;
      }
    }

    public override Transform LayoutTransform
    {
      get
      {
        if (this.view.ProjectContext.ResolveProperty(BaseFrameworkElement.LayoutTransformProperty) != null)
          return (Transform) this.view.ConvertToWpfValue(this.viewVisual.GetCurrentValue(this.view.ProjectContext.ResolveProperty(BaseFrameworkElement.LayoutTransformProperty)));
        return Transform.Identity;
      }
    }

    public override IEnumerable<ViewItem> LogicalChildren
    {
      get
      {
        yield break;
      }
    }

    public override ViewItem LogicalParent
    {
      get
      {
        return this.VisualParent;
      }
    }

    public override Vector Offset
    {
      get
      {
        return new Vector(0.0, 0.0);
      }
    }

    public override object PlatformObject
    {
      get
      {
        return this.viewVisual.PlatformSpecificObject;
      }
    }

    public override Size RenderSize
    {
      get
      {
        return this.ViewVisual.RenderSize;
      }
    }

    public override Rect RenderSizeBounds
    {
      get
      {
        if (!this.view.IsClosing)
          return this.view.GetActualBounds((IViewObject) this.ViewVisual);
        return Rect.Empty;
      }
    }

    public override Rect SelectionFrameBounds
    {
      get
      {
        if (!this.view.IsClosing)
          return this.view.GetActualBoundsInParent((IViewObject) this.ViewVisual);
        return Rect.Empty;
      }
    }

    public override Transform Transform
    {
      get
      {
        DocumentNode correspondingDocumentNode = this.view.GetCorrespondingDocumentNode((IViewObject) this.viewVisual, false);
        if (correspondingDocumentNode != null)
        {
          Base2DElement base2Delement = this.view.ViewModel.GetSceneNode(correspondingDocumentNode) as Base2DElement;
          if (base2Delement != null && base2Delement.IsAttached)
            return (Transform) new MatrixTransform(base2Delement.GetEffectiveRenderTransform(false));
        }
        if (this.view.ProjectContext.ResolveProperty(Base2DElement.RenderTransformProperty) != null)
          return (Transform) this.view.ConvertToWpfValue(this.viewVisual.GetCurrentValue(this.view.ProjectContext.ResolveProperty(Base2DElement.RenderTransformProperty)));
        return Transform.Identity;
      }
    }

    public override Visibility Visibility
    {
      get
      {
        return (Visibility) this.view.ConvertToWpfValue(this.viewVisual.GetCurrentValue(this.view.ProjectContext.ResolveProperty(Base2DElement.VisibilityProperty)));
      }
    }

    public override IEnumerable<ViewItem> VisualChildren
    {
      get
      {
        for (int i = 0; i < this.ViewVisual.VisualChildrenCount; ++i)
        {
          IViewVisual child = this.ViewVisual.GetVisualChild(i);
          if (child != null)
            yield return (ViewItem) new SceneNodeViewItem(this.view, child);
        }
      }
    }

    public override ViewItem VisualParent
    {
      get
      {
        IViewVisual viewVisual = this.ViewVisual.VisualParent as IViewVisual;
        if (viewVisual != null)
          return (ViewItem) new SceneNodeViewItem(this.view, viewVisual);
        return (ViewItem) null;
      }
    }

    public override Type ItemType
    {
      get
      {
        return this.viewVisual.TargetType;
      }
    }

    public SceneNodeViewItem(SceneNode sceneNode)
      : this(sceneNode.ViewModel.DefaultView, (IViewVisual) sceneNode.ViewObject)
    {
    }

    public SceneNodeViewItem(SceneView view, IViewVisual viewVisual)
    {
      if (viewVisual == null)
        throw new ArgumentNullException("viewVisual");
      this.view = view;
      this.viewVisual = viewVisual;
    }

    public override bool IsDescendantOf(ViewItem ancestor)
    {
      SceneNodeViewItem sceneNodeViewItem = ancestor as SceneNodeViewItem;
      if ((ViewItem) sceneNodeViewItem != (ViewItem) null)
        return sceneNodeViewItem.ViewVisual.IsAncestorOf(this.ViewVisual);
      return false;
    }

    public override bool IsDescendantOf(Visual ancestor)
    {
      return true;
    }

    public override Point PointToScreen(Point point)
    {
      return point;
    }

    public override void UpdateLayout()
    {
    }

    public override GeneralTransform TransformFromVisual(Visual visual)
    {
      Matrix matrix = Matrix.Identity;
      if (!this.view.IsClosing)
        matrix = this.view.GetComputedTransformFromRoot((IViewObject) this.ViewVisual);
      return (GeneralTransform) new MatrixTransform(matrix);
    }

    public override GeneralTransform TransformToView(ViewItem view)
    {
      GeneralTransform generalTransform = (GeneralTransform) Transform.Identity;
      if (!this.view.IsClosing)
        generalTransform = this.view.ComputeTransformToVisual((IViewObject) this.ViewVisual, (IViewObject) ((SceneNodeViewItem) view).ViewVisual);
      return generalTransform;
    }

    public override GeneralTransform TransformToVisual(Visual visual)
    {
      Matrix matrix = Matrix.Identity;
      if (!this.view.IsClosing)
        matrix = this.view.GetComputedTransformToRoot((IViewObject) this.ViewVisual);
      return (GeneralTransform) new MatrixTransform(matrix);
    }

    public override ViewHitTestResult HitTest(Microsoft.Windows.Design.Interaction.ViewHitTestFilterCallback filterCallback, Microsoft.Windows.Design.Interaction.ViewHitTestResultCallback resultCallback, HitTestParameters hitTestParameters)
    {
      return (ViewHitTestResult) null;
    }
  }
}
