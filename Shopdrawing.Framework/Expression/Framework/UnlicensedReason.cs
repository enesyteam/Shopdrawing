// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.UnlicensedReason
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;

namespace Microsoft.Expression.Framework
{
  [CLSCompliant(false)]
  public enum UnlicensedReason : uint
  {
    None = 0U,
    OobGracePeriod = 1074065420U,
    MismatchedPid = 3221549061U,
    GraceTimeExpired = 3221549065U,
    ProductKeyNotInstalled = 3221549076U,
    InvalidLicense = 3221549087U,
    ValidityTimeExpired = 3221549089U,
    DuplicatePolicy = 3221549138U,
  }
}
