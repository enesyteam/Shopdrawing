// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.ExtensionMethods
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.VisualStudio.PlatformUI
{
  public static class ExtensionMethods
  {
    public static void ThrowIfNullOrEmpty(this string value, string message)
    {
      if (value == null)
        throw new ArgumentNullException(message);
      if (string.IsNullOrEmpty(value))
        throw new ArgumentException(message);
    }

    public static void CopyTo(this Stream sourceStream, Stream targetStream)
    {
      byte[] buffer = new byte[4096];
      int count;
      while ((count = sourceStream.Read(buffer, 0, 4096)) > 0)
        targetStream.Write(buffer, 0, count);
    }

    public static DependencyObject GetVisualOrLogicalParent(this DependencyObject sourceElement)
    {
      return VisualTreeHelper.GetParent(sourceElement) ?? LogicalTreeHelper.GetParent(sourceElement);
    }

    public static TAncestorType FindAncestorOrSelf<TAncestorType>(this Visual obj) where TAncestorType : Visual
    {
      TAncestorType ancestorType = obj as TAncestorType;
      if ((object) ancestorType != null)
        return ancestorType;
      return ExtensionMethods.FindAncestor<TAncestorType, DependencyObject>((DependencyObject) obj, new Func<DependencyObject, DependencyObject>(ExtensionMethods.GetVisualOrLogicalParent));
    }

    public static TAncestorType FindAncestor<TAncestorType>(this Visual obj) where TAncestorType : Visual
    {
      return ExtensionMethods.FindAncestor<TAncestorType, DependencyObject>((DependencyObject) obj, new Func<DependencyObject, DependencyObject>(ExtensionMethods.GetVisualOrLogicalParent));
    }

    public static bool IsAncestorOf<TElementType>(this TElementType element, TElementType other, Func<TElementType, TElementType> parentEvaluator) where TElementType : class
    {
      for (TElementType elementType = parentEvaluator(other); (object) elementType != null; elementType = parentEvaluator(elementType))
      {
        if ((object) elementType == (object) element)
          return true;
      }
      return false;
    }

    public static TAncestorType FindAncestor<TAncestorType, TElementType>(this TElementType obj, Func<TElementType, TElementType> parentEvaluator) where TAncestorType : class
    {
      for (TElementType elementType = parentEvaluator(obj); (object) elementType != null; elementType = parentEvaluator(elementType))
      {
        TAncestorType ancestorType = (object) elementType as TAncestorType;
        if ((object) ancestorType != null)
          return ancestorType;
      }
      return default (TAncestorType);
    }

    public static T FindDescendant<T>(this DependencyObject obj) where T : class
    {
      T obj1 = default (T);
      for (int childIndex = 0; childIndex < VisualTreeHelper.GetChildrenCount(obj); ++childIndex)
      {
        DependencyObject child = VisualTreeHelper.GetChild(obj, childIndex);
        obj1 = child as T;
        if ((object) obj1 == null)
        {
          obj1 = ExtensionMethods.FindDescendant<T>(child);
          if ((object) obj1 != null)
            break;
        }
        else
          break;
      }
      return obj1;
    }

    public static DependencyObject FindCommonAncestor(this DependencyObject obj1, DependencyObject obj2)
    {
      return ExtensionMethods.FindCommonAncestor<DependencyObject>(obj1, obj2, new Func<DependencyObject, DependencyObject>(ExtensionMethods.GetVisualOrLogicalParent));
    }

    public static T FindCommonAncestor<T>(this T obj1, T obj2, Func<T, T> parentEvaluator) where T : DependencyObject
    {
      if ((object) obj1 == null || (object) obj2 == null)
        return default (T);
      HashSet<T> hashSet = new HashSet<T>();
      for (obj1 = parentEvaluator(obj1); (object) obj1 != null; obj1 = parentEvaluator(obj1))
        hashSet.Add(obj1);
      for (obj2 = parentEvaluator(obj2); (object) obj2 != null; obj2 = parentEvaluator(obj2))
      {
        if (hashSet.Contains(obj2))
          return obj2;
      }
      return default (T);
    }

    private static bool IsAncestorOf(IntPtr thisWindow, IntPtr otherWindow)
    {
      while (otherWindow != IntPtr.Zero)
      {
        otherWindow = NativeMethods.GetWindow(otherWindow, 4);
        if (otherWindow == thisWindow)
          return true;
      }
      return false;
    }

    public static bool IsTopmost(IntPtr hwnd)
    {
      WINDOWINFO pwi = new WINDOWINFO();
      pwi.cbSize = Marshal.SizeOf((object) pwi);
      if (!NativeMethods.GetWindowInfo(hwnd, ref pwi))
        return false;
      return (pwi.dwExStyle & 8) == 8;
    }

    public static bool IsBelow(IntPtr thisHwnd, IntPtr otherHwnd)
    {
      if (!NativeMethods.IsWindowVisible(thisHwnd))
        return true;
      if (!NativeMethods.IsWindowVisible(otherHwnd))
        return false;
      bool flag1 = ExtensionMethods.IsTopmost(thisHwnd);
      bool flag2 = ExtensionMethods.IsTopmost(otherHwnd);
      if (flag1 && !flag2)
        return false;
      if (!flag1 && flag2 || ExtensionMethods.IsAncestorOf(thisHwnd, otherHwnd))
        return true;
      if (ExtensionMethods.IsAncestorOf(otherHwnd, thisHwnd))
        return false;
      while (true)
      {
        IntPtr window1 = NativeMethods.GetWindow(thisHwnd, 4);
        IntPtr window2 = NativeMethods.GetWindow(otherHwnd, 4);
        if (!(window1 == window2))
        {
          thisHwnd = window1 == IntPtr.Zero ? thisHwnd : window1;
          otherHwnd = window2 == IntPtr.Zero ? otherHwnd : window2;
        }
        else
          break;
      }
      while (otherHwnd != IntPtr.Zero)
      {
        otherHwnd = NativeMethods.GetWindow(otherHwnd, 2);
        if (otherHwnd == thisHwnd)
          return true;
      }
      return false;
    }

    public static void RaiseEvent<TEventArgs>(this EventHandler<TEventArgs> eventHandler, object source, TEventArgs args) where TEventArgs : EventArgs
    {
      if (eventHandler == null)
        return;
      eventHandler(source, args);
    }

    public static void RaiseEvent(this EventHandler eventHandler, object source)
    {
      ExtensionMethods.RaiseEvent(eventHandler, source, EventArgs.Empty);
    }

    public static void RaiseEvent(this EventHandler eventHandler, object source, EventArgs args)
    {
      if (eventHandler == null)
        return;
      eventHandler(source, args);
    }

    public static void RaiseEvent(this CancelEventHandler eventHandler, object source, CancelEventArgs args)
    {
      if (eventHandler == null)
        return;
      eventHandler(source, args);
    }

    public static bool IsNonreal(this double value)
    {
      if (!double.IsNaN(value))
        return double.IsInfinity(value);
      return true;
    }

    public static bool IsNearlyEqual(this double value1, double value2)
    {
      if (ExtensionMethods.IsNonreal(value1) || ExtensionMethods.IsNonreal(value2))
        return value1.CompareTo(value2) == 0;
      return Math.Abs(value1 - value2) < 1E-05;
    }
  }
}
