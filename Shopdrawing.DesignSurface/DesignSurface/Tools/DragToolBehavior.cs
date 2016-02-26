// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.DragToolBehavior
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal abstract class DragToolBehavior : ToolBehavior
  {
    private BaseFrameworkElement provisionalContainer;
    protected Pen whitePen;
    protected Pen dashPen;

    public TextFeedbackAdorner FeedbackAdorner { get; private set; }

    protected BaseFrameworkElement ProvisionalContainer
    {
      get
      {
        return this.provisionalContainer;
      }
      set
      {
        if (this.provisionalContainer == value)
          return;
        this.provisionalContainer = value;
        this.UpdateProvisionalContainerHighlight();
      }
    }

    protected Pen WhitePen
    {
      get
      {
        if (this.whitePen == null)
        {
          this.whitePen = new Pen();
          this.whitePen.Brush = (Brush) Brushes.White;
        }
        return this.whitePen;
      }
    }

    protected Pen DashPen
    {
      get
      {
        if (this.dashPen == null)
        {
          this.dashPen = new Pen();
          this.dashPen.Brush = (Brush) Brushes.Black;
          this.dashPen.DashStyle = new DashStyle((IEnumerable<double>) new double[2]
          {
            3.0,
            3.0
          }, 0.0);
        }
        return this.dashPen;
      }
    }

    protected DragToolBehavior(ToolBehaviorContext toolContext)
      : base(toolContext)
    {
      this.FeedbackAdorner = new TextFeedbackAdorner(this.ActiveView);
      this.FeedbackAdorner.Offset = new Vector(10.0, 20.0);
      this.FeedbackAdorner.CloseDelay = TimeSpan.MaxValue;
    }

    protected void SetTextCuePosition(Point p)
    {
      if (this.FeedbackAdorner == null)
        return;
      this.FeedbackAdorner.SetPosition(p);
    }

    protected SceneElement GetUnlockedAncestorInEditingContainer(DocumentNodePath nodePath)
    {
      IStoryboardContainer storyboardContainer = this.ActiveSceneViewModel.ActiveStoryboardContainer;
      DocumentNodePath targetElementPath = storyboardContainer.TargetElement != null ? storyboardContainer.TargetElement.DocumentNodePath : ((SceneNode) storyboardContainer).DocumentNodePath;
      DocumentNodePath editingContainer = this.ActiveSceneViewModel.GetAncestorInEditingContainer(nodePath, this.ActiveSceneViewModel.ActiveEditingContainerPath, targetElementPath);
      nodePath = editingContainer == null ? this.ActiveSceneViewModel.GetAncestorInEditingContainer(nodePath, this.ActiveSceneViewModel.ViewRoot.DocumentNodePath, this.ActiveSceneViewModel.ViewRoot.DocumentNodePath) : editingContainer;
      SceneElement sceneElement = (SceneElement) null;
      if (nodePath != null)
        sceneElement = this.ActiveSceneViewModel.GetUnlockedAncestor(nodePath);
      return sceneElement;
    }

    protected BaseFrameworkElement GetHitElement(Point point, IList<BaseFrameworkElement> ignoredElements)
    {
      SceneElement elementAtPoint = this.ActiveView.GetElementAtPoint(point, new HitTestModifier(this.GetUnlockedAncestorInEditingContainer), (InvisibleObjectHitTestModifier) null, (ICollection<BaseFrameworkElement>) ignoredElements);
      BaseFrameworkElement frameworkElement = elementAtPoint as BaseFrameworkElement;
      Base3DElement base3Delement = elementAtPoint as Base3DElement;
      if (base3Delement != null)
        frameworkElement = (BaseFrameworkElement) base3Delement.Viewport;
      return frameworkElement;
    }

    protected void UpdateProvisionalContainerHighlight()
    {
      if (this.ProvisionalContainer == null)
      {
        this.FeedbackAdorner.CloseAdorner();
      }
      else
      {
        SceneView activeView = this.ActiveView;
        BaseFrameworkElement frameworkElement = this.provisionalContainer;
        this.FeedbackAdorner.Text = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.ProvisionalDragDropText, new object[1]
        {
          (object) frameworkElement.DisplayNameNoTextContent
        });
        DrawingContext context = this.OpenFeedback();
        Rect computedTightBounds = frameworkElement.GetComputedTightBounds();
        System.Windows.Media.Geometry rectangleGeometry = Adorner.GetTransformedRectangleGeometry(activeView, (SceneElement)frameworkElement, computedTightBounds, 0.0, false);
        this.DashPen.Thickness = 1.5 / activeView.Zoom;
        this.WhitePen.Thickness = 1.5 / activeView.Zoom;
        context.DrawGeometry((Brush) null, this.WhitePen, rectangleGeometry);
        context.DrawGeometry((Brush) null, this.DashPen, rectangleGeometry);
        this.FeedbackAdorner.DrawAdorner(context);
        this.CloseFeedback();
      }
    }
  }
}
