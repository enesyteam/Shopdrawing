// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Licenses.StudioPremiumRtmV4Licenses
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.Framework.Licenses
{
  public class StudioPremiumRtmV4Licenses : ApplicationLicenses
  {
    private static readonly Guid premiumSku = new Guid("{463e13b0-6790-4187-9f8c-802e800c2d1a}");
    private static readonly Guid mpnSku = new Guid("{069de601-cfd0-4dbd-a5b5-0633730332a0}");
    private static readonly Guid MsdnSku = new Guid("{c6e70907-d45f-4a6c-943c-750b0ccd59b3}");
    private static readonly Guid volumeSku = new Guid("{e9ee0197-890d-4d52-9aeb-256f78981d25}");
    private static readonly Guid webSiteSparkSku = new Guid("{d29b1461-3dc9-4148-910f-bd5c7e7fc2d1}");
    private static readonly Guid[] skus = new Guid[5]
    {
      StudioPremiumRtmV4Licenses.premiumSku,
      StudioPremiumRtmV4Licenses.mpnSku,
      StudioPremiumRtmV4Licenses.MsdnSku,
      StudioPremiumRtmV4Licenses.volumeSku,
      StudioPremiumRtmV4Licenses.webSiteSparkSku
    };
    public const string ApplicationIdString = "{e6620b01-588a-4af4-ba10-e67080d239f9}";
    internal const string PremiumSkuString = "{463e13b0-6790-4187-9f8c-802e800c2d1a}";
    internal const string MpnSkuString = "{069de601-cfd0-4dbd-a5b5-0633730332a0}";
    internal const string MsdnSkuString = "{c6e70907-d45f-4a6c-943c-750b0ccd59b3}";
    internal const string VolumeSkuString = "{e9ee0197-890d-4d52-9aeb-256f78981d25}";
    internal const string WebSiteSparkSkuString = "{d29b1461-3dc9-4148-910f-bd5c7e7fc2d1}";

    public StudioPremiumRtmV4Licenses()
    {
      this.ApplicationId = new Guid("{e6620b01-588a-4af4-ba10-e67080d239f9}");
      this.MpcDictionary["1033"] = 2768;
      this.MpcDictionary["1036"] = 2769;
      this.MpcDictionary["3082"] = 2803;
      this.MpcDictionary["1031"] = 2771;
      this.MpcDictionary["1040"] = 2776;
      this.MpcDictionary["1041"] = 2777;
      this.MpcDictionary["2052"] = 1512;
      this.MpcDictionary["1028"] = 2765;
      this.MpcDictionary["1042"] = 2800;
      this.SqmSkuIdMapping["{463e13b0-6790-4187-9f8c-802e800c2d1a}"] = 20;
      this.SqmSkuIdMapping["{069de601-cfd0-4dbd-a5b5-0633730332a0}"] = 140;
      this.SqmSkuIdMapping["{c6e70907-d45f-4a6c-943c-750b0ccd59b3}"] = 60;
      this.SqmSkuIdMapping["{d29b1461-3dc9-4148-910f-bd5c7e7fc2d1}"] = 120;
      this.Licenses = new LicenseInformation(this.ApplicationId, (IEnumerable<Guid>) StudioPremiumRtmV4Licenses.skus, this.SqmSkuIdMapping);
    }
  }
}
