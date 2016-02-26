// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.SnapToGridRenderer
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  public sealed class SnapToGridRenderer : UIElement
  {
    private const double minGridSpacing = 8.0;
    private static Brush brushOne;
    private static Brush brushFive;
    private static Brush brushTen;
    private Artboard artboard;
    private DesignerContext designerContext;
    private Vector originOffset;
    private bool isOriginOffsetLocked;

    public Vector OriginOffset
    {
      get
      {
        return this.originOffset;
      }
    }

    public bool IsOriginOffsetLocked
    {
      get
      {
        return this.isOriginOffsetLocked;
      }
      set
      {
        this.isOriginOffsetLocked = value;
      }
    }

    static SnapToGridRenderer()
    {
      Brush brush = (Brush) new SolidColorBrush(Color.FromScRgb(1f, 0.125f, 0.125f, 0.125f));
      SnapToGridRenderer.brushOne = brush.Clone();
      SnapToGridRenderer.brushOne.Opacity = 0.1;
      SnapToGridRenderer.brushOne.Freeze();
      SnapToGridRenderer.brushFive = brush.Clone();
      SnapToGridRenderer.brushFive.Opacity = 0.25;
      SnapToGridRenderer.brushFive.Freeze();
      SnapToGridRenderer.brushTen = brush.Clone();
      SnapToGridRenderer.brushTen.Opacity = 0.5;
      SnapToGridRenderer.brushTen.Freeze();
    }

    public SnapToGridRenderer(Artboard artboard)
    {
      this.artboard = artboard;
    }

    internal void Attach(DesignerContext designerContext)
    {
      if (designerContext == null)
        return;
      this.designerContext = designerContext;
      this.designerContext.ArtboardOptionsChanged += new EventHandler(this.OnArtboardOptionsChanged);
      this.LayoutUpdated += new EventHandler(this.OnLayoutUpdated);
      this.InvalidateVisual();
    }

    public void Detach()
    {
      if (this.designerContext == null)
        return;
      this.LayoutUpdated -= new EventHandler(this.OnLayoutUpdated);
      this.designerContext.ArtboardOptionsChanged -= new EventHandler(this.OnArtboardOptionsChanged);
      this.designerContext = (DesignerContext) null;
      this.artboard = (Artboard) null;
    }

    private void OnLayoutUpdated(object sender, EventArgs e)
    {
      if (this.IsOriginOffsetLocked)
        return;
      Vector vector = new Vector();
      if (this.artboard != null)
      {
        FrameworkElement editableContent = this.artboard.EditableContent;
        ContentControl contentControl = editableContent as ContentControl;
        if (contentControl != null)
        {
          Visual visual = contentControl.Content as Visual;
          if (visual != null && editableContent.IsAncestorOf((DependencyObject) visual))
          {
            Point point = new Point();
            try
            {
              vector = visual.TransformToAncestor((Visual) editableContent).Transform(point) - point;
            }
            catch (InvalidOperationException ex)
            {
            }
          }
        }
      }
      if (!(this.originOffset != vector))
        return;
      this.originOffset = vector;
      this.InvalidateVisual();
    }

    private void OnArtboardOptionsChanged(object sender, EventArgs e)
    {
      this.InvalidateVisual();
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
      if (this.artboard == null || this.designerContext == null)
        return;
      ArtboardOptionsModel artboardOptionsModel = this.designerContext.ArtboardOptionsModel;
      if (!artboardOptionsModel.ShowGrid)
        return;
      double num1 = artboardOptionsModel.GridSpacing * this.artboard.Zoom;
      while (num1 < 8.0)
      {
        num1 *= 5.0;
        if (num1 < 8.0)
          num1 *= 2.0;
      }
      Point point = this.artboard.TransformFromContentToArtboard(new Point(0.0, 0.0) + this.originOffset);
      Size renderSize = this.artboard.RenderSize;
      Vector rootToArtboardScale = this.artboard.ViewRootToArtboardScale;
      double num2 = num1 * rootToArtboardScale.X;
      int num3 = (int) Math.Floor((0.0 - point.X) / num2);
      int num4 = (int) Math.Ceiling((renderSize.Width - point.X) / num2);
      for (int index = num3; index <= num4; ++index)
      {
        Brush brush = index % 10 == 0 ? SnapToGridRenderer.brushTen : (index % 5 == 0 ? SnapToGridRenderer.brushFive : SnapToGridRenderer.brushOne);
        double x = point.X + (double) index * num2;
        drawingContext.DrawRectangle(brush, (Pen) null, new Rect(x, 0.0, 1.0, renderSize.Height));
      }
      double num5 = num1 * rootToArtboardScale.Y;
      int num6 = (int) Math.Floor((0.0 - point.Y) / num5);
      int num7 = (int) Math.Ceiling((renderSize.Height - point.Y) / num5);
      for (int index = num6; index <= num7; ++index)
      {
        Brush brush = index % 10 == 0 ? SnapToGridRenderer.brushTen : (index % 5 == 0 ? SnapToGridRenderer.brushFive : SnapToGridRenderer.brushOne);
        double y = point.Y + (double) index * num5;
        drawingContext.DrawRectangle(brush, (Pen) null, new Rect(0.0, y, renderSize.Width, 1.0));
      }
    }
  }
}
