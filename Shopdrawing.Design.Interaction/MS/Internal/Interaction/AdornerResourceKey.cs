// Decompiled with JetBrains decompiler
// Type: MS.Internal.Interaction.AdornerResourceKey
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Markup;

namespace MS.Internal.Interaction
{
  [TypeConverter(typeof (AdornerResourceKey.AdornerResourceKeyConverter))]
  internal sealed class AdornerResourceKey : ResourceKey, ISerializable
  {
    private Type _type;
    private string _member;

    public override Assembly Assembly
    {
      get
      {
        return typeof (AdornerResourceKey).Assembly;
      }
    }

    private AdornerResourceKey(SerializationInfo info, StreamingContext cxt)
    {
      this._type = (Type) info.GetValue("Type", typeof (Type));
      this._member = info.GetString("Member");
    }

    internal AdornerResourceKey(Type type, string member)
    {
      this._type = type;
      this._member = member;
    }

    public override string ToString()
    {
      return this._type.FullName + "." + this._member;
    }

    void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
    {
      info.AddValue("Type", (object) this._type);
      info.AddValue("Member", (object) this._member);
    }

    private class AdornerResourceKeyConverter : TypeConverter
    {
      public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
      {
        if (destinationType == typeof (MarkupExtension) && context is IValueSerializerContext)
          return true;
        return base.CanConvertTo(context, destinationType);
      }

      public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
      {
        if (destinationType == typeof (MarkupExtension))
        {
          AdornerResourceKey adornerResourceKey = value as AdornerResourceKey;
          IValueSerializerContext context1 = context as IValueSerializerContext;
          if (adornerResourceKey != null && context1 != null)
          {
            ValueSerializer valueSerializerFor = context1.GetValueSerializerFor(typeof (Type));
            if (valueSerializerFor != null)
              return (object) new StaticExtension(valueSerializerFor.ConvertToString((object) adornerResourceKey._type, context1) + "." + adornerResourceKey._member);
          }
        }
        return base.ConvertTo(context, culture, value, destinationType);
      }
    }
  }
}
