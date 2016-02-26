// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.XmlElementReference
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Markup.Xml;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Expression.DesignModel.Markup
{
  internal class XmlElementReference : XmlContainerReference, IXmlNamespaceResolver
  {
    private ITextRange startTagRange;
    private ITextRange initialStartTagSpan;
    private XmlSpace xmlSpace;
    private IList<XmlNamespace> ignorableNamespaces;
    private IList<XmlElementReference.Attribute> attributesToPreserve;
    private XmlContainerReference.ChildNode comment;

    public ITextRange StartTagRange
    {
      get
      {
        return this.startTagRange;
      }
      set
      {
        this.startTagRange = value;
      }
    }

    public ITextRange InitialStartTagSpan
    {
      get
      {
        return this.initialStartTagSpan;
      }
      set
      {
        this.initialStartTagSpan = value;
      }
    }

    public bool IsRootElement
    {
      get
      {
        return this.Parent is XmlDocumentReference;
      }
    }

    internal override XmlSpace XmlSpace
    {
      get
      {
        return this.xmlSpace;
      }
    }

    public IEnumerable<XmlNamespace> IgnorableNamespaces
    {
      get
      {
        return (IEnumerable<XmlNamespace>) this.ignorableNamespaces ?? Enumerable.Empty<XmlNamespace>();
      }
    }

    public IEnumerable<XmlElementReference.Attribute> AttributesToPreserve
    {
      get
      {
        return (IEnumerable<XmlElementReference.Attribute>) this.attributesToPreserve ?? Enumerable.Empty<XmlElementReference.Attribute>();
      }
    }

    public int AttributesToPreserveCount
    {
      get
      {
        if (this.attributesToPreserve == null)
          return 0;
        return this.attributesToPreserve.Count;
      }
    }

    internal XmlContainerReference.ChildNode Comment
    {
      get
      {
        return this.comment;
      }
      set
      {
        this.comment = value;
      }
    }

    internal XmlElementReference(XmlContainerReference parent, XmlElement xmlElement, bool beginTracking)
      : this(parent, beginTracking ? xmlElement.SourceContext : new SourceContext(), !beginTracking || xmlElement.EndTagContext.EndCol <= xmlElement.EndTagContext.StartCol ? new SourceContext() : xmlElement.StartTagContext, beginTracking)
    {
    }

    internal XmlElementReference(XmlContainerReference parent, SourceContext sourceContext, SourceContext startTagContext, bool beginTracking)
      : base(parent, parent.TextBuffer, sourceContext, beginTracking)
    {
      if (beginTracking && startTagContext.EndCol > startTagContext.StartCol)
        this.startTagRange = XamlSourceContext.ToTextRange(this.TextBuffer, startTagContext);
      this.xmlSpace = parent.XmlSpace;
    }

    internal void SetXmlSpace(XmlSpace xmlSpace)
    {
      this.xmlSpace = xmlSpace;
    }

    public XmlElementReference.Attribute GetAttributeToPreserveAt(int index)
    {
      return this.attributesToPreserve[index];
    }

    public void RemoveAttributeToPreserveAt(int index)
    {
      this.attributesToPreserve.RemoveAt(index);
    }

    public bool RemoveMatchingAttributes(Func<XmlElementReference.Attribute, bool> match)
    {
      bool flag = false;
      if (this.attributesToPreserve != null)
      {
        for (int index = this.attributesToPreserve.Count - 1; index >= 0; --index)
        {
          if (match(this.attributesToPreserve[index]))
          {
            this.attributesToPreserve.RemoveAt(index);
            flag = true;
          }
        }
      }
      return flag;
    }

    public void AddIgnorableNamespace(XmlNamespace xmlNamespace)
    {
      if (this.ignorableNamespaces == null)
        this.ignorableNamespaces = (IList<XmlNamespace>) new List<XmlNamespace>();
      this.ignorableNamespaces.Add(xmlNamespace);
    }

    public void AddAttributeToPreserve(XmlElementReference.Attribute attribute)
    {
      if (this.attributesToPreserve == null)
        this.attributesToPreserve = (IList<XmlElementReference.Attribute>) new List<XmlElementReference.Attribute>();
      this.attributesToPreserve.Add(attribute);
    }

    internal override void CloneCopy(XamlSourceContext other, bool keepOldRanges)
    {
      base.CloneCopy(other, keepOldRanges);
      XmlElementReference elementReference = (XmlElementReference) other;
      elementReference.comment = this.Comment == null ? (XmlContainerReference.ChildNode) null : new XmlContainerReference.ChildNode(this.Comment);
      elementReference.attributesToPreserve = this.attributesToPreserve == null ? (IList<XmlElementReference.Attribute>) null : (IList<XmlElementReference.Attribute>) new List<XmlElementReference.Attribute>((IEnumerable<XmlElementReference.Attribute>) this.attributesToPreserve);
      elementReference.ignorableNamespaces = this.ignorableNamespaces == null ? (IList<XmlNamespace>) null : (IList<XmlNamespace>) new List<XmlNamespace>((IEnumerable<XmlNamespace>) this.ignorableNamespaces);
      elementReference.xmlSpace = this.xmlSpace;
      elementReference.initialStartTagSpan = this.initialStartTagSpan;
      if (!keepOldRanges)
        return;
      elementReference.startTagRange = this.startTagRange;
    }

    public override INodeSourceContext Clone(bool keepOldRanges)
    {
      XmlElementReference elementReference = new XmlElementReference(this.Parent, new SourceContext(), new SourceContext(), false);
      this.CloneCopy((XamlSourceContext) elementReference, keepOldRanges);
      return (INodeSourceContext) elementReference;
    }

    public override INodeSourceContext FreezeText(bool isClone)
    {
      if (this.TextBuffer.Length != 0)
        return (INodeSourceContext) new RemovedXmlElementReference(this, isClone);
      return (INodeSourceContext) null;
    }

    XmlNamespace IXmlNamespaceResolver.GetXmlNamespace(XmlnsPrefix prefix, XmlNamespaceCanonicalization canonicalization)
    {
      for (XmlElementReference elementReference = this; elementReference != null; elementReference = elementReference.Parent as XmlElementReference)
      {
        foreach (XmlElementReference.Attribute attribute in elementReference.AttributesToPreserve)
        {
          if ((attribute.Type == XmlElementReference.AttributeType.Xmlns || attribute.Type == XmlElementReference.AttributeType.SerializerAddedXmlns) && string.Equals(string.IsNullOrEmpty(attribute.XmlAttribute.Prefix) ? attribute.XmlAttribute.Prefix : attribute.XmlAttribute.LocalName, prefix.Value, StringComparison.Ordinal))
            return XmlNamespace.ToNamespace(attribute.XmlAttribute.Value.Value, canonicalization);
        }
      }
      return (XmlNamespace) null;
    }

    bool IXmlNamespaceResolver.IsIgnorable(XmlNamespace xmlNamespace)
    {
      for (XmlElementReference elementReference = this; elementReference != null; elementReference = elementReference.Parent as XmlElementReference)
      {
        if (elementReference.ignorableNamespaces != null && elementReference.ignorableNamespaces.Contains(xmlNamespace))
          return true;
      }
      return false;
    }

    public enum AttributeType
    {
      XmlLang,
      XmlSpace,
      Xmlns,
      Compatibility,
      Ignored,
      XamlNamespace,
      SerializerAddedXmlns,
    }

    public sealed class Attribute
    {
      private readonly XmlElementReference.AttributeType type;
      private readonly XmlAttribute attribute;
      private XamlSourceContext sourceContext;

      public XmlElementReference.AttributeType Type
      {
        get
        {
          return this.type;
        }
      }

      internal XmlAttribute XmlAttribute
      {
        get
        {
          return this.attribute;
        }
      }

      internal XamlSourceContext SourceContext
      {
        get
        {
          return this.sourceContext;
        }
        set
        {
          this.sourceContext = value;
        }
      }

      internal Attribute(XmlElementReference.AttributeType type, XmlAttribute attribute, XamlSourceContext sourceContext)
      {
        this.type = type;
        this.sourceContext = sourceContext;
        this.attribute = new XmlAttribute(XmlElementReference.Attribute.CloneIdentifier(attribute.Name));
        this.attribute.SourceContext = XmlElementReference.Attribute.CloneSourceContext(attribute.SourceContext);
        this.attribute.AddChild((Microsoft.Expression.DesignModel.Markup.Xml.Node) new Literal(attribute.Value.Value, XmlElementReference.Attribute.CloneSourceContext(attribute.Value.SourceContext)));
      }

      private static Identifier CloneIdentifier(Identifier identifier)
      {
        if (identifier == null)
          return (Identifier) null;
        ComplexIdentifier complexIdentifier = new ComplexIdentifier(identifier.Name);
        if (identifier.Prefix != null)
          complexIdentifier.Prefix = XmlElementReference.Attribute.CloneIdentifier(identifier.Prefix);
        if (identifier.NamespaceURI != null)
          complexIdentifier.NamespaceURI = XmlElementReference.Attribute.CloneIdentifier(identifier.NamespaceURI);
        return (Identifier) complexIdentifier;
      }

      private static SourceContext CloneSourceContext(SourceContext sourceContext)
      {
        return new SourceContext((Document) null, sourceContext.StartCol, sourceContext.EndCol);
      }
    }
  }
}
