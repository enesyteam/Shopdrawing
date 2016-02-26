// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.Classifiers.TokenClassificationStore
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.VisualStudio.Text.Classification;
using System.Collections.Generic;

namespace Microsoft.Expression.Code.Classifiers
{
  public static class TokenClassificationStore
  {
    private static Dictionary<string, IClassificationType> map = new Dictionary<string, IClassificationType>();
    private static int currentTokenId;

    public static IClassificationType GetTokenType(string name)
    {
      IClassificationType classificationType;
      if (!TokenClassificationStore.map.TryGetValue(name, out classificationType))
      {
        classificationType = (IClassificationType) new TokenClassification(name, TokenClassificationStore.currentTokenId++);
        TokenClassificationStore.map[name] = classificationType;
      }
      return classificationType;
    }
  }
}
