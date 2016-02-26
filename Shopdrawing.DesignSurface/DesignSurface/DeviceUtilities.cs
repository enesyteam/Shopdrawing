// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.DeviceUtilities
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface
{
  public static class DeviceUtilities
  {
    public static Vector Dpi
    {
      get
      {
        Vector scaleToDevice = DeviceUtilities.ScaleToDevice;
        return new Vector(scaleToDevice.X * 96.0, scaleToDevice.Y * 96.0);
      }
    }

    public static Vector ScaleToDevice
    {
      get
      {
        Window mainWindow = Application.Current.MainWindow;
        if (mainWindow != null)
        {
          PresentationSource presentationSource = PresentationSource.FromVisual((Visual) mainWindow);
          if (presentationSource != null)
          {
            Matrix transformToDevice = presentationSource.CompositionTarget.TransformToDevice;
            return new Vector(transformToDevice.M11, transformToDevice.M22);
          }
        }
        return new Vector(1.0, 1.0);
      }
    }

    public static Size GetDevicePixelSize(double scale)
    {
      Vector scaleToDevice = DeviceUtilities.ScaleToDevice;
      return new Size(1.0 / (scaleToDevice.X * scale), 1.0 / (scaleToDevice.Y * scale));
    }
  }
}
