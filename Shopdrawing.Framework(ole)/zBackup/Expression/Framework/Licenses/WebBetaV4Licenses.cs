// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Licenses.WebBetaV4Licenses
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.Framework.Licenses
{
  public sealed class WebBetaV4Licenses : ApplicationLicenses
  {
    private static readonly Guid webBetaV4Sku = new Guid("{10f04870-5296-4be3-81f9-fd61794f7d1a}");
    private static readonly Guid[] skus = new Guid[1]
    {
      WebBetaV4Licenses.webBetaV4Sku
    };
    public const string ApplicationIdString = "{d137e610-f1f2-4065-a176-abea2ef27408}";
    public const string WebBetaV4SkuString = "{10f04870-5296-4be3-81f9-fd61794f7d1a}";

    public WebBetaV4Licenses()
    {
      this.ApplicationId = new Guid("{d137e610-f1f2-4065-a176-abea2ef27408}");
      this.MpcDictionary["1033"] = 121;
      this.MpcDictionary["1036"] = 122;
      this.MpcDictionary["3082"] = (int) sbyte.MaxValue;
      this.MpcDictionary["1031"] = 123;
      this.MpcDictionary["1040"] = 124;
      this.MpcDictionary["1041"] = 125;
      this.MpcDictionary["2052"] = 119;
      this.MpcDictionary["1028"] = 120;
      this.MpcDictionary["1042"] = 126;
      this.SqmSkuIdMapping["{10f04870-5296-4be3-81f9-fd61794f7d1a}"] = 100;
      this.Licenses = new LicenseInformation(this.ApplicationId, (IEnumerable<Guid>) WebBetaV4Licenses.skus, this.SqmSkuIdMapping);
    }
  }
}
