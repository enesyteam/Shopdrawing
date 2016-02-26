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
        return this.ParentProperty;
      }
    }

    public override PropertyValueSource Source
    {
      get
      {
        throw new InvalidOperationException(ExceptionStringTable.PropertyReferencePropertyValuesDoNotHaveAccessToPropertyValueSource);
      }
    }

    public override bool IsDefaultValue
    {
      get
      {
        return this.property.IsDefaultValue;
      }
    }

    public override bool IsMixedValue
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

    public override bool CanConvertFromString
    {
      get
      {
        if (this.property.Converter != null && this.property.Converter.CanConvertFrom(typeof (string)))
          return !this.property.IsReadOnly;
        return false;
      }
    }

    public override bool HasSubProperties
    {
      get
      {
        return false;
      }
    }

    public override PropertyEntryCollection SubProperties
    {
      get
      {
        throw new InvalidOperationException(ExceptionStringTable.PropertyReferencePropertyValuesDoNotHaveAccessToSubProperties);
      }
    }

    public override bool IsCollection
    {
      get
      {
        return false;
      }
    }

    public override PropertyValueCollection Collection
    {
      get
      {
        throw new InvalidOperationException(ExceptionStringTable.PropertyReferencePropertyValuesDoNotHaveAccessToCollections);
      }
    }

    public PropertyReferencePropertyValue(PropertyReferenceProperty property)
      : base((PropertyEntry) property)
    {
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

    protected override void ValidateValue(object valueToValidate)
    {
      if (this.property.Validator == null)
        return;
      this.property.Validator.Validate(valueToValidate);
    }

    protected override object ConvertStringToValue(string stringToConvert)
    {
      if (typeof (string).Equals(this.property.PropertyType))
        return (object) stringToConvert;
      if (stringToConvert.Length == 0)
        return (object) null;
      if (this.property.Converter.CanConvertFrom(typeof (string)))
        return this.property.Converter.ConvertFromString((ITypeDescriptorContext) null, this.GetSerializationCulture(), stringToConvert);
      throw new InvalidOperationException(ExceptionStringTable.DoesNotSupportValueToStringConversion);
    }

    protected override string ConvertValueToString(object valueToConvert)
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

    protected override object GetValueCore()
    {
      return this.property.GetValue();
    }

    protected override void SetValueCore(object value)
    {
      for (PropertyValue parentValue = this.ParentProperty.ParentValue; parentValue != null; parentValue = parentValue.ParentProperty.ParentValue)
      {
        PropertyEntry parentProperty = parentValue.ParentProperty;
        if (parentValue.Source != DependencyPropertyValueSource.LocalValue && parentValue.Source != DependencyPropertyValueSource.InheritedValue && parentValue.Source != DependencyPropertyValueSource.DefaultValue)
          throw new InvalidOperationException(ExceptionStringTable.SettingSubpropertyOfExpression);
      }
      if (value == null && this.property.ShouldClearValueWhenSettingNull)
        this.property.ClearValue();
      else
        this.property.SetValue(value);
    }

    public override void ClearValue()
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
