// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ElementUtilities
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.InstanceBuilders;
using Microsoft.Expression.DesignSurface.Geometry.Core;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface
{
  public static class ElementUtilities
  {
    public static IEnumerable GetElementTree(FrameworkElement rootElement)
    {
      return (IEnumerable) new ElementUtilities.DepthFirstFrameworkElementCollection(rootElement);
    }

    public static IEnumerable GetVisualTree(Visual rootVisual)
    {
      return (IEnumerable) new ElementUtilities.DepthFirstVisualCollection(rootVisual);
    }

    public static FrameworkElement FindElement(FrameworkElement element, string id)
    {
      return LogicalTreeHelper.FindLogicalNode((DependencyObject) element, id) as FrameworkElement;
    }

    public static FrameworkElement FindElementInVisualTree(Visual root, string id)
    {
      FrameworkElement frameworkElement = root as FrameworkElement;
      if (frameworkElement != null && frameworkElement.Name == id)
        return frameworkElement;
      for (int childIndex = 0; childIndex < VisualTreeHelper.GetChildrenCount((DependencyObject) root); ++childIndex)
      {
        Visual root1 = VisualTreeHelper.GetChild((DependencyObject) root, childIndex) as Visual;
        if (root1 != null)
        {
          FrameworkElement elementInVisualTree = ElementUtilities.FindElementInVisualTree(root1, id);
          if (elementInVisualTree != null)
            return elementInVisualTree;
        }
      }
      return (FrameworkElement) null;
    }

    public static bool HasVisualTreeAncestorOfType(DependencyObject element, Type type)
    {
      return ElementUtilities.GetVisualTreeAncestorOfType(element, type) != null;
    }

    public static DependencyObject GetVisualTreeAncestorOfType(DependencyObject element, Type type)
    {
      if (element is Visual || element is Visual3D)
      {
        for (; element != null; element = VisualTreeHelper.GetParent(element))
        {
          if (type.IsInstanceOfType((object) element))
            return element;
        }
      }
      return (DependencyObject) null;
    }

    public static bool IsDescendant(DependencyObject parent, DependencyObject descendant)
    {
      if (parent == null)
        return false;
      for (; descendant != null; descendant = LogicalTreeHelper.GetParent(descendant))
      {
        if (descendant == parent)
          return true;
      }
      return false;
    }

    public static DependencyObject GetLeastCommonAncestor(DependencyObject firstDescendant, DependencyObject secondDescendant, DependencyObject root)
    {
      DependencyObject current1 = firstDescendant;
      ArrayList arrayList1 = new ArrayList();
      for (; current1 != null && LogicalTreeHelper.GetParent(current1) != null; current1 = LogicalTreeHelper.GetParent(current1))
        arrayList1.Add((object) current1);
      DependencyObject current2 = secondDescendant;
      ArrayList arrayList2 = new ArrayList();
      for (; current2 != null && LogicalTreeHelper.GetParent(current2) != null; current2 = LogicalTreeHelper.GetParent(current2))
        arrayList2.Add((object) current2);
      DependencyObject dependencyObject = root;
      while (arrayList1.Count > 0 && arrayList2.Count > 0 && arrayList1[arrayList1.Count - 1] == arrayList2[arrayList2.Count - 1])
      {
        dependencyObject = (DependencyObject) arrayList1[arrayList1.Count - 1];
        arrayList1.RemoveAt(arrayList1.Count - 1);
        arrayList2.RemoveAt(arrayList2.Count - 1);
      }
      return dependencyObject;
    }

    public static FrameworkElement GetLogicalElementForVisual(DependencyObject dependencyObject, IInstanceDictionary dictionary)
    {
      if (dictionary == null)
        throw new ArgumentNullException("dictionary");
      for (; dependencyObject != null; dependencyObject = VisualTreeHelper.GetParent(dependencyObject))
      {
        FrameworkElement frameworkElement = dependencyObject as FrameworkElement;
        if (frameworkElement != null && dictionary.GetViewNode((object) frameworkElement, false) != null)
          return frameworkElement;
      }
      return (FrameworkElement) null;
    }

    public static Rect GetActualBounds(FrameworkElement element)
    {
      return ElementUtilities.GetActualBoundsCore(element, false);
    }

    public static Rect GetActualBoundsInParent(FrameworkElement element)
    {
      return ElementUtilities.GetActualBoundsCore(element, true);
    }

    private static Rect GetActualBoundsCore(FrameworkElement element, bool inParent)
    {
      if (element == null)
        return new Rect();
      Rect layoutSlot = LayoutInformation.GetLayoutSlot(element);
      HorizontalAlignment horizontalAlignment = element.HorizontalAlignment;
      VerticalAlignment verticalAlignment = element.VerticalAlignment;
      Thickness margin = element.Margin;
      Point point1 = new Point();
      Point point2 = new Point();
      double width1 = element.RenderSize.Width;
      double height1 = element.RenderSize.Height;
      double width2 = element.Width;
      double height2 = element.Height;
      double minWidth = element.MinWidth;
      double minHeight = element.MinHeight;
      double num1 = !double.IsNaN(width2) ? Math.Max(width2, minWidth) : (horizontalAlignment == HorizontalAlignment.Stretch ? Math.Max(minWidth, layoutSlot.Width - margin.Left - margin.Right) : width1);
      double num2 = !double.IsNaN(height2) ? Math.Max(height2, minHeight) : (verticalAlignment == VerticalAlignment.Stretch ? Math.Max(minHeight, layoutSlot.Height - margin.Top - margin.Bottom) : height1);
      double num3 = Math.Min(width1, num1);
      double num4 = Math.Min(height1, num2);
      switch (horizontalAlignment)
      {
        case HorizontalAlignment.Left:
          point1.X = layoutSlot.Left + margin.Left;
          point2.X = layoutSlot.Left + margin.Left;
          break;
        case HorizontalAlignment.Center:
          point1.X = (layoutSlot.Left + margin.Left + layoutSlot.Right - margin.Right) / 2.0 - num1 / 2.0;
          point2.X = (layoutSlot.Left + margin.Left + layoutSlot.Right - margin.Right) / 2.0 - num1 / 2.0;
          break;
        case HorizontalAlignment.Right:
          point1.X = layoutSlot.Right - margin.Right - num3;
          point2.X = layoutSlot.Right - margin.Right - num1;
          break;
        case HorizontalAlignment.Stretch:
          point1.X = Math.Max(layoutSlot.Left + margin.Left, (layoutSlot.Left + margin.Left + layoutSlot.Right - margin.Right) / 2.0 - num3 / 2.0);
          point2.X = Math.Max(layoutSlot.Left + margin.Left, (layoutSlot.Left + margin.Left + layoutSlot.Right - margin.Right) / 2.0 - num1 / 2.0);
          break;
      }
      switch (verticalAlignment)
      {
        case VerticalAlignment.Top:
          point1.Y = layoutSlot.Top + margin.Top;
          point2.Y = layoutSlot.Top + margin.Top;
          break;
        case VerticalAlignment.Center:
          point1.Y = (layoutSlot.Top + margin.Top + layoutSlot.Bottom - margin.Bottom) / 2.0 - num2 / 2.0;
          point2.Y = (layoutSlot.Top + margin.Top + layoutSlot.Bottom - margin.Bottom) / 2.0 - num2 / 2.0;
          break;
        case VerticalAlignment.Bottom:
          point1.Y = layoutSlot.Bottom - margin.Bottom - num4;
          point2.Y = layoutSlot.Bottom - margin.Bottom - num2;
          break;
        case VerticalAlignment.Stretch:
          point1.Y = Math.Max(layoutSlot.Top + margin.Top, (layoutSlot.Top + margin.Top + layoutSlot.Bottom - margin.Bottom) / 2.0 - num4 / 2.0);
          point2.Y = Math.Max(layoutSlot.Top + margin.Top, (layoutSlot.Top + margin.Top + layoutSlot.Bottom - margin.Bottom) / 2.0 - num2 / 2.0);
          break;
      }
      if (inParent)
        return new Rect(point2.X, point2.Y, num1, num2);
      return new Rect(point2.X - point1.X, point2.Y - point1.Y, num1, num2);
    }

    public static Matrix GetComputedTransform(Visual targetVisual, Visual visual)
    {
      if (targetVisual == null || visual == null || (Visual) visual.FindCommonVisualAncestor((DependencyObject) targetVisual) == null)
        return Matrix.Identity;
      return VectorUtilities.GetMatrixFromTransform(visual.TransformToVisual(targetVisual));
    }

    public static Matrix GetInverseMatrix(Matrix m)
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

    public static Vector GetCorrespondingVector(Vector vector, Matrix matrix)
    {
      return ElementUtilities.GetCorrespondingVector(vector, matrix, (AxisConstraint) null);
    }

    public static Vector GetCorrespondingVector(Vector vector, Matrix matrix, AxisConstraint axisConstraint)
    {
      if (matrix.HasInverse)
      {
        if (axisConstraint != null)
          vector = axisConstraint.GetConstrainedVector(vector);
        Matrix matrix1 = matrix;
        matrix1.Invert();
        return vector * matrix1;
      }
      Vector vector1 = matrix.Transform(new Vector(1.0, 0.0));
      double length1 = vector1.Length;
      Vector vector2 = matrix.Transform(new Vector(0.0, 1.0));
      double length2 = vector2.Length;
      if (FloatingPointArithmetic.IsVerySmall(length1) && FloatingPointArithmetic.IsVerySmall(length2))
        return new Vector(0.0, 0.0);
      Vector vector3 = length1 > length2 ? vector1 : vector2;
      vector3.Normalize();
      double num1 = vector * vector3;
      double num2 = length1 * length1 + length2 * length2;
      return new Vector(length1 * num1 / num2, length2 * num1 / num2);
    }

    internal static void SortElementsByDepth(List<SceneElement> elements)
    {
      elements.Sort((IComparer<SceneElement>) new ElementUtilities.DepthComparer());
    }

    private class DepthComparer : IComparer<SceneElement>
    {
      public int Compare(SceneElement first, SceneElement second)
      {
        return this.GetDepth((SceneNode) second) - this.GetDepth((SceneNode) first);
      }

      private int GetDepth(SceneNode node)
      {
        int num = 0;
        for (; node.Parent != null; node = node.Parent)
          ++num;
        return num;
      }
    }

    private sealed class DepthFirstFrameworkElementCollection : IEnumerable
    {
      private FrameworkElement root;

      public DepthFirstFrameworkElementCollection(FrameworkElement root)
      {
        this.root = root;
      }

      public IEnumerator GetEnumerator()
      {
        return (IEnumerator) new ElementUtilities.DepthFirstFrameworkElementEnumerator(this.root);
      }
    }

    private sealed class DepthFirstFrameworkElementEnumerator : IEnumerator
    {
      private bool isBeforeFirstElement = true;
      private Stack stack = new Stack();
      private FrameworkElement root;

      public FrameworkElement Current
      {
        get
        {
          IEnumerator enumerator = (IEnumerator) this.stack.Peek();
          if (enumerator == null)
            throw new InvalidOperationException(ExceptionStringTable.ElementUtilitiesEnumeratorOutOfRangeError);
          return (FrameworkElement) enumerator.Current;
        }
      }

      object IEnumerator.Current
      {
        get
        {
          return (object) this.Current;
        }
      }

      public DepthFirstFrameworkElementEnumerator(FrameworkElement root)
      {
        if (root == null)
          throw new ArgumentNullException("root");
        this.root = root;
      }

      public void Reset()
      {
        this.isBeforeFirstElement = true;
        this.stack.Clear();
      }

      public bool MoveNext()
      {
        bool flag = false;
        if (this.isBeforeFirstElement)
        {
          this.isBeforeFirstElement = false;
          IEnumerator enumerator = new ArrayList(1)
          {
            (object) this.root
          }.GetEnumerator();
          enumerator.MoveNext();
          this.stack.Push((object) enumerator);
          flag = true;
        }
        else
        {
          IEnumerator enumerator1 = (IEnumerator) this.stack.Peek();
          while (enumerator1 != null)
          {
            DependencyObject current = enumerator1.Current as DependencyObject;
            IEnumerator enumerator2 = current != null ? LogicalTreeHelper.GetChildren(current).GetEnumerator() : (IEnumerator) null;
            if (enumerator2 != null && enumerator2.MoveNext())
            {
              this.stack.Push((object) enumerator2);
              enumerator1 = enumerator2;
            }
            else
            {
              for (; enumerator1 != null && !enumerator1.MoveNext(); enumerator1 = this.stack.Count > 0 ? (IEnumerator) this.stack.Peek() : (IEnumerator) null)
                this.stack.Pop();
            }
            if (enumerator1 != null && enumerator1.Current is FrameworkElement)
            {
              flag = true;
              break;
            }
          }
        }
        return flag;
      }
    }

    private sealed class DepthFirstVisualCollection : IEnumerable
    {
      private Visual root;

      public DepthFirstVisualCollection(Visual root)
      {
        this.root = root;
      }

      public IEnumerator GetEnumerator()
      {
        return (IEnumerator) new ElementUtilities.DepthFirstVisualEnumerator(this.root);
      }
    }

    private sealed class DepthFirstVisualEnumerator : IEnumerator
    {
      private Stack stack = new Stack();
      private Visual root;

      public Visual Current
      {
        get
        {
          Visual visual = (Visual) null;
          IEnumerator enumerator = (IEnumerator) this.stack.Peek();
          if (enumerator != null)
            visual = (Visual) enumerator.Current;
          return visual;
        }
      }

      object IEnumerator.Current
      {
        get
        {
          return (object) this.Current;
        }
      }

      public DepthFirstVisualEnumerator(Visual root)
      {
        if (root == null)
          throw new ArgumentNullException("root");
        this.root = root;
      }

      public void Reset()
      {
        this.stack.Clear();
      }

      public bool MoveNext()
      {
        bool flag = false;
        if (this.stack.Count < 1)
        {
          IEnumerator enumerator = new ArrayList(1)
          {
            (object) this.root
          }.GetEnumerator();
          flag = enumerator.MoveNext();
          this.stack.Push((object) enumerator);
        }
        else
        {
          IEnumerator enumerator1 = (IEnumerator) this.stack.Peek();
          Visual visual1 = enumerator1.Current as Visual;
          int childrenCount = VisualTreeHelper.GetChildrenCount((DependencyObject) visual1);
          if (childrenCount > 0)
          {
            IList<Visual> list = (IList<Visual>) new List<Visual>(childrenCount);
            for (int childIndex = 0; childIndex < childrenCount; ++childIndex)
            {
              Visual visual2 = VisualTreeHelper.GetChild((DependencyObject) visual1, childIndex) as Visual;
              if (visual2 != null)
                list.Add(visual2);
            }
            IEnumerator enumerator2 = (IEnumerator) list.GetEnumerator();
            flag = enumerator2.MoveNext();
            this.stack.Push((object) enumerator2);
          }
          else if (enumerator1.MoveNext())
          {
            flag = true;
          }
          else
          {
            for (; this.stack.Count > 1 && !flag; flag = ((IEnumerator) this.stack.Peek()).MoveNext())
              this.stack.Pop();
          }
        }
        return flag;
      }
    }
  }
}
