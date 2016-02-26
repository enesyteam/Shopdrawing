// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Controls.GroupableComboBox
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Microsoft.Expression.Framework.Controls
{
  public class GroupableComboBox : ComboBox
  {
    public static readonly DependencyProperty BindableGroupStyleProperty = DependencyProperty.Register("BindableGroupStyle", typeof (GroupStyle), typeof (GroupableComboBox));

    public GroupStyle BindableGroupStyle
    {
      get
      {
        return (GroupStyle) this.GetValue(GroupableComboBox.BindableGroupStyleProperty);
      }
      set
      {
        this.SetValue(GroupableComboBox.BindableGroupStyleProperty, (object) value);
      }
    }

    protected override void OnInitialized(EventArgs e)
    {
      base.OnInitialized(e);
      this.GroupStyleSelector = new GroupStyleSelector(this.BoundGroupStyleSelector);
    }

    private GroupStyle BoundGroupStyleSelector(CollectionViewGroup group, int level)
    {
      return this.BindableGroupStyle;
    }
  }
}
