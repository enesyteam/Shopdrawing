// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.Classifiers.RuleScannerState
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using System;

namespace Microsoft.Expression.Code.Classifiers
{
  internal struct RuleScannerState : IComparable<RuleScannerState>
  {
    private int currentRule;
    private RuleState ruleState;

    public int CurrentRule
    {
      get
      {
        return this.currentRule;
      }
      set
      {
        this.currentRule = value;
      }
    }

    public RuleState RuleState
    {
      get
      {
        return this.ruleState;
      }
      set
      {
        this.ruleState = value;
      }
    }

    public RuleScannerState(int currentRule, RuleState ruleState)
    {
      this.currentRule = currentRule;
      this.ruleState = ruleState;
    }

    public int CompareTo(RuleScannerState other)
    {
      if (this.currentRule != other.currentRule)
        return 1;
      return this.ruleState.CompareTo(other.ruleState);
    }
  }
}
