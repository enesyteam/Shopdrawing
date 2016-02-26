// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Controls.MultiSlider
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.ValueEditors;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.Framework.Controls
{
  public class MultiSlider : Selector
  {
    private static readonly double checkerboardSize = 10.0;
    public static readonly RoutedCommand Click = new RoutedCommand("Click", typeof (MultiSlider));
    public static readonly RoutedCommand DragOff = new RoutedCommand("DragOff", typeof (MultiSlider));
    public static readonly RoutedCommand DragOn = new RoutedCommand("DragOn", typeof (MultiSlider));
    public static readonly DependencyProperty BeginEditCommandProperty = DependencyProperty.Register("BeginEditCommand", typeof (ICommand), typeof (MultiSlider), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
    public static readonly DependencyProperty ContinueEditCommandProperty = DependencyProperty.Register("ContinueEditCommand", typeof (ICommand), typeof (MultiSlider), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
    public static readonly DependencyProperty EndEditCommandProperty = DependencyProperty.Register("EndEditCommand", typeof (ICommand), typeof (MultiSlider), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
    public static readonly DependencyProperty CancelEditCommandProperty = DependencyProperty.Register("CancelEditCommand", typeof (ICommand), typeof (MultiSlider), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
    private const float HitTolerance = 2f;
    private const float RemoveTolerance = 16f;
    public static readonly DependencyProperty MaximumProperty;
    public static readonly DependencyProperty MinimumProperty;
    public static readonly DependencyProperty MinimumItemCountProperty;
    private bool isButtonDown;
    private bool haveRemovedThumb;
    private Point lastMousePosition;

    public double Minimum
    {
      get
      {
        return (double) this.GetValue(MultiSlider.MinimumProperty);
      }
      set
      {
        this.SetValue(MultiSlider.MinimumProperty, (object) value);
      }
    }

    public double Maximum
    {
      get
      {
        return (double) this.GetValue(MultiSlider.MaximumProperty);
      }
      set
      {
        this.SetValue(MultiSlider.MaximumProperty, (object) value);
      }
    }

    public int MinimumItemCount
    {
      get
      {
        return (int) this.GetValue(MultiSlider.MinimumItemCountProperty);
      }
      set
      {
        this.SetValue(MultiSlider.MinimumItemCountProperty, (object) value);
      }
    }

    public ICommand BeginEditCommand
    {
      get
      {
        return (ICommand) this.GetValue(MultiSlider.BeginEditCommandProperty);
      }
      set
      {
        this.SetValue(MultiSlider.BeginEditCommandProperty, (object) value);
      }
    }

    public ICommand ContinueEditCommand
    {
      get
      {
        return (ICommand) this.GetValue(MultiSlider.ContinueEditCommandProperty);
      }
      set
      {
        this.SetValue(MultiSlider.ContinueEditCommandProperty, (object) value);
      }
    }

    public ICommand EndEditCommand
    {
      get
      {
        return (ICommand) this.GetValue(MultiSlider.EndEditCommandProperty);
      }
      set
      {
        this.SetValue(MultiSlider.EndEditCommandProperty, (object) value);
      }
    }

    public ICommand CancelEditCommand
    {
      get
      {
        return (ICommand) this.GetValue(MultiSlider.CancelEditCommandProperty);
      }
      set
      {
        this.SetValue(MultiSlider.CancelEditCommandProperty, (object) value);
      }
    }

    static MultiSlider()
    {
      FrameworkElement.StyleProperty.OverrideMetadata(typeof (MultiSlider), (PropertyMetadata) new FrameworkPropertyMetadata((object) MultiSlider.GetDefaultStyle()));
      MultiSlider.MinimumProperty = DependencyProperty.Register("Minimum", typeof (double), typeof (MultiSlider), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0.0)
      {
        Inherits = false,
        BindsTwoWayByDefault = true,
        IsNotDataBindable = false
      });
      MultiSlider.MaximumProperty = DependencyProperty.Register("Maximum", typeof (double), typeof (MultiSlider), (PropertyMetadata) new FrameworkPropertyMetadata((object) 100.0)
      {
        Inherits = false,
        BindsTwoWayByDefault = true,
        IsNotDataBindable = false
      });
      MultiSlider.MinimumItemCountProperty = DependencyProperty.Register("MinimumItemCount", typeof (int), typeof (MultiSlider), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0));
    }

    internal void NotifyItemUIRemoved(FrameworkElement item)
    {
      if (!(item is MultiSliderThumb))
        return;
      item.RemoveHandler(Mouse.MouseDownEvent, (Delegate) new MouseButtonEventHandler(this.Thumb_OnPointerButtonDown));
    }

    protected override DependencyObject GetContainerForItemOverride()
    {
      MultiSliderThumb multiSliderThumb = new MultiSliderThumb();
      multiSliderThumb.AddHandler(Mouse.MouseDownEvent, (Delegate) new MouseButtonEventHandler(this.Thumb_OnPointerButtonDown));
      return (DependencyObject) multiSliderThumb;
    }

    protected override bool IsItemItsOwnContainerOverride(object item)
    {
      return item is MultiSliderThumb;
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
      this.Focus();
      this.lastMousePosition = e.MouseDevice.GetPosition((IInputElement) this);
      if (this.Update(this.lastMousePosition, MultiSlider.SequencePosition.First))
      {
        this.isButtonDown = true;
        this.haveRemovedThumb = false;
        e.MouseDevice.Capture((IInputElement) this);
        e.Handled = true;
      }
      base.OnMouseLeftButtonDown(e);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
      if (e.LeftButton == MouseButtonState.Pressed && this.isButtonDown)
      {
        Point position = e.MouseDevice.GetPosition((IInputElement) this);
        if (position != this.lastMousePosition)
        {
          this.lastMousePosition = position;
          this.Update(position, MultiSlider.SequencePosition.Middle);
          e.Handled = true;
        }
      }
      base.OnMouseMove(e);
    }

    private void FinishEditing(bool commit)
    {
      this.isButtonDown = false;
      InputManager.Current.PostNotifyInput -= new NotifyInputEventHandler(this.Current_PostNotifyInput);
      ValueEditorUtils.ExecuteCommand(commit ? this.EndEditCommand : this.CancelEditCommand, (IInputElement) this, (object) null);
      if (!this.IsMouseCaptured)
        return;
      this.ReleaseMouseCapture();
    }

    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
      if (this.isButtonDown)
      {
        this.FinishEditing(true);
        e.Handled = true;
      }
      base.OnMouseLeftButtonUp(e);
    }

    protected override void OnLostMouseCapture(MouseEventArgs e)
    {
      if (this.isButtonDown)
        this.FinishEditing(false);
      base.OnLostMouseCapture(e);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
      base.OnKeyDown(e);
      if (!this.isButtonDown)
        return;
      e.Handled = true;
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
      base.OnKeyUp(e);
      if (!this.isButtonDown)
        return;
      e.Handled = true;
    }

    private void Current_PostNotifyInput(object sender, NotifyInputEventArgs e)
    {
      KeyEventArgs keyEventArgs = e.StagingItem.Input as KeyEventArgs;
      if (keyEventArgs == null || keyEventArgs.Key != Key.Escape || !this.isButtonDown)
        return;
      this.FinishEditing(false);
      keyEventArgs.Handled = true;
    }

    private int ElementIndex(MultiSliderThumb thumb)
    {
      return this.ItemContainerGenerator.IndexFromContainer((DependencyObject) thumb);
    }

    private void Thumb_OnPointerButtonDown(object sender, MouseButtonEventArgs args)
    {
      this.lastMousePosition = args.MouseDevice.GetPosition((IInputElement) this);
      MultiSliderThumb thumb = sender as MultiSliderThumb;
      if (args.ChangedButton != MouseButton.Left || thumb == null)
        return;
      this.Focus();
      this.haveRemovedThumb = false;
      this.SelectedIndex = this.ElementIndex(thumb);
      ValueEditorUtils.ExecuteCommand(this.BeginEditCommand, (IInputElement) this, (object) null);
      InputManager.Current.PostNotifyInput += new NotifyInputEventHandler(this.Current_PostNotifyInput);
      this.isButtonDown = true;
      args.MouseDevice.Capture((IInputElement) this);
      args.Handled = true;
    }

    private bool Update(Point pointerPosition, MultiSlider.SequencePosition sequencePosition)
    {
      if (sequencePosition == MultiSlider.SequencePosition.First)
      {
        if (pointerPosition.X < -2.0 || (pointerPosition.X > this.RenderSize.Width + 2.0 || pointerPosition.Y < -2.0) || pointerPosition.Y > this.RenderSize.Height + 2.0)
          return false;
      }
      else if (pointerPosition.Y > this.RenderSize.Height + 16.0)
      {
        if (!this.haveRemovedThumb && MultiSlider.DragOff.CanExecute((object) null, (IInputElement) this) && this.Items.Count > this.MinimumItemCount)
        {
          MultiSlider.DragOff.Execute((object) null, (IInputElement) this);
          this.haveRemovedThumb = true;
        }
        if (this.haveRemovedThumb)
          return true;
      }
      double minimum = this.Minimum;
      double maximum = this.Maximum;
      double num1 = pointerPosition.X / this.ActualWidth;
      double num2 = Math.Round(Math.Min(maximum, Math.Max(minimum, minimum + num1 * (maximum - minimum))), 3);
      if (sequencePosition == MultiSlider.SequencePosition.First)
      {
        ValueEditorUtils.ExecuteCommand(this.BeginEditCommand, (IInputElement) this, (object) null);
        if (MultiSlider.Click.CanExecute((object) num2, (IInputElement) this))
          MultiSlider.Click.Execute((object) num2, (IInputElement) this);
      }
      else if (this.haveRemovedThumb)
      {
        if (MultiSlider.DragOn.CanExecute((object) null, (IInputElement) this))
          MultiSlider.DragOn.Execute((object) null, (IInputElement) this);
        this.haveRemovedThumb = false;
      }
      if (sequencePosition == MultiSlider.SequencePosition.Middle)
      {
        MultiSliderThumb multiSliderThumb = this.ItemContainerGenerator.ContainerFromItem(this.SelectedItem) as MultiSliderThumb;
        if (multiSliderThumb != null && multiSliderThumb.SliderValue != num2)
        {
          multiSliderThumb.SliderValue = num2;
          ValueEditorUtils.ExecuteCommand(this.ContinueEditCommand, (IInputElement) this, (object) null);
        }
      }
      return true;
    }

    private static Style GetDefaultStyle()
    {
      Style style = new Style(typeof (MultiSlider));
      ControlTemplate controlTemplate = new ControlTemplate(typeof (MultiSlider));
      style.Setters.Add((SetterBase) new Setter(Control.TemplateProperty, (object) controlTemplate));
      Brush brush = MultiSliderThumb.MakeCheckerboardBrush(MultiSlider.checkerboardSize);
      style.Setters.Add((SetterBase) new Setter(Control.BackgroundProperty, (object) brush));
      FrameworkElementFactory frameworkElementFactory = new FrameworkElementFactory(typeof (Grid), "mainGrid");
      frameworkElementFactory.SetValue(UIElement.ClipToBoundsProperty, (object) false);
      frameworkElementFactory.SetValue(FrameworkElement.WidthProperty, (object) new TemplateBindingExtension(FrameworkElement.WidthProperty));
      frameworkElementFactory.SetValue(FrameworkElement.HeightProperty, (object) new TemplateBindingExtension(FrameworkElement.HeightProperty));
      controlTemplate.VisualTree = frameworkElementFactory;
      FrameworkElementFactory child1 = new FrameworkElementFactory(typeof (Border), "mainBorder");
      child1.SetValue(Border.BackgroundProperty, (object) new TemplateBindingExtension(Control.BackgroundProperty));
      child1.SetValue(Border.BorderBrushProperty, (object) new TemplateBindingExtension(Control.BorderBrushProperty));
      child1.SetValue(Border.BorderThicknessProperty, (object) new TemplateBindingExtension(Control.BorderThicknessProperty));
      frameworkElementFactory.AppendChild(child1);
      FrameworkElementFactory child2 = new FrameworkElementFactory(typeof (Border), "fillBorder");
      child2.SetValue(Border.BackgroundProperty, (object) new TemplateBindingExtension(Control.ForegroundProperty));
      child2.SetValue(Border.BorderBrushProperty, (object) Brushes.Transparent);
      child2.SetValue(Border.BorderThicknessProperty, (object) new TemplateBindingExtension(Control.BorderThicknessProperty));
      frameworkElementFactory.AppendChild(child2);
      FrameworkElementFactory child3 = new FrameworkElementFactory(typeof (Canvas), "itemsCanvas");
      child3.SetValue(UIElement.ClipToBoundsProperty, (object) false);
      child3.SetValue(Panel.IsItemsHostProperty, (object) true);
      frameworkElementFactory.AppendChild(child3);
      Trigger trigger = new Trigger();
      trigger.Property = UIElement.IsEnabledProperty;
      trigger.Value = (object) false;
      trigger.Setters.Add((SetterBase) new Setter(UIElement.OpacityProperty, (object) 0.5));
      style.Triggers.Add((TriggerBase) trigger);
      return style;
    }

    private enum SequencePosition
    {
      First,
      Middle,
      Last,
    }
  }
}
