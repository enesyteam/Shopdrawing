// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Data.SwitchCase
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System.Windows;

namespace Microsoft.Expression.Framework.Data
{
  public class SwitchCase : DependencyObject
  {
    private static readonly DependencyProperty InProperty = DependencyProperty.Register("In", typeof (object), typeof (SwitchCase));
    private static readonly DependencyProperty OutProperty = DependencyProperty.Register("Out", typeof (object), typeof (SwitchCase));

    public object In
    {
      get
      {
        return this.GetValue(SwitchCase.InProperty);
      }
      set
      {
        this.SetValue(SwitchCase.InProperty, value);
      }
    }

    public object Out
    {
      get
      {
        return this.GetValue(SwitchCase.OutProperty);
      }
      set
      {
        this.SetValue(SwitchCase.OutProperty, value);
      }
    }
  }
}
