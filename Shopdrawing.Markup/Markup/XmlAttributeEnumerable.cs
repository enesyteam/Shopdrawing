// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.XmlAttributeEnumerable
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using Microsoft.Expression.DesignModel.Markup.Xml;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignModel.Markup
{
  internal struct XmlAttributeEnumerable : IEnumerable<XmlAttribute>, IEnumerable
  {
    private XmlElement element;
    private Predicate<XmlAttribute> predicate;

    public XmlAttributeEnumerable(XmlElement element, Predicate<XmlAttribute> predicate)
    {
      this.element = element;
      this.predicate = predicate;
    }

    public static bool All(XmlAttribute attribute)
    {
      return true;
    }

    public static bool NotXmlns(XmlAttribute attribute)
    {
      return !XmlUtilities.IsXmlnsDeclaration(attribute);
    }

    public static bool NotDirective(XmlAttribute attribute)
    {
      return XmlUtilities.GetProcessingAttributeType(attribute) == XmlProcessingAttributeType.NotProcessingAttribute;
    }

    public static bool NotXmlnsNorDirective(XmlAttribute attribute)
    {
      if (XmlAttributeEnumerable.NotXmlns(attribute))
        return XmlAttributeEnumerable.NotDirective(attribute);
      return false;
    }

    public static XmlAttributeEnumerable All(XmlElement element)
    {
      return new XmlAttributeEnumerable(element, new Predicate<XmlAttribute>(XmlAttributeEnumerable.All));
    }

    public static XmlAttributeEnumerable NotXmlnsNorDirective(XmlElement element)
    {
      return new XmlAttributeEnumerable(element, new Predicate<XmlAttribute>(XmlAttributeEnumerable.NotXmlnsNorDirective));
    }

    public XmlAttributeEnumerator GetEnumerator()
    {
      return new XmlAttributeEnumerator(this.element, this.predicate);
    }

    IEnumerator<XmlAttribute> IEnumerable<XmlAttribute>.GetEnumerator()
    {
      return (IEnumerator<XmlAttribute>) this.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.GetEnumerator();
    }
  }
}
