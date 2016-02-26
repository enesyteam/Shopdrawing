// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.SizeBehavior
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Designers;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal class SizeBehavior : BaseSizeBehavior
  {
    private PathElement.PathTransformHelper transformHelper;
    private Rect initialRect;
    private LayoutOverrides initialOverrides;

    public override string ActionString
    {
      get
      {
        return StringTable.UndoUnitResize;
      }
    }

    protected override bool AllowScaleAroundCenter
    {
      get
      {
        return true;
      }
    }

    public SizeBehavior(ToolBehaviorContext toolContext)
      : base(toolContext)
    {
    }

    protected override void Initialize()
    {
      base.Initialize();
      ILayoutDesigner designerForChild = this.ActiveSceneViewModel.GetLayoutDesignerForChild(this.EditingElement, true);
      this.initialRect = designerForChild.GetChildRect(this.BaseEditingElement);
      this.initialOverrides = designerForChild.ComputeOverrides(this.BaseEditingElement);
      this.transformHelper = PathElement.TryCreateTransformHelper(this.EditingElement, this.StartSize);
    }

    protected override Size ComputeSizeFromScaledDimensions(double scaledWidth, double scaledHeight)
    {
      Size size = new Size(Math.Abs(scaledWidth), Math.Abs(scaledHeight));
      double val2 = 0.0;
      if (this.EditingElement is ShapeElement)
        val2 = Math.Abs((double) this.EditingElement.GetComputedValue(ShapeElement.StrokeThicknessProperty));
      size.Width = Math.Max(Math.Min(this.StartSize.Width, val2), size.Width);
      size.Height = Math.Max(Math.Min(this.StartSize.Height, val2), size.Height);
      return size;
    }

    protected override void ApplyScale(Vector scale, Point center)
    {
      Vector scale1 = new Vector(Math.Abs(scale.X), Math.Abs(scale.Y));
      Vector scale2 = new Vector(scale.X < 0.0 ? -1.0 : 1.0, scale.Y < 0.0 ? -1.0 : 1.0);
      Vector vector1 = new Vector(this.StartCenter.X, this.StartCenter.Y);
      Size newSize = this.ComputeNewSize(scale1);
      Point topLeft = this.StartBounds.TopLeft;
      CanonicalTransform canonicalTransform = new CanonicalTransform(this.StartTransform);
      canonicalTransform.ApplyScale(scale, this.StartCenter, center);
      Point point1 = (topLeft - vector1) * canonicalTransform.TransformGroup.Value + vector1;
      Point renderTransformOrigin = this.BaseEditingElement.RenderTransformOrigin;
      renderTransformOrigin.X *= newSize.Width;
      renderTransformOrigin.Y *= newSize.Height;
      CanonicalTransform newTransform = new CanonicalTransform(this.StartTransform);
      newTransform.ApplyScale(scale2, renderTransformOrigin, center);
      Vector vector2 = (Vector) renderTransformOrigin;
      Point point2 = (topLeft - vector2) * newTransform.TransformGroup.Value + vector2;
      newTransform.Translation += point1 - point2;
      AdornedToolBehavior.UpdateElementTransform(this.EditingElement, newTransform, AdornedToolBehavior.TransformPropertyFlags.Scale);
      ILayoutDesigner designerForChild = this.ActiveSceneViewModel.GetLayoutDesignerForChild(this.EditingElement, true);
      Rect rect = this.initialRect;
      rect.Offset(newTransform.TranslationX - this.StartTransform.TranslationX, newTransform.TranslationY - this.StartTransform.TranslationY);
      rect.Width = newSize.Width;
      rect.Height = newSize.Height;
      LayoutOverrides overridesToIgnore = (LayoutOverrides) (0 | (!object.Equals((object) rect.Width, (object) this.StartSize.Width) ? 16 : 0) | (!object.Equals((object) rect.Height, (object) this.StartSize.Height) ? 32 : 0));
      designerForChild.SetChildRect(this.BaseEditingElement, rect, this.initialOverrides, overridesToIgnore, LayoutOverrides.None);
      if (!object.Equals((object) rect.Width, (object) this.StartSize.Width) || !object.Equals((object) rect.Height, (object) this.StartSize.Height))
        PathElement.EnsureStretchIsFill((SceneNode) this.EditingElement);
      if (this.transformHelper == null)
        return;
      this.transformHelper.Update(scale1.X, scale1.Y);
    }

    protected override bool OnDragEnd(Point dragStartPosition, Point dragEndPosition)
    {
      if (this.transformHelper != null)
      {
        this.transformHelper.OnDragEnd();
        this.transformHelper = (PathElement.PathTransformHelper) null;
      }
      return base.OnDragEnd(dragStartPosition, dragEndPosition);
    }
  }
}
