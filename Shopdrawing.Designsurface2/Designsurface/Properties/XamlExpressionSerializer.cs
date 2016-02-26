// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Properties.XamlExpressionSerializer
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Markup;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Expression.DesignSurface.Properties
{
  public static class XamlExpressionSerializer
  {
    public static string GetStringFromExpression(DocumentNode expression, DocumentNode parentNode)
    {
      if (parentNode != null)
      {
        XamlDocument xamlDocument = parentNode.DocumentRoot as XamlDocument;
        if (xamlDocument != null && xamlDocument.DocumentContext != null)
        {
          string str = XamlSerializer.SerializeValue((IDocumentRoot) xamlDocument, (IXamlSerializerFilter) new DefaultXamlSerializerFilter(), expression, CultureInfo.CurrentCulture);
          if (str != null)
            return str;
        }
      }
      return string.Empty;
    }

    public static DocumentNode GetExpressionFromString(string text, DocumentNode parentNode, Type propertyType, out IList<XamlParseError> errors)
    {
      if (parentNode != null)
      {
        XamlDocument document = parentNode.DocumentRoot as XamlDocument;
        if (document != null)
          return XamlExpressionSerializer.GetExpressionFromString(text, document, parentNode, propertyType, out errors);
      }
      errors = (IList<XamlParseError>) null;
      return (DocumentNode) null;
    }

    public static DocumentNode GetExpressionFromString(string text, XamlDocument document, DocumentNode parentNode, Type propertyType, out IList<XamlParseError> errors)
    {
      if (parentNode != null)
      {
        IType type = document.TypeResolver.GetType(propertyType);
        return XamlParser.ParseValue((IDocumentRoot) document, parentNode, type, text, out errors);
      }
      errors = (IList<XamlParseError>) null;
      return (DocumentNode) null;
    }

    public static string GetUserFriendlyDescription(DocumentCompositeNode expressionNode, DocumentNode parentNode)
    {
      if (DocumentNodeUtilities.IsDynamicResource((DocumentNode) expressionNode) || DocumentNodeUtilities.IsStaticResource((DocumentNode) expressionNode))
      {
        DocumentNode resourceKey = ResourceNodeHelper.GetResourceKey(expressionNode);
        DocumentPrimitiveNode documentPrimitiveNode = resourceKey as DocumentPrimitiveNode;
        DocumentCompositeNode expressionNode1 = resourceKey as DocumentCompositeNode;
        if (documentPrimitiveNode != null && documentPrimitiveNode.Value != null)
          return documentPrimitiveNode.Value.ToString();
        if (expressionNode1 != null && DocumentNodeUtilities.IsMarkupExtension((DocumentNode) expressionNode1))
          return XamlExpressionSerializer.GetUserFriendlyDescription(expressionNode1, (DocumentNode) expressionNode);
      }
      else if (DocumentNodeUtilities.IsStaticResource((DocumentNode) expressionNode))
      {
        DocumentPrimitiveNode documentPrimitiveNode = expressionNode.Properties[StaticExtensionProperties.MemberProperty] as DocumentPrimitiveNode;
        if (documentPrimitiveNode != null)
          return documentPrimitiveNode.Value.ToString();
      }
      return XamlExpressionSerializer.GetStringFromExpression((DocumentNode) expressionNode, parentNode);
    }
  }
}
