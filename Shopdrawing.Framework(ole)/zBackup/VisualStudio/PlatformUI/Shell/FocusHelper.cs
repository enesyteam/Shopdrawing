// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.FocusHelper
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace Microsoft.VisualStudio.PlatformUI.Shell
{
  public static class FocusHelper
  {
    public static void MoveFocusInto(UIElement element)
    {
      if (element.IsKeyboardFocusWithin)
        return;
      HwndSource source = (HwndSource) PresentationSource.FromVisual((Visual) element);
      if (source != null)
        FocusHelper.SetRestoreFocusFields(source, (object) null, IntPtr.Zero);
      element.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
    }

    internal static void SetRestoreFocusFields(HwndSource source, object restoreFocus, IntPtr restoreFocusWindow)
    {
      object fieldValue = FocusHelper.GetFieldValue(FocusHelper.GetFieldValue((object) source, "_keyboard"), "_value");
      if (fieldValue == null)
        return;
      FocusHelper.SetFieldValue(fieldValue, "_restoreFocusWindow", (object) restoreFocusWindow);
      FocusHelper.SetFieldValue(fieldValue, "_restoreFocus", restoreFocus);
    }

    internal static void ClearRestoreFocusWindowIf(HwndSource source, Predicate<IntPtr> predicate)
    {
      object fieldValue1 = FocusHelper.GetFieldValue(FocusHelper.GetFieldValue((object) source, "_keyboard"), "_value");
      if (fieldValue1 == null)
        return;
      object fieldValue2 = FocusHelper.GetFieldValue(fieldValue1, "_restoreFocusWindow");
      if (!(fieldValue2 is IntPtr))
        return;
      IntPtr num = (IntPtr) fieldValue2;
      if (!(num != IntPtr.Zero) || !predicate(num))
        return;
      FocusHelper.SetFieldValue(fieldValue1, "_restoreFocusWindow", (object) IntPtr.Zero);
    }

    private static object GetFieldValue(object source, string fieldName)
    {
      if (source == null)
        return (object) null;
      FieldInfo field = source.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
      if (field == (FieldInfo) null)
        return (object) null;
      return field.GetValue(source);
    }

    private static void SetFieldValue(object source, string fieldName, object value)
    {
      if (source == null)
        return;
      FieldInfo field = source.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
      if (field == (FieldInfo) null)
        return;
      try
      {
        field.SetValue(source, value);
      }
      catch (Exception ex)
      {
      }
    }
  }
}
