// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Controls.MultiSliderThumb
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Microsoft.Expression.Framework.Controls
{
  public class MultiSliderThumb : ContentControl
  {
    private static readonly double checkerboardSize = 10.0;
    public static readonly DependencyProperty SliderValueProperty = DependencyProperty.Register("SliderValue", typeof (double), typeof (MultiSliderThumb), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0.0)
    {
      Inherits = false,
      BindsTwoWayByDefault = true,
      IsNotDataBindable = false
    });

    public double Minimum
    {
      get
      {
        return this.ParentSlider.Minimum;
      }
    }

    public double Maximum
    {
      get
      {
        return this.ParentSlider.Maximum;
      }
    }

    public double SliderValue
    {
      get
      {
        return (double) this.GetValue(MultiSliderThumb.SliderValueProperty);
      }
      set
      {
        this.SetValue(MultiSliderThumb.SliderValueProperty, (object) value);
      }
    }

    public MultiSlider ParentSlider
    {
      get
      {
        return ItemsControl.ItemsControlFromItemContainer((DependencyObject) this) as MultiSlider;
      }
    }

    static MultiSliderThumb()
    {
      Style defaultStyle = MultiSliderThumb.GetDefaultStyle();
      FrameworkElement.StyleProperty.OverrideMetadata(typeof (MultiSliderThumb), (PropertyMetadata) new FrameworkPropertyMetadata((object) defaultStyle));
    }

    protected override void OnVisualParentChanged(DependencyObject oldParent)
    {
      if (VisualTreeHelper.GetParent((DependencyObject) this) == null)
      {
        if (this.ParentSlider != null)
          this.ParentSlider.NotifyItemUIRemoved((FrameworkElement) this);
      }
      else
        this.InvalidateProperty(MultiSliderThumb.SliderValueProperty);
      base.OnVisualParentChanged(oldParent);
    }

    private static Style GetDefaultStyle()
    {
      Style style = new Style(typeof (MultiSliderThumb));
      ControlTemplate controlTemplate = new ControlTemplate(typeof (MultiSliderThumb));
      style.Setters.Add((SetterBase) new Setter(Control.TemplateProperty, (object) controlTemplate));
      Brush brush = MultiSliderThumb.MakeCheckerboardBrush(MultiSliderThumb.checkerboardSize);
      style.Setters.Add((SetterBase) new Setter(Control.BackgroundProperty, (object) brush));
      FrameworkElementFactory frameworkElementFactory = new FrameworkElementFactory(typeof (Canvas), "mainCanvas");
      controlTemplate.VisualTree = frameworkElementFactory;
      GeometryConverter geometryConverter = new GeometryConverter();
      FrameworkElementFactory child1 = new FrameworkElementFactory(typeof (Path), "outerPath");
      child1.SetValue(Shape.FillProperty, (object) Brushes.White);
      child1.SetValue(Shape.StrokeProperty, (object) Brushes.Black);
      child1.SetValue(Shape.StrokeThicknessProperty, (object) 1.0);
      child1.SetValue(Shape.StrokeLineJoinProperty, (object) PenLineJoin.Round);
      child1.SetValue(Path.DataProperty, geometryConverter.ConvertFromInvariantString("M 0 8 L 8 0 L 16 8 L 16 20 L 0 20 Z"));
      child1.SetValue(Canvas.TopProperty, (object) 12.0);
      child1.SetValue(Canvas.LeftProperty, (object) -8.0);
      frameworkElementFactory.AppendChild(child1);
      FrameworkElementFactory child2 = new FrameworkElementFactory(typeof (Path), "innerPath");
      child2.SetValue(Shape.FillProperty, (object) Brushes.Black);
      child2.SetValue(Shape.StrokeProperty, (object) Brushes.Black);
      child2.SetValue(UIElement.VisibilityProperty, (object) Visibility.Hidden);
      child2.SetValue(Shape.StrokeThicknessProperty, (object) 1.0);
      child2.SetValue(Shape.StrokeLineJoinProperty, (object) PenLineJoin.Round);
      child2.SetValue(Path.DataProperty, geometryConverter.ConvertFromInvariantString("M 3 0 L 6 3 L 0 3 Z"));
      child2.SetValue(Canvas.TopProperty, (object) 15.0);
      child2.SetValue(Canvas.LeftProperty, (object) -3.0);
      frameworkElementFactory.AppendChild(child2);
      FrameworkElementFactory child3 = new FrameworkElementFactory(typeof (Rectangle), "innerRectangle");
      child3.SetValue(Shape.FillProperty, (object) new TemplateBindingExtension(Control.ForegroundProperty));
      child3.SetValue(Shape.FillProperty, (object) Brushes.Blue);
      child3.SetValue(Shape.StrokeProperty, (object) Brushes.Black);
      child3.SetValue(Shape.StrokeThicknessProperty, (object) 0.25);
      child3.SetValue(Shape.StrokeLineJoinProperty, (object) PenLineJoin.Round);
      child3.SetValue(FrameworkElement.WidthProperty, (object) 12.0);
      child3.SetValue(FrameworkElement.HeightProperty, (object) 8.0);
      child3.SetValue(Canvas.TopProperty, (object) 21.0);
      child3.SetValue(Canvas.LeftProperty, (object) -6.0);
      frameworkElementFactory.AppendChild(child3);
      Trigger trigger = new Trigger();
      trigger.Property = Selector.IsSelectedProperty;
      trigger.Value = (object) true;
      trigger.Setters.Add((SetterBase) new Setter(UIElement.VisibilityProperty, (object) Visibility.Visible, "innerPath"));
      trigger.Setters.Add((SetterBase) new Setter(Shape.StrokeThicknessProperty, (object) 2.0, "outerPath"));
      controlTemplate.Triggers.Add((TriggerBase) trigger);
      return style;
    }

    internal static Brush MakeCheckerboardBrush(double checkerboardSize)
    {
      DrawingGroup drawingGroup = new DrawingGroup();
      DrawingContext drawingContext = drawingGroup.Open();
      drawingContext.DrawRectangle((Brush) Brushes.White, (Pen) null, new Rect(new Point(0.0, 0.0), new Point(checkerboardSize * 2.0, checkerboardSize * 2.0)));
      for (int index1 = 0; index1 < 2; ++index1)
      {
        for (int index2 = 0; index2 < 2; ++index2)
        {
          if ((index1 + index2) % 2 == 1)
          {
            Point point1 = new Point(checkerboardSize * (double) index1, checkerboardSize * (double) index2);
            Point point2 = new Point(checkerboardSize * (double) (index1 + 1), checkerboardSize * (double) (index2 + 1));
            drawingContext.DrawRectangle((Brush) Brushes.LightGray, (Pen) null, new Rect(point1, point2));
          }
        }
      }
      drawingContext.Close();
      DrawingBrush drawingBrush = new DrawingBrush((Drawing) drawingGroup);
      drawingBrush.ViewportUnits = BrushMappingMode.Absolute;
      drawingBrush.Viewport = new Rect(0.0, 0.0, checkerboardSize * 2.0, checkerboardSize * 2.0);
      drawingBrush.Stretch = Stretch.Uniform;
      drawingBrush.TileMode = TileMode.Tile;
      drawingBrush.Freeze();
      return (Brush) drawingBrush;
    }
  }
}
