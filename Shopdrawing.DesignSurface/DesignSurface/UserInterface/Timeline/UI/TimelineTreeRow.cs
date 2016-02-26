// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI.TimelineTreeRow
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Timeline.DragDrop;
using Microsoft.Expression.DesignSurface.Tools.Path;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.UserInterface.Timeline;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.ValueEditors;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI
{
  public class TimelineTreeRow : Grid
  {
    public static readonly DependencyProperty RenameCommandProperty = DependencyProperty.Register("RenameCommand", typeof (ICommand), typeof (TimelineTreeRow), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(TimelineTreeRow.OnRenameCommandChanged)));
    public static readonly DependencyProperty PositionProperty = DependencyProperty.RegisterAttached("Position", typeof (TimelineTreeRowPosition), typeof (TimelineTreeRow), (PropertyMetadata) new FrameworkPropertyMetadata((object) TimelineTreeRowPosition.Left, new PropertyChangedCallback(TimelineTreeRow.OnAttachedPositionPropertyChanged)));
    public static readonly DependencyProperty IndentProperty = DependencyProperty.Register("Indent", typeof (double), typeof (TimelineTreeRow), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0.0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));
    public static readonly DependencyProperty BackgroundHighlightedProperty = DependencyProperty.Register("BackgroundHighlighted", typeof (bool), typeof (TimelineTreeRow), new PropertyMetadata((object) false));
    public static readonly RoutedCommand EnsureSelectCommand = new RoutedCommand("EnsureSelect", typeof (TimelineTreeRow));
    public static readonly RoutedCommand ExpandCollapseCommand;
    private double rightColumnWidth;
    private TimelineItem timelineItem;
    private ModifierKeys mouseDownModifiers;
    private bool handledMouseDown;
    private bool gotMouseDown;
    private PathAdornerSet highlightedPathAdornerSet;

    public bool BackgroundHighlighted
    {
      get
      {
        return (bool) this.GetValue(TimelineTreeRow.BackgroundHighlightedProperty);
      }
      set
      {
        this.SetValue(TimelineTreeRow.BackgroundHighlightedProperty, (object) (bool) (value ? true : false));
      }
    }

    public double Indent
    {
      get
      {
        return (double) this.GetValue(TimelineTreeRow.IndentProperty);
      }
      set
      {
        this.SetValue(TimelineTreeRow.IndentProperty, (object) value);
      }
    }

    private double ViewportOffset
    {
      get
      {
        return this.TimelineItem.ViewportOffset;
      }
    }

    private double ViewportWidth
    {
      get
      {
        return this.TimelineItem.ViewportWidth;
      }
    }

    public ICommand RenameCommand
    {
      get
      {
        return (ICommand) this.GetValue(TimelineTreeRow.RenameCommandProperty);
      }
      set
      {
        this.SetValue(TimelineTreeRow.RenameCommandProperty, (object) value);
      }
    }

    public virtual IDragDropHandler DragDropHandler
    {
      get
      {
        return (IDragDropHandler) new TreeRowDragDropHandler(this);
      }
    }

    protected virtual bool CustomLayout
    {
      get
      {
        return false;
      }
    }

    internal bool IsEditingTitle
    {
      get
      {
        StringEditor stringEditor = this.FindName("ItemTitle") as StringEditor;
        return stringEditor != null && stringEditor.IsEditing;
      }
    }

    public virtual TimelineItem TimelineItem
    {
      get
      {
        return this.timelineItem;
      }
      set
      {
        this.timelineItem = value;
      }
    }

    static TimelineTreeRow()
    {
      TimelineTreeRow.EnsureSelectCommand.InputGestures.Add((InputGesture) new MouseGesture(MouseAction.RightClick));
      TimelineTreeRow.ExpandCollapseCommand = new RoutedCommand("ExpandCollapse", typeof (TimelineTreeRow));
      EventManager.RegisterClassHandler(typeof (TimelineTreeRow), Mouse.MouseUpEvent, (Delegate) new MouseButtonEventHandler(TimelineTreeRow.OnMouseUp), true);
      EventManager.RegisterClassHandler(typeof (TimelineTreeRow), FrameworkElement.RequestBringIntoViewEvent, (Delegate) new RequestBringIntoViewEventHandler(TimelineTreeRow.HandleBringIntoView));
    }

    public TimelineTreeRow()
    {
      this.DataContextChanged += new DependencyPropertyChangedEventHandler(this.This_DataContextChanged);
      this.CommandBindings.Add(new CommandBinding((ICommand) TimelineTreeRow.EnsureSelectCommand, new ExecutedRoutedEventHandler(this.OnEnsureSelectCommand)));
      this.CommandBindings.Add(new CommandBinding((ICommand) TimelineTreeRow.ExpandCollapseCommand, new ExecutedRoutedEventHandler(this.OnExpandCollapseCommand)));
    }

    public static void SetPosition(DependencyObject target, TimelineTreeRowPosition position)
    {
      target.SetValue(TimelineTreeRow.PositionProperty, (object) position);
    }

    public static TimelineTreeRowPosition GetPosition(DependencyObject target)
    {
      return (TimelineTreeRowPosition) target.GetValue(TimelineTreeRow.PositionProperty);
    }

    public static void OnAttachedPositionPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
      Visual visual = o as Visual;
      if (visual == null)
        return;
      ElementTreeRow elementTreeRow = VisualTreeHelper.GetParent((DependencyObject) visual) as ElementTreeRow;
      if (elementTreeRow == null)
        return;
      elementTreeRow.InvalidateMeasure();
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      if (!this.CustomLayout)
        return base.MeasureOverride(availableSize);
      UIElementCollection internalChildren = this.InternalChildren;
      Size size1 = new Size(double.PositiveInfinity, availableSize.Height);
      Size size2 = new Size(this.ViewportWidth, availableSize.Height);
      double height = 0.0;
      double indent = this.Indent;
      this.rightColumnWidth = 0.0;
      for (int index = 0; index < internalChildren.Count; ++index)
      {
        UIElement uiElement = internalChildren[index];
        if (uiElement.Visibility != Visibility.Collapsed)
        {
          TimelineTreeRowPosition position = TimelineTreeRow.GetPosition((DependencyObject) uiElement);
          Size availableSize1 = position != TimelineTreeRowPosition.Border ? size1 : size2;
          uiElement.Measure(availableSize1);
          Size desiredSize = uiElement.DesiredSize;
          if (desiredSize.Height > height)
            height = desiredSize.Height;
          bool flag = true;
          if (position == TimelineTreeRowPosition.Right)
            this.rightColumnWidth += desiredSize.Width;
          else if (position == TimelineTreeRowPosition.Border)
            flag = false;
          if (flag)
            indent += desiredSize.Width;
        }
      }
      return new Size(indent, height);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
      if (!this.CustomLayout)
        return base.ArrangeOverride(finalSize);
      UIElementCollection internalChildren = this.InternalChildren;
      double indent = this.Indent;
      double height = 0.0;
      double num1 = this.rightColumnWidth;
      double num2 = Math.Max(this.ViewportOffset + this.ViewportWidth - this.rightColumnWidth - this.Indent, 0.0);
      Thickness margin = this.Margin;
      for (int index = 0; index < internalChildren.Count; ++index)
      {
        UIElement uiElement = internalChildren[index];
        if (uiElement.Visibility != Visibility.Collapsed)
        {
          TimelineTreeRowPosition position = TimelineTreeRow.GetPosition((DependencyObject) uiElement);
          double width = uiElement.DesiredSize.Width;
          if (position == TimelineTreeRowPosition.Left)
          {
            if (width < num2)
            {
              num2 -= width;
            }
            else
            {
              width = num2;
              num2 = 0.0;
            }
          }
          Size size = new Size(width, finalSize.Height);
          if (position == TimelineTreeRowPosition.Left)
          {
            uiElement.Arrange(new Rect(new Point(indent, 0.0), size));
            indent += size.Width;
          }
          else if (position == TimelineTreeRowPosition.Right)
          {
            uiElement.Arrange(new Rect(new Point(this.ViewportOffset + this.ViewportWidth - num1, 0.0), size));
            num1 -= size.Width;
          }
          else if (position == TimelineTreeRowPosition.Border)
            uiElement.Arrange(new Rect(new Point(this.ViewportOffset, 0.0), new Size(this.ViewportWidth, finalSize.Height)));
          if (size.Height > height)
            height = size.Height;
        }
      }
      return new Size(this.ViewportWidth + this.ViewportOffset, height);
    }

    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
      base.OnPropertyChanged(e);
      if (e.Property != FrameworkElement.ActualHeightProperty || this.TimelineItem == null)
        return;
      this.TimelineItem.RowHeight = (double) e.NewValue;
    }

    protected override void OnInitialized(EventArgs e)
    {
      base.OnInitialized(e);
      this.AllowDrop = true;
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
      if (!this.IsEditingTitle)
      {
        Keyboard.Focus((IInputElement) null);
        this.mouseDownModifiers = Keyboard.Modifiers;
        this.handledMouseDown = false;
        if (!this.timelineItem.IsSelected)
        {
          if (Keyboard.Modifiers == ModifierKeys.None)
            this.timelineItem.Select();
          else if (Keyboard.Modifiers == ModifierKeys.Control)
            this.timelineItem.ToggleSelect();
          else if (Keyboard.Modifiers == ModifierKeys.Shift)
            this.timelineItem.ExtendSelect();
          this.handledMouseDown = true;
        }
        this.gotMouseDown = true;
        e.Handled = true;
      }
      base.OnMouseLeftButtonDown(e);
    }

    private static void OnMouseUp(object sender, MouseButtonEventArgs e)
    {
      TimelineTreeRow timelineTreeRow = (TimelineTreeRow) sender;
      if (timelineTreeRow.gotMouseDown && !timelineTreeRow.IsEditingTitle)
      {
        timelineTreeRow.gotMouseDown = false;
        if (!timelineTreeRow.handledMouseDown)
        {
          if (timelineTreeRow.mouseDownModifiers == ModifierKeys.Control)
            timelineTreeRow.timelineItem.ToggleSelect();
          else if (timelineTreeRow.mouseDownModifiers == ModifierKeys.Shift)
            timelineTreeRow.timelineItem.ExtendSelect();
          else if (timelineTreeRow.mouseDownModifiers == ModifierKeys.None)
            timelineTreeRow.timelineItem.Select();
        }
        e.Handled = true;
      }
      timelineTreeRow.handledMouseDown = false;
      timelineTreeRow.mouseDownModifiers = ModifierKeys.None;
    }

    private void OnEnsureSelectCommand(object target, ExecutedRoutedEventArgs args)
    {
      this.timelineItem.EnsureSelect();
    }

    private void OnExpandCollapseCommand(object target, ExecutedRoutedEventArgs args)
    {
      this.timelineItem.IsExpanded = !this.timelineItem.IsExpanded;
    }

    protected override void OnContextMenuOpening(ContextMenuEventArgs e)
    {
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.InvokeTimelineItemContextMenu);
      base.OnContextMenuOpening(e);
      this.timelineItem.UpdateContextMenu();
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.InvokeTimelineItemContextMenu);
    }

    private void This_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      TimelineItem timelineItem = this.DataContext as TimelineItem;
      if (timelineItem == null || timelineItem == this.TimelineItem)
        return;
      this.TimelineItem = timelineItem;
    }

    private static void HandleBringIntoView(object sender, RequestBringIntoViewEventArgs e)
    {
      if (e.TargetObject == sender)
        return;
      TimelineTreeRow timelineTreeRow = (TimelineTreeRow) sender;
      InlineStringEditor inlineStringEditor = timelineTreeRow.FindName("ItemTitle") as InlineStringEditor;
      if (inlineStringEditor == null || !inlineStringEditor.IsEditing || e.TargetRect.IsEmpty)
        return;
      timelineTreeRow.BringIntoView(new Rect(e.TargetRect.X, e.TargetRect.Y, inlineStringEditor.ActualWidth, inlineStringEditor.ActualHeight));
      e.Handled = true;
    }

    protected override void OnMouseEnter(MouseEventArgs e)
    {
      base.OnMouseEnter(e);
      PropertyTimelineItem propertyTimelineItem = this.timelineItem as PropertyTimelineItem;
      if (propertyTimelineItem == null)
        return;
      PathEditMode pathEditMode = PathEditMode.None;
      PropertyReference targetProperty = propertyTimelineItem.TargetProperty;
      if (targetProperty[0].Equals((object) PathElement.DataProperty))
        pathEditMode = PathEditMode.ScenePath;
      else if (targetProperty[0].Equals((object) Base2DElement.ClipProperty))
        pathEditMode = PathEditMode.ClippingPath;
      if (pathEditMode == PathEditMode.None)
        return;
      foreach (AdornerSet adornerSet in (IEnumerable<AdornerSet>) propertyTimelineItem.Element.ViewModel.DefaultView.AdornerLayer.Get2DAdornerSets(propertyTimelineItem.Element))
      {
        PathAdornerSet pathAdornerSet = adornerSet as PathAdornerSet;
        if (pathAdornerSet != null && pathAdornerSet.PathEditorTarget.PathEditMode == pathEditMode)
        {
          pathAdornerSet.HighlightProperty(propertyTimelineItem.TargetProperty);
          this.highlightedPathAdornerSet = pathAdornerSet;
        }
      }
    }

    protected override void OnMouseLeave(MouseEventArgs e)
    {
      base.OnMouseLeave(e);
      if (this.highlightedPathAdornerSet == null)
        return;
      this.highlightedPathAdornerSet.ClearHighlight();
      this.highlightedPathAdornerSet = (PathAdornerSet) null;
    }

    protected virtual void OnRenameCommandChanged(ICommand newCommand)
    {
    }

    private static void OnRenameCommandChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
      TimelineTreeRow timelineTreeRow = o as TimelineTreeRow;
      if (timelineTreeRow == null)
        return;
      timelineTreeRow.OnRenameCommandChanged((ICommand) e.NewValue);
    }
  }
}
