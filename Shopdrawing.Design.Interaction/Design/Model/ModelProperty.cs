// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Model.ModelProperty
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Services;
using System;
using System.Collections.Generic;

namespace Microsoft.Windows.Design.Model
{
  public abstract class ModelProperty
  {
    public abstract ModelItemCollection Collection { get; }

    public abstract object ComputedValue { get; set; }

    public abstract Type AttachedOwnerType { get; }

    public abstract object DefaultValue { get; }

    public abstract ModelItemDictionary Dictionary { get; }

    public abstract bool IsBrowsable { get; }

    public abstract bool IsCollection { get; }

    public abstract bool IsDictionary { get; }

    public abstract bool IsReadOnly { get; }

    public abstract bool IsSet { get; }

    public abstract bool IsAttached { get; }

    public abstract ModelItem Value { get; }

    public abstract string Name { get; }

    public abstract ModelItem Parent { get; }

    public abstract Type PropertyType { get; }

    public static bool operator ==(ModelProperty first, ModelProperty second)
    {
      if (object.ReferenceEquals((object) first, (object) second))
        return true;
      if (object.ReferenceEquals((object) first, (object) null) || object.ReferenceEquals((object) second, (object) null) || first.Parent != second.Parent)
        return false;
      return first.Name.Equals(second.Name);
    }

    public static bool operator !=(ModelProperty first, ModelProperty second)
    {
      if (object.ReferenceEquals((object) first, (object) second))
        return false;
      if (object.ReferenceEquals((object) first, (object) null) || object.ReferenceEquals((object) second, (object) null) || first.Parent != second.Parent)
        return true;
      return !first.Name.Equals(second.Name);
    }

    public abstract void ClearValue();

    public abstract IEnumerable<object> GetAttributes(Type attributeType);

    public virtual IEnumerable<object> GetAttributes(TypeIdentifier attributeTypeIdentifier)
    {
      Type attributeType = this.Parent.Context.Services.GetRequiredService<ModelService>().InvokeResolveType(attributeTypeIdentifier);
      if (attributeType != null)
        return this.GetAttributes(attributeType);
      return (IEnumerable<object>) new object[0];
    }

    public virtual bool IsPropertyOfType(Type type)
    {
      if (type == null)
        throw new ArgumentNullException("type");
      return type.IsAssignableFrom(this.PropertyType);
    }

    public virtual bool IsPropertyOfType(TypeIdentifier typeIdentifier)
    {
      Type type = this.Parent.Context.Services.GetRequiredService<ModelService>().InvokeResolveType(typeIdentifier);
      if (type != null)
        return this.IsPropertyOfType(type);
      return false;
    }

    public abstract ModelItem SetValue(object value);

    public override bool Equals(object obj)
    {
      if (object.ReferenceEquals(obj, (object) this))
        return true;
      ModelProperty modelProperty = obj as ModelProperty;
      if (object.ReferenceEquals((object) modelProperty, (object) null) || modelProperty.Parent != this.Parent)
        return false;
      return modelProperty.Name.Equals(this.Name);
    }

    public override int GetHashCode()
    {
      return this.Parent.GetHashCode() ^ this.Name.GetHashCode();
    }
  }
}
