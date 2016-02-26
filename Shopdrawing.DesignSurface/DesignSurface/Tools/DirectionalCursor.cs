// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.DirectionalCursor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.Tools
{
  public class DirectionalCursor
  {
    private Cursor[] cursors;
    private double repeatAngle;

    public DirectionalCursor(string resourceFormat, int count, double repeatAngle)
    {
      if (string.IsNullOrEmpty(resourceFormat))
        throw new ArgumentNullException("resourceFormat");
      if (count < 1 || (double) count > repeatAngle)
        throw new ArgumentOutOfRangeException("count");
      if (repeatAngle <= 0.0 || repeatAngle > 360.0)
        throw new ArgumentOutOfRangeException("repeatAngle");
      this.cursors = new Cursor[count];
      for (int index = 1; index <= count; ++index)
      {
        string name = string.Format((IFormatProvider) CultureInfo.InvariantCulture, resourceFormat, new object[1]
        {
          (object) index
        });
        this.cursors[index - 1] = FileTable.GetCursor(name);
      }
      this.repeatAngle = repeatAngle;
    }

    public Cursor GetCursor(Vector direction)
    {
      double num1 = Math.Atan2(direction.Y, direction.X) * 180.0 / Math.PI;
      if (num1 < 0.0)
        num1 += 360.0;
      double num2 = this.repeatAngle / (double) this.cursors.Length;
      return this.cursors[(int) Math.Round(num1 / num2) % this.cursors.Length];
    }
  }
}
