// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.FallbackPropertyInformation
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.ComponentModel;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class FallbackPropertyInformation : IPropertyInformation
  {
    private string propertyName;
    private IType propertyType;

    public TypeConverter TypeConverter
    {
      get
      {
        return (TypeConverter) new StringConverter();
      }
    }

    public AttributeCollection Attributes
    {
      get
      {
        return new AttributeCollection(new Attribute[1]
        {
          (Attribute) new TypeConverterAttribute(typeof (StringConverter))
        });
      }
    }

    public IType PropertyType
    {
      get
      {
        return this.propertyType;
      }
    }

    public object DefaultValue
    {
      get
      {
        return (object) null;
      }
    }

    public string Name
    {
      get
      {
        return this.propertyName;
      }
    }

    public string DetailedDescription
    {
      get
      {
        return (string) null;
      }
    }

    public string GroupBy
    {
      get
      {
        return (string) null;
      }
    }

    public FallbackPropertyInformation(string propertyName)
    {
      this.propertyName = propertyName;
    }

    public FallbackPropertyInformation(string propertyName, IType propertyType)
    {
      this.propertyName = propertyName;
      this.propertyType = propertyType;
    }

    public override bool Equals(object obj)
    {
      FallbackPropertyInformation propertyInformation = obj as FallbackPropertyInformation;
      if (propertyInformation != null)
        return this.propertyName.Equals(propertyInformation.propertyName);
      return base.Equals(obj);
    }

    public override int GetHashCode()
    {
      return this.propertyName.GetHashCode();
    }

    public override string ToString()
    {
      return this.propertyName;
    }
  }
}
