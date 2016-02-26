// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Licenses.AggregateLicenseInformation
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Expression.Framework.Licenses
{
  public sealed class AggregateLicenseInformation
  {
    public IEnumerable<LicenseInformation> Licenses { get; private set; }

    public AggregateLicenseInformation(IEnumerable<ApplicationLicenses> licenses)
    {
      if (licenses != null)
        this.Licenses = (IEnumerable<LicenseInformation>) new List<LicenseInformation>((IEnumerable<LicenseInformation>) new List<LicenseInformation>(Enumerable.Select<ApplicationLicenses, LicenseInformation>(licenses, (Func<ApplicationLicenses, LicenseInformation>) (license => license.Licenses))));
      else
        this.Licenses = (IEnumerable<LicenseInformation>) new List<LicenseInformation>((IEnumerable<LicenseInformation>) new List<LicenseInformation>());
    }

    public int FindSqmSkuId(Guid productSkuId)
    {
      foreach (LicenseInformation licenseInformation in this.Licenses)
      {
        int sqmSkuId = licenseInformation.FindSqmSkuId(productSkuId);
        if (sqmSkuId != 0)
          return sqmSkuId;
      }
      return 0;
    }
  }
}
