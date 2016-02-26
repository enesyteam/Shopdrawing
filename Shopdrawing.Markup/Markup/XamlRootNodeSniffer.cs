// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.XamlRootNodeSniffer
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using System.IO;
using System.Xml;

namespace Microsoft.Expression.DesignModel.Markup
{
  public static class XamlRootNodeSniffer
  {
    public static ITypeId SniffRootNodeType(Stream stream, IDocumentContext documentContext, out string xamlClassAttribute)
    {
      xamlClassAttribute = (string) null;
      try
      {
        ITypeResolver typeResolver = documentContext.TypeResolver;
        using (XmlReader xmlReader = XmlReader.Create(stream))
        {
          ClrNamespaceUriParseCache documentNamespaces = new ClrNamespaceUriParseCache(typeResolver);
          while (xmlReader.Read())
          {
            if (xmlReader.MoveToContent() == XmlNodeType.Element)
            {
              xamlClassAttribute = xmlReader.GetAttribute("Class", "http://schemas.microsoft.com/winfx/2006/xaml");
              string str = xmlReader.LookupNamespace(xmlReader.Prefix);
              if (!string.IsNullOrEmpty(str))
                return (ITypeId) XamlTypeHelper.GetTypeId(typeResolver, documentNamespaces, XmlNamespace.ToNamespace(str, XmlNamespace.GetNamespaceCanonicalization(typeResolver)), xmlReader.LocalName, false, false);
            }
          }
        }
      }
      catch (XmlException ex)
      {
      }
      return (ITypeId) null;
    }
  }
}
