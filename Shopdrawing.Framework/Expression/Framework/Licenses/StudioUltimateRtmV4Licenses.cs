// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Licenses.StudioUltimateRtmV4Licenses
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.Framework.Licenses
{
  public class StudioUltimateRtmV4Licenses : ApplicationLicenses
  {
    private static readonly Guid fppSku = new Guid("{b8e73f9e-e01a-4f6c-815d-8eb0f54c26a4}");
    private static readonly Guid bizSparkSku = new Guid("{b4c18846-4ffa-4945-b455-97d3c005a1a4}");
    private static readonly Guid dreamSparkSku = new Guid("{856b0b25-0069-4573-ba07-11fc7978cd72}");
    private static readonly Guid MsdnSku = new Guid("{7f5bf21d-4ef0-43d0-bb95-59ac9e6c9724}");
    private static readonly Guid MsdnAaSku = new Guid("{cf91f2ad-8883-44fa-be48-dc9544e730bf}");
    private static readonly Guid volumeSku = new Guid("{f4bbf0ae-cb89-47e7-9cac-559a1886036c}");
    private static readonly Guid[] skus = new Guid[6]
    {
      StudioUltimateRtmV4Licenses.fppSku,
      StudioUltimateRtmV4Licenses.bizSparkSku,
      StudioUltimateRtmV4Licenses.dreamSparkSku,
      StudioUltimateRtmV4Licenses.MsdnSku,
      StudioUltimateRtmV4Licenses.MsdnAaSku,
      StudioUltimateRtmV4Licenses.volumeSku
    };
    public const string ApplicationIdString = "{d258e7f7-e0ca-44e6-8793-aa2147c3e9a1}";
    internal const string FppSkuString = "{b8e73f9e-e01a-4f6c-815d-8eb0f54c26a4}";
    internal const string BizSparkSkuString = "{b4c18846-4ffa-4945-b455-97d3c005a1a4}";
    internal const string DreamSparkSkuString = "{856b0b25-0069-4573-ba07-11fc7978cd72}";
    internal const string MsdnSkuString = "{7f5bf21d-4ef0-43d0-bb95-59ac9e6c9724}";
    internal const string MsdnAaSkuString = "{cf91f2ad-8883-44fa-be48-dc9544e730bf}";
    internal const string VolumeSkuString = "{f4bbf0ae-cb89-47e7-9cac-559a1886036c}";

    public StudioUltimateRtmV4Licenses()
    {
      this.ApplicationId = new Guid("{d258e7f7-e0ca-44e6-8793-aa2147c3e9a1}");
      this.MpcDictionary["1033"] = 2768;
      this.MpcDictionary["1036"] = 2769;
      this.MpcDictionary["3082"] = 2803;
      this.MpcDictionary["1031"] = 2771;
      this.MpcDictionary["1040"] = 2776;
      this.MpcDictionary["1041"] = 2777;
      this.MpcDictionary["2052"] = 1512;
      this.MpcDictionary["1028"] = 2765;
      this.MpcDictionary["1042"] = 2800;
      this.SqmSkuIdMapping["{b8e73f9e-e01a-4f6c-815d-8eb0f54c26a4}"] = 20;
      this.SqmSkuIdMapping["{b4c18846-4ffa-4945-b455-97d3c005a1a4}"] = 70;
      this.SqmSkuIdMapping["{856b0b25-0069-4573-ba07-11fc7978cd72}"] = 30;
      this.SqmSkuIdMapping["{7f5bf21d-4ef0-43d0-bb95-59ac9e6c9724}"] = 50;
      this.SqmSkuIdMapping["{cf91f2ad-8883-44fa-be48-dc9544e730bf}"] = 60;
      this.SqmSkuIdMapping["{f4bbf0ae-cb89-47e7-9cac-559a1886036c}"] = 40;
      this.Licenses = new LicenseInformation(this.ApplicationId, (IEnumerable<Guid>) StudioUltimateRtmV4Licenses.skus, this.SqmSkuIdMapping);
    }
  }
}
