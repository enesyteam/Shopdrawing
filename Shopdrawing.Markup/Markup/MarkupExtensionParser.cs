// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.MarkupExtensionParser
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Markup.Xml;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.Text;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace Microsoft.Expression.DesignModel.Markup
{
  internal static class MarkupExtensionParser
  {
    public static MarkupExtensionDescription Tokenize(XamlParserContext parserContext, ITextLocation lineInformation, string text)
    {
      MarkupExtensionDescription extensionDescription = new MarkupExtensionDescription();
      MarkupExtensionParser.ScannerState scannerState = MarkupExtensionParser.ScannerState.Begin;
      int startIndex = 0;
      int index = 0;
      string key = (string) null;
      while (index < text.Length)
      {
        char c = text[index];
        ++index;
        switch (scannerState)
        {
          case MarkupExtensionParser.ScannerState.Begin:
            if ((int) c == 123)
            {
              scannerState = MarkupExtensionParser.ScannerState.ScanningMarkupExtensionName;
              while (index < text.Length && Scanner.IsXmlWhitespace(text[index]))
                ++index;
              startIndex = index;
              continue;
            }
            if (!Scanner.IsXmlWhitespace(c))
            {
              parserContext.ReportError(XamlParseErrors.UnexpectedCharacter(lineInformation, c));
              return (MarkupExtensionDescription) null;
            }
            continue;
          case MarkupExtensionParser.ScannerState.ScanningMarkupExtensionName:
            if ((int) c == 125 || Scanner.IsXmlWhitespace(c))
            {
              int length = index - startIndex - 1;
              string str = MarkupExtensionParser.ReplaceEscapedCharacters(MarkupExtensionParser.TrimOffWhitespace(text.Substring(startIndex, length)));
              if (str.Length > 0)
              {
                extensionDescription.Name = str;
                scannerState = (int) c == 125 ? MarkupExtensionParser.ScannerState.End : MarkupExtensionParser.ScannerState.ScanningNameOrValue;
                startIndex = index;
                continue;
              }
              parserContext.ReportError(XamlParseErrors.MissingMarkupExtensionName(lineInformation));
              return (MarkupExtensionDescription) null;
            }
            if ((int) c == 123 || (int) c == 44 || (int) c == 61)
            {
              parserContext.ReportError(XamlParseErrors.UnexpectedCharacter(lineInformation, c));
              return (MarkupExtensionDescription) null;
            }
            continue;
          case MarkupExtensionParser.ScannerState.ScanningNameOrValue:
          case MarkupExtensionParser.ScannerState.ScanningValue:
            switch (c)
            {
              case '=':
                int length1 = index - startIndex - 1;
                key = MarkupExtensionParser.ReplaceEscapedCharacters(MarkupExtensionParser.TrimOffQuotes(MarkupExtensionParser.TrimOffWhitespace(text.Substring(startIndex, length1))));
                startIndex = index;
                if (key.Length > 0)
                {
                  scannerState = MarkupExtensionParser.ScannerState.ScanningValue;
                  continue;
                }
                parserContext.ReportError(XamlParseErrors.MissingArgumentName(lineInformation));
                return (MarkupExtensionDescription) null;
              case '\\':
                if (index < text.Length)
                {
                  ++index;
                  continue;
                }
                continue;
              case '{':
                int num = 1;
                while (index < text.Length)
                {
                  char ch = text[index];
                  ++index;
                  switch (ch)
                  {
                    case '\\':
                      if (index < text.Length)
                      {
                        ++index;
                        break;
                      }
                      break;
                    case '{':
                      ++num;
                      break;
                    case '}':
                      --num;
                      break;
                  }
                  if (num == 0)
                    break;
                }
                if (num != 0)
                {
                  parserContext.ReportError(XamlParseErrors.UnexpectedEnd(lineInformation));
                  return (MarkupExtensionDescription) null;
                }
                continue;
              case '}':
              case ',':
                int length2 = index - startIndex - 1;
                string argumentName = MarkupExtensionParser.TrimOffQuotes(MarkupExtensionParser.TrimOffWhitespace(text.Substring(startIndex, length2)));
                startIndex = index;
                if (scannerState == MarkupExtensionParser.ScannerState.ScanningNameOrValue)
                {
                  if (extensionDescription.NamedArguments.Count > 0)
                  {
                    parserContext.ReportError(XamlParseErrors.UnexpectedPositionalArgument(lineInformation, argumentName));
                    return (MarkupExtensionDescription) null;
                  }
                  extensionDescription.PositionalArguments.Add(argumentName);
                }
                else
                {
                  if (string.IsNullOrEmpty(key))
                  {
                    parserContext.ReportError(XamlParseErrors.UnexpectedPositionalArgument(lineInformation, argumentName));
                    return (MarkupExtensionDescription) null;
                  }
                  extensionDescription.NamedArguments.Add(new KeyValuePair<string, string>(key, argumentName));
                }
                scannerState = (int) c != 125 ? MarkupExtensionParser.ScannerState.ScanningNameOrValue : MarkupExtensionParser.ScannerState.End;
                continue;
              case '"':
              case '\'':
                char ch1 = c;
                while (index < text.Length)
                {
                  char ch2 = text[index];
                  ++index;
                  if ((int) ch1 == (int) ch2)
                    break;
                }
                continue;
              default:
                continue;
            }
          default:
            if (!Scanner.IsXmlWhitespace(c))
            {
              parserContext.ReportError(XamlParseErrors.UnexpectedCharacter(lineInformation, c));
              return (MarkupExtensionDescription) null;
            }
            continue;
        }
      }
      if (scannerState == MarkupExtensionParser.ScannerState.End)
        return extensionDescription;
      parserContext.ReportError(XamlParseErrors.UnexpectedEnd(lineInformation));
      return (MarkupExtensionDescription) null;
    }

    private static string ReplaceEscapedCharacters(string text)
    {
      char[] chArray = new char[text.Length];
      int length = 0;
      bool flag = false;
      foreach (char ch in text)
      {
        if (flag || (int) ch != 92)
        {
          chArray[length++] = ch;
          flag = false;
        }
        else
          flag = true;
      }
      return new string(chArray, 0, length);
    }

    private static string TrimOffQuotes(string text)
    {
      int length = text.Length;
      if (length > 1)
      {
        switch (text[0])
        {
          case '"':
          case '\'':
            if ((int) text[length - 1] == (int) text[0])
              return text.Substring(1, length - 2);
            break;
        }
      }
      return text;
    }

    private static string TrimOffWhitespace(string text)
    {
      int startIndex = 0;
      int length = 0;
      for (int index = 0; index < text.Length; ++index)
      {
        char c = text[index];
        if (!Scanner.IsXmlWhitespace(c))
        {
          if (length == 0)
            startIndex = index;
          if ((int) c == 92 && index < text.Length - 1)
            ++index;
          length = index - startIndex + 1;
        }
      }
      if (startIndex > 0 || length < text.Length)
        return text.Substring(startIndex, length);
      return text;
    }

    public static DocumentNode ParseMarkupExtension(XamlParserContext parserContext, ITextLocation lineInformation, IDocumentNodeReference nodeReference, IXmlNamespaceResolver xmlNamespaceResolver, IType valueType, string text)
    {
      MarkupExtensionDescription description = MarkupExtensionParser.Tokenize(parserContext, lineInformation, text);
      if (description != null)
      {
        IType typeId1 = XamlTypeHelper.GetTypeId(parserContext, lineInformation, xmlNamespaceResolver, description.Name, true);
        if (typeId1 != null)
        {
          if (parserContext.PlatformMetadata.KnownTypes.NullExtension.Equals((object) typeId1))
          {
            if (description.PositionalArguments.Count > 0 || description.NamedArguments.Count > 0)
              parserContext.ReportError(XamlParseErrors.InvalidMarkupExtensionArguments(lineInformation, description.Name));
            if (valueType.SupportsNullValues)
              return (DocumentNode) parserContext.DocumentContext.CreateNode((ITypeId) valueType, (IDocumentNodeValue) null);
            parserContext.ReportError(XamlParseErrors.TypeIsNotNullable(lineInformation, (ITypeId) valueType));
            return (DocumentNode) null;
          }
          if (parserContext.PlatformMetadata.KnownTypes.TypeExtension.IsAssignableFrom((ITypeId) typeId1))
          {
            string extensionArgument = MarkupExtensionParser.GetRequiredMarkupExtensionArgument(parserContext, lineInformation, description, parserContext.PlatformMetadata.KnownProperties.TypeExtensionTypeName.Name);
            if (extensionArgument != null)
            {
              IType typeId2 = XamlTypeHelper.GetTypeId(parserContext, lineInformation, xmlNamespaceResolver, extensionArgument, true);
              if (typeId2 != null)
                return (DocumentNode) parserContext.DocumentContext.CreateNode(parserContext.PlatformMetadata.KnownTypes.Type, (IDocumentNodeValue) new DocumentNodeMemberValue((IMember) typeId2));
            }
          }
          else if (parserContext.PlatformMetadata.KnownTypes.StaticExtension.IsAssignableFrom((ITypeId) typeId1))
          {
            string extensionArgument = MarkupExtensionParser.GetRequiredMarkupExtensionArgument(parserContext, lineInformation, description, parserContext.PlatformMetadata.KnownProperties.StaticExtensionMember.Name);
            if (extensionArgument != null)
            {
              IType typeId2;
              string memberName;
              if (MarkupExtensionParser.GetTypeAndMemberName(parserContext, lineInformation, xmlNamespaceResolver, extensionArgument, (IType) null, out typeId2, out memberName))
              {
                MemberAccessTypes allowableMemberAccess = TypeHelper.GetAllowableMemberAccess(parserContext.TypeResolver, typeId2);
                MemberType memberTypes = MemberType.LocalProperty | MemberType.Field;
                if (parserContext.TypeResolver.IsCapabilitySet(PlatformCapability.IncompleteAttachedPropertiesInMarkupExtensions))
                  memberTypes |= MemberType.IncompleteAttachedProperty;
                IMember memberId = (IMember) typeId2.GetMember(memberTypes, memberName, allowableMemberAccess) ?? XamlTypeHelper.AddMemberIfPossible(parserContext.PlatformMetadata, typeId2, MemberType.LocalProperty, memberName);
                if (memberId != null)
                  return (DocumentNode) DocumentNodeHelper.NewStaticNode(parserContext.DocumentContext, memberId);
                parserContext.ReportError(XamlParseErrors.UnrecognizedOrInaccessibleMember(lineInformation, memberName));
              }
              return (DocumentNode) null;
            }
          }
          else
          {
            if (!parserContext.TypeResolver.PlatformMetadata.KnownTypes.Binding.Equals((object) typeId1) || description.PositionalArguments.Count != 1)
              return MarkupExtensionParser.ParseMarkupExtension(parserContext, lineInformation, nodeReference, xmlNamespaceResolver, typeId1, description);
            description.NamedArguments.Insert(0, new KeyValuePair<string, string>(parserContext.TypeResolver.PlatformMetadata.KnownProperties.BindingPath.Name, description.PositionalArguments[0]));
            description.PositionalArguments.Clear();
            return MarkupExtensionParser.ParseMarkupExtension(parserContext, lineInformation, nodeReference, xmlNamespaceResolver, typeId1, description);
          }
        }
      }
      return (DocumentNode) null;
    }

    public static DocumentNode GetPropertyValue(XamlParserContext parserContext, ITextLocation lineInformation, IDocumentNodeReference nodeReference, IXmlNamespaceResolver xmlNamespaceResolver, IProperty propertyKey, string value)
    {
      return MarkupExtensionParser.GetPropertyValue(parserContext, lineInformation, nodeReference, xmlNamespaceResolver, propertyKey, value, false);
    }

    public static DocumentNode GetPropertyValue(XamlParserContext parserContext, ITextLocation lineInformation, IDocumentNodeReference nodeReference, IXmlNamespaceResolver xmlNamespaceResolver, IProperty propertyKey, string value, bool removeEscapeCharacters)
    {
      TypeConverter typeConverter = propertyKey.TypeConverter ?? propertyKey.PropertyType.TypeConverter;
      return MarkupExtensionParser.GetPropertyValue(parserContext, lineInformation, nodeReference, xmlNamespaceResolver, typeConverter, propertyKey.PropertyType, value, removeEscapeCharacters);
    }

    public static DocumentNode GetPropertyValue(XamlParserContext parserContext, ITextLocation lineInformation, IDocumentNodeReference nodeReference, IXmlNamespaceResolver xmlNamespaceResolver, TypeConverter typeConverter, IType valueTypeId, string value, bool removeEscapeCharacters)
    {
      bool flag = false;
      if (value.StartsWith("{", StringComparison.Ordinal))
      {
        if (value.StartsWith("{}", StringComparison.Ordinal))
          value = value.Substring(2);
        else
          flag = true;
      }
      if (removeEscapeCharacters)
        value = MarkupExtensionParser.ReplaceEscapedCharacters(value);
      if (flag)
        return MarkupExtensionParser.ParseMarkupExtension(parserContext, lineInformation, nodeReference, xmlNamespaceResolver, valueTypeId, value);
      return MarkupExtensionParser.CreateNodeFromTextValue(parserContext, lineInformation, nodeReference, xmlNamespaceResolver, typeConverter, (ITypeId) valueTypeId, value);
    }

    public static DocumentNode CreateNodeFromTextValue(XamlParserContext parserContext, ITextLocation lineInformation, IDocumentNodeReference nodeReference, IXmlNamespaceResolver xmlNamespaceResolver, TypeConverter typeConverter, ITypeId valueTypeId, string value)
    {
      if (parserContext.PlatformMetadata.KnownTypes.DependencyProperty.IsAssignableFrom(valueTypeId))
      {
        MemberType memberTypes = MemberType.Property | MemberType.Field;
        if (parserContext.TypeResolver.IsCapabilitySet(PlatformCapability.IncompleteAttachedPropertiesInMarkupExtensions))
          memberTypes |= MemberType.IncompleteAttachedProperty;
        MemberType defaultType = value.IndexOf('.') >= 0 ? MemberType.AttachedProperty : MemberType.LocalProperty;
        IProperty propertyKey = XamlTypeHelper.GetPropertyKey(parserContext, lineInformation, xmlNamespaceResolver, value, (XmlNamespace) null, nodeReference.TargetType, memberTypes, defaultType, false);
        if (propertyKey == null)
          return (DocumentNode) null;
        MarkupExtensionParser.VerifyMemberIsDependencyProperty(parserContext, lineInformation, (IMember) propertyKey);
        return (DocumentNode) parserContext.DocumentContext.CreateNode(valueTypeId, (IDocumentNodeValue) new DocumentNodeMemberValue((IMember) propertyKey));
      }
      if (parserContext.PlatformMetadata.KnownTypes.ICommand.IsAssignableFrom(valueTypeId))
      {
        bool flag = typeConverter != null && !parserContext.PlatformMetadata.KnownTypes.CommandConverter.Equals((object) parserContext.TypeResolver.GetType(typeConverter.GetType()));
        if (value.IndexOf('.') < 0 || flag)
          return (DocumentNode) parserContext.DocumentContext.CreateNode(valueTypeId, (IDocumentNodeValue) new DocumentNodeStringValue(value));
        IType typeId;
        string memberName;
        if (MarkupExtensionParser.GetTypeAndMemberName(parserContext, lineInformation, xmlNamespaceResolver, value, (IType) null, out typeId, out memberName))
        {
          MemberAccessTypes allowableMemberAccess = TypeHelper.GetAllowableMemberAccess(parserContext.TypeResolver, typeId);
          IMember member = (IMember) typeId.GetMember(MemberType.LocalProperty | MemberType.Field, memberName, allowableMemberAccess) ?? XamlTypeHelper.AddMemberIfPossible(parserContext.PlatformMetadata, typeId, MemberType.LocalProperty, memberName);
          if (member != null)
            return (DocumentNode) parserContext.DocumentContext.CreateNode(valueTypeId, (IDocumentNodeValue) new DocumentNodeMemberValue(member));
          parserContext.ReportError(XamlParseErrors.UnrecognizedOrInaccessibleMember(lineInformation, memberName));
        }
        return (DocumentNode) null;
      }
      if (parserContext.PlatformMetadata.KnownTypes.RoutedEvent.IsAssignableFrom(valueTypeId))
      {
        IType typeId;
        string memberName;
        if (!MarkupExtensionParser.GetTypeAndMemberName(parserContext, lineInformation, xmlNamespaceResolver, value, nodeReference.TargetType, out typeId, out memberName))
          return (DocumentNode) null;
        MemberAccessTypes allowableMemberAccess = TypeHelper.GetAllowableMemberAccess(parserContext.TypeResolver, typeId);
        IMember member = (IMember) typeId.GetMember(MemberType.Event, memberName, allowableMemberAccess);
        if (member != null)
          MarkupExtensionParser.VerifyMemberIsRoutedEvent(parserContext, lineInformation, member);
        else
          member = XamlTypeHelper.AddMemberIfPossible(parserContext.PlatformMetadata, typeId, MemberType.RoutedEvent, memberName);
        if (member != null)
          return (DocumentNode) parserContext.DocumentContext.CreateNode(valueTypeId, (IDocumentNodeValue) new DocumentNodeMemberValue(member));
        parserContext.ReportError(XamlParseErrors.UnrecognizedOrInaccessibleMember(lineInformation, memberName));
        return (DocumentNode) null;
      }
      if (parserContext.PlatformMetadata.KnownTypes.Delegate.IsAssignableFrom(valueTypeId))
      {
        string memberName = value;
        if (parserContext.RootClassAttributes == null && parserContext.TypeResolver.IsCapabilitySet(PlatformCapability.IsWpf))
          parserContext.ReportError(XamlParseErrors.CannotDetermineMemberTargetType(lineInformation, memberName));
        return (DocumentNode) parserContext.DocumentContext.CreateNode(valueTypeId, (IDocumentNodeValue) new DocumentNodeStringValue(memberName));
      }
      if (parserContext.PlatformMetadata.KnownTypes.PropertyPath.IsAssignableFrom(valueTypeId))
        return MarkupExtensionParser.ParsePropertyPath(parserContext, lineInformation, xmlNamespaceResolver, value);
      if (parserContext.PlatformMetadata.KnownTypes.Uri.IsAssignableFrom(valueTypeId))
        return (DocumentNode) parserContext.DocumentContext.CreateNode(valueTypeId, (IDocumentNodeValue) new DocumentNodeStringValue(value));
      if (parserContext.PlatformMetadata.KnownTypes.Type.IsAssignableFrom(valueTypeId))
      {
        IType typeId = XamlTypeHelper.GetTypeId(parserContext, lineInformation, xmlNamespaceResolver, value, false);
        if (typeId != null)
          return (DocumentNode) parserContext.DocumentContext.CreateNode(valueTypeId, (IDocumentNodeValue) new DocumentNodeMemberValue((IMember) typeId));
        return (DocumentNode) null;
      }
      if (valueTypeId.Equals((object) parserContext.TypeResolver.PlatformMetadata.KnownTypes.Object))
        valueTypeId = (ITypeId) parserContext.TypeResolver.ResolveType(parserContext.TypeResolver.PlatformMetadata.KnownTypes.String);
      if (valueTypeId.IsResolvable && valueTypeId != parserContext.TypeResolver.ResolveType(parserContext.TypeResolver.PlatformMetadata.KnownTypes.String) && (typeConverter == null || !typeConverter.CanConvertFrom(typeof (string))))
        parserContext.ReportError(XamlParseErrors.CannotConvertFromString(lineInformation, valueTypeId));
      return (DocumentNode) parserContext.DocumentContext.CreateNode(valueTypeId, (IDocumentNodeValue) new DocumentNodeStringValue(value));
    }

    internal static DocumentNode ParsePropertyPath(XamlParserContext parserContext, ITextLocation lineInformation, IXmlNamespaceResolver xmlNamespaceResolver, string path)
    {
      if (parserContext.TypeResolver.ResolveProperty(parserContext.PlatformMetadata.KnownProperties.PropertyPathPathParameters) == null)
      {
        DocumentCompositeNode node = parserContext.DocumentContext.CreateNode(parserContext.PlatformMetadata.KnownTypes.PropertyPath);
        node.Properties[parserContext.PlatformMetadata.KnownProperties.PropertyPathPath] = (DocumentNode) parserContext.DocumentContext.CreateNode(path);
        return (DocumentNode) node;
      }
      string propertyPath;
      IList<IProperty> pathParameters;
      MarkupExtensionParser.ParsePropertyPathParameters(parserContext, lineInformation, xmlNamespaceResolver, path, out propertyPath, out pathParameters);
      return (DocumentNode) MarkupExtensionParser.ConstructPropertyPathNode(parserContext, propertyPath, pathParameters);
    }

    internal static DocumentCompositeNode ConstructPropertyPathNode(XamlParserContext parserContext, string propertyPath, IList<IProperty> pathParameters)
    {
      DocumentCompositeNode node1 = parserContext.DocumentContext.CreateNode(parserContext.PlatformMetadata.KnownTypes.PropertyPath);
      node1.Properties[parserContext.PlatformMetadata.KnownProperties.PropertyPathPath] = (DocumentNode) parserContext.DocumentContext.CreateNode(propertyPath);
      if (pathParameters.Count > 0)
      {
        DocumentCompositeNode node2 = parserContext.DocumentContext.CreateNode(typeof (Collection<object>));
        node1.Properties[parserContext.PlatformMetadata.KnownProperties.PropertyPathPathParameters] = (DocumentNode) node2;
        foreach (IProperty property in (IEnumerable<IProperty>) pathParameters)
        {
          DocumentNode documentNode = (DocumentNode) parserContext.DocumentContext.CreateNode(parserContext.PlatformMetadata.KnownTypes.DependencyProperty, (IDocumentNodeValue) new DocumentNodeMemberValue((IMember) property));
          node2.Children.Add(documentNode);
        }
      }
      return node1;
    }

    internal static void ParsePropertyPathParameters(XamlParserContext parserContext, ITextLocation lineInformation, IXmlNamespaceResolver xmlNamespaceResolver, string path, out string propertyPath, out IList<IProperty> pathParameters)
    {
      pathParameters = (IList<IProperty>) new List<IProperty>();
      StringBuilder stringBuilder = new StringBuilder();
      int startIndex;
      int num1;
      int num2;
      for (startIndex = 0; (num1 = path.IndexOf('(', startIndex)) >= 0; startIndex = num2 + 1)
      {
        num2 = path.IndexOf(')', num1 + 1);
        if (num2 < 0)
        {
          parserContext.ReportError(XamlParseErrors.InvalidPropertyPathSyntax(lineInformation, path));
          startIndex = path.Length;
          break;
        }
        string prefixAndTypeName = path.Substring(num1 + 1, num2 - num1 - 1);
        if (prefixAndTypeName.IndexOf('.') >= 0)
        {
          stringBuilder.Append(path.Substring(startIndex, num1 - startIndex));
          stringBuilder.Append('(');
          stringBuilder.Append(pathParameters.Count);
          stringBuilder.Append(')');
          IProperty property = (IProperty) null;
          XmlnsPrefix prefix;
          string typeName;
          if (XamlTypeHelper.SplitTypeName(parserContext, lineInformation, prefixAndTypeName, out prefix, out typeName))
          {
            XmlNamespace xmlNamespace = parserContext.GetXmlNamespace(lineInformation, xmlNamespaceResolver, prefix);
            MemberType memberTypes = MemberType.Property | MemberType.Field;
            if (parserContext.TypeResolver.IsCapabilitySet(PlatformCapability.IncompleteAttachedPropertiesInMarkupExtensions))
              memberTypes |= MemberType.IncompleteAttachedProperty;
            IMember member = (IMember) XamlTypeHelper.GetPropertyKey(parserContext, lineInformation, xmlNamespace, typeName, (XmlNamespace) null, (IType) null, memberTypes, MemberType.AttachedProperty, false);
            if (member != null)
              property = MarkupExtensionParser.VerifyMemberIsDependencyProperty(parserContext, lineInformation, member);
          }
          pathParameters.Add(property);
        }
        else
          stringBuilder.Append(path.Substring(startIndex, num2 - startIndex + 1));
      }
      stringBuilder.Append(path.Substring(startIndex));
      propertyPath = stringBuilder.ToString();
    }

    private static DocumentNode ParseMarkupExtension(XamlParserContext parserContext, ITextLocation lineInformation, IDocumentNodeReference nodeReference, IXmlNamespaceResolver xmlNamespaceResolver, IType typeId, MarkupExtensionDescription description)
    {
      int count = description.PositionalArguments.Count;
      DocumentCompositeNode node = parserContext.DocumentContext.CreateNode((ITypeId) typeId);
      IDocumentNodeReference documentNodeReference = (IDocumentNodeReference) new DocumentCompositeNodeReference(nodeReference, node);
      IConstructor constructor1 = (IConstructor) null;
      if (typeId.IsResolvable)
      {
        IList<IConstructor> constructors = typeId.GetConstructors();
        if (constructors.Count > 0)
        {
          int num = 0;
          foreach (IConstructor constructor2 in (IEnumerable<IConstructor>) constructors)
          {
            if (constructor2.Parameters.Count == count)
            {
              if (num == 0)
                constructor1 = constructor2;
              ++num;
            }
          }
          if (num == 0)
            parserContext.ReportError(XamlParseErrors.IncorrectNumberOfConstructorArguments(lineInformation, (ITypeId) typeId));
          else if (num <= 1)
            ;
        }
        else
          parserContext.ReportError(XamlParseErrors.NoAccessibleConstructor(lineInformation, (ITypeId) typeId));
      }
      DocumentNode[] documentNodeArray = constructor1 != null ? new DocumentNode[constructor1.Parameters.Count] : (DocumentNode[]) null;
      IConstructorArgumentProperties argumentProperties = typeId.GetConstructorArgumentProperties();
      if (count > 0)
      {
        for (int index = 0; index < count; ++index)
        {
          string str = description.PositionalArguments[index];
          bool flag = constructor1 != null && index < constructor1.Parameters.Count;
          IType type = parserContext.TypeResolver.ResolveType(parserContext.TypeResolver.PlatformMetadata.KnownTypes.Object);
          if (flag)
          {
            IParameter parameter = constructor1.Parameters[index];
            type = parserContext.TypeResolver.ResolveType((ITypeId) parameter.ParameterType);
            IProperty property = argumentProperties[parameter.Name];
            if (property != null && !property.PropertyType.IsAssignableFrom((ITypeId) type))
            {
              flag = false;
              parserContext.ReportError(XamlParseErrors.ConstructorArgumentDeclaredTypeDoesNotMatchProperty(lineInformation, (IPropertyId) property));
            }
          }
          TypeConverter typeConverter = type.TypeConverter;
          DocumentNode propertyValue = MarkupExtensionParser.GetPropertyValue(parserContext, lineInformation, documentNodeReference, xmlNamespaceResolver, typeConverter, type, str, true);
          if (flag && propertyValue != null)
            documentNodeArray[index] = XamlParser.CanAssignTo(parserContext, lineInformation, type, propertyValue) ? propertyValue : (DocumentNode) null;
        }
      }
      node.SetConstructor(constructor1, (IList<DocumentNode>) documentNodeArray);
      int num1 = 0;
      foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) description.NamedArguments)
      {
        string key = keyValuePair.Key;
        MemberType memberTypes = MemberType.LocalProperty | MemberType.AttachedProperty | MemberType.Field;
        if (parserContext.TypeResolver.IsCapabilitySet(PlatformCapability.IncompleteAttachedPropertiesInMarkupExtensions))
          memberTypes |= MemberType.IncompleteAttachedProperty;
        IProperty propertyKey = XamlTypeHelper.GetPropertyKey(parserContext, lineInformation, xmlNamespaceResolver, key, (XmlNamespace) null, typeId, memberTypes, MemberType.LocalProperty, false);
        if (propertyKey != null)
        {
          DocumentNode propertyValue = MarkupExtensionParser.GetPropertyValue(parserContext, lineInformation, documentNodeReference, xmlNamespaceResolver, propertyKey, keyValuePair.Value, true);
          if (propertyValue != null && XamlParser.SetProperty(parserContext, lineInformation, new DocumentCompositeNodeReference(documentNodeReference, node), (XamlSourceContext) null, (IPropertyId) propertyKey, propertyValue))
            propertyValue.ContainerSourceContext = (INodeSourceContext) new MarkupExtensionParser.MinimalSourceContext(num1++);
        }
      }
      return (DocumentNode) node;
    }

    public static bool GetTypeAndMemberName(XamlParserContext parserContext, ITextLocation lineInformation, IXmlNamespaceResolver xmlNamespaceResolver, string fullMemberName, IType targetTypeId, out IType typeId, out string memberName)
    {
      typeId = (IType) null;
      memberName = (string) null;
      XmlnsPrefix prefix;
      string typeName1;
      if (XamlTypeHelper.SplitTypeName(parserContext, lineInformation, fullMemberName, out prefix, out typeName1))
      {
        XmlNamespace xmlNamespace = parserContext.GetXmlNamespace(lineInformation, xmlNamespaceResolver, prefix);
        string typeName2;
        if (XamlTypeHelper.SplitMemberName(parserContext, lineInformation, typeName1, out typeName2, out memberName))
        {
          typeId = typeName2 == null ? targetTypeId : XamlTypeHelper.GetTypeId(parserContext, lineInformation, xmlNamespace, typeName2);
          if (typeId != null)
            return true;
          parserContext.ReportError(XamlParseErrors.CannotDetermineMemberTargetType(lineInformation, memberName));
        }
      }
      return false;
    }

    private static string GetRequiredMarkupExtensionArgument(XamlParserContext parserContext, ITextLocation lineInformation, MarkupExtensionDescription description, string propertyName)
    {
      if (description.PositionalArguments.Count == 1 && description.NamedArguments.Count == 0)
        return description.PositionalArguments[0];
      if (description.PositionalArguments.Count == 0 && description.NamedArguments.Count == 1 && description.NamedArguments[0].Key == propertyName)
        return description.NamedArguments[0].Value;
      parserContext.ReportError(XamlParseErrors.InvalidMarkupExtensionArguments(lineInformation, description.Name));
      return (string) null;
    }

    private static IProperty VerifyMemberIsDependencyProperty(XamlParserContext parserContext, ITextLocation lineInformation, IMember member)
    {
      IDependencyProperty dependencyProperty = member as IDependencyProperty;
      if (dependencyProperty != null)
        return (IProperty) dependencyProperty;
      if (member.IsResolvable)
      {
        IType declaringType = member.DeclaringType;
        Exception initializationException = declaringType.InitializationException;
        if (initializationException != null)
          parserContext.ReportError(XamlParseErrors.TypeInitializationException(lineInformation, (ITypeId) declaringType, initializationException.Message));
        else
          parserContext.ReportError(XamlParseErrors.MemberNotDependencyProperty(lineInformation, (IMemberId) member));
      }
      return (IProperty) null;
    }

    private static IEvent VerifyMemberIsRoutedEvent(XamlParserContext parserContext, ITextLocation lineInformation, IMember member)
    {
      IEvent @event = member as IEvent;
      if (@event != null && @event.IncludesRoutedEvent)
        return @event;
      if (member.IsResolvable)
      {
        IType declaringType = member.DeclaringType;
        Exception initializationException = declaringType.InitializationException;
        if (initializationException != null)
          parserContext.ReportError(XamlParseErrors.TypeInitializationException(lineInformation, (ITypeId) declaringType, initializationException.Message));
        else
          parserContext.ReportError(XamlParseErrors.MemberNotRoutedEvent(lineInformation, (IMemberId) member));
      }
      return (IEvent) null;
    }

    private enum ScannerState
    {
      Begin,
      ScanningMarkupExtensionName,
      ScanningNameOrValue,
      ScanningValue,
      End,
    }

    private class MinimalSourceContext : INodeSourceContext
    {
      private int offset;

      public bool IsCloned
      {
        get
        {
          return false;
        }
      }

      public int Ordering
      {
        get
        {
          return this.offset;
        }
      }

      public ITextRange TextRange
      {
        get
        {
          return (ITextRange) new TextRange(this.offset, this.offset);
        }
      }

      public ITextLocation TextLocation
      {
        get
        {
          return (ITextLocation) new TextLocation(this.offset, 0);
        }
      }

      public IReadableSelectableTextBuffer TextBuffer
      {
        get
        {
          return (IReadableSelectableTextBuffer) null;
        }
      }

      public MinimalSourceContext(int offset)
      {
        this.offset = offset;
      }

      public INodeSourceContext Clone(bool keepOldRanges)
      {
        return (INodeSourceContext) this;
      }

      public INodeSourceContext FreezeText(bool isClone)
      {
        return (INodeSourceContext) this;
      }
    }
  }
}
