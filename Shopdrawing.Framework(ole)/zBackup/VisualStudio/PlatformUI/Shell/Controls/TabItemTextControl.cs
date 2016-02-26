// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.Controls.TabItemTextControl
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System.Windows;
using System.Windows.Controls;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
  public class TabItemTextControl : UserControl
  {
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof (string), typeof (TabItemTextControl), new PropertyMetadata((object) "", new PropertyChangedCallback(TabItemTextControl.OnTextChanged)));
    private TextBlock contentText;

    public string Text
    {
      get
      {
        return (string) this.GetValue(TabItemTextControl.TextProperty);
      }
      set
      {
        this.SetValue(TabItemTextControl.TextProperty, (object) value);
      }
    }

    public string DisplayedText
    {
      get
      {
        return this.contentText.Text;
      }
    }

    public TabItemTextControl()
    {
      this.contentText = new TextBlock();
      this.Content = (object) this.contentText;
    }

    private void UpdateTextContent()
    {
      Size availableSize = new Size(double.PositiveInfinity, this.DesiredSize.Height);
      this.contentText.Text = this.Text;
      this.contentText.Measure(availableSize);
      if (this.contentText.DesiredSize.Width <= this.MaxWidth)
        return;
      this.SplitTextToFit(availableSize);
    }

    private void SplitTextToFit(Size availableSize)
    {
      int rightTrim = this.Text.Length / 2;
      int leftTrim = this.Text.Length / 2;
      int num1 = 1;
      int num2 = 1;
      bool flag1 = false;
      bool flag2 = false;
      bool flag3 = true;
      do
      {
        this.contentText.Text = TabItemTextControl.InsertEllipsis(this.Text, leftTrim, rightTrim);
        this.contentText.Measure(availableSize);
        if (this.contentText.DesiredSize.Width <= this.MaxWidth)
        {
          flag1 = true;
          flag2 = true;
        }
        else if (flag3)
        {
          flag3 = false;
          --leftTrim;
          if (leftTrim < num2)
          {
            flag1 = true;
            leftTrim = num2;
          }
        }
        else
        {
          flag3 = true;
          --rightTrim;
          if (rightTrim < num1)
          {
            flag2 = true;
            rightTrim = num1;
          }
        }
      }
      while (!flag1 || !flag2);
    }

    private static string InsertEllipsis(string tabText, int leftTrim, int rightTrim)
    {
      return tabText.Substring(0, leftTrim).TrimEnd(' ') + "…" + tabText.Substring(tabText.Length - rightTrim).TrimStart(' ');
    }

    private static void OnTextChanged(DependencyObject depObject, DependencyPropertyChangedEventArgs e)
    {
      TabItemTextControl tabItemTextControl = depObject as TabItemTextControl;
      if (tabItemTextControl == null)
        return;
      tabItemTextControl.UpdateTextContent();
    }
  }
}
