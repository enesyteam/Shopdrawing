// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.DefaultXmlNamespaceResolver
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignModel.Markup
{
  internal sealed class DefaultXmlNamespaceResolver : IXmlNamespaceResolver
  {
    public static readonly DefaultXmlNamespaceResolver Instance = new DefaultXmlNamespaceResolver();

    private DefaultXmlNamespaceResolver()
    {
    }

    public XmlNamespace GetXmlNamespace(XmlnsPrefix prefix, XmlNamespaceCanonicalization canonicalization)
    {
      if (prefix == XmlnsPrefix.EmptyPrefix)
        return XmlNamespace.AvalonXmlNamespace;
      if (prefix.Value == "x")
        return XmlNamespace.XamlXmlNamespace;
      return (XmlNamespace) null;
    }

    public bool IsIgnorable(XmlNamespace xmlNamespace)
    {
      return false;
    }
  }
}
