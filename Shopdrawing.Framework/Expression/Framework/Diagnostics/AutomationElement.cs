// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Diagnostics.AutomationElement
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Windows;

namespace Microsoft.Expression.Framework.Diagnostics
{
  public static class AutomationElement
  {
    public static readonly DependencyProperty IdProperty = DependencyProperty.RegisterAttached("Id", typeof (string), typeof (AutomationElement));

    public static string GetId(DependencyObject target)
    {
      if (target == null)
        throw new ArgumentNullException("target");
      return (string) target.GetValue(AutomationElement.IdProperty);
    }

    public static void SetId(DependencyObject target, string value)
    {
      if (target == null)
        throw new ArgumentNullException("target");
      target.SetValue(AutomationElement.IdProperty, (object) value);
    }
  }
}
