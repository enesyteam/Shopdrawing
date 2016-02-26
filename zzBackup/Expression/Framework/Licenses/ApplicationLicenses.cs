// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Licenses.ApplicationLicenses
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.Framework.Licenses
{
  public abstract class ApplicationLicenses
  {
    private readonly Dictionary<string, int> mpcCodes = new Dictionary<string, int>();
    private readonly Dictionary<string, int> sqmSkuIdMapping = new Dictionary<string, int>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    protected const string English = "1033";
    protected const string French = "1036";
    protected const string Spanish = "3082";
    protected const string German = "1031";
    protected const string Italian = "1040";
    protected const string Japanese = "1041";
    protected const string ChineseSimplified = "2052";
    protected const string ChineseTraditional = "1028";
    protected const string Korean = "1042";

    public Guid ApplicationId { get; protected set; }

    public LicenseInformation Licenses { get; protected set; }

    public Dictionary<string, int> SqmSkuIdMapping
    {
      get
      {
        return this.sqmSkuIdMapping;
      }
    }

    public Dictionary<string, int> MpcDictionary
    {
      get
      {
        return this.mpcCodes;
      }
    }

    public int DefaultMpc
    {
      get
      {
        return this.mpcCodes["1033"];
      }
    }
  }
}
