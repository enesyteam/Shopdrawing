// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.XamlParseErrors
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using Microsoft.Expression.DesignModel.Markup.Xml;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.Text;
using System;
using System.Globalization;
using System.Runtime.Versioning;

namespace Microsoft.Expression.DesignModel.Markup
{
  internal static class XamlParseErrors
  {
    public static XamlParseError NewParseError(XamlErrorSeverity severity, XamlErrorCode errorCode, ITextLocation lineInformation, string messageFormat, params string[] parameters)
    {
      return new XamlParseError(severity, (int) errorCode, lineInformation, messageFormat, parameters);
    }

    public static XamlParseError XmlError(ErrorNode errorNode, IReadableSelectableTextBuffer textBuffer, int offset)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.XmlError, textBuffer.GetLocation(errorNode.SourceContext.StartCol + offset), errorNode.ErrorCode, errorNode.ErrorParameters);
    }

    public static XamlParseError NoRootElement(ITextLocation lineInformation)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.NoRootElement, lineInformation, StringTable.ParserNoRootElement);
    }

    public static XamlParseError UnexpectedRootType(ITextLocation lineInformation, ITypeId expectedType)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.UnexpectedRootType, lineInformation, StringTable.ParserUnexpectedRootType, expectedType.Name);
    }

    public static XamlParseError MultipleRootElements(ITextLocation lineInformation)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.MultipleRootElements, lineInformation, StringTable.ParserMultipleRootElements);
    }

    public static XamlParseError NoDefaultNamespace(ITextLocation lineInformation)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.NoDefaultNamespace, lineInformation, StringTable.ParserNoDefaultNamespace);
    }

    public static XamlParseError InvalidClassName(ITextLocation lineInformation, string typeName)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Warning, XamlErrorCode.InvalidClassName, lineInformation, StringTable.ParserInvalidClassName, typeName);
    }

    public static XamlParseError UnrecognizedXmlnsPrefix(ITextLocation lineInformation, XmlnsPrefix prefix)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.UnrecognizedXmlnsPrefix, lineInformation, StringTable.ParserUnrecognizedXmlnsPrefix, prefix.Value);
    }

    public static XamlParseError DuplicateXmlnsPrefix(ITextLocation lineInformation, XmlnsPrefix prefix)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.DuplicateXmlnsPrefix, lineInformation, StringTable.ParserDuplicateXmlnsPrefix, prefix.Value);
    }

    public static XamlParseError UnrecognizedPlatformTypeName(ITextLocation lineInformation, bool isSilverlightProject, string typeName)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.UnrecognizedTypeName, lineInformation, StringTable.ParserUnrecognizedPlatformTypeName, isSilverlightProject ? StringTable.ParserSilverlightPlatformName : StringTable.ParserWPFPlatformName, typeName);
    }

    public static XamlParseError UnrecognizedTypeName(ITextLocation lineInformation, XmlNamespace xmlNamespace, string typeName)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.UnrecognizedTypeName, lineInformation, StringTable.ParserUnrecognizedTypeName, xmlNamespace.Value, typeName);
    }

    public static XamlParseError InaccessibleType(ITextLocation lineInformation, string typeName)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.InaccessibleType, lineInformation, StringTable.ParserInaccessibleType, typeName);
    }

    public static XamlParseError InnerTypesNotSupported(ITextLocation lineInformation, string typeName)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.InnerTypesNotSupported, lineInformation, StringTable.ParserInnerTypesNotSupported, typeName);
    }

    public static XamlParseError NestedPropertiesNotSupported(ITextLocation lineInformation, string propertyName)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.NestedPropertiesNotSupported, lineInformation, StringTable.ParserNestedPropertiesNotSupported, propertyName);
    }

    public static XamlParseError UnrecognizedOrInaccessibleMember(ITextLocation lineInformation, string memberName)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.UnrecognizedOrInaccessibleMember, lineInformation, StringTable.ParserUnrecognizedOrInaccessibleMember, memberName);
    }

    public static XamlParseError PropertyIsNotWritable(ITextLocation lineInformation, IPropertyId propertyKey)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.PropertyIsNotWritable, lineInformation, StringTable.ParserPropertyIsNotWritable, propertyKey.Name);
    }

    public static XamlParseError CannotDetermineMemberTargetType(ITextLocation lineInformation, string memberName)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.CannotDetermineMemberTargetType, lineInformation, StringTable.ParserCannotDetermineMemberTargetType, memberName);
    }

    public static XamlParseError UnrecognizedDesignTimeProperty(ITextLocation lineInformation, string propertyName)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.UnrecognizedDesignTimeProperty, lineInformation, StringTable.ParserUnrecognizedDesignTimeProperty, propertyName);
    }

    public static XamlParseError AmbiguousDesignTimeProperty(ITextLocation lineInformation, string propertyName)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.AmbiguousDesignTimeProperty, lineInformation, StringTable.ParserAmbiguousDesignTimeProperty, propertyName);
    }

    public static XamlParseError MissingTypeName(ITextLocation lineInformation)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.MissingTypeName, lineInformation, StringTable.ParserMissingTypeName);
    }

    public static XamlParseError InvalidPrefixQualifiedTypeName(ITextLocation lineInformation, string prefixQualifiedTypeName)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.InvalidPrefixQualifiedTypeName, lineInformation, StringTable.ParserInvalidPrefixQualifiedTypeName, prefixQualifiedTypeName);
    }

    public static XamlParseError InvalidTypeQualifiedMemberName(ITextLocation lineInformation, string typeQualifiedMemberName)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.InvalidTypeQualifiedMemberName, lineInformation, StringTable.ParserInvalidTypeQualifiedMemberName, typeQualifiedMemberName);
    }

    public static XamlParseError PrimitiveTypeWithNoValue(ITextLocation lineInformation, ITypeId typeId)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.PrimitiveTypeWithNoValue, lineInformation, StringTable.ParserPrimitiveTypeWithNoValue, typeId.Name);
    }

    public static XamlParseError AbstractTypeWithNoValue(ITextLocation lineInformation, ITypeId typeId)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.AbstractTypeWithNoValue, lineInformation, StringTable.ParserAbstractTypeWithNoValue, typeId.Name);
    }

    public static XamlParseError ArrayPropertyRequiresExplicitPropertyTag(ITextLocation lineInformation)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.ArrayPropertyRequiresExplicitPropertyTag, lineInformation, StringTable.ParserArrayPropertyRequiresExplicitPropertyTag);
    }

    public static XamlParseError NoAccessibleConstructor(ITextLocation lineInformation, ITypeId typeId)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.NoAccessibleConstructor, lineInformation, StringTable.ParserNoAccessibleConstructor, typeId.Name);
    }

    public static XamlParseError IncorrectNumberOfConstructorArguments(ITextLocation lineInformation, ITypeId typeId)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.IncorrectNumberOfConstructorArguments, lineInformation, StringTable.ParserIncorrectNumberOfConstructorArguments, typeId.Name);
    }

    public static XamlParseError ConstructorArgumentDeclaredTypeDoesNotMatchProperty(ITextLocation lineInformation, IPropertyId propertyKey)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.ConstructorArgumentDeclaredTypeDoesNotMatchProperty, lineInformation, StringTable.ParserConstructorArgumentDeclaredTypeDoesNotMatchProperty, propertyKey.Name);
    }

    public static XamlParseError TypeIsNotNullable(ITextLocation lineInformation, ITypeId typeId)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.TypeIsNotNullable, lineInformation, StringTable.ParserTypeIsNotNullable, typeId.Name);
    }

    public static XamlParseError TypeDoesNotSupportInlineXml(ITextLocation lineInformation, ITypeId typeId)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.TypeDoesNotSupportInlineXml, lineInformation, StringTable.ParserTypeDoesNotSupportInlineXml, typeId.Name);
    }

    public static XamlParseError CannotConvertFromString(ITextLocation lineInformation, ITypeId typeId)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.CannotConvertFromString, lineInformation, StringTable.ParserCannotConvertFromString, typeId.Name);
    }

    public static XamlParseError UnrecognizedName(ITextLocation lineInformation, string name)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.UnrecognizedName, lineInformation, StringTable.ParserUnrecognizedName, name);
    }

    public static XamlParseError RepeatedName(ITextLocation lineInformation, string name)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.RepeatedName, lineInformation, StringTable.ParserRepeatedName, name);
    }

    public static XamlParseError RepeatedKey(ITextLocation lineInformation)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.RepeatedKey, lineInformation, StringTable.ParserRepeatedKey);
    }

    public static XamlParseError UnrecognizedAttribute(ITextLocation lineInformation, XmlNamespace xmlNamespace, string attributeName)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.UnrecognizedAttribute, lineInformation, StringTable.ParserUnrecognizedAttribute, xmlNamespace.Value, attributeName);
    }

    public static XamlParseError UnrecognizedXmlAttribute(ITextLocation lineInformation, string attributeName)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.UnrecognizedXmlAttribute, lineInformation, StringTable.ParserUnrecognizedXmlAttribute, attributeName);
    }

    public static XamlParseError AttributeValidAtRootOnly(ITextLocation lineInformation, string attributeName)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.AttributeValidAtRootOnly, lineInformation, StringTable.ParserAttributeValidAtRootOnly, attributeName);
    }

    public static XamlParseError AttributeRequiresExplicitClassAttribute(ITextLocation lineInformation, string attributeName)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Warning, XamlErrorCode.AttributeRequiresExplicitClassAttribute, lineInformation, StringTable.ParserAttributeRequiresExplicitClassAttribute, attributeName);
    }

    public static XamlParseError AttributeValidInSpecificContainerOnly(ITextLocation lineInformation, ITypeId containerType, string attributeName)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.AttributeValidInSpecificContainerOnly, lineInformation, StringTable.ParserAttributeValidInSpecificContainerOnly, attributeName, containerType.Name);
    }

    public static XamlParseError MissingProperty(ITextLocation lineInformation, IPropertyId propertyKey)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.MissingProperty, lineInformation, StringTable.ParserMissingProperty, propertyKey.Name);
    }

    public static XamlParseError PropertyElementWithNoValue(ITextLocation lineInformation, IPropertyId propertyKey)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.PropertyElementWithNoValue, lineInformation, StringTable.ParserPropertyElementWithNoValue, propertyKey.Name);
    }

    public static XamlParseError PropertySetMultipleTimes(ITextLocation lineInformation, IPropertyId propertyKey)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.PropertySetMultipleTimes, lineInformation, StringTable.ParserPropertySetMultipleTimes, propertyKey.Name);
    }

    public static XamlParseError SingleValuedPropertySetMultipleTimes(ITextLocation lineInformation, IPropertyId propertyKey)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.SingleValuedPropertySetMultipleTimes, lineInformation, StringTable.ParserSingleValuedPropertySetMultipleTimes, propertyKey.Name);
    }

    public static XamlParseError MemberNotDependencyProperty(ITextLocation lineInformation, IMemberId memberId)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.MemberNotDependencyProperty, lineInformation, StringTable.ParserMemberNotDependencyProperty, memberId.Name);
    }

    public static XamlParseError MemberNotRoutedEvent(ITextLocation lineInformation, IMemberId memberId)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.MemberNotRoutedEvent, lineInformation, StringTable.ParserMemberNotRoutedEvent, memberId.Name);
    }

    public static XamlParseError TypeInitializationException(ITextLocation lineInformation, ITypeId typeId, string message)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.TypeInitializationException, lineInformation, StringTable.ParserTypeInitializationException, typeId.Name, message);
    }

    public static XamlParseError LocalPropertyDoesNotApply(ITextLocation lineInformation, IProperty propertyKey)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.LocalPropertyDoesNotApply, lineInformation, StringTable.ParserLocalPropertyDoesNotApply, propertyKey.Name, propertyKey.DeclaringType.Name);
    }

    public static XamlParseError AttachedPropertyDoesNotApply(ITextLocation lineInformation, IProperty propertyKey)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.AttachedPropertyDoesNotApply, lineInformation, StringTable.ParserAttachedPropertyDoesNotApply, propertyKey.Name, propertyKey.TargetType.Name);
    }

    public static XamlParseError MissingDictionaryKey(ITextLocation lineInformation)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.MissingDictionaryKey, lineInformation, StringTable.ParserMissingDictionaryKey);
    }

    public static XamlParseError UnexpectedValueType(ITextLocation lineInformation, ITypeId expectedType)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.UnexpectedValueType, lineInformation, StringTable.ParserUnexpectedValueType, expectedType.Name);
    }

    public static XamlParseError UnexpectedChildType(ITextLocation lineInformation, ITypeId expectedType)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.UnexpectedChildType, lineInformation, StringTable.ParserUnexpectedChildType, expectedType.Name);
    }

    public static XamlParseError ParentTypeDoesNotSupportChildren(ITextLocation lineInformation, ITypeId parentType)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.ParentTypeDoesNotSupportChildren, lineInformation, StringTable.ParserParentTypeDoesNotSupportChildren, parentType.Name);
    }

    public static XamlParseError MissingMarkupExtensionName(ITextLocation lineInformation)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.MissingMarkupExtensionName, lineInformation, StringTable.ParserMissingMarkupExtensionName);
    }

    public static XamlParseError MissingArgumentName(ITextLocation lineInformation)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.MissingArgumentName, lineInformation, StringTable.ParserMissingArgumentName);
    }

    public static XamlParseError UnexpectedPositionalArgument(ITextLocation lineInformation, string argumentName)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.UnexpectedPositionalArgument, lineInformation, StringTable.ParserUnexpectedPositionalArgument, argumentName);
    }

    public static XamlParseError UnexpectedEnd(ITextLocation lineInformation)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.UnexpectedEnd, lineInformation, StringTable.ParserUnexpectedEnd);
    }

    public static XamlParseError InvalidMarkupExtensionArguments(ITextLocation lineInformation, string markupExtensionName)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.InvalidMarkupExtensionArguments, lineInformation, StringTable.ParserInvalidMarkupExtensionArguments, markupExtensionName);
    }

    public static XamlParseError UnexpectedCharacter(ITextLocation lineInformation, char c)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.UnexpectedCharacter, lineInformation, StringTable.ParserUnexpectedCharacter, c.ToString());
    }

    public static XamlParseError InvalidPropertyPathSyntax(ITextLocation lineInformation, string propertyPath)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.InvalidPropertyPathSyntax, lineInformation, StringTable.ParserInvalidPropertyPathSyntax, propertyPath);
    }

    public static XamlParseError InvalidClrNamespaceUri(ITextLocation lineInformation, string uriValue)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.InvalidClrNamespaceUri, lineInformation, StringTable.ParserInvalidClrNamespaceUri, uriValue);
    }

    public static XamlParseError InvalidXmlSpace(ITextLocation lineInformation, string value)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.InvalidXmlSpace, lineInformation, StringTable.ParserInvalidXmlSpace, value);
    }

    public static XamlParseError CDataNotSupported(ITextLocation lineInformation)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.CDataNotSupported, lineInformation, StringTable.ParserCDataNotSupported);
    }

    public static XamlParseError UnknownError(ITextLocation lineInformation)
    {
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.UnknownError, lineInformation, StringTable.ParserUnknownError);
    }

    public static XamlParseError MismatchedVersionSDKType(ITextLocation lineInformation, IType childType, FrameworkName frameworkName)
    {
      string str = (string) (frameworkName.Identifier == "Silverlight" ? (object) frameworkName.Identifier : (object) "Windows Presentation Framework") + (object) " " + (string) (object) frameworkName.Version.Major;
      string messageFormat = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.MismatchedVersionSDKType, (object) childType.Name, (object) childType.RuntimeAssembly.Name, (object) str);
      return XamlParseErrors.NewParseError(XamlErrorSeverity.Error, XamlErrorCode.MismatchedType, lineInformation, messageFormat);
    }
  }
}
