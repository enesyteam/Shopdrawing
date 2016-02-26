// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.Classifiers.TokenClassification
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.VisualStudio.Text.Classification;
using System.Collections.Generic;

namespace Microsoft.Expression.Code.Classifiers
{
  public class TokenClassification : IClassificationType
  {
    private string classification;
    private int uniqueId;

    public int UniqueId
    {
      get
      {
        return this.uniqueId;
      }
    }

    public IEnumerable<IClassificationType> BaseTypes
    {
      get
      {
        yield break;
      }
    }

    public string Classification
    {
      get
      {
        return this.classification;
      }
    }

    public TokenClassification(string classification, int uniqueId)
    {
      this.classification = classification;
      this.uniqueId = uniqueId;
    }

    public bool IsOfType(string type)
    {
      return this.classification == type;
    }

    public override string ToString()
    {
      return this.classification;
    }
  }
}
