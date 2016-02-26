// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.ExtensionMethods
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Windows;

namespace Microsoft.VisualStudio.PlatformUI.Shell
{
  public static class ExtensionMethods
  {
    internal static TAncestorType FindAncestor<TAncestorType>(this ViewElement obj) where TAncestorType : ViewElement
    {
      return Microsoft.VisualStudio.PlatformUI.ExtensionMethods.FindAncestor<TAncestorType, ViewElement>(obj, (Func<ViewElement, ViewElement>) (e => (ViewElement) e.Parent));
    }

    internal static bool IsAncestorOf(this ViewElement viewElement, ViewElement otherElement)
    {
      return Microsoft.VisualStudio.PlatformUI.ExtensionMethods.IsAncestorOf<ViewElement>(viewElement, otherElement, (Func<ViewElement, ViewElement>) (e => (ViewElement) e.Parent));
    }

    internal static ViewBookmarkType GetBookmarkType(this ViewElement element)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      ViewBookmarkType viewBookmarkType = ViewBookmarkType.Default;
      if (element is DocumentGroup || element is DocumentGroupContainer || element.Parent != null && element.Parent is DocumentGroup)
        viewBookmarkType = ViewBookmarkType.DocumentWell;
      return viewBookmarkType;
    }

    internal static void InsertNewParent(this ViewElement oldView, ViewGroup parent)
    {
      ViewGroup parent1 = oldView.Parent;
      if (parent1 != null)
      {
        int index = parent1.Children.IndexOf(oldView);
        parent.IsVisible = oldView.IsVisible;
        parent1.Children[index] = (ViewElement) parent;
      }
      parent.Children.Add(oldView);
    }

    internal static string GetAutomationPeerCaption(this ViewElement viewElement)
    {
      string str = string.Empty;
      View view1 = viewElement as View;
      if (view1 != null)
      {
        str = view1.Title.ToString();
      }
      else
      {
        TabGroup tabGroup = viewElement as TabGroup;
        if (tabGroup != null)
        {
          View view2 = tabGroup.SelectedElement as View;
          if (view2 != null)
            str = view2.Title.ToString();
        }
        else
        {
          DockGroup dockGroup = viewElement as DockGroup;
          if (dockGroup != null && dockGroup.VisibleChildren.Count == 1)
            str = ExtensionMethods.GetAutomationPeerCaption(dockGroup.VisibleChildren[0]);
        }
      }
      return str ?? string.Empty;
    }

    public static void SetDeviceFloatingLeft(this ViewElement element, double deviceFloatingLeft)
    {
      element.FloatingLeft = deviceFloatingLeft * DpiHelper.DeviceToLogicalUnitsScalingFactorX;
    }

    public static double GetDeviceFloatingLeft(this ViewElement element)
    {
      return element.FloatingLeft * DpiHelper.LogicalToDeviceUnitsScalingFactorX;
    }

    public static void SetDeviceFloatingTop(this ViewElement element, double deviceFloatingTop)
    {
      element.FloatingTop = deviceFloatingTop * DpiHelper.DeviceToLogicalUnitsScalingFactorY;
    }

    public static double GetDeviceFloatingTop(this ViewElement element)
    {
      return element.FloatingTop * DpiHelper.LogicalToDeviceUnitsScalingFactorY;
    }

    public static void SetDeviceFloatingWidth(this ViewElement element, double deviceFloatingWidth)
    {
      element.FloatingWidth = deviceFloatingWidth * DpiHelper.DeviceToLogicalUnitsScalingFactorX;
    }

    public static double GetDeviceFloatingWidth(this ViewElement element)
    {
      return element.FloatingWidth * DpiHelper.LogicalToDeviceUnitsScalingFactorX;
    }

    public static void SetDeviceFloatingHeight(this ViewElement element, double deviceFloatingHeight)
    {
      element.FloatingHeight = deviceFloatingHeight * DpiHelper.DeviceToLogicalUnitsScalingFactorY;
    }

    public static double GetDeviceFloatingHeight(this ViewElement element)
    {
      return element.FloatingHeight * DpiHelper.LogicalToDeviceUnitsScalingFactorY;
    }

    public static bool IsConnectedToPresentationSource(this DependencyObject obj)
    {
      return PresentationSource.FromDependencyObject(obj) != null;
    }

    public static Rect Resize(this Rect rect, Vector positionChangeDelta, Vector sizeChangeDelta, Size minSize, Size maxSize)
    {
      double width = Math.Min(Math.Max(minSize.Width, rect.Width + sizeChangeDelta.X), maxSize.Width);
      double height = Math.Min(Math.Max(minSize.Height, rect.Height + sizeChangeDelta.Y), maxSize.Height);
      double right = rect.Right;
      double bottom = rect.Bottom;
      return new Rect(Math.Min(right - minSize.Width, Math.Max(right - maxSize.Width, rect.Left + positionChangeDelta.X)), Math.Min(bottom - minSize.Height, Math.Max(bottom - maxSize.Height, rect.Top + positionChangeDelta.Y)), width, height);
    }
  }
}
