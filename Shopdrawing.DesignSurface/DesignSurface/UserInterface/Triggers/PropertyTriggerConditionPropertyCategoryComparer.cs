// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Triggers.PropertyTriggerConditionPropertyCategoryComparer
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.UserInterface;
using System.Collections;
using System.Globalization;

namespace Microsoft.Expression.DesignSurface.UserInterface.Triggers
{
  internal class PropertyTriggerConditionPropertyCategoryComparer : IComparer
  {
    int IComparer.Compare(object x, object y)
    {
      PropertyInformation propertyInformation1 = x as PropertyInformation;
      PropertyInformation propertyInformation2 = y as PropertyInformation;
      if ((TriggerSourceInformation) propertyInformation1 == (TriggerSourceInformation) null || (TriggerSourceInformation) propertyInformation2 == (TriggerSourceInformation) null)
        return 0;
      int num = string.Compare(propertyInformation1.GroupBy, propertyInformation2.GroupBy, true, CultureInfo.CurrentCulture);
      if (num == 0)
        return string.Compare(propertyInformation1.DisplayName, propertyInformation2.DisplayName, true, CultureInfo.CurrentCulture);
      if (propertyInformation1.GroupBy == StringTable.CategoryNameCommonProperties)
        return -1;
      if (propertyInformation2.GroupBy == StringTable.CategoryNameCommonProperties)
        return 1;
      return num;
    }
  }
}
