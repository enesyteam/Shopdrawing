// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Controls.ExpressionWindow
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.UserInterface;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace Microsoft.Expression.Framework.Controls
{
  public class ExpressionWindow : Window
  {
    public virtual bool IsOverridingWindowsChrome
    {
      get
      {
        return true;
      }
    }

    protected override void OnInitialized(EventArgs e)
    {
      if (this.IsOverridingWindowsChrome)
      {
        this.SetResourceReference(FrameworkElement.StyleProperty, (object) "WindowsChromeOverride");
        this.SourceInitialized += new EventHandler(this.ExpressionWindow_SourceInitialized);
      }
      base.OnInitialized(e);
    }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      if (!this.IsOverridingWindowsChrome)
        return;
      FrameworkElement frameworkElement = (FrameworkElement) this.GetTemplateChild("Caption");
      if (frameworkElement != null)
        frameworkElement.MouseLeftButtonDown += new MouseButtonEventHandler(this.DoDragMove);
      Button button = (Button) this.GetTemplateChild("Close");
      if (button == null)
        return;
      button.Click += new RoutedEventHandler(this.Close);
    }

    private void DoDragMove(object sender, MouseButtonEventArgs e)
    {
      this.DragMove();
    }

    private void Close(object sender, EventArgs e)
    {
      this.Close();
    }

    private void ExpressionWindow_SourceInitialized(object sender, EventArgs e)
    {
      if (this.ResizeMode != ResizeMode.CanResize && this.ResizeMode != ResizeMode.CanResizeWithGrip)
        return;
      HwndSource hwndSource = PresentationSource.FromVisual((Visual) this) as HwndSource;
      int windowLong = Microsoft.Expression.Framework.UserInterface.UnsafeNativeMethods.GetWindowLong(hwndSource.Handle, -16);
      uint num = 4294770687U;
      Microsoft.Expression.Framework.UserInterface.UnsafeNativeMethods.SetWindowLong(hwndSource.Handle, -16, windowLong & (int)num);
    }
  }
}
