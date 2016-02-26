// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Model.ModelEvent
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Services;
using System;
using System.Collections.Generic;

namespace Microsoft.Windows.Design.Model
{
  public abstract class ModelEvent
  {
    public abstract Type EventType { get; }

    public abstract ICollection<string> Handlers { get; }

    public abstract bool IsBrowsable { get; }

    public abstract string Name { get; }

    public abstract ModelItem Parent { get; }

    public abstract IEnumerable<object> GetAttributes(Type attributeType);

    public virtual IEnumerable<object> GetAttributes(TypeIdentifier attributeTypeIdentifier)
    {
      Type attributeType = this.Parent.Context.Services.GetRequiredService<ModelService>().InvokeResolveType(attributeTypeIdentifier);
      if (attributeType != null)
        return this.GetAttributes(attributeType);
      return (IEnumerable<object>) new object[0];
    }

    public virtual bool IsEventOfType(Type type)
    {
      if (type == null)
        throw new ArgumentNullException("type");
      return type.IsAssignableFrom(this.EventType);
    }

    public virtual bool IsEventOfType(TypeIdentifier typeIdentifier)
    {
      Type type = this.Parent.Context.Services.GetRequiredService<ModelService>().InvokeResolveType(typeIdentifier);
      if (type != null)
        return this.IsEventOfType(type);
      return false;
    }
  }
}
