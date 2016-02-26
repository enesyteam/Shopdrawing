// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.MakeDrawingBrushCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal class MakeDrawingBrushCommand : MakeTileBrushCommand
  {
    private bool showDrawingVideoWarning;

    protected override string UndoUnitName
    {
      get
      {
        return StringTable.UndoUnitMakeDrawingBrush;
      }
    }

    protected override bool CreateResource
    {
      get
      {
        return true;
      }
    }

    protected override ITypeId Type
    {
      get
      {
        return PlatformTypes.DrawingBrush;
      }
    }

    public MakeDrawingBrushCommand(SceneViewModel viewModel)
      : base(viewModel)
    {
    }

    internal static Drawing SimplifyImageDrawing(ImageDrawing imageDrawing, string projectPath)
    {
      if (imageDrawing.ImageSource == null)
        return (Drawing) null;
      BitmapImage bitmapImage = imageDrawing.ImageSource as BitmapImage;
      if (bitmapImage != null)
      {
        Uri uriSource = bitmapImage.UriSource;
        Uri uri1 = new Uri(projectPath + (object) Path.DirectorySeparatorChar, UriKind.RelativeOrAbsolute).MakeRelativeUri(uriSource);
        if (!uri1.IsAbsoluteUri && !uri1.OriginalString.StartsWith("..", StringComparison.OrdinalIgnoreCase))
        {
          Uri uri2 = new Uri("/" + (object) uri1, UriKind.Relative);
          bitmapImage.UriSource = uri2;
        }
      }
      return (Drawing) imageDrawing;
    }

    protected override object CreateTileBrush(BaseFrameworkElement element)
    {
      Visual visual1 = element.Visual != null ? element.Visual.PlatformSpecificObject as Visual : (Visual) null;
      if (visual1 == null)
        return (object) null;
      foreach (Visual visual2 in ElementUtilities.GetVisualTree(visual1))
      {
        if (MakeDrawingBrushCommand.IsMediaElementInstance(visual2))
        {
          this.showDrawingVideoWarning = true;
          break;
        }
      }
      Drawing drawing = MakeDrawingBrushCommand.SimplifyDrawing(MakeDrawingBrushCommand.CreateDrawing(visual1, false), Path.GetDirectoryName(this.SceneViewModel.ProjectContext.ProjectPath));
      DrawingBrush drawingBrush = new DrawingBrush();
      if (drawing != null)
      {
        drawingBrush.Drawing = drawing;
        UIElement uiElement = visual1 as UIElement;
        if (uiElement != null)
        {
          drawingBrush.Viewbox = new Rect(uiElement.RenderSize);
          drawingBrush.ViewboxUnits = BrushMappingMode.Absolute;
        }
      }
      return (object) drawingBrush;
    }

    public override void Execute()
    {
      base.Execute();
      if (!this.showDrawingVideoWarning)
        return;
      int num = (int) this.DesignerContext.MessageDisplayService.ShowMessage(new MessageBoxArgs()
      {
        Message = StringTable.ConvertMediaElementToVideoDrawingDialogMessage,
        Button = MessageBoxButton.OK,
        Image = MessageBoxImage.Asterisk
      });
      this.showDrawingVideoWarning = false;
    }

    private static Drawing CreateDrawing(Visual visual, bool includeTransform)
    {
      DrawingGroup drawingGroup = new DrawingGroup();
      System.Windows.Media.Geometry clip = VisualTreeHelper.GetClip(visual);
      if (clip != null)
        drawingGroup.ClipGeometry = clip;
      if (includeTransform)
      {
        Transform transform = VisualTreeHelper.GetTransform(visual);
        Vector offset = VisualTreeHelper.GetOffset(visual);
        Matrix matrix = transform == null ? Matrix.Identity : transform.Value;
        matrix.Translate(offset.X, offset.Y);
        if (!matrix.IsIdentity)
          drawingGroup.Transform = (Transform) new MatrixTransform(matrix);
      }
      double opacity = VisualTreeHelper.GetOpacity(visual);
      if (opacity != 1.0)
        drawingGroup.Opacity = opacity;
      Brush opacityMask = VisualTreeHelper.GetOpacityMask(visual);
      if (opacityMask != null)
        drawingGroup.OpacityMask = opacityMask;
      BitmapEffect bitmapEffect = VisualTreeHelper.GetBitmapEffect(visual);
      if (bitmapEffect != null)
        drawingGroup.BitmapEffect = bitmapEffect;
      BitmapEffectInput bitmapEffectInput = VisualTreeHelper.GetBitmapEffectInput(visual);
      if (bitmapEffectInput != null && (!bitmapEffectInput.AreaToApplyEffect.IsEmpty || bitmapEffectInput.Input != BitmapEffectInput.ContextInputSource))
        drawingGroup.BitmapEffectInput = bitmapEffectInput;
      Drawing drawing1;
      if (MakeDrawingBrushCommand.IsMediaElementInstance(visual))
      {
        Rect bounds = VisualTreeHelper.GetDrawing(visual).Bounds;
        drawing1 = (Drawing) new VideoDrawing()
        {
          Rect = bounds
        };
      }
      else
        drawing1 = (Drawing) VisualTreeHelper.GetDrawing(visual);
      if (drawing1 != null)
        drawingGroup.Children.Add(drawing1);
      for (int childIndex = 0; childIndex < VisualTreeHelper.GetChildrenCount((DependencyObject) visual); ++childIndex)
      {
        Visual visual1 = VisualTreeHelper.GetChild((DependencyObject) visual, childIndex) as Visual;
        if (visual1 != null)
        {
          Drawing drawing2 = MakeDrawingBrushCommand.CreateDrawing(visual1, true);
          if (drawing2 != null)
            drawingGroup.Children.Add(drawing2);
        }
      }
      if (drawingGroup.Children.Count == 0)
        return (Drawing) null;
      if (drawingGroup.Children.Count == 1 && drawingGroup.ClipGeometry == null && (drawingGroup.Transform == null || drawingGroup.Transform.Value.IsIdentity) && (drawingGroup.Opacity == 1.0 && drawingGroup.OpacityMask == null && drawingGroup.BitmapEffect == null))
        return drawingGroup.Children[0];
      return (Drawing) drawingGroup;
    }

    private static bool IsMediaElementInstance(Visual visual)
    {
      if (visual is MediaElement)
      {
        System.Type type = visual.GetType();
        if (PlatformTypes.IsExpressionInteractiveType(type))
        {
          try
          {
            return type.GetCustomAttributes(typeof (DesignTimeMediaElementAttribute), false).Length != 0;
          }
          catch (Exception ex)
          {
          }
        }
      }
      return false;
    }

    private static Drawing SimplifyDrawing(Drawing drawing, string projectPath)
    {
      if (drawing != null)
      {
        if (drawing.IsFrozen)
          drawing = drawing.Clone();
        DrawingGroup drawingGroup;
        if ((drawingGroup = drawing as DrawingGroup) != null)
        {
          drawing = MakeDrawingBrushCommand.SimplifyDrawingGroup(drawingGroup, projectPath);
        }
        else
        {
          GeometryDrawing geometryDrawing;
          if ((geometryDrawing = drawing as GeometryDrawing) != null)
          {
            drawing = MakeDrawingBrushCommand.SimplifyGeometryDrawing(geometryDrawing);
          }
          else
          {
            GlyphRunDrawing glyphRunDrawing;
            if ((glyphRunDrawing = drawing as GlyphRunDrawing) != null)
            {
              drawing = MakeDrawingBrushCommand.SimplifyGlyphRunDrawing(glyphRunDrawing);
            }
            else
            {
              ImageDrawing imageDrawing;
              if ((imageDrawing = drawing as ImageDrawing) != null)
              {
                drawing = MakeDrawingBrushCommand.SimplifyImageDrawing(imageDrawing, projectPath);
              }
              else
              {
                VideoDrawing videoDrawing;
                if ((videoDrawing = drawing as VideoDrawing) != null)
                  drawing = MakeDrawingBrushCommand.SimplifyVideoDrawing(videoDrawing);
              }
            }
          }
        }
      }
      return drawing;
    }

    private static Drawing SimplifyDrawingGroup(DrawingGroup drawingGroup, string projectPath)
    {
      for (int index = drawingGroup.Children.Count - 1; index >= 0; --index)
      {
        Drawing drawing = MakeDrawingBrushCommand.SimplifyDrawing(drawingGroup.Children[index], projectPath);
        if (drawing == null)
          drawingGroup.Children.RemoveAt(index);
        else
          drawingGroup.Children[index] = drawing;
      }
      if (drawingGroup.ClipGeometry == null)
        drawingGroup.ClearValue(DrawingGroup.ClipGeometryProperty);
      if (drawingGroup.Transform == null || drawingGroup.Transform.Value.IsIdentity)
        drawingGroup.ClearValue(DrawingGroup.TransformProperty);
      if (drawingGroup.Opacity >= 1.0)
        drawingGroup.ClearValue(DrawingGroup.OpacityProperty);
      if (drawingGroup.OpacityMask == null)
        drawingGroup.ClearValue(DrawingGroup.OpacityMaskProperty);
      if (drawingGroup.BitmapEffect == null)
        drawingGroup.ClearValue(DrawingGroup.BitmapEffectProperty);
      if (drawingGroup.BitmapEffectInput == null || drawingGroup.BitmapEffectInput.AreaToApplyEffect.IsEmpty && drawingGroup.BitmapEffectInput.Input == BitmapEffectInput.ContextInputSource)
        drawingGroup.ClearValue(DrawingGroup.BitmapEffectInputProperty);
      if (drawingGroup.Children.Count == 0 || drawingGroup.Opacity <= 0.0)
        return (Drawing) null;
      if (drawingGroup.Children.Count == 1 && drawingGroup.ClipGeometry == null && (drawingGroup.Transform == null || drawingGroup.Transform.Value.IsIdentity) && (drawingGroup.Opacity == 1.0 && drawingGroup.OpacityMask == null && drawingGroup.BitmapEffect == null))
        return drawingGroup.Children[0];
      return (Drawing) drawingGroup;
    }

    private static Drawing SimplifyGeometryDrawing(GeometryDrawing geometryDrawing)
    {
      bool flag = geometryDrawing.Pen == null || geometryDrawing.Pen.Thickness == 0.0;
      if (geometryDrawing.Geometry == null || geometryDrawing.Geometry.IsEmpty() || geometryDrawing.Brush == null && flag)
        return (Drawing) null;
      System.Windows.Media.Geometry geometry = geometryDrawing.Geometry;
      if (geometry is StreamGeometry && geometry.Transform != null && !geometry.Transform.Value.IsIdentity)
      {
        PathGeometry pathGeometry = new PathGeometry();
        pathGeometry.AddGeometry(geometry);
        geometryDrawing.Geometry = (System.Windows.Media.Geometry)pathGeometry;
      }
      if (geometryDrawing.Brush == null)
        geometryDrawing.ClearValue(GeometryDrawing.BrushProperty);
      if (flag)
        geometryDrawing.ClearValue(GeometryDrawing.PenProperty);
      return (Drawing) geometryDrawing;
    }

    private static Drawing SimplifyGlyphRunDrawing(GlyphRunDrawing glyphRunDrawing)
    {
      if (glyphRunDrawing.ForegroundBrush == null || glyphRunDrawing.GlyphRun == null)
        return (Drawing) null;
      System.Windows.Media.Geometry geometry = glyphRunDrawing.GlyphRun.BuildGeometry();
      if (geometry == null || geometry.IsEmpty())
        return (Drawing) null;
      return MakeDrawingBrushCommand.SimplifyGeometryDrawing(new GeometryDrawing(glyphRunDrawing.ForegroundBrush, (Pen) null, geometry));
    }

    private static Drawing SimplifyVideoDrawing(VideoDrawing videoDrawing)
    {
      return (Drawing) videoDrawing;
    }
  }
}
