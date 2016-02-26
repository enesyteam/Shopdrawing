// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.XmlContainerReference
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using Microsoft.Expression.DesignModel.Markup.Xml;
using Microsoft.Expression.DesignModel.Text;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Expression.DesignModel.Markup
{
  internal abstract class XmlContainerReference : XamlSourceContext
  {
    private IList<XmlContainerReference.ChildNode> childNodesToPreserve;

    internal abstract XmlSpace XmlSpace { get; }

    public IEnumerable<XmlContainerReference.ChildNode> ChildNodesToPreserve
    {
      get
      {
        return (IEnumerable<XmlContainerReference.ChildNode>) this.childNodesToPreserve ?? Enumerable.Empty<XmlContainerReference.ChildNode>();
      }
    }

    public int ChildNodesToPreserveCount
    {
      get
      {
        if (this.childNodesToPreserve == null)
          return 0;
        return this.childNodesToPreserve.Count;
      }
    }

    protected XmlContainerReference(XmlContainerReference parent, IReadableSelectableTextBuffer textBuffer, SourceContext sourceContext, bool beginTracking)
      : base(parent, textBuffer, sourceContext, beginTracking)
    {
    }

    public XmlContainerReference.ChildNode GetChildNodeAt(int index)
    {
      return this.childNodesToPreserve[index];
    }

    public void AddChildNode(XmlContainerReference.ChildNode childNode)
    {
      if (this.childNodesToPreserve == null)
        this.childNodesToPreserve = (IList<XmlContainerReference.ChildNode>) new List<XmlContainerReference.ChildNode>();
      this.childNodesToPreserve.Add(childNode);
    }

    public void RemoveChildNodeAt(int index)
    {
      this.childNodesToPreserve.RemoveAt(index);
    }

    internal override void CloneCopy(XamlSourceContext other, bool keepOldRanges)
    {
      XmlContainerReference containerReference = (XmlContainerReference) other;
      if (this.ChildNodesToPreserveCount > 0)
      {
        foreach (XmlContainerReference.ChildNode childNode in this.ChildNodesToPreserve)
          containerReference.AddChildNode(childNode);
      }
      base.CloneCopy(other, keepOldRanges);
    }

    public enum ChildType
    {
      Declaration,
      ProcessingInstruction,
      Comment,
      Code,
      Ignored,
    }

    public sealed class ChildNode
    {
      private readonly XmlContainerReference.ChildType type;
      private XamlSourceContext sourceContext;
      private string text;

      public XmlContainerReference.ChildType Type
      {
        get
        {
          return this.type;
        }
      }

      public string Text
      {
        get
        {
          return this.text;
        }
      }

      public XamlSourceContext SourceContext
      {
        get
        {
          return this.sourceContext;
        }
      }

      public ChildNode(XmlContainerReference.ChildType type, XmlContainerReference parent, Microsoft.Expression.DesignModel.Markup.Xml.Node node)
      {
        this.type = type;
        this.sourceContext = (XamlSourceContext) new UnprocessedNodeReference(parent, node.SourceContext);
        this.text = node.SourceContext.SourceText;
      }

      public ChildNode(XmlContainerReference.ChildNode other)
      {
        this.type = other.type;
        this.text = other.text;
      }
    }
  }
}
