// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.PropertyEditing.IPropertyFilterTarget
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using System;

namespace Microsoft.Windows.Design.PropertyEditing
{
  public interface IPropertyFilterTarget
  {
    bool MatchesFilter { get; }

    event EventHandler<PropertyFilterAppliedEventArgs> FilterApplied;

    void ApplyFilter(PropertyFilter filter);

    bool MatchesPredicate(PropertyFilterPredicate predicate);
  }
}
