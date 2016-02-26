// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Licenses.EncoderRtmV3ForEncoderRtmV4Licenses
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.Framework.Licenses
{
  public class EncoderRtmV3ForEncoderRtmV4Licenses : ApplicationLicenses
  {
    private static readonly Guid retailSku = new Guid("{4bd8f8e5-c877-40a3-ae7f-8cfa524be8a1}");
    private static readonly Guid[] skus = new Guid[1]
    {
      EncoderRtmV3ForEncoderRtmV4Licenses.retailSku
    };
    public const string ApplicationIdString = "{10499802-32b3-4382-9020-50b2349774a1}";
    internal const string RetailSkuString = "{4bd8f8e5-c877-40a3-ae7f-8cfa524be8a1}";

    public EncoderRtmV3ForEncoderRtmV4Licenses()
    {
      this.ApplicationId = new Guid("{10499802-32b3-4382-9020-50b2349774a1}");
      this.MpcDictionary["1033"] = 2768;
      this.MpcDictionary["1036"] = 2769;
      this.MpcDictionary["3082"] = 2803;
      this.MpcDictionary["1031"] = 2771;
      this.MpcDictionary["1040"] = 2776;
      this.MpcDictionary["1041"] = 2777;
      this.MpcDictionary["2052"] = 1512;
      this.MpcDictionary["1028"] = 2765;
      this.MpcDictionary["1042"] = 2800;
      this.SqmSkuIdMapping["{4bd8f8e5-c877-40a3-ae7f-8cfa524be8a1}"] = 20;
      this.Licenses = new LicenseInformation(this.ApplicationId, (IEnumerable<Guid>) EncoderRtmV3ForEncoderRtmV4Licenses.skus, this.SqmSkuIdMapping);
    }
  }
}
