// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Model.ModelItem
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.Windows.Design.Model
{
  public abstract class ModelItem : INotifyPropertyChanged
  {
    public abstract ModelProperty Content { get; }

    public abstract EditingContext Context { get; }

    public abstract ModelEventCollection Events { get; }

    public abstract Type ItemType { get; }

    public abstract string Name { get; set; }

    public abstract ModelItem Parent { get; }

    public abstract ModelItem Root { get; }

    public abstract ModelPropertyCollection Properties { get; }

    public abstract ModelProperty Source { get; }

    public abstract ViewItem View { get; }

    public abstract event PropertyChangedEventHandler PropertyChanged;

    public abstract ModelEditingScope BeginEdit();

    public abstract ModelEditingScope BeginEdit(string description);

    public abstract IEnumerable<object> GetAttributes(Type attributeType);

    public virtual IEnumerable<object> GetAttributes(TypeIdentifier attributeTypeIdentifier)
    {
      Type attributeType = this.Context.Services.GetRequiredService<ModelService>().InvokeResolveType(attributeTypeIdentifier);
      if (attributeType != null)
        return this.GetAttributes(attributeType);
      return (IEnumerable<object>) new object[0];
    }

    public abstract object GetCurrentValue();

    public virtual bool IsItemOfType(Type type)
    {
      if (type == null)
        throw new ArgumentNullException("type");
      return type.IsAssignableFrom(this.ItemType);
    }

    public virtual bool IsItemOfType(TypeIdentifier typeIdentifier)
    {
      Type type = this.Context.Services.GetRequiredService<ModelService>().InvokeResolveType(typeIdentifier);
      if (type != null)
        return this.IsItemOfType(type);
      return false;
    }
  }
}
