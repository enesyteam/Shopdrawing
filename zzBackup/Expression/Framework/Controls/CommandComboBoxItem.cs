// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Controls.CommandComboBoxItem
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Microsoft.Expression.Framework.Controls
{
  public class CommandComboBoxItem : ComboBoxItem
  {
    public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof (ICommand), typeof (CommandComboBoxItem), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));

    public ICommand Command
    {
      get
      {
        return (ICommand) this.GetValue(CommandComboBoxItem.CommandProperty);
      }
      set
      {
        this.SetValue(CommandComboBoxItem.CommandProperty, (object) value);
      }
    }

    protected override void OnSelected(RoutedEventArgs e)
    {
      base.OnSelected(e);
      if (this.Command == null)
        return;
      this.Command.Execute((object) null);
    }
  }
}
