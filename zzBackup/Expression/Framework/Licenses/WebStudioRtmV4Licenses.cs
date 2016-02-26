// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Licenses.WebStudioRtmV4Licenses
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.Framework.Licenses
{
  public class WebStudioRtmV4Licenses : ApplicationLicenses
  {
    private static readonly Guid fppSku = new Guid("{d4fc22d4-1099-4520-baed-53f7bebc2dda}");
    private static readonly Guid mpnSku = new Guid("{cd98ea84-3800-4822-9e90-a250b51e5870}");
    private static readonly Guid MsdnSku = new Guid("{6a17ac99-d3d6-4dc0-a682-b823790a66e9}");
    private static readonly Guid volumeSku = new Guid("{5fe02fcb-f830-477d-a83f-feb71e26ded3}");
    private static readonly Guid webSiteSparkSku = new Guid("{9b031466-127f-45b1-9aae-94dfcf9fda46}");
    private static readonly Guid[] skus = new Guid[5]
    {
      WebStudioRtmV4Licenses.fppSku,
      WebStudioRtmV4Licenses.mpnSku,
      WebStudioRtmV4Licenses.MsdnSku,
      WebStudioRtmV4Licenses.volumeSku,
      WebStudioRtmV4Licenses.webSiteSparkSku
    };
    public const string ApplicationIdString = "{daedec6e-42d2-4011-a6f2-6265845e9908}";
    internal const string FppSkuString = "{d4fc22d4-1099-4520-baed-53f7bebc2dda}";
    internal const string MpnSkuString = "{cd98ea84-3800-4822-9e90-a250b51e5870}";
    internal const string MsdnSkuString = "{6a17ac99-d3d6-4dc0-a682-b823790a66e9}";
    internal const string VolumeSkuString = "{5fe02fcb-f830-477d-a83f-feb71e26ded3}";
    internal const string WebSiteSparkSkuString = "{9b031466-127f-45b1-9aae-94dfcf9fda46}";

    public WebStudioRtmV4Licenses()
    {
      this.ApplicationId = new Guid("{daedec6e-42d2-4011-a6f2-6265845e9908}");
      this.MpcDictionary["1033"] = 2735;
      this.MpcDictionary["1036"] = 2738;
      this.MpcDictionary["3082"] = 2760;
      this.MpcDictionary["1031"] = 2739;
      this.MpcDictionary["1040"] = 2740;
      this.MpcDictionary["1041"] = 2751;
      this.MpcDictionary["2052"] = 2727;
      this.MpcDictionary["1028"] = 2734;
      this.MpcDictionary["1042"] = 2755;
      this.SqmSkuIdMapping["{d4fc22d4-1099-4520-baed-53f7bebc2dda}"] = 20;
      this.SqmSkuIdMapping["{cd98ea84-3800-4822-9e90-a250b51e5870}"] = 140;
      this.SqmSkuIdMapping["{6a17ac99-d3d6-4dc0-a682-b823790a66e9}"] = 50;
      this.SqmSkuIdMapping["{5fe02fcb-f830-477d-a83f-feb71e26ded3}"] = 40;
      this.SqmSkuIdMapping["{9b031466-127f-45b1-9aae-94dfcf9fda46}"] = 120;
      this.Licenses = new LicenseInformation(this.ApplicationId, (IEnumerable<Guid>) WebStudioRtmV4Licenses.skus, this.SqmSkuIdMapping);
    }
  }
}
