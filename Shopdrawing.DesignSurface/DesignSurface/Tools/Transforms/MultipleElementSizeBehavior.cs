// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.MultipleElementSizeBehavior
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Designers;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal class MultipleElementSizeBehavior : MultipleElementScaleBehavior
  {
    private Dictionary<SceneElement, bool> startWidthSet = new Dictionary<SceneElement, bool>();
    private Dictionary<SceneElement, bool> startHeightSet = new Dictionary<SceneElement, bool>();
    private Dictionary<SceneElement, LayoutOverrides> initialOverrides = new Dictionary<SceneElement, LayoutOverrides>();
    private Dictionary<SceneElement, Rect> initialRects = new Dictionary<SceneElement, Rect>();
    private Dictionary<SceneElement, CanonicalDecomposition> initialTransforms = new Dictionary<SceneElement, CanonicalDecomposition>();
    private Dictionary<SceneElement, PathElement.PathTransformHelper> pathTransformHelpers = new Dictionary<SceneElement, PathElement.PathTransformHelper>();

    public override string ActionString
    {
      get
      {
        return StringTable.UndoUnitResize;
      }
    }

    public MultipleElementSizeBehavior(ToolBehaviorContext toolContext)
      : base(toolContext)
    {
    }

    protected override void Initialize()
    {
      base.Initialize();
      ILayoutDesigner designerForChild = this.ActiveSceneViewModel.GetLayoutDesignerForChild(this.EditingElementSet.PrimaryElement, true);
      this.pathTransformHelpers.Clear();
      foreach (SceneElement element in this.EditingElementSet.Elements)
      {
        this.startWidthSet[element] = element.IsSet(BaseFrameworkElement.WidthProperty) == PropertyState.Set;
        this.startHeightSet[element] = element.IsSet(BaseFrameworkElement.HeightProperty) == PropertyState.Set;
        this.initialOverrides[element] = designerForChild.ComputeOverrides((BaseFrameworkElement) element);
        this.initialRects[element] = designerForChild.GetChildRect((BaseFrameworkElement) element);
        this.initialTransforms[element] = new CanonicalDecomposition(this.StartTransformsDictionary[element]);
        PathElement.PathTransformHelper transformHelper = PathElement.TryCreateTransformHelper(element, this.StartBoundsDictionary[element].Size);
        if (transformHelper != null)
          this.pathTransformHelpers[element] = transformHelper;
      }
    }

    private Dictionary<SceneElement, Size> CalculateClampedSizes(ref Vector scale)
    {
      Dictionary<SceneElement, Size> dictionary = new Dictionary<SceneElement, Size>();
      Vector vector = new Vector(Math.Abs(scale.X), Math.Abs(scale.Y));
      bool flag1 = false;
      bool flag2 = false;
      for (int index1 = 0; index1 < this.EditingElementSet.Elements.Count; ++index1)
      {
        SceneElement index2 = this.EditingElementSet.Elements[index1];
        Rect rect = this.StartBoundsDictionary[index2];
        Size size = new Size();
        bool flag3 = Math.Abs(rect.Width) < 1E-06;
        bool flag4 = Math.Abs(rect.Height) < 1E-06;
        size.Width = !flag3 || !this.ActiveAdorner.TestFlags(EdgeFlags.LeftOrRight) ? rect.Width * vector.X : Math.Abs(this.CurrentPointerPosition.X - this.StartPointerPosition.X);
        size.Height = !flag4 || !this.ActiveAdorner.TestFlags(EdgeFlags.TopOrBottom) ? rect.Height * vector.Y : Math.Abs(this.CurrentPointerPosition.Y - this.StartPointerPosition.Y);
        double num = index2 is ShapeElement ? (double) index2.GetComputedValue(ShapeElement.StrokeThicknessProperty) : 0.0;
        if (rect.Width < num || rect.Height < num)
        {
          bool flag5 = false;
          if (!flag1 && rect.Height < num)
          {
            scale.X = 1.0;
            vector.X = 1.0;
            flag1 = true;
            flag5 = true;
            size.Height = rect.Height;
          }
          if (!flag2 && rect.Width < num)
          {
            scale.Y = 1.0;
            vector.Y = 1.0;
            flag2 = true;
            flag5 = true;
            size.Width = rect.Width;
          }
          if (flag5)
          {
            index1 = -1;
            continue;
          }
        }
        else if (size.Width < num || size.Height < num)
        {
          if (!flag1 && size.Width < num)
          {
            scale.X = Math.Min(1.0, (num + 0.1) / rect.Width);
            vector.X = Math.Abs(scale.X);
          }
          if (!flag2 && size.Height < num)
          {
            scale.Y = Math.Min(1.0, (num + 0.1) / rect.Height);
            vector.Y = Math.Abs(scale.Y);
          }
          index1 = -1;
          continue;
        }
        dictionary[index2] = size;
      }
      return dictionary;
    }

    protected override void ApplyScale(Vector scale, Point center)
    {
      Dictionary<SceneElement, Size> dictionary = this.CalculateClampedSizes(ref scale);
      Vector vector1 = new Vector(scale.X < 0.0 ? -1.0 : 1.0, scale.Y < 0.0 ? -1.0 : 1.0);
      Matrix startSharedTransform = this.StartSharedTransform;
      Matrix inverseMatrix = ElementUtilities.GetInverseMatrix(startSharedTransform);
      ILayoutDesigner designerForChild = this.ActiveSceneViewModel.GetLayoutDesignerForChild(this.EditingElementSet.PrimaryElement, true);
      foreach (SceneElement index in this.EditingElementSet.Elements)
      {
        double width = dictionary[index].Width;
        double height = dictionary[index].Height;
        Rect rect1 = this.StartBoundsDictionary[index];
        Point renderTransformOrigin = ((Base2DElement) index).RenderTransformOrigin;
        Point point1 = new Point(rect1.X + rect1.Width * renderTransformOrigin.X, rect1.Y + rect1.Height * renderTransformOrigin.Y);
        Matrix matrix1 = this.ElementToElementsTransformDictionary[index];
        Matrix matrix2 = new Matrix();
        matrix2.Translate(-center.X, -center.Y);
        matrix1 *= matrix2;
        Point point2 = matrix1.Transform(point1);
        Vector vector2 = new Point(point2.X * scale.X, point2.Y * scale.Y) - point2;
        Vector vector3 = new Vector((width - rect1.Width) * renderTransformOrigin.X, (height - rect1.Height) * renderTransformOrigin.Y);
        Vector vector4 = vector2 * startSharedTransform - vector3;
        Matrix matrix3 = this.StartTransformsDictionary[index];
        Matrix m = new Matrix();
        m.Scale(scale.X, scale.Y);
        Matrix matrix4 = ElementUtilities.GetInverseMatrix(m) * matrix3 * inverseMatrix;
        Vector vector5 = new Vector(matrix3.OffsetX, matrix3.OffsetY) * inverseMatrix;
        matrix4.ScaleAt(scale.X, scale.Y, vector5.X, vector5.Y);
        CanonicalDecomposition newTransform = new CanonicalDecomposition(matrix4 * startSharedTransform);
        newTransform.ScaleX *= vector1.X;
        newTransform.ScaleY *= vector1.Y;
        newTransform.Translation += vector4;
        AdornedToolBehavior.UpdateElementTransform(index, newTransform, AdornedToolBehavior.TransformPropertyFlags.Scale | AdornedToolBehavior.TransformPropertyFlags.Skew | AdornedToolBehavior.TransformPropertyFlags.RotatationAngle);
        Rect rect2 = this.initialRects[index];
        rect2.Offset(newTransform.TranslationX - this.initialTransforms[index].TranslationX, newTransform.TranslationY - this.initialTransforms[index].TranslationY);
        rect2.Width = width;
        rect2.Height = height;
        BaseFrameworkElement child = (BaseFrameworkElement) index;
        LayoutOverrides overridesToIgnore = (LayoutOverrides) (0 | (!object.Equals((object) width, (object) rect1.Width) ? 16 : 0) | (!object.Equals((object) height, (object) rect1.Height) ? 32 : 0));
        designerForChild.SetChildRect(child, rect2, this.initialOverrides[index], overridesToIgnore, LayoutOverrides.None);
        if (!object.Equals((object) width, (object) rect1.Width) || !object.Equals((object) height, (object) rect1.Height))
          PathElement.EnsureStretchIsFill((SceneNode) index);
        PathElement.PathTransformHelper pathTransformHelper;
        if (this.pathTransformHelpers.TryGetValue(index, out pathTransformHelper))
          pathTransformHelper.Update(scale.X * vector1.X, scale.Y * vector1.Y);
      }
    }

    protected override bool OnDragEnd(Point dragStartPosition, Point dragEndPosition)
    {
      foreach (KeyValuePair<SceneElement, PathElement.PathTransformHelper> keyValuePair in this.pathTransformHelpers)
        keyValuePair.Value.OnDragEnd();
      this.pathTransformHelpers.Clear();
      return base.OnDragEnd(dragStartPosition, dragEndPosition);
    }
  }
}
