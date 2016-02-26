// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Licenses.StudioRtmV3ForV4Licenses
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.Framework.Licenses
{
  public class StudioRtmV3ForV4Licenses : ApplicationLicenses
  {
    private static readonly Guid fppSku = new Guid("{5a269d7b-546f-4e2b-9b59-c115f074240d}");
    private static readonly Guid[] skus = new Guid[1]
    {
      StudioRtmV3ForV4Licenses.fppSku
    };
    public const string ApplicationIdString = "{5f7ac106-a519-4526-96d7-6d82112e5882}";
    internal const string FppSkuString = "{5a269d7b-546f-4e2b-9b59-c115f074240d}";

    public StudioRtmV3ForV4Licenses()
    {
      this.ApplicationId = new Guid("{5f7ac106-a519-4526-96d7-6d82112e5882}");
      this.MpcDictionary["1033"] = 2768;
      this.MpcDictionary["1036"] = 2769;
      this.MpcDictionary["3082"] = 2803;
      this.MpcDictionary["1031"] = 2771;
      this.MpcDictionary["1040"] = 2776;
      this.MpcDictionary["1041"] = 2777;
      this.MpcDictionary["2052"] = 1512;
      this.MpcDictionary["1028"] = 2765;
      this.MpcDictionary["1042"] = 2800;
      this.SqmSkuIdMapping["{5a269d7b-546f-4e2b-9b59-c115f074240d}"] = 20;
      this.Licenses = new LicenseInformation(this.ApplicationId, (IEnumerable<Guid>) StudioRtmV3ForV4Licenses.skus, this.SqmSkuIdMapping);
    }
  }
}
