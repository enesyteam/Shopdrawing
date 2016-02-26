// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.DocumentProcessors.InvalidReferenceModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.DocumentProcessors
{
  public sealed class InvalidReferenceModel
  {
    public IPropertyId InvalidProperty { get; private set; }

    public DocumentNode InvalidValueNode { get; private set; }

    public string InvalidNodeName { get; private set; }

    public ITypeId InvalidNodeType { get; private set; }

    public string InvalidPropertyValue { get; private set; }

    public InvalidReferenceModel(DocumentCompositeNode invalidObjectNode, IPropertyId invalidProperty)
      : this(invalidObjectNode, invalidProperty, DocumentPrimitiveNode.GetValueAsString(invalidObjectNode.Properties[invalidProperty]))
    {
    }

    public InvalidReferenceModel(DocumentCompositeNode invalidObjectNode, IPropertyId invalidProperty, string invalidPropertyValue)
    {
      DocumentNode node = invalidObjectNode.Properties[invalidProperty];
      if (node != null)
        DocumentNodeHelper.GetNodeSpan(node);
      else
        DocumentNodeHelper.GetNodeSpan((DocumentNode) invalidObjectNode);
      this.InvalidValueNode = node ?? (DocumentNode) invalidObjectNode;
      this.InvalidNodeName = invalidObjectNode.Name;
      this.InvalidNodeType = (ITypeId) invalidObjectNode.Type;
      this.InvalidProperty = invalidProperty;
      this.InvalidPropertyValue = invalidPropertyValue;
    }
  }
}
