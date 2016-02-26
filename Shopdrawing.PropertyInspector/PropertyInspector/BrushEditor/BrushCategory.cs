// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.BrushEditor.BrushCategory
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.BrushEditor
{
  internal class BrushCategory
  {
    public static readonly BrushCategory NullBrush = new BrushCategory("No color", PlatformTypes.Void);
    public static readonly BrushCategory SolidColor = new BrushCategory("Solid Color", PlatformTypes.SolidColorBrush);
    public static readonly BrushCategory Gradient = new BrushCategory("Gradient", PlatformTypes.GradientBrush);
    public static readonly BrushCategory LinearGradient = new BrushCategory("Linear Gradient", PlatformTypes.LinearGradientBrush);
    public static readonly BrushCategory RadialGradient = new BrushCategory("Radial Gradient", PlatformTypes.RadialGradientBrush);
    public static readonly BrushCategory Drawing = new BrushCategory("Drawing", PlatformTypes.DrawingBrush);
    public static readonly BrushCategory Image = new BrushCategory("Image", PlatformTypes.ImageBrush);
    public static readonly BrushCategory Visual = new BrushCategory("Visual", PlatformTypes.VisualBrush);
    public static readonly BrushCategory Video = new BrushCategory("Video", PlatformTypes.VideoBrush);
    public static readonly BrushCategory Tile = new BrushCategory("Tile", PlatformTypes.TileBrush);
    public static readonly BrushCategory Html = new BrushCategory("Html", PlatformTypes.WebBrowserBrush);
    private static Brush defaultNullBrush = (Brush) null;
    private static Brush defaultSolidColorBrush = (Brush) new SolidColorBrush(Colors.Black);
    private static Brush defaultLinearGradientBrush = (Brush) new LinearGradientBrush(Colors.Black, Colors.White, new Point(0.5, 0.0), new Point(0.5, 1.0));
    private static Brush defaultRadialGradientBrush = (Brush) new RadialGradientBrush(Colors.Black, Colors.White);
    private static Brush defaultDrawingBrush = (Brush) BrushCategory.CreateDefaultDrawingBrush();
    private static Brush defaultImageBrush = (Brush) new ImageBrush();
    private static Brush defaultVisualBrush = (Brush) BrushCategory.CreateDefaultVisualBrush();
    private static Brush defaultVideoBrush = (Brush) null;
    private static Brush defaultHtmlBrush = (Brush) null;
    private static Dictionary<BrushCategory, BrushCategory.LastBrushEntry> LastUsedBrush = new Dictionary<BrushCategory, BrushCategory.LastBrushEntry>();
    private string name;
    private ITypeId type;

    public string Name
    {
      get
      {
        return this.name;
      }
    }

    public ITypeId Type
    {
      get
      {
        return this.type;
      }
    }

    public Brush DefaultBrush
    {
      get
      {
        return (Brush) BrushCategory.GetLastUsed((DesignerContext) null, (IDocumentContext) null, this);
      }
    }

    static BrushCategory()
    {
      BrushCategory.ResetLastUsedBrushes();
    }

    private BrushCategory(string name, ITypeId type)
    {
      this.name = name;
      this.type = type;
    }

    public static object GetLastUsed(DesignerContext designerContext, IDocumentContext documentContext, BrushCategory brushCategory)
    {
      object obj = (object) null;
      BrushCategory.LastBrushEntry lastBrushEntry = (BrushCategory.LastBrushEntry) null;
      BrushCategory.LastUsedBrush.TryGetValue(brushCategory, out lastBrushEntry);
      if (lastBrushEntry != null)
        obj = lastBrushEntry.GetBrush(designerContext, documentContext);
      return obj;
    }

    public static void SetLastUsed(BrushCategory brushCategory, object brush)
    {
      BrushCategory.LastUsedBrush[brushCategory].SetBrush(brush);
      if (brushCategory == BrushCategory.LinearGradient || brushCategory == BrushCategory.RadialGradient)
      {
        BrushCategory.LastUsedBrush[BrushCategory.Gradient].SetBrush(brush);
      }
      else
      {
        if (brushCategory != BrushCategory.Drawing && brushCategory != BrushCategory.Visual && (brushCategory != BrushCategory.Image && brushCategory != BrushCategory.Video))
          return;
        BrushCategory.LastUsedBrush[BrushCategory.Tile].SetBrush(brush);
      }
    }

    public static void ResetLastUsedBrushes()
    {
      BrushCategory.LastUsedBrush[BrushCategory.NullBrush] = new BrushCategory.LastBrushEntry(BrushCategory.defaultNullBrush);
      BrushCategory.LastUsedBrush[BrushCategory.SolidColor] = new BrushCategory.LastBrushEntry(BrushCategory.defaultSolidColorBrush);
      BrushCategory.LastUsedBrush[BrushCategory.Gradient] = new BrushCategory.LastBrushEntry(BrushCategory.defaultLinearGradientBrush);
      BrushCategory.LastUsedBrush[BrushCategory.LinearGradient] = new BrushCategory.LastBrushEntry(BrushCategory.defaultLinearGradientBrush);
      BrushCategory.LastUsedBrush[BrushCategory.RadialGradient] = new BrushCategory.LastBrushEntry(BrushCategory.defaultRadialGradientBrush);
      BrushCategory.LastUsedBrush[BrushCategory.Drawing] = new BrushCategory.LastBrushEntry(BrushCategory.defaultDrawingBrush);
      BrushCategory.LastUsedBrush[BrushCategory.Image] = new BrushCategory.LastBrushEntry(BrushCategory.defaultImageBrush);
      BrushCategory.LastUsedBrush[BrushCategory.Visual] = new BrushCategory.LastBrushEntry(BrushCategory.defaultVisualBrush);
      BrushCategory.LastUsedBrush[BrushCategory.Video] = new BrushCategory.LastBrushEntry(BrushCategory.defaultVideoBrush);
      BrushCategory.LastUsedBrush[BrushCategory.Tile] = new BrushCategory.LastBrushEntry(BrushCategory.defaultImageBrush);
      BrushCategory.LastUsedBrush[BrushCategory.Html] = new BrushCategory.LastBrushEntry(BrushCategory.defaultHtmlBrush);
    }

    private static DrawingBrush CreateDefaultDrawingBrush()
    {
      DrawingBrush drawingBrush = new DrawingBrush((System.Windows.Media.Drawing) new DrawingGroup()
      {
        Children = {
          (System.Windows.Media.Drawing) new GeometryDrawing()
          {
            Brush = (Brush) Brushes.LightGray,
            Geometry = (Geometry) new RectangleGeometry(new Rect(0.0, 0.0, 20.0, 20.0))
          },
          (System.Windows.Media.Drawing) new GeometryDrawing()
          {
            Brush = (Brush) Brushes.Black,
            Geometry = (Geometry) new EllipseGeometry(new Point(0.0, 0.0), 10.0, 10.0)
          },
          (System.Windows.Media.Drawing) new GeometryDrawing()
          {
            Brush = (Brush) Brushes.Black,
            Geometry = (Geometry) new EllipseGeometry(new Point(20.0, 20.0), 10.0, 10.0)
          },
          (System.Windows.Media.Drawing) new GeometryDrawing()
          {
            Brush = (Brush) Brushes.White,
            Geometry = (Geometry) new EllipseGeometry(new Point(20.0, 0.0), 10.0, 10.0)
          },
          (System.Windows.Media.Drawing) new GeometryDrawing()
          {
            Brush = (Brush) Brushes.White,
            Geometry = (Geometry) new EllipseGeometry(new Point(0.0, 20.0), 10.0, 10.0)
          }
        }
      });
      drawingBrush.Viewbox = new Rect(0.0, 0.0, 20.0, 20.0);
      drawingBrush.ViewboxUnits = BrushMappingMode.Absolute;
      drawingBrush.Freeze();
      return drawingBrush;
    }

    private static VisualBrush CreateDefaultVisualBrush()
    {
      TextBlock textBlock = new TextBlock();
      textBlock.Text = "VisualBrush";
      textBlock.FontStyle = FontStyles.Italic;
      textBlock.Foreground = (Brush) Brushes.Black;
      VisualBrush visualBrush = new VisualBrush();
      visualBrush.Visual = (System.Windows.Media.Visual) textBlock;
      visualBrush.Stretch = Stretch.Uniform;
      return visualBrush;
    }

    private class LastBrushEntry
    {
      private object lastBrush;
      private Brush defaultBrush;

      public LastBrushEntry(Brush defaultBrush)
      {
        this.defaultBrush = defaultBrush;
      }

      public void SetBrush(object brush)
      {
        this.lastBrush = brush;
      }

      public object GetBrush(DesignerContext designerContext, IDocumentContext documentContext)
      {
        if (designerContext == null || documentContext == null)
          return (object) this.defaultBrush;
        object obj = this.lastBrush;
        bool flag = designerContext.ActiveProjectContext != null && designerContext.ActiveProjectContext.IsCapabilitySet(PlatformCapability.IsWpf);
        if (obj == null)
          obj = flag ? (object) this.defaultBrush : designerContext.PlatformConverter.ConvertToSilverlight(documentContext, (object) this.defaultBrush);
        return obj;
      }
    }
  }
}
