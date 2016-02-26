// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Licenses.XWebRtmV3Licenses
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.Framework.Licenses
{
  internal sealed class XWebRtmV3Licenses : ApplicationLicenses
  {
    private static readonly Guid volumeTfsSku = new Guid("{89f199f0-aa24-423b-82ab-ce0af3210fd3}");
    private static readonly Guid msdnTfsSku = new Guid("{df3b5921-bcd3-4b24-8f0a-faf181ed8519}");
    private static readonly Guid retailTfsSku = new Guid("{6e639a98-943b-4c27-8386-a3a7321edc0a}");
    private static readonly Guid volumeSku = new Guid("{ea582887-6e99-43c2-b1f6-f23447c441ba}");
    private static readonly Guid msdnSku = new Guid("{e48755ff-ede3-46fa-b3d9-3f34bf7ecf5f}");
    private static readonly Guid retailSku = new Guid("{0f5ced6d-1c9b-47e4-a61b-a22c0e855268}");
    private static readonly Guid[] skus = new Guid[6]
    {
      XWebRtmV3Licenses.volumeTfsSku,
      XWebRtmV3Licenses.msdnTfsSku,
      XWebRtmV3Licenses.retailTfsSku,
      XWebRtmV3Licenses.volumeSku,
      XWebRtmV3Licenses.msdnSku,
      XWebRtmV3Licenses.retailSku
    };
    public const string ApplicationIdString = "{84fdb498-cdc0-4d7f-8326-c88aa8f21124}";
    public const string volumeTfsSkuString = "{89f199f0-aa24-423b-82ab-ce0af3210fd3}";
    public const string msdnTfsSkuString = "{df3b5921-bcd3-4b24-8f0a-faf181ed8519}";
    public const string retailTfsSkuString = "{6e639a98-943b-4c27-8386-a3a7321edc0a}";
    public const string volumeSkuString = "{ea582887-6e99-43c2-b1f6-f23447c441ba}";
    public const string msdnSkuString = "{e48755ff-ede3-46fa-b3d9-3f34bf7ecf5f}";
    public const string retailSkuString = "{0f5ced6d-1c9b-47e4-a61b-a22c0e855268}";

    public XWebRtmV3Licenses()
    {
      this.ApplicationId = new Guid("{84fdb498-cdc0-4d7f-8326-c88aa8f21124}");
      this.MpcDictionary["1033"] = 2122;
      this.MpcDictionary["1036"] = 2149;
      this.MpcDictionary["3082"] = 2154;
      this.MpcDictionary["1031"] = 2150;
      this.MpcDictionary["1040"] = 2151;
      this.MpcDictionary["1041"] = 2152;
      this.MpcDictionary["2052"] = 2147;
      this.MpcDictionary["1028"] = 2148;
      this.MpcDictionary["1042"] = 2153;
      this.SqmSkuIdMapping["{89f199f0-aa24-423b-82ab-ce0af3210fd3}"] = 40;
      this.SqmSkuIdMapping["{df3b5921-bcd3-4b24-8f0a-faf181ed8519}"] = 50;
      this.SqmSkuIdMapping["{6e639a98-943b-4c27-8386-a3a7321edc0a}"] = 20;
      this.SqmSkuIdMapping["{ea582887-6e99-43c2-b1f6-f23447c441ba}"] = 40;
      this.SqmSkuIdMapping["{e48755ff-ede3-46fa-b3d9-3f34bf7ecf5f}"] = 50;
      this.SqmSkuIdMapping["{0f5ced6d-1c9b-47e4-a61b-a22c0e855268}"] = 20;
      this.Licenses = new LicenseInformation(this.ApplicationId, (IEnumerable<Guid>) XWebRtmV3Licenses.skus, this.SqmSkuIdMapping);
    }
  }
}
