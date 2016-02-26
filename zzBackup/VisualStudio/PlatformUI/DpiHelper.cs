// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.DpiHelper
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace Microsoft.VisualStudio.PlatformUI
{
  public static class DpiHelper
  {
    private const double LogicalDpi = 96.0;
    private static Matrix transformFromDevice;
    private static Matrix transformToDevice;

    public static Matrix TransformFromDevice
    {
      get
      {
        return DpiHelper.transformFromDevice;
      }
    }

    public static Matrix TransformToDevice
    {
      get
      {
        return DpiHelper.transformToDevice;
      }
    }

    public static double DeviceDpiX { get; private set; }

    public static double DeviceDpiY { get; private set; }

    public static double DeviceToLogicalUnitsScalingFactorX
    {
      get
      {
        return DpiHelper.TransformFromDevice.M11;
      }
    }

    public static double DeviceToLogicalUnitsScalingFactorY
    {
      get
      {
        return DpiHelper.TransformFromDevice.M22;
      }
    }

    public static double LogicalToDeviceUnitsScalingFactorX
    {
      get
      {
        return DpiHelper.TransformToDevice.M11;
      }
    }

    public static double LogicalToDeviceUnitsScalingFactorY
    {
      get
      {
        return DpiHelper.TransformToDevice.M22;
      }
    }

    static DpiHelper()
    {
      IntPtr dc = NativeMethods.GetDC(IntPtr.Zero);
      if (dc != IntPtr.Zero)
      {
        DpiHelper.DeviceDpiX = (double) NativeMethods.GetDeviceCaps(dc, 88);
        DpiHelper.DeviceDpiY = (double) NativeMethods.GetDeviceCaps(dc, 90);
        NativeMethods.ReleaseDC(IntPtr.Zero, dc);
      }
      else
      {
        DpiHelper.DeviceDpiX = 96.0;
        DpiHelper.DeviceDpiY = 96.0;
      }
      DpiHelper.transformToDevice.Scale(DpiHelper.DeviceDpiX / 96.0, DpiHelper.DeviceDpiY / 96.0);
      DpiHelper.transformFromDevice.Scale(96.0 / DpiHelper.DeviceDpiX, 96.0 / DpiHelper.DeviceDpiY);
    }

    public static Point LogicalToDeviceUnits(this Point logicalPoint)
    {
      return DpiHelper.TransformToDevice.Transform(logicalPoint);
    }

    public static Rect LogicalToDeviceUnits(this Rect logicalRect)
    {
      Rect rect = logicalRect;
      rect.Transform(DpiHelper.TransformToDevice);
      return rect;
    }

    public static Size LogicalToDeviceUnits(this Size logicalSize)
    {
      return new Size(logicalSize.Width * DpiHelper.LogicalToDeviceUnitsScalingFactorX, logicalSize.Height * DpiHelper.LogicalToDeviceUnitsScalingFactorY);
    }

    public static Point DeviceToLogicalUnits(this Point devicePoint)
    {
      return DpiHelper.TransformFromDevice.Transform(devicePoint);
    }

    public static Rect DeviceToLogicalUnits(this Rect deviceRect)
    {
      Rect rect = deviceRect;
      rect.Transform(DpiHelper.TransformFromDevice);
      return rect;
    }

    public static Size DeviceToLogicalUnits(this Size deviceSize)
    {
      return new Size(deviceSize.Width * DpiHelper.DeviceToLogicalUnitsScalingFactorX, deviceSize.Height * DpiHelper.DeviceToLogicalUnitsScalingFactorY);
    }

    public static void SetDeviceLeft(this Window window, double deviceLeft)
    {
      window.Left = deviceLeft * DpiHelper.DeviceToLogicalUnitsScalingFactorX;
    }

    public static double GetDeviceLeft(this Window window)
    {
      return window.Left * DpiHelper.LogicalToDeviceUnitsScalingFactorX;
    }

    public static void SetDeviceTop(this Window window, double deviceTop)
    {
      window.Top = deviceTop * DpiHelper.DeviceToLogicalUnitsScalingFactorY;
    }

    public static double GetDeviceTop(this Window window)
    {
      return window.Top * DpiHelper.LogicalToDeviceUnitsScalingFactorY;
    }

    public static void SetDeviceWidth(this Window window, double deviceWidth)
    {
      window.Width = deviceWidth * DpiHelper.DeviceToLogicalUnitsScalingFactorX;
    }

    public static double GetDeviceWidth(this Window window)
    {
      return window.Width * DpiHelper.LogicalToDeviceUnitsScalingFactorX;
    }

    public static void SetDeviceHeight(this Window window, double deviceHeight)
    {
      window.Height = deviceHeight * DpiHelper.DeviceToLogicalUnitsScalingFactorY;
    }

    public static double GetDeviceHeight(this Window window)
    {
      return window.Height * DpiHelper.LogicalToDeviceUnitsScalingFactorY;
    }

    public static Rect GetDeviceRect(this Window window)
    {
      RECT lpRect;
      NativeMethods.GetWindowRect(new WindowInteropHelper(window).Handle, out lpRect);
      return new Rect(new Point((double) lpRect.Left, (double) lpRect.Top), new Size((double) lpRect.Width, (double) lpRect.Height));
    }

    public static Size GetDeviceActualSize(this FrameworkElement element)
    {
      return DpiHelper.LogicalToDeviceUnits(new Size(element.ActualWidth, element.ActualHeight));
    }
  }
}
