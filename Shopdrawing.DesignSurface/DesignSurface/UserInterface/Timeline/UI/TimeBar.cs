// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI.TimeBar
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI
{
  public class TimeBar : Control
  {
    public static readonly DependencyProperty LevelProperty = DependencyProperty.Register("Level", typeof (TimeBarLevel), typeof (TimeBar), (PropertyMetadata) new FrameworkPropertyMetadata((object) TimeBarLevel.Property));
    public static readonly DependencyProperty IsConformingProperty = DependencyProperty.Register("IsConforming", typeof (bool), typeof (TimeBar), (PropertyMetadata) new FrameworkPropertyMetadata((object) true));
    public static readonly DependencyProperty ClipBeginProperty = DependencyProperty.Register("ClipBegin", typeof (double), typeof (TimeBar), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0.0));
    public static readonly DependencyProperty BeginProperty = DependencyProperty.Register("Begin", typeof (double), typeof (TimeBar), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0.0));
    public static readonly DependencyProperty DurationProperty = DependencyProperty.Register("Duration", typeof (double), typeof (TimeBar), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0.0));
    public static readonly DependencyProperty ClipEndProperty = DependencyProperty.Register("ClipEnd", typeof (double), typeof (TimeBar), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0.0));
    public static readonly DependencyProperty BeginEditCommandProperty = DependencyProperty.Register("BeginEditCommand", typeof (ICommand), typeof (TimeBar), (PropertyMetadata) new FrameworkPropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty EndEditCommandProperty = DependencyProperty.Register("EndEditCommand", typeof (ICommand), typeof (TimeBar), (PropertyMetadata) new FrameworkPropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty BeginMinProperty = DependencyProperty.Register("BeginMin", typeof (double), typeof (TimeBar), (PropertyMetadata) new FrameworkPropertyMetadata((object) double.NegativeInfinity));
    public static readonly DependencyProperty BeginMaxProperty = DependencyProperty.Register("BeginMax", typeof (double), typeof (TimeBar), (PropertyMetadata) new FrameworkPropertyMetadata((object) double.PositiveInfinity));
    public static readonly DependencyProperty EndMinProperty = DependencyProperty.Register("EndMin", typeof (double), typeof (TimeBar), (PropertyMetadata) new FrameworkPropertyMetadata((object) double.NegativeInfinity));
    public static readonly DependencyProperty EndMaxProperty = DependencyProperty.Register("EndMax", typeof (double), typeof (TimeBar), (PropertyMetadata) new FrameworkPropertyMetadata((object) double.PositiveInfinity));

    public TimeBarLevel Level
    {
      get
      {
        return (TimeBarLevel) this.GetValue(TimeBar.LevelProperty);
      }
      set
      {
        this.SetValue(TimeBar.LevelProperty, (object) value);
      }
    }

    public bool IsConforming
    {
      get
      {
        return (bool) this.GetValue(TimeBar.IsConformingProperty);
      }
      set
      {
        this.SetValue(TimeBar.IsConformingProperty, (object) (bool) (value ? true : false));
      }
    }

    public double ClipBegin
    {
      get
      {
        return (double) this.GetValue(TimeBar.ClipBeginProperty);
      }
      set
      {
        this.SetValue(TimeBar.ClipBeginProperty, (object) value);
      }
    }

    public double Begin
    {
      get
      {
        return (double) this.GetValue(TimeBar.BeginProperty);
      }
      set
      {
        this.SetValue(TimeBar.BeginProperty, (object) value);
      }
    }

    public double Duration
    {
      get
      {
        return (double) this.GetValue(TimeBar.DurationProperty);
      }
      set
      {
        this.SetValue(TimeBar.DurationProperty, (object) value);
      }
    }

    public double ClipEnd
    {
      get
      {
        return (double) this.GetValue(TimeBar.ClipEndProperty);
      }
      set
      {
        this.SetValue(TimeBar.ClipEndProperty, (object) value);
      }
    }

    public ICommand BeginEditCommand
    {
      get
      {
        return (ICommand) this.GetValue(TimeBar.BeginEditCommandProperty);
      }
      set
      {
        this.SetValue(TimeBar.BeginEditCommandProperty, (object) value);
      }
    }

    public ICommand EndEditCommand
    {
      get
      {
        return (ICommand) this.GetValue(TimeBar.EndEditCommandProperty);
      }
      set
      {
        this.SetValue(TimeBar.EndEditCommandProperty, (object) value);
      }
    }

    public double BeginMin
    {
      get
      {
        return (double) this.GetValue(TimeBar.BeginMinProperty);
      }
      set
      {
        this.SetValue(TimeBar.BeginMinProperty, (object) value);
      }
    }

    public double BeginMax
    {
      get
      {
        return (double) this.GetValue(TimeBar.BeginMaxProperty);
      }
      set
      {
        this.SetValue(TimeBar.BeginMaxProperty, (object) value);
      }
    }

    public double EndMin
    {
      get
      {
        return (double) this.GetValue(TimeBar.EndMinProperty);
      }
      set
      {
        this.SetValue(TimeBar.EndMinProperty, (object) value);
      }
    }

    public double EndMax
    {
      get
      {
        return (double) this.GetValue(TimeBar.EndMaxProperty);
      }
      set
      {
        this.SetValue(TimeBar.EndMaxProperty, (object) value);
      }
    }
  }
}
