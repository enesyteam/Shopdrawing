// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI.TimeMarkerControl
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI
{
  public class TimeMarkerControl : Control
  {
    private double markerSpacingTime = 1.0;
    private int markerIntervalIndex = 2;
    private List<FormattedText> timeFormattedTexts = new List<FormattedText>();
    public static readonly DependencyProperty ControllingViewerProperty = DependencyProperty.Register("ControllingViewer", typeof (ScrollViewer), typeof (TimeMarkerControl));
    public static readonly DependencyProperty UnitsPerSecondProperty = DependencyProperty.Register("UnitsPerSecond", typeof (double), typeof (TimeMarkerControl));
    public static readonly DependencyProperty DisplayWidthProperty = DependencyProperty.Register("DisplayWidth", typeof (double), typeof (TimeMarkerControl));
    public static readonly DependencyProperty BigTickBrushProperty = DependencyProperty.Register("BigTickBrush", typeof (Brush), typeof (TimeMarkerControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(TimeMarkerControl.BigTickBrushChanged)));
    public static readonly DependencyProperty TickBrushProperty = DependencyProperty.Register("TickBrush", typeof (Brush), typeof (TimeMarkerControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(TimeMarkerControl.TickBrushChanged)));
    public static readonly DependencyProperty TickTopProperty = DependencyProperty.Register("TickTop", typeof (double), typeof (TimeMarkerControl));
    public static readonly DependencyProperty TickHeightProperty = DependencyProperty.Register("TickHeight", typeof (double), typeof (TimeMarkerControl));
    private static readonly double[] defaultMarkerTimeIntervals = new double[6]
    {
      5.0,
      2.0,
      1.0,
      0.5,
      0.25,
      0.1
    };
    private const double minimumMarkerSpacing = 50.0;
    private const double maximumMarkerSpacing = 125.0;
    private const double widthBuffer = 500.0;
    private const double minimumTicksPerInterval = 4.0;
    private const double maximumTickSeparation = 20.0;
    private ScrollViewer controllingViewer;
    private double minimumMarkerTime;
    private int markerCount;
    private Pen tickPen;
    private Pen bigTickPen;
    private double unitsPerSecond;
    private bool isUnitsPerSecondValid;
    private double displayWidth;
    private bool isDisplayWidthValid;
    private bool isTimeFormattedTextsValid;

    public double DisplayWidth
    {
      get
      {
        if (!this.isDisplayWidthValid)
        {
          this.displayWidth = (double) this.GetValue(TimeMarkerControl.DisplayWidthProperty);
          this.isDisplayWidthValid = true;
        }
        return this.displayWidth;
      }
      set
      {
        this.SetValue(TimeMarkerControl.DisplayWidthProperty, (object) value);
      }
    }

    public ScrollViewer ControllingViewer
    {
      get
      {
        return (ScrollViewer) this.GetValue(TimeMarkerControl.ControllingViewerProperty);
      }
      set
      {
        this.SetValue(TimeMarkerControl.ControllingViewerProperty, (object) value);
      }
    }

    public double UnitsPerSecond
    {
      get
      {
        if (!this.isUnitsPerSecondValid)
        {
          this.unitsPerSecond = (double) this.GetValue(TimeMarkerControl.UnitsPerSecondProperty);
          this.isUnitsPerSecondValid = true;
        }
        return this.unitsPerSecond;
      }
      set
      {
        this.SetValue(TimeMarkerControl.UnitsPerSecondProperty, (object) value);
      }
    }

    public Brush TickBrush
    {
      get
      {
        return (Brush) this.GetValue(TimeMarkerControl.TickBrushProperty);
      }
      set
      {
        this.SetValue(TimeMarkerControl.TickBrushProperty, (object) value);
      }
    }

    public Brush BigTickBrush
    {
      get
      {
        return (Brush) this.GetValue(TimeMarkerControl.BigTickBrushProperty);
      }
      set
      {
        this.SetValue(TimeMarkerControl.BigTickBrushProperty, (object) value);
      }
    }

    public double TickTop
    {
      get
      {
        return (double) this.GetValue(TimeMarkerControl.TickTopProperty);
      }
      set
      {
        this.SetValue(TimeMarkerControl.TickTopProperty, (object) value);
      }
    }

    public double TickHeight
    {
      get
      {
        return (double) this.GetValue(TimeMarkerControl.TickHeightProperty);
      }
      set
      {
        this.SetValue(TimeMarkerControl.TickHeightProperty, (object) value);
      }
    }

    private Pen TickPen
    {
      get
      {
        if (this.tickPen == null && this.TickBrush != null)
        {
          this.tickPen = new Pen(this.TickBrush, 1.0);
          this.tickPen.Freeze();
        }
        return this.tickPen;
      }
    }

    private Pen BigTickPen
    {
      get
      {
        if (this.bigTickPen == null && this.BigTickBrush != null)
        {
          this.bigTickPen = new Pen(this.BigTickBrush, 1.0);
          this.bigTickPen.Freeze();
        }
        return this.bigTickPen;
      }
    }

    private double WidthToCover
    {
      get
      {
        return this.DisplayWidth + 500.0;
      }
    }

    private double MinimumMarkerTime
    {
      get
      {
        return this.minimumMarkerTime;
      }
    }

    private double MaximumMarkerTime
    {
      get
      {
        return this.minimumMarkerTime + this.CoveredTime;
      }
    }

    private double MarkerSpacingTime
    {
      get
      {
        return this.markerSpacingTime;
      }
    }

    private double MarkerSpacing
    {
      get
      {
        return this.MarkerSpacingTime * this.UnitsPerSecond;
      }
    }

    private double CoveredWidth
    {
      get
      {
        return this.MarkerSpacing * (double) (this.markerCount - 1);
      }
    }

    private double CoveredTime
    {
      get
      {
        return this.MarkerSpacingTime * (double) (this.markerCount - 1);
      }
    }

    private double DisplayTime
    {
      get
      {
        return this.DisplayWidth / this.UnitsPerSecond;
      }
    }

    static TimeMarkerControl()
    {
      FrameworkPropertyMetadata propertyMetadata1 = new FrameworkPropertyMetadata();
      propertyMetadata1.DefaultValue = (object) null;
      propertyMetadata1.PropertyChangedCallback = new PropertyChangedCallback(TimeMarkerControl.ControllingViewerInvalidated);
      TimeMarkerControl.ControllingViewerProperty.OverrideMetadata(typeof (TimeMarkerControl), (PropertyMetadata) propertyMetadata1);
      FrameworkPropertyMetadata propertyMetadata2 = new FrameworkPropertyMetadata();
      propertyMetadata2.DefaultValue = (object) 50.0;
      propertyMetadata2.PropertyChangedCallback = new PropertyChangedCallback(TimeMarkerControl.UnitsPerSecondInvalidated);
      TimeMarkerControl.UnitsPerSecondProperty.OverrideMetadata(typeof (TimeMarkerControl), (PropertyMetadata) propertyMetadata2);
      FrameworkPropertyMetadata propertyMetadata3 = new FrameworkPropertyMetadata();
      propertyMetadata3.DefaultValue = (object) 1500.0;
      propertyMetadata3.PropertyChangedCallback = new PropertyChangedCallback(TimeMarkerControl.DisplayWidthInvalidated);
      TimeMarkerControl.DisplayWidthProperty.OverrideMetadata(typeof (TimeMarkerControl), (PropertyMetadata) propertyMetadata3);
      FrameworkPropertyMetadata propertyMetadata4 = new FrameworkPropertyMetadata();
      propertyMetadata4.DefaultValue = (object) 0.0;
      propertyMetadata4.AffectsRender = true;
      TimeMarkerControl.TickTopProperty.OverrideMetadata(typeof (TimeMarkerControl), (PropertyMetadata) propertyMetadata4);
      FrameworkPropertyMetadata propertyMetadata5 = new FrameworkPropertyMetadata();
      propertyMetadata5.DefaultValue = (object) 10.0;
      propertyMetadata5.AffectsRender = true;
      TimeMarkerControl.TickHeightProperty.OverrideMetadata(typeof (TimeMarkerControl), (PropertyMetadata) propertyMetadata5);
    }

    public TimeMarkerControl()
    {
      this.RebuildMarkers();
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
      double num1 = 4.0;
      double num2;
      for (num2 = this.MarkerSpacing / num1; num2 > 20.0; num2 = this.MarkerSpacing / num1)
        num1 *= 2.0;
      Point point0 = new Point(0.0, this.TickTop);
      Point point1 = new Point(0.0, point0.Y + this.TickHeight);
      Pen tickPen = this.TickPen;
      double num3 = this.minimumMarkerTime * this.UnitsPerSecond + 10.0;
      if (tickPen != null)
      {
        for (double num4 = 0.0; num4 < (double) this.markerCount * num1; ++num4)
        {
          point0.X = num3 + num4 * num2;
          point1.X = point0.X;
          drawingContext.DrawLine(tickPen, point0, point1);
        }
      }
      Point origin = new Point(0.0, -4.0);
      point0.Y = point1.Y;
      point1.Y = this.RenderSize.Height;
      Pen bigTickPen = this.BigTickPen;
      this.ValidateTimeMarkerTexts();
      for (int index = 0; index < this.markerCount; ++index)
      {
        point0.X = num3 + (double) index * this.MarkerSpacing;
        point1.X = point0.X;
        origin.X = point0.X;
        if (bigTickPen != null)
          drawingContext.DrawLine(bigTickPen, point0, point1);
        drawingContext.DrawText(this.timeFormattedTexts[index], origin);
      }
    }

    private static void TickBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      TimeMarkerControl timeMarkerControl = d as TimeMarkerControl;
      if (timeMarkerControl == null)
        return;
      timeMarkerControl.tickPen = (Pen) null;
      timeMarkerControl.isTimeFormattedTextsValid = false;
    }

    private static void BigTickBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      TimeMarkerControl timeMarkerControl = d as TimeMarkerControl;
      if (timeMarkerControl == null)
        return;
      timeMarkerControl.bigTickPen = (Pen) null;
      timeMarkerControl.isTimeFormattedTextsValid = false;
    }

    private void RebuildMarkers()
    {
      while (this.MarkerSpacing < 50.0)
      {
        --this.markerIntervalIndex;
        if (this.markerIntervalIndex >= 0 && this.markerIntervalIndex < TimeMarkerControl.defaultMarkerTimeIntervals.Length)
          this.markerSpacingTime = TimeMarkerControl.defaultMarkerTimeIntervals[this.markerIntervalIndex];
        else
          this.markerSpacingTime *= 2.0;
      }
      while (this.MarkerSpacing > 125.0)
      {
        ++this.markerIntervalIndex;
        if (this.markerIntervalIndex >= 0 && this.markerIntervalIndex < TimeMarkerControl.defaultMarkerTimeIntervals.Length)
          this.markerSpacingTime = TimeMarkerControl.defaultMarkerTimeIntervals[this.markerIntervalIndex];
        else
          this.markerSpacingTime /= 2.0;
      }
      this.markerCount = (int) (this.WidthToCover / this.MarkerSpacing);
      this.minimumMarkerTime = 0.0;
    }

    internal void UpdateMarkers(bool forceUpdate)
    {
      if (this.controllingViewer == null)
        return;
      bool flag = forceUpdate;
      if ((this.CoveredTime - this.DisplayTime) / 2.0 < this.MarkerSpacingTime)
      {
        this.RebuildMarkers();
        flag = true;
      }
      double num = this.controllingViewer.HorizontalOffset / this.UnitsPerSecond;
      if (num < this.minimumMarkerTime || num > this.minimumMarkerTime + (this.CoveredTime - this.DisplayTime))
      {
        this.minimumMarkerTime = Math.Max(num - (this.CoveredTime - this.DisplayTime) / 2.0, 0.0);
        this.minimumMarkerTime = this.Snap(this.minimumMarkerTime, this.MarkerSpacingTime);
        flag = true;
      }
      if (!flag)
        return;
      this.isTimeFormattedTextsValid = false;
      this.InvalidateVisual();
    }

    private void ValidateTimeMarkerTexts()
    {
      if (this.isTimeFormattedTextsValid)
        return;
      CultureInfo currentCulture = CultureInfo.CurrentCulture;
      Typeface typeface = new Typeface(this.FontFamily, this.FontStyle, this.FontWeight, this.FontStretch);
      double fontSize = this.FontSize;
      Brush foreground = this.Foreground;
      this.timeFormattedTexts.Clear();
      for (int index = 0; index < this.markerCount; ++index)
      {
        double num1 = this.MinimumMarkerTime + (double) index * this.MarkerSpacingTime;
        string textToFormat;
        if (num1 > 60.0)
        {
          int num2 = (int) num1 / 60;
          double num3 = num1 - (double) (60 * num2);
          textToFormat = num2.ToString("0", (IFormatProvider) currentCulture) + currentCulture.DateTimeFormat.TimeSeparator + num3.ToString("00.###", (IFormatProvider) currentCulture);
        }
        else
          textToFormat = num1.ToString("0.###", (IFormatProvider) currentCulture);
        this.timeFormattedTexts.Add(new FormattedText(textToFormat, currentCulture, FlowDirection.LeftToRight, typeface, fontSize, foreground)
        {
          TextAlignment = TextAlignment.Center
        });
      }
      this.isTimeFormattedTextsValid = true;
    }

    private double Snap(double time, double resolution)
    {
      return resolution * (double) (int) (time / resolution);
    }

    private void ControllingViewerChanged()
    {
      if (this.controllingViewer != null)
        this.controllingViewer.ScrollChanged -= new ScrollChangedEventHandler(this.ControllingViewer_ScrollChange);
      this.controllingViewer = (ScrollViewer) this.GetValue(TimeMarkerControl.ControllingViewerProperty);
      if (this.controllingViewer != null)
        this.controllingViewer.ScrollChanged += new ScrollChangedEventHandler(this.ControllingViewer_ScrollChange);
      this.UpdateMarkers(false);
    }

    private void UnitsPerSecondChanged()
    {
      if (this.MarkerSpacing > 125.0 || this.MarkerSpacing < 50.0)
        this.RebuildMarkers();
      this.UpdateMarkers(true);
    }

    private void DisplayWidthChanged()
    {
      this.UpdateMarkers(false);
    }

    private void ControllingViewer_ScrollChange(object sender, ScrollChangedEventArgs e)
    {
      this.UpdateMarkers(false);
    }

    private static void ControllingViewerInvalidated(DependencyObject target, DependencyPropertyChangedEventArgs e)
    {
      ((TimeMarkerControl) target).ControllingViewerChanged();
    }

    private static void UnitsPerSecondInvalidated(DependencyObject target, DependencyPropertyChangedEventArgs e)
    {
      TimeMarkerControl timeMarkerControl = (TimeMarkerControl) target;
      timeMarkerControl.isUnitsPerSecondValid = false;
      timeMarkerControl.UnitsPerSecondChanged();
    }

    private static void DisplayWidthInvalidated(DependencyObject target, DependencyPropertyChangedEventArgs e)
    {
      TimeMarkerControl timeMarkerControl = (TimeMarkerControl) target;
      timeMarkerControl.isDisplayWidthValid = false;
      timeMarkerControl.DisplayWidthChanged();
    }
  }
}
