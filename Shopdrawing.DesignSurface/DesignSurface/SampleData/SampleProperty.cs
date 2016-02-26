// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SampleData.SampleProperty
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Microsoft.Expression.DesignSurface.SampleData
{
  public class SampleProperty : ReferenceStep, IProperty, IMember, IPropertyId, IMemberId
  {
    public static readonly string FormatAttribute = "Format";
    public static readonly string FormatParametersAttribute = "FormatParameters";
    private static int seedSortValue = 1610612733;
    private string propertyName;
    private int sortValue;
    private PropertyInfo propertyInfo;

    public SampleCompositeType DeclaringSampleType { get; private set; }

    public SampleType PropertySampleType { get; private set; }

    public override string Name
    {
      get
      {
        return this.propertyName;
      }
    }

    public SampleDataSet DeclaringDataSet
    {
      get
      {
        return this.DeclaringSampleType.DeclaringDataSet;
      }
    }

    IType IProperty.PropertyType
    {
      get
      {
        return !this.PropertySampleType.IsBasicType ? (IType) this.PropertySampleType : this.DeclaringSampleType.PlatformMetadata.ResolveType(this.PropertySampleType.TypeId);
      }
    }

    int IPropertyId.SortValue
    {
      get
      {
        return this.sortValue;
      }
    }

    public string Format { get; set; }

    public string FormatParameters { get; set; }

    public bool IsCollection
    {
      get
      {
        return this.PropertySampleType.IsCollection;
      }
    }

    public bool IsBasicType
    {
      get
      {
        return this.PropertySampleType.IsBasicType;
      }
    }

    public override Type TargetType
    {
      get
      {
        this.EnsurePropertyInfo();
        return this.propertyInfo.PropertyType;
      }
    }

    public override AttributeCollection Attributes
    {
      get
      {
        return (AttributeCollection) null;
      }
    }

    public override bool IsResolvable
    {
      get
      {
        return true;
      }
    }

    public override MemberType MemberType
    {
      get
      {
        return MemberType.LocalProperty;
      }
    }

    public override MemberAccessType ReadAccess
    {
      get
      {
        return MemberAccessType.Public;
      }
    }

    public override MemberAccessType WriteAccess
    {
      get
      {
        return MemberAccessType.Public;
      }
    }

    public override bool IsProxy
    {
      get
      {
        return false;
      }
    }

    public override bool IsAttachable
    {
      get
      {
        return false;
      }
    }

    public override bool ShouldSerialize
    {
      get
      {
        return true;
      }
    }

    public override ITypeId MemberTypeId
    {
      get
      {
        return (ITypeId) this.DeclaringSampleType;
      }
    }

    public override MemberAccessType Access
    {
      get
      {
        return MemberAccessType.Public;
      }
    }

    public SampleProperty(string name, SampleType propertySampleType, SampleCompositeType declaringSampleType)
      : base((IType) declaringSampleType, name, declaringSampleType.DeclaringDataSet.ResolveSampleType(propertySampleType), SampleProperty.seedSortValue++)
    {
      this.propertyName = name;
      this.PropertySampleType = propertySampleType;
      this.DeclaringSampleType = declaringSampleType;
      this.sortValue = SampleProperty.seedSortValue;
      this.DeclaringDataSet.ThrowIfRecursiveRootType(this.PropertySampleType);
    }

    public string GetUniqueNameForRename(string name)
    {
      string str = name;
      if (this.Name != name)
        str = this.DeclaringSampleType.GetUniquePropertyName(name, this.Name);
      return str;
    }

    public void Rename(string newName)
    {
      if (this.Name == newName)
        return;
      if (string.IsNullOrEmpty(newName))
        throw new InvalidOperationException(ExceptionStringTable.EmptyPropertyName);
      if (this.DeclaringSampleType.GetSampleProperty(newName) != null)
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ExceptionStringTable.NameIsAlreadyInUse, new object[1]
        {
          (object) newName
        }));
      string name = this.Name;
      this.propertyName = newName;
      this.DeclaringDataSet.OnPropertyRenamed(this, name);
    }

    internal void TransferSortValue(SampleProperty fromProperty)
    {
      this.sortValue = fromProperty.sortValue;
    }

    public void ChangeType(SampleType newType)
    {
      if (this.PropertySampleType == newType)
        return;
      this.ChangeTypeAndFormat(newType, (string) null, (string) null);
    }

    public void ChangeFormat(string newFormat, string newFormatParameters)
    {
      this.ChangeTypeAndFormat(this.PropertySampleType, newFormat, newFormatParameters);
    }

    public void ChangeTypeAndFormat(SampleType newType, string newFormat, string newFormatParameters)
    {
      newFormat = SampleDataFormatHelper.NormalizeFormat(this.PropertySampleType as SampleBasicType, newFormat, true);
      newFormatParameters = SampleDataFormatHelper.NormalizeFormatParameters(this.PropertySampleType as SampleBasicType, newFormatParameters, true);
      if (this.PropertySampleType == newType && this.Format == newFormat && this.FormatParameters == newFormatParameters)
        return;
      if (this.PropertySampleType != newType)
        this.DeclaringDataSet.ThrowIfRecursiveRootType(newType);
      SampleType propertySampleType = this.PropertySampleType;
      string format = this.Format;
      string formatParameters = this.FormatParameters;
      this.PropertySampleType = newType;
      this.Format = newFormat;
      this.FormatParameters = newFormatParameters;
      this.DeclaringDataSet.OnPropertyTypeOrFormatChanged(this, propertySampleType, format, formatParameters);
    }

    public void Delete()
    {
      this.DeclaringSampleType.DeleteProperty(this);
    }

    internal void ResetPropertyInfo()
    {
      this.propertyInfo = (PropertyInfo) null;
    }

    private void EnsurePropertyInfo()
    {
      if (!(this.propertyInfo == (PropertyInfo) null))
        return;
      this.propertyInfo = this.DeclaringSampleType.DesignTimeType.GetProperty(this.Name);
    }

    public override object GetValue(object objToInspect)
    {
      this.EnsurePropertyInfo();
      return this.propertyInfo.GetValue(objToInspect, (object[]) null);
    }

    public override object SetValue(object target, object valueToSet)
    {
      this.EnsurePropertyInfo();
      this.propertyInfo.SetValue(target, valueToSet, (object[]) null);
      return (object) null;
    }

    public override void ClearValue(object target)
    {
    }

    public override bool IsSet(object objToInspect)
    {
      return true;
    }

    public override bool IsAnimated(object target)
    {
      return false;
    }

    public override bool ShouldSerializeMethod(object objToInspect)
    {
      return true;
    }

    public override object[] GetCustomAttributes(Type attributeType, bool inherit)
    {
      return (object[]) null;
    }

    public override bool HasDefaultValue(Type targetType)
    {
      return false;
    }

    public override object GetDefaultValue(Type targetType)
    {
      return (object) null;
    }

    public override string ToString()
    {
      if (string.IsNullOrEmpty(this.Name))
        return "Uninitialized " + base.ToString();
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", new object[2]
      {
        (object) this.DeclaringSampleType.ToString(),
        (object) this.Name
      });
      if (!string.IsNullOrEmpty(this.Format))
      {
        if (!string.IsNullOrEmpty(this.FormatParameters))
          stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, " [{0}({1})]", new object[2]
          {
            (object) this.Format,
            (object) this.FormatParameters
          });
        else
          stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, " [{0}]", new object[1]
          {
            (object) this.Format
          });
      }
      return stringBuilder.ToString();
    }
  }
}
