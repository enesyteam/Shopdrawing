// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Diagnostics.FocusTracker
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Media;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.Framework.Diagnostics
{
  public static class FocusTracker
  {
    private static bool hasClassHandler;
    private static bool isTracking;

    public static bool IsTracking
    {
      get
      {
        return FocusTracker.isTracking;
      }
      set
      {
        FocusTracker.isTracking = value;
        if (!value || FocusTracker.hasClassHandler)
          return;
        EventManager.RegisterClassHandler(typeof (UIElement), UIElement.LostKeyboardFocusEvent, (Delegate) new KeyboardFocusChangedEventHandler(FocusTracker.OnKeyboardFocusChanged), true);
        FocusTracker.hasClassHandler = true;
      }
    }

    private static void OnKeyboardFocusChanged(object sender, KeyboardFocusChangedEventArgs args)
    {
      if (!FocusTracker.isTracking || args.NewFocus == null || (args.OldFocus == null || args.NewFocus == args.OldFocus) || sender != args.OldFocus)
        return;
      SystemSounds.Exclamation.Play();
      for (DependencyObject dependencyObject = args.NewFocus as DependencyObject; dependencyObject != null; dependencyObject = VisualTreeHelper.GetParent(dependencyObject))
      {
        string.IsNullOrEmpty(AutomationElement.GetId(dependencyObject));
        FrameworkElement frameworkElement = dependencyObject as FrameworkElement;
      }
    }
  }
}
