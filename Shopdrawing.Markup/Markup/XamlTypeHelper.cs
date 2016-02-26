// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.XamlTypeHelper
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.Text;

namespace Microsoft.Expression.DesignModel.Markup
{
  internal static class XamlTypeHelper
  {
    internal static IType GetTypeId(XamlParserContext parserContext, ITextLocation lineInformation, XmlNamespace xmlNamespace, string typeName)
    {
      return XamlTypeHelper.GetTypeId(parserContext, lineInformation, xmlNamespace, typeName, false);
    }

    internal static IType GetTypeId(XamlParserContext parserContext, ITextLocation lineInformation, XmlNamespace xmlNamespace, string typeName, bool inMarkupExtension)
    {
      if (string.IsNullOrEmpty(typeName))
      {
        parserContext.ReportError(XamlParseErrors.MissingTypeName(lineInformation));
        return (IType) null;
      }
      if (typeName.IndexOf('.') >= 0)
      {
        parserContext.ReportError(XamlParseErrors.InnerTypesNotSupported(lineInformation, typeName));
        return (IType) null;
      }
      IType typeId = XamlTypeHelper.GetTypeId(parserContext.TypeResolver, parserContext.DocumentNamespaces, xmlNamespace, typeName, true, inMarkupExtension);
      if (typeId != null && typeId.IsResolvable)
      {
        if (!parserContext.TypeResolver.InTargetAssembly(typeId) && !TypeHelper.IsSet(MemberAccessTypes.Public, typeId.Access))
        {
          parserContext.ReportError(XamlParseErrors.InaccessibleType(lineInformation, typeName));
          return (IType) null;
        }
      }
      else if (xmlNamespace == XmlNamespace.AvalonXmlNamespace || xmlNamespace == XmlNamespace.XamlXmlNamespace)
        parserContext.ReportError(XamlParseErrors.UnrecognizedPlatformTypeName(lineInformation, !parserContext.TypeResolver.IsCapabilitySet(PlatformCapability.IsWpf), typeName));
      else
        parserContext.ReportError(XamlParseErrors.UnrecognizedTypeName(lineInformation, xmlNamespace, typeName));
      return typeId;
    }

    internal static IType GetTypeId(ITypeResolver typeResolver, ClrNamespaceUriParseCache documentNamespaces, XmlNamespace xmlNamespace, string typeName, bool instantiateUnrecognizedTypes, bool inMarkupExtension)
    {
      IType designTimeType = typeResolver.PlatformMetadata.GetDesignTimeType(typeResolver, (IXmlNamespace) xmlNamespace, typeName);
      if (designTimeType != null)
        return designTimeType;
      AssemblyNamespace assemblyNamespace;
      documentNamespaces.GetNamespace((IXmlNamespace) xmlNamespace, out assemblyNamespace);
      IType type;
      if (assemblyNamespace != null)
      {
        type = XamlTypeHelper.ResolveType(typeResolver, (IXmlNamespaceTypeResolver) documentNamespaces, xmlNamespace, typeName, inMarkupExtension);
        if (type == null && instantiateUnrecognizedTypes)
        {
          IAssembly assembly = assemblyNamespace.Assembly;
          string clrNamespace = assemblyNamespace.ClrNamespace;
          type = typeResolver.GetType(assembly.Name, TypeHelper.CombineNamespaceAndTypeName(clrNamespace, typeName)) ?? typeResolver.PlatformMetadata.CreateUnknownType(typeResolver, assembly, clrNamespace, typeName);
        }
      }
      else
      {
        type = XamlTypeHelper.ResolveType(typeResolver, typeResolver.ProjectNamespaces, xmlNamespace, typeName, inMarkupExtension);
        if (type == null && instantiateUnrecognizedTypes)
          type = typeResolver.PlatformMetadata.CreateUnknownType(typeResolver, (IXmlNamespace) xmlNamespace, typeName);
      }
      return type;
    }

    internal static IType GetTypeId(XamlParserContext parserContext, ITextLocation lineInformation, IXmlNamespaceResolver xmlNamespaceResolver, string fullTypeName, bool inMarkupExtension)
    {
      XmlnsPrefix prefix;
      string typeName;
      if (XamlTypeHelper.SplitTypeName(parserContext, lineInformation, fullTypeName, out prefix, out typeName))
      {
        XmlNamespace xmlNamespace = parserContext.GetXmlNamespace(lineInformation, xmlNamespaceResolver, prefix);
        if (xmlNamespace != null)
          return XamlTypeHelper.GetTypeId(parserContext, lineInformation, xmlNamespace, typeName, inMarkupExtension);
      }
      return (IType) null;
    }

    internal static bool IsIgnorable(XamlParserContext parserContext, IXmlNamespaceResolver xmlNamespaceResolver, XmlNamespace xmlNamespace)
    {
      if (xmlNamespace == XmlNamespace.DesignTimeXmlNamespace || xmlNamespace == XmlNamespace.AnnotationsXmlNamespace || (!xmlNamespaceResolver.IsIgnorable(xmlNamespace) || parserContext.TypeResolver.ProjectNamespaces.Contains((IXmlNamespace) xmlNamespace)))
        return false;
      AssemblyNamespace assemblyNamespace;
      parserContext.DocumentNamespaces.GetNamespace((IXmlNamespace) xmlNamespace, out assemblyNamespace);
      return assemblyNamespace == null;
    }

    private static IType ResolveType(ITypeResolver typeResolver, IXmlNamespaceTypeResolver xmlNamespaceTypeResolver, XmlNamespace xmlNamespace, string typeName, bool inMarkupExtension)
    {
      return XamlTypeHelper.ResolveTypeInternal(typeResolver, xmlNamespaceTypeResolver, xmlNamespace, inMarkupExtension ? typeName + "Extension" : typeName) ?? XamlTypeHelper.ResolveTypeInternal(typeResolver, xmlNamespaceTypeResolver, xmlNamespace, inMarkupExtension ? typeName : typeName + "Extension");
    }

    private static IType ResolveTypeInternal(ITypeResolver typeResolver, IXmlNamespaceTypeResolver xmlNamespaceTypeResolver, XmlNamespace xmlNamespace, string typeName)
    {
      IType type = xmlNamespaceTypeResolver.GetType((IXmlNamespace) xmlNamespace, typeName);
      if (type == null && xmlNamespace == XmlNamespace.DesignTimeXmlNamespace)
        type = typeResolver.PlatformMetadata.GetDesignTimeType(typeResolver, (IXmlNamespace) xmlNamespace, typeName);
      return type;
    }

    internal static IProperty GetPropertyKey(XamlParserContext parserContext, ITextLocation lineInformation, IXmlNamespaceResolver xmlElementReference, string fullPropertyName, XmlNamespace targetTypeNamespace, IType targetTypeId, MemberType memberTypes, MemberType defaultType, bool allowProtectedPropertiesOnTargetType)
    {
      XmlnsPrefix prefix;
      string typeName;
      if (!XamlTypeHelper.SplitTypeName(parserContext, lineInformation, fullPropertyName, out prefix, out typeName))
        return (IProperty) null;
      XmlNamespace xmlNamespace = (XmlNamespace) null;
      if (prefix != XmlnsPrefix.EmptyPrefix || typeName.IndexOf('.') >= 0)
      {
        xmlNamespace = parserContext.GetXmlNamespace(lineInformation, xmlElementReference, prefix);
        if (xmlNamespace == null)
          return (IProperty) null;
      }
      return XamlTypeHelper.GetPropertyKey(parserContext, lineInformation, xmlNamespace, typeName, targetTypeNamespace, targetTypeId, memberTypes, defaultType, allowProtectedPropertiesOnTargetType);
    }

    internal static IProperty GetPropertyKey(XamlParserContext parserContext, ITextLocation lineInformation, XmlNamespace xmlNamespace, string typeAndPropertyName, XmlNamespace targetTypeNamespace, IType targetTypeId, MemberType memberTypes, MemberType defaultType, bool allowProtectedPropertiesOnTargetType)
    {
      string typeName;
      string memberName;
      if (XamlTypeHelper.SplitMemberName(parserContext, lineInformation, typeAndPropertyName, out typeName, out memberName))
      {
        if (string.IsNullOrEmpty(memberName))
        {
          parserContext.ReportError(XamlParseErrors.MissingArgumentName(lineInformation));
          return (IProperty) null;
        }
        if (xmlNamespace == XmlNamespace.DesignTimeXmlNamespace)
          return parserContext.PlatformMetadata.GetDesignTimeProperty(memberName, targetTypeId);
        if (xmlNamespace != null && typeName == null)
        {
          if (xmlNamespace == XmlNamespace.XamlXmlNamespace)
          {
            IProperty property = XamlProcessingAttributes.GetProperty(memberName, parserContext.TypeResolver.PlatformMetadata);
            if (property != null)
              return property;
            parserContext.ReportError(XamlParseErrors.UnrecognizedAttribute(lineInformation, xmlNamespace, memberName));
            return (IProperty) null;
          }
          if (xmlNamespace == XmlNamespace.PresentationOptionsXmlNamespace)
          {
            if (memberName == parserContext.PlatformMetadata.KnownProperties.DesignTimeFreeze.Name)
              return parserContext.TypeResolver.ResolveProperty(parserContext.PlatformMetadata.KnownProperties.DesignTimeFreeze);
            parserContext.ReportError(XamlParseErrors.UnrecognizedAttribute(lineInformation, xmlNamespace, memberName));
            return (IProperty) null;
          }
          if (targetTypeNamespace == null || !xmlNamespace.Equals((object) targetTypeNamespace))
          {
            parserContext.ReportError(XamlParseErrors.UnrecognizedAttribute(lineInformation, xmlNamespace, memberName));
            return (IProperty) null;
          }
        }
        MemberAccessTypes access = MemberAccessTypes.None;
        if (typeName != null)
        {
          if (xmlNamespace != null)
          {
            targetTypeId = XamlTypeHelper.GetTypeId(parserContext, lineInformation, xmlNamespace, typeName);
            if (parserContext.PlatformMetadata.IsNullType((ITypeId) targetTypeId))
              return (IProperty) null;
            access = TypeHelper.GetAllowableMemberAccess(parserContext.TypeResolver, targetTypeId);
          }
        }
        else if (!parserContext.PlatformMetadata.IsNullType((ITypeId) targetTypeId))
        {
          access = TypeHelper.GetAllowableMemberAccess(parserContext.TypeResolver, targetTypeId);
          if (allowProtectedPropertiesOnTargetType)
            access |= MemberAccessTypes.Protected;
        }
        if (!parserContext.PlatformMetadata.IsNullType((ITypeId) targetTypeId))
        {
          IProperty property1 = (IProperty) targetTypeId.GetMember(defaultType, memberName, access);
          if (property1 == null)
          {
            MemberType memberTypes1 = memberTypes & ~defaultType;
            if (memberTypes1 != MemberType.None)
              property1 = (IProperty) targetTypeId.GetMember(memberTypes1, memberName, access);
          }
          if (property1 == null && targetTypeId.PlatformMetadata.GetProxyProperties(parserContext.TypeResolver) != null)
          {
            foreach (IProperty property2 in targetTypeId.PlatformMetadata.GetProxyProperties(parserContext.TypeResolver))
            {
              if (memberName == property2.Name && property2.DeclaringType.IsAssignableFrom((ITypeId) targetTypeId) && TypeHelper.IsSet(memberTypes, property2.MemberType))
              {
                property1 = property2;
                break;
              }
            }
          }
          if (property1 == null)
            property1 = (IProperty) XamlTypeHelper.AddMemberIfPossible(parserContext.PlatformMetadata, targetTypeId, defaultType, memberName);
          if (property1 == null)
            parserContext.ReportError(XamlParseErrors.UnrecognizedOrInaccessibleMember(lineInformation, memberName));
          return property1;
        }
        parserContext.ReportError(XamlParseErrors.CannotDetermineMemberTargetType(lineInformation, memberName));
      }
      return (IProperty) null;
    }

    internal static bool SplitTypeName(XamlParserContext parserContext, ITextLocation lineInformation, string prefixAndTypeName, out XmlnsPrefix prefix, out string typeName)
    {
      int length = prefixAndTypeName.IndexOf(':');
      if (length >= 0)
      {
        if (length == 0 || length == prefixAndTypeName.Length - 1)
        {
          parserContext.ReportError(XamlParseErrors.InvalidPrefixQualifiedTypeName(lineInformation, prefixAndTypeName));
          prefix = (XmlnsPrefix) null;
          typeName = (string) null;
          return false;
        }
        prefix = XmlnsPrefix.ToPrefix(prefixAndTypeName.Substring(0, length));
        typeName = prefixAndTypeName.Substring(length + 1);
        return true;
      }
      prefix = XmlnsPrefix.EmptyPrefix;
      typeName = prefixAndTypeName;
      return true;
    }

    internal static bool SplitMemberName(XamlParserContext parserContext, ITextLocation lineInformation, string typeAndMemberName, out string typeName, out string memberName)
    {
      typeName = (string) null;
      memberName = (string) null;
      int length = typeAndMemberName.LastIndexOf('.');
      if (length >= 0)
      {
        if (length == 0 || length == typeAndMemberName.Length - 1)
        {
          parserContext.ReportError(XamlParseErrors.InvalidTypeQualifiedMemberName(lineInformation, typeAndMemberName));
          return false;
        }
        typeName = typeAndMemberName.Substring(0, length);
        memberName = typeAndMemberName.Substring(length + 1);
        return true;
      }
      memberName = typeAndMemberName;
      return true;
    }

    internal static IMember AddMemberIfPossible(IPlatformMetadata platformMetadata, IType typeId, MemberType memberType, string memberName)
    {
      if (!typeId.IsResolvable)
        return platformMetadata.CreateUnknownMember(typeId, memberType, memberName);
      return (IMember) null;
    }
  }
}
