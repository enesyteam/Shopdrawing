// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.ImageBrushNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class ImageBrushNode : TileBrushNode
  {
    public static readonly IPropertyId ImageSourceProperty = (IPropertyId) PlatformTypes.ImageBrush.GetMember(MemberType.LocalProperty, "ImageSource", MemberAccessTypes.Public);
    public static readonly ImageBrushNode.ConcreteImageBrushNodeFactory Factory = new ImageBrushNode.ConcreteImageBrushNodeFactory();

    public SceneNode ImageSource
    {
      get
      {
        return this.GetLocalValueAsSceneNode(ImageBrushNode.ImageSourceProperty);
      }
      set
      {
        this.SetValueAsSceneNode(ImageBrushNode.ImageSourceProperty, value);
      }
    }

    public string Source
    {
      get
      {
        DocumentPrimitiveNode documentPrimitiveNode = (DocumentPrimitiveNode) null;
        DocumentNodePath valueAsDocumentNode = this.GetLocalValueAsDocumentNode(ImageBrushNode.ImageSourceProperty);
        if (valueAsDocumentNode != null)
          documentPrimitiveNode = valueAsDocumentNode.Node as DocumentPrimitiveNode;
        if (documentPrimitiveNode != null)
          return documentPrimitiveNode.GetValue<string>();
        return (string) null;
      }
      set
      {
        DocumentNode documentNode = (DocumentNode) this.DocumentNode.Context.CreateNode(PlatformTypes.ImageSource, (IDocumentNodeValue) new DocumentNodeStringValue(value));
        if (documentNode == null)
          return;
        this.SetValue(ImageBrushNode.ImageSourceProperty, (object) documentNode);
      }
    }

    public class ConcreteImageBrushNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new ImageBrushNode();
      }
    }
  }
}
