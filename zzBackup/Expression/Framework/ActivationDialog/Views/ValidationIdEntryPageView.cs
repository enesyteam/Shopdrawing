// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.ActivationDialog.Views.ValidationIdEntryPageView
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;

namespace Microsoft.Expression.Framework.ActivationDialog.Views
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  internal class ValidationIdEntryPageView : UserControl, IComponentConnector, IStyleConnector
  {
    internal ItemsControl itemsControl;
    private bool _contentLoaded;

    public ValidationIdEntryPageView()
    {
      this.InitializeComponent();
      this.Loaded += new RoutedEventHandler(this.ValidationIdEntryPageView_Loaded);
    }

    private void ValidationIdEntryPageView_Loaded(object sender, RoutedEventArgs e)
    {
      this.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
    }

    private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
      if (this.IsValidText(e.Text))
        return;
      e.Handled = true;
    }

    private bool IsValidText(string text)
    {
      return Regex.IsMatch(text, "^[0-9a-fA-F]*$", RegexOptions.IgnoreCase | RegexOptions.Singleline);
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      TextBox textBox = (TextBox) sender;
      this.ValidateText(textBox);
      bool flag = textBox.Text.Length >= 6;
      if (textBox.Text.Length > 6)
        textBox.Text = textBox.Text.Substring(0, 6);
      if (!flag)
        return;
      this.MoveFocusToNextTextbox(textBox);
    }

    private void ValidateText(TextBox textBox)
    {
      StringBuilder stringBuilder = new StringBuilder(textBox.Text.Length);
      for (int index = 0; index < textBox.Text.Length; ++index)
      {
        if (this.IsValidText(textBox.Text[index].ToString()))
          stringBuilder.Append(textBox.Text[index]);
      }
      string str = stringBuilder.ToString();
      if (!(textBox.Text != str))
        return;
      textBox.Text = str;
    }

    private void MoveFocusToNextTextbox(TextBox currentTextBox)
    {
      bool flag = false;
      IList list = this.itemsControl.ItemsSource as IList;
      if (list != null)
        flag = list.IndexOf(currentTextBox.DataContext) == list.Count - 1;
      if (flag)
        return;
      currentTextBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
    }

    private void TextBox_Pasting(object sender, DataObjectPastingEventArgs e)
    {
      if (e.DataObject.GetDataPresent(typeof (string)))
      {
        if (this.IsValidText((string) e.DataObject.GetData(typeof (string))))
          return;
        e.CancelCommand();
      }
      else
        e.CancelCommand();
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.Framework;component/licensing/activationdialog/view/validationidentrypageview.xaml", UriKind.Relative));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerNonUserCode]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      if (connectionId == 1)
        this.itemsControl = (ItemsControl) target;
      else
        this._contentLoaded = true;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerNonUserCode]
    void IStyleConnector.Connect(int connectionId, object target)
    {
      if (connectionId != 2)
        return;
      ((UIElement) target).AddHandler(DataObject.PastingEvent, (Delegate) new DataObjectPastingEventHandler(this.TextBox_Pasting));
      ((TextBoxBase) target).TextChanged += new TextChangedEventHandler(this.TextBox_TextChanged);
      ((UIElement) target).PreviewTextInput += new TextCompositionEventHandler(this.TextBox_PreviewTextInput);
    }
  }
}
