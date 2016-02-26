// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.DocumentProcessors.ReferenceChangeModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.DocumentProcessors
{
  internal abstract class ReferenceChangeModel
  {
    public string OldReferenceValue { get; private set; }

    public string NewReferenceValue { get; private set; }

    public abstract DocumentSearchScope ChangeScope { get; }

    public abstract IEnumerable<ReferenceRepairer> ReferenceRepairers { get; }

    public ReferenceChangeModel(string oldReferenceValue, string newReferenceValue)
    {
      this.OldReferenceValue = oldReferenceValue;
      this.NewReferenceValue = newReferenceValue;
    }
  }
}
