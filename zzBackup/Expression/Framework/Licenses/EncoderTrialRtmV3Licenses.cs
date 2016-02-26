// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Licenses.EncoderTrialRtmV3Licenses
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.Framework.Licenses
{
  internal class EncoderTrialRtmV3Licenses : ApplicationLicenses
  {
    private static readonly Guid trialSku = new Guid("{37f1889e-63ab-428f-b748-c8d20ae365eb}");
    private static readonly Guid[] skus = new Guid[1]
    {
      EncoderTrialRtmV3Licenses.trialSku
    };
    public const string ApplicationIdString = "{3656cde6-54d8-4d28-ab2e-9f0ba2952a95}";
    private const string trialSkuString = "{37f1889e-63ab-428f-b748-c8d20ae365eb}";

    public EncoderTrialRtmV3Licenses()
    {
      this.ApplicationId = new Guid("{3656cde6-54d8-4d28-ab2e-9f0ba2952a95}");
      this.MpcDictionary["1033"] = 2131;
      this.MpcDictionary["1036"] = 2132;
      this.MpcDictionary["3082"] = 2137;
      this.MpcDictionary["1031"] = 2133;
      this.MpcDictionary["1040"] = 2134;
      this.MpcDictionary["1041"] = 2135;
      this.MpcDictionary["2052"] = 2123;
      this.MpcDictionary["1028"] = 2124;
      this.MpcDictionary["1042"] = 2136;
      this.SqmSkuIdMapping["{37f1889e-63ab-428f-b748-c8d20ae365eb}"] = 10;
      this.Licenses = new LicenseInformation(this.ApplicationId, (IEnumerable<Guid>) EncoderTrialRtmV3Licenses.skus, this.SqmSkuIdMapping);
    }
  }
}
