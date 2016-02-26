// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Licenses.DesignTrialRtmV4Licenses
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.Framework.Licenses
{
  public sealed class DesignTrialRtmV4Licenses : ApplicationLicenses
  {
    private static readonly Guid trialSku = new Guid("{0d5b4a1d-12e2-4c28-9a80-06f9e0a880b3}");
    private static readonly Guid[] skus = new Guid[1]
    {
      DesignTrialRtmV4Licenses.trialSku
    };
    public const string ApplicationIdString = "{336e3ad2-abef-40ad-9d85-a63f6d1d1662}";
    internal const string TrialSkuString = "{0d5b4a1d-12e2-4c28-9a80-06f9e0a880b3}";

    public DesignTrialRtmV4Licenses()
    {
      this.ApplicationId = new Guid("{336e3ad2-abef-40ad-9d85-a63f6d1d1662}");
      this.MpcDictionary["1033"] = 2768;
      this.MpcDictionary["1036"] = 2769;
      this.MpcDictionary["3082"] = 2803;
      this.MpcDictionary["1031"] = 2771;
      this.MpcDictionary["1040"] = 2776;
      this.MpcDictionary["1041"] = 2777;
      this.MpcDictionary["2052"] = 1512;
      this.MpcDictionary["1028"] = 2765;
      this.MpcDictionary["1042"] = 2800;
      this.SqmSkuIdMapping["{0d5b4a1d-12e2-4c28-9a80-06f9e0a880b3}"] = 10;
      this.Licenses = new LicenseInformation(this.ApplicationId, (IEnumerable<Guid>) DesignTrialRtmV4Licenses.skus, this.SqmSkuIdMapping);
    }
  }
}
