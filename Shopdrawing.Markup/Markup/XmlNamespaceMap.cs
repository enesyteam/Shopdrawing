// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.XmlNamespaceMap
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using Microsoft.Expression.DesignModel.Metadata;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignModel.Markup
{
  internal sealed class XmlNamespaceMap : IEnumerable<KeyValuePair<XmlnsPrefix, IXmlNamespace>>, IEnumerable
  {
    private Dictionary<XmlnsPrefix, IXmlNamespace> dictionary;

    public XmlNamespaceMap()
    {
      this.dictionary = new Dictionary<XmlnsPrefix, IXmlNamespace>();
    }

    public XmlnsPrefix GetPrefix(IXmlNamespace xmlNamespace)
    {
      foreach (KeyValuePair<XmlnsPrefix, IXmlNamespace> keyValuePair in this.dictionary)
      {
        if (keyValuePair.Value.Equals((object) xmlNamespace))
          return keyValuePair.Key;
      }
      return (XmlnsPrefix) null;
    }

    public IXmlNamespace GetNamespace(XmlnsPrefix prefix)
    {
      IXmlNamespace xmlNamespace;
      this.dictionary.TryGetValue(prefix, out xmlNamespace);
      return xmlNamespace;
    }

    public void AddNamespace(XmlnsPrefix prefix, IXmlNamespace xmlNamespace)
    {
      this.dictionary[prefix] = xmlNamespace;
    }

    public IEnumerator<KeyValuePair<XmlnsPrefix, IXmlNamespace>> GetEnumerator()
    {
      return (IEnumerator<KeyValuePair<XmlnsPrefix, IXmlNamespace>>) this.dictionary.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.GetEnumerator();
    }
  }
}
