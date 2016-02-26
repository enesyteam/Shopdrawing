// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.Classifiers.WordRule
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.VisualStudio.Text.Classification;
using System.Collections.Generic;

namespace Microsoft.Expression.Code.Classifiers
{
  public class WordRule : IRule
  {
    private IClassificationType classification;
    private List<string> words;

    public WordRule(ICollection<string> words, IClassificationType classification)
    {
      this.words = new List<string>(words.Count);
      foreach (string str in (IEnumerable<string>) words)
        this.words.Add(str);
      this.words.Sort();
      this.classification = classification;
    }

    public IClassificationType Run(CharacterScanner scanner, RuleState startState, out RuleState endState)
    {
      endState = new RuleState();
      if (!WordRule.IsCharIdentifierToken(scanner.Peek(-1)))
      {
        for (int index = 0; index < this.words.Count; ++index)
        {
          if (scanner.MatchString(this.words[index]))
          {
            if (scanner.IsEndOfScan || !WordRule.IsCharIdentifierToken(scanner.CurrentCharacter))
              return this.classification;
            scanner.Seek(-this.words[index].Length);
          }
        }
      }
      return (IClassificationType) null;
    }

    private static bool IsCharIdentifierToken(char c)
    {
      if (!char.IsLetterOrDigit(c))
        return (int) c == 95;
      return true;
    }
  }
}
