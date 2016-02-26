// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Geometry.PathConversionHelper
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface.Geometry.Core;
using Microsoft.Expression.DesignSurface.Tools.Text;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Geometry
{
  internal static class PathConversionHelper
  {
    public static bool CanConvert(SceneElement element)
    {
      if (!element.IsViewObjectValid)
        return false;
      BaseFrameworkElement frameworkElement = element as BaseFrameworkElement;
      if (frameworkElement == null)
        return false;
      ITypeId type = (ITypeId) frameworkElement.ViewObject.GetIType((ITypeResolver) element.ProjectContext);
      return PlatformTypes.Shape.IsAssignableFrom(type) || PlatformTypes.RichTextBox.IsAssignableFrom(type) || (PlatformTypes.TextBox.IsAssignableFrom(type) || PlatformTypes.TextBlock.IsAssignableFrom(type));
    }

    public static PathGeometry ConvertToPathGeometry(SceneElement element)
    {
      PathGeometry[] pathGeometryArray = PathConversionHelper.ConvertToPathGeometries(element);
      if (pathGeometryArray.Length == 0)
        return new PathGeometry();
      if (pathGeometryArray.Length > 1)
      {
        for (int index = 1; index < pathGeometryArray.Length; ++index)
            pathGeometryArray[0].AddGeometry((System.Windows.Media.Geometry)pathGeometryArray[index]);
      }
      PathGeometry pathGeometry = pathGeometryArray[0];
      if (pathGeometry != null && Enumerable.Any<PathFigure>((IEnumerable<PathFigure>) pathGeometry.Figures, (Func<PathFigure, bool>) (figure => Enumerable.Any<PathSegment>((IEnumerable<PathSegment>) figure.Segments, (Func<PathSegment, bool>) (segment => segment is ArcSegment)))))
          pathGeometry = PathGeometryUtilities.TransformGeometry((System.Windows.Media.Geometry)PathGeometryUtilities.TransformGeometry((System.Windows.Media.Geometry)pathGeometry, (Transform)new TranslateTransform(1.0, 0.0)), (Transform)new TranslateTransform(-1.0, 0.0));
      return pathGeometry;
    }

    public static PathGeometry[] ConvertToPathGeometries(SceneElement element)
    {
      if (!PathConversionHelper.CanConvert(element))
        return new PathGeometry[0];
      object platformSpecificObject = element.ViewObject.PlatformSpecificObject;
      ITypeId type = (ITypeId) element.ViewObject.GetIType((ITypeResolver) element.ProjectContext);
      List<PathGeometry> pathList = new List<PathGeometry>();
      if (PlatformTypes.Shape.IsAssignableFrom(type))
      {
        PathGeometry pathGeometry = PathConversionHelper.SimplifyGeometry(element.ViewModel.DefaultView.GetRenderedGeometryAsWpf(element), true, ProjectNeutralTypes.PrimitiveShape.IsAssignableFrom((ITypeId) element.Type));
        pathList.Add(pathGeometry);
      }
      else if (platformSpecificObject is TextBlock)
      {
        TextBlock textBlock = platformSpecificObject as TextBlock;
        if (!textBlock.IsArrangeValid)
          textBlock.UpdateLayout();
        PathConversionHelper.ConvertTextRangeToGeometry(new TextRange(textBlock.ContentStart, textBlock.ContentEnd), textBlock.FlowDirection, pathList);
      }
      else if (platformSpecificObject is RichTextBox)
        PathConversionHelper.ConvertRichTextBoxToGeometry(platformSpecificObject as RichTextBox, pathList);
      else if (platformSpecificObject is TextBox)
        PathConversionHelper.ConvertTextBoxToGeometry(platformSpecificObject as TextBox, pathList);
      else
        PathConversionHelper.ConvertToPathGeometriesViaProxy(element as BaseFrameworkElement, pathList);
      return pathList.ToArray();
    }

    private static void ConvertToPathGeometriesViaProxy(BaseFrameworkElement frameworkElement, List<PathGeometry> pathList)
    {
      TextEditProxy editProxy = TextEditProxyFactory.CreateEditProxy(frameworkElement);
      IViewTextBoxBase editingElement = editProxy.EditingElement;
      UIElement uiElement = editingElement.PlatformSpecificObject as UIElement;
      if (uiElement == null)
        return;
      frameworkElement.DesignerContext.ActiveView.AddLiveControl((IViewControl) editingElement);
      editProxy.ForceLoadOnInstantiate = true;
      editProxy.Instantiate();
      Rect computedTightBounds = frameworkElement.GetComputedTightBounds();
      editingElement.Width = computedTightBounds.Width;
      editingElement.Height = computedTightBounds.Height;
      if (uiElement is RichTextBox)
        PathConversionHelper.ConvertRichTextBoxToGeometry(uiElement as RichTextBox, pathList);
      else if (uiElement is TextBox)
        PathConversionHelper.ConvertTextBoxToGeometry(uiElement as TextBox, pathList);
      frameworkElement.DesignerContext.ActiveView.RemoveLiveControl((IViewControl) editingElement);
    }

    private static void ConvertTextRangeToGeometry(TextRange textRange, FlowDirection flowDirection, List<PathGeometry> pathList)
    {
      TextPointer position1 = textRange.Start.GetInsertionPosition(LogicalDirection.Forward);
      for (TextPointer insertionPosition = position1.GetNextInsertionPosition(LogicalDirection.Forward); insertionPosition != null && insertionPosition.CompareTo(textRange.End) <= 0; insertionPosition = insertionPosition.GetNextInsertionPosition(LogicalDirection.Forward))
      {
        TextRange textRange1 = new TextRange(position1, insertionPosition);
        Rect characterRect = position1.GetCharacterRect(LogicalDirection.Forward);
        characterRect.Union(insertionPosition.GetCharacterRect(LogicalDirection.Backward));
        FontFamily fontFamily = (FontFamily) textRange1.GetPropertyValue(TextElement.FontFamilyProperty);
        FontStyle style = (FontStyle) textRange1.GetPropertyValue(TextElement.FontStyleProperty);
        FontWeight weight = (FontWeight) textRange1.GetPropertyValue(TextElement.FontWeightProperty);
        FontStretch stretch = (FontStretch) textRange1.GetPropertyValue(TextElement.FontStretchProperty);
        double emSize = (double) textRange1.GetPropertyValue(TextElement.FontSizeProperty);
        Typeface typeface = new Typeface(fontFamily, style, weight, stretch);
        PathConversionHelper.ConvertFormattedTextToGeometry(new FormattedText(textRange1.Text, CultureInfo.CurrentCulture, flowDirection, typeface, emSize, (Brush) Brushes.Red), characterRect.TopLeft, pathList);
        position1 = insertionPosition;
      }
    }

    private static void ConvertRichTextBoxToGeometry(RichTextBox richTextBox, List<PathGeometry> pathList)
    {
      FlowDocument document = richTextBox.Document;
      if (!richTextBox.IsArrangeValid)
        richTextBox.UpdateLayout();
      PathConversionHelper.ConvertTextRangeToGeometry(new TextRange(document.ContentStart, document.ContentEnd), richTextBox.FlowDirection, pathList);
    }

    private static void ConvertTextBoxToGeometry(TextBox textBox, List<PathGeometry> pathList)
    {
      if (!textBox.IsArrangeValid)
        textBox.UpdateLayout();
      Typeface typeface = new Typeface(textBox.FontFamily, textBox.FontStyle, textBox.FontWeight, textBox.FontStretch);
      string text = textBox.Text;
      for (int index = 0; index < text.Length; ++index)
        PathConversionHelper.ConvertFormattedTextToGeometry(new FormattedText(text.Substring(index, 1), CultureInfo.CurrentCulture, textBox.FlowDirection, typeface, textBox.FontSize, textBox.Foreground), textBox.GetRectFromCharacterIndex(index).TopLeft, pathList);
    }

    private static void ConvertFormattedTextToGeometry(FormattedText formattedText, Point origin, List<PathGeometry> pathList)
    {
      PathGeometry pathGeometry = PathConversionHelper.SimplifyGeometry(formattedText.BuildGeometry(origin), false, false);
      pathList.Add(pathGeometry);
    }

    private static void ReplacePolySegments(PathSegmentCollection pathSegments)
    {
      for (int index1 = 0; index1 < pathSegments.Count; ++index1)
      {
        PathSegment pathSegment = pathSegments[index1];
        PolyLineSegment polyLineSegment = pathSegment as PolyLineSegment;
        if (polyLineSegment != null)
        {
          pathSegments.RemoveAt(index1);
          for (int index2 = 0; index2 < polyLineSegment.Points.Count; ++index2)
          {
            LineSegment lineSegment = PathSegmentUtilities.CreateLineSegment(polyLineSegment.Points[index2], polyLineSegment.IsStroked);
            pathSegments.Insert(index1, (PathSegment) lineSegment);
            ++index1;
          }
          --index1;
        }
        PolyQuadraticBezierSegment quadraticBezierSegment1 = pathSegment as PolyQuadraticBezierSegment;
        if (quadraticBezierSegment1 != null)
        {
          pathSegments.RemoveAt(index1);
          int index2 = 0;
          while (index2 < quadraticBezierSegment1.Points.Count - 1)
          {
            QuadraticBezierSegment quadraticBezierSegment2 = PathSegmentUtilities.CreateQuadraticBezierSegment(quadraticBezierSegment1.Points[index2], quadraticBezierSegment1.Points[index2 + 1], quadraticBezierSegment1.IsStroked);
            pathSegments.Insert(index1, (PathSegment) quadraticBezierSegment2);
            ++index1;
            index2 += 2;
          }
          --index1;
        }
        PolyBezierSegment polyBezierSegment = pathSegment as PolyBezierSegment;
        if (polyBezierSegment != null)
        {
          pathSegments.RemoveAt(index1);
          int index2 = 0;
          while (index2 < polyBezierSegment.Points.Count - 2)
          {
            BezierSegment bezierSegment = PathSegmentUtilities.CreateBezierSegment(polyBezierSegment.Points[index2], polyBezierSegment.Points[index2 + 1], polyBezierSegment.Points[index2 + 2], polyBezierSegment.IsStroked);
            pathSegments.Insert(index1, (PathSegment) bezierSegment);
            ++index1;
            index2 += 3;
          }
          --index1;
        }
      }
    }

    private static void RemoveDegenerateSegments(PathFigure pathFigure, bool removeInvisible)
    {
      PathSegmentCollection segments = pathFigure.Segments;
      Point b = pathFigure.StartPoint;
      Point startPoint = pathFigure.StartPoint;
      for (int index = 0; index < segments.Count; ++index)
      {
        PathSegment pathSegment = segments[index];
        LineSegment lineSegment;
        if ((lineSegment = pathSegment as LineSegment) != null)
        {
          if (VectorUtilities.ArePathPointsVeryClose(lineSegment.Point, b) || removeInvisible && !lineSegment.IsStroked && pathFigure.Segments.Count == 1)
            segments.RemoveAt(index--);
          else
            b = lineSegment.Point;
        }
        else
        {
          QuadraticBezierSegment quadraticBezierSegment;
          if ((quadraticBezierSegment = pathSegment as QuadraticBezierSegment) != null)
          {
            if (VectorUtilities.ArePathPointsVeryClose(quadraticBezierSegment.Point1, b) && VectorUtilities.ArePathPointsVeryClose(quadraticBezierSegment.Point2, b))
              segments.RemoveAt(index--);
            else
              b = quadraticBezierSegment.Point2;
          }
          else
          {
            BezierSegment bezierSegment;
            if ((bezierSegment = pathSegment as BezierSegment) != null)
            {
              if (VectorUtilities.ArePathPointsVeryClose(bezierSegment.Point1, b) && VectorUtilities.ArePathPointsVeryClose(bezierSegment.Point2, b) && VectorUtilities.ArePathPointsVeryClose(bezierSegment.Point3, b))
                segments.RemoveAt(index--);
              else
                b = bezierSegment.Point3;
            }
          }
        }
      }
      if (segments.Count < 1)
        return;
      LineSegment lineSegment1 = segments[segments.Count - 1] as LineSegment;
      if (lineSegment1 == null || !pathFigure.IsClosed || !VectorUtilities.ArePathPointsVeryClose(startPoint, lineSegment1.Point))
        return;
      segments.RemoveAt(segments.Count - 1);
    }

    private static PathGeometry SimplifyGeometry(System.Windows.Media.Geometry geometry, bool removeSmoothJoin, bool removeInvisible)
    {
      PathGeometry pathGeometry = new PathGeometry();
      pathGeometry.AddGeometry(geometry);
      for (int index = 0; index < pathGeometry.Figures.Count; ++index)
      {
        PathFigure pathFigure = pathGeometry.Figures[index];
        if (pathFigure.IsFrozen)
        {
          pathFigure = pathFigure.Clone();
          pathGeometry.Figures[index] = pathFigure;
        }
        PathConversionHelper.ReplacePolySegments(pathFigure.Segments);
        PathConversionHelper.RemoveDegenerateSegments(pathFigure, removeInvisible);
        if (removeInvisible && pathFigure.Segments.Count == 0)
          pathGeometry.Figures.RemoveAt(index--);
      }
      foreach (PathFigure pathFigure in pathGeometry.Figures)
      {
        for (int index = 0; index < pathFigure.Segments.Count; ++index)
        {
          PathSegment pathSegment = pathFigure.Segments[index];
          object obj1 = pathSegment.ReadLocalValue(PathSegment.IsStrokedProperty);
          if (obj1 is bool && (bool) obj1)
            pathSegment.ClearValue(PathSegment.IsStrokedProperty);
          if (removeSmoothJoin)
          {
            object obj2 = pathSegment.ReadLocalValue(PathSegment.IsSmoothJoinProperty);
            if (obj2 is bool && (bool) obj2)
              pathSegment.ClearValue(PathSegment.IsSmoothJoinProperty);
          }
        }
      }
      return pathGeometry;
    }

    public static PathGeometry RemoveDegeneratePoints(System.Windows.Media.Geometry geometry)
    {
      if (geometry == null)
        return (PathGeometry) null;
      PathGeometry pathGeometry1 = new PathGeometry();
      pathGeometry1.AddGeometry(geometry);
      PathGeometry pathGeometry2 = geometry as PathGeometry;
      if (pathGeometry2 != null)
        pathGeometry1.FillRule = pathGeometry2.FillRule;
      for (int index = 0; index < pathGeometry1.Figures.Count; ++index)
      {
        PathFigure pathFigure = pathGeometry1.Figures[index];
        if (pathFigure.IsFrozen)
        {
          pathFigure = pathFigure.Clone();
          pathGeometry1.Figures[index] = pathFigure;
        }
        PathConversionHelper.ReplacePolySegments(pathFigure.Segments);
        bool removeInvisible = true;
        PathConversionHelper.RemoveDegenerateSegments(pathFigure, removeInvisible);
        if (pathFigure.Segments.Count == 0)
          pathGeometry1.Figures.RemoveAt(index--);
      }
      return pathGeometry1;
    }
  }
}
