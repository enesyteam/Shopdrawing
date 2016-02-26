// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Licenses.StudioDevV4Licenses
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.Framework.Licenses
{
  public sealed class StudioDevV4Licenses : ApplicationLicenses
  {
    private static readonly Guid studioDevV4Sku = new Guid("{889c5dd4-fb5f-4585-82cc-e00da205cecf}");
    private static readonly Guid[] skus = new Guid[1]
    {
      StudioDevV4Licenses.studioDevV4Sku
    };
    public const string ApplicationIdString = "{fee8d295-5cac-4611-8be5-9b7499f069a5}";
    public const string StudioDevV4SkuString = "{889c5dd4-fb5f-4585-82cc-e00da205cecf}";

    public StudioDevV4Licenses()
    {
      this.ApplicationId = new Guid("{fee8d295-5cac-4611-8be5-9b7499f069a5}");
      this.MpcDictionary["1033"] = 121;
      this.MpcDictionary["1036"] = 122;
      this.MpcDictionary["3082"] = (int) sbyte.MaxValue;
      this.MpcDictionary["1031"] = 123;
      this.MpcDictionary["1040"] = 124;
      this.MpcDictionary["1041"] = 125;
      this.MpcDictionary["2052"] = 119;
      this.MpcDictionary["1028"] = 120;
      this.MpcDictionary["1042"] = 126;
      this.SqmSkuIdMapping["{889c5dd4-fb5f-4585-82cc-e00da205cecf}"] = 100;
      this.Licenses = new LicenseInformation(this.ApplicationId, (IEnumerable<Guid>) StudioDevV4Licenses.skus, this.SqmSkuIdMapping);
    }
  }
}
