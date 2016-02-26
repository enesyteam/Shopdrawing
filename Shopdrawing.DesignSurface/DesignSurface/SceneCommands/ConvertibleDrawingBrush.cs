// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.ConvertibleDrawingBrush
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal sealed class ConvertibleDrawingBrush : ConvertibleDrawing
  {
    private DrawingBrush startDrawingBrush;

    public ConvertibleDrawingBrush(DrawingBrush drawingBrush, string projectPath)
      : base(drawingBrush.Drawing, projectPath)
    {
      this.startDrawingBrush = drawingBrush;
    }

    protected override Rect GetDrawingBounds()
    {
      if (this.startDrawingBrush.ViewboxUnits == BrushMappingMode.Absolute)
        return this.startDrawingBrush.Viewbox;
      return base.GetDrawingBounds();
    }

    protected override void PrepareCanvas(Canvas canvas)
    {
      base.PrepareCanvas(canvas);
      canvas.Opacity = this.startDrawingBrush.Opacity;
      canvas.RenderTransform = this.startDrawingBrush.Transform;
    }
  }
}
