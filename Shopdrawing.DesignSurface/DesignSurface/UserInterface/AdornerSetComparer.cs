// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.AdornerSetComparer
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Windows.Design;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  public sealed class AdornerSetComparer : IComparer<IAdornerSet>
  {
    private IList<IAdornerSet> initialOrdering;

    public AdornerSetComparer(IList<IAdornerSet> initialOrdering)
    {
      this.initialOrdering = (IList<IAdornerSet>) new List<IAdornerSet>((IEnumerable<IAdornerSet>) initialOrdering);
    }

    public int Compare(IAdornerSet x, IAdornerSet y)
    {
      int num = x.Order.CompareTo((OrderToken) y.Order);
      if (num == 0)
        return this.initialOrdering.IndexOf(x) - this.initialOrdering.IndexOf(y);
      return num;
    }
  }
}
