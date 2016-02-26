// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.PropertyEditing.NewItemTypesAttribute
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using MS.Internal;
using MS.Internal.Properties;
using System;
using System.Collections.Generic;

namespace Microsoft.Windows.Design.PropertyEditing
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true)]
  public sealed class NewItemTypesAttribute : Attribute
  {
    private Type _factoryType;
    private Type[] _types;

    public IEnumerable<Type> Types
    {
      get
      {
        return (IEnumerable<Type>) this._types;
      }
    }

    public Type FactoryType
    {
      get
      {
        return this._factoryType;
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException("value");
        if (!typeof (NewItemFactory).IsAssignableFrom(value))
          throw new ArgumentException(Resources.Error_InvalidFactoryType);
        this._factoryType = value;
      }
    }

    public override object TypeId
    {
      get
      {
        object[] objArray = new object[this._types.Length + 2];
        for (int index = 0; index < this._types.Length; ++index)
          objArray[index + 2] = (object) this._types[index];
        objArray[0] = (object) typeof (NewItemTypesAttribute);
        objArray[1] = (object) this._factoryType;
        return (object) new EqualityArray(objArray);
      }
    }

    public NewItemTypesAttribute(Type type)
    {
      if (type == null)
        throw new ArgumentNullException("type");
      this._factoryType = typeof (NewItemFactory);
      this._types = new Type[1]
      {
        type
      };
    }

    public NewItemTypesAttribute(params Type[] types)
    {
      if (types == null || types.Length < 1)
        throw new ArgumentNullException("types");
      this._factoryType = typeof (NewItemFactory);
      this._types = types;
    }
  }
}
