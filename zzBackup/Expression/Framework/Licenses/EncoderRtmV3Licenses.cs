// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Licenses.EncoderRtmV3Licenses
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.Framework.Licenses
{
  internal class EncoderRtmV3Licenses : ApplicationLicenses
  {
    private static readonly Guid retailSku = new Guid("{4bd8f8e5-c877-40a3-ae7f-8cfa524be8a1}");
    private static readonly Guid oemSku = new Guid("{3c161fa8-9825-4060-8c1b-5a04b9300f00}");
    private static readonly Guid volumeSku = new Guid("{ec794143-5bbb-4f0b-a2f2-dd4799db5163}");
    private static readonly Guid[] skus = new Guid[3]
    {
      EncoderRtmV3Licenses.retailSku,
      EncoderRtmV3Licenses.volumeSku,
      EncoderRtmV3Licenses.oemSku
    };
    public const string ApplicationIdString = "{10499802-32b3-4382-9020-50b2349774a1}";
    private const string retailSkuString = "{4bd8f8e5-c877-40a3-ae7f-8cfa524be8a1}";
    private const string oemSkuString = "{3c161fa8-9825-4060-8c1b-5a04b9300f00}";
    private const string volumeSkuString = "{ec794143-5bbb-4f0b-a2f2-dd4799db5163}";

    public EncoderRtmV3Licenses()
    {
      this.ApplicationId = new Guid("{10499802-32b3-4382-9020-50b2349774a1}");
      this.MpcDictionary["1033"] = 2131;
      this.MpcDictionary["1036"] = 2132;
      this.MpcDictionary["3082"] = 2137;
      this.MpcDictionary["1031"] = 2133;
      this.MpcDictionary["1040"] = 2134;
      this.MpcDictionary["1041"] = 2135;
      this.MpcDictionary["2052"] = 2123;
      this.MpcDictionary["1028"] = 2124;
      this.MpcDictionary["1042"] = 2136;
      this.SqmSkuIdMapping["{4bd8f8e5-c877-40a3-ae7f-8cfa524be8a1}"] = 20;
      this.SqmSkuIdMapping["{ec794143-5bbb-4f0b-a2f2-dd4799db5163}"] = 40;
      this.SqmSkuIdMapping["{3c161fa8-9825-4060-8c1b-5a04b9300f00}"] = 80;
      this.Licenses = new LicenseInformation(this.ApplicationId, (IEnumerable<Guid>) EncoderRtmV3Licenses.skus, this.SqmSkuIdMapping);
    }
  }
}
