// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Policies.SecondarySelectionPolicy
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;

namespace Microsoft.Windows.Design.Policies
{
  public class SecondarySelectionPolicy : SelectionPolicy
  {
    protected override bool IsInPolicy(Selection selection, ModelItem item)
    {
      return item != selection.PrimarySelection;
    }
  }
}
