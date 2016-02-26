// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.DockOperations
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.PlatformUI.Shell.Controls;
using Microsoft.VisualStudio.PlatformUI.Shell.Preferences;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.VisualStudio.PlatformUI.Shell
{
  public static class DockOperations
  {
    public const int InvalidInsertPosition = -1;
    private const double FloatStepX = 20.0;
    private const double FloatStepY = 20.0;
    private const double PositionRetries = 100.0;
    private const int VerticalUndockHeaderTopPadding = 8;
    private const int HorizontalUndockHeaderRightPadding = 50;
    private static double currentFloatLeft;
    private static double currentFloatTop;

    public static event EventHandler<DockEventArgs> DockPositionChanging;

    public static event EventHandler<DockEventArgs> DockPositionChanged;

    public static void Undock(this ViewElement element, WindowProfile windowProfile, Point undockingPoint, Rect currentUndockingRect)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      if (windowProfile == null)
        throw new ArgumentNullException("windowProfile");
      using (new DockOperations.DockEventRaiser(new DockEventArgs(DockAction.Undock, element)))
      {
        using (LayoutSynchronizer.BeginLayoutSynchronization())
        {
          PerformanceUtility.MeasurePerformanceUntilRender(PerformanceEvent.UndockPalette);
          ViewElement result = element;
          if (DockOperations.IsBookmarkable(element))
            DockOperations.ReplaceWithBookmark(element, out result, windowProfile);
          else
            element.Detach();
          FloatSite floatSite = FloatSite.Create();
          Size size = new Size(result.FloatingWidth, element.FloatingHeight);
          floatSite.FloatingLeft = result.FloatingLeft;
          floatSite.FloatingTop = result.FloatingTop;
          floatSite.FloatingWidth = size.Width;
          floatSite.FloatingHeight = size.Height;
          floatSite.MinimumHeight = result.MinimumHeight;
          floatSite.MinimumWidth = result.MinimumWidth;
          DockOperations.CalculateUndockPosition((ViewElement) floatSite, undockingPoint, currentUndockingRect, element.IsFloatingSizeExplicitlySet);
          floatSite.Children.Add(result);
          using (DockManager.Instance.CreateUndockingScope(result, undockingPoint))
          {
            using (ViewManager.Instance.DeferActiveViewChanges())
              windowProfile.Children.Add((ViewElement) floatSite);
          }
        }
      }
    }

    public static void Float(this ViewElement element, WindowProfile windowProfile)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      if (windowProfile == null)
        throw new ArgumentNullException("windowProfile");
      using (new DockOperations.DockEventRaiser(new DockEventArgs(DockAction.Float, element)))
      {
        using (LayoutSynchronizer.BeginLayoutSynchronization())
        {
          PerformanceUtility.MeasurePerformanceUntilRender(PerformanceEvent.FloatPalette);
          ViewElement result = element;
          double floatLeft = 0.0;
          double floatTop = 0.0;
          DockOperations.CalculateFloatingPosition(result, windowProfile, out floatLeft, out floatTop);
          if (DockOperations.IsBookmarkable(element))
            DockOperations.ReplaceWithBookmark(element, out result, windowProfile);
          else
            element.Detach();
          using (ViewManager.Instance.DeferActiveViewChanges())
          {
            FloatSite floatSite = FloatSite.Create();
            floatSite.FloatingWidth = result.FloatingWidth;
            floatSite.FloatingHeight = result.FloatingHeight;
            floatSite.MinimumHeight = result.MinimumHeight;
            floatSite.MinimumWidth = result.MinimumWidth;
            floatSite.FloatingLeft = floatLeft;
            floatSite.FloatingTop = floatTop;
            floatSite.Children.Add(result);
            windowProfile.Children.Add((ViewElement) floatSite);
          }
        }
      }
    }

    private static bool IsDockingViewValid(ViewElement dockingView)
    {
      if (!(dockingView is DockGroup) && !(dockingView is TabGroup))
        return dockingView is View;
      return true;
    }

    private static bool IsDockTargetValid(ViewElement targetView)
    {
      if (!(targetView is AutoHideGroup) && !(targetView is AutoHideChannel) && !(targetView is AutoHideRoot))
        return !(targetView is ViewSite);
      return false;
    }

    public static void Dock(this ViewElement targetView, ViewElement dockingView, DockDirection dockDirection)
    {
      if (targetView == null)
        throw new ArgumentNullException("targetView");
      if (dockingView == null)
        throw new ArgumentNullException("dockingView");
      if (dockingView == targetView || ExtensionMethods.IsAncestorOf(dockingView, targetView))
        throw new ArgumentException("targetView and dockingView must not be the same instance, and dockingView cannot be an ancestor of targetView.");
      if (!DockOperations.IsDockingViewValid(dockingView))
        throw new ArgumentException("dockingView is not a dockable ViewElement");
      if (!DockOperations.IsDockTargetValid(targetView))
        throw new ArgumentException("targetView is not a valid target for docking");
      if (dockDirection < DockDirection.FirstValue || dockDirection > DockDirection.Fill)
        throw new InvalidEnumArgumentException("dockDirection", (int) dockDirection, typeof (DockDirection));
      if (!DockOperations.AreDockRestrictionsFulfilled(dockingView, targetView))
        throw new ArgumentException("dockingView can not be docked in targetView due to restrictions present in dockingView");
      using (new DockOperations.DockEventRaiser(new DockEventArgs(DockAction.Dock, dockingView)))
      {
        using (targetView.PreventCollapse())
        {
          using (LayoutSynchronizer.BeginLayoutSynchronization())
          {
            dockingView.Detach();
            using (ViewManager.Instance.DeferActiveViewChanges())
            {
              if (dockDirection == DockDirection.Fill)
                DockOperations.DockNested(DockOperations.GetEffectiveDockTarget(targetView), dockingView);
              else
                DockOperations.DockBeside(DockOperations.GetEffectiveDockTarget(targetView), dockingView, dockDirection);
            }
          }
        }
      }
    }

    private static ViewElement GetEffectiveDockTarget(ViewElement targetElement)
    {
      View view = targetElement as View;
      if (view != null && AutoHideChannel.IsAutoHidden((ViewElement) view))
      {
        ViewBookmark bookmark = DockOperations.FindBookmark(view, view.WindowProfile);
        if (bookmark == null)
        {
          bookmark = ViewBookmark.Create(view);
          DockOperations.DockBeside(DockOperations.GetAutoHideCenter(view.WindowProfile), (ViewElement) bookmark, DockDirection.Bottom);
        }
        return DockOperations.GetEffectiveDockTarget((ViewElement) bookmark);
      }
      if (targetElement.Parent is NestedGroup)
        return (ViewElement) targetElement.Parent;
      return targetElement;
    }

    public static void AutoHide(this ViewElement viewElement)
    {
      if (viewElement == null)
        throw new ArgumentNullException("viewElement");
      if (!DockOperations.CanAutoHide(viewElement))
        return;
      using (new DockOperations.DockEventRaiser(new DockEventArgs(DockAction.AutoHide, viewElement)))
      {
        viewElement.AutoHideHeight = viewElement.DockedHeight.Value;
        viewElement.AutoHideWidth = viewElement.DockedWidth.Value;
        AutoHideChannel autoHideChannel = DockOperations.FindAutoHideChannel(viewElement);
        ViewElement result;
        ViewElement viewElement1 = DockOperations.ReplaceWithBookmark(viewElement, out result, viewElement.WindowProfile);
        List<ViewElement> list = new List<ViewElement>(result.FindAll((Predicate<ViewElement>) (e => e is View)));
        if (list.Count <= 0)
          return;
        ViewGroup originalLocation = viewElement1 is ViewBookmark ? viewElement1.Parent : (ViewGroup) viewElement1;
        AutoHideGroup existingAutoHideGroup = DockOperations.FindExistingAutoHideGroup(autoHideChannel, originalLocation);
        if (existingAutoHideGroup == null)
        {
          existingAutoHideGroup = AutoHideGroup.Create();
          autoHideChannel.Children.Add((ViewElement) existingAutoHideGroup);
        }
        foreach (View view in list)
        {
          view.AutoHideHeight = result.AutoHideHeight;
          view.AutoHideWidth = result.AutoHideWidth;
          view.IsSelected = false;
          existingAutoHideGroup.Children.Add((ViewElement) view);
        }
      }
    }

    public static void DockViewElementOrGroup(ViewElement element, bool autoHideOnlyActiveView)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      if (!AutoHideChannel.IsAutoHidden(element))
        throw new ArgumentException("element must be autohidden");
      if (!autoHideOnlyActiveView)
      {
        foreach (ViewElement element1 in new List<ViewElement>((IEnumerable<ViewElement>) (element.Parent as AutoHideGroup).Children))
        {
          if (element1 != element)
            DockOperations.SnapToBookmark(element1);
        }
      }
      DockOperations.SnapToBookmark(element);
    }

    public static void DockViewElementOrGroup(ViewElement element)
    {
      DockOperations.DockViewElementOrGroup(element, ViewManager.Instance.Preferences.AutoHideOnlyActiveView);
    }

    public static void AutoHideViewElementOrGroup(ViewElement element, bool autoHideOnlyActiveView)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      if (!(ViewElement.FindRootElement(element) is MainSite))
        throw new ArgumentException("element must be docked in a MainSite.");
      if (!autoHideOnlyActiveView)
      {
        TabGroup tabGroup = element.Parent as TabGroup;
        if (tabGroup == null)
          return;
        foreach (ViewElement viewElement in new List<ViewElement>((IEnumerable<ViewElement>) tabGroup.Children))
          DockOperations.AutoHide(viewElement);
      }
      else
        DockOperations.AutoHide(element);
    }

    public static void AutoHideViewElementOrGroup(ViewElement element)
    {
      DockOperations.AutoHideViewElementOrGroup(element, ViewManager.Instance.Preferences.AutoHideOnlyActiveView);
    }

    private static AutoHideGroup FindExistingAutoHideGroup(AutoHideChannel channel, ViewGroup originalLocation)
    {
      if (!(originalLocation is TabGroup))
        return (AutoHideGroup) null;
      AutoHideGroup autoHideGroup1 = (AutoHideGroup) null;
      foreach (AutoHideGroup autoHideGroup2 in (IEnumerable<ViewElement>) channel.Children)
      {
        if (autoHideGroup2.OriginalGroup == originalLocation)
        {
          autoHideGroup1 = autoHideGroup2;
          break;
        }
      }
      return autoHideGroup1;
    }

    internal static bool CanAutoHide(ViewElement element)
    {
      if (AutoHideChannel.IsAutoHidden(element) || FloatSite.IsFloating(element) || DocumentGroup.IsTabbedDocument(element))
        return false;
      if (!(element is View))
        return element is TabGroup;
      return true;
    }

    private static ViewElement GetAutoHideCenter(WindowProfile profile)
    {
      return profile.Find((Predicate<ViewElement>) (e => AutoHideRoot.GetIsAutoHideCenter(e)));
    }

    private static AutoHideChannel FindAutoHideChannel(ViewElement element)
    {
      ViewElement autoHideCenter = DockOperations.GetAutoHideCenter(element.WindowProfile);
      ViewElement commonAncestor = Microsoft.VisualStudio.PlatformUI.ExtensionMethods.FindCommonAncestor<ViewElement>(element, autoHideCenter, (Func<ViewElement, ViewElement>) (e => (ViewElement) e.Parent));
      if (commonAncestor == null)
        return (AutoHideChannel) null;
      DockGroup dockGroup = commonAncestor as DockGroup;
      if (dockGroup == null)
        return (AutoHideChannel) null;
      int subtreeIndex1 = DockOperations.FindSubtreeIndex((ViewGroup) dockGroup, autoHideCenter);
      int subtreeIndex2 = DockOperations.FindSubtreeIndex((ViewGroup) dockGroup, element);
      Dock dock = dockGroup.Orientation != Orientation.Horizontal ? (subtreeIndex2 >= subtreeIndex1 ? System.Windows.Controls.Dock.Bottom : System.Windows.Controls.Dock.Top) : (subtreeIndex2 >= subtreeIndex1 ? System.Windows.Controls.Dock.Right : System.Windows.Controls.Dock.Left);
      return DockOperations.GetAutoHideChannel(element.WindowProfile, dock);
    }

    private static AutoHideChannel GetAutoHideChannel(WindowProfile profile, Dock dock)
    {
      foreach (AutoHideChannel autoHideChannel in profile.FindAll((Predicate<ViewElement>) (e => e is AutoHideChannel)))
      {
        if ((Dock) autoHideChannel.GetValue(DockPanel.DockProperty) == dock)
          return autoHideChannel;
      }
      return (AutoHideChannel) null;
    }

    private static int FindSubtreeIndex(ViewGroup rootElement, ViewElement subtreeElement)
    {
      while (subtreeElement.Parent != rootElement)
        subtreeElement = (ViewElement) subtreeElement.Parent;
      return rootElement.Children.IndexOf(subtreeElement);
    }

    public static void DockAt(this ViewElement targetView, ViewElement dockingView, int dockPosition)
    {
      if (targetView == null)
        throw new ArgumentNullException("targetView");
      if (dockingView == null)
        throw new ArgumentNullException("dockingView");
      if (dockingView == targetView || ExtensionMethods.IsAncestorOf(dockingView, targetView))
        throw new ArgumentException("targetView and dockingView must not be the same instance, and dockingView cannot be an ancestor of targetView.");
      if (!DockOperations.IsDockingViewValid(dockingView))
        throw new ArgumentException("dockingView is not a dockable ViewElement");
      if (!DockOperations.IsDockTargetValid(targetView))
        throw new ArgumentException("targetView is not a valid target for docking");
      if (!DockOperations.AreDockRestrictionsFulfilled(dockingView, targetView))
        throw new ArgumentException("dockingView can not be docked in targetView due to restrictions present in dockingView");
      using (new DockOperations.DockEventRaiser(new DockEventArgs(DockAction.Dock, dockingView)))
      {
        using (targetView.PreventCollapse())
        {
          using (LayoutSynchronizer.BeginLayoutSynchronization())
          {
            ViewElement result = dockingView;
            if (DockOperations.IsBookmarkable(dockingView))
              DockOperations.ReplaceWithBookmark(dockingView, out result, dockingView.WindowProfile);
            else
              dockingView.Detach();
            using (ViewManager.Instance.DeferActiveViewChanges())
              DockOperations.DockNested(DockOperations.GetEffectiveDockTarget(targetView), result, dockPosition);
          }
        }
      }
    }

    public static DocumentGroup GetPrimaryDocumentGroup(WindowProfile profile)
    {
      if (profile == null)
        throw new ArgumentNullException("profile");
      return profile.Find((Predicate<ViewElement>) (e =>
      {
        if (e is DocumentGroup)
          return e.IsVisible;
        return false;
      })) as DocumentGroup ?? profile.Find((Predicate<ViewElement>) (e => e is DocumentGroup)) as DocumentGroup;
    }

    private static void DockNested(ViewElement targetView, ViewElement dockingView)
    {
      DockOperations.DockNested(targetView, dockingView, -1);
    }

    private static void DockNested(ViewElement targetView, ViewElement dockingView, int dockPosition)
    {
      NestedGroup stackedParent = DockOperations.GetStackedParent(targetView);
      if (-1 != dockPosition && (dockPosition < 0 || dockPosition > stackedParent.VisibleChildren.Count))
        throw new ArgumentOutOfRangeException(string.Concat(new object[4]
        {
          (object) "dockPosition is out of range. parent.visiblechildren: ",
          (object) stackedParent.VisibleChildren.Count,
          (object) ", dockPosition: ",
          (object) dockPosition
        }));
      List<ViewElement> list = new List<ViewElement>(dockingView.FindAll((Predicate<ViewElement>) (v => v is View)));
      foreach (ViewElement view in list)
      {
        WindowProfile windowProfile = WindowProfile.FindWindowProfile(view);
        if (windowProfile != null)
          DockOperations.ClearBookmark(view as View, windowProfile, ExtensionMethods.GetBookmarkType(targetView));
      }
      if ((!(stackedParent is DocumentGroup) ? ViewManager.Instance.Preferences.TabDockPreference : ViewManager.Instance.Preferences.DocumentDockPreference) == DockPreference.DockAtBeginning && dockPosition == -1)
        dockPosition = 0;
      ViewElement viewElement1 = (ViewElement) null;
      int index = stackedParent.ChildIndexFromVisibleChildIndex(dockPosition);
      foreach (ViewElement viewElement2 in list)
      {
        if (viewElement2.IsVisible)
          viewElement1 = viewElement2;
        if (-1 != dockPosition)
        {
          stackedParent.Children.Insert(index, viewElement2);
          ++index;
        }
        else
          stackedParent.Children.Add(viewElement2);
      }
      stackedParent.SelectedElement = viewElement1 ?? stackedParent.SelectedElement;
    }

    public static void MoveTab(ViewElement tab, int position)
    {
      if (tab == null)
        throw new ArgumentNullException("tab");
      if (!(tab is View))
        throw new ArgumentException("Tab must be a View");
      ViewGroup parent = tab.Parent;
      if (parent == null)
        throw new ArgumentException("tab should have a parent");
      if (!(parent is NestedGroup))
        throw new ArgumentException("Parent of tab must be a NestedGroup.");
      using (new DockOperations.DockEventRaiser(new DockEventArgs(DockAction.ReorderTab, tab)))
      {
        using (parent.PreventCollapse())
        {
          using (LayoutSynchronizer.BeginLayoutSynchronization())
          {
            if (position < 0 || position > parent.VisibleChildren.Count)
              throw new ArgumentOutOfRangeException(string.Concat(new object[4]
              {
                (object) "position: ",
                (object) position,
                (object) " parent.visiblechildren: ",
                (object) parent.VisibleChildren.Count
              }));
            using (ViewManager.Instance.DeferActiveViewChanges())
            {
              tab.Detach();
              int index = parent.ChildIndexFromVisibleChildIndex(position);
              parent.Children.Insert(index, tab);
              parent.SelectedElement = tab;
            }
          }
        }
      }
    }

    public static DocumentGroup CreateDocumentGroupAt(DocumentGroupContainer container, int position)
    {
      if (container == null)
        throw new ArgumentNullException("container");
      if (position < 0 || position > container.Children.Count)
        throw new ArgumentOutOfRangeException("position: " + (object) position);
      DocumentGroup documentGroup = DocumentGroup.Create();
      container.Children.Insert(position, (ViewElement) documentGroup);
      return documentGroup;
    }

    public static bool AreDockRestrictionsFulfilled(ViewElement element, ViewElement target)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      if (target == null)
        throw new ArgumentNullException("target");
      return DockOperations.AreDockRestrictionsFulfilled(element.DockRestriction, target);
    }

    public static bool AreDockRestrictionsFulfilled(DockRestrictionType restriction, ViewElement target)
    {
      if (target == null)
        throw new ArgumentNullException("target");
      bool flag = false;
      if (restriction == DockRestrictionType.None)
      {
        flag = true;
      }
      else
      {
        if ((restriction & DockRestrictionType.DocumentGroup) == DockRestrictionType.DocumentGroup && (target is DocumentGroup || target is DocumentGroupContainer))
          flag = true;
        if ((restriction & DockRestrictionType.Document) == DockRestrictionType.Document && target.Parent != null && target.Parent is DocumentGroup)
          flag = true;
      }
      return flag;
    }

    private static NestedGroup GetStackedParent(ViewElement view)
    {
      DocumentGroupContainer documentGroupContainer = view as DocumentGroupContainer;
      NestedGroup nestedGroup;
      if (documentGroupContainer != null)
      {
        if (documentGroupContainer.Children.Count == 0)
          throw new InvalidOperationException("A DocumentGroupContainer must always have at least one child");
        nestedGroup = (NestedGroup) DockOperations.GetPrimaryDocumentGroup(documentGroupContainer.WindowProfile);
      }
      else
      {
        nestedGroup = view as NestedGroup;
        if (nestedGroup == null)
        {
          nestedGroup = view.Parent as NestedGroup;
          if (nestedGroup == null)
          {
            nestedGroup = (NestedGroup) TabGroup.Create();
            DockOperations.CopySize(view, (ViewElement) nestedGroup);
            ExtensionMethods.InsertNewParent(view, (ViewGroup) nestedGroup);
          }
        }
      }
      return nestedGroup;
    }

    private static void CopySize(ViewElement source, ViewElement target)
    {
      target.DockedWidth = source.DockedWidth;
      target.DockedHeight = source.DockedHeight;
    }

    private static void CopySizeFixedToStretch(ViewElement source, ViewElement target)
    {
      target.DockedWidth = new SplitterLength(source.DockedWidth.Value, source.DockedWidth.IsFixed ? SplitterUnitType.Stretch : source.DockedWidth.SplitterUnitType);
      target.DockedHeight = new SplitterLength(source.DockedHeight.Value, source.DockedHeight.IsFixed ? SplitterUnitType.Stretch : source.DockedHeight.SplitterUnitType);
    }

    private static void DockBeside(ViewElement targetView, ViewElement dockingView, DockDirection dockDirection)
    {
      DockGroup dockParent = DockOperations.GetDockParent(targetView, dockDirection);
      int index = dockParent.Children.IndexOf(targetView);
      DockOperations.Dock(dockParent, dockingView, index, dockDirection);
    }

    private static DockGroup GetDockParent(ViewElement view, DockDirection dockDirection)
    {
      DockGroup dockGroup = view.Parent as DockGroup;
      if (dockGroup == null)
      {
        dockGroup = DockGroup.Create();
        DockOperations.CopySizeFixedToStretch(view, (ViewElement) dockGroup);
        dockGroup.Orientation = dockDirection == DockDirection.Left || dockDirection == DockDirection.Right ? Orientation.Horizontal : Orientation.Vertical;
        ExtensionMethods.InsertNewParent(view, (ViewGroup) dockGroup);
      }
      return dockGroup;
    }

    private static void Dock(DockGroup parent, ViewElement view, int index, DockDirection dockDirection)
    {
      if (parent.Orientation == Orientation.Horizontal)
      {
        switch (dockDirection)
        {
          case DockDirection.FirstValue:
            DockOperations.DockCounterOrientation(parent, view, index, 0);
            break;
          case DockDirection.Bottom:
            DockOperations.DockCounterOrientation(parent, view, index, 1);
            break;
          case DockDirection.Left:
            DockOperations.DockSameOrientation(parent, view, index);
            break;
          case DockDirection.Right:
            DockOperations.DockSameOrientation(parent, view, index + 1);
            break;
        }
      }
      else
      {
        switch (dockDirection)
        {
          case DockDirection.FirstValue:
            DockOperations.DockSameOrientation(parent, view, index);
            break;
          case DockDirection.Bottom:
            DockOperations.DockSameOrientation(parent, view, index + 1);
            break;
          case DockDirection.Left:
            DockOperations.DockCounterOrientation(parent, view, index, 0);
            break;
          case DockDirection.Right:
            DockOperations.DockCounterOrientation(parent, view, index, 1);
            break;
        }
      }
    }

    private static void DockSameOrientation(DockGroup parent, ViewElement view, int index)
    {
      DockOperations.DockSameOrientation<ViewElement>((IList<ViewElement>) parent.Children, view, parent.Orientation, index, true, (Func<ViewElement, ViewElement>) (v => v));
    }

    private static void DockCounterOrientation(DockGroup parent, ViewElement view, int index, int counterIndex)
    {
      DockOperations.DockSameOrientation(DockOperations.NestDockAt(parent, index), view, counterIndex);
    }

    private static DockGroup NestDockAt(DockGroup parent, int index)
    {
      ViewElement viewElement = parent.Children[index];
      DockGroup dockGroup = DockGroup.Create();
      dockGroup.Orientation = parent.Orientation == Orientation.Horizontal ? Orientation.Vertical : Orientation.Horizontal;
      DockOperations.CopySizeFixedToStretch(viewElement, (ViewElement) dockGroup);
      ExtensionMethods.InsertNewParent(viewElement, (ViewGroup) dockGroup);
      return dockGroup;
    }

    internal static void DockSameOrientation<TChild>(IList<TChild> collection, ViewElement dockingView, Orientation orientation, int targetIndex, bool modifySourceObjects, Func<ViewElement, TChild> viewToT)
    {
      DockGroup dockGroup;
      for (dockGroup = dockingView as DockGroup; dockGroup != null && dockGroup.Children.Count == 1; dockGroup = dockingView as DockGroup)
      {
        dockingView = dockGroup.Children[0];
        if (modifySourceObjects)
          dockGroup.Children.Clear();
      }
      if (dockGroup != null && dockGroup.Orientation == orientation)
      {
        List<TChild> list = new List<TChild>();
        foreach (ViewElement viewElement in (IEnumerable<ViewElement>) dockGroup.Children)
          list.Add(viewToT(viewElement));
        if (modifySourceObjects)
          dockGroup.Children.Clear();
        foreach (TChild child in list)
        {
          collection.Insert(targetIndex, child);
          ++targetIndex;
        }
      }
      else
        collection.Insert(targetIndex, viewToT(dockingView));
    }

    public static void DockOutside(this ViewGroup targetView, ViewElement dockingView, DockDirection dockDirection)
    {
      if (targetView == null)
        throw new ArgumentNullException("targetView");
      if (dockingView == null)
        throw new ArgumentNullException("dockingView");
      if (dockDirection == DockDirection.Fill)
        throw new ArgumentException("DockDirection cannot be Fill in DockOutside");
      if (dockDirection < DockDirection.FirstValue || dockDirection > DockDirection.Fill)
        throw new InvalidEnumArgumentException("dockDirection", (int) dockDirection, typeof (DockDirection));
      if (dockingView == targetView || ExtensionMethods.IsAncestorOf(dockingView, (ViewElement) targetView))
        throw new ArgumentException("targetView and dockingView must not be the same instance, and dockingView cannot be an ancestor of targetView.");
      if (!DockOperations.IsDockingViewValid(dockingView))
        throw new ArgumentException("dockingView is not a dockable ViewElement");
      if (!DockOperations.IsDockTargetValid((ViewElement) targetView))
        throw new ArgumentException("targetView is not a valid target for docking");
      if (targetView.Children.Count > 1)
        throw new ArgumentException("targetView of DockOutside cannot have more than 1 child");
      using (new DockOperations.DockEventRaiser(new DockEventArgs(DockAction.Dock, dockingView)))
      {
        using (LayoutSynchronizer.BeginLayoutSynchronization())
        {
          dockingView.Detach();
          if (targetView.Children.Count == 0)
          {
            targetView.Children.Add(dockingView);
          }
          else
          {
            ViewElement viewElement = targetView.Children[0];
            DocumentGroupContainer documentGroupContainer = viewElement as DocumentGroupContainer;
            DockGroup dockGroup = viewElement as DockGroup;
            Orientation orientation = dockDirection == DockDirection.Left || dockDirection == DockDirection.Right ? Orientation.Horizontal : Orientation.Vertical;
            if (dockGroup == null || documentGroupContainer != null)
              dockGroup = DockOperations.NestDock(targetView, orientation);
            int index = 0;
            DockGroup parent = dockGroup;
            if (dockGroup.Orientation == Orientation.Horizontal)
            {
              switch (dockDirection)
              {
                case DockDirection.FirstValue:
                case DockDirection.Bottom:
                  parent = DockOperations.NestDock(targetView, dockGroup.Orientation == Orientation.Horizontal ? Orientation.Vertical : Orientation.Horizontal);
                  break;
                case DockDirection.Right:
                  index = dockGroup.Children.Count - 1;
                  break;
              }
            }
            else
            {
              switch (dockDirection)
              {
                case DockDirection.Bottom:
                  index = dockGroup.Children.Count - 1;
                  break;
                case DockDirection.Left:
                case DockDirection.Right:
                  parent = DockOperations.NestDock(targetView, dockGroup.Orientation == Orientation.Horizontal ? Orientation.Vertical : Orientation.Horizontal);
                  break;
              }
            }
            DockOperations.Dock(parent, dockingView, index, dockDirection);
          }
        }
      }
    }

    private static DockGroup NestDock(ViewGroup site, Orientation orientation)
    {
      DockGroup dockGroup = DockGroup.Create();
      dockGroup.Orientation = orientation;
      ViewElement viewElement = site.Children[0];
      SplitterLength dockedWidth = viewElement.DockedWidth;
      SplitterLength splitterLength = viewElement == null || !dockedWidth.IsFill ? new SplitterLength(100.0, SplitterUnitType.Stretch) : new SplitterLength(1.0, SplitterUnitType.Fill);
      dockGroup.DockedWidth = splitterLength;
      dockGroup.DockedHeight = splitterLength;
      site.Children[0] = (ViewElement) dockGroup;
      if (viewElement != null)
        dockGroup.Children.Add(viewElement);
      return dockGroup;
    }

    public static IDisposable PreventCollapse(this IEnumerable<ViewElement> elements)
    {
      return (IDisposable) new DockOperations.PreventCollapseScope(elements);
    }

    public static void TryCollapse(this ViewElement element)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      DockOperations.TryCollapseInternal(element);
    }

    private static void TryCollapseInternal(ViewElement element)
    {
      if (!element.IsCollapsible)
        return;
      DocumentGroupContainer docGroupContainer = element as DocumentGroupContainer;
      if (docGroupContainer != null)
      {
        DockOperations.TryCollapseInternal(docGroupContainer);
      }
      else
      {
        DockGroup dockGroup = element as DockGroup;
        if (dockGroup != null)
        {
          DockOperations.TryCollapseInternal(dockGroup);
        }
        else
        {
          TabGroup tabGroup = element as TabGroup;
          if (tabGroup != null)
          {
            DockOperations.TryCollapseInternal(tabGroup);
          }
          else
          {
            FloatSite floatSite = element as FloatSite;
            if (floatSite != null)
            {
              DockOperations.TryCollapseInternal(floatSite);
            }
            else
            {
              DocumentGroup docGroup = element as DocumentGroup;
              if (docGroup != null)
              {
                DockOperations.TryCollapseInternal(docGroup);
              }
              else
              {
                AutoHideGroup autoHideGroup = element as AutoHideGroup;
                if (autoHideGroup == null)
                  return;
                DockOperations.TryCollapseInternal(autoHideGroup);
              }
            }
          }
        }
      }
    }

    private static void TryCollapseInternal(AutoHideGroup autoHideGroup)
    {
      if (autoHideGroup.Children.Count != 0)
        return;
      autoHideGroup.Detach();
    }

    private static void TryCollapseInternal(FloatSite floatSite)
    {
      if (floatSite.Children.Count != 0)
        return;
      floatSite.WindowProfile.Children.Remove((ViewElement) floatSite);
    }

    private static void TryCollapseInternal(TabGroup tabGroup)
    {
      if (tabGroup.Children.Count == 0)
      {
        tabGroup.Detach();
      }
      else
      {
        if (tabGroup.Children.Count != 1)
          return;
        ViewGroup parent = tabGroup.Parent;
        if (parent == null)
          return;
        DockOperations.Collapse((ViewGroup) tabGroup, parent);
      }
    }

    private static void TryCollapseInternal(DocumentGroupContainer docGroupContainer)
    {
    }

    private static void TryCollapseInternal(DockGroup dockGroup)
    {
      if (dockGroup.Children.Count == 0)
        dockGroup.Detach();
      else if (dockGroup.Children.Count == 1)
      {
        ViewGroup parent = dockGroup.Parent;
        if (parent == null)
          return;
        DockOperations.Collapse((ViewGroup) dockGroup, parent);
      }
      else
      {
        DockGroup parentDockGroup = dockGroup.Parent as DockGroup;
        if (parentDockGroup == null || parentDockGroup.Orientation != dockGroup.Orientation)
          return;
        DockOperations.Collapse(dockGroup, parentDockGroup);
      }
    }

    private static void TryCollapseInternal(DocumentGroup docGroup)
    {
      if (docGroup.Parent == null)
        return;
      List<ViewElement> list = new List<ViewElement>(docGroup.Parent.FindAll((Predicate<ViewElement>) (v => v is DocumentGroup)));
      if (docGroup.Children.Count != 0 || list.Count <= 1)
        return;
      docGroup.Detach();
    }

    private static void Collapse(DockGroup dockGroup, DockGroup parentDockGroup)
    {
      int index = parentDockGroup.Children.IndexOf((ViewElement) dockGroup);
      List<ViewElement> list = new List<ViewElement>((IEnumerable<ViewElement>) dockGroup.Children);
      using (dockGroup.PreventCollapse())
      {
        foreach (ViewElement viewElement in list)
        {
          parentDockGroup.Children.Insert(index, viewElement);
          ++index;
        }
        parentDockGroup.Children.Remove((ViewElement) dockGroup);
      }
      foreach (ViewElement element in list)
        DockOperations.TryCollapseInternal(element);
    }

    private static void Collapse(ViewGroup viewGroup, ViewGroup parentGroup)
    {
      int index = parentGroup.Children.IndexOf((ViewElement) viewGroup);
      ViewElement element = viewGroup.Children[0];
      parentGroup.Children[index] = element;
      DockOperations.TryCollapseInternal(element);
    }

    private static void ReplaceElement(ViewElement currentElement, ViewElement newElement)
    {
      int index = currentElement.Parent.Children.IndexOf(currentElement);
      currentElement.Parent.Children[index] = newElement;
    }

    public static ViewElement ReplaceWithBookmark(ViewElement element, out ViewElement result, WindowProfile windowProfile)
    {
      if (windowProfile == null)
        throw new ArgumentNullException("windowProfile");
      result = element;
      if (DockManager.Instance.IsDragging)
        return (ViewElement) null;
      View view = element as View;
      if (view != null)
        return (ViewElement) DockOperations.ReplaceWithBookmark(view, windowProfile);
      TabGroup tabGroup = element as TabGroup;
      if (tabGroup != null)
        return (ViewElement) DockOperations.ReplaceWithBookmark(tabGroup, out result, windowProfile);
      throw new ArgumentException("Only TabGroup and View elements are supported for bookmark replacement.");
    }

    private static ViewBookmark ReplaceWithBookmark(View view, WindowProfile windowProfile)
    {
      ViewBookmarkType bookmarkType = ExtensionMethods.GetBookmarkType((ViewElement) view);
      DockOperations.ClearBookmark(view, windowProfile, bookmarkType);
      DockOperations.RenumberBookmarkAccessOrder(view.Name, windowProfile);
      ViewBookmark viewBookmark = ViewBookmark.Create(view);
      DockOperations.ReplaceElement((ViewElement) view, (ViewElement) viewBookmark);
      return viewBookmark;
    }

    private static TabGroup ReplaceWithBookmark(TabGroup tabGroup, out ViewElement result, WindowProfile windowProfile)
    {
      result = (ViewElement) tabGroup;
      using (tabGroup.PreventCollapse())
      {
        TabGroup tabGroup1 = TabGroup.Create();
        tabGroup1.DockedHeight = tabGroup.DockedHeight;
        tabGroup1.DockedWidth = tabGroup.DockedWidth;
        foreach (ViewElement viewElement in new List<ViewElement>((IEnumerable<ViewElement>) tabGroup.Children))
        {
          ViewBookmark viewBookmark = viewElement as ViewBookmark;
          if (viewBookmark == null)
          {
            View view = (View) viewElement;
            DockOperations.ClearBookmark(view, windowProfile, ExtensionMethods.GetBookmarkType((ViewElement) view));
            DockOperations.RenumberBookmarkAccessOrder(view.Name, windowProfile);
            viewBookmark = ViewBookmark.Create(view);
          }
          tabGroup1.Children.Add((ViewElement) viewBookmark);
        }
        DockOperations.ReplaceElement((ViewElement) tabGroup, (ViewElement) tabGroup1);
        if (tabGroup.Children.Count == 1)
        {
          result = tabGroup.Children[0];
          result.AutoHideHeight = tabGroup.AutoHideHeight;
          result.AutoHideWidth = tabGroup.AutoHideWidth;
          result.Detach();
        }
        return tabGroup1;
      }
    }

    private static void DefaultUnresolvedBookmarkHandler(View view, WindowProfile windowProfile, ViewBookmarkType bookmarkType)
    {
      DocumentGroup primaryDocumentGroup = DockOperations.GetPrimaryDocumentGroup(windowProfile);
      if (primaryDocumentGroup == null)
        return;
      if (bookmarkType == ViewBookmarkType.DocumentWell)
        DockOperations.Dock((ViewElement) primaryDocumentGroup, (ViewElement) view, DockDirection.Fill);
      else
        DockOperations.Dock((ViewElement) primaryDocumentGroup.Parent, (ViewElement) view, DockDirection.Bottom);
      view.IsSelected = true;
    }

    public static void SnapToBookmark(this ViewElement element)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      if (element.DockRestriction == DockRestrictionType.AlwaysFloating)
        return;
      using (ViewManager.Instance.DeferActiveViewChanges())
        DockOperations.SnapToBookmark(element, element.WindowProfile, ViewBookmarkType.All, new Action<View, WindowProfile, ViewBookmarkType>(DockOperations.DefaultUnresolvedBookmarkHandler));
    }

    public static void SnapToBookmark(this ViewElement element, ViewBookmarkType type)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      using (ViewManager.Instance.DeferActiveViewChanges())
        DockOperations.SnapToBookmark(element, element.WindowProfile, type, new Action<View, WindowProfile, ViewBookmarkType>(DockOperations.DefaultUnresolvedBookmarkHandler));
    }

    public static void SnapToBookmark(this ViewElement element, ViewBookmarkType type, Action<View, WindowProfile, ViewBookmarkType> unresolvedBookmarkHandler)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      if (unresolvedBookmarkHandler == null)
        throw new ArgumentNullException("unresolvedBookmarkHandler");
      using (ViewManager.Instance.DeferActiveViewChanges())
        DockOperations.SnapToBookmark(element, element.WindowProfile, type, unresolvedBookmarkHandler);
    }

    private static void SnapToBookmark(View view, WindowProfile windowProfile, ViewBookmarkType type, Action<View, WindowProfile, ViewBookmarkType> unresolvedBookmarkHandler)
    {
      ViewBookmarkType bookmarkType = ExtensionMethods.GetBookmarkType((ViewElement) view);
      ViewBookmark bookmark = DockOperations.FindBookmark(view.Name, windowProfile, type);
      ViewBookmark viewBookmark = bookmarkType != type ? DockOperations.FindBookmark(view, view.WindowProfile, bookmarkType) : (ViewBookmark) null;
      using (new DockOperations.DockEventRaiser(new DockEventArgs(DockAction.SnapToBookmark, (ViewElement) view)))
      {
        using (LayoutSynchronizer.BeginLayoutSynchronization())
        {
          if (bookmarkType != type && viewBookmark == null && DockOperations.IsBookmarkable((ViewElement) view))
            DockOperations.ReplaceWithBookmark(view, view.WindowProfile);
          if (bookmark == null)
          {
            unresolvedBookmarkHandler(view, windowProfile, type);
          }
          else
          {
            DockOperations.ReplaceElement((ViewElement) bookmark, (ViewElement) view);
            view.IsSelected = true;
          }
        }
      }
    }

    private static void SnapToBookmark(ViewGroup group, WindowProfile windowProfile, ViewBookmarkType type, Action<View, WindowProfile, ViewBookmarkType> unresolvedBookmarkHandler)
    {
      using (new DockOperations.DockEventRaiser(new DockEventArgs(DockAction.SnapToBookmark, (ViewElement) group)))
      {
        List<ViewElement> list1 = new List<ViewElement>(group.FindAll((Predicate<ViewElement>) (e => e is View)));
        List<ViewElement> list2 = new List<ViewElement>();
        using (LayoutSynchronizer.BeginLayoutSynchronization())
        {
          foreach (View view in list1)
          {
            if (view.IsSelected)
              list2.Add((ViewElement) view);
            else
              DockOperations.SnapToBookmark(view, windowProfile, type, unresolvedBookmarkHandler);
          }
          foreach (View view in list2)
            DockOperations.SnapToBookmark(view, windowProfile, type, unresolvedBookmarkHandler);
        }
      }
    }

    private static void SnapToBookmark(ViewElement element, WindowProfile windowProfile, ViewBookmarkType type, Action<View, WindowProfile, ViewBookmarkType> unresolvedBookmarkHandler)
    {
      View view = element as View;
      if (view != null)
      {
        DockOperations.SnapToBookmark(view, windowProfile, type, unresolvedBookmarkHandler);
      }
      else
      {
        ViewGroup group = element as ViewGroup;
        if (group == null)
          throw new InvalidOperationException("Only TabGroup and View elements are supported for bookmark replacement.");
        DockOperations.SnapToBookmark(group, windowProfile, type, unresolvedBookmarkHandler);
      }
    }

    public static ViewBookmark FindBookmark(View view, WindowProfile windowProfile)
    {
      return DockOperations.FindBookmark(view.Name, windowProfile);
    }

    public static ViewBookmark FindBookmark(string name, WindowProfile windowProfile)
    {
      return DockOperations.FindBookmark(name, windowProfile, ViewBookmarkType.All);
    }

    public static ViewBookmark FindBookmark(View view, WindowProfile windowProfile, ViewBookmarkType type)
    {
      return DockOperations.FindBookmark(view.Name, windowProfile, type);
    }

    public static ViewBookmark FindBookmark(string name, WindowProfile windowProfile, ViewBookmarkType type)
    {
      ViewBookmark viewBookmark = (ViewBookmark) null;
      List<ViewBookmark> list = new List<ViewBookmark>(DockOperations.FindBookmarks(name, windowProfile, type));
      if (list.Count > 0)
        viewBookmark = list[0];
      return viewBookmark;
    }

    public static IEnumerable<ViewBookmark> FindBookmarks(View view, WindowProfile profile, ViewBookmarkType type)
    {
      return DockOperations.FindBookmarks(view.Name, profile, type);
    }

    public static IEnumerable<ViewBookmark> FindBookmarks(string name, WindowProfile profile, ViewBookmarkType type)
    {
      List<ViewBookmark> list = new List<ViewBookmark>();
      ViewBookmark viewBookmark;
      foreach (ViewBookmark viewBookmark1 in profile.FindAll((Predicate<ViewElement>) (e =>
      {
        if ((viewBookmark = e as ViewBookmark) == null || !(viewBookmark.Name == name))
          return false;
        if (viewBookmark.ViewBookmarkType != type)
          return ViewBookmarkType.All == type;
        return true;
      })))
        list.Add(viewBookmark1);
      list.Sort(new Comparison<ViewBookmark>(DockOperations.CompareBookmarksByAccessOrder));
      return (IEnumerable<ViewBookmark>) list;
    }

    public static void ClearBookmark(View view, WindowProfile windowProfile, ViewBookmarkType type)
    {
      foreach (ViewElement viewElement in DockOperations.FindBookmarks(view.Name, windowProfile, type))
        viewElement.Detach();
    }

    internal static void ClearViewBookmarks(List<ViewElement> elements)
    {
      if (elements == null)
        return;
      foreach (ViewElement viewElement in elements)
      {
        ViewElement element = (ViewElement) viewElement.Parent;
        if (element == null)
          throw new InvalidOperationException("dockingView should have a parent.");
        if (!FloatSite.IsFloating(element))
        {
          WindowProfile windowProfile = element.WindowProfile;
          ViewBookmarkType bookmarkType = ExtensionMethods.GetBookmarkType(element);
          View view = viewElement as View;
          if (view != null)
            DockOperations.ClearBookmark(view, windowProfile, bookmarkType);
        }
      }
    }

    private static bool IsBookmarkable(ViewElement element)
    {
      if (!(element is View) && !(element is TabGroup) || !(ViewElement.FindRootElement(element) is MainSite))
        return false;
      if (!(element.Parent is DockGroup) && !(element.Parent is TabGroup))
        return element.Parent is DocumentGroup;
      return true;
    }

    public static void RenumberBookmarkAccessOrder(string name, WindowProfile profile)
    {
      List<ViewBookmark> list = new List<ViewBookmark>(DockOperations.FindBookmarks(name, profile, ViewBookmarkType.All));
      int num = 1;
      foreach (ViewBookmark viewBookmark in list)
      {
        viewBookmark.AccessOrder = num;
        ++num;
      }
    }

    private static int CompareBookmarksByAccessOrder(ViewBookmark bookmark1, ViewBookmark bookmark2)
    {
      return bookmark1.AccessOrder.CompareTo(bookmark2.AccessOrder);
    }

    private static void CalculateFloatingPosition(ViewElement element, WindowProfile profile, out double floatLeft, out double floatTop)
    {
      floatLeft = element.FloatingLeft;
      floatTop = element.FloatingTop;
      if (!Microsoft.VisualStudio.PlatformUI.ExtensionMethods.IsNearlyEqual(floatLeft, double.NaN) && !Microsoft.VisualStudio.PlatformUI.ExtensionMethods.IsNearlyEqual(floatTop, double.NaN))
        return;
      Rect screenSubRect;
      Rect monitorRect;
      NativeMethods.FindMaximumSingleMonitorRectangle(DpiHelper.GetDeviceRect(Application.Current.MainWindow), out screenSubRect, out monitorRect);
      Rect rect = DpiHelper.DeviceToLogicalUnits(monitorRect);
      for (int index = 0; (double) index < 100.0 && DockOperations.IsFloatSiteAtPosition(profile, DockOperations.currentFloatLeft, DockOperations.currentFloatTop); ++index)
      {
        DockOperations.currentFloatLeft += 20.0;
        DockOperations.currentFloatTop += 20.0;
      }
      if (element.FloatingWidth / 3.0 + DockOperations.currentFloatLeft > rect.Right)
        DockOperations.currentFloatLeft = 0.0;
      if (element.FloatingHeight / 3.0 + DockOperations.currentFloatTop > rect.Bottom)
        DockOperations.currentFloatTop = 0.0;
      if (DockOperations.currentFloatLeft < rect.Left)
        DockOperations.currentFloatLeft = rect.Left;
      if (DockOperations.currentFloatTop < rect.Top)
        DockOperations.currentFloatTop = rect.Top;
      floatLeft = DockOperations.currentFloatLeft;
      floatTop = DockOperations.currentFloatTop;
    }

    private static bool IsFloatSiteAtPosition(WindowProfile profile, double floatLeft, double floatTop)
    {
      foreach (FloatSite floatSite in new List<ViewElement>(profile.FindAll((Predicate<ViewElement>) (e => e is FloatSite))))
      {
        if (floatSite.IsVisible && Microsoft.VisualStudio.PlatformUI.ExtensionMethods.IsNearlyEqual(floatSite.FloatingLeft, floatLeft) && Microsoft.VisualStudio.PlatformUI.ExtensionMethods.IsNearlyEqual(floatSite.FloatingTop, floatTop))
          return true;
      }
      return false;
    }

    private static void CalculateUndockPosition(ViewElement element, Point cursorDevicePoint, Rect currentUndockingDeviceRect, bool isFloatingSizeSet)
    {
      double num1 = element.FloatingWidth;
      double num2 = element.FloatingHeight;
      Point point = DpiHelper.DeviceToLogicalUnits(cursorDevicePoint);
      Rect rect = DpiHelper.DeviceToLogicalUnits(currentUndockingDeviceRect);
      if (!isFloatingSizeSet && !rect.IsEmpty)
      {
        num1 = rect.Width;
        num2 = rect.Height;
      }
      double num3;
      double num4;
      if (!rect.IsEmpty)
      {
        num3 = Math.Max(rect.Left, point.X + 50.0 - num1);
        num4 = point.Y - 8.0;
      }
      else
      {
        num3 = point.X - num1 / 2.0;
        num4 = point.Y - 8.0;
      }
      element.FloatingLeft = num3;
      element.FloatingTop = num4;
      element.FloatingWidth = num1;
      element.FloatingHeight = num2;
    }

    private class DockEventRaiser : DisposableObject
    {
      private DockEventArgs EventArgs { get; set; }

      public DockEventRaiser(DockEventArgs args)
      {
        if (args == null)
          throw new ArgumentNullException("args");
        this.EventArgs = args;
        Microsoft.VisualStudio.PlatformUI.ExtensionMethods.RaiseEvent<DockEventArgs>(DockOperations.DockPositionChanging, (object) null, args);
      }

      protected override void DisposeManagedResources()
      {
        Microsoft.VisualStudio.PlatformUI.ExtensionMethods.RaiseEvent<DockEventArgs>(DockOperations.DockPositionChanged, (object) null, this.EventArgs);
      }
    }

    private sealed class PreventCollapseScope : DisposableObject
    {
      private IEnumerable<IDisposable> nestedDisposables;

      public PreventCollapseScope(IEnumerable<ViewElement> elements)
      {
        List<IDisposable> list = new List<IDisposable>();
        foreach (ViewElement viewElement in elements)
          list.Add(viewElement.PreventCollapse());
        this.nestedDisposables = (IEnumerable<IDisposable>) list;
      }

      protected override void DisposeManagedResources()
      {
        foreach (IDisposable disposable in this.nestedDisposables)
          disposable.Dispose();
      }
    }
  }
}
