// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Licenses.EncoderUltimateRtmV4Licenses
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Extensions.Enumerable;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Expression.Framework.Licenses
{
  public sealed class EncoderUltimateRtmV4Licenses : ApplicationLicenses
  {
    private static readonly Guid EncoderUltimateSku = new Guid("{e27610de-1b2b-4c66-884e-b1a6820c44c2}");
    private static readonly Guid EncoderUltimateVlSku = new Guid("{e0cb3045-63a2-496c-aef2-17f27afc62ca}");
    private static readonly Guid[] skus = new Guid[2]
    {
      EncoderUltimateRtmV4Licenses.EncoderUltimateSku,
      EncoderUltimateRtmV4Licenses.EncoderUltimateVlSku
    };
    public const string ApplicationIdString = "{eaa89a7c-d288-4a52-9b68-54930f18ffb7}";
    internal const string EncoderUltimateSkuString = "{e27610de-1b2b-4c66-884e-b1a6820c44c2}";
    internal const string EncoderUltimateVlSkuString = "{e0cb3045-63a2-496c-aef2-17f27afc62ca}";

    public EncoderUltimateRtmV4Licenses()
    {
      this.ApplicationId = new Guid("{eaa89a7c-d288-4a52-9b68-54930f18ffb7}");
      this.MpcDictionary["1033"] = 2768;
      this.MpcDictionary["1036"] = 2769;
      this.MpcDictionary["3082"] = 2803;
      this.MpcDictionary["1031"] = 2771;
      this.MpcDictionary["1040"] = 2776;
      this.MpcDictionary["1041"] = 2777;
      this.MpcDictionary["2052"] = 1512;
      this.MpcDictionary["1028"] = 2765;
      this.MpcDictionary["1042"] = 2800;
      this.SqmSkuIdMapping["{e27610de-1b2b-4c66-884e-b1a6820c44c2}"] = 20;
      this.SqmSkuIdMapping["{e0cb3045-63a2-496c-aef2-17f27afc62ca}"] = 40;
      this.Licenses = new LicenseInformation(this.ApplicationId, (IEnumerable<Guid>) EncoderUltimateRtmV4Licenses.skus, this.SqmSkuIdMapping);
    }

    public AggregateLicenseInformation CreateLicenseStore(IList<ApplicationLicenses> studioLicenses)
    {
      return new AggregateLicenseInformation(Enumerable.Concat<ApplicationLicenses>((IEnumerable<ApplicationLicenses>) studioLicenses, EnumerableExtensions.AsEnumerable<ApplicationLicenses>((ApplicationLicenses) this)));
    }
  }
}
