// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Model.PropertyIdentifierCollection
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design.Metadata;
using MS.Internal.Properties;
using System;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Microsoft.Windows.Design.Model
{
  public class PropertyIdentifierCollection : Collection<PropertyIdentifier>
  {
    public void Add(TypeIdentifier typeIdentifier, string name)
    {
      this.Add(new PropertyIdentifier(typeIdentifier, name));
    }

    public void Add(Type ownerType, string name)
    {
      this.Add(new PropertyIdentifier(ownerType, name));
    }

    protected override void InsertItem(int index, PropertyIdentifier item)
    {
      if (item.IsEmpty)
        throw new ArgumentNullException("item");
      if (this.Contains(item))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_DuplicateItem, new object[1]
        {
          (object) item.Name
        }));
      this.InsertItem(index, item);
    }

    protected override void SetItem(int index, PropertyIdentifier item)
    {
      if (item.IsEmpty)
        throw new ArgumentNullException("item");
      if (this.Contains(item))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_DuplicateItem, new object[1]
        {
          (object) item.Name
        }));
      this.SetItem(index, item);
    }
  }
}
