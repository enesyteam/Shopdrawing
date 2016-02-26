// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.CenterEditBehavior
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal class CenterEditBehavior : AdornedToolBehavior
  {
    private Point originalCenter;
    private string alternateString;

    public override string ActionString
    {
      get
      {
        if (this.alternateString == null)
          return StringTable.UndoUnitMoveCenterTransformTool;
        return this.alternateString;
      }
    }

    private BaseFrameworkElement BaseEditingElement
    {
      get
      {
        return this.EditingElement as BaseFrameworkElement;
      }
    }

    public CenterEditBehavior(ToolBehaviorContext toolContext)
      : base(toolContext)
    {
    }

    private void SetTempActionString(string alternateString)
    {
      this.alternateString = alternateString;
    }

    protected override bool OnButtonDown(Point pointerPosition)
    {
      this.Begin();
      this.originalCenter = this.EditingElementSet.RenderTransformOriginInElementCoordinates;
      return true;
    }

    protected override bool OnDrag(Point dragStartPosition, Point dragCurrentPosition, bool scrollNow)
    {
      this.EnsureEditTransaction();
      if (!this.ToolBehaviorContext.SnappingEngine.IsStarted)
        this.ToolBehaviorContext.SnappingEngine.Start(this.ToolBehaviorContext, (BaseFrameworkElement) null, (IList<BaseFrameworkElement>) null);
      dragCurrentPosition = this.ToolBehaviorContext.SnappingEngine.SnapPoint(dragCurrentPosition);
      Matrix transformMatrix = this.EditingElementSet.GetTransformMatrix((IViewObject) this.ActiveView.HitTestRoot);
      Point point = this.originalCenter * transformMatrix;
      this.UpdateCenterPoint(this.originalCenter + ElementUtilities.GetCorrespondingVector(dragCurrentPosition - point, transformMatrix));
      this.UpdateEditTransaction();
      this.ActiveView.EnsureVisible(this.ActiveAdorner, scrollNow);
      return true;
    }

    protected override bool OnDragEnd(Point dragStartPosition, Point dragEndPosition)
    {
      this.AllDone();
      return base.OnDragEnd(dragStartPosition, dragEndPosition);
    }

    protected override bool OnClickEnd(Point pointerPosition, int clickCount)
    {
      if (clickCount > 1)
      {
        this.SetTempActionString(StringTable.UndoUnitResetCenter);
        this.EnsureEditTransaction();
        this.SetTempActionString((string) null);
        this.UpdateCenterPoint(this.EditingElementSet.GetComputedCenter());
      }
      this.AllDone();
      return base.OnClickEnd(pointerPosition, clickCount);
    }

    private void AllDone()
    {
      this.Finish();
      this.ToolBehaviorContext.SnappingEngine.Stop();
      this.CommitEditTransaction();
    }

    protected virtual void Begin()
    {
    }

    protected virtual void Finish()
    {
    }

    protected virtual void UpdateCenterPoint(Point centerPoint)
    {
      CanonicalTransform canonicalTransform = new CanonicalTransform((Transform) this.EditingElement.GetComputedValueAsWpf(Base2DElement.RenderTransformProperty));
      Point elementCoordinates = this.BaseEditingElement.RenderTransformOriginInElementCoordinates;
      Vector translation = canonicalTransform.Translation;
      Rect computedTightBounds = ((Base2DElement) this.EditingElement).GetComputedTightBounds();
      Point renderTransformOrigin = this.BaseEditingElement.RenderTransformOrigin;
      Point point2 = new Point((centerPoint.X - computedTightBounds.Left) / (computedTightBounds.Width == 0.0 ? 1.0 : computedTightBounds.Width), (centerPoint.Y - computedTightBounds.Top) / (computedTightBounds.Height == 0.0 ? 1.0 : computedTightBounds.Height));
      point2 = RoundingHelper.RoundPosition(point2);
      Point newOrigin = new Point(computedTightBounds.Left + point2.X * computedTightBounds.Width, computedTightBounds.Top + point2.Y * computedTightBounds.Height);
      canonicalTransform.UpdateForNewOrigin(elementCoordinates, newOrigin);
      canonicalTransform.TranslationX = RoundingHelper.RoundLength(canonicalTransform.TranslationX);
      canonicalTransform.TranslationY = RoundingHelper.RoundLength(canonicalTransform.TranslationY);
      if (!Point.Equals(renderTransformOrigin, point2))
        this.BaseEditingElement.RenderTransformOrigin = point2;
      if (!object.Equals((object) translation.X, (object) canonicalTransform.TranslationX))
        this.EditingElement.SetValue(this.EditingElement.Platform.Metadata.CommonProperties.RenderTransformTranslationX, (object) canonicalTransform.TranslationX);
      if (object.Equals((object) translation.Y, (object) canonicalTransform.TranslationY))
        return;
      this.EditingElement.SetValue(this.EditingElement.Platform.Metadata.CommonProperties.RenderTransformTranslationY, (object) canonicalTransform.TranslationY);
    }
  }
}
