// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Licenses.StudioBetaV4Licenses
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.Framework.Licenses
{
  public sealed class StudioBetaV4Licenses : ApplicationLicenses
  {
    private static readonly Guid studioV4BetaSku = new Guid("{d17e9278-a600-483e-9335-8336142a6c0e}");
    private static readonly Guid studioV4BetaVolumeSku = new Guid("{0fc4f414-8785-46b5-80e7-5350c7937714}");
    private static readonly Guid[] skus = new Guid[2]
    {
      StudioBetaV4Licenses.studioV4BetaSku,
      StudioBetaV4Licenses.studioV4BetaVolumeSku
    };
    public const string ApplicationIdString = "{e8709c0c-8372-4695-b0fc-73e793d60789}";
    public const string StudioBetaV4String = "{d17e9278-a600-483e-9335-8336142a6c0e}";
    public const string StudioBetaV4VolumeString = "{0fc4f414-8785-46b5-80e7-5350c7937714}";

    public StudioBetaV4Licenses()
    {
      this.ApplicationId = new Guid("{e8709c0c-8372-4695-b0fc-73e793d60789}");
      this.MpcDictionary["1033"] = 121;
      this.MpcDictionary["1036"] = 122;
      this.MpcDictionary["3082"] = (int) sbyte.MaxValue;
      this.MpcDictionary["1031"] = 123;
      this.MpcDictionary["1040"] = 124;
      this.MpcDictionary["1041"] = 125;
      this.MpcDictionary["2052"] = 119;
      this.MpcDictionary["1028"] = 120;
      this.MpcDictionary["1042"] = 126;
      this.SqmSkuIdMapping["{d17e9278-a600-483e-9335-8336142a6c0e}"] = 100;
      this.SqmSkuIdMapping["{0fc4f414-8785-46b5-80e7-5350c7937714}"] = 110;
      this.Licenses = new LicenseInformation(this.ApplicationId, (IEnumerable<Guid>) StudioBetaV4Licenses.skus, this.SqmSkuIdMapping);
    }
  }
}
