// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.CodeAid.CodeAidTypeInfo
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Code;
using Microsoft.Expression.DesignModel.Code;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.Expression.DesignSurface.CodeAid
{
  internal class CodeAidTypeInfo : CodeAidInfoBase, ICodeAidTypeInfo, ICodeAidMemberInfo
  {
    private IType type;
    private string cachedDescription;

    public IEnumerable<ICodeAidMemberInfo> Properties
    {
      get
      {
        for (IType currentType = this.type; currentType != null; currentType = currentType.BaseType)
        {
          foreach (IProperty property in currentType.GetProperties(MemberAccessTypes.Public))
          {
            if ((TypeHelper.IsPropertyWritable((ITypeResolver) this.owner.ProjectContext, property, false) || property.ReadAccess == MemberAccessType.Public && property.PropertyType.ItemType != null) && !property.IsAttachable)
              yield return (ICodeAidMemberInfo) new CodeAidPropertyInfo(this.owner, property);
          }
        }
      }
    }

    private IAttachedPropertiesProvider AttachedPropertiesProvider
    {
      get
      {
        return this.owner.ProjectContext.GetService(typeof (IAttachedPropertiesProvider)) as IAttachedPropertiesProvider;
      }
    }

    public IEnumerable<ICodeAidMemberInfo> AllAttachedProperties
    {
      get
      {
        IAttachedPropertiesProvider propertiesProvider = this.AttachedPropertiesProvider;
        if (propertiesProvider == null)
          throw new NotSupportedException();
        using (IAttachedPropertiesAccessToken propertiesAccessToken = propertiesProvider.AttachedProperties.Access())
          return Enumerable.Cast<ICodeAidMemberInfo>((IEnumerable) Enumerable.Select<IAttachedPropertyMetadata, CodeAidAttachedPropertyInfo>((IEnumerable<IAttachedPropertyMetadata>) propertiesAccessToken.AttachedPropertiesForType(this.type), (Func<IAttachedPropertyMetadata, CodeAidAttachedPropertyInfo>) (property => new CodeAidAttachedPropertyInfo(this.owner, property.Name, property.PropertyType))));
      }
    }

    public IEnumerable<ICodeAidMemberInfo> Events
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public IEnumerable<ICodeAidMemberInfo> EnumerationValues
    {
      get
      {
        TypeConverter typeConverter = MetadataStore.GetTypeConverter(this.type.RuntimeType);
        if (typeConverter != null && typeConverter.GetStandardValuesSupported())
        {
          foreach (object obj in (IEnumerable) typeConverter.GetStandardValues())
            yield return (ICodeAidMemberInfo) new CodeAidValueInfo(this.owner, obj == null ? (string) null : obj.ToString());
        }
      }
    }

    public ICodeAidTypeInfo DefaultContentPropertyType
    {
      get
      {
        IProperty defaultContentProperty = this.type.Metadata.DefaultContentProperty;
        if (defaultContentProperty == null)
          return (ICodeAidTypeInfo) null;
        return (ICodeAidTypeInfo) new CodeAidTypeInfo(this.owner, defaultContentProperty.PropertyType);
      }
    }

    public ICodeAidTypeInfo CollectionItemType
    {
      get
      {
        if (this.type.ItemType == null)
          return (ICodeAidTypeInfo) null;
        return (ICodeAidTypeInfo) new CodeAidTypeInfo(this.owner, this.type.ItemType);
      }
    }

    public override string DescriptionText
    {
      get
      {
        if (this.cachedDescription == null)
        {
          string result;
          if (PlatformNeutralAttributeHelper.TryGetAttributeValue<string>((IEnumerable) TypeUtilities.GetAttributes(this.type.RuntimeType), PlatformTypes.DescriptionAttribute, "Description", out result))
            this.cachedDescription = result;
          if (this.cachedDescription == null)
            this.cachedDescription = (string) null;
        }
        return this.cachedDescription;
      }
    }

    public bool IsDictionaryType
    {
      get
      {
        if (this.type == null)
          return false;
        if (!PlatformTypes.IDictionary.IsAssignableFrom((ITypeId) this.type))
          return PlatformTypes.ResourceDictionary.IsAssignableFrom((ITypeId) this.type);
        return true;
      }
    }

    public CodeAidTypeInfo(CodeAidProvider owner, IType type)
      : base(owner, type.Name)
    {
      this.type = type;
    }

    public IEnumerable<ICodeAidMemberInfo> FilteredAttachedProperties(ICodeAidTypeInfo parentTypeInfo, IEnumerable<ICodeAidTypeInfo> ancestorTypeInfos)
    {
      if (parentTypeInfo == null)
        return Enumerable.Empty<ICodeAidMemberInfo>();
      IAttachedPropertiesProvider propertiesProvider = this.AttachedPropertiesProvider;
      if (propertiesProvider == null)
        throw new NotSupportedException();
      using (IAttachedPropertiesAccessToken propertiesAccessToken = propertiesProvider.AttachedProperties.Access())
      {
        CodeAidTypeInfo realParentTypeInfo = parentTypeInfo as CodeAidTypeInfo;
        IEnumerable<Type> realAncestorTypeInfos = ancestorTypeInfos != null ? Enumerable.Select<CodeAidTypeInfo, Type>(Enumerable.Cast<CodeAidTypeInfo>((IEnumerable) ancestorTypeInfos), (Func<CodeAidTypeInfo, Type>) (ti => ti.type.RuntimeType)) : (IEnumerable<Type>) null;
        return Enumerable.Cast<ICodeAidMemberInfo>((IEnumerable) Enumerable.Select<IAttachedPropertyMetadata, CodeAidAttachedPropertyInfo>(Enumerable.Where<IAttachedPropertyMetadata>((IEnumerable<IAttachedPropertyMetadata>) propertiesAccessToken.AttachedPropertiesForType(this.type), (Func<IAttachedPropertyMetadata, bool>) (property => property.IsBrowsable(realParentTypeInfo.type.RuntimeType, realAncestorTypeInfos))), (Func<IAttachedPropertyMetadata, CodeAidAttachedPropertyInfo>) (property => new CodeAidAttachedPropertyInfo(this.owner, property.Name, property.PropertyType))));
      }
    }

    public bool IsAssignableFrom(ICodeAidTypeInfo type)
    {
      return this.type.IsAssignableFrom((ITypeId) ((CodeAidTypeInfo) type).type);
    }

    public static string FormatTypeName(Type type)
    {
      if (type == (Type) null)
        return string.Empty;
      Type nullableType = PlatformTypeHelper.GetNullableType(type);
      string shortName = TypeNameFormatter.GetShortName(TypeNameFormatter.FormatTypeForDefaultLanguage(nullableType, true));
      if (!(nullableType != (Type) null))
        return shortName;
      return shortName + "?";
    }
  }
}
