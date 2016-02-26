// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.Xml.XmlNamespaceScope
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using System.Collections;
using System.Xml;

namespace Microsoft.Expression.DesignModel.Markup.Xml
{
  internal class XmlNamespaceScope
  {
    private XmlNamespaceScope parent;
    private Hashtable map;

    public XmlNamespaceScope(XmlNamespaceScope parent)
    {
      this.parent = parent;
    }

    public void AddNamespace(string prefix, string nsuri)
    {
      if (this.map == null)
        this.map = new Hashtable();
      if (this.map.Contains((object) prefix))
        return;
      this.map.Add((object) prefix, (object) nsuri);
    }

    public string LookupNamespace(string prefix)
    {
      if (this.map != null)
      {
        string str = (string) this.map[(object) prefix];
        if (str != null)
          return str;
      }
      if (this.parent != null)
        return this.parent.LookupNamespace(prefix);
      return (string) null;
    }

    public XmlNamespaceScope PushScope()
    {
      return new XmlNamespaceScope(this);
    }

    public XmlNamespaceScope PopScope()
    {
      return this.parent;
    }

    public void CopyLocalScope(XmlNamespaceScope scope)
    {
      if (this.map == null)
        return;
      foreach (string prefix in (IEnumerable) this.map.Keys)
      {
        string nsuri = (string) this.map[(object) prefix];
        scope.AddNamespace(prefix, nsuri);
      }
    }

    public void CopyLocalScope(XmlNamespaceManager mgr)
    {
      if (this.map == null)
        return;
      foreach (string prefix in (IEnumerable) this.map.Keys)
      {
        string uri = (string) this.map[(object) prefix];
        mgr.AddNamespace(prefix, uri);
      }
    }

    public string LookupPrefix(string uri, string def)
    {
      string str1 = (string) null;
      if (uri == StandardXmlIdentifiers.xmlnsUri.Name)
        return "xmlns";
      if (uri == StandardXmlIdentifiers.xmlURI.Name)
        return "xml";
      if (this.map != null)
      {
        foreach (DictionaryEntry dictionaryEntry in this.map)
        {
          string str2 = dictionaryEntry.Key as string;
          if (dictionaryEntry.Value as string == uri)
          {
            if (str2 == def)
              return str2;
            str1 = str2;
          }
        }
      }
      if (this.parent != null)
      {
        string str2 = this.parent.LookupPrefix(uri, def);
        if (str2 != null)
          return str2;
      }
      return str1;
    }

    public Hashtable GetNamespacesInScope()
    {
      Hashtable result = new Hashtable();
      this.GetNamespacesInScope(result);
      return result;
    }

    private void GetNamespacesInScope(Hashtable result)
    {
      if (this.map != null)
      {
        foreach (string str1 in (IEnumerable) this.map.Keys)
        {
          string str2 = (string) this.map[(object) str1];
          if (!result.Contains((object) str2))
            result.Add((object) str2, (object) str1);
        }
      }
      if (this.parent == null)
        return;
      this.parent.GetNamespacesInScope(result);
    }
  }
}
