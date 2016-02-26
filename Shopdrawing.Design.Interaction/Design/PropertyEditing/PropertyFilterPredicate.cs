// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.PropertyEditing.PropertyFilterPredicate
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using System;
using System.Globalization;

namespace Microsoft.Windows.Design.PropertyEditing
{
  public class PropertyFilterPredicate
  {
    private string _matchText;

    protected string MatchText
    {
      get
      {
        return this._matchText;
      }
    }

    public PropertyFilterPredicate(string matchText)
    {
      if (matchText == null)
        throw new ArgumentNullException("matchText");
      this._matchText = matchText.ToUpper(CultureInfo.InvariantCulture);
    }

    public virtual bool Match(string target)
    {
      if (target != null)
        return target.ToUpper(CultureInfo.InvariantCulture).Contains(this._matchText);
      return false;
    }
  }
}
