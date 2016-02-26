// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Licenses.BlendChinaTrialRtmV3Licenses
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.Framework.Licenses
{
  public sealed class BlendChinaTrialRtmV3Licenses : ApplicationLicenses
  {
    private static readonly Guid trialSku = new Guid("{cbdc4e75-9a06-49ad-9cbb-de7c15b84a92}");
    private static readonly Guid[] skus = new Guid[1]
    {
      BlendChinaTrialRtmV3Licenses.trialSku
    };
    public const string ApplicationIdString = "{cbdc4e75-9a06-49ad-9cbb-de7c15b84a92}";
    private const string trialSkuString = "{cbdc4e75-9a06-49ad-9cbb-de7c15b84a92}";

    public BlendChinaTrialRtmV3Licenses()
    {
      this.ApplicationId = new Guid("{cbdc4e75-9a06-49ad-9cbb-de7c15b84a92}");
      this.MpcDictionary["1033"] = 2121;
      this.MpcDictionary["1036"] = 2141;
      this.MpcDictionary["3082"] = 2146;
      this.MpcDictionary["1031"] = 2142;
      this.MpcDictionary["1040"] = 2143;
      this.MpcDictionary["1041"] = 2144;
      this.MpcDictionary["2052"] = 2138;
      this.MpcDictionary["1028"] = 2139;
      this.MpcDictionary["1042"] = 2145;
      this.SqmSkuIdMapping["{cbdc4e75-9a06-49ad-9cbb-de7c15b84a92}"] = 10;
      this.Licenses = new LicenseInformation(this.ApplicationId, (IEnumerable<Guid>) BlendChinaTrialRtmV3Licenses.skus, this.SqmSkuIdMapping);
    }
  }
}
