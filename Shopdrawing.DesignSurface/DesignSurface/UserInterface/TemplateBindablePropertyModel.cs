// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.TemplateBindablePropertyModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using System;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  public class TemplateBindablePropertyModel
  {
    public static readonly IPropertyId PropertyProperty = (IPropertyId) PlatformTypes.TemplateBinding.GetMember(MemberType.LocalProperty, "Property", MemberAccessTypes.Public);
    private DependencyPropertyReferenceStep referenceStep;

    public DependencyPropertyReferenceStep ReferenceStep
    {
      get
      {
        return this.referenceStep;
      }
    }

    public string PropertyName
    {
      get
      {
        if (this.referenceStep.IsAttachable)
          return this.referenceStep.ToString();
        return this.referenceStep.Name;
      }
    }

    public Type PropertyType
    {
      get
      {
        return PlatformTypeHelper.GetPropertyType((IProperty) this.referenceStep);
      }
    }

    public TemplateBindablePropertyModel(DependencyPropertyReferenceStep referenceStep)
    {
      if (referenceStep == null)
        throw new ArgumentNullException("referenceStep");
      this.referenceStep = referenceStep;
    }

    public override string ToString()
    {
      return this.PropertyName;
    }
  }
}
