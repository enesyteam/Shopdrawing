// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInformation
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Properties;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  public class PropertyInformation : TriggerSourceInformation
  {
    private DependencyPropertyReferenceStep referenceStep;

    public DependencyPropertyReferenceStep PropertyReferenceStep
    {
      get
      {
        return this.referenceStep;
      }
      set
      {
        this.referenceStep = value;
      }
    }

    public DependencyProperty DependencyProperty
    {
      get
      {
        return (DependencyProperty) this.referenceStep.DependencyProperty;
      }
    }

    public override string DisplayName
    {
      get
      {
        return this.PropertyReferenceStep.Name;
      }
    }

    public override string DetailedDescription
    {
      get
      {
        return this.PropertyReferenceStep.Description;
      }
    }

    public override string GroupBy
    {
      get
      {
        string categoryName = this.PropertyReferenceStep.Category;
        CategoryLocalizationHelper.CategoryName canonicalCategoryName = CategoryLocalizationHelper.GetCanonicalCategoryName(categoryName);
        if (canonicalCategoryName != CategoryLocalizationHelper.CategoryName.Unknown)
          categoryName = CategoryLocalizationHelper.GetLocalizedCategoryName(canonicalCategoryName);
        return categoryName;
      }
    }

    public PropertyInformation(DependencyPropertyReferenceStep referenceStep)
    {
      this.referenceStep = referenceStep;
    }

    public override bool Equals(object obj)
    {
      PropertyInformation propertyInformation = obj as PropertyInformation;
      if ((TriggerSourceInformation) propertyInformation != (TriggerSourceInformation) null)
        return this.DependencyProperty == propertyInformation.DependencyProperty;
      return base.Equals(obj);
    }

    public override int GetHashCode()
    {
      return this.DependencyProperty.GetHashCode();
    }

    public override string ToString()
    {
      return this.referenceStep.Name;
    }

    public override int CompareTo(object obj)
    {
      PropertyInformation propertyInformation = obj as PropertyInformation;
      if (!((TriggerSourceInformation) propertyInformation != (TriggerSourceInformation) null))
        return 0;
      if (this.GroupBy != propertyInformation.GroupBy)
        return this.GroupBy.CompareTo(propertyInformation.GroupBy);
      return this.PropertyReferenceStep.Name.CompareTo(propertyInformation.PropertyReferenceStep.Name);
    }

    public static IEnumerable<PropertyInformation> GetPropertiesForType(Type objectType, ITypeResolver typeResolver)
    {
      IType type = typeResolver.GetType(objectType);
      if (type != null)
      {
        foreach (IProperty property in ITypeExtensions.GetProperties(type, MemberAccessTypes.PublicOrInternal, true))
        {
          DependencyPropertyReferenceStep dependencyPropertyReferenceStep = property as DependencyPropertyReferenceStep;
          if (dependencyPropertyReferenceStep != null)
            yield return new PropertyInformation(dependencyPropertyReferenceStep);
        }
      }
    }

    public static PropertyInformation FromDependencyProperty(DependencyProperty property, Type objectType, ITypeResolver typeResolver)
    {
      if (property != null)
      {
        DependencyPropertyReferenceStep referenceStep = DependencyPropertyReferenceStep.GetReferenceStep(typeResolver, objectType, property);
        if (referenceStep != null)
          return new PropertyInformation(referenceStep);
      }
      return (PropertyInformation) null;
    }
  }
}
