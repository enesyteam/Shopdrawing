// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.CodeAid.CodeAidPropertyInfo
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Code;
using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections;
using System.ComponentModel;

namespace Microsoft.Expression.DesignSurface.CodeAid
{
  internal class CodeAidPropertyInfo : CodeAidInfoBase, ICodeAidPropertyInfo, ICodeAidMemberInfo
  {
    private IProperty property;
    private string cachedPropertyDescription;

    public ICodeAidTypeInfo PropertyType
    {
      get
      {
        return (ICodeAidTypeInfo) new CodeAidTypeInfo(this.owner, this.property.PropertyType);
      }
    }

    public override string DescriptionText
    {
      get
      {
        if (this.cachedPropertyDescription == null)
        {
          AttributeCollection attributes = this.property.Attributes;
          string result = (string) null;
          if (PlatformNeutralAttributeHelper.TryGetAttributeValue<string>((IEnumerable) attributes, PlatformTypes.DescriptionAttribute, "Description", out result))
            this.cachedPropertyDescription = result;
        }
        if (this.cachedPropertyDescription == null)
          this.cachedPropertyDescription = (string) null;
        return this.cachedPropertyDescription;
      }
    }

    public override string DescriptionSubtitle
    {
      get
      {
        if (this.property.PropertyType.RuntimeType != (Type) null)
          return CodeAidTypeInfo.FormatTypeName(this.property.PropertyType.RuntimeType);
        return string.Empty;
      }
    }

    public CodeAidPropertyInfo(CodeAidProvider owner, IProperty property)
      : base(owner, property.Name)
    {
      this.property = property;
    }
  }
}
