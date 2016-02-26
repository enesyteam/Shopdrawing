// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Licenses.BlendMobileRtmV4Licenses
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.Framework.Licenses
{
  public sealed class BlendMobileRtmV4Licenses : ApplicationLicenses
  {
    private static readonly Guid blendMobilTrialV4Sku = new Guid("{f688aae7-8353-4f77-a45c-b15abbc2dd0d}");
    private static readonly Guid blendMobileRtmV4Sku = new Guid("{def1f499-3ff3-4e86-afcc-afb5ffa89659}");
    private static readonly Guid[] skus = new Guid[2]
    {
      BlendMobileRtmV4Licenses.blendMobileRtmV4Sku,
      BlendMobileRtmV4Licenses.blendMobilTrialV4Sku
    };
    public const string ApplicationIdString = "{7a8138b9-5847-44af-a0a4-3abc5e508e00}";
    internal const string BlendMobileTrialRtmv4SkuString = "{f688aae7-8353-4f77-a45c-b15abbc2dd0d}";
    internal const string BlendMobileRtmV4SkuString = "{def1f499-3ff3-4e86-afcc-afb5ffa89659}";

    public BlendMobileRtmV4Licenses()
    {
      this.ApplicationId = new Guid("{7a8138b9-5847-44af-a0a4-3abc5e508e00}");
      this.MpcDictionary["1033"] = 2768;
      this.MpcDictionary["1036"] = 2769;
      this.MpcDictionary["3082"] = 2803;
      this.MpcDictionary["1031"] = 2771;
      this.MpcDictionary["1040"] = 2776;
      this.MpcDictionary["1041"] = 2777;
      this.MpcDictionary["2052"] = 1512;
      this.MpcDictionary["1028"] = 2765;
      this.MpcDictionary["1042"] = 2800;
      this.SqmSkuIdMapping["{f688aae7-8353-4f77-a45c-b15abbc2dd0d}"] = 10;
      this.SqmSkuIdMapping["{def1f499-3ff3-4e86-afcc-afb5ffa89659}"] = 40;
      this.Licenses = new LicenseInformation(this.ApplicationId, (IEnumerable<Guid>) BlendMobileRtmV4Licenses.skus, this.SqmSkuIdMapping);
    }
  }
}
