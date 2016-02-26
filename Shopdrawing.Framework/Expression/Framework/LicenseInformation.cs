// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.LicenseInformation
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Expression.Framework
{
  public class LicenseInformation
  {
    private readonly Dictionary<string, int> skuGuidMappingTable;

    public Guid ApplicationId { get; private set; }

    public IEnumerable<Guid> SkuIds { get; private set; }

    public LicenseInformation(Guid applicationId, IEnumerable<Guid> skuIds, Dictionary<string, int> skuGuidMapping)
    {
      this.ApplicationId = applicationId;
      this.SkuIds = skuIds;
      this.skuGuidMappingTable = skuGuidMapping;
    }

    public bool ContainsSku(Guid skuId)
    {
      if (this.SkuIds != null)
        return Enumerable.Contains<Guid>(this.SkuIds, skuId);
      return false;
    }

    public bool ContainsAnySku(IList<Guid> skus)
    {
      return Enumerable.Count<Guid>(Enumerable.Intersect<Guid>(this.SkuIds, (IEnumerable<Guid>) skus)) > 0;
    }

    public int FindSqmSkuId(Guid skuId)
    {
      return this.FindSqmSkuId(skuId.ToString("B"));
    }

    private int FindSqmSkuId(string skuId)
    {
      int num;
      if (this.skuGuidMappingTable != null && this.skuGuidMappingTable.TryGetValue(skuId, out num))
        return num;
      return 0;
    }
  }
}
