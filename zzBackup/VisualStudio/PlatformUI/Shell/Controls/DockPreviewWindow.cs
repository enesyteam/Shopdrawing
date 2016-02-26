// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.Controls.DockPreviewWindow
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
  public sealed class DockPreviewWindow : FrameworkElement, IDockPreviewWindow, IDisposable
  {
    public static readonly DependencyProperty LeftProperty = DependencyProperty.Register("Left", typeof (double), typeof (DockPreviewWindow), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(DockPreviewWindow.OnPositionChanged)));
    public static readonly DependencyProperty TopProperty = DependencyProperty.Register("Top", typeof (double), typeof (DockPreviewWindow), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(DockPreviewWindow.OnPositionChanged)));
    public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register("Background", typeof (Color), typeof (DockPreviewWindow), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(DockPreviewWindow.OnBackgroundChanged)));
    private DockPreviewWindow.DockPreviewHwndWrapper hwndWrapper;

    public Color Background
    {
      get
      {
        return (Color) this.GetValue(DockPreviewWindow.BackgroundProperty);
      }
      set
      {
        this.SetValue(DockPreviewWindow.BackgroundProperty, (object) value);
      }
    }

    public double Top
    {
      get
      {
        return (double) this.GetValue(DockPreviewWindow.TopProperty);
      }
      set
      {
        this.SetValue(DockPreviewWindow.TopProperty, (object) value);
      }
    }

    public double Left
    {
      get
      {
        return (double) this.GetValue(DockPreviewWindow.LeftProperty);
      }
      set
      {
        this.SetValue(DockPreviewWindow.LeftProperty, (object) value);
      }
    }

    private int DeviceLeft
    {
      get
      {
        return (int) (this.Left / DpiHelper.DeviceToLogicalUnitsScalingFactorX);
      }
    }

    private int DeviceTop
    {
      get
      {
        return (int) (this.Top / DpiHelper.DeviceToLogicalUnitsScalingFactorY);
      }
    }

    private int DeviceWidth
    {
      get
      {
        return (int) (this.Width / DpiHelper.DeviceToLogicalUnitsScalingFactorX);
      }
    }

    private int DeviceHeight
    {
      get
      {
        return (int) (this.Height / DpiHelper.DeviceToLogicalUnitsScalingFactorY);
      }
    }

    private bool IsWindowCreated
    {
      get
      {
        return this.hwndWrapper != null;
      }
    }

    static DockPreviewWindow()
    {
      FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (DockPreviewWindow), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (DockPreviewWindow)));
      FrameworkElement.WidthProperty.OverrideMetadata(typeof (DockPreviewWindow), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(DockPreviewWindow.OnPositionChanged)));
      FrameworkElement.HeightProperty.OverrideMetadata(typeof (DockPreviewWindow), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(DockPreviewWindow.OnPositionChanged)));
      UIElement.OpacityProperty.OverrideMetadata(typeof (DockPreviewWindow), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(DockPreviewWindow.OnOpacityChanged)));
    }

    private static void OnPositionChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      ((DockPreviewWindow) obj).UpdatePosition();
    }

    private static void OnBackgroundChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      ((DockPreviewWindow) obj).UpdateBackground();
    }

    private static void OnOpacityChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      ((DockPreviewWindow) obj).UpdateOpacity();
    }

    private void CreateWindow()
    {
      this.BeginInit();
      this.EndInit();
      this.hwndWrapper = new DockPreviewWindow.DockPreviewHwndWrapper(this);
      this.UpdateOpacity();
    }

    private void UpdateBackground()
    {
    }

    private void UpdateOpacity()
    {
      if (!this.IsWindowCreated)
        return;
      NativeMethods.SetLayeredWindowAttributes(this.hwndWrapper.Handle, 0, (byte) ((double) byte.MaxValue * this.Opacity), 2);
    }

    private void UpdatePosition()
    {
      if (!this.IsWindowCreated)
        return;
      NativeMethods.MoveWindow(this.hwndWrapper.Handle, this.DeviceLeft, this.DeviceTop, this.DeviceWidth, this.DeviceHeight, true);
    }

    public void Show()
    {
      if (!this.IsWindowCreated)
        this.CreateWindow();
      NativeMethods.ShowWindow(this.hwndWrapper.Handle, 8);
    }

    public void Hide()
    {
      NativeMethods.ShowWindow(this.hwndWrapper.Handle, 0);
    }

    public void Close()
    {
      NativeMethods.SendMessage(this.hwndWrapper.Handle, 16, IntPtr.Zero, IntPtr.Zero);
    }

    public void Dispose()
    {
      if (this.hwndWrapper == null)
        return;
      this.hwndWrapper.Dispose();
      this.hwndWrapper = (DockPreviewWindow.DockPreviewHwndWrapper) null;
    }

    private class DockPreviewHwndWrapper : HwndWrapper
    {
      private DockPreviewWindow Owner { get; set; }

      public DockPreviewHwndWrapper(DockPreviewWindow owner)
      {
        this.Owner = owner;
      }

      protected override IntPtr CreateWindowCore()
      {
        int dwStyle = -2046820352;
        int dwExStyle = 524296;
        IntPtr moduleHandle = NativeMethods.GetModuleHandle((string) null);
        return NativeMethods.CreateWindowEx(dwExStyle, new IntPtr((int) this.WindowClassAtom), (string) null, dwStyle, this.Owner.DeviceLeft, this.Owner.DeviceTop, this.Owner.DeviceWidth, this.Owner.DeviceHeight, HiddenHwndWrapper.Instance.Handle, IntPtr.Zero, moduleHandle, IntPtr.Zero);
      }

      protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam)
      {
        if (msg != 20)
          return base.WndProc(hwnd, msg, wParam, lParam);
        IntPtr solidBrush = NativeMethods.CreateSolidBrush(this.GetRGB(this.Owner.Background));
        RECT rect = new RECT()
        {
          Left = 0,
          Top = 0,
          Right = this.Owner.DeviceWidth,
          Bottom = this.Owner.DeviceHeight
        };
        NativeMethods.FillRect(wParam, ref rect, solidBrush);
        NativeMethods.DeleteObject(solidBrush);
        return new IntPtr(1);
      }

      private int GetRGB(Color color)
      {
        return (int) color.R | (int) color.G << 8 | (int) color.B << 16;
      }
    }
  }
}
