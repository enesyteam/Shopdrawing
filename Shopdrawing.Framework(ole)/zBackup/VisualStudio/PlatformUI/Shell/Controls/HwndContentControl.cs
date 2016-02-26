// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.Controls.HwndContentControl
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
  [DefaultProperty("Content")]
  [System.Windows.Markup.ContentProperty("Content")]
  public class HwndContentControl : FocusableHwndHost
  {
    public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof (object), typeof (HwndContentControl), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(HwndContentControl.OnContentChanged)));

    public HwndSource HwndSource { get; private set; }

    protected ContentPresenter HwndSourcePresenter { get; private set; }

    public object Content
    {
      get
      {
        return this.GetValue(HwndContentControl.ContentProperty);
      }
      set
      {
        this.SetValue(HwndContentControl.ContentProperty, value);
      }
    }

    public HwndContentControl()
    {
      this.HwndSourcePresenter = new ContentPresenter();
    }

    protected override HandleRef BuildWindowCore(HandleRef hwndParent)
    {
      this.HwndSource = this.CreateHwndSource(hwndParent);
      return new HandleRef((object) this, this.HwndSource.Handle);
    }

    protected override void DestroyWindowCore(HandleRef hwnd)
    {
      if (this.HwndSource == null)
        return;
      this.HwndSource.Dispose();
      this.HwndSource = (HwndSource) null;
    }

    private static void OnContentChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      HwndContentControl hwndContentControl = (HwndContentControl) obj;
      hwndContentControl.HwndSourcePresenter.Content = args.NewValue;
      LayoutSynchronizer.Update((Visual) hwndContentControl.HwndSourcePresenter);
    }

    protected override void RegisterSourceKeyboardInputSink(IKeyboardInputSink sourceKeyboardInputSink)
    {
    }

    protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
      if (msg != 61)
        return base.WndProc(hwnd, msg, wParam, lParam, ref handled);
      handled = true;
      return IntPtr.Zero;
    }

    private HwndSource CreateHwndSource(HandleRef parent)
    {
      HwndSource hwndSource = new HwndSource(new HwndSourceParameters()
      {
        Width = 0,
        Height = 0,
        WindowStyle = 1174405120,
        ParentWindow = parent.Handle
      });
      hwndSource.RootVisual = (Visual) this.HwndSourcePresenter;
      this.AddLogicalChild((object) this.HwndSourcePresenter);
      Microsoft.VisualStudio.PlatformUI.NativeMethods.BringWindowToTop(hwndSource.Handle);
      return hwndSource;
    }

    protected override AutomationPeer OnCreateAutomationPeer()
    {
      return (AutomationPeer) new HwndContentControlAutomationPeer(this, (Visual) this.HwndSourcePresenter);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.HwndSource != null)
        this.HwndSource.Dispose();
      base.Dispose(disposing);
    }
  }
}
