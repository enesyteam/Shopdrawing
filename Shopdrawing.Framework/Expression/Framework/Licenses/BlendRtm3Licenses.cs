// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Licenses.BlendRtm3Licenses
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.Framework.Licenses
{
  public sealed class BlendRtm3Licenses : ApplicationLicenses
  {
    private static readonly Guid volumeTfsSku = new Guid("{6d797e33-8455-4f03-ad7e-b700451553e7}");
    private static readonly Guid volumeSku = new Guid("{0ca8ec52-2d98-4b61-a4b6-74e993cde40e}");
    private static readonly Guid msdnSku = new Guid("{3f142a72-191f-4a62-b904-27ef6edd91ac}");
    private static readonly Guid msdnTfsSku = new Guid("{2e98d80f-a5fc-4e0e-9558-ee9dca6bfde4}");
    private static readonly Guid msdnSketchFlowSku = new Guid("{744810e6-5441-4a5b-9041-9cee3bf12e4a}");
    private static readonly Guid msdnTfsSketchFlowSku = new Guid("{4309f50f-5372-400b-9ce0-09e92b8ad55b}");
    private static readonly Guid[] skus = new Guid[6]
    {
      BlendRtm3Licenses.volumeTfsSku,
      BlendRtm3Licenses.volumeSku,
      BlendRtm3Licenses.msdnSku,
      BlendRtm3Licenses.msdnTfsSku,
      BlendRtm3Licenses.msdnSketchFlowSku,
      BlendRtm3Licenses.msdnTfsSketchFlowSku
    };
    public const string ApplicationIdString = "{ca99f1ce-22eb-42d8-827c-80d1e7de789d}";
    private const string volumeTfsSkuString = "{6d797e33-8455-4f03-ad7e-b700451553e7}";
    private const string volumeSkuString = "{0ca8ec52-2d98-4b61-a4b6-74e993cde40e}";
    private const string msdnSkuString = "{3f142a72-191f-4a62-b904-27ef6edd91ac}";
    private const string msdnTfsSkuString = "{2e98d80f-a5fc-4e0e-9558-ee9dca6bfde4}";
    private const string msdnSketchFlowSkuString = "{744810e6-5441-4a5b-9041-9cee3bf12e4a}";
    private const string msdnTfsSketchFlowSkuString = "{4309f50f-5372-400b-9ce0-09e92b8ad55b}";

    public BlendRtm3Licenses()
    {
      this.ApplicationId = new Guid("{ca99f1ce-22eb-42d8-827c-80d1e7de789d}");
      this.MpcDictionary["1033"] = 2121;
      this.MpcDictionary["1036"] = 2141;
      this.MpcDictionary["3082"] = 2146;
      this.MpcDictionary["1031"] = 2142;
      this.MpcDictionary["1040"] = 2143;
      this.MpcDictionary["1041"] = 2144;
      this.MpcDictionary["2052"] = 2138;
      this.MpcDictionary["1028"] = 2139;
      this.MpcDictionary["1042"] = 2145;
      this.SqmSkuIdMapping["{6d797e33-8455-4f03-ad7e-b700451553e7}"] = 40;
      this.SqmSkuIdMapping["{0ca8ec52-2d98-4b61-a4b6-74e993cde40e}"] = 40;
      this.SqmSkuIdMapping["{3f142a72-191f-4a62-b904-27ef6edd91ac}"] = 50;
      this.SqmSkuIdMapping["{2e98d80f-a5fc-4e0e-9558-ee9dca6bfde4}"] = 50;
      this.SqmSkuIdMapping["{744810e6-5441-4a5b-9041-9cee3bf12e4a}"] = 50;
      this.SqmSkuIdMapping["{4309f50f-5372-400b-9ce0-09e92b8ad55b}"] = 50;
      this.Licenses = new LicenseInformation(this.ApplicationId, (IEnumerable<Guid>) BlendRtm3Licenses.skus, this.SqmSkuIdMapping);
    }
  }
}
