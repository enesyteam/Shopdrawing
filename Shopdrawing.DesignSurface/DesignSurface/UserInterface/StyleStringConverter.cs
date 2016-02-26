// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.StyleStringConverter
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  public class StyleStringConverter : DependencyObject, IMultiValueConverter
  {
    public static readonly DependencyProperty ResourcesProperty = DependencyProperty.Register("Resources", typeof (ResourceDictionary), typeof (StyleStringConverter));
    private Style currentStyle;

    public ResourceDictionary Resources
    {
      get
      {
        return (ResourceDictionary) this.GetValue(StyleStringConverter.ResourcesProperty);
      }
      set
      {
        this.SetValue(StyleStringConverter.ResourcesProperty, (object) value);
      }
    }

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if (values != null && values.Length > 0 && values[0] is string)
        return (object) this.InternalConvert((string) values[0], values, culture);
      return (object) null;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    private TextBlock InternalConvert(string styleFormat, object[] values, CultureInfo culture)
    {
      string pattern = "(\\{{1}\\p{L}\\w+\\}{1})";
      TextBlock textBlock = new TextBlock();
      this.currentStyle = (Style) null;
      foreach (string str in Regex.Split(styleFormat, pattern))
      {
        if (Regex.IsMatch(str, pattern))
          this.ProcessStyleInline(textBlock.Inlines, str.Substring(1, str.Length - 2));
        else if (!string.IsNullOrEmpty(str))
          this.AddFormatInline(textBlock.Inlines, str, values, culture);
      }
      return textBlock;
    }

    private void ProcessStyleInline(InlineCollection inlines, string resourceKey)
    {
      object obj = this.Resources[(object) resourceKey];
      if (obj is Style)
      {
        Type targetType = ((Style) obj).TargetType;
        if (typeof (Run).IsAssignableFrom(targetType) || typeof (TextBlock).IsAssignableFrom(targetType) || typeof (Span).IsAssignableFrom(targetType))
          this.currentStyle = (Style) obj;
        else if (typeof (LineBreak).IsAssignableFrom(targetType))
        {
          LineBreak lineBreak = new LineBreak();
          lineBreak.Style = (Style) obj;
          inlines.Add((Inline) lineBreak);
        }
        else
        {
          if (!typeof (FrameworkElement).IsAssignableFrom(targetType))
            return;
          FrameworkElement frameworkElement = (FrameworkElement) Activator.CreateInstance(targetType);
          frameworkElement.Style = (Style) obj;
          inlines.Add((Inline) new InlineUIContainer((UIElement) frameworkElement));
        }
      }
      else
      {
        if (!(obj is Inline))
          return;
        inlines.Add((Inline) obj);
      }
    }

    private void AddFormatInline(InlineCollection inlines, string format, object[] values, CultureInfo culture)
    {
      string text = format;
      try
      {
        if (format.Contains("{}"))
          format = format.Replace("{}", "");
        text = string.Format((IFormatProvider) culture, format, values);
      }
      catch (FormatException ex)
      {
      }
      if (this.currentStyle == null)
        inlines.Add((Inline) new Run(text));
      else if (typeof (Run).IsAssignableFrom(this.currentStyle.TargetType))
      {
        Run run = Activator.CreateInstance(this.currentStyle.TargetType) as Run;
        run.Style = this.currentStyle;
        run.Text = text;
        inlines.Add((Inline) run);
      }
      else if (typeof (TextBlock).IsAssignableFrom(this.currentStyle.TargetType))
      {
        TextBlock textBlock = Activator.CreateInstance(this.currentStyle.TargetType) as TextBlock;
        textBlock.Style = this.currentStyle;
        textBlock.Text = text;
        inlines.Add((Inline) new InlineUIContainer((UIElement) textBlock));
      }
      else if (typeof (Span).IsAssignableFrom(this.currentStyle.TargetType))
      {
        Span span = Activator.CreateInstance(this.currentStyle.TargetType) as Span;
        span.Style = this.currentStyle;
        span.Inlines.Add(text);
        inlines.Add((Inline) span);
      }
      else
        inlines.Add((Inline) new Run(text));
    }
  }
}
