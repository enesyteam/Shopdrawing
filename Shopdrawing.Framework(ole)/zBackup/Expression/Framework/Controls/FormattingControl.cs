// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Controls.FormattingControl
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Microsoft.Expression.Framework.Controls
{
  [Localizability(LocalizationCategory.Text)]
  [ContentProperty("Items")]
  public class FormattingControl : Control
  {
    public static readonly DependencyProperty TextStyleProperty = DependencyProperty.Register("TextStyle", typeof (Style), typeof (FormattingControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.AffectsMeasure));
    public static readonly DependencyProperty FormatStringProperty = DependencyProperty.Register("FormatString", typeof (string), typeof (FormattingControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.AffectsMeasure));
    private Collection<UIElement> items;

    public Collection<UIElement> Items
    {
      get
      {
        if (this.items == null)
        {
          this.items = new Collection<UIElement>();
          this.UpdatePanel();
        }
        return this.items;
      }
    }

    public Style TextStyle
    {
      get
      {
        return (Style) this.GetValue(FormattingControl.TextStyleProperty);
      }
      set
      {
        this.SetValue(FormattingControl.TextStyleProperty, (object) value);
      }
    }

    public string FormatString
    {
      get
      {
        return (string) this.GetValue(FormattingControl.FormatStringProperty);
      }
      set
      {
        this.SetValue(FormattingControl.FormatStringProperty, (object) value);
      }
    }

    public override void OnApplyTemplate()
    {
      this.UpdatePanel();
      base.OnApplyTemplate();
    }

    private void UpdatePanel()
    {
      Panel panel = this.GetTemplateChild("PART_Panel") as Panel;
      if (panel == null || this.Items.Count <= 0)
        return;
      List<UIElement> formattedList = new List<UIElement>();
      FormattingControl.Format(this.FormatString, this.Items, this.TextStyle, formattedList);
      if (formattedList.Count <= 0)
        return;
      panel.Children.Clear();
      foreach (UIElement element in formattedList)
        panel.Children.Add(element);
    }

    internal static void Format(string formatString, Collection<UIElement> items, Style textStyle, List<UIElement> formattedList)
    {
      StringBuilder text = new StringBuilder();
      for (int index1 = 0; index1 < formatString.Length; ++index1)
      {
        if ((int) formatString[index1] != 123)
        {
          text.Append(formatString[index1]);
        }
        else
        {
          ++index1;
          if (index1 < formatString.Length && (int) formatString[index1] == 123)
            text.Append('{');
          else if (index1 >= formatString.Length || (int) formatString[index1] != 125)
          {
            FormattingControl.Emit(text, textStyle, formattedList);
            text = new StringBuilder();
            StringBuilder stringBuilder = new StringBuilder();
            for (; index1 < formatString.Length && (int) formatString[index1] != 125; ++index1)
              stringBuilder.Append(formatString[index1]);
            int index2 = int.Parse(stringBuilder.ToString(), (IFormatProvider) CultureInfo.InvariantCulture);
            formattedList.Add(items[index2]);
          }
        }
      }
      FormattingControl.Emit(text, textStyle, formattedList);
    }

    private static void Emit(StringBuilder text, Style textStyle, List<UIElement> formattedList)
    {
      if (text.Length == 0)
        return;
      TextBlock textBlock = new TextBlock();
      textBlock.Text = text.ToString();
      textBlock.Style = textStyle;
      textBlock.VerticalAlignment = VerticalAlignment.Center;
      formattedList.Add((UIElement) textBlock);
    }
  }
}
