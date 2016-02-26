// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.Controls.DockManager
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.PlatformUI.Shell;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
  public class DockManager
  {
    public static readonly RoutedEvent FloatingElementDockedEvent = EventManager.RegisterRoutedEvent("FloatingElementDocked", RoutingStrategy.Direct, typeof (EventHandler<FloatingElementDockedEventArgs>), typeof (DockManager));
    private Dictionary<Visual, DockManager.DockSite> visualToSite = new Dictionary<Visual, DockManager.DockSite>();
    private Dictionary<IntPtr, DockManager.DockSite> hwndToSite = new Dictionary<IntPtr, DockManager.DockSite>();
    private List<DependencyObject> currentlyDragWithin = new List<DependencyObject>();
    private static DockManager instance;
    private List<ViewElement> draggedViewElements;
    private IDockPreviewWindow dockPreviewWindow;

    public DockManager.UndockingScope UndockingInformation { get; private set; }

    internal DraggedTabInfo DraggedTabInfo { get; set; }

    internal bool IsDragging { get; set; }

    internal List<ViewElement> DraggedViewElements
    {
      get
      {
        if (this.draggedViewElements == null)
          this.draggedViewElements = new List<ViewElement>();
        return this.draggedViewElements;
      }
    }

    internal bool IsFloatingOverDockAdorner
    {
      get
      {
        bool flag = false;
        foreach (DependencyObject dependencyObject in this.currentlyDragWithin)
        {
          DockAdornerWindow dockAdornerWindow = dependencyObject as DockAdornerWindow;
          if (dockAdornerWindow != null && dockAdornerWindow.IsVisible)
          {
            flag = true;
            break;
          }
        }
        return flag;
      }
    }

    private IDockPreviewWindow DockPreviewWindow
    {
      get
      {
        if (this.dockPreviewWindow == null)
          this.dockPreviewWindow = this.CreateDockPreviewWindow();
        return this.dockPreviewWindow;
      }
    }

    public static DockManager Instance
    {
      get
      {
        return DockManager.instance ?? (DockManager.instance = new DockManager());
      }
      internal set
      {
        DockManager.instance = value;
      }
    }

    public DockManager.UndockingScope CreateUndockingScope(ViewElement element, Point undockingPoint)
    {
      if (this.UndockingInformation != null)
        throw new InvalidOperationException("Only one undocking operation can happen at a time.");
      return new DockManager.UndockingScope(element, undockingPoint);
    }

    public void PerformDrop(DragAbsoluteEventArgs args)
    {
      DragUndockHeader dragUndockHeader = args.OriginalSource as DragUndockHeader;
      FloatingWindow floatingWindow = Microsoft.VisualStudio.PlatformUI.ExtensionMethods.FindAncestor<FloatingWindow>((Visual) dragUndockHeader);
      DockManager.DockSiteHitTestResult hitElement = this.FindHitElement(args.ScreenPoint, (Predicate<DockManager.DockSite>) (s => s.Visual != floatingWindow));
      if (hitElement != null)
      {
        DockSiteAdorner ancestorOrSelf1 = Microsoft.VisualStudio.PlatformUI.ExtensionMethods.FindAncestorOrSelf<DockSiteAdorner>(hitElement.VisualHit);
        DockAdornerWindow ancestorOrSelf2 = Microsoft.VisualStudio.PlatformUI.ExtensionMethods.FindAncestorOrSelf<DockAdornerWindow>(hitElement.VisualHit);
        DockTarget dockTarget = Microsoft.VisualStudio.PlatformUI.ExtensionMethods.FindAncestorOrSelf<DockTarget>(hitElement.VisualHit);
        DockDirection dockDirection = DockDirection.Fill;
        bool flag = false;
        bool createDocumentGroup = false;
        if (floatingWindow != null && this.IsValidFillPreviewOperation(dockTarget, dragUndockHeader.ViewElement))
        {
          dockDirection = DockDirection.Fill;
          flag = true;
        }
        if (ancestorOrSelf1 != null && ancestorOrSelf2 != null && ancestorOrSelf2.AdornedElement != null)
        {
          dockDirection = ancestorOrSelf1.DockDirection;
          dockTarget = ancestorOrSelf2.AdornedElement as DockTarget;
          if (DockOperations.AreDockRestrictionsFulfilled(dragUndockHeader.ViewElement, dockTarget.TargetElement))
          {
            flag = true;
            createDocumentGroup = ancestorOrSelf1.CreatesDocumentGroup;
          }
        }
        if (flag)
        {
          PerformanceUtility.MeasurePerformanceUntilRender(PerformanceEvent.DockPalette);
          dockTarget.RaiseEvent((RoutedEventArgs) new FloatingElementDockedEventArgs(DockManager.FloatingElementDockedEvent, dragUndockHeader.ViewElement, dockDirection, createDocumentGroup));
        }
      }
      this.ClearAdorners();
    }

    public void UpdateTargets(DragAbsoluteEventArgs args)
    {
      FloatingWindow ancestor = Microsoft.VisualStudio.PlatformUI.ExtensionMethods.FindAncestor<FloatingWindow>((Visual) args.OriginalSource);
      this.UpdateDockPreview(args, (FloatingElement) ancestor);
      this.UpdateAdorners(args, ancestor);
      this.UpdateIsFloatingWindowDragWithin(args, ancestor);
    }

    internal virtual DraggedTabInfo GetAutodockTarget(DragAbsoluteEventArgs args)
    {
      DraggedTabInfo draggedTabInfo = (DraggedTabInfo) null;
      FloatingElement floatingElement = Microsoft.VisualStudio.PlatformUI.ExtensionMethods.FindAncestor<FloatingElement>((Visual) (args.OriginalSource as DragUndockHeader));
      if (this.DraggedTabInfo != null && this.DraggedTabInfo.TabStripRect.Contains(args.ScreenPoint))
      {
        draggedTabInfo = this.DraggedTabInfo;
      }
      else
      {
        DockManager.DockSiteHitTestResult hitElement = this.FindHitElement(args.ScreenPoint, (Predicate<DockManager.DockSite>) (s =>
        {
          if (s.Visual != floatingElement)
            return !(s.Visual is DockAdornerWindow);
          return false;
        }));
        if (hitElement != null)
        {
          ReorderTabPanel reorderTabPanel = (ReorderTabPanel) null;
          for (DependencyObject sourceElement = (DependencyObject) hitElement.VisualHit; sourceElement != null && reorderTabPanel == null; sourceElement = Microsoft.VisualStudio.PlatformUI.ExtensionMethods.GetVisualOrLogicalParent(sourceElement))
          {
            DockTarget dockTarget = sourceElement as DockTarget;
            if (dockTarget != null && dockTarget.DockTargetType == DockTargetType.Auto && dockTarget.Visibility == Visibility.Visible)
              reorderTabPanel = Microsoft.VisualStudio.PlatformUI.ExtensionMethods.FindDescendant<ReorderTabPanel>((DependencyObject) dockTarget);
          }
          if (reorderTabPanel != null)
          {
            draggedTabInfo = new DraggedTabInfo();
            draggedTabInfo.TabStrip = reorderTabPanel;
            draggedTabInfo.MeasureTabStrip();
            ViewGroup viewGroup = reorderTabPanel.DataContext as ViewGroup;
            if (viewGroup == null)
              throw new InvalidOperationException("Reorder tab panel should always have ViewGroup as its DataContext.");
            draggedTabInfo.NestedGroup = (ViewElement) viewGroup;
            if (viewGroup.VisibleChildren.Count > 0)
              draggedTabInfo.Sibling = viewGroup.VisibleChildren[0];
          }
        }
      }
      return draggedTabInfo;
    }

    public void ClearAdorners()
    {
      this.ClearAdorners((Predicate<Visual>) (s => true));
      this.CloseDockPreview();
    }

    internal void SetDraggedViewElements(ViewElement dragged)
    {
      if (dragged == null)
        throw new ArgumentNullException("dragged");
      this.DraggedViewElements.Clear();
      ViewGroup viewGroup = dragged as ViewGroup;
      if (viewGroup == null)
      {
        this.DraggedViewElements.Add(dragged);
      }
      else
      {
        foreach (ViewElement viewElement in (IEnumerable<ViewElement>) viewGroup.Children)
          this.DraggedViewElements.Add(viewElement);
      }
    }

    private void UpdateIsFloatingWindowDragWithin(DragAbsoluteEventArgs args, FloatingWindow floatingWindow)
    {
      FloatingWindow.SetIsFloatingWindowDragWithin((IEnumerable<DependencyObject>) this.currentlyDragWithin, false);
      DockManager.DockSiteHitTestResult hitElement = this.FindHitElement(args.ScreenPoint, (Predicate<DockManager.DockSite>) (s =>
      {
        if (s.Visual != floatingWindow)
          return s.Visual is DockAdornerWindow;
        return false;
      }));
      if (hitElement != null)
      {
        this.currentlyDragWithin = new List<DependencyObject>(this.GetHierarchy((DependencyObject) hitElement.VisualHit, (DependencyObject) null));
        FloatingWindow.SetIsFloatingWindowDragWithin((IEnumerable<DependencyObject>) this.currentlyDragWithin, true);
      }
      else
        this.currentlyDragWithin.Clear();
    }

    protected virtual IDockPreviewWindow CreateDockPreviewWindow()
    {
      return (IDockPreviewWindow) new Microsoft.VisualStudio.PlatformUI.Shell.Controls.DockPreviewWindow();
    }

    private void HideDockPreview()
    {
      if (this.dockPreviewWindow == null)
        return;
      this.dockPreviewWindow.Hide();
    }

    private void CloseDockPreview()
    {
      if (this.dockPreviewWindow != null)
        this.dockPreviewWindow.Close();
      this.dockPreviewWindow = (IDockPreviewWindow) null;
    }

    protected virtual void UpdateAdorners(DragAbsoluteEventArgs args, FloatingElement floatingElement, IList<DockSiteType> types, ViewElement element)
    {
      DockManager.DockSiteHitTestResult hitElement = this.FindHitElement(args.ScreenPoint, (Predicate<DockManager.DockSite>) (s =>
      {
        if (s.Visual != floatingElement)
          return !(s.Visual is DockAdornerWindow);
        return false;
      }));
      ICollection<DockAdornerWindow> addedAdorners = (ICollection<DockAdornerWindow>) new List<DockAdornerWindow>();
      PerformanceUtility.MeasurePerformanceUntilRender(PerformanceEvent.DisplayDockingAdorner);
      if (hitElement != null)
        addedAdorners = this.AddAdorners(hitElement.DockSite, hitElement.VisualHit, types, element);
      this.ClearAdorners((Predicate<Visual>) (visual => !addedAdorners.Contains(visual as DockAdornerWindow)));
    }

    private void UpdateAdorners(DragAbsoluteEventArgs args, FloatingWindow floatingWindow)
    {
      List<DockSiteType> list = new List<DockSiteType>();
      list.Add(DockSiteType.Default);
      list.Add(DockSiteType.NonDraggable);
      FloatSite floatSite = floatingWindow.DataContext as FloatSite;
      if (floatSite == null)
        return;
      ViewElement child = floatSite.Child;
      if (child == null)
        return;
      this.UpdateAdorners(args, (FloatingElement) floatingWindow, (IList<DockSiteType>) list, child);
    }

    private void UpdateDockPreview(DragAbsoluteEventArgs args, FloatingElement floatingElement)
    {
      DockManager.DockSiteHitTestResult hitElement = this.FindHitElement(args.ScreenPoint, (Predicate<DockManager.DockSite>) (s => s.Visual != floatingElement));
      if (hitElement != null)
      {
        DockSiteAdorner ancestorOrSelf1 = Microsoft.VisualStudio.PlatformUI.ExtensionMethods.FindAncestorOrSelf<DockSiteAdorner>(hitElement.VisualHit);
        DockAdornerWindow ancestorOrSelf2 = Microsoft.VisualStudio.PlatformUI.ExtensionMethods.FindAncestorOrSelf<DockAdornerWindow>(hitElement.VisualHit);
        DockTarget ancestorOrSelf3 = Microsoft.VisualStudio.PlatformUI.ExtensionMethods.FindAncestorOrSelf<DockTarget>(hitElement.VisualHit);
        FloatSite floatSite = floatingElement.Content as FloatSite;
        DockDirection dockDirection = DockDirection.Fill;
        FrameworkElement adornedElement = (FrameworkElement) null;
        if (floatSite == null)
          throw new InvalidOperationException("Dragging element must be a FloatSite");
        if (floatSite.Child == null)
          throw new InvalidOperationException("floatSite must have at least one child.");
        ViewElement child = floatSite.Child;
        if (this.IsValidFillPreviewOperation(ancestorOrSelf3, child))
        {
          dockDirection = DockDirection.Fill;
          adornedElement = ancestorOrSelf3.AdornmentTarget == null ? (FrameworkElement) ancestorOrSelf3 : ancestorOrSelf3.AdornmentTarget;
        }
        if (ancestorOrSelf1 != null && ancestorOrSelf2 != null && ancestorOrSelf2.AdornedElement != null)
        {
          dockDirection = ancestorOrSelf1.DockDirection;
          adornedElement = ancestorOrSelf2.AdornedElement;
          if (!ancestorOrSelf1.CreatesDocumentGroup && dockDirection != DockDirection.Fill && adornedElement.DataContext is DocumentGroup)
            adornedElement = (FrameworkElement) Microsoft.VisualStudio.PlatformUI.ExtensionMethods.FindAncestor<DocumentGroupContainerControl>((Visual) adornedElement);
        }
        if (adornedElement != null)
        {
          Rect dockPreviewRect = this.GetDockPreviewRect(dockDirection, adornedElement, child);
          this.DockPreviewWindow.Left = dockPreviewRect.Left;
          this.DockPreviewWindow.Top = dockPreviewRect.Top;
          this.DockPreviewWindow.Width = dockPreviewRect.Width;
          this.DockPreviewWindow.Height = dockPreviewRect.Height;
          this.OnDockPreviewWindowShowing(this.DockPreviewWindow, dockDirection);
          this.DockPreviewWindow.Show();
        }
        else
          this.HideDockPreview();
      }
      else
        this.HideDockPreview();
    }

    protected virtual void OnDockPreviewWindowShowing(IDockPreviewWindow dockPreviewWindow, DockDirection dockDirection)
    {
    }

    protected virtual bool IsValidFillPreviewOperation(DockTarget dockTarget, ViewElement dockingView)
    {
      bool flag = false;
      if (dockTarget != null && dockTarget.DockTargetType == DockTargetType.FillPreview && (DockOperations.AreDockRestrictionsFulfilled(dockingView, dockTarget.TargetElement) && dockTarget.TargetElement.AreDockTargetsEnabled))
        flag = true;
      return flag;
    }

    protected void ClearAdorners(Predicate<Visual> includeAdorner)
    {
      foreach (DockAdornerWindow dockAdornerWindow in Enumerable.Select<DockManager.DockSite, Visual>(Enumerable.Where<DockManager.DockSite>((IEnumerable<DockManager.DockSite>) new List<DockManager.DockSite>((IEnumerable<DockManager.DockSite>) this.visualToSite.Values), (Func<DockManager.DockSite, bool>) (s =>
      {
        if (s.Visual is DockAdornerWindow)
          return includeAdorner(s.Visual);
        return false;
      })), (Func<DockManager.DockSite, Visual>) (s => s.Visual)))
        dockAdornerWindow.PrepareAndHide();
    }

    private Rect GetDockPreviewRect(DockDirection dockDirection, FrameworkElement adornedElement, ViewElement element)
    {
      Rect rect1 = new Rect();
      Orientation orientation = Orientation.Horizontal;
      ViewElement viewElement = adornedElement.DataContext as ViewElement;
      SplitterLength itemLength = new SplitterLength();
      if (dockDirection == DockDirection.FirstValue || dockDirection == DockDirection.Bottom)
      {
        orientation = Orientation.Vertical;
        itemLength = element.DockedHeight;
      }
      else if (dockDirection == DockDirection.Left || dockDirection == DockDirection.Right)
      {
        orientation = Orientation.Horizontal;
        itemLength = element.DockedWidth;
      }
      Rect rect2;
      if (dockDirection != DockDirection.Fill)
      {
        SplitterPanel panel = (SplitterPanel) null;
        int targetIndex = -1;
        this.GetPreviewSplitterPanel(out panel, out targetIndex, dockDirection, viewElement, adornedElement, orientation);
        rect2 = panel == null || orientation != panel.Orientation ? this.PreviewDockCounterOrientation(dockDirection, adornedElement, viewElement, itemLength, orientation) : this.PreviewDockSameOrientation(dockDirection, panel, viewElement, itemLength, orientation, targetIndex);
      }
      else
        rect2 = this.PreviewDockFill(adornedElement);
      return rect2;
    }

    private void GetPreviewSplitterPanel(out SplitterPanel panel, out int targetIndex, DockDirection dockDirection, ViewElement viewElement, FrameworkElement adornedElement, Orientation orientation)
    {
      targetIndex = -1;
      panel = Microsoft.VisualStudio.PlatformUI.ExtensionMethods.FindAncestor<SplitterPanel>((Visual) adornedElement);
      if (panel != null)
      {
        SplitterItem ancestor = Microsoft.VisualStudio.PlatformUI.ExtensionMethods.FindAncestor<SplitterItem>((Visual) adornedElement);
        targetIndex = SplitterPanel.GetIndex((UIElement) ancestor);
      }
      MainSite mainSite = viewElement as MainSite;
      if (mainSite == null)
        return;
      DockGroup dockGroup = mainSite.Child as DockGroup;
      if (dockGroup == null || dockGroup.Orientation != orientation)
        return;
      panel = (SplitterPanel) null;
      DependencyObject reference = (DependencyObject) adornedElement;
      while (panel == null && reference != null)
      {
        reference = VisualTreeHelper.GetChild(reference, 0);
        panel = reference as SplitterPanel;
      }
      if (panel == null)
        return;
      if (dockDirection == DockDirection.Left || dockDirection == DockDirection.FirstValue)
        targetIndex = 0;
      else
        targetIndex = panel.Children.Count - 1;
    }

    private Rect PreviewDockSameOrientation(DockDirection dockDirection, SplitterPanel panel, ViewElement viewElement, SplitterLength itemLength, Orientation orientation, int originalIndex)
    {
      List<UIElement> list = new List<UIElement>();
      Point point1 = new Point(0.0, 0.0);
      SplitterItem splitterItem1 = new SplitterItem();
      Size availableSize = new Size();
      availableSize.Width = panel.ActualWidth;
      availableSize.Height = panel.ActualHeight;
      SplitterItem splitterItem2 = (SplitterItem) null;
      foreach (SplitterItem splitterItem3 in panel.Children)
      {
        list.Add((UIElement) splitterItem3);
        if (splitterItem3.Content == viewElement)
        {
          splitterItem2 = splitterItem3;
          panel.Children.IndexOf((UIElement) splitterItem2);
        }
      }
      int index = splitterItem2 != null ? list.IndexOf((UIElement) splitterItem2) : originalIndex;
      if (dockDirection == DockDirection.Right || dockDirection == DockDirection.Bottom)
        ++index;
      list.Insert(index, (UIElement) splitterItem1);
      SplitterPanel.SetSplitterLength((UIElement) splitterItem1, itemLength);
      Point point2 = DpiHelper.DeviceToLogicalUnits(panel.PointToScreen(new Point(0.0, 0.0)));
      Rect[] elementBounds;
      SplitterPanel.Measure(availableSize, orientation, (IEnumerable) list, false, out elementBounds, panel);
      Rect rect = elementBounds[index];
      rect.Offset(point2.X, point2.Y);
      return rect;
    }

    private Rect PreviewDockCounterOrientation(DockDirection dockDirection, FrameworkElement adornedElement, ViewElement viewElement, SplitterLength itemLength, Orientation orientation)
    {
      List<UIElement> list = new List<UIElement>();
      Point point1 = new Point(0.0, 0.0);
      SplitterItem splitterItem1 = new SplitterItem();
      int index = 0;
      Size availableSize = new Size(adornedElement.ActualWidth, adornedElement.ActualHeight);
      SplitterItem splitterItem2 = new SplitterItem();
      list.Add((UIElement) splitterItem2);
      if (dockDirection == DockDirection.Right || dockDirection == DockDirection.Bottom)
        index = 1;
      list.Insert(index, (UIElement) splitterItem1);
      SplitterLength splitterLength = !(viewElement is MainSite) ? (orientation == Orientation.Horizontal ? viewElement.DockedWidth : viewElement.DockedHeight) : new SplitterLength(1.0, SplitterUnitType.Fill);
      SplitterPanel.SetSplitterLength((UIElement) splitterItem2, splitterLength);
      SplitterPanel.SetSplitterLength((UIElement) splitterItem1, itemLength);
      Point point2 = DpiHelper.DeviceToLogicalUnits(adornedElement.PointToScreen(new Point(0.0, 0.0)));
      Rect[] elementBounds;
      SplitterPanel.Measure(availableSize, orientation, (IEnumerable) list, false, out elementBounds, (SplitterPanel) null);
      Rect rect = elementBounds[index];
      rect.Offset(point2.X, point2.Y);
      return rect;
    }

    private Rect PreviewDockFill(FrameworkElement adornedElement)
    {
      Point location = DpiHelper.DeviceToLogicalUnits(adornedElement.PointToScreen(new Point(0.0, 0.0)));
      Size size = new Size();
      if (Microsoft.VisualStudio.PlatformUI.ExtensionMethods.IsNonreal(adornedElement.ActualHeight) || Microsoft.VisualStudio.PlatformUI.ExtensionMethods.IsNonreal(adornedElement.ActualWidth))
      {
        size = adornedElement.DesiredSize;
      }
      else
      {
        size.Width = adornedElement.ActualWidth;
        size.Height = adornedElement.ActualHeight;
      }
      return new Rect(location, size);
    }

    private IEnumerable<DependencyObject> GetHierarchy(DependencyObject sourceElement, DependencyObject commonAncestor)
    {
      while (sourceElement != null)
      {
        yield return sourceElement;
        sourceElement = Microsoft.VisualStudio.PlatformUI.ExtensionMethods.GetVisualOrLogicalParent(sourceElement);
        if (sourceElement == commonAncestor)
          break;
      }
    }

    protected virtual bool IsValidDockTarget(DockTarget target, IList<DockSiteType> types, ViewElement floatingElement)
    {
      if (target != null && (target.TargetElement == null || target.TargetElement.AreDockTargetsEnabled) && types.Contains(target.DockSiteType))
        return DockOperations.AreDockRestrictionsFulfilled(floatingElement.DockRestriction, target.TargetElement);
      return false;
    }

    protected virtual ICollection<DockAdornerWindow> PrepareAndShowAdornerLayers(SortedList<DockDirection, DockTarget> targets, DockManager.DockSite site, ViewElement floatingElement)
    {
      HashSet<DockAdornerWindow> hashSet = new HashSet<DockAdornerWindow>();
      DockAdornerWindow insertAfter = (DockAdornerWindow) null;
      foreach (KeyValuePair<DockDirection, DockTarget> keyValuePair in targets)
      {
        DockAdornerWindow adornerLayer = site.GetAdornerLayer(keyValuePair.Key);
        hashSet.Add(adornerLayer);
        adornerLayer.AdornedElement = (FrameworkElement) keyValuePair.Value;
        adornerLayer.DockDirection = keyValuePair.Key;
        adornerLayer.Orientation = this.GetTargetOrientation(keyValuePair.Value);
        adornerLayer.AreOuterTargetsEnabled = floatingElement.DockRestriction == DockRestrictionType.None;
        adornerLayer.AreInnerTargetsEnabled = !this.IsDocumentGroupContainerTarget(keyValuePair.Value);
        adornerLayer.IsInnerCenterTargetEnabled = keyValuePair.Value.DockTargetType != DockTargetType.SidesOnly;
        adornerLayer.AreInnerSideTargetsEnabled = keyValuePair.Value.DockTargetType != DockTargetType.CenterOnly;
        this.PrepareAdornerLayer(adornerLayer, floatingElement);
        adornerLayer.PrepareAndShow(insertAfter);
        insertAfter = adornerLayer;
      }
      return (ICollection<DockAdornerWindow>) hashSet;
    }

    protected virtual void PrepareAdornerLayer(DockAdornerWindow adornerLayer, ViewElement floatingElement)
    {
    }

    private ICollection<DockAdornerWindow> AddAdorners(DockManager.DockSite site, Visual ownerVisual, IList<DockSiteType> types, ViewElement floatingElement)
    {
      SortedList<DockDirection, DockTarget> targets = new SortedList<DockDirection, DockTarget>();
      for (DependencyObject sourceElement = (DependencyObject) ownerVisual; sourceElement != null; sourceElement = Microsoft.VisualStudio.PlatformUI.ExtensionMethods.GetVisualOrLogicalParent(sourceElement))
      {
        DockTarget target = sourceElement as DockTarget;
        if (this.IsValidDockTarget(target, types, floatingElement))
        {
          if (target.DockTargetType == DockTargetType.Outside)
          {
            targets[DockDirection.Left] = target;
            targets[DockDirection.Right] = target;
            targets[DockDirection.FirstValue] = target;
            targets[DockDirection.Bottom] = target;
          }
          else
          {
            DockTarget dockTarget;
            if ((target.DockTargetType == DockTargetType.Inside || target.DockTargetType == DockTargetType.SidesOnly || target.DockTargetType == DockTargetType.CenterOnly) && !targets.TryGetValue(DockDirection.Fill, out dockTarget))
              targets[DockDirection.Fill] = target;
          }
        }
      }
      return this.PrepareAndShowAdornerLayers(targets, site, floatingElement);
    }

    protected Orientation? GetTargetOrientation(DockTarget target)
    {
      Orientation? nullable = new Orientation?();
      if (target.DockSiteType == DockSiteType.NonDraggable)
      {
        DocumentGroupContainer documentGroupContainer = target.TargetElement as DocumentGroupContainer;
        if (documentGroupContainer == null)
        {
          DocumentGroup documentGroup = target.TargetElement as DocumentGroup;
          if (documentGroup != null)
            documentGroupContainer = documentGroup.Parent as DocumentGroupContainer;
        }
        if (documentGroupContainer != null && documentGroupContainer.VisibleChildren.Count > 1)
          nullable = new Orientation?(documentGroupContainer.Orientation);
      }
      return nullable;
    }

    protected bool IsDocumentGroupContainerTarget(DockTarget target)
    {
      if (target.DockSiteType == DockSiteType.NonDraggable)
        return target.TargetElement is DocumentGroupContainer;
      return false;
    }

    public void RegisterSite(Window window)
    {
      HwndSource hwndSource = PresentationSource.FromDependencyObject((DependencyObject) window) as HwndSource;
      if (hwndSource == null)
        window.SourceInitialized += new EventHandler(this.OnSourceInitialized);
      else
        this.RegisterSite((Visual) window, hwndSource.Handle);
    }

    public void RegisterSite(Visual visual, IntPtr hwnd)
    {
      if (this.visualToSite.ContainsKey(visual))
      {
        if (this.visualToSite[visual].Handle != hwnd)
          throw new InvalidOperationException("Visual cannot be used in RegisterSite with two different window handles");
      }
      else
      {
        DockManager.DockSite dockSite = new DockManager.DockSite()
        {
          Handle = hwnd,
          Visual = visual
        };
        this.visualToSite[visual] = dockSite;
        this.hwndToSite[hwnd] = dockSite;
      }
    }

    private void OnSourceInitialized(object sender, EventArgs args)
    {
      Window window = (Window) sender;
      window.SourceInitialized -= new EventHandler(this.OnSourceInitialized);
      this.RegisterSite(window);
    }

    public void UnregisterSite(Visual visual)
    {
      DockManager.DockSite dockSite;
      if (!this.visualToSite.TryGetValue(visual, out dockSite))
        return;
      this.visualToSite.Remove(visual);
      this.hwndToSite.Remove(dockSite.Handle);
    }

    protected DockManager.DockSiteHitTestResult FindHitElement(Point point, Predicate<DockManager.DockSite> includedSites)
    {
      using (IEnumerator<DockManager.DockSiteHitTestResult> enumerator = this.FindHitElements(point, includedSites).GetEnumerator())
      {
        if (enumerator.MoveNext())
          return enumerator.Current;
      }
      return (DockManager.DockSiteHitTestResult) null;
    }

    private List<DockManager.DockSite> GetSortedDockSites()
    {
      List<DockManager.DockSite> sortedSites = new List<DockManager.DockSite>();
      NativeMethods.EnumThreadWindows(NativeMethods.GetCurrentThreadId(), (NativeMethods.EnumWindowsProc) ((hWnd, lParam) =>
      {
        NativeMethods.EnumChildWindows(hWnd, (NativeMethods.EnumWindowsProc) ((hWndChild, lParamChild) =>
        {
          this.AddDockSiteFromHwnd(sortedSites, hWndChild);
          return true;
        }), IntPtr.Zero);
        this.AddDockSiteFromHwnd(sortedSites, hWnd);
        return true;
      }), IntPtr.Zero);
      return sortedSites;
    }

    private void AddDockSiteFromHwnd(List<DockManager.DockSite> sortedSites, IntPtr hWnd)
    {
      DockManager.DockSite dockSite;
      if (!this.hwndToSite.TryGetValue(hWnd, out dockSite))
        return;
      sortedSites.Add(dockSite);
    }

    private IEnumerable<DockManager.DockSiteHitTestResult> FindHitElements(Point point, Predicate<DockManager.DockSite> includedSites)
    {
      List<DockManager.DockSite> sortedDockSites = this.GetSortedDockSites();
      List<DockManager.DockSiteHitTestResult> results = new List<DockManager.DockSiteHitTestResult>();
      using (IEnumerator<DockManager.DockSite> enumerator = Enumerable.Where<DockManager.DockSite>((IEnumerable<DockManager.DockSite>) sortedDockSites, (Func<DockManager.DockSite, bool>) (w => includedSites(w))).GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          DockManager.DockSite site = enumerator.Current;
          if (PresentationSource.FromDependencyObject((DependencyObject) site.Visual) != null)
            VisualTreeHelper.HitTest(site.Visual, new HitTestFilterCallback(this.ExcludeNonVisualElements), (HitTestResultCallback) (result =>
            {
              results.Add(new DockManager.DockSiteHitTestResult(site, (Visual) result.VisualHit));
              return HitTestResultBehavior.Stop;
            }), (HitTestParameters) new PointHitTestParameters(site.Visual.PointFromScreen(point)));
        }
      }
      return (IEnumerable<DockManager.DockSiteHitTestResult>) results;
    }

    private HitTestFilterBehavior ExcludeNonVisualElements(DependencyObject potentialHitTestTarget)
    {
      if (!(potentialHitTestTarget is Visual))
        return HitTestFilterBehavior.ContinueSkipSelfAndChildren;
      UIElement uiElement = potentialHitTestTarget as UIElement;
      return uiElement == null || uiElement.IsVisible && uiElement.IsEnabled ? HitTestFilterBehavior.Continue : HitTestFilterBehavior.ContinueSkipSelfAndChildren;
    }

    public IList<DockAdornerWindow> GetAdornerWindows()
    {
      List<DockAdornerWindow> list = new List<DockAdornerWindow>();
      foreach (DockManager.DockSite dockSite in this.visualToSite.Values)
      {
        DockAdornerWindow dockAdornerWindow = dockSite.Visual as DockAdornerWindow;
        if (dockAdornerWindow != null)
          list.Add(dockAdornerWindow);
      }
      return (IList<DockAdornerWindow>) list;
    }

    protected class DockSite : IComparable<DockManager.DockSite>
    {
      private Dictionary<DockDirection, DockAdornerWindow> adorners = new Dictionary<DockDirection, DockAdornerWindow>();

      public IntPtr Handle { get; set; }

      public Visual Visual { get; set; }

      public DockAdornerWindow GetAdornerLayer(DockDirection type)
      {
        DockAdornerWindow dockAdornerWindow;
        if (!this.adorners.TryGetValue(type, out dockAdornerWindow))
        {
          dockAdornerWindow = new DockAdornerWindow(this.Handle);
          this.adorners[type] = dockAdornerWindow;
        }
        return dockAdornerWindow;
      }

      public int CompareTo(DockManager.DockSite other)
      {
        if (other.Handle == this.Handle)
          return 0;
        return !Microsoft.VisualStudio.PlatformUI.ExtensionMethods.IsBelow(this.Handle, other.Handle) ? -1 : 1;
      }
    }

    protected class DockSiteHitTestResult
    {
      public DockManager.DockSite DockSite { get; private set; }

      public Visual VisualHit { get; private set; }

      public DockSiteHitTestResult(DockManager.DockSite site, Visual visualHit)
      {
        this.DockSite = site;
        this.VisualHit = visualHit;
      }
    }

    public sealed class UndockingScope : IDisposable
    {
      public ViewElement Element { get; private set; }

      public Point Location { get; private set; }

      public UndockingScope(ViewElement element, Point undockingPoint)
      {
        this.Element = element;
        this.Location = undockingPoint;
        DockManager.Instance.UndockingInformation = this;
      }

      public void Dispose()
      {
        DockManager.Instance.UndockingInformation = (DockManager.UndockingScope) null;
      }
    }
  }
}
