// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Licenses.BlendTrialRtmV4Licenses
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.Framework.Licenses
{
  public sealed class BlendTrialRtmV4Licenses : ApplicationLicenses
  {
    private static readonly Guid trialSku = new Guid("{7ff82fbc-d611-4eac-93a8-b58985e3074b}");
    private static readonly Guid[] skus = new Guid[1]
    {
      BlendTrialRtmV4Licenses.trialSku
    };
    public const string ApplicationIdString = "{5d76ab22-cd7a-42ea-9756-629f133abd8e}";
    internal const string TrialSkuString = "{7ff82fbc-d611-4eac-93a8-b58985e3074b}";

    public BlendTrialRtmV4Licenses()
    {
      this.ApplicationId = new Guid("{5d76ab22-cd7a-42ea-9756-629f133abd8e}");
      this.MpcDictionary["1033"] = 2768;
      this.MpcDictionary["1036"] = 2769;
      this.MpcDictionary["3082"] = 2803;
      this.MpcDictionary["1031"] = 2771;
      this.MpcDictionary["1040"] = 2776;
      this.MpcDictionary["1041"] = 2777;
      this.MpcDictionary["2052"] = 1512;
      this.MpcDictionary["1028"] = 2765;
      this.MpcDictionary["1042"] = 2800;
      this.SqmSkuIdMapping["{7ff82fbc-d611-4eac-93a8-b58985e3074b}"] = 10;
      this.Licenses = new LicenseInformation(this.ApplicationId, (IEnumerable<Guid>) BlendTrialRtmV4Licenses.skus, this.SqmSkuIdMapping);
    }
  }
}
