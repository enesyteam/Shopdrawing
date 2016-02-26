// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Designers.LayoutMoveStrategy
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.Designers
{
  internal class LayoutMoveStrategy : MoveStrategy
  {
    public bool EnableAnimationMode { get; set; }

    public LayoutMoveStrategy(MoveStrategyContext context)
      : base(context)
    {
    }

    protected override void OnBeginDrag()
    {
      if (this.EnableAnimationMode && this.Context.IsRecordingKeyframes)
        return;
      this.ReparentDraggedElementsIntoContainer();
    }

    protected override void OnContinueDrag(BaseFrameworkElement hitElement)
    {
      this.MoveDraggedElementsInContainer();
    }

    protected override bool OnEndDrag(bool commit)
    {
      return true;
    }

    protected void ReparentDraggedElementsIntoContainer()
    {
      HashSet<BaseFrameworkElement> hashSet = new HashSet<BaseFrameworkElement>();
      foreach (BaseFrameworkElement frameworkElement in this.DraggedElements)
      {
        if (frameworkElement.Parent != this.LayoutContainer)
        {
          if (frameworkElement.IsAttached)
            this.ActiveSceneViewModel.RemoveElement((SceneNode) frameworkElement);
          hashSet.Add(frameworkElement);
        }
      }
      this.Context.Transaction.UpdateEditTransaction();
      foreach (SceneNode sceneNode in hashSet)
        this.LayoutContainer.DefaultContent.Add(sceneNode);
      this.Context.Transaction.UpdateEditTransaction();
      this.AdjustLayoutAfterReparenting((ICollection<BaseFrameworkElement>) hashSet);
    }

    protected void MoveDraggedElementsInContainer()
    {
      Point point1 = this.DragCurrentPosition;
      if (this.IsConstraining)
        point1 = this.ConstrainPointToAxis(this.DragStartPosition, this.DragCurrentPosition);
      Point point2 = this.ActiveView.TransformPoint((IViewObject) this.ActiveView.HitTestRoot, this.LayoutContainer.Visual, point1);
      Point point3 = this.ActiveView.TransformPoint((IViewObject) this.ActiveView.HitTestRoot, this.LayoutContainer.Visual, this.DragStartPosition);
      for (int index = 0; index < this.DraggedElements.Count; ++index)
      {
        BaseFrameworkElement child = this.DraggedElements[index];
        if (child.IsViewObjectValid)
        {
          LayoutOverrides overridesToIgnore = LayoutOverrides.None;
          if (point1.X != this.DragStartPosition.X)
            overridesToIgnore |= LayoutOverrides.CenterHorizontalAlignment;
          if (point1.Y != this.DragStartPosition.Y)
            overridesToIgnore |= LayoutOverrides.CenterVerticalAlignment;
          if (this.Context.IsRecordingKeyframes)
          {
            Vector vector = point2 - point3;
            CanonicalTransform canonicalTransform = this.Context.BaseRenderTransforms[index];
            double num1 = RoundingHelper.RoundLength(vector.X + canonicalTransform.TranslationX);
            double num2 = RoundingHelper.RoundLength(vector.Y + canonicalTransform.TranslationY);
            if (!object.Equals((object) canonicalTransform.TranslationX, (object) num1))
              child.SetValue(child.Platform.Metadata.CommonProperties.RenderTransformTranslationX, (object) num1);
            if (!object.Equals((object) canonicalTransform.TranslationY, (object) num2))
              child.SetValue(child.Platform.Metadata.CommonProperties.RenderTransformTranslationY, (object) num2);
          }
          else
          {
            Point location = point2;
            ILayoutDesigner designerForChild = this.ActiveView.ViewModel.GetLayoutDesignerForChild((SceneElement) child, false);
            Rect childRect = designerForChild.GetChildRect(child);
            Point point4 = this.ActiveView.TransformPoint(child.Visual, this.LayoutContainer.Visual, this.Context.Offsets[index]);
            point4.Offset(-childRect.X, -childRect.Y);
            location.Offset(-point4.X, -point4.Y);
            Rect rect = new Rect(location, this.Context.LayoutCacheRecords[index].Rect.Size);
            designerForChild.SetChildRect(child, rect, this.Context.LayoutCacheRecords[index].Overrides, overridesToIgnore, LayoutOverrides.None);
          }
        }
      }
      this.Context.Transaction.UpdateEditTransaction();
    }
  }
}
