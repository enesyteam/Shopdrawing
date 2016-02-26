// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.ReferenceStepPropertyInformation
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class ReferenceStepPropertyInformation : IComparable, IPropertyInformation
  {
    private ReferenceStep referenceStep;

    public string Name
    {
      get
      {
        return this.referenceStep.Name;
      }
    }

    public string DetailedDescription
    {
      get
      {
        string description = this.referenceStep.Description;
        if (string.IsNullOrEmpty(description))
          return (string) null;
        return description;
      }
    }

    public string GroupBy
    {
      get
      {
        string categoryName = this.referenceStep.Category;
        CategoryLocalizationHelper.CategoryName canonicalCategoryName = CategoryLocalizationHelper.GetCanonicalCategoryName(categoryName);
        if (canonicalCategoryName != CategoryLocalizationHelper.CategoryName.Unknown)
          categoryName = CategoryLocalizationHelper.GetLocalizedCategoryName(canonicalCategoryName);
        return categoryName;
      }
    }

    public TypeConverter TypeConverter
    {
      get
      {
        return this.referenceStep.TypeConverter;
      }
    }

    public AttributeCollection Attributes
    {
      get
      {
        return this.referenceStep.Attributes;
      }
    }

    public IType PropertyType
    {
      get
      {
        return this.referenceStep.PropertyType;
      }
    }

    public object DefaultValue
    {
      get
      {
        return this.referenceStep.GetDefaultValue(this.referenceStep.TargetType);
      }
    }

    public ReferenceStepPropertyInformation(ReferenceStep referenceStep)
    {
      this.referenceStep = referenceStep;
    }

    public override bool Equals(object obj)
    {
      ReferenceStepPropertyInformation propertyInformation = obj as ReferenceStepPropertyInformation;
      if (propertyInformation != null)
        return this.referenceStep.Equals((object) propertyInformation.referenceStep);
      return base.Equals(obj);
    }

    public override int GetHashCode()
    {
      return this.referenceStep.GetHashCode();
    }

    public override string ToString()
    {
      return this.referenceStep.Name;
    }

    public int CompareTo(object obj)
    {
      ReferenceStepPropertyInformation propertyInformation = obj as ReferenceStepPropertyInformation;
      if (propertyInformation == null)
        return 0;
      if (this.GroupBy != propertyInformation.GroupBy)
        return string.Compare(this.GroupBy, propertyInformation.GroupBy, StringComparison.CurrentCulture);
      return string.Compare(this.referenceStep.Name, propertyInformation.referenceStep.Name, StringComparison.CurrentCulture);
    }

    public static IEnumerable<ReferenceStepPropertyInformation> GetPropertiesForType(IType type)
    {
      List<IProperty> properties = new List<IProperty>();
      for (; type != null; type = type.BaseType)
        properties.AddRange(type.GetProperties(MemberAccessTypes.Public));
      foreach (IProperty property in properties)
      {
        ReferenceStep referenceStep = property as ReferenceStep;
        if (referenceStep != null && referenceStep.WriteAccess == MemberAccessType.Public)
          yield return new ReferenceStepPropertyInformation(referenceStep);
      }
    }
  }
}
