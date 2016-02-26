// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.TextBlockElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignModel.ViewObjects.TextObjects;
using System;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public sealed class TextBlockElement : BaseFrameworkElement, ITextFlowSceneNode
  {
    public static readonly IPropertyId TextProperty = (IPropertyId) PlatformTypes.TextBlock.GetMember(MemberType.LocalProperty, "Text", MemberAccessTypes.Public);
    public static readonly IPropertyId TextAlignmentProperty = (IPropertyId) PlatformTypes.TextBlock.GetMember(MemberType.LocalProperty, "TextAlignment", MemberAccessTypes.Public);
    public static readonly IPropertyId TextDecorationsProperty = (IPropertyId) PlatformTypes.TextBlock.GetMember(MemberType.LocalProperty, "TextDecorations", MemberAccessTypes.Public);
    public static readonly IPropertyId InlinesProperty = (IPropertyId) PlatformTypes.TextBlock.GetMember(MemberType.LocalProperty, "Inlines", MemberAccessTypes.Public);
    public static readonly IPropertyId TextWrappingProperty = (IPropertyId) PlatformTypes.TextBlock.GetMember(MemberType.LocalProperty, "TextWrapping", MemberAccessTypes.Public);
    public static readonly IPropertyId ForegroundProperty = (IPropertyId) PlatformTypes.TextBlock.GetMember(MemberType.LocalProperty, "Foreground", MemberAccessTypes.Public);
    public static readonly IPropertyId BackgroundProperty = (IPropertyId) PlatformTypes.TextBlock.GetMember(MemberType.LocalProperty, "Background", MemberAccessTypes.Public);
    public static readonly IPropertyId LineHeightProperty = (IPropertyId) PlatformTypes.TextBlock.GetMember(MemberType.LocalProperty, "LineHeight", MemberAccessTypes.Public);
    public static readonly IPropertyId FontSizeProperty = (IPropertyId) PlatformTypes.TextBlock.GetMember(MemberType.LocalProperty, "FontSize", MemberAccessTypes.Public);
    public static readonly IPropertyId FontFamilyProperty = (IPropertyId) PlatformTypes.TextBlock.GetMember(MemberType.LocalProperty, "FontFamily", MemberAccessTypes.Public);
    public static readonly IPropertyId FontWeightProperty = (IPropertyId) PlatformTypes.TextBlock.GetMember(MemberType.LocalProperty, "FontWeight", MemberAccessTypes.Public);
    public static readonly IPropertyId FontStyleProperty = (IPropertyId) PlatformTypes.TextBlock.GetMember(MemberType.LocalProperty, "FontStyle", MemberAccessTypes.Public);
    public static readonly IPropertyId LineStackingStrategyProperty = (IPropertyId) PlatformTypes.TextBlock.GetMember(MemberType.LocalProperty, "LineStackingStrategy", MemberAccessTypes.Public);
    public static readonly IPropertyId PaddingProperty = (IPropertyId) PlatformTypes.TextBlock.GetMember(MemberType.LocalProperty, "Padding", MemberAccessTypes.Public);
    public static readonly TextBlockElement.ConcreteTextBlockElementFactory Factory = new TextBlockElement.ConcreteTextBlockElementFactory();

    public override bool IsContainer
    {
      get
      {
        return (bool) this.ViewModel.ProjectContext.GetCapabilityValue(PlatformCapability.SupportsTextBlockInlineUIContainer);
      }
    }

    public IViewTextPointer ContentStart
    {
      get
      {
        return this.TextBlock.ContentStart;
      }
    }

    public IViewTextPointer ContentEnd
    {
      get
      {
        return this.TextBlock.ContentEnd;
      }
    }

    private IViewTextBlock TextBlock
    {
      get
      {
        return this.ViewObject as IViewTextBlock;
      }
    }

    public ISceneNodeCollection<SceneNode> TextFlowCollectionForTextChildProperty
    {
      get
      {
        return (ISceneNodeCollection<SceneNode>) new SceneNode.TextFlowSceneNodeCollection((BaseFrameworkElement) this, (SceneElement) this, (SceneElement) this, this.TextChildProperty);
      }
    }

    public ISceneNodeCollection<SceneNode> CollectionForTextChildProperty
    {
      get
      {
        return base.GetCollectionForProperty(this.TextChildProperty);
      }
    }

    public IPropertyId TextChildProperty
    {
      get
      {
        return TextBlockElement.InlinesProperty;
      }
    }

    public override ISceneNodeCollection<SceneNode> GetCollectionForProperty(IPropertyId childProperty)
    {
      if (this.TextChildProperty.Equals((object) childProperty) && (bool) this.ViewModel.ProjectContext.GetCapabilityValue(PlatformCapability.SupportsTextBlockInlineUIContainer))
        return this.TextFlowCollectionForTextChildProperty;
      return base.GetCollectionForProperty(childProperty);
    }

    public IViewTextPointer GetPositionFromPoint(Point point)
    {
      return this.TextBlock.GetPositionFromPoint(point);
    }

    public SceneElement EnsureTextParent()
    {
      DocumentCompositeNode documentCompositeNode1 = (DocumentCompositeNode) this.DocumentNode;
      DocumentCompositeNode documentCompositeNode2 = documentCompositeNode1.Properties[TextBlockElement.InlinesProperty] as DocumentCompositeNode;
      if (documentCompositeNode2 == null || !documentCompositeNode2.SupportsChildren)
      {
        Type runtimeType = this.ProjectContext.ResolveType(PlatformTypes.InlineCollection).RuntimeType;
        DocumentCompositeNode documentCompositeNode3 = this.TextBlock == null ? this.DocumentContext.CreateNode(runtimeType) : (DocumentCompositeNode) this.DocumentContext.CreateNode(runtimeType, this.TextBlock.Inlines);
        documentCompositeNode1.Properties[TextBlockElement.InlinesProperty] = (DocumentNode) documentCompositeNode3;
        documentCompositeNode1.Properties[TextBlockElement.TextProperty] = (DocumentNode) null;
      }
      return (SceneElement) this;
    }

    public void AddInlineTextChild(DocumentNode child)
    {
      this.EnsureTextParent();
      ((DocumentCompositeNode) ((DocumentCompositeNode) this.DocumentNode).Properties[TextBlockElement.InlinesProperty]).Children.Add(child);
    }

    public void InsertInlineTextChild(int index, DocumentNode child)
    {
      this.EnsureTextParent();
      ((DocumentCompositeNode) ((DocumentCompositeNode) this.DocumentNode).Properties[TextBlockElement.InlinesProperty]).Children.Insert(index, child);
    }

    protected override void ModifyValue(PropertyReference propertyReference, object valueToSet, SceneNode.Modification modification, int index)
    {
      if (propertyReference.FirstStep.Equals((object) TextBlockElement.TextProperty))
      {
        ISceneNodeCollection<SceneNode> collectionForProperty = this.GetCollectionForProperty(TextBlockElement.InlinesProperty);
        if (collectionForProperty != null)
          collectionForProperty.Clear();
      }
      base.ModifyValue(propertyReference, valueToSet, modification, index);
    }

    public class ConcreteTextBlockElementFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new TextBlockElement();
      }
    }
  }
}
