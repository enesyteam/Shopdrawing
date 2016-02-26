// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.SplitterPanel
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Microsoft.VisualStudio.PlatformUI
{
  public class SplitterPanel : Panel
  {
    public static readonly DependencyProperty SplitterLengthProperty = DependencyProperty.RegisterAttached("SplitterLength", typeof (SplitterLength), typeof (SplitterPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) new SplitterLength(100.0), FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange));
    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof (Orientation), typeof (SplitterPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) Orientation.Vertical, FrameworkPropertyMetadataOptions.AffectsMeasure));
    public static readonly DependencyProperty MinimumLengthProperty = DependencyProperty.RegisterAttached("MinimumLength", typeof (double), typeof (SplitterPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0.0));
    private static readonly DependencyProperty IsMinimumReachedProperty = DependencyProperty.RegisterAttached("IsMinimumReached", typeof (bool), typeof (SplitterPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
    private static readonly DependencyPropertyKey ActualSplitterLengthPropertyKey = DependencyProperty.RegisterAttachedReadOnly("ActualSplitterLength", typeof (double), typeof (SplitterPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0.0));
    private static readonly DependencyPropertyKey IndexPropertyKey = DependencyProperty.RegisterAttachedReadOnly("Index", typeof (int), typeof (SplitterPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) -1));
    private static readonly DependencyPropertyKey IsFirstPropertyKey = DependencyProperty.RegisterAttachedReadOnly("IsFirst", typeof (bool), typeof (SplitterPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
    private static readonly DependencyPropertyKey IsLastPropertyKey = DependencyProperty.RegisterAttachedReadOnly("IsLast", typeof (bool), typeof (SplitterPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
    public static readonly DependencyProperty ActualSplitterLengthProperty = SplitterPanel.ActualSplitterLengthPropertyKey.DependencyProperty;
    public static readonly DependencyProperty IndexProperty = SplitterPanel.IndexPropertyKey.DependencyProperty;
    public static readonly DependencyProperty IsFirstProperty = SplitterPanel.IsFirstPropertyKey.DependencyProperty;
    public static readonly DependencyProperty IsLastProperty = SplitterPanel.IsLastPropertyKey.DependencyProperty;

    public Orientation Orientation
    {
      get
      {
        return (Orientation) this.GetValue(SplitterPanel.OrientationProperty);
      }
      set
      {
        this.SetValue(SplitterPanel.OrientationProperty, (object) value);
      }
    }

    static SplitterPanel()
    {
      FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (SplitterPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (SplitterPanel)));
    }

    public SplitterPanel()
    {
      this.AddHandler(Thumb.DragDeltaEvent, (Delegate) new DragDeltaEventHandler(this.OnSplitterResized));
      AutomationProperties.SetAutomationId((DependencyObject) this, "SplitterPanel");
    }

    public static double GetActualSplitterLength(UIElement element)
    {
      return (double) element.GetValue(SplitterPanel.ActualSplitterLengthProperty);
    }

    protected static void SetActualSplitterLength(UIElement element, double value)
    {
      element.SetValue(SplitterPanel.ActualSplitterLengthPropertyKey, (object) value);
    }

    public static int GetIndex(UIElement element)
    {
      return (int) element.GetValue(SplitterPanel.IndexProperty);
    }

    public static bool GetIsFirst(UIElement element)
    {
      return (bool) element.GetValue(SplitterPanel.IsFirstProperty);
    }

    protected static void SetIsFirst(UIElement element, bool value)
    {
        element.SetValue(SplitterPanel.IsFirstPropertyKey, value);
    }

    public static bool GetIsLast(UIElement element)
    {
      return (bool) element.GetValue(SplitterPanel.IsLastProperty);
    }

    protected static void SetIsLast(UIElement element, bool value)
    {
        element.SetValue(SplitterPanel.IsLastPropertyKey, value);
    }

    protected static void SetIndex(UIElement element, int value)
    {
      element.SetValue(SplitterPanel.IndexPropertyKey, (object) value);
    }

    public static SplitterLength GetSplitterLength(UIElement element)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      return (SplitterLength) element.GetValue(SplitterPanel.SplitterLengthProperty);
    }

    public static void SetSplitterLength(UIElement element, SplitterLength value)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      element.SetValue(SplitterPanel.SplitterLengthProperty, (object) value);
    }

    public static double GetMinimumLength(UIElement element)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      return (double) element.GetValue(SplitterPanel.MinimumLengthProperty);
    }

    public static void SetMinimumLength(UIElement element, double value)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      element.SetValue(SplitterPanel.MinimumLengthProperty, (object) value);
    }

    private static bool GetIsMinimumReached(UIElement element)
    {
      return (bool) element.GetValue(SplitterPanel.IsMinimumReachedProperty);
    }

    private static void SetIsMinimumReached(UIElement element, bool value)
    {
        element.SetValue(SplitterPanel.IsMinimumReachedProperty, value);
    }

    private void UpdateIndices()
    {
      int count = this.InternalChildren.Count;
      int num = this.InternalChildren.Count - 1;
      for (int index = 0; index < count; ++index)
      {
        SplitterPanel.SetIndex(this.InternalChildren[index], index);
        SplitterPanel.SetIsFirst(this.InternalChildren[index], index == 0);
        SplitterPanel.SetIsLast(this.InternalChildren[index], index == num);
      }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      this.UpdateIndices();
      Rect[] elementBounds;
      return SplitterPanel.Measure(availableSize, this.Orientation, (IEnumerable) this.InternalChildren, true, out elementBounds, this);
    }

    public static Size Measure(Size availableSize, Orientation orientation, IEnumerable uiElements, bool remeasureElements, out Rect[] elementBounds, SplitterPanel splitterPanel)
    {
      double num1 = 0.0;
      double num2 = 0.0;
      double num3 = 0.0;
      double num4 = 0.0;
      double val2 = 0.0;
      double val1_1 = 0.0;
      double val1_2 = 0.0;
      List<UIElement> list1 = new List<UIElement>();
      foreach (UIElement uiElement in uiElements)
        list1.Add(uiElement);
      foreach (UIElement element in list1)
      {
        if (remeasureElements)
          element.Measure(availableSize);
        if (orientation == Orientation.Horizontal)
        {
          val1_2 += element.DesiredSize.Height;
          val1_1 = Math.Max(val1_1, element.DesiredSize.Width);
        }
        else
        {
          val1_1 += element.DesiredSize.Width;
          val1_2 = Math.Max(val1_2, element.DesiredSize.Height);
        }
        SplitterLength splitterLength = SplitterPanel.GetSplitterLength(element);
        double minimumLength = SplitterPanel.GetMinimumLength(element);
        if (splitterLength.IsStretch)
        {
          num1 += splitterLength.Value;
          num4 += minimumLength;
        }
        else if (splitterLength.IsFixed)
        {
          val2 += splitterLength.Value;
        }
        else
        {
          num2 += splitterLength.Value;
          num3 += minimumLength;
        }
        SplitterPanel.SetIsMinimumReached(element, false);
      }
      double num5 = num4 + num3 + val2;
      double width = ExtensionMethods.IsNonreal(availableSize.Width) ? val1_1 : availableSize.Width;
      double height = ExtensionMethods.IsNonreal(availableSize.Height) ? val1_2 : availableSize.Height;
      double val1_3 = orientation == Orientation.Horizontal ? width : height;
      double num6 = val2 == 0.0 ? 0.0 : Math.Min(val1_3, val2);
      double num7 = num2 == 0.0 ? 0.0 : Math.Max(0.0, val1_3 - num1 - val2);
      double num8 = num7 == 0.0 ? val1_3 - num6 : val1_3 - num7 - num6;
      if (num5 <= val1_3)
      {
        if (num7 < num3)
        {
          num7 = num3;
          num8 = val1_3 - num7 - num6;
        }
        foreach (UIElement element in list1)
        {
          SplitterLength splitterLength = SplitterPanel.GetSplitterLength(element);
          double minimumLength = SplitterPanel.GetMinimumLength(element);
          if (splitterLength.IsFill)
          {
            if ((num2 == 0.0 ? 0.0 : splitterLength.Value / num2 * num7) < minimumLength)
            {
              SplitterPanel.SetIsMinimumReached(element, true);
              num7 -= minimumLength;
              num2 -= splitterLength.Value;
            }
          }
          else if (splitterLength.IsStretch && (num1 == 0.0 ? 0.0 : splitterLength.Value / num1 * num8) < minimumLength)
          {
            SplitterPanel.SetIsMinimumReached(element, true);
            num8 -= minimumLength;
            num1 -= splitterLength.Value;
          }
        }
      }
      Size availableSize1 = new Size(width, height);
      List<Rect> list2 = new List<Rect>();
      Rect rect = new Rect(0.0, 0.0, width, height);
      bool flag = false;
      if (splitterPanel != null && splitterPanel.SnapsToDevicePixels)
      {
        PresentationSource presentationSource = PresentationSource.FromVisual((Visual) splitterPanel);
        if (presentationSource != null)
        {
          Visual rootVisual = presentationSource.RootVisual;
          if (rootVisual != null)
          {
            Transform transform = splitterPanel.TransformToAncestor(rootVisual) as Transform;
            if (transform != null && transform.Value.HasInverse)
              flag = true;
          }
        }
      }
      double num9 = num6;
      foreach (UIElement element in list1)
      {
        SplitterLength splitterLength = SplitterPanel.GetSplitterLength(element);
        double num10;
        if (!SplitterPanel.GetIsMinimumReached(element))
        {
          if (splitterLength.IsFill)
            num10 = num2 == 0.0 ? 0.0 : splitterLength.Value / num2 * num7;
          else if (splitterLength.IsFixed)
          {
            num10 = splitterLength.Value;
            if (num10 > num9)
              num10 = num9;
            num9 -= num10;
          }
          else
            num10 = num1 == 0.0 ? 0.0 : splitterLength.Value / num1 * num8;
        }
        else
          num10 = SplitterPanel.GetMinimumLength(element);
        if (flag)
        {
          Point point1 = splitterPanel.Orientation != Orientation.Horizontal ? new Point(0.0, num10) : new Point(num10, 0.0);
          Point point2 = splitterPanel.PointFromScreen(splitterPanel.PointToScreen(point1));
          num10 = splitterPanel.Orientation != Orientation.Horizontal ? point2.Y : point2.X;
        }
        if (num10 < 0.0)
          num10 = 0.0;
        if (remeasureElements)
          SplitterPanel.SetActualSplitterLength(element, num10);
        if (orientation == Orientation.Horizontal)
        {
          availableSize1.Width = num10;
          list2.Add(new Rect(rect.Left, rect.Top, num10, rect.Height));
          rect.X += num10;
          if (remeasureElements)
            element.Measure(availableSize1);
        }
        else
        {
          availableSize1.Height = num10;
          list2.Add(new Rect(rect.Left, rect.Top, rect.Width, num10));
          rect.Y += num10;
          if (remeasureElements)
            element.Measure(availableSize1);
        }
      }
      elementBounds = list2.ToArray();
      return new Size(width, height);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
      Rect finalRect = new Rect(0.0, 0.0, finalSize.Width, finalSize.Height);
      foreach (UIElement element in this.InternalChildren)
      {
        double actualSplitterLength = SplitterPanel.GetActualSplitterLength(element);
        if (this.Orientation == Orientation.Horizontal)
        {
          finalRect.Width = actualSplitterLength;
          element.Arrange(finalRect);
          finalRect.X += actualSplitterLength;
        }
        else
        {
          finalRect.Height = actualSplitterLength;
          element.Arrange(finalRect);
          finalRect.Y += actualSplitterLength;
        }
      }
      return finalSize;
    }

    private void OnSplitterResized(object sender, DragDeltaEventArgs args)
    {
      SplitterGrip splitterGrip = args.OriginalSource as SplitterGrip;
      if (splitterGrip == null)
        return;
      PerformanceUtility.MeasurePerformanceUntilRender(PerformanceEvent.ResizePalette);
      args.Handled = true;
      for (int index = 0; index < this.InternalChildren.Count; ++index)
      {
        if (this.InternalChildren[index].IsAncestorOf((DependencyObject) splitterGrip))
        {
          double pixelAmount = this.Orientation == Orientation.Horizontal ? args.HorizontalChange : args.VerticalChange;
          switch (splitterGrip.ResizeBehavior)
          {
            case GridResizeBehavior.CurrentAndNext:
              this.ResizeChildren(index, index + 1, pixelAmount);
              continue;
            case GridResizeBehavior.PreviousAndCurrent:
              this.ResizeChildren(index - 1, index, pixelAmount);
              continue;
            case GridResizeBehavior.PreviousAndNext:
              this.ResizeChildren(index - 1, index + 1, pixelAmount);
              continue;
            default:
              throw new InvalidOperationException("BasedOnAlignment is not a valid resize behavior");
          }
        }
      }
    }

    internal void ResizeChildren(int index1, int index2, double pixelAmount)
    {
      if (!this.IsArrangeValid)
        return;
      UIElement element1;
      SplitterLength splitterLength1;
      do
      {
        element1 = this.InternalChildren[index1];
        splitterLength1 = SplitterPanel.GetSplitterLength(element1);
        if (splitterLength1.IsFixed)
          --index1;
      }
      while (splitterLength1.IsFixed && index1 >= 0);
      UIElement element2;
      SplitterLength splitterLength2;
      do
      {
        element2 = this.InternalChildren[index2];
        splitterLength2 = SplitterPanel.GetSplitterLength(element2);
        if (splitterLength2.IsFixed)
          ++index2;
      }
      while (splitterLength2.IsFixed && index2 < this.InternalChildren.Count);
      if (index1 < 0 || index2 >= this.InternalChildren.Count)
        return;
      double actualSplitterLength1 = SplitterPanel.GetActualSplitterLength(element1);
      double actualSplitterLength2 = SplitterPanel.GetActualSplitterLength(element2);
      double num1 = Math.Max(0.0, Math.Min(actualSplitterLength1 + actualSplitterLength2, actualSplitterLength1 + pixelAmount));
      double num2 = Math.Max(0.0, Math.Min(actualSplitterLength1 + actualSplitterLength2, actualSplitterLength2 - pixelAmount));
      double minimumLength1 = SplitterPanel.GetMinimumLength(element1);
      double minimumLength2 = SplitterPanel.GetMinimumLength(element2);
      if (minimumLength1 + minimumLength2 > num1 + num2)
        return;
      if (num1 < minimumLength1)
      {
        num2 -= minimumLength1 - num1;
        num1 = minimumLength1;
      }
      if (num2 < minimumLength2)
      {
        num1 -= minimumLength2 - num2;
        num2 = minimumLength2;
      }
      if (splitterLength1.IsFill && splitterLength2.IsFill || splitterLength1.IsStretch && splitterLength2.IsStretch)
      {
        SplitterPanel.SetSplitterLength(element1, new SplitterLength(num1 / (num1 + num2) * (splitterLength1.Value + splitterLength2.Value), splitterLength1.SplitterUnitType));
        SplitterPanel.SetSplitterLength(element2, new SplitterLength(num2 / (num1 + num2) * (splitterLength1.Value + splitterLength2.Value), splitterLength1.SplitterUnitType));
      }
      else if (splitterLength1.IsFill)
        SplitterPanel.SetSplitterLength(element2, new SplitterLength(num2, SplitterUnitType.Stretch));
      else
        SplitterPanel.SetSplitterLength(element1, new SplitterLength(num1, SplitterUnitType.Stretch));
      this.InvalidateMeasure();
    }
  }
}
