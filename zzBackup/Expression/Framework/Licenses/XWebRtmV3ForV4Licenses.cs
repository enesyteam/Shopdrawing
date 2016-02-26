// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Licenses.XWebRtmV3ForV4Licenses
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.Framework.Licenses
{
  public sealed class XWebRtmV3ForV4Licenses : ApplicationLicenses
  {
    private static readonly Guid retailSku = new Guid("{0f5ced6d-1c9b-47e4-a61b-a22c0e855268}");
    private static readonly Guid[] skus = new Guid[1]
    {
      XWebRtmV3ForV4Licenses.retailSku
    };
    public const string ApplicationIdString = "{84fdb498-cdc0-4d7f-8326-c88aa8f21124}";
    public const string RetailSkuString = "{0f5ced6d-1c9b-47e4-a61b-a22c0e855268}";

    public XWebRtmV3ForV4Licenses()
    {
      this.ApplicationId = new Guid("{84fdb498-cdc0-4d7f-8326-c88aa8f21124}");
      this.MpcDictionary["1033"] = 2735;
      this.MpcDictionary["1036"] = 2738;
      this.MpcDictionary["3082"] = 2760;
      this.MpcDictionary["1031"] = 2739;
      this.MpcDictionary["1040"] = 2740;
      this.MpcDictionary["1041"] = 2751;
      this.MpcDictionary["2052"] = 2727;
      this.MpcDictionary["1028"] = 2734;
      this.MpcDictionary["1042"] = 2755;
      this.SqmSkuIdMapping["{0f5ced6d-1c9b-47e4-a61b-a22c0e855268}"] = 20;
      this.Licenses = new LicenseInformation(this.ApplicationId, (IEnumerable<Guid>) XWebRtmV3ForV4Licenses.skus, this.SqmSkuIdMapping);
    }
  }
}
