// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.Classifiers.RuleBasedScanner
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using System.Collections.Generic;

namespace Microsoft.Expression.Code.Classifiers
{
  internal class RuleBasedScanner : IClassificationScanner<RuleScannerState>
  {
    private List<IRule> rules;

    public RuleBasedScanner(IList<IRule> rules)
    {
      this.rules = new List<IRule>((IEnumerable<IRule>) rules);
    }

    public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span, RuleScannerState currentState, out RuleScannerState endState)
    {
      CharacterScanner scanner = new CharacterScanner(span);
      List<ClassificationSpan> list = new List<ClassificationSpan>();
      int currentRule = currentState.CurrentRule;
      RuleState endState1 = currentState.RuleState;
      while (!scanner.IsEndOfScan)
      {
        bool flag = false;
        int cursor = scanner.Cursor;
        if (!endState1.IsResuming)
          currentRule = 0;
        for (; currentRule < this.rules.Count; ++currentRule)
        {
          IClassificationType classification = this.rules[currentRule].Run(scanner, endState1, out endState1);
          if (classification != null)
          {
            list.Add(new ClassificationSpan(new SnapshotSpan(scanner.Snapshot, cursor, scanner.Cursor - cursor), classification));
            flag = true;
            break;
          }
        }
        if (!flag)
          scanner.MoveNext();
      }
      endState = new RuleScannerState(currentRule, endState1);
      return (IList<ClassificationSpan>) list;
    }
  }
}
