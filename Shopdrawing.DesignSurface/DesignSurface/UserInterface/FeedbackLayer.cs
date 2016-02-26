// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.FeedbackLayer
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  public class FeedbackLayer : Canvas
  {
    private DrawingVisual drawingVisual;
    private VisualCollection childrenVisuals;

    protected override int VisualChildrenCount
    {
      get
      {
        return this.childrenVisuals.Count;
      }
    }

    public FeedbackLayer()
    {
      this.childrenVisuals = new VisualCollection((Visual) this);
      this.drawingVisual = new DrawingVisual();
      this.childrenVisuals.Add((Visual) this.drawingVisual);
    }

    protected override Visual GetVisualChild(int index)
    {
      return this.childrenVisuals[index];
    }

    public new DrawingContext RenderOpen()
    {
      return this.drawingVisual.RenderOpen();
    }

    public void Clear()
    {
      this.RenderOpen().Close();
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      this.childrenVisuals.Remove((Visual) this.drawingVisual);
      Size size = base.MeasureOverride(availableSize);
      this.childrenVisuals.Add((Visual) this.drawingVisual);
      return size;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
      this.childrenVisuals.Remove((Visual) this.drawingVisual);
      Size size = base.ArrangeOverride(finalSize);
      this.childrenVisuals.Add((Visual) this.drawingVisual);
      return size;
    }
  }
}
