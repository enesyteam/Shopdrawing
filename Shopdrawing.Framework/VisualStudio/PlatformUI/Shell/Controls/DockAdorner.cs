// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.Controls.DockAdorner
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI.Shell;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
  public class DockAdorner : ContentControl
  {
    public static readonly DependencyProperty AdornedElementProperty = DependencyProperty.Register("AdornedElement", typeof (FrameworkElement), typeof (DockAdorner), new PropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty DockDirectionProperty = DependencyProperty.Register("DockDirection", typeof (DockDirection), typeof (DockAdorner), new PropertyMetadata((object) DockDirection.Fill));
    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof (System.Windows.Controls.Orientation?), typeof (DockAdorner), new PropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty AreOuterTargetsEnabledProperty = DependencyProperty.Register("AreOuterTargetsEnabled", typeof (bool), typeof (DockAdorner), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
    public static readonly DependencyProperty AreInnerTargetsEnabledProperty = DependencyProperty.Register("AreInnerTargetsEnabled", typeof (bool), typeof (DockAdorner), (PropertyMetadata) new FrameworkPropertyMetadata((object) true));
    public static readonly DependencyProperty IsInnerCenterTargetEnabledProperty = DependencyProperty.Register("IsInnerCenterTargetEnabled", typeof (bool), typeof (DockAdorner), (PropertyMetadata) new FrameworkPropertyMetadata((object) true));
    public static readonly DependencyProperty AreInnerSideTargetsEnabledProperty = DependencyProperty.Register("AreInnerSideTargetsEnabled", typeof (bool), typeof (DockAdorner), (PropertyMetadata) new FrameworkPropertyMetadata((object) true));

    public IntPtr OwnerHwnd { get; set; }

    public FrameworkElement AdornedElement
    {
      get
      {
        return (FrameworkElement) this.GetValue(DockAdorner.AdornedElementProperty);
      }
      set
      {
        this.SetValue(DockAdorner.AdornedElementProperty, (object) value);
      }
    }

    public DockDirection DockDirection
    {
      get
      {
        return (DockDirection) this.GetValue(DockAdorner.DockDirectionProperty);
      }
      set
      {
        this.SetValue(DockAdorner.DockDirectionProperty, (object) value);
      }
    }

    public System.Windows.Controls.Orientation? Orientation
    {
      get
      {
        return (System.Windows.Controls.Orientation?) this.GetValue(DockAdorner.OrientationProperty);
      }
      set
      {
        this.SetValue(DockAdorner.OrientationProperty, (object) value);
      }
    }

    public bool AreOuterTargetsEnabled
    {
        get
        {
            return (bool)base.GetValue(DockAdorner.AreOuterTargetsEnabledProperty);
        }
        set
        {
            base.SetValue(DockAdorner.AreOuterTargetsEnabledProperty, value);
        }
    }

    public bool AreInnerTargetsEnabled
    {
        get
        {
            return (bool)base.GetValue(DockAdorner.AreInnerTargetsEnabledProperty);
        }
        set
        {
            base.SetValue(DockAdorner.AreInnerTargetsEnabledProperty, value);
        }
    }

    public bool IsInnerCenterTargetEnabled
    {
        get
        {
            return (bool)base.GetValue(DockAdorner.IsInnerCenterTargetEnabledProperty);
        }
        set
        {
            base.SetValue(DockAdorner.IsInnerCenterTargetEnabledProperty, value);
        }
    }

    public bool AreInnerSideTargetsEnabled
    {
        get
        {
            return (bool)base.GetValue(DockAdorner.AreInnerSideTargetsEnabledProperty);
        }
        set
        {
            base.SetValue(DockAdorner.AreInnerSideTargetsEnabledProperty, value);
        }
    }

    public void UpdateContent()
    {
      this.UpdateContentCore();
      this.InvalidateArrange();
      this.UpdateLayout();
    }

    protected virtual void UpdateContentCore()
    {
    }
  }
}
