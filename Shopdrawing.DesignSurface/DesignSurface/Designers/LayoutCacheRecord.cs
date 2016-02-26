// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Designers.LayoutCacheRecord
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.Designers
{
  public class LayoutCacheRecord
  {
    public Rect Rect { get; private set; }

    public Point SlotOrigin { get; private set; }

    public LayoutOverrides Overrides { get; private set; }

    public bool Overlapping { get; private set; }

    public LayoutCacheRecord(Rect rect, Point slotOrigin, LayoutOverrides overrides, bool overlapping)
    {
      this.Rect = rect;
      this.SlotOrigin = slotOrigin;
      this.Overrides = overrides;
      this.Overlapping = overlapping;
    }
  }
}
