// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.DataStorePropertyEntrySorter
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.Collections;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class DataStorePropertyEntrySorter : IComparer
  {
    public int Compare(object x, object y)
    {
      DataStorePropertyEntry storePropertyEntry1 = (DataStorePropertyEntry) x;
      DataStorePropertyEntry storePropertyEntry2 = (DataStorePropertyEntry) y;
      if (storePropertyEntry1.IsCreateNewPropertyEntry && storePropertyEntry2.IsCreateNewPropertyEntry)
        return 0;
      if (storePropertyEntry1.IsCreateNewPropertyEntry)
        return -1;
      if (storePropertyEntry2.IsCreateNewPropertyEntry)
        return 1;
      int num = string.Compare(storePropertyEntry1.DataSetName, storePropertyEntry2.DataSetName, StringComparison.CurrentCulture);
      if (num != 0)
        return num;
      return string.Compare(storePropertyEntry1.Name, storePropertyEntry2.Name, StringComparison.CurrentCulture);
    }
  }
}
