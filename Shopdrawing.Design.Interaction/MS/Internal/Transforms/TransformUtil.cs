// Decompiled with JetBrains decompiler
// Type: MS.Internal.Transforms.TransformUtil
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using MS.Internal;
using System;
using System.Windows;
using System.Windows.Media;

namespace MS.Internal.Transforms
{
  internal static class TransformUtil
  {
    public static CanonicalTransform GetCanonicalTransformToAncestor(DependencyObject childOrDescendant, Visual root)
    {
      if (root == null)
        throw new ArgumentNullException("root");
      if (childOrDescendant == null)
        throw new ArgumentNullException("childOrDescendant");
      return new CanonicalTransform(TransformUtil.GetTransformToAncestor(childOrDescendant, root));
    }

    internal static Transform GetTransformToAncestor(DependencyObject childOrDescendant, Visual ancestor)
    {
      if (ancestor == null)
        throw new ArgumentNullException("ancestor");
      if (childOrDescendant == null)
        throw new ArgumentNullException("childOrDescendant");
      Visual visual = childOrDescendant as Visual;
      if (visual == null)
        return Transform.Identity;
      return (ancestor.IsAncestorOf((DependencyObject) visual) ? visual.TransformToAncestor(ancestor) as Transform : (Transform) null) ?? Transform.Identity;
    }

    internal static Transform GetTransformToAncestor(ViewItem childOrDescendant, Visual ancestor)
    {
      if (ancestor == null)
        throw new ArgumentNullException("ancestor");
      if (childOrDescendant == (ViewItem) null)
        throw new ArgumentNullException("childOrDescendant");
      return (childOrDescendant.IsDescendantOf(ancestor) ? childOrDescendant.TransformToVisual(ancestor) as Transform : (Transform) null) ?? Transform.Identity;
    }

    internal static Transform GetTransformToAncestor(ViewItem childOrDescendant, ViewItem ancestor)
    {
      if (ancestor == (ViewItem) null)
        throw new ArgumentNullException("ancestor");
      if (childOrDescendant == (ViewItem) null)
        throw new ArgumentNullException("childOrDescendant");
      return (childOrDescendant.IsDescendantOf(ancestor) ? childOrDescendant.TransformToView(ancestor) as Transform : (Transform) null) ?? Transform.Identity;
    }

    internal static Transform GetParentTransformToAncestor(ViewItem item, Visual ancestor)
    {
      if (ancestor == null)
        throw new ArgumentNullException("ancestor");
      if (item == (ViewItem) null)
        throw new ArgumentNullException("item");
      ViewItem visualParent = item.VisualParent;
      Transform transform;
      if (visualParent == (ViewItem) null)
      {
        transform = item.TransformToVisual(ancestor) as Transform;
      }
      else
      {
        try
        {
          transform = visualParent.TransformToVisual(ancestor) as Transform;
        }
        catch
        {
          transform = (Transform) null;
        }
      }
      if (transform == null)
        transform = Transform.Identity;
      return transform;
    }

    public static Transform GetTransformToDescendant(Visual parentVisual, Visual toVisual)
    {
      if (parentVisual == null)
        throw new ArgumentNullException("parentVisual");
      if (toVisual == null)
        throw new ArgumentNullException("toVisual");
      return (parentVisual.IsAncestorOf((DependencyObject) toVisual) ? parentVisual.TransformToDescendant(toVisual) as Transform : (Transform) null) ?? Transform.Identity;
    }

    public static Transform GetTransformToImmediateParent(DependencyObject child)
    {
      if (child == null)
        throw new ArgumentNullException("child");
      Visual reference = child as Visual;
      if (reference == null)
        return Transform.Identity;
      return VisualTreeHelper.GetTransform(reference) ?? Transform.Identity;
    }

    public static Transform GetTransformToImmediateParent(ViewItem child)
    {
      if (child == (ViewItem) null)
        throw new ArgumentNullException("child");
      return child.Transform ?? Transform.Identity;
    }

    public static CanonicalTransform GetCanonicalTransformToImmediateParent(DependencyObject child)
    {
      if (child == null)
        throw new ArgumentNullException("child");
      return new CanonicalTransform(TransformUtil.GetTransformToImmediateParent(child));
    }

    public static Vector GetScaleFromTransform(Transform transform)
    {
      if (transform == null)
        throw new ArgumentNullException("transform");
      Matrix transform1 = transform.Value;
      if (transform1.M12 == 0.0 && transform1.M21 == 0.0)
        return new Vector(transform1.M11, transform1.M22);
      return new CanonicalTransform(transform1).Scale;
    }

    public static Vector GetScaleFromMatrix(Matrix transformMatrix)
    {
      if (transformMatrix.M12 == 0.0 && transformMatrix.M21 == 0.0)
        return new Vector(transformMatrix.M11, transformMatrix.M22);
      return new CanonicalTransform(transformMatrix).Scale;
    }

    internal static Transform GetTransformToChild(Visual root, DependencyObject childOrDescendant)
    {
      if (root == null)
        throw new ArgumentNullException("root");
      if (childOrDescendant == null)
        throw new ArgumentNullException("childOrDescendant");
      Visual descendant = childOrDescendant as Visual;
      if (descendant == null)
        return Transform.Identity;
      return (root.IsAncestorOf((DependencyObject) descendant) ? root.TransformToDescendant(descendant) as Transform : (Transform) null) ?? (Transform) new MatrixTransform(Matrix.Identity);
    }

    internal static Transform GetTransformToChild(Visual root, ViewItem childOrDescendant)
    {
      if (root == null)
        throw new ArgumentNullException("root");
      if (childOrDescendant == (ViewItem) null)
        throw new ArgumentNullException("childOrDescendant");
      return childOrDescendant.TransformFromVisual(root) as Transform ?? (Transform) new MatrixTransform(Matrix.Identity);
    }

    internal static Transform GetRenderSizeTransformToDesignerView(DependencyObject itemView)
    {
      if (itemView == null)
        throw new ArgumentNullException("itemView");
      Visual visual = itemView as Visual;
      if (visual == null)
        return Transform.Identity;
      DesignerView designerView = TransformUtil.GetDesignerView((DependencyObject) visual);
      if (designerView == null)
        return Transform.Identity;
      return TransformUtil.GetTransformToAncestor((DependencyObject) visual, (Visual) designerView);
    }

    internal static Transform GetRenderSizeTransformToDesignerView(ModelItem item)
    {
      if (item == null)
        throw new ArgumentNullException("item");
      DesignerView designerView = TransformUtil.GetDesignerView(item);
      if (designerView == null)
        return Transform.Identity;
      return TransformUtil.GetTransformToAncestor(item.View, (Visual) designerView);
    }

    internal static DesignerView GetDesignerView(ModelItem item)
    {
      EditingContext context = item.Context;
      DesignerView designerView = (DesignerView) null;
      if (context != null)
        designerView = DesignerView.FromContext(context);
      return designerView;
    }

    internal static DesignerView GetDesignerView(DependencyObject visual)
    {
      DesignerView designerView = DesignerView.GetDesignerView(visual);
      while (designerView == null && visual != null)
      {
        visual = VisualTreeHelper.GetParent(visual);
        if (visual != null)
          designerView = DesignerView.GetDesignerView(visual);
      }
      return designerView;
    }

    internal static Transform GetTransformFromDesignerView(DependencyObject visualObject)
    {
      if (visualObject == null)
        throw new ArgumentNullException("visualObject");
      Visual visual = visualObject as Visual;
      if (visual == null)
        return Transform.Identity;
      DesignerView designerView = TransformUtil.GetDesignerView((DependencyObject) visual);
      if (designerView == null)
        return Transform.Identity;
      return TransformUtil.GetTransformToChild((Visual) designerView, (DependencyObject) visual);
    }

    internal static Transform GetTransformFromDesignerView(ModelItem item)
    {
      if (item == null)
        throw new ArgumentNullException("item");
      DesignerView designerView = TransformUtil.GetDesignerView(item);
      if (designerView == null)
        return Transform.Identity;
      return TransformUtil.GetTransformToChild((Visual) designerView, item.View);
    }

    public static Transform SafeInvert(Transform transform)
    {
      return (Transform) new MatrixTransform(TransformUtil.SafeInvert(transform.Value));
    }

    public static Matrix SafeInvert(Matrix m)
    {
      if (!m.HasInverse)
      {
        Vector vector1 = new Vector(1.0, 0.0) * m;
        Vector vector2 = new Vector(0.0, 1.0) * m;
        bool flag = vector1.LengthSquared > vector2.LengthSquared;
        Vector vector3 = flag ? vector1 : vector2;
        if (vector3.LengthSquared < FloatingPointArithmetic.DistanceTolerance)
          vector3 = new Vector(1.0, 0.0);
        Vector vector4 = new Vector(-vector3.Y, vector3.X);
        vector4 /= vector4.Length;
        m = !flag ? new Matrix(vector4.X, vector4.Y, vector3.X, vector3.Y, m.OffsetX, m.OffsetY) : new Matrix(vector3.X, vector3.Y, vector4.X, vector4.Y, m.OffsetX, m.OffsetY);
      }
      m.Invert();
      return m;
    }

    public static Vector TranslateDesignerViewDelta(DependencyObject itemView, Vector delta)
    {
      if (itemView == null)
        throw new ArgumentNullException("itemView");
      Transform transform = TransformUtil.SafeInvert(TransformUtil.GetRenderSizeTransformToDesignerView(itemView));
      return (Vector) transform.Transform((Point) delta) - (Vector) transform.Transform(new Point());
    }

    public static Vector TranslateDesignerViewDelta(ModelItem item, Vector delta)
    {
      if (item == null)
        throw new ArgumentNullException("item");
      Transform transform = TransformUtil.SafeInvert(TransformUtil.GetRenderSizeTransformToDesignerView(item));
      return (Vector) transform.Transform((Point) delta) - (Vector) transform.Transform(new Point());
    }

    internal static CanonicalTransform GetCanonicalTransformToDesignerView(Visual visual)
    {
      if (visual == null)
        throw new ArgumentNullException("visual");
      DesignerView designerView = TransformUtil.GetDesignerView((DependencyObject) visual);
      if (designerView == null)
        return new CanonicalTransform(Transform.Identity);
      return new CanonicalTransform(TransformUtil.GetTransformToAncestor((DependencyObject) visual, (Visual) designerView));
    }

    internal static CanonicalTransform GetCanonicalTransformToDesignerView(EditingContext context, ViewItem view)
    {
      if (view == (ViewItem) null)
        throw new ArgumentNullException("view");
      DesignerView designerView = DesignerView.FromContext(context);
      if (designerView == null)
        return new CanonicalTransform(Transform.Identity);
      return new CanonicalTransform(TransformUtil.GetTransformToAncestor(view, (Visual) designerView));
    }

    internal static Transform GetSelectionFrameTransformToDesignerView(DependencyObject view)
    {
      DesignerView designerView = TransformUtil.GetDesignerView(view);
      if (designerView == null)
        return Transform.Identity;
      return TransformUtil.GetSelectionFrameTransformToParentVisual(view, (Visual) designerView);
    }

    internal static Transform GetSelectionFrameTransformToDesignerView(EditingContext context, ViewItem view)
    {
      DesignerView designerView = DesignerView.FromContext(context);
      if (designerView == null)
        return Transform.Identity;
      return TransformUtil.GetSelectionFrameTransformToParentVisual(view, (Visual) designerView);
    }

    internal static Transform GetSelectionFrameTransformToParentVisual(DependencyObject view, Visual ancestorView)
    {
      FrameworkElement frameworkElement = view as FrameworkElement;
      if (frameworkElement != null && (frameworkElement.LayoutTransform == null || frameworkElement.LayoutTransform == Transform.Identity))
      {
        Transform toImmediateParent = TransformUtil.GetTransformToImmediateParent(view);
        DependencyObject parent = VisualTreeHelper.GetParent(view);
        Rect selectionFrameBounds = ElementUtilities.GetSelectionFrameBounds(view);
        Vector vector = new Vector(selectionFrameBounds.X, selectionFrameBounds.Y);
        Matrix matrix = toImmediateParent.Value;
        matrix.Translate(vector.X, vector.Y);
        if (parent != ancestorView)
        {
          Transform transformToAncestor = TransformUtil.GetTransformToAncestor(parent, ancestorView);
          return (Transform) new MatrixTransform(matrix * transformToAncestor.Value);
        }
      }
      return TransformUtil.GetTransformToAncestor(view, ancestorView);
    }

    internal static Transform GetSelectionFrameTransformToParentView(ViewItem view, ViewItem ancestorView)
    {
      if (view.LayoutTransform == null || view.LayoutTransform == Transform.Identity)
      {
        Transform toImmediateParent = TransformUtil.GetTransformToImmediateParent(view);
        ViewItem visualParent = view.VisualParent;
        if (visualParent != (ViewItem) null)
        {
          Rect selectionFrameBounds = view.SelectionFrameBounds;
          Vector vector = new Vector(selectionFrameBounds.X, selectionFrameBounds.Y);
          Matrix matrix = toImmediateParent.Value;
          matrix.Translate(vector.X, vector.Y);
          if (!(ancestorView != visualParent))
            return (Transform) new MatrixTransform(matrix);
          Transform transformToAncestor = TransformUtil.GetTransformToAncestor(visualParent, ancestorView);
          return (Transform) new MatrixTransform(matrix * transformToAncestor.Value);
        }
      }
      return TransformUtil.GetTransformToAncestor(view, ancestorView);
    }

    internal static Transform GetSelectionFrameTransformToParentVisual(ViewItem view, Visual ancestorView)
    {
      if (view != (ViewItem) null && (view.LayoutTransform == null || view.LayoutTransform == Transform.Identity))
      {
        Transform toImmediateParent = TransformUtil.GetTransformToImmediateParent(view);
        Rect selectionFrameBounds = view.SelectionFrameBounds;
        Vector vector = new Vector(selectionFrameBounds.X, selectionFrameBounds.Y);
        Matrix matrix = toImmediateParent.Value;
        matrix.Translate(vector.X, vector.Y);
        if ((view.VisualParent == (ViewItem) null ? (object) DesignerView.GetDesignerView((DependencyObject) ancestorView) : view.VisualParent.PlatformObject) != ancestorView)
        {
          Transform transformToAncestor = TransformUtil.GetParentTransformToAncestor(view, ancestorView);
          return (Transform) new MatrixTransform(matrix * transformToAncestor.Value);
        }
      }
      return TransformUtil.GetTransformToAncestor(view, ancestorView);
    }

    internal static bool IsNotRotateSkew(Transform transform)
    {
      if (transform == null || transform == Transform.Identity)
        return true;
      Matrix matrix = transform.Value;
      return matrix.M12 == 0.0 && matrix.M21 == 0.0;
    }
  }
}
