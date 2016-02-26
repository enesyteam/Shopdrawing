// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Interaction.RelativeValueCollection
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using System;
using System.Collections.ObjectModel;

namespace Microsoft.Windows.Design.Interaction
{
  public class RelativeValueCollection : Collection<RelativeValue>
  {
    public RelativeValue Find(RelativePosition position)
    {
      if (position == (RelativePosition) null)
        throw new ArgumentNullException("position");
      foreach (RelativeValue relativeValue in (Collection<RelativeValue>) this)
      {
        if (relativeValue.Position == position)
          return relativeValue;
      }
      return new RelativeValue();
    }
  }
}
