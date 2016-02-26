// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Licenses.WebTrialRtmV4Licenses
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.Framework.Licenses
{
  public sealed class WebTrialRtmV4Licenses : ApplicationLicenses
  {
    private static readonly Guid trialSku = new Guid("{df43e38d-b4cc-4153-ad69-381ff01d7d9c}");
    private static readonly Guid[] skus = new Guid[1]
    {
      WebTrialRtmV4Licenses.trialSku
    };
    public const string ApplicationIdString = "{cac81ac6-50d9-4670-aaf3-bee3c581f3c8}";
    internal const string TrialSkuString = "{df43e38d-b4cc-4153-ad69-381ff01d7d9c}";

    public WebTrialRtmV4Licenses()
    {
      this.ApplicationId = new Guid("{cac81ac6-50d9-4670-aaf3-bee3c581f3c8}");
      this.MpcDictionary["1033"] = 2735;
      this.MpcDictionary["1036"] = 2738;
      this.MpcDictionary["3082"] = 2760;
      this.MpcDictionary["1031"] = 2739;
      this.MpcDictionary["1040"] = 2740;
      this.MpcDictionary["1041"] = 2751;
      this.MpcDictionary["2052"] = 2727;
      this.MpcDictionary["1028"] = 2734;
      this.MpcDictionary["1042"] = 2755;
      this.SqmSkuIdMapping["{df43e38d-b4cc-4153-ad69-381ff01d7d9c}"] = 10;
      this.Licenses = new LicenseInformation(this.ApplicationId, (IEnumerable<Guid>) WebTrialRtmV4Licenses.skus, this.SqmSkuIdMapping);
    }
  }
}
