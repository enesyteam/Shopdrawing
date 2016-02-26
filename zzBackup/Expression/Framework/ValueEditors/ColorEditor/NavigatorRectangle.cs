// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.ValueEditors.ColorEditor.NavigatorRectangle
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace Microsoft.Expression.Framework.ValueEditors.ColorEditor
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class NavigatorRectangle : Border, IComponentConnector
  {
    public static readonly DependencyProperty NavigateBeginCommandProperty = DependencyProperty.Register("NavigateBeginCommand", typeof (ICommand), typeof (NavigatorRectangle), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
    public static readonly DependencyProperty NavigateContinueCommandProperty = DependencyProperty.Register("NavigateContinueCommand", typeof (ICommand), typeof (NavigatorRectangle), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
    public static readonly DependencyProperty NavigateEndCommandProperty = DependencyProperty.Register("NavigateEndCommand", typeof (ICommand), typeof (NavigatorRectangle), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
    public static readonly DependencyProperty XValueProperty = DependencyProperty.Register("XValue", typeof (double), typeof (NavigatorRectangle), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(NavigatorRectangle.XValueInvalidated), (CoerceValueCallback) null));
    public static readonly DependencyProperty YValueProperty = DependencyProperty.Register("YValue", typeof (double), typeof (NavigatorRectangle), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(NavigatorRectangle.YValueInvalidated), (CoerceValueCallback) null));
    public static readonly DependencyProperty XMinimumProperty = DependencyProperty.Register("XMinimum", typeof (double), typeof (NavigatorRectangle), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0.0));
    public static readonly DependencyProperty XMaximumProperty = DependencyProperty.Register("XMaximum", typeof (double), typeof (NavigatorRectangle), (PropertyMetadata) new FrameworkPropertyMetadata((object) 1.0));
    public static readonly DependencyProperty YMinimumProperty = DependencyProperty.Register("YMinimum", typeof (double), typeof (NavigatorRectangle), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0.0));
    public static readonly DependencyProperty YMaximumProperty = DependencyProperty.Register("YMaximum", typeof (double), typeof (NavigatorRectangle), (PropertyMetadata) new FrameworkPropertyMetadata((object) 1.0));
    public static readonly DependencyProperty XOffsetProperty = DependencyProperty.Register("XOffset", typeof (double), typeof (NavigatorRectangle), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0.0));
    public static readonly DependencyProperty YOffsetProperty = DependencyProperty.Register("YOffset", typeof (double), typeof (NavigatorRectangle), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0.0));
    public static readonly DependencyProperty TrackGeometryProperty = DependencyProperty.Register("TrackGeometry", typeof (Geometry), typeof (NavigatorRectangle));
    public static readonly DependencyProperty TrackFillProperty = DependencyProperty.Register("TrackFill", typeof (Brush), typeof (NavigatorRectangle));
    public static readonly DependencyProperty TrackStrokeProperty = DependencyProperty.Register("TrackStroke", typeof (Brush), typeof (NavigatorRectangle));
    private bool isCurrentlySliding;
    private bool hasChanged;
    internal NavigatorRectangle NavigatorRectangleRoot;
    internal Canvas Track;
    private bool _contentLoaded;

    public ICommand NavigateBeginCommand
    {
      get
      {
        return (ICommand) this.GetValue(NavigatorRectangle.NavigateBeginCommandProperty);
      }
      set
      {
        this.SetValue(NavigatorRectangle.NavigateBeginCommandProperty, (object) value);
      }
    }

    public ICommand NavigateContinueCommand
    {
      get
      {
        return (ICommand) this.GetValue(NavigatorRectangle.NavigateContinueCommandProperty);
      }
      set
      {
        this.SetValue(NavigatorRectangle.NavigateContinueCommandProperty, (object) value);
      }
    }

    public ICommand NavigateEndCommand
    {
      get
      {
        return (ICommand) this.GetValue(NavigatorRectangle.NavigateEndCommandProperty);
      }
      set
      {
        this.SetValue(NavigatorRectangle.NavigateEndCommandProperty, (object) value);
      }
    }

    public double XMinimum
    {
      get
      {
        return (double) this.GetValue(NavigatorRectangle.XMinimumProperty);
      }
      set
      {
        this.SetValue(NavigatorRectangle.XMinimumProperty, (object) value);
      }
    }

    public double XMaximum
    {
      get
      {
        return (double) this.GetValue(NavigatorRectangle.XMaximumProperty);
      }
      set
      {
        this.SetValue(NavigatorRectangle.XMaximumProperty, (object) value);
      }
    }

    public double YMinimum
    {
      get
      {
        return (double) this.GetValue(NavigatorRectangle.YMinimumProperty);
      }
      set
      {
        this.SetValue(NavigatorRectangle.YMinimumProperty, (object) value);
      }
    }

    public double YMaximum
    {
      get
      {
        return (double) this.GetValue(NavigatorRectangle.YMaximumProperty);
      }
      set
      {
        this.SetValue(NavigatorRectangle.YMaximumProperty, (object) value);
      }
    }

    public double XValue
    {
      get
      {
        return (double) this.GetValue(NavigatorRectangle.XValueProperty);
      }
      set
      {
        this.SetValue(NavigatorRectangle.XValueProperty, (object) value);
      }
    }

    public double YValue
    {
      get
      {
        return (double) this.GetValue(NavigatorRectangle.YValueProperty);
      }
      set
      {
        this.SetValue(NavigatorRectangle.YValueProperty, (object) value);
      }
    }

    public double XOffset
    {
      get
      {
        return (double) this.GetValue(NavigatorRectangle.XOffsetProperty);
      }
      set
      {
        this.SetValue(NavigatorRectangle.XOffsetProperty, (object) value);
      }
    }

    public double YOffset
    {
      get
      {
        return (double) this.GetValue(NavigatorRectangle.YOffsetProperty);
      }
      set
      {
        this.SetValue(NavigatorRectangle.YOffsetProperty, (object) value);
      }
    }

    public Geometry TrackGeometry
    {
      get
      {
        return (Geometry) this.GetValue(NavigatorRectangle.TrackGeometryProperty);
      }
      set
      {
        this.SetValue(NavigatorRectangle.TrackGeometryProperty, (object) value);
      }
    }

    public Brush TrackFill
    {
      get
      {
        return (Brush) this.GetValue(NavigatorRectangle.TrackFillProperty);
      }
      set
      {
        this.SetValue(NavigatorRectangle.TrackFillProperty, (object) value);
      }
    }

    public Brush TrackStroke
    {
      get
      {
        return (Brush) this.GetValue(NavigatorRectangle.TrackStrokeProperty);
      }
      set
      {
        this.SetValue(NavigatorRectangle.TrackStrokeProperty, (object) value);
      }
    }

    public NavigatorRectangle()
    {
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.Framework;v" + (object) this.GetType().Assembly.GetName().Version + ";component/valueeditors/coloreditor/navigatorrectangle.xaml", UriKind.Relative));
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
      base.OnMouseLeftButtonDown(e);
      if (this.isCurrentlySliding)
        return;
      this.isCurrentlySliding = true;
      this.hasChanged = false;
      this.Focus();
      e.MouseDevice.Capture((IInputElement) this);
      e.Handled = true;
      this.UpdatePropertiesFromMousePosition(e.GetPosition((IInputElement) this.Track));
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
      base.OnMouseMove(e);
      if (!this.isCurrentlySliding)
        return;
      this.UpdatePropertiesFromMousePosition(e.GetPosition((IInputElement) this.Track));
      e.Handled = true;
    }

    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
      base.OnMouseLeftButtonUp(e);
      if (!this.isCurrentlySliding)
        return;
      this.isCurrentlySliding = false;
      e.MouseDevice.Capture((IInputElement) null);
      e.Handled = true;
      if (!this.hasChanged)
        return;
      this.ExecuteCommand(this.NavigateEndCommand);
    }

    protected override void OnLostMouseCapture(MouseEventArgs e)
    {
      base.OnLostMouseCapture(e);
      if (this == e.MouseDevice.Captured || !this.isCurrentlySliding)
        return;
      this.isCurrentlySliding = false;
      if (!this.hasChanged)
        return;
      this.ExecuteCommand(this.NavigateEndCommand);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
      base.OnKeyDown(e);
      if (!this.isCurrentlySliding)
        return;
      e.Handled = true;
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
      base.OnKeyUp(e);
      if (!this.isCurrentlySliding)
        return;
      e.Handled = true;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
      Size size = base.ArrangeOverride(finalSize);
      this.UpdateXOffset();
      this.UpdateYOffset();
      return size;
    }

    private void UpdatePropertiesFromMousePosition(Point position)
    {
      double num1 = NavigatorRectangle.ConvertToRange(position.X / this.Track.ActualWidth, this.XMinimum, this.XMaximum);
      double num2 = NavigatorRectangle.ConvertToRange(position.Y / this.Track.ActualHeight, this.YMinimum, this.YMaximum);
      if (this.XValue == num1 && this.YValue == num2)
        return;
      if (!this.hasChanged)
      {
        this.hasChanged = true;
        this.ExecuteCommand(this.NavigateBeginCommand);
      }
      if (this.XValue != num1)
        this.XValue = num1;
      if (this.YValue != num2)
        this.YValue = num2;
      this.ExecuteCommand(this.NavigateContinueCommand);
    }

    private static void XValueInvalidated(DependencyObject target, DependencyPropertyChangedEventArgs e)
    {
      NavigatorRectangle navigatorRectangle = target as NavigatorRectangle;
      if (navigatorRectangle == null)
        return;
      navigatorRectangle.UpdateXOffset();
    }

    private static void YValueInvalidated(DependencyObject target, DependencyPropertyChangedEventArgs e)
    {
      NavigatorRectangle navigatorRectangle = target as NavigatorRectangle;
      if (navigatorRectangle == null)
        return;
      navigatorRectangle.UpdateYOffset();
    }

    private void UpdateXOffset()
    {
      this.XOffset = NavigatorRectangle.ConvertFromRange(this.XValue, this.XMinimum, this.XMaximum) * (this.Track != null ? this.Track.ActualWidth : 0.0);
    }

    private void UpdateYOffset()
    {
      this.YOffset = NavigatorRectangle.ConvertFromRange(this.YValue, this.YMinimum, this.YMaximum) * (this.Track != null ? this.Track.ActualHeight : 0.0);
    }

    private static double ConvertToRange(double value, double minimum, double maximum)
    {
      return Math.Min(maximum, Math.Max(minimum, minimum + value * (maximum - minimum)));
    }

    private static double ConvertFromRange(double value, double minimum, double maximum)
    {
      if (minimum != maximum)
        return (value - minimum) / (maximum - minimum);
      return 0.5;
    }

    private void ExecuteCommand(ICommand commandToExecute)
    {
      if (commandToExecute == null || !commandToExecute.CanExecute((object) null))
        return;
      commandToExecute.Execute((object) null);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.Framework;component/valueeditors/coloreditor/navigatorrectangle.xaml", UriKind.Relative));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerNonUserCode]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.NavigatorRectangleRoot = (NavigatorRectangle) target;
          break;
        case 2:
          this.Track = (Canvas) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
