// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.ScaleBehavior
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.Tools.Layout;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Diagnostics;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal class ScaleBehavior : AdornedToolBehavior
  {
    private LayoutRoundingOverride layoutRoundingOverride = new LayoutRoundingOverride();
    private bool scaleAroundCenter;
    private bool constrainAspectRatio;
    private Matrix elementToDocumentTransform;
    private CanonicalTransform startTransform;
    private Point startCenter;
    private Rect startBounds;
    private Rect startBoundsInParent;
    private Point startPointerPosition;
    private Vector adornerOffset;
    private Point currentPointerPosition;
    private Point currentCenter;

    public override string ActionString
    {
      get
      {
        return StringTable.UndoUnitScale;
      }
    }

    protected AnchorPointAdorner ActiveAdorner
    {
      get
      {
        return (AnchorPointAdorner) base.ActiveAdorner;
      }
    }

    protected Point StartPointerPosition
    {
      get
      {
        return this.startPointerPosition;
      }
    }

    protected Point CurrentPointerPosition
    {
      get
      {
        return this.currentPointerPosition;
      }
    }

    protected Point StartCenter
    {
      get
      {
        return this.startCenter;
      }
      set
      {
        this.startCenter = value;
        this.UpdateCenter();
      }
    }

    protected Rect StartBounds
    {
      get
      {
        return this.startBounds;
      }
      set
      {
        this.startBounds = value;
        this.UpdateCenter();
      }
    }

    protected CanonicalTransform StartTransform
    {
      get
      {
        return this.startTransform;
      }
    }

    protected BaseFrameworkElement BaseEditingElement
    {
      get
      {
        return this.EditingElement as BaseFrameworkElement;
      }
    }

    protected virtual bool AllowScaleAroundCenter
    {
      get
      {
        return true;
      }
    }

    protected virtual bool UseSnappingEngine
    {
      get
      {
        return true;
      }
    }

    private bool ShouldScaleAroundCenter
    {
      get
      {
        if (this.IsAltDown)
          return this.AllowScaleAroundCenter;
        return false;
      }
    }

    private bool ShouldConstrainAspectRatio
    {
      get
      {
        return this.IsShiftDown;
      }
    }

    public ScaleBehavior(ToolBehaviorContext toolContext)
      : base(toolContext)
    {
    }

    protected virtual Matrix ComputeElementToDocumentTransform()
    {
      return this.EditingElementSet.GetTransformMatrix((IViewObject) this.ActiveView.HitTestRoot);
    }

    protected override bool OnButtonDown(Point pointerPosition)
    {
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.ResizeElement);
      PerformanceUtility.MeasurePerformanceUntilRender(PerformanceEvent.ResizeViaAdorners);
      this.EnsureEditTransaction();
      this.CreateSubTransaction();
      this.layoutRoundingOverride.SetValue((IEnumerable<SceneElement>) this.EditingElementSet.Elements, false);
      this.elementToDocumentTransform = this.ComputeElementToDocumentTransform();
      this.startBounds = this.BaseEditingElement.GetComputedTightBounds();
      this.startBoundsInParent = this.ActiveView.GetActualBoundsInParent(this.BaseEditingElement.Visual);
      this.startPointerPosition = this.ActiveAdorner.GetAnchorPoint(Matrix.Identity);
      this.adornerOffset = this.startPointerPosition * this.elementToDocumentTransform - pointerPosition;
      this.startTransform = new CanonicalTransform((Transform) this.EditingElement.GetComputedValueAsWpf(Base2DElement.RenderTransformProperty));
      this.startCenter = this.BaseEditingElement.RenderTransformOriginInElementCoordinates;
      this.constrainAspectRatio = this.ShouldConstrainAspectRatio;
      this.scaleAroundCenter = this.ShouldScaleAroundCenter;
      this.currentPointerPosition = this.startPointerPosition;
      this.UpdateCenter();
      this.Initialize();
      return true;
    }

    protected override bool OnDrag(Point dragStartPosition, Point dragCurrentPosition, bool scrollNow)
    {
      dragCurrentPosition += this.adornerOffset;
      dragStartPosition += this.adornerOffset;
      if (this.UseSnappingEngine && !this.ToolBehaviorContext.SnappingEngine.IsStarted)
      {
        List<BaseFrameworkElement> list = new List<BaseFrameworkElement>();
        foreach (SceneElement sceneElement in this.EditingElementSet.Elements)
        {
          if (this.EditingElement != sceneElement)
            list.Add((BaseFrameworkElement) sceneElement);
        }
        this.ToolBehaviorContext.SnappingEngine.Start(this.ToolBehaviorContext, this.BaseEditingElement, (IList<BaseFrameworkElement>) list);
      }
      Vector offset = dragCurrentPosition - dragStartPosition;
      SceneElement container = this.EditingElement.VisualElementAncestor ?? this.EditingElement;
      if (this.UseSnappingEngine)
      {
        Vector vector = this.ToolBehaviorContext.SnappingEngine.SnapRect(this.startBoundsInParent, container, offset, this.ActiveAdorner.EdgeFlags);
        dragCurrentPosition += vector;
      }
      this.currentPointerPosition = this.startPointerPosition + ElementUtilities.GetCorrespondingVector(dragCurrentPosition - dragStartPosition, this.elementToDocumentTransform);
      if (!this.ActiveSceneViewModel.AnimationEditor.IsRecording && !(this is DesignTimeSizeBehavior))
        this.ReplaceSubTransaction();
      this.UpdateScale();
      this.UpdateEditTransaction();
      this.ActiveView.UpdateLayout();
      this.ActiveView.EnsureVisible((IAdorner) this.ActiveAdorner, scrollNow);
      return true;
    }

    protected override bool OnDragEnd(Point dragStartPosition, Point dragEndPosition)
    {
      this.AllDone();
      return base.OnDragEnd(dragStartPosition, dragEndPosition);
    }

    protected override bool OnClickEnd(Point pointerPosition, int clickCount)
    {
      this.AllDone();
      return base.OnClickEnd(pointerPosition, clickCount);
    }

    protected virtual void AllDone()
    {
      if (this.UseSnappingEngine)
        this.ToolBehaviorContext.SnappingEngine.Stop();
      this.layoutRoundingOverride.Restore(false);
      this.CommitEditTransaction();
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.ResizeElement);
    }

    protected override bool OnKey(KeyEventArgs args)
    {
      if (!this.IsDragging)
        return false;
      bool flag = false;
      bool scaleAroundCenter = this.ShouldScaleAroundCenter;
      if (this.scaleAroundCenter != scaleAroundCenter)
      {
        this.scaleAroundCenter = scaleAroundCenter;
        this.UpdateCenter();
        flag = true;
      }
      bool constrainAspectRatio = this.ShouldConstrainAspectRatio;
      if (flag || this.constrainAspectRatio != constrainAspectRatio)
      {
        this.constrainAspectRatio = constrainAspectRatio;
        if (!this.ActiveSceneViewModel.AnimationEditor.IsRecording)
          this.ReplaceSubTransaction();
        this.UpdateScale();
      }
      this.UpdateEditTransaction();
      return true;
    }

    protected virtual void Initialize()
    {
    }

    protected virtual void ApplyScale(Vector scale, Point center)
    {
      CanonicalTransform newTransform = new CanonicalTransform(this.startTransform);
      newTransform.ApplyScale(scale, this.BaseEditingElement.RenderTransformOriginInElementCoordinates, center);
      newTransform.ScaleX = RoundingHelper.RoundScale(newTransform.ScaleX);
      newTransform.ScaleY = RoundingHelper.RoundScale(newTransform.ScaleY);
      newTransform.TranslationX = RoundingHelper.RoundLength(newTransform.TranslationX);
      newTransform.TranslationY = RoundingHelper.RoundLength(newTransform.TranslationY);
      AdornedToolBehavior.UpdateElementTransform(this.EditingElement, newTransform, AdornedToolBehavior.TransformPropertyFlags.Scale | AdornedToolBehavior.TransformPropertyFlags.Translation);
    }

    private void UpdateCenter()
    {
      this.currentCenter = this.startCenter;
      if (this.scaleAroundCenter)
        return;
      this.currentCenter = new Point((this.startBounds.Left + this.startBounds.Right) / 2.0, (this.startBounds.Top + this.startBounds.Bottom) / 2.0);
      if (this.ActiveAdorner.TestFlags(EdgeFlags.Left))
        this.currentCenter.X = this.startBounds.Right;
      else if (this.ActiveAdorner.TestFlags(EdgeFlags.Right))
        this.currentCenter.X = this.startBounds.Left;
      if (this.ActiveAdorner.TestFlags(EdgeFlags.Top))
      {
        this.currentCenter.Y = this.startBounds.Bottom;
      }
      else
      {
        if (!this.ActiveAdorner.TestFlags(EdgeFlags.Bottom))
          return;
        this.currentCenter.Y = this.startBounds.Top;
      }
    }

    private void UpdateScale()
    {
      Vector scale = new Vector(1.0, 1.0);
      bool flag1 = Math.Abs(this.startBounds.Width) < 1E-06;
      bool flag2 = Math.Abs(this.startBounds.Height) < 1E-06;
      if (this.constrainAspectRatio && !flag1 && !flag2)
        scale.X = scale.Y = 0.0;
      if (this.ActiveAdorner.TestFlags(EdgeFlags.LeftOrRight))
      {
        double num = this.startPointerPosition.X - this.currentCenter.X;
        if (Math.Abs(num) < 1E-06 || flag1)
        {
          scale.X = 1.0;
          if (this.currentPointerPosition.X - this.startPointerPosition.X < 0.0)
            scale.X = -scale.X;
        }
        else
          scale.X = (this.currentPointerPosition.X - this.currentCenter.X) / num;
      }
      if (this.ActiveAdorner.TestFlags(EdgeFlags.TopOrBottom))
      {
        double num = this.startPointerPosition.Y - this.currentCenter.Y;
        if (Math.Abs(num) < 1E-06 || flag2)
        {
          scale.Y = 1.0;
          if (this.currentPointerPosition.Y - this.startPointerPosition.Y < 0.0)
            scale.Y = -scale.Y;
        }
        else
          scale.Y = (this.currentPointerPosition.Y - this.currentCenter.Y) / num;
      }
      if (this.constrainAspectRatio && !flag1 && !flag2)
      {
        double num = Math.Max(Math.Abs(scale.X), Math.Abs(scale.Y));
        scale.X = scale.X < 0.0 ? -num : num;
        scale.Y = scale.Y < 0.0 ? -num : num;
      }
      if (double.IsNaN(scale.X) || double.IsNaN(scale.Y))
        return;
      this.ApplyScale(scale, this.currentCenter);
    }
  }
}
