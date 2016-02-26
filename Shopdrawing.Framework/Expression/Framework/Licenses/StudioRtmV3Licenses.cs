// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Licenses.StudioRtmV3Licenses
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.Framework.Licenses
{
  public class StudioRtmV3Licenses : ApplicationLicenses
  {
    private static readonly Guid bizSparkSku = new Guid("{04e47d4b-3241-4181-b751-b8177ca8bf94}");
    private static readonly Guid fppTfsSku = new Guid("{6e7e421a-149c-4992-aef3-d412b94e9b07}");
    private static readonly Guid volumeTfsSku = new Guid("{e5cef98e-9c83-4a02-b10a-b4529998b8f7}");
    private static readonly Guid dreamSparkSku = new Guid("{2e43a4ad-4cfc-44c8-95cc-c47ece1935e8}");
    private static readonly Guid fppSku = new Guid("{5a269d7b-546f-4e2b-9b59-c115f074240d}");
    private static readonly Guid volumeSku = new Guid("{b9627d39-3b31-4810-b0b8-d2971b8a4392}");
    private static readonly Guid roleSku = new Guid("{d7d3d886-f1ef-4eea-ae2e-099088d7ca07}");
    private static readonly Guid[] skus = new Guid[7]
    {
      StudioRtmV3Licenses.bizSparkSku,
      StudioRtmV3Licenses.fppTfsSku,
      StudioRtmV3Licenses.volumeTfsSku,
      StudioRtmV3Licenses.dreamSparkSku,
      StudioRtmV3Licenses.fppSku,
      StudioRtmV3Licenses.volumeSku,
      StudioRtmV3Licenses.roleSku
    };
    public const string ApplicationIdString = "{5f7ac106-a519-4526-96d7-6d82112e5882}";
    private const string bizSparkSkuString = "{04e47d4b-3241-4181-b751-b8177ca8bf94}";
    private const string fppTfsSkuString = "{6e7e421a-149c-4992-aef3-d412b94e9b07}";
    private const string volumeTfsSkuString = "{e5cef98e-9c83-4a02-b10a-b4529998b8f7}";
    private const string dreamSparkSkuString = "{2e43a4ad-4cfc-44c8-95cc-c47ece1935e8}";
    private const string fppSkuString = "{5a269d7b-546f-4e2b-9b59-c115f074240d}";
    private const string volumeSkuString = "{b9627d39-3b31-4810-b0b8-d2971b8a4392}";
    private const string roleSkuString = "{d7d3d886-f1ef-4eea-ae2e-099088d7ca07}";

    public StudioRtmV3Licenses()
    {
      this.ApplicationId = new Guid("{5f7ac106-a519-4526-96d7-6d82112e5882}");
      this.MpcDictionary["1033"] = 2121;
      this.MpcDictionary["1036"] = 2141;
      this.MpcDictionary["3082"] = 2146;
      this.MpcDictionary["1031"] = 2142;
      this.MpcDictionary["1040"] = 2143;
      this.MpcDictionary["1041"] = 2144;
      this.MpcDictionary["2052"] = 2138;
      this.MpcDictionary["1028"] = 2139;
      this.MpcDictionary["1042"] = 2145;
      this.SqmSkuIdMapping["{04e47d4b-3241-4181-b751-b8177ca8bf94}"] = 70;
      this.SqmSkuIdMapping["{6e7e421a-149c-4992-aef3-d412b94e9b07}"] = 120;
      this.SqmSkuIdMapping["{e5cef98e-9c83-4a02-b10a-b4529998b8f7}"] = 60;
      this.SqmSkuIdMapping["{2e43a4ad-4cfc-44c8-95cc-c47ece1935e8}"] = 30;
      this.SqmSkuIdMapping["{5a269d7b-546f-4e2b-9b59-c115f074240d}"] = 20;
      this.SqmSkuIdMapping["{b9627d39-3b31-4810-b0b8-d2971b8a4392}"] = 40;
      this.SqmSkuIdMapping["{d7d3d886-f1ef-4eea-ae2e-099088d7ca07}"] = 130;
      this.Licenses = new LicenseInformation(this.ApplicationId, (IEnumerable<Guid>) StudioRtmV3Licenses.skus, this.SqmSkuIdMapping);
    }
  }
}
