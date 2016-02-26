// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.ControlPointEditor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class ControlPointEditor
  {
    public double CenterX { get; set; }

    public double CenterY { get; set; }

    public double Radius { get; set; }

    public ControlPointEditor()
    {
      this.Radius = 4.0;
    }

    public bool HitTest(double x, double y)
    {
      if (x > this.CenterX - this.Radius && x < this.CenterX + this.Radius && y > this.CenterY - this.Radius)
        return y < this.CenterY + this.Radius;
      return false;
    }

    public void OnRender(DrawingContext dc, EaseCurveEditor editor)
    {
      if (editor.ControlPointBrush == null)
        return;
      dc.DrawEllipse(editor.ControlPointBrush, (Pen) null, new Point(this.CenterX - 0.5, this.CenterY - 0.5), this.Radius, this.Radius);
    }
  }
}
