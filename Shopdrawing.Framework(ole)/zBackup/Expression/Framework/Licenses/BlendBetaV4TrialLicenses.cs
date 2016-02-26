// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Licenses.BlendBetaV4TrialLicenses
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.Framework.Licenses
{
  public sealed class BlendBetaV4TrialLicenses : ApplicationLicenses
  {
    private static readonly Guid blendBetaV4TrialSku = new Guid("{18983bd1-cfa2-4aef-a122-cb54aec83c07}");
    private static readonly Guid[] skus = new Guid[1]
    {
      BlendBetaV4TrialLicenses.blendBetaV4TrialSku
    };
    public const string ApplicationIdString = "{4d66144b-d01e-4620-9dee-89304db7a974}";
    private const string blendBetaV4TrialSkuString = "{18983bd1-cfa2-4aef-a122-cb54aec83c07}";

    public BlendBetaV4TrialLicenses()
    {
      this.ApplicationId = new Guid("{4d66144b-d01e-4620-9dee-89304db7a974}");
      this.MpcDictionary["1033"] = 74;
      this.MpcDictionary["1036"] = 78;
      this.MpcDictionary["3082"] = 84;
      this.MpcDictionary["1031"] = 80;
      this.MpcDictionary["1040"] = 81;
      this.MpcDictionary["1041"] = 82;
      this.MpcDictionary["2052"] = 75;
      this.MpcDictionary["1028"] = 76;
      this.MpcDictionary["1042"] = 83;
      this.SqmSkuIdMapping["{18983bd1-cfa2-4aef-a122-cb54aec83c07}"] = 100;
      this.Licenses = new LicenseInformation(this.ApplicationId, (IEnumerable<Guid>) BlendBetaV4TrialLicenses.skus, this.SqmSkuIdMapping);
    }
  }
}
