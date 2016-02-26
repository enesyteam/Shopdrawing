// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.Classifiers.XamlNamePositionFlags
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using System;

namespace Microsoft.Expression.Code.Classifiers
{
  [Flags]
  public enum XamlNamePositionFlags
  {
    None = 0,
    BeforeStart = 1,
    InBetween = 2,
    AfterEnd = 4,
    CaretPositionMask = AfterEnd | InBetween | BeforeStart,
    Prefix = 8,
    TypeSpecifier = 16,
    Name = 32,
    PartMask = Name | TypeSpecifier | Prefix,
  }
}
