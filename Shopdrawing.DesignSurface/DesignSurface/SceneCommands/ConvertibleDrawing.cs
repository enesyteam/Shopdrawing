// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.ConvertibleDrawing
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal class ConvertibleDrawing
  {
    public static readonly IPropertyId DrawingBrushDrawingProperty = (IPropertyId) PlatformTypes.DrawingBrush.GetMember(MemberType.LocalProperty, "Drawing", MemberAccessTypes.Public);
    public static readonly IPropertyId DrawingGroupChildrenProperty = (IPropertyId) PlatformTypes.DrawingGroup.GetMember(MemberType.LocalProperty, "Children", MemberAccessTypes.Public);
    private string projectPath;
    private Drawing drawing;

    public Drawing Drawing
    {
      get
      {
        return this.drawing;
      }
    }

    public string ProjectPath
    {
      get
      {
        return this.projectPath;
      }
    }

    protected ConvertibleDrawing(Drawing drawing, string projectPath)
    {
      this.drawing = drawing;
      this.projectPath = projectPath;
    }

    protected virtual Rect GetDrawingBounds()
    {
      return this.Drawing.Bounds;
    }

    public static bool IsResourceTypeSupported(IType resourceType)
    {
      return PlatformTypes.DrawingBrush.IsAssignableFrom((ITypeId) resourceType);
    }

    public static bool CanCreateConvertibleDrawing(DocumentNode resourceNode)
    {
      if (ConvertibleDrawing.IsResourceTypeSupported(resourceNode.Type))
        return !ConvertibleDrawing.ContainsVideoDrawing(resourceNode);
      return false;
    }

    internal static bool ContainsVideoDrawing(DocumentNode node)
    {
      DocumentCompositeNode documentCompositeNode1 = node as DocumentCompositeNode;
      if (PlatformTypes.DrawingBrush.IsAssignableFrom((ITypeId) node.Type))
        documentCompositeNode1 = documentCompositeNode1.Properties[ConvertibleDrawing.DrawingBrushDrawingProperty] as DocumentCompositeNode;
      if (documentCompositeNode1 != null && PlatformTypes.Drawing.IsAssignableFrom((ITypeId) documentCompositeNode1.Type))
      {
        if (PlatformTypes.VideoDrawing.IsAssignableFrom((ITypeId) documentCompositeNode1.Type))
          return true;
        if (PlatformTypes.DrawingGroup.IsAssignableFrom((ITypeId) documentCompositeNode1.Type))
        {
          DocumentCompositeNode documentCompositeNode2 = documentCompositeNode1.Properties[ConvertibleDrawing.DrawingGroupChildrenProperty] as DocumentCompositeNode;
          if (documentCompositeNode2 != null)
          {
            foreach (DocumentNode node1 in (IEnumerable<DocumentNode>) documentCompositeNode2.Children)
            {
              if (ConvertibleDrawing.ContainsVideoDrawing(node1))
                return true;
            }
          }
        }
      }
      return false;
    }

    internal static bool ContainsVideoDrawing(Drawing drawing)
    {
      if (drawing is VideoDrawing)
        return true;
      DrawingGroup drawingGroup = drawing as DrawingGroup;
      if (drawingGroup != null)
      {
        foreach (Drawing drawing1 in drawingGroup.Children)
        {
          if (ConvertibleDrawing.ContainsVideoDrawing(drawing1))
            return true;
        }
      }
      return false;
    }

    public static ConvertibleDrawing CreateConvertibleDrawing(object resource, string projectPath)
    {
      DrawingBrush drawingBrush = resource as DrawingBrush;
      if (drawingBrush != null)
        return (ConvertibleDrawing) new ConvertibleDrawingBrush(drawingBrush, projectPath);
      throw new ArgumentException("Unsupported resource type", "resource");
    }

    protected static void SetPositionInCanvas(UIElement element, Point offset)
    {
      ConvertibleDrawing.SetValueIfNotDefault(Canvas.TopProperty, (DependencyObject) element, (object) offset.Y);
      ConvertibleDrawing.SetValueIfNotDefault(Canvas.LeftProperty, (DependencyObject) element, (object) offset.X);
    }

    public virtual FrameworkElement Convert()
    {
      Canvas canvas = new Canvas();
      if (this.Drawing != null)
      {
        this.PrepareCanvas(canvas);
        this.PutDrawingOnCanvas(canvas);
      }
      else
      {
        canvas.Width = 0.0;
        canvas.Height = 0.0;
      }
      return (FrameworkElement) canvas;
    }

    protected virtual void PrepareCanvas(Canvas canvas)
    {
      Rect drawingBounds = this.GetDrawingBounds();
      canvas.Width = drawingBounds.Width;
      canvas.Height = drawingBounds.Height;
    }

    protected void PutDrawingOnCanvas(Canvas canvas)
    {
      Stack<ConvertibleDrawing.DrawingStackEntry> stack = new Stack<ConvertibleDrawing.DrawingStackEntry>();
      ConvertibleDrawing.DrawingStackEntry drawingStackEntry1 = new ConvertibleDrawing.DrawingStackEntry(canvas, this.Drawing, Matrix.Identity);
      stack.Push(drawingStackEntry1);
      while (stack.Count > 0)
      {
        ConvertibleDrawing.DrawingStackEntry entry = stack.Pop();
        DrawingGroup drawingGroup = entry.Drawing as DrawingGroup;
        if (drawingGroup != null)
        {
          Matrix matrix = Matrix.Identity;
          Canvas canvas1;
          if (this.ShouldCreateCanvasFromDrawingGroup(drawingGroup))
          {
            canvas1 = this.AddCanvasLevel(entry);
          }
          else
          {
            Matrix identity = Matrix.Identity;
            if (drawingGroup.Transform != null)
              identity = drawingGroup.Transform.Value;
            canvas1 = entry.Canvas;
            matrix = entry.Matrix * identity;
          }
          for (int index = drawingGroup.Children.Count - 1; index >= 0; --index)
          {
            ConvertibleDrawing.DrawingStackEntry drawingStackEntry2 = new ConvertibleDrawing.DrawingStackEntry(canvas1, drawingGroup.Children[index], matrix);
            stack.Push(drawingStackEntry2);
          }
        }
        else
          this.AddSimpleDrawingToCanvas(entry);
      }
    }

    private bool ShouldCreateCanvasFromDrawingGroup(DrawingGroup drawingGroup)
    {
      if (drawingGroup.Children.Count <= 1 && drawingGroup.BitmapEffect == null && (drawingGroup.BitmapEffectInput == null && drawingGroup.OpacityMask == null))
        return drawingGroup.ClipGeometry != null;
      return true;
    }

    protected void AddSimpleDrawingToCanvas(ConvertibleDrawing.DrawingStackEntry entry)
    {
      if (entry.Drawing is GeometryDrawing)
        this.AddGeometryDrawing(entry);
      else if (entry.Drawing is GlyphRunDrawing)
        this.AddGlyphRunDrawing(entry);
      else if (entry.Drawing is ImageDrawing)
        this.AddImageDrawing(entry);
      else if (entry.Drawing is VideoDrawing)
        throw new NotSupportedException("VideoDrawing currently not supported.");
    }

    private static void SetValueIfNotDefault(DependencyProperty dependencyProperty, DependencyObject dependencyObject, object value)
    {
      if (object.Equals(dependencyProperty.DefaultMetadata.DefaultValue, value))
        return;
      dependencyObject.SetValue(dependencyProperty, value);
    }

    private Canvas AddCanvasLevel(ConvertibleDrawing.DrawingStackEntry entry)
    {
      Canvas canvas = new Canvas();
      DrawingGroup drawingGroup = entry.Drawing as DrawingGroup;
      if (entry.Transform != Transform.Identity)
      {
        CanonicalTransform canonicalTransform = new CanonicalTransform(entry.Transform);
        if (drawingGroup.Transform != null)
          canonicalTransform = new CanonicalTransform(entry.Transform.Value * drawingGroup.Transform.Value);
        ConvertibleDrawing.SetValueIfNotDefault(UIElement.RenderTransformProperty, (DependencyObject) canvas, (object) canonicalTransform.TransformGroup);
      }
      ConvertibleDrawing.SetPositionInCanvas((UIElement) canvas, entry.Offset);
      ConvertibleDrawing.SetValueIfNotDefault(UIElement.BitmapEffectProperty, (DependencyObject) canvas, (object) drawingGroup.BitmapEffect);
      ConvertibleDrawing.SetValueIfNotDefault(UIElement.BitmapEffectInputProperty, (DependencyObject) canvas, (object) drawingGroup.BitmapEffectInput);
      ConvertibleDrawing.SetValueIfNotDefault(UIElement.OpacityProperty, (DependencyObject) canvas, (object) drawingGroup.Opacity);
      ConvertibleDrawing.SetValueIfNotDefault(UIElement.OpacityMaskProperty, (DependencyObject) canvas, (object) drawingGroup.OpacityMask);
      ConvertibleDrawing.SetValueIfNotDefault(UIElement.ClipProperty, (DependencyObject) canvas, (object) drawingGroup.ClipGeometry);
      if (!double.IsInfinity(drawingGroup.Bounds.Height))
        ConvertibleDrawing.SetValueIfNotDefault(FrameworkElement.HeightProperty, (DependencyObject) canvas, (object) drawingGroup.Bounds.Height);
      if (!double.IsInfinity(drawingGroup.Bounds.Width))
        ConvertibleDrawing.SetValueIfNotDefault(FrameworkElement.WidthProperty, (DependencyObject) canvas, (object) drawingGroup.Bounds.Width);
      entry.Canvas.Children.Add((UIElement) canvas);
      return canvas;
    }

    private void AddGeometryDrawing(ConvertibleDrawing.DrawingStackEntry entry)
    {
      GeometryDrawing geometryDrawing = entry.Drawing as GeometryDrawing;
      Path path = new Path();
      ConvertibleDrawing.SetPositionInCanvas((UIElement) path, entry.Offset);
      if (entry.Transform != Transform.Identity)
        ConvertibleDrawing.SetValueIfNotDefault(UIElement.RenderTransformProperty, (DependencyObject) path, (object) entry.Transform);
      entry.Canvas.Children.Add((UIElement) path);
      ConvertibleDrawing.SetValueIfNotDefault(Shape.FillProperty, (DependencyObject) path, (object) geometryDrawing.Brush);
      if (geometryDrawing.Pen != null)
      {
        Pen pen = geometryDrawing.Pen;
        ConvertibleDrawing.SetValueIfNotDefault(Shape.StrokeProperty, (DependencyObject) path, (object) pen.Brush);
        ConvertibleDrawing.SetValueIfNotDefault(Shape.StrokeDashArrayProperty, (DependencyObject) path, (object) pen.DashStyle.Dashes);
        ConvertibleDrawing.SetValueIfNotDefault(Shape.StrokeDashCapProperty, (DependencyObject) path, (object) pen.DashCap);
        ConvertibleDrawing.SetValueIfNotDefault(Shape.StrokeDashOffsetProperty, (DependencyObject) path, (object) pen.DashStyle.Offset);
        ConvertibleDrawing.SetValueIfNotDefault(Shape.StrokeEndLineCapProperty, (DependencyObject) path, (object) pen.EndLineCap);
        ConvertibleDrawing.SetValueIfNotDefault(Shape.StrokeLineJoinProperty, (DependencyObject) path, (object) pen.LineJoin);
        ConvertibleDrawing.SetValueIfNotDefault(Shape.StrokeMiterLimitProperty, (DependencyObject) path, (object) pen.MiterLimit);
        ConvertibleDrawing.SetValueIfNotDefault(Shape.StrokeStartLineCapProperty, (DependencyObject) path, (object) pen.StartLineCap);
        ConvertibleDrawing.SetValueIfNotDefault(Shape.StrokeThicknessProperty, (DependencyObject) path, (object) pen.Thickness);
      }
      path.Data = geometryDrawing.Geometry;
    }

    private void AddGlyphRunDrawing(ConvertibleDrawing.DrawingStackEntry entry)
    {
      GlyphRunDrawing glyphRunDrawing = entry.Drawing as GlyphRunDrawing;
      System.Windows.Media.Geometry geometry = (System.Windows.Media.Geometry)null;
      if (glyphRunDrawing.GlyphRun != null)
        geometry = glyphRunDrawing.GlyphRun.BuildGeometry();
      Path path = new Path();
      ConvertibleDrawing.SetValueIfNotDefault(Shape.FillProperty, (DependencyObject) path, (object) glyphRunDrawing.ForegroundBrush);
      ConvertibleDrawing.SetValueIfNotDefault(Path.DataProperty, (DependencyObject) path, (object) geometry);
      ConvertibleDrawing.SetPositionInCanvas((UIElement) path, entry.Offset);
      if (entry.Transform != Transform.Identity)
        ConvertibleDrawing.SetValueIfNotDefault(UIElement.RenderTransformProperty, (DependencyObject) path, (object) entry.Transform);
      entry.Canvas.Children.Add((UIElement) path);
    }

    private void AddImageDrawing(ConvertibleDrawing.DrawingStackEntry entry)
    {
      ImageDrawing imageDrawing = entry.Drawing as ImageDrawing;
      if (this.projectPath != null)
        imageDrawing = (ImageDrawing) MakeDrawingBrushCommand.SimplifyImageDrawing(imageDrawing, this.projectPath);
      Image image = new Image();
      ConvertibleDrawing.SetValueIfNotDefault(FrameworkElement.HeightProperty, (DependencyObject) image, (object) imageDrawing.Bounds.Height);
      ConvertibleDrawing.SetValueIfNotDefault(FrameworkElement.WidthProperty, (DependencyObject) image, (object) imageDrawing.Bounds.Width);
      ConvertibleDrawing.SetValueIfNotDefault(Image.SourceProperty, (DependencyObject) image, (object) imageDrawing.ImageSource);
      ConvertibleDrawing.SetPositionInCanvas((UIElement) image, entry.Offset);
      if (entry.Transform != Transform.Identity)
        ConvertibleDrawing.SetValueIfNotDefault(UIElement.RenderTransformProperty, (DependencyObject) image, (object) entry.Transform);
      entry.Canvas.Children.Add((UIElement) image);
    }

    protected class DrawingStackEntry
    {
      private Canvas canvas;
      private Drawing drawing;
      private Matrix matrix;
      private Point offset;
      private CanonicalTransform transform;

      public Canvas Canvas
      {
        get
        {
          return this.canvas;
        }
      }

      public Drawing Drawing
      {
        get
        {
          return this.drawing;
        }
      }

      public Matrix Matrix
      {
        get
        {
          return this.matrix;
        }
      }

      public Transform Transform
      {
        get
        {
          return (Transform) this.transform.TransformGroup;
        }
      }

      public Point Offset
      {
        get
        {
          return this.offset;
        }
      }

      public DrawingStackEntry(Canvas canvas, Drawing drawing, Matrix matrix)
      {
        this.canvas = canvas;
        this.drawing = drawing;
        this.matrix = matrix;
        this.offset = new Point(this.matrix.OffsetX, this.matrix.OffsetY);
        this.transform = new CanonicalTransform(this.matrix);
        this.transform.Translation = new Vector(0.0, 0.0);
      }
    }
  }
}
