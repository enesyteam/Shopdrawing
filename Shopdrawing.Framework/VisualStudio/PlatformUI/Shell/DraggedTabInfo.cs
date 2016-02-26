// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.DraggedTabInfo
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.PlatformUI.Shell.Controls;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Microsoft.VisualStudio.PlatformUI.Shell
{
  internal class DraggedTabInfo
  {
    private List<Rect> tabRects = new List<Rect>();
    private int draggedTabPosition = -1;
    private SplitterLength groupDockedWidth = new SplitterLength();
    private SplitterLength groupDockedHeight = new SplitterLength();
    private int groupPosition = -1;
    public const int InvalidTabPosition = -1;
    private Rect tabStripRect;
    private ViewElement sibling;
    private ViewElement nestedGroup;
    private Rect virtualTabRect;
    private double groupFloatingWidth;
    private double groupFloatingHeight;
    private DocumentGroupContainer groupContainer;

    public ViewElement DraggedViewElement { get; set; }

    public Rect TabStripRect
    {
      get
      {
        return this.tabStripRect;
      }
    }

    public List<Rect> TabRects
    {
      get
      {
        return this.tabRects;
      }
    }

    public ViewElement Sibling
    {
      get
      {
        return this.sibling;
      }
      set
      {
        this.sibling = value;
      }
    }

    public ViewElement NestedGroup
    {
      get
      {
        return this.nestedGroup;
      }
      set
      {
        this.nestedGroup = value;
      }
    }

    public ViewElement TargetElement
    {
      get
      {
        return this.nestedGroup ?? this.sibling;
      }
    }

    public int DraggedTabPosition
    {
      get
      {
        return this.draggedTabPosition;
      }
      set
      {
        this.draggedTabPosition = value;
      }
    }

    public Rect VirtualTabRect
    {
      get
      {
        return this.virtualTabRect;
      }
    }

    public SplitterLength GroupDockedWidth
    {
      get
      {
        return this.groupDockedWidth;
      }
    }

    public SplitterLength GroupDockedHeight
    {
      get
      {
        return this.groupDockedHeight;
      }
    }

    public double GroupFloatingWidth
    {
      get
      {
        return this.groupFloatingWidth;
      }
    }

    public double GroupFloatingHeight
    {
      get
      {
        return this.groupFloatingHeight;
      }
    }

    public int GroupPosition
    {
      get
      {
        return this.groupPosition;
      }
    }

    public DocumentGroupContainer GroupContainer
    {
      get
      {
        return this.groupContainer;
      }
    }

    public ReorderTabPanel TabStrip { get; set; }

    static DraggedTabInfo()
    {
      EventManager.RegisterClassHandler(typeof (ReorderTabPanel), ReorderTabPanel.PanelLayoutUpdatedEvent, (Delegate) new RoutedEventHandler(DraggedTabInfo.OnPanelLayoutUpdated));
    }

    private static void OnPanelLayoutUpdated(object sender, RoutedEventArgs args)
    {
      DraggedTabInfo draggedTabInfo = DockManager.Instance.DraggedTabInfo;
      if (draggedTabInfo == null || draggedTabInfo.TabStrip != sender)
        return;
      draggedTabInfo.MeasureTabStrip();
    }

    public void Initialize(ViewElement view)
    {
      this.sibling = DraggedTabInfo.FindFirstSibling(view);
      ViewGroup parent = view.Parent;
      this.nestedGroup = (ViewElement) null;
      this.groupPosition = -1;
      this.groupFloatingHeight = 0.0;
      this.groupFloatingWidth = 0.0;
      this.groupContainer = (DocumentGroupContainer) null;
      if (parent == null)
        return;
      if (parent.Parent != null)
      {
        this.groupPosition = parent.Parent.Children.IndexOf((ViewElement) parent);
        if (parent != null && parent.Children.Count == 1 && parent.Parent.Children.Count == 1)
          this.nestedGroup = (ViewElement) parent;
      }
      this.groupDockedWidth = parent.DockedWidth;
      this.groupDockedHeight = parent.DockedHeight;
      this.groupFloatingWidth = parent.FloatingWidth;
      this.groupFloatingHeight = parent.FloatingHeight;
      this.groupContainer = parent.Parent as DocumentGroupContainer;
    }

    public Rect GetTabRectAt(Point screenPoint)
    {
      Rect rect1 = new Rect(0.0, 0.0, 0.0, 0.0);
      foreach (Rect rect2 in this.tabRects)
      {
        if (rect2.Contains(screenPoint))
        {
          rect1 = rect2;
          break;
        }
      }
      return rect1;
    }

    public int GetTabIndexAt(Point screenPoint)
    {
      int num = -1;
      for (int index = 0; index < this.tabRects.Count; ++index)
      {
        if (this.tabRects[index].Contains(screenPoint))
        {
          num = index;
          break;
        }
      }
      return num;
    }

    public Rect GetClosestTabRectAt(Point screenPoint)
    {
      Rect rect1 = this.GetTabRectAt(screenPoint);
      if (rect1.Width == 0.0)
      {
        double num1 = double.MaxValue;
        foreach (Rect rect2 in this.tabRects)
        {
          double num2 = DraggedTabInfo.AverageDistance(rect2, screenPoint);
          if (num2 < num1)
          {
            num1 = num2;
            rect1 = rect2;
          }
        }
      }
      return rect1;
    }

    public int GetClosestTabIndexAt(Point screenPoint)
    {
      int num1 = this.GetTabIndexAt(screenPoint);
      if (-1 == num1)
      {
        double num2 = double.MaxValue;
        for (int index = 0; index < this.tabRects.Count; ++index)
        {
          double num3 = DraggedTabInfo.AverageDistance(this.tabRects[index], screenPoint);
          if (num3 < num2)
          {
            num2 = num3;
            num1 = index;
          }
        }
      }
      return num1;
    }

    private static double AverageDistance(Rect rect, Point point)
    {
      return (DraggedTabInfo.DistanceSquare(rect.TopLeft, point) + DraggedTabInfo.DistanceSquare(rect.TopRight, point) + DraggedTabInfo.DistanceSquare(rect.BottomLeft, point) + DraggedTabInfo.DistanceSquare(rect.BottomRight, point)) / 4.0;
    }

    private static double DistanceSquare(Point point1, Point point2)
    {
      return Math.Pow(point1.X - point2.X, 2.0) + Math.Pow(point1.Y - point2.Y, 2.0);
    }

    public void MoveTabRect(int from, int to)
    {
      if (from < 0 || from >= this.tabRects.Count)
        throw new ArgumentOutOfRangeException(string.Concat(new object[4]
        {
          (object) "from: ",
          (object) from,
          (object) " tabRects.count: ",
          (object) this.tabRects.Count
        }));
      if (to < 0 || to >= this.tabRects.Count)
        throw new ArgumentOutOfRangeException(string.Concat(new object[4]
        {
          (object) "to: ",
          (object) to,
          (object) " tabRects.count: ",
          (object) this.tabRects.Count
        }));
      double x = this.tabRects[0].X;
      Rect rect = this.tabRects[from];
      this.tabRects.Remove(rect);
      this.tabRects.Insert(to, rect);
      this.RearrangeTabCoordinates(x);
    }

    public void RemoveTabRect(int position)
    {
      if (position < 0 || position >= this.tabRects.Count)
        throw new ArgumentOutOfRangeException(string.Concat(new object[4]
        {
          (object) "position: ",
          (object) position,
          (object) " tabRects.Count: ",
          (object) this.tabRects.Count
        }));
      double x = this.tabRects[0].X;
      this.tabRects.RemoveAt(position);
      if (this.tabRects.Count <= 0)
        return;
      this.RearrangeTabCoordinates(x);
    }

    public void SetVirtualTabRect(int position)
    {
      if (position < 0 || position >= this.tabRects.Count)
        throw new ArgumentOutOfRangeException(string.Concat(new object[4]
        {
          (object) "position: ",
          (object) position,
          (object) " tabRects.count: ",
          (object) this.tabRects.Count
        }));
      this.virtualTabRect = this.tabRects[position];
    }

    public void ClearVirtualTabRect()
    {
      this.virtualTabRect.X = 0.0;
      this.virtualTabRect.Y = 0.0;
      this.virtualTabRect.Width = 0.0;
      this.virtualTabRect.Height = 0.0;
    }

    public Rect MeasureTabStrip()
    {
      if (this.TabStrip == null)
        throw new InvalidOperationException("TabStrip must be initialized.");
      this.draggedTabPosition = -1;
      Point topLeft = new Point(double.MaxValue, double.MaxValue);
      Point bottomRight = new Point(double.MinValue, double.MinValue);
      this.tabRects.Clear();
      foreach (UIElement uiElement in this.TabStrip.Children)
      {
        TabItem tabChild = uiElement as TabItem;
        if (tabChild != null)
        {
          this.MeasureTabItem(tabChild, ref topLeft, ref bottomRight);
          ViewElement viewElement = tabChild.DataContext as ViewElement;
          if (viewElement != null && viewElement == this.DraggedViewElement)
            this.draggedTabPosition = this.tabRects.Count - 1;
        }
      }
      if (topLeft.X == double.MaxValue || topLeft.Y == double.MaxValue || (bottomRight.X == double.MinValue || bottomRight.Y == double.MinValue))
      {
        topLeft.X = 0.0;
        topLeft.Y = 0.0;
        bottomRight.X = 0.0;
        bottomRight.Y = 0.0;
        this.tabRects.Clear();
      }
      this.tabStripRect = new Rect(topLeft, bottomRight);
      DockTarget ancestor = Microsoft.VisualStudio.PlatformUI.ExtensionMethods.FindAncestor<DockTarget>((Visual) this.TabStrip);
      if (ancestor != null && ancestor.DockTargetType == DockTargetType.Auto && ExtensionMethods.IsConnectedToPresentationSource((DependencyObject) ancestor))
        this.tabStripRect = new Rect(ancestor.PointToScreen(new Point(0.0, 0.0)), DpiHelper.LogicalToDeviceUnits(new Size(ancestor.ActualWidth, ancestor.ActualHeight)));
      this.NormalizeTabHeight();
      return this.tabStripRect;
    }

    private void MeasureTabItem(TabItem tabChild, ref Point topLeft, ref Point bottomRight)
    {
      if (tabChild.Visibility != Visibility.Visible)
        return;
      Point location = tabChild.PointToScreen(new Point(0.0, 0.0));
      Size size = DpiHelper.LogicalToDeviceUnits(new Size(tabChild.ActualWidth, tabChild.ActualHeight));
      Rect rect = new Rect(location, size);
      if (location.X < topLeft.X)
        topLeft.X = location.X;
      if (location.Y < topLeft.Y)
        topLeft.Y = location.Y;
      if (location.X + size.Width > bottomRight.X)
        bottomRight.X = location.X + size.Width;
      if (location.Y + size.Height > bottomRight.Y)
        bottomRight.Y = location.Y + size.Height;
      int index = 0;
      while (index < this.tabRects.Count && rect.X >= this.tabRects[index].X)
        ++index;
      this.tabRects.Insert(index, rect);
    }

    private void RearrangeTabCoordinates(double originalX)
    {
      if (this.tabRects.Count == 0)
        throw new InvalidOperationException("tabRects is empty.");
      Rect rect1 = this.tabRects[0];
      rect1.X = originalX;
      this.tabRects[0] = rect1;
      for (int index = 1; index < this.tabRects.Count; ++index)
      {
        Rect rect2 = this.tabRects[index];
        rect2.X = this.tabRects[index - 1].X + Math.Round(this.tabRects[index - 1].Width, 0) + 1.0;
        this.tabRects[index] = rect2;
      }
    }

    private void NormalizeTabHeight()
    {
      for (int index = 0; index < this.tabRects.Count; ++index)
      {
        Rect rect = this.tabRects[index];
        rect.Height = this.tabStripRect.Height;
        rect.Y = this.tabStripRect.Y;
        this.tabRects[index] = rect;
      }
    }

    private static ViewElement FindFirstSibling(ViewElement element)
    {
      ViewElement viewElement1 = (ViewElement) null;
      if (element.Parent != null)
      {
        foreach (ViewElement viewElement2 in (IEnumerable<ViewElement>) element.Parent.Children)
        {
          if (viewElement2 != element && !(viewElement2 is ViewBookmark))
          {
            viewElement1 = viewElement2;
            break;
          }
        }
      }
      return viewElement1;
    }
  }
}
