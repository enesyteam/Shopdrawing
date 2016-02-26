// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Data.LocalizableSwitchConverter
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Microsoft.Expression.Framework.Data
{
  [ContentProperty("Cases")]
  public class LocalizableSwitchConverter : DependencyObject, IValueConverter
  {
    private static readonly DependencyProperty DefaultValueProperty = DependencyProperty.Register("DefaultValue", typeof (string), typeof (LocalizableSwitchConverter));
    private static readonly DependencyProperty BasedOnProperty = DependencyProperty.Register("BasedOn", typeof (LocalizableSwitchConverter), typeof (LocalizableSwitchConverter));
    private List<LocalizableSwitchCase> cases;

    public List<LocalizableSwitchCase> Cases
    {
      get
      {
        return this.cases;
      }
    }

    [Localizability(LocalizationCategory.Text, Modifiability = Modifiability.Modifiable, Readability = Readability.Readable)]
    public string DefaultValue
    {
      get
      {
        return (string) this.GetValue(LocalizableSwitchConverter.DefaultValueProperty);
      }
      set
      {
        this.SetValue(LocalizableSwitchConverter.DefaultValueProperty, (object) value);
      }
    }

    public LocalizableSwitchConverter BasedOn
    {
      get
      {
        return (LocalizableSwitchConverter) this.GetValue(LocalizableSwitchConverter.BasedOnProperty);
      }
      set
      {
        this.SetValue(LocalizableSwitchConverter.BasedOnProperty, (object) value);
      }
    }

    public LocalizableSwitchConverter()
    {
      this.cases = new List<LocalizableSwitchCase>();
    }

    public object Convert(object o, Type targetType, object parameter, CultureInfo culture)
    {
      foreach (LocalizableSwitchCase localizableSwitchCase in this.Cases)
      {
        if (object.Equals(localizableSwitchCase.In, o))
          return (object) localizableSwitchCase.Out;
      }
      if (this.BasedOn != null)
        return this.BasedOn.Convert(o, targetType, parameter, culture);
      return (object) this.DefaultValue;
    }

    public object ConvertBack(object o, Type targetType, object parameter, CultureInfo culture)
    {
      throw new InvalidOperationException(ExceptionStringTable.SwitchConverterIsOneWay);
    }
  }
}
