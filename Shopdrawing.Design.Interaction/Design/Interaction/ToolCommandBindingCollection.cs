// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Interaction.ToolCommandBindingCollection
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Windows.Design.Interaction
{
  public class ToolCommandBindingCollection : Collection<ToolCommandBinding>
  {
    public void AddRange(ICollection<ToolCommandBinding> bindings)
    {
      if (bindings == null)
        throw new ArgumentNullException("bindings");
      foreach (ToolCommandBinding toolCommandBinding in (IEnumerable<ToolCommandBinding>) bindings)
        this.Add(toolCommandBinding);
    }

    protected override void InsertItem(int index, ToolCommandBinding item)
    {
      if (item == null)
        throw new ArgumentNullException("item");
      this.InsertItem(index, item);
    }

    protected override void SetItem(int index, ToolCommandBinding item)
    {
      if (item == null)
        throw new ArgumentNullException("item");
      this.SetItem(index, item);
    }
  }
}
