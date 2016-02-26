// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Data.ArrayInsertion
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System.Windows;

namespace Microsoft.Expression.Framework.Data
{
  public class ArrayInsertion : DependencyObject
  {
    private static readonly DependencyProperty PositionProperty = DependencyProperty.Register("Position", typeof (int), typeof (ArrayInsertion));
    private static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof (object), typeof (ArrayInsertion));

    public int Position
    {
      get
      {
        return (int) this.GetValue(ArrayInsertion.PositionProperty);
      }
      set
      {
        this.SetValue(ArrayInsertion.PositionProperty, (object) value);
      }
    }

    public object Value
    {
      get
      {
        return this.GetValue(ArrayInsertion.ValueProperty);
      }
      set
      {
        this.SetValue(ArrayInsertion.ValueProperty, value);
      }
    }
  }
}
