// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.SplitterGrip
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Microsoft.VisualStudio.PlatformUI
{
  public class SplitterGrip : Thumb
  {
    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof (Orientation), typeof (SplitterGrip), (PropertyMetadata) new FrameworkPropertyMetadata((object) Orientation.Vertical));
    public static readonly DependencyProperty ResizeBehaviorProperty = DependencyProperty.Register("ResizeBehavior", typeof (GridResizeBehavior), typeof (SplitterGrip), (PropertyMetadata) new FrameworkPropertyMetadata((object) GridResizeBehavior.CurrentAndNext));

    public Orientation Orientation
    {
      get
      {
        return (Orientation) this.GetValue(SplitterGrip.OrientationProperty);
      }
      set
      {
        this.SetValue(SplitterGrip.OrientationProperty, (object) value);
      }
    }

    public GridResizeBehavior ResizeBehavior
    {
      get
      {
        return (GridResizeBehavior) this.GetValue(SplitterGrip.ResizeBehaviorProperty);
      }
      set
      {
        this.SetValue(SplitterGrip.ResizeBehaviorProperty, (object) value);
      }
    }

    static SplitterGrip()
    {
      FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (SplitterGrip), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (SplitterGrip)));
    }

    public SplitterGrip()
    {
      AutomationProperties.SetAutomationId((DependencyObject) this, "SplitterGrip");
    }
  }
}
