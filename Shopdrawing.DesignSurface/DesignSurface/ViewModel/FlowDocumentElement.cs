// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.FlowDocumentElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects.TextObjects;
using System.Windows;
using System.Windows.Documents;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class FlowDocumentElement : SceneElement, ITextFlowSceneNode
  {
    public static readonly IPropertyId BlocksProperty = (IPropertyId) PlatformTypes.FlowDocument.GetMember(MemberType.LocalProperty, "Blocks", MemberAccessTypes.Public);
    public static readonly IPropertyId FontFamilyProperty = (IPropertyId) PlatformTypes.FlowDocument.GetMember(MemberType.LocalProperty, "FontFamily", MemberAccessTypes.Public);
    public static readonly IPropertyId FontSizeProperty = (IPropertyId) PlatformTypes.FlowDocument.GetMember(MemberType.LocalProperty, "FontSize", MemberAccessTypes.Public);
    public static readonly IPropertyId LineHeightProperty = (IPropertyId) PlatformTypes.FlowDocument.GetMember(MemberType.LocalProperty, "LineHeight", MemberAccessTypes.Public);
    public static readonly IPropertyId PagePaddingProperty = (IPropertyId) PlatformTypes.FlowDocument.GetMember(MemberType.LocalProperty, "PagePadding", MemberAccessTypes.Public);
    public static readonly FlowDocumentElement.ConcreteFlowDocumentElementFactory Factory = new FlowDocumentElement.ConcreteFlowDocumentElementFactory();

    private IViewFlowDocument FlowDocument
    {
      get
      {
        return this.ViewObject as IViewFlowDocument;
      }
    }

    public override bool IsSelectable
    {
      get
      {
        return false;
      }
    }

    public IViewTextPointer ContentStart
    {
      get
      {
        if (this.FlowDocument == null)
          return (IViewTextPointer) null;
        return this.FlowDocument.ContentStart;
      }
    }

    public IViewTextPointer ContentEnd
    {
      get
      {
        if (this.FlowDocument == null)
          return (IViewTextPointer) null;
        return this.FlowDocument.ContentEnd;
      }
    }

    public IPropertyId TextChildProperty
    {
      get
      {
        return FlowDocumentElement.BlocksProperty;
      }
    }

    public ISceneNodeCollection<SceneNode> TextFlowCollectionForTextChildProperty
    {
      get
      {
        return (ISceneNodeCollection<SceneNode>) new SceneNode.TextFlowSceneNodeCollection((BaseFrameworkElement) this.Parent, (SceneElement) this, (SceneElement) this, this.TextChildProperty);
      }
    }

    public ISceneNodeCollection<SceneNode> CollectionForTextChildProperty
    {
      get
      {
        return base.GetCollectionForProperty(this.TextChildProperty);
      }
    }

    public override ISceneNodeCollection<SceneNode> GetCollectionForProperty(IPropertyId childProperty)
    {
      if (this.TextChildProperty.Equals((object) childProperty) && this.Parent is ITextFlowSceneNode)
        return this.TextFlowCollectionForTextChildProperty;
      return base.GetCollectionForProperty(childProperty);
    }

    public IViewTextPointer GetPositionFromPoint(Point point)
    {
      ITextFlowSceneNode textFlowSceneNode = this.Parent as ITextFlowSceneNode;
      if (textFlowSceneNode != null)
        return textFlowSceneNode.GetPositionFromPoint(point);
      return this.ContentEnd;
    }

    public void AddInlineTextChild(Microsoft.Expression.DesignModel.DocumentModel.DocumentNode child)
    {
      this.InsertInlineTextChild(-1, child);
    }

    public void InsertInlineTextChild(int index, Microsoft.Expression.DesignModel.DocumentModel.DocumentNode child)
    {
      this.EnsureTextParent();
      DocumentCompositeNode documentCompositeNode1 = (DocumentCompositeNode) this.DocumentNode;
      DocumentCompositeNode documentCompositeNode2 = (DocumentCompositeNode) documentCompositeNode1.Properties[FlowDocumentElement.BlocksProperty];
      if (!documentCompositeNode2.SupportsChildren)
      {
        documentCompositeNode2 = this.DocumentContext.CreateNode(PlatformTypes.BlockCollection);
        documentCompositeNode1.Properties[FlowDocumentElement.BlocksProperty] = (Microsoft.Expression.DesignModel.DocumentModel.DocumentNode) documentCompositeNode2;
      }
      DocumentCompositeNode documentCompositeNode3 = (DocumentCompositeNode) null;
      if (documentCompositeNode2.Children.Count != 0)
        documentCompositeNode3 = (DocumentCompositeNode) documentCompositeNode2.Children[documentCompositeNode2.Children.Count - 1];
      if (documentCompositeNode3 == null || !PlatformTypes.Paragraph.IsAssignableFrom((ITypeId) documentCompositeNode3.Type))
      {
        documentCompositeNode3 = this.DocumentContext.CreateNode(PlatformTypes.Paragraph);
        documentCompositeNode2.Children.Add((Microsoft.Expression.DesignModel.DocumentModel.DocumentNode) documentCompositeNode3);
      }
      DocumentCompositeNode documentCompositeNode4 = (DocumentCompositeNode) documentCompositeNode3.Properties[ParagraphElement.InlinesProperty];
      if (documentCompositeNode4 == null || !documentCompositeNode4.SupportsChildren)
      {
        documentCompositeNode4 = this.DocumentContext.CreateNode(PlatformTypes.InlineCollection);
        documentCompositeNode3.Properties[ParagraphElement.InlinesProperty] = (Microsoft.Expression.DesignModel.DocumentModel.DocumentNode) documentCompositeNode4;
      }
      if (index == -1)
        documentCompositeNode4.Children.Add(child);
      else
        documentCompositeNode4.Children.Insert(index, child);
    }

    public SceneElement EnsureTextParent()
    {
      DocumentCompositeNode documentCompositeNode1 = this.DocumentNode as DocumentCompositeNode;
      DocumentCompositeNode documentCompositeNode2 = (DocumentCompositeNode) null;
      if (documentCompositeNode1 != null)
        documentCompositeNode2 = documentCompositeNode1.Properties[FlowDocumentElement.BlocksProperty] as DocumentCompositeNode;
      if (documentCompositeNode1 == null)
      {
        documentCompositeNode1 = this.DocumentContext.CreateNode(PlatformTypes.FlowDocument);
        this.DocumentNode.Parent.Properties[(IPropertyId) this.DocumentNode.SitePropertyKey] = (Microsoft.Expression.DesignModel.DocumentModel.DocumentNode) documentCompositeNode1;
      }
      if (documentCompositeNode2 == null)
      {
        DocumentCompositeNode documentCompositeNode3 = this.FlowDocument == null || this.FlowDocument.Blocks == null || this.FlowDocument.Blocks.Count <= 0 ? this.DocumentContext.CreateNode(typeof (BlockCollection)) : (DocumentCompositeNode) this.DocumentContext.CreateNode(typeof (BlockCollection), (object) this.FlowDocument.Blocks);
        documentCompositeNode1.Properties[FlowDocumentElement.BlocksProperty] = (Microsoft.Expression.DesignModel.DocumentModel.DocumentNode) documentCompositeNode3;
      }
      return (SceneElement) this;
    }

    public class ConcreteFlowDocumentElementFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new FlowDocumentElement();
      }
    }
  }
}
