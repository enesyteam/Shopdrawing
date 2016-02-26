// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.Classifiers.RuleState
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using System;

namespace Microsoft.Expression.Code.Classifiers
{
  public struct RuleState : IComparable<RuleState>
  {
    private bool isResuming;

    public bool IsResuming
    {
      get
      {
        return this.isResuming;
      }
      set
      {
        this.isResuming = value;
      }
    }

    public RuleState(bool isResuming)
    {
      this.isResuming = isResuming;
    }

    public int CompareTo(RuleState other)
    {
      return this.isResuming != other.isResuming ? 1 : 0;
    }
  }
}
