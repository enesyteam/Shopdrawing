// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Assets.CamelCaseWrappingTextBlock
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Expression.DesignSurface.Tools.Assets
{
  public class CamelCaseWrappingTextBlock : TextBlock
  {
    public static readonly DependencyProperty DesiredTextProperty = DependencyProperty.Register("DesiredText", typeof (string), typeof (CamelCaseWrappingTextBlock), (PropertyMetadata) new FrameworkPropertyMetadata((PropertyChangedCallback) null));

    public string DesiredText
    {
      get
      {
        return (string) this.GetValue(CamelCaseWrappingTextBlock.DesiredTextProperty);
      }
      set
      {
        this.SetValue(CamelCaseWrappingTextBlock.DesiredTextProperty, (object) value);
      }
    }

    private bool NeedWrapping
    {
      get
      {
        this.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        return this.DesiredSize.Width - this.Margin.Left - this.Margin.Right > this.ActualWidth;
      }
    }

    protected override void OnInitialized(EventArgs e)
    {
      base.OnInitialized(e);
      this.LayoutUpdated += new EventHandler(this.CamelCaseWrappingTextBlock_LayoutUpdated);
    }

    private void CamelCaseWrappingTextBlock_LayoutUpdated(object sender, EventArgs e)
    {
      if (this.IsMeasureValid && this.ActualWidth > 0.0)
      {
        if (this.NeedWrapping)
          this.BreakCamelCaseText();
        this.LayoutUpdated -= new EventHandler(this.CamelCaseWrappingTextBlock_LayoutUpdated);
      }
      else
      {
        if (string.IsNullOrEmpty(this.DesiredText))
          return;
        this.Text = this.DesiredText;
      }
    }

    private void BreakCamelCaseText()
    {
      string[] strArray = Regex.Split(this.DesiredText, "(?=[A-Z])");
      if (strArray.Length >= 2)
      {
        string str = strArray[0];
        for (int index = 1; index < strArray.Length; ++index)
        {
          this.Text = str + strArray[index];
          if (this.NeedWrapping)
          {
            this.Text = str + " " + this.DesiredText.Substring(str.Length);
            return;
          }
          str += strArray[index];
        }
      }
      this.Text = this.DesiredText;
    }
  }
}
