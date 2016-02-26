// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Model.PropertyInvalidatedEventArgs
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design.Metadata;
using System;

namespace Microsoft.Windows.Design.Model
{
  public class PropertyInvalidatedEventArgs : EventArgs
  {
    private ModelItem _item;
    private PropertyIdentifier _property;

    public ModelItem Item
    {
      get
      {
        return this._item;
      }
    }

    public PropertyIdentifier InvalidatedProperty
    {
      get
      {
        return this._property;
      }
    }

    public PropertyInvalidatedEventArgs(ModelItem item, PropertyIdentifier property)
    {
      if (item == null)
        throw new ArgumentNullException("item");
      if (item == null)
        throw new ArgumentNullException("property");
      this._item = item;
      this._property = property;
    }
  }
}
