// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Diagnostics.DumpNodeToCountedStringListConverter
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Microsoft.Expression.Framework.Diagnostics
{
  public abstract class DumpNodeToCountedStringListConverter : IValueConverter
  {
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return (object) null;
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null)
        return (object) null;
      List<CountedString> countedStrings = new List<CountedString>();
      DumpNode dumpNode = value as DumpNode;
      if (dumpNode != null)
      {
        this.BuildCountedStrings(dumpNode.Visual, countedStrings);
        countedStrings.Sort(new Comparison<CountedString>(this.CompareCountedStrings));
      }
      return (object) countedStrings;
    }

    protected abstract string StringFromVisual(Visual visual);

    private void BuildCountedStrings(Visual root, List<CountedString> countedStrings)
    {
      for (int childIndex = 0; childIndex < VisualTreeHelper.GetChildrenCount((DependencyObject) root); ++childIndex)
      {
        Visual root1 = VisualTreeHelper.GetChild((DependencyObject) root, childIndex) as Visual;
        if (root1 != null)
          this.BuildCountedStrings(root1, countedStrings);
      }
      string stringValue = this.StringFromVisual(root);
      foreach (CountedString countedString in countedStrings)
      {
        if (countedString.StringValue == stringValue)
        {
          ++countedString.Count;
          return;
        }
      }
      countedStrings.Add(new CountedString(stringValue));
    }

    private int CompareCountedStrings(CountedString x, CountedString y)
    {
      return y.Count - x.Count;
    }
  }
}
