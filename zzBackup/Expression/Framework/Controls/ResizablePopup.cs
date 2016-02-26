// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Controls.ResizablePopup
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.Configuration;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.Framework.Controls
{
  public abstract class ResizablePopup : WorkaroundPopup
  {
    private Size defaultSize = Size.Empty;
    private ContentControl contentControl;
    private IConfigurationObject configuration;
    private string widthConfigurationProperty;
    private string heightConfigurationProperty;

    protected abstract Size MinSize { get; }

    protected ContentControl ContentControl
    {
      get
      {
        return this.contentControl;
      }
    }

    protected double? ResizeGripOpacity { get; set; }

    protected ResizablePopup(ContentControl contentControl, IConfigurationObject configuration, string configurationName)
    {
      this.contentControl = contentControl;
      this.configuration = configuration;
      this.widthConfigurationProperty = configurationName + "Width";
      this.heightConfigurationProperty = configurationName + "Height";
    }

    protected ResizablePopup(ContentControl contentControl, IConfigurationObject configuration, string configurationName, Size defaultSize)
      : this(contentControl, configuration, configurationName)
    {
      this.defaultSize = defaultSize;
    }

    protected override void OnInitialized(EventArgs e)
    {
      base.OnInitialized(e);
      Thumb thumb = new Thumb();
      thumb.HorizontalAlignment = HorizontalAlignment.Right;
      thumb.VerticalAlignment = VerticalAlignment.Bottom;
      thumb.DragDelta += new DragDeltaEventHandler(this.ResizePopup);
      thumb.Cursor = Cursors.SizeNWSE;
      thumb.Style = (Style) this.FindResource((object) "DefaultGrip");
      thumb.Margin = new Thickness(0.0, 0.0, 3.0, 3.0);
      thumb.Focusable = this.ContentControl.Focusable;
      if (this.ResizeGripOpacity.HasValue)
        thumb.Opacity = this.ResizeGripOpacity.Value;
      Grid grid = (Grid) new ResizablePopup.PopupGrid();
      double val2_1 = this.MinSize.Width;
      double val2_2 = this.MinSize.Height;
      if (!this.defaultSize.IsEmpty)
      {
        val2_1 = Math.Max(this.defaultSize.Width, val2_1);
        val2_2 = Math.Max(this.defaultSize.Height, val2_2);
      }
      double d1 = (double) this.configuration.GetProperty(this.widthConfigurationProperty, (object) val2_1);
      double d2 = (double) this.configuration.GetProperty(this.heightConfigurationProperty, (object) val2_2);
      grid.Width = Math.Max(this.MinSize.Width, double.IsNaN(d1) || double.IsInfinity(d1) ? 0.0 : d1);
      grid.Height = Math.Max(this.MinSize.Height, double.IsNaN(d2) || double.IsInfinity(d2) ? 0.0 : d2);
      grid.Focusable = this.ContentControl.Focusable;
      grid.Children.Add((UIElement) this.ContentControl);
      grid.Children.Add((UIElement) thumb);
      KeyboardNavigation.SetIsTabStop((DependencyObject) grid, false);
      this.Child = (UIElement) grid;
      this.StaysOpen = false;
    }

    private void ResizePopup(object sender, DragDeltaEventArgs e)
    {
      FrameworkElement frameworkElement = this.Child as FrameworkElement;
      if (frameworkElement == null)
        return;
      frameworkElement.Width = Math.Max(this.MinSize.Width, frameworkElement.ActualWidth + e.HorizontalChange);
      frameworkElement.Height = Math.Max(this.MinSize.Height, frameworkElement.ActualHeight + e.VerticalChange);
      this.configuration.SetProperty(this.widthConfigurationProperty, (object) frameworkElement.Width);
      this.configuration.SetProperty(this.heightConfigurationProperty, (object) frameworkElement.Height);
    }

    private class PopupGrid : Grid
    {
      protected override Size ArrangeOverride(Size arrangeSize)
      {
        Visual visual = (Visual) this;
        FrameworkElement frameworkElement = (FrameworkElement) null;
        for (; visual != null; visual = VisualTreeHelper.GetParent((DependencyObject) visual) as Visual)
          frameworkElement = visual as FrameworkElement;
        if (frameworkElement != null)
          return base.ArrangeOverride(frameworkElement.DesiredSize);
        return base.ArrangeOverride(arrangeSize);
      }
    }
  }
}
