// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.EaseCurveEditor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.ValueEditors;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class EaseCurveEditor : Control, IComponentConnector
  {
    private ControlPointEditor controlPoint1Editor = new ControlPointEditor();
    private ControlPointEditor controlPoint2Editor = new ControlPointEditor();
    private bool pointPositionsInvalid = true;
    public static readonly DependencyProperty BeginEditCommandProperty = DependencyProperty.Register("BeginEditCommand", typeof (ICommand), typeof (EaseCurveEditor));
    public static readonly DependencyProperty CommitEditCommandProperty = DependencyProperty.Register("CommitEditCommand", typeof (ICommand), typeof (EaseCurveEditor));
    public static readonly DependencyProperty CancelEditCommandProperty = DependencyProperty.Register("CancelEditCommand", typeof (ICommand), typeof (EaseCurveEditor));
    public static readonly DependencyProperty X1Property = DependencyProperty.Register("X1", typeof (double), typeof (EaseCurveEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(EaseCurveEditor.PointPropertyChanged)));
    public static readonly DependencyProperty Y1Property = DependencyProperty.Register("Y1", typeof (double), typeof (EaseCurveEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(EaseCurveEditor.PointPropertyChanged)));
    public static readonly DependencyProperty X2Property = DependencyProperty.Register("X2", typeof (double), typeof (EaseCurveEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) 1.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(EaseCurveEditor.PointPropertyChanged)));
    public static readonly DependencyProperty Y2Property = DependencyProperty.Register("Y2", typeof (double), typeof (EaseCurveEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) 1.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(EaseCurveEditor.PointPropertyChanged)));
    public static readonly DependencyProperty X1IsNinchedProperty = DependencyProperty.Register("X1IsNinched", typeof (bool), typeof (EaseCurveEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty Y1IsNinchedProperty = DependencyProperty.Register("Y1IsNinched", typeof (bool), typeof (EaseCurveEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty X2IsNinchedProperty = DependencyProperty.Register("X2IsNinched", typeof (bool), typeof (EaseCurveEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty Y2IsNinchedProperty = DependencyProperty.Register("Y2IsNinched", typeof (bool), typeof (EaseCurveEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty GridBrushProperty = DependencyProperty.Register("GridBrush", typeof (Brush), typeof (EaseCurveEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(EaseCurveEditor.BrushPropertyChanged)));
    public static readonly DependencyProperty ControlPointBrushProperty = DependencyProperty.Register("ControlPointBrush", typeof (Brush), typeof (EaseCurveEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(EaseCurveEditor.BrushPropertyChanged)));
    private static readonly int maxPrecision = 2;
    private ControlPointEditor activePointEditor;
    private StreamGeometry pathGeometry;
    private Pen curvePen;
    private Pen linePen;
    private Brush brushOne;
    private Brush brushFive;
    private Brush brushTen;
    private bool _contentLoaded;

    public ICommand BeginEditCommand
    {
      get
      {
        return (ICommand) this.GetValue(EaseCurveEditor.BeginEditCommandProperty);
      }
      set
      {
        this.SetValue(EaseCurveEditor.BeginEditCommandProperty, (object) value);
      }
    }

    public ICommand CommitEditCommand
    {
      get
      {
        return (ICommand) this.GetValue(EaseCurveEditor.CommitEditCommandProperty);
      }
      set
      {
        this.SetValue(EaseCurveEditor.CommitEditCommandProperty, (object) value);
      }
    }

    public ICommand CancelEditCommand
    {
      get
      {
        return (ICommand) this.GetValue(EaseCurveEditor.CancelEditCommandProperty);
      }
      set
      {
        this.SetValue(EaseCurveEditor.CancelEditCommandProperty, (object) value);
      }
    }

    public double X1
    {
      get
      {
        return (double) this.GetValue(EaseCurveEditor.X1Property);
      }
      set
      {
        this.SetValue(EaseCurveEditor.X1Property, (object) value);
      }
    }

    public double Y1
    {
      get
      {
        return (double) this.GetValue(EaseCurveEditor.Y1Property);
      }
      set
      {
        this.SetValue(EaseCurveEditor.Y1Property, (object) value);
      }
    }

    public double X2
    {
      get
      {
        return (double) this.GetValue(EaseCurveEditor.X2Property);
      }
      set
      {
        this.SetValue(EaseCurveEditor.X2Property, (object) value);
      }
    }

    public double Y2
    {
      get
      {
        return (double) this.GetValue(EaseCurveEditor.Y2Property);
      }
      set
      {
        this.SetValue(EaseCurveEditor.Y2Property, (object) value);
      }
    }

    public bool X1IsNinched
    {
      get
      {
        return (bool) this.GetValue(EaseCurveEditor.X1IsNinchedProperty);
      }
      set
      {
        this.SetValue(EaseCurveEditor.X1IsNinchedProperty, (object) (bool) (value ? 1 : 0));
      }
    }

    public bool Y1IsNinched
    {
      get
      {
        return (bool) this.GetValue(EaseCurveEditor.Y1IsNinchedProperty);
      }
      set
      {
        this.SetValue(EaseCurveEditor.Y1IsNinchedProperty, (object) (bool) (value ? 1 : 0));
      }
    }

    public bool X2IsNinched
    {
      get
      {
        return (bool) this.GetValue(EaseCurveEditor.X2IsNinchedProperty);
      }
      set
      {
        this.SetValue(EaseCurveEditor.X2IsNinchedProperty, (object) (bool) (value ? 1 : 0));
      }
    }

    public bool Y2IsNinched
    {
      get
      {
        return (bool) this.GetValue(EaseCurveEditor.Y2IsNinchedProperty);
      }
      set
      {
        this.SetValue(EaseCurveEditor.Y2IsNinchedProperty, (object) (bool) (value ? 1 : 0));
      }
    }

    public Brush GridBrush
    {
      get
      {
        return (Brush) this.GetValue(EaseCurveEditor.GridBrushProperty);
      }
      set
      {
        this.SetValue(EaseCurveEditor.GridBrushProperty, (object) value);
      }
    }

    public Brush ControlPointBrush
    {
      get
      {
        return (Brush) this.GetValue(EaseCurveEditor.ControlPointBrushProperty);
      }
      set
      {
        this.SetValue(EaseCurveEditor.ControlPointBrushProperty, (object) value);
      }
    }

    static EaseCurveEditor()
    {
      Control.ForegroundProperty.AddOwner(typeof (EaseCurveEditor), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(EaseCurveEditor.BrushPropertyChanged)));
    }

    public EaseCurveEditor()
    {
      this.InitializeComponent();
      this.ToolTip = (object) null;
    }

    private static void PointPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      EaseCurveEditor easeCurveEditor = d as EaseCurveEditor;
      if (easeCurveEditor == null)
        return;
      easeCurveEditor.pointPositionsInvalid = true;
    }

    private static void BrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      EaseCurveEditor easeCurveEditor = d as EaseCurveEditor;
      if (easeCurveEditor == null)
        return;
      easeCurveEditor.curvePen = (Pen) null;
      easeCurveEditor.linePen = (Pen) null;
      easeCurveEditor.brushOne = (Brush) null;
      easeCurveEditor.brushFive = (Brush) null;
      easeCurveEditor.brushTen = (Brush) null;
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
      base.OnMouseLeftButtonDown(e);
      Point position = e.MouseDevice.GetPosition((IInputElement) this);
      if (this.controlPoint1Editor.HitTest(position.X, position.Y))
        this.activePointEditor = this.controlPoint1Editor;
      if (this.controlPoint2Editor.HitTest(position.X, position.Y))
        this.activePointEditor = this.controlPoint2Editor;
      if (this.activePointEditor == null)
        return;
      if (!this.CaptureMouse())
        this.activePointEditor = (ControlPointEditor) null;
      else
        ValueEditorUtils.ExecuteCommand(this.BeginEditCommand, (IInputElement) this, (object) null);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
      base.OnMouseMove(e);
      if (this.activePointEditor == null)
        return;
      double actualWidth = this.ActualWidth;
      double actualHeight = this.ActualHeight;
      Point position = e.MouseDevice.GetPosition((IInputElement) this);
      double x = Math.Min(Math.Max(position.X, 0.0), actualWidth);
      double y = Math.Min(Math.Max(position.Y, 0.0), actualHeight);
      if (this.activePointEditor == this.controlPoint1Editor)
      {
        this.X1 = Math.Round(this.XToNormalizedX(x, actualWidth), EaseCurveEditor.maxPrecision);
        this.Y1 = Math.Round(this.YToNormalizedY(y, actualHeight), EaseCurveEditor.maxPrecision);
      }
      else
      {
        this.X2 = Math.Round(this.XToNormalizedX(x, actualWidth), EaseCurveEditor.maxPrecision);
        this.Y2 = Math.Round(this.YToNormalizedY(y, actualHeight), EaseCurveEditor.maxPrecision);
      }
    }

    protected override void OnMouseUp(MouseButtonEventArgs e)
    {
      base.OnMouseUp(e);
      this.ReleaseMouseCapture();
    }

    protected override void OnLostMouseCapture(MouseEventArgs e)
    {
      base.OnLostMouseCapture(e);
      this.activePointEditor = (ControlPointEditor) null;
      ValueEditorUtils.ExecuteCommand(this.CommitEditCommand, (IInputElement) this, (object) null);
    }

    private double NormalizedXToX(double normalizedX, double width)
    {
      return normalizedX * width;
    }

    private double NormalizedYToY(double normalizedY, double height)
    {
      return (1.0 - normalizedY) * height;
    }

    private double XToNormalizedX(double x, double width)
    {
      return x / width;
    }

    private double YToNormalizedY(double y, double height)
    {
      return 1.0 - y / height;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
      this.pointPositionsInvalid = true;
      return base.ArrangeOverride(finalSize);
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
      base.OnRender(drawingContext);
      if (this.curvePen == null)
      {
        this.curvePen = new Pen(this.Foreground, 1.0);
        this.curvePen.Freeze();
      }
      if (this.ControlPointBrush != null && this.linePen == null)
      {
        this.linePen = new Pen(this.ControlPointBrush, 1.0);
        this.linePen.Freeze();
      }
      double actualWidth = this.ActualWidth;
      double actualHeight = this.ActualHeight;
      if (this.Background != null)
        drawingContext.DrawRectangle(this.Background, (Pen) null, new Rect(0.0, 0.0, actualWidth, actualHeight));
      if (this.GridBrush != null)
      {
        if (this.brushOne == null)
        {
          this.brushOne = this.GridBrush.Clone();
          this.brushOne.Opacity = 0.1;
          this.brushOne.Freeze();
        }
        if (this.brushFive == null)
        {
          this.brushFive = this.GridBrush.Clone();
          this.brushFive.Opacity = 0.25;
          this.brushFive.Freeze();
        }
        if (this.brushTen == null)
        {
          this.brushTen = this.GridBrush.Clone();
          this.brushTen.Opacity = 0.5;
          this.brushTen.Freeze();
        }
        Size size = new Size(actualWidth, actualHeight);
        double num1 = size.Width / 10.0;
        Point point = new Point(-1.0, -1.0);
        int num2 = (int) Math.Floor((0.0 - point.X) / num1);
        int num3 = (int) Math.Ceiling((size.Width - point.X) / num1);
        for (int index = num2; index <= num3; ++index)
        {
          Brush brush = index % 10 == 0 ? this.brushTen : (index % 5 == 0 ? this.brushFive : this.brushOne);
          double x = point.X + (double) index * num1;
          drawingContext.DrawRectangle(brush, (Pen) null, new Rect(x, 0.0, 1.0, size.Height));
        }
        double num4 = size.Height / 10.0;
        int num5 = (int) Math.Floor((0.0 - point.Y) / num4);
        int num6 = (int) Math.Ceiling((size.Height - point.Y) / num4);
        for (int index = num5; index <= num6; ++index)
        {
          Brush brush = index % 10 == 0 ? this.brushTen : (index % 5 == 0 ? this.brushFive : this.brushOne);
          double y = point.Y + (double) index * num4;
          drawingContext.DrawRectangle(brush, (Pen) null, new Rect(0.0, y, size.Width, 1.0));
        }
      }
      if (this.pointPositionsInvalid)
      {
        this.controlPoint1Editor.CenterX = this.NormalizedXToX(this.X1, actualWidth);
        this.controlPoint1Editor.CenterY = this.NormalizedYToY(this.Y1, actualHeight);
        this.controlPoint2Editor.CenterX = this.NormalizedXToX(this.X2, actualWidth);
        this.controlPoint2Editor.CenterY = this.NormalizedYToY(this.Y2, actualHeight);
        this.pathGeometry = new StreamGeometry();
        StreamGeometryContext streamGeometryContext = this.pathGeometry.Open();
        streamGeometryContext.BeginFigure(new Point(-0.5, actualHeight - 0.5), false, false);
        streamGeometryContext.BezierTo(new Point(this.controlPoint1Editor.CenterX - 0.5, this.controlPoint1Editor.CenterY - 0.5), new Point(this.controlPoint2Editor.CenterX - 0.5, this.controlPoint2Editor.CenterY - 0.5), new Point(actualWidth - 0.5, -0.5), true, false);
        streamGeometryContext.Close();
        this.pathGeometry.Freeze();
        this.pointPositionsInvalid = false;
      }
      if (!this.X1IsNinched && !this.Y1IsNinched && (!this.X2IsNinched && !this.Y2IsNinched))
        drawingContext.DrawGeometry((Brush) null, this.curvePen, (Geometry) this.pathGeometry);
      if (this.linePen != null)
      {
        drawingContext.DrawLine(this.linePen, new Point(-0.5, actualHeight - 0.5), new Point(this.controlPoint1Editor.CenterX - 0.5, this.controlPoint1Editor.CenterY - 0.5));
        drawingContext.DrawLine(this.linePen, new Point(actualWidth - 0.5, -0.5), new Point(this.controlPoint2Editor.CenterX - 0.5, this.controlPoint2Editor.CenterY - 0.5));
      }
      this.controlPoint1Editor.OnRender(drawingContext, this);
      this.controlPoint2Editor.OnRender(drawingContext, this);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/propertyinspector/categoryeditors/easing/easecurveeditor.xaml", UriKind.Relative));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerNonUserCode]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      this._contentLoaded = true;
    }
  }
}
