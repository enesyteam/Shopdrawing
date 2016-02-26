// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Utility.Globalization.CultureInfoLcidComparer
// Assembly: Microsoft.Expression.Utility, Version=5.0.30709.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: B77F0E80-A3D7-4861-BF76-6A6A586443F3
// Assembly location: C:\Users\M4600\Documents\Project\4.5\Microsoft.Expression.ProjectReferences\Microsoft.Expression.Utility.dll

using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Expression.Utility.Globalization
{
  public class CultureInfoLcidComparer : IEqualityComparer<CultureInfo>
  {
    public bool Equals(CultureInfo x, CultureInfo y)
    {
      return CultureInfoLcidComparer.AreEqual(x, y);
    }

    public static bool AreEqual(CultureInfo x, CultureInfo y)
    {
      if (object.ReferenceEquals((object) x, (object) y))
        return true;
      if (object.ReferenceEquals((object) x, (object) null) || object.ReferenceEquals((object) y, (object) null))
        return false;
      return x.LCID == y.LCID;
    }

    public int GetHashCode(CultureInfo obj)
    {
      if (object.ReferenceEquals((object) obj, (object) null))
        return 0;
      return obj.LCID;
    }
  }
}
