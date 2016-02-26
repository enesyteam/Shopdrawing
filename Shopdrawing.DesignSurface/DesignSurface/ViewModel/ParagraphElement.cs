// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.ParagraphElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects.TextObjects;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class ParagraphElement : TextElementSceneElement
  {
    public static readonly IPropertyId InlinesProperty = (IPropertyId) PlatformTypes.Paragraph.GetMember(MemberType.LocalProperty, "Inlines", MemberAccessTypes.Public);
    public static readonly IPropertyId TextIndentProperty = (IPropertyId) PlatformTypes.Paragraph.GetMember(MemberType.LocalProperty, "TextIndent", MemberAccessTypes.Public);
    public static readonly IPropertyId TextAlignmentProperty = (IPropertyId) PlatformTypes.Paragraph.GetMember(MemberType.LocalProperty, "TextAlignment", MemberAccessTypes.Public);
    public static readonly ParagraphElement.ConcreteParagraphElementFactory Factory = new ParagraphElement.ConcreteParagraphElementFactory();

    public IViewParagraph Paragraph
    {
      get
      {
        return (IViewParagraph) this.ViewObject;
      }
    }

    public override SceneElement EnsureTextParent()
    {
      DocumentCompositeNode documentCompositeNode1 = (DocumentCompositeNode) this.DocumentNode;
      DocumentCompositeNode documentCompositeNode2 = documentCompositeNode1.Properties[ParagraphElement.InlinesProperty] as DocumentCompositeNode;
      if (documentCompositeNode2 == null || !documentCompositeNode2.SupportsChildren)
      {
        object platformSpecificInlines = this.Paragraph.PlatformSpecificInlines;
        DocumentCompositeNode documentCompositeNode3 = platformSpecificInlines != null ? (DocumentCompositeNode) this.DocumentContext.CreateNode(platformSpecificInlines.GetType(), platformSpecificInlines) : this.DocumentContext.CreateNode(PlatformTypes.InlineCollection);
        documentCompositeNode1.Properties[ParagraphElement.InlinesProperty] = (DocumentNode) documentCompositeNode3;
      }
      return (SceneElement) this;
    }

    public override void AddInlineTextChild(DocumentNode child)
    {
      this.EnsureTextParent();
      (((DocumentCompositeNode) this.DocumentNode).Properties[ParagraphElement.InlinesProperty] as DocumentCompositeNode).Children.Add(child);
    }

    public class ConcreteParagraphElementFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new ParagraphElement();
      }
    }
  }
}
