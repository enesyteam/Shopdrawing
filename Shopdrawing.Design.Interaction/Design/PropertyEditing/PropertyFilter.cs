// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.PropertyEditing.PropertyFilter
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Windows.Design.PropertyEditing
{
  public class PropertyFilter
  {
    private List<PropertyFilterPredicate> _predicates = new List<PropertyFilterPredicate>();

    public bool IsEmpty
    {
      get
      {
        if (this._predicates != null)
          return this._predicates.Count == 0;
        return true;
      }
    }

    public PropertyFilter(string filterText)
    {
      this.SetPredicates(filterText);
    }

    public PropertyFilter(IEnumerable<PropertyFilterPredicate> predicates)
    {
      this.SetPredicates(predicates);
    }

    private void SetPredicates(string filterText)
    {
      if (string.IsNullOrEmpty(filterText))
        return;
      string[] strArray = filterText.Split(' ');
      for (int index = 0; index < strArray.Length; ++index)
      {
        if (!string.IsNullOrEmpty(strArray[index]))
          this._predicates.Add(new PropertyFilterPredicate(strArray[index]));
      }
    }

    private void SetPredicates(IEnumerable<PropertyFilterPredicate> predicates)
    {
      if (predicates == null)
        return;
      foreach (PropertyFilterPredicate propertyFilterPredicate in predicates)
      {
        if (propertyFilterPredicate != null)
          this._predicates.Add(propertyFilterPredicate);
      }
    }

    public bool Match(IPropertyFilterTarget target)
    {
      if (target == null)
        throw new ArgumentNullException("target");
      if (this.IsEmpty)
        return true;
      bool flag = false;
      for (int index = 0; index < this._predicates.Count; ++index)
      {
        if (!target.MatchesPredicate(this._predicates[index]))
          return false;
        flag = true;
      }
      return flag;
    }
  }
}
