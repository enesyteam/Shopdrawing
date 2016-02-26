// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.Classifiers.PatternRule
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.VisualStudio.Text.Classification;

namespace Microsoft.Expression.Code.Classifiers
{
  public class PatternRule : IRule
  {
    private char? escapeCharacter = new char?();
    private string startSequence;
    private string endSequence;
    private IClassificationType classification;
    private bool finishesOnEOL;
    private string skipString;

    public char? EscapeCharacter
    {
      get
      {
        return this.escapeCharacter;
      }
      set
      {
        this.escapeCharacter = value;
      }
    }

    public string SkipString
    {
      get
      {
        return this.skipString;
      }
      set
      {
        this.skipString = value;
      }
    }

    public PatternRule(string startSequence, string endSequence, IClassificationType classification, bool finishesOnEOL)
    {
      this.startSequence = startSequence;
      this.endSequence = endSequence;
      this.classification = classification;
      this.finishesOnEOL = finishesOnEOL;
    }

    public IClassificationType Run(CharacterScanner scanner, RuleState startState, out RuleState endState)
    {
      RuleState ruleState = startState;
      int cursor = scanner.Cursor;
      if (!ruleState.IsResuming && !string.IsNullOrEmpty(this.startSequence))
      {
        if ((int) scanner.CurrentCharacter != (int) this.startSequence[0])
        {
          endState = new RuleState();
          return (IClassificationType) null;
        }
        if (!scanner.MatchString(this.startSequence))
        {
          endState = new RuleState();
          return (IClassificationType) null;
        }
      }
      bool flag = false;
      if (string.IsNullOrEmpty(this.endSequence))
      {
        while (!scanner.IsEndOfScan)
        {
          if (this.finishesOnEOL && (int) scanner.CurrentCharacter == 10)
          {
            flag = true;
            break;
          }
          scanner.MoveNext();
        }
      }
      else
        flag = this.MatchEndSequence(scanner) || scanner.IsEndOfFile;
      endState = new RuleState(!flag);
      return this.classification;
    }

    private bool MatchEndSequence(CharacterScanner scanner)
    {
      bool flag = false;
      while (!scanner.IsEndOfScan)
      {
        char currentCharacter = scanner.CurrentCharacter;
        if (this.skipString == null || flag || !scanner.MatchString(this.skipString))
        {
          if ((int) currentCharacter == (int) this.endSequence[0] && !flag && scanner.MatchString(this.endSequence))
            return true;
          int num1;
          if (this.escapeCharacter.HasValue)
          {
            char? nullable = this.escapeCharacter;
            int num2 = (int) currentCharacter;
            num1 = (int) nullable.GetValueOrDefault() != num2 ? 0 : (nullable.HasValue ? 1 : 0);
          }
          else
            num1 = 0;
          flag = num1 != 0;
          scanner.MoveNext();
          if (this.finishesOnEOL && (int) currentCharacter == 10)
            return true;
        }
      }
      return false;
    }
  }
}
