// Decompiled with JetBrains decompiler
// Type: MS.Internal.ElementUtilities
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design.Interaction;
using MS.Internal.Transforms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace MS.Internal
{
  internal static class ElementUtilities
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

    public static bool HasAncestorOfType(FrameworkElement rootElement, FrameworkElement element, Type type)
    {
      return ElementUtilities.GetAncestorOfType(rootElement, element, type) != null;
    }

    internal static FrameworkElement GetAncestorOfType(FrameworkElement rootElement, FrameworkElement element, Type type)
    {
      if (type.IsInstanceOfType((object) element))
        return element;
      if (rootElement == element)
        return (FrameworkElement) null;
      FrameworkElement frameworkElement1 = (FrameworkElement) null;
      FrameworkElement frameworkElement2 = rootElement != null ? (FrameworkElement) rootElement.Parent : (FrameworkElement) null;
      for (element = element.Parent as FrameworkElement; element != null && element != frameworkElement2; element = element.Parent as FrameworkElement)
      {
        if (type.IsInstanceOfType((object) element))
        {
          frameworkElement1 = element;
          break;
        }
      }
      return frameworkElement1;
    }

    public static bool HasAncestorOfTypeInVisualTree(ViewItem rootElement, ViewItem element, Type type)
    {
      return ElementUtilities.GetAncestorOfTypeInVisualTree(rootElement, element, type) != (ViewItem) null;
    }

    internal static ViewItem GetAncestorOfTypeInVisualTree(ViewItem rootElement, ViewItem element, Type type)
    {
      if (type.IsAssignableFrom(element.ItemType))
        return element;
      if (rootElement == element)
        return (ViewItem) null;
      ViewItem viewItem1 = (ViewItem) null;
      ViewItem viewItem2 = rootElement != (ViewItem) null ? rootElement.LogicalParent : (ViewItem) null;
      for (element = element.LogicalParent; element != (ViewItem) null && element != viewItem2; element = element.VisualParent)
      {
        if (type.IsAssignableFrom(element.ItemType))
        {
          viewItem1 = element;
          break;
        }
      }
      return viewItem1;
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

    public static bool IsDescendant(ViewItem parent, ViewItem descendant)
    {
      if (parent == (ViewItem) null)
        return false;
      for (; descendant != (ViewItem) null; descendant = descendant.LogicalParent)
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

    public static Rect GetTransformedBounds(Visual visual, Visual ancestor)
    {
      FrameworkElement element = visual as FrameworkElement;
      Rect rect = new Rect();
      if (element != null)
      {
        rect = ElementUtilities.GetActualBoundsCore(element, false);
      }
      else
      {
        UIElement uiElement = visual as UIElement;
        if (uiElement != null)
          rect.Size = uiElement.RenderSize;
        else
          rect = VisualTreeHelper.GetContentBounds(visual);
      }
      return TransformUtil.GetTransformToAncestor((DependencyObject) visual, ancestor).TransformBounds(rect);
    }

    public static Rect GetLayoutRect(DependencyObject element)
    {
      FrameworkElement element1 = element as FrameworkElement;
      if (element1 != null)
        return LayoutInformation.GetLayoutSlot(element1);
      Rect renderSizeBounds = ElementUtilities.GetRenderSizeBounds(element);
      renderSizeBounds.Location = new Point();
      return renderSizeBounds;
    }

    public static Rect GetElementRelativeSelectionFrameBounds(DependencyObject element)
    {
      FrameworkElement element1 = element as FrameworkElement;
      if (element1 != null)
        return ElementUtilities.GetActualBoundsCore(element1, false);
      Rect renderSizeBounds = ElementUtilities.GetRenderSizeBounds(element);
      renderSizeBounds.Location = new Point();
      return renderSizeBounds;
    }

    public static Rect GetActualBounds(FrameworkElement element)
    {
      return ElementUtilities.GetActualBoundsCore(element, false);
    }

    private static Rect GetActualBoundsInParent(FrameworkElement element)
    {
      return ElementUtilities.GetActualBoundsCore(element, true);
    }

    private static Rect GetActualBoundsCore(FrameworkElement element, bool inParent)
    {
      Rect layoutSlot = LayoutInformation.GetLayoutSlot(element);
      HorizontalAlignment horizontalAlignment = element.HorizontalAlignment;
      VerticalAlignment verticalAlignment = element.VerticalAlignment;
      Thickness margin = element.Margin;
      Point point1 = new Point();
      Point point2 = new Point();
      double val1_1 = element.RenderSize.Width;
      double val1_2 = element.RenderSize.Height;
      double num1 = element.Width;
      double num2 = element.Height;
      if (double.IsNaN(num1))
        num1 = horizontalAlignment == HorizontalAlignment.Stretch ? Math.Max(0.0, layoutSlot.Width - margin.Left - margin.Right) : val1_1;
      if (double.IsNaN(num2))
        num2 = verticalAlignment == VerticalAlignment.Stretch ? Math.Max(0.0, layoutSlot.Height - margin.Top - margin.Bottom) : val1_2;
      if (!double.IsNaN(element.MinWidth))
      {
        val1_1 = Math.Max(val1_1, element.MinWidth);
        num1 = Math.Max(num1, element.MinWidth);
      }
      if (!double.IsNaN(element.MaxWidth))
      {
        val1_1 = Math.Min(val1_1, element.MaxWidth);
        num1 = Math.Min(num1, element.MaxWidth);
      }
      if (!double.IsNaN(element.MinHeight))
      {
        val1_2 = Math.Max(val1_2, element.MinHeight);
        num2 = Math.Max(num2, element.MinHeight);
      }
      if (!double.IsNaN(element.MaxHeight))
      {
        val1_2 = Math.Min(val1_2, element.MaxHeight);
        num2 = Math.Min(num2, element.MaxHeight);
      }
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
          point1.X = layoutSlot.Right - margin.Right - val1_1;
          point2.X = layoutSlot.Right - margin.Right - num1;
          break;
        case HorizontalAlignment.Stretch:
          point1.X = Math.Max(layoutSlot.Left + margin.Left, (layoutSlot.Left + margin.Left + layoutSlot.Right - margin.Right) / 2.0 - val1_1 / 2.0);
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
          point1.Y = layoutSlot.Bottom - margin.Bottom - val1_2;
          point2.Y = layoutSlot.Bottom - margin.Bottom - num2;
          break;
        case VerticalAlignment.Stretch:
          point1.Y = Math.Max(layoutSlot.Top + margin.Top, (layoutSlot.Top + margin.Top + layoutSlot.Bottom - margin.Bottom) / 2.0 - val1_2 / 2.0);
          point2.Y = Math.Max(layoutSlot.Top + margin.Top, (layoutSlot.Top + margin.Top + layoutSlot.Bottom - margin.Bottom) / 2.0 - num2 / 2.0);
          break;
      }
      if (inParent)
        return new Rect(point2.X, point2.Y, num1, num2);
      return new Rect(point2.X - point1.X, point2.Y - point1.Y, num1, num2);
    }

    public static Rect GetRenderSizeBounds(DependencyObject element)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      Rect rect = new Rect();
      Visual visual = element as Visual;
      if (visual == null)
        return rect;
      rect.Location = (Point) VisualTreeHelper.GetOffset(visual);
      UIElement uiElement = visual as UIElement;
      if (uiElement == null)
      {
        Size size = VisualTreeHelper.GetContentBounds(visual).Size;
        if (size.Width <= 0.0 && size.Height <= 0.0)
        {
          Visual parentVisual = VisualTreeHelper.GetParent((DependencyObject) visual) as Visual;
          if (parentVisual != null)
            size = TransformUtil.GetTransformToDescendant(parentVisual, visual).TransformBounds(ElementUtilities.GetSelectionFrameBounds((DependencyObject) parentVisual)).Size;
        }
        rect.Size = size;
      }
      else
        rect.Size = uiElement.RenderSize;
      return rect;
    }

    public static Rect GetRenderSizeBounds(ViewItem view)
    {
      return view.RenderSizeBounds;
    }

    public static Rect GetSelectionFrameBounds(DependencyObject element)
    {
      FrameworkElement element1 = element as FrameworkElement;
      if (element1 != null && (element1.LayoutTransform == null || element1.LayoutTransform == Transform.Identity))
        return ElementUtilities.GetActualBoundsInParent(element1);
      return ElementUtilities.GetRenderSizeBounds(element);
    }

    public static Rect GetSelectionFrameBounds(ViewItem view)
    {
      return view.SelectionFrameBounds;
    }

    public static Vector ComputePositionDeltaInTarget(DesignerView dview, ViewItem target, Point startPosition, Point currentPosition)
    {
      GeneralTransform generalTransform = target.TransformFromVisual((Visual) dview);
      Point point = generalTransform.Transform(startPosition);
      return generalTransform.Transform(currentPosition) - point;
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
            throw new InvalidOperationException("ExceptionStringTable.ElementUtilitiesEnumeratorOutOfRangeError");
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
