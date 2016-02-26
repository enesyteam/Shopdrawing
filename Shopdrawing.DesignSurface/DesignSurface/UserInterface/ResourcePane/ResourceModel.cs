// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.ResourcePane.ResourceModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.UserInterface.ResourcePane
{
  public class ResourceModel
  {
    private DocumentCompositeNode resourceNode;
    private DocumentNodeMarker marker;

    public string Name
    {
      get
      {
        if (this.KeyNode != null)
          return XamlExpressionSerializer.GetStringFromExpression(this.KeyNode, (DocumentNode) this.resourceNode);
        return string.Empty;
      }
    }

    public string Location
    {
      get
      {
        if (this.resourceNode.DocumentRoot != null)
          return DocumentReferenceLocator.GetDocumentReference(this.resourceNode.DocumentRoot.DocumentContext).DisplayName;
        return string.Empty;
      }
    }

    public IType Type
    {
      get
      {
        return this.ValueNode.Type;
      }
    }

    public System.Type TargetType
    {
      get
      {
        return this.ValueNode.TargetType;
      }
    }

    public DocumentNode KeyNode
    {
      get
      {
        return ResourceNodeHelper.GetResourceEntryKey(this.resourceNode);
      }
    }

    public DocumentNode ValueNode
    {
      get
      {
        return this.resourceNode.Properties[DictionaryEntryNode.ValueProperty];
      }
    }

    public DocumentCompositeNode ResourceNode
    {
      get
      {
        return this.resourceNode;
      }
    }

    public DocumentNodeMarker Marker
    {
      get
      {
        return this.marker;
      }
    }

    public ResourceModel(DocumentCompositeNode resourceNode)
    {
      this.resourceNode = resourceNode;
      this.marker = resourceNode.Marker;
    }

    public bool IsResourceReachable(SceneNode sourceNode)
    {
      if (!PlatformTypes.PlatformsCompatible(sourceNode.Type.PlatformMetadata, this.ResourceNode.PlatformMetadata))
        return false;
      DocumentNodePath documentNodePath = sourceNode.DocumentNodePath;
      return new ExpressionEvaluator(sourceNode.ViewModel.DocumentRootResolver).EvaluateResource(documentNodePath, this.KeyNode) == this.ValueNode;
    }
  }
}
