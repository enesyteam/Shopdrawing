// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.PropertyReferencePropertyValue
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class PropertyReferencePropertyValue : PropertyValue
  {
    private static ITypeId[] CultureInvariantTypes = new ITypeId[19]
    {
      PlatformTypes.CornerRadius,
      PlatformTypes.Point3D,
      PlatformTypes.Point4D,
      PlatformTypes.Point3DCollection,
      PlatformTypes.Matrix3D,
      PlatformTypes.Quaternion,
      PlatformTypes.Rect3D,
      PlatformTypes.Size3D,
      PlatformTypes.Vector3D,
      PlatformTypes.Vector3DCollection,
      PlatformTypes.PointCollection,
      PlatformTypes.VectorCollection,
      PlatformTypes.Point,
      PlatformTypes.Rect,
      PlatformTypes.Size,
      PlatformTypes.Thickness,
      PlatformTypes.Vector,
      PlatformTypes.Matrix,
      PlatformTypes.DoubleCollection
    };
    private PropertyReferenceProperty property;

    public PropertyEntry OwnerProperty
    {
      get
      {
        return this.get_ParentProperty();
      }
    }

    public virtual PropertyValueSource Source
    {
      get
      {
        throw new InvalidOperationException(ExceptionStringTable.PropertyReferencePropertyValuesDoNotHaveAccessToPropertyValueSource);
      }
    }

    public virtual bool IsDefaultValue
    {
      get
      {
        return this.property.IsDefaultValue;
      }
    }

    public virtual bool IsMixedValue
    {
      get
      {
        return this.property.IsMixedValue;
      }
    }

    public TypeConverter Converter
    {
      get
      {
        return this.property.Converter;
      }
    }

    public virtual bool CanConvertFromString
    {
      get
      {
        if (this.property.Converter != null && this.property.Converter.CanConvertFrom(typeof (string)))
          return !((PropertyEntry) this.property).get_IsReadOnly();
        return false;
      }
    }

    public virtual bool HasSubProperties
    {
      get
      {
        return false;
      }
    }

    public virtual PropertyEntryCollection SubProperties
    {
      get
      {
        throw new InvalidOperationException(ExceptionStringTable.PropertyReferencePropertyValuesDoNotHaveAccessToSubProperties);
      }
    }

    public virtual bool IsCollection
    {
      get
      {
        return false;
      }
    }

    public virtual PropertyValueCollection Collection
    {
      get
      {
        throw new InvalidOperationException(ExceptionStringTable.PropertyReferencePropertyValuesDoNotHaveAccessToCollections);
      }
    }

    public PropertyReferencePropertyValue(PropertyReferenceProperty property)
    {
      this.\u002Ector((PropertyEntry) property);
      this.property = property;
      this.property.PropertyReferenceChanged += new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnPropertyReferenceChanged);
    }

    private void OnPropertyReferenceChanged(object sender, PropertyReferenceChangedEventArgs e)
    {
      this.NotifyPropertyReferenceChanged(e.PropertyReference);
      if (e.PropertyReference.ReferenceSteps != null && e.PropertyReference.Count - this.property.Reference.Count > 0)
        this.NotifySubPropertyChanged();
      else
        this.NotifyRootValueChanged();
    }

    protected virtual void NotifyPropertyReferenceChanged(PropertyReference propertyReference)
    {
    }

    protected virtual void ValidateValue(object valueToValidate)
    {
      if (this.property.Validator == null)
        return;
      this.property.Validator.Validate(valueToValidate);
    }

    protected virtual object ConvertStringToValue(string stringToConvert)
    {
      if (typeof (string).Equals(((PropertyEntry) this.property).get_PropertyType()))
        return (object) stringToConvert;
      if (stringToConvert.Length == 0)
        return null;
      if (this.property.Converter.CanConvertFrom(typeof (string)))
        return this.property.Converter.ConvertFromString((ITypeDescriptorContext) null, this.GetSerializationCulture(), stringToConvert);
      throw new InvalidOperationException(ExceptionStringTable.DoesNotSupportValueToStringConversion);
    }

    protected virtual string ConvertValueToString(object valueToConvert)
    {
      string str1 = string.Empty;
      if (valueToConvert == null)
        return str1;
      string str2;
      if ((str2 = valueToConvert as string) != null)
        return str2;
      TypeConverter converter = this.property.Converter;
      string str3 = !converter.CanConvertTo(typeof (string)) ? valueToConvert.ToString() : converter.ConvertToString((ITypeDescriptorContext) null, this.GetSerializationCulture(), valueToConvert);
      if (string.IsNullOrEmpty(str3) && valueToConvert is IEnumerable)
        str3 = StringTable.CollectionValue;
      return str3;
    }

    protected virtual object GetValueCore()
    {
      return this.property.GetValue();
    }

    protected virtual void SetValueCore(object value)
    {
      for (PropertyValue parentValue = this.get_ParentProperty().get_ParentValue(); parentValue != null; parentValue = parentValue.get_ParentProperty().get_ParentValue())
      {
        parentValue.get_ParentProperty();
        if (parentValue.get_Source() != DependencyPropertyValueSource.get_LocalValue() && parentValue.get_Source() != DependencyPropertyValueSource.get_InheritedValue() && parentValue.get_Source() != DependencyPropertyValueSource.get_DefaultValue())
          throw new InvalidOperationException(ExceptionStringTable.SettingSubpropertyOfExpression);
      }
      if (value == null && this.property.ShouldClearValueWhenSettingNull)
        this.property.ClearValue();
      else
        this.property.SetValue(value);
    }

    public virtual void ClearValue()
    {
      this.property.ClearValue();
    }

    public virtual void Unhook()
    {
    }

    protected virtual CultureInfo GetSerializationCulture()
    {
      CultureInfo cultureInfo = CultureInfo.CurrentCulture;
      if (this.ShouldForceInvariantCultureForType((ITypeId) this.property.PropertyTypeId) || PlatformTypes.Geometry.IsAssignableFrom((ITypeId) this.property.PropertyTypeId))
        cultureInfo = CultureInfo.InvariantCulture;
      return cultureInfo;
    }

    private bool ShouldForceInvariantCultureForType(ITypeId typeId)
    {
      foreach (ITypeId typeId1 in PropertyReferencePropertyValue.CultureInvariantTypes)
      {
        if (typeId1.IsAssignableFrom(typeId))
          return true;
      }
      return false;
    }
  }
}
