// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.Controls.GlyphButton
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
  public class GlyphButton : Button
  {
    public static readonly DependencyProperty PressedBackgroundProperty = DependencyProperty.Register("PressedBackground", typeof (Brush), typeof (GlyphButton));
    public static readonly DependencyProperty PressedBorderBrushProperty = DependencyProperty.Register("PressedBorderBrush", typeof (Brush), typeof (GlyphButton));
    public static readonly DependencyProperty PressedBorderThicknessProperty = DependencyProperty.Register("PressedBorderThickness", typeof (Thickness), typeof (GlyphButton));
    public static readonly DependencyProperty HoverBackgroundProperty = DependencyProperty.Register("HoverBackground", typeof (Brush), typeof (GlyphButton));
    public static readonly DependencyProperty HoverBorderBrushProperty = DependencyProperty.Register("HoverBorderBrush", typeof (Brush), typeof (GlyphButton));
    public static readonly DependencyProperty HoverBorderThicknessProperty = DependencyProperty.Register("HoverBorderThickness", typeof (Thickness), typeof (GlyphButton));
    public static readonly DependencyProperty GlyphForegroundProperty = DependencyProperty.Register("GlyphForeground", typeof (Brush), typeof (GlyphButton));
    public static readonly DependencyProperty HoverForegroundProperty = DependencyProperty.Register("HoverForeground", typeof (Brush), typeof (GlyphButton));
    public static readonly DependencyProperty PressedForegroundProperty = DependencyProperty.Register("PressedForeground", typeof (Brush), typeof (GlyphButton));

    public Brush PressedBackground
    {
      get
      {
        return (Brush) this.GetValue(GlyphButton.PressedBackgroundProperty);
      }
      set
      {
        this.SetValue(GlyphButton.PressedBackgroundProperty, (object) value);
      }
    }

    public Brush PressedBorderBrush
    {
      get
      {
        return (Brush) this.GetValue(GlyphButton.PressedBorderBrushProperty);
      }
      set
      {
        this.SetValue(GlyphButton.PressedBorderBrushProperty, (object) value);
      }
    }

    public Thickness PressedBorderThickness
    {
      get
      {
        return (Thickness) this.GetValue(GlyphButton.PressedBorderThicknessProperty);
      }
      set
      {
        this.SetValue(GlyphButton.PressedBorderThicknessProperty, (object) value);
      }
    }

    public Brush HoverBackground
    {
      get
      {
        return (Brush) this.GetValue(GlyphButton.HoverBackgroundProperty);
      }
      set
      {
        this.SetValue(GlyphButton.HoverBackgroundProperty, (object) value);
      }
    }

    public Brush HoverBorderBrush
    {
      get
      {
        return (Brush) this.GetValue(GlyphButton.HoverBorderBrushProperty);
      }
      set
      {
        this.SetValue(GlyphButton.HoverBorderBrushProperty, (object) value);
      }
    }

    public Thickness HoverBorderThickness
    {
      get
      {
        return (Thickness) this.GetValue(GlyphButton.HoverBorderThicknessProperty);
      }
      set
      {
        this.SetValue(GlyphButton.HoverBorderThicknessProperty, (object) value);
      }
    }

    public Brush GlyphForeground
    {
      get
      {
        return (Brush) this.GetValue(GlyphButton.GlyphForegroundProperty);
      }
      set
      {
        this.SetValue(GlyphButton.GlyphForegroundProperty, (object) value);
      }
    }

    public Brush HoverForeground
    {
      get
      {
        return (Brush) this.GetValue(GlyphButton.HoverForegroundProperty);
      }
      set
      {
        this.SetValue(GlyphButton.HoverForegroundProperty, (object) value);
      }
    }

    public Brush PressedForeground
    {
      get
      {
        return (Brush) this.GetValue(GlyphButton.PressedForegroundProperty);
      }
      set
      {
        this.SetValue(GlyphButton.PressedForegroundProperty, (object) value);
      }
    }

    static GlyphButton()
    {
      FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (GlyphButton), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (GlyphButton)));
    }
  }
}
