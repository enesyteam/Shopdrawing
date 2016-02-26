// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Licenses.SketchFlowDevV4Licenses
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.Framework.Licenses
{
  public sealed class SketchFlowDevV4Licenses : ApplicationLicenses
  {
    private static readonly Guid sketchFlowDevV4Sku = new Guid("{e197e308-61d1-4057-ba06-c3e8392e0fbd}");
    private static readonly Guid[] skus = new Guid[1]
    {
      SketchFlowDevV4Licenses.sketchFlowDevV4Sku
    };
    private const string ApplicationIdString = "{6356fa23-b3ce-4f63-97f9-0f9f3c7f66a2}";
    public const string SketchFlowDevV4SkuString = "{e197e308-61d1-4057-ba06-c3e8392e0fbd}";

    public SketchFlowDevV4Licenses()
    {
      this.ApplicationId = new Guid("{6356fa23-b3ce-4f63-97f9-0f9f3c7f66a2}");
      this.MpcDictionary["1033"] = 121;
      this.MpcDictionary["1036"] = 122;
      this.MpcDictionary["3082"] = (int) sbyte.MaxValue;
      this.MpcDictionary["1031"] = 123;
      this.MpcDictionary["1040"] = 124;
      this.MpcDictionary["1041"] = 125;
      this.MpcDictionary["2052"] = 119;
      this.MpcDictionary["1028"] = 120;
      this.MpcDictionary["1042"] = 126;
      this.SqmSkuIdMapping["{e197e308-61d1-4057-ba06-c3e8392e0fbd}"] = 100;
      this.Licenses = new LicenseInformation(this.ApplicationId, (IEnumerable<Guid>) SketchFlowDevV4Licenses.skus, this.SqmSkuIdMapping);
    }
  }
}
