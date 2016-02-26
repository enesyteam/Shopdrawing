// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.DictionaryEntrySourceContextReference
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using Microsoft.Expression.DesignModel.DocumentModel;

namespace Microsoft.Expression.DesignModel.Markup
{
  internal sealed class DictionaryEntrySourceContextReference : SourceContextReference
  {
    private DocumentNode valueNode;

    public override XamlSourceContext SourceContext
    {
      get
      {
        return this.valueNode.SourceContext as XamlSourceContext;
      }
      set
      {
        if (value != null)
        {
          XamlSourceContext xamlSourceContext = (XamlSourceContext) value.Clone(false);
          xamlSourceContext.Parent = (XmlContainerReference) value;
          this.SetSourceContext(this.valueNode, (INodeSourceContext) xamlSourceContext);
        }
        else
          this.SetSourceContext(this.valueNode, (INodeSourceContext) null);
        this.SetSourceContext(this.DocumentNode, (INodeSourceContext) value);
      }
    }

    public DictionaryEntrySourceContextReference(DocumentCompositeNode dictionaryEntryNode, DocumentNode valueNode)
      : base((DocumentNode) dictionaryEntryNode)
    {
      this.valueNode = valueNode;
    }
  }
}
