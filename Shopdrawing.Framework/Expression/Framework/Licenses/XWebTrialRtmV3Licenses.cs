// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Licenses.XWebTrialRtmV3Licenses
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.Framework.Licenses
{
  internal class XWebTrialRtmV3Licenses : ApplicationLicenses
  {
    private static readonly Guid trialSku = new Guid("{b8b6e9f5-82c2-4b60-b35c-acd7a223697e}");
    private static readonly Guid[] skus = new Guid[1]
    {
      XWebTrialRtmV3Licenses.trialSku
    };
    public const string ApplicationIdString = "{db257659-da28-40ee-b163-44e52f629055}";
    public const string trialSkuString = "{b8b6e9f5-82c2-4b60-b35c-acd7a223697e}";

    public XWebTrialRtmV3Licenses()
    {
      this.ApplicationId = new Guid("{db257659-da28-40ee-b163-44e52f629055}");
      this.MpcDictionary["1033"] = 2122;
      this.MpcDictionary["1036"] = 2149;
      this.MpcDictionary["3082"] = 2154;
      this.MpcDictionary["1031"] = 2150;
      this.MpcDictionary["1040"] = 2151;
      this.MpcDictionary["1041"] = 2152;
      this.MpcDictionary["2052"] = 2147;
      this.MpcDictionary["1028"] = 2148;
      this.MpcDictionary["1042"] = 2153;
      this.SqmSkuIdMapping["{b8b6e9f5-82c2-4b60-b35c-acd7a223697e}"] = 10;
      this.Licenses = new LicenseInformation(this.ApplicationId, (IEnumerable<Guid>) XWebTrialRtmV3Licenses.skus, this.SqmSkuIdMapping);
    }
  }
}
