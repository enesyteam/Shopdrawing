// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.XmlAttributeEnumerator
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using Microsoft.Expression.DesignModel.Markup.Xml;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignModel.Markup
{
  internal struct XmlAttributeEnumerator : IEnumerator<XmlAttribute>, IDisposable, IEnumerator
  {
    private static Microsoft.Expression.DesignModel.Markup.Xml.Node sentinelNode = (Microsoft.Expression.DesignModel.Markup.Xml.Node) new XmlAttributeEnumerator.SentinelNode();
    private XmlElement element;
    private Predicate<XmlAttribute> predicate;
    private Microsoft.Expression.DesignModel.Markup.Xml.Node currentAttribute;

    public XmlAttribute Current
    {
      get
      {
        return (XmlAttribute) this.currentAttribute;
      }
    }

    object IEnumerator.Current
    {
      get
      {
        return (object) this.Current;
      }
    }

    public XmlAttributeEnumerator(XmlElement element, Predicate<XmlAttribute> predicate)
    {
      this.element = element;
      this.predicate = predicate;
      this.currentAttribute = XmlAttributeEnumerator.sentinelNode;
    }

    public void Dispose()
    {
    }

    public bool MoveNext()
    {
      for (this.currentAttribute = this.currentAttribute == XmlAttributeEnumerator.sentinelNode ? this.element.FirstAttribute : this.currentAttribute.NextNode; this.currentAttribute != null; this.currentAttribute = this.currentAttribute.NextNode)
      {
        if (this.predicate((XmlAttribute) this.currentAttribute))
          return true;
      }
      return false;
    }

    public void Reset()
    {
      this.currentAttribute = XmlAttributeEnumerator.sentinelNode;
    }

    private class SentinelNode : Microsoft.Expression.DesignModel.Markup.Xml.Node
    {
      public SentinelNode()
        : base(NodeType.Error)
      {
      }
    }
  }
}
