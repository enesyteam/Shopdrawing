// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.DisplayPropertyAttribute
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using System;

namespace Microsoft.Windows.Design
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
  public sealed class DisplayPropertyAttribute : Attribute
  {
    private string _displayPropertyName;
    private Type _valueConverterType;

    public string DisplayPropertyName
    {
      get
      {
        return this._displayPropertyName;
      }
    }

    public Type ValueConverterType
    {
      get
      {
        return this._valueConverterType;
      }
    }

    public override object TypeId
    {
      get
      {
        return (object) typeof (DisplayPropertyAttribute);
      }
    }

    public DisplayPropertyAttribute(string displayPropertyName)
    {
      this._displayPropertyName = displayPropertyName;
    }

    public DisplayPropertyAttribute(string displayPropertyName, Type valueConverterType)
    {
      this._displayPropertyName = displayPropertyName;
      this._valueConverterType = valueConverterType;
    }

    public override bool Equals(object obj)
    {
      if (object.ReferenceEquals((object) this, obj))
        return true;
      DisplayPropertyAttribute propertyAttribute = obj as DisplayPropertyAttribute;
      if (propertyAttribute != null && object.Equals((object) this.DisplayPropertyName, (object) propertyAttribute.DisplayPropertyName))
        return object.Equals((object) this.ValueConverterType, (object) propertyAttribute.ValueConverterType);
      return false;
    }

    public override int GetHashCode()
    {
      int num = 0;
      if (this.DisplayPropertyName != null)
        num ^= this.DisplayPropertyName.GetHashCode();
      if (this.ValueConverterType != null)
        num ^= this.ValueConverterType.GetHashCode();
      return num;
    }
  }
}
