// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.RichTextBoxElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignModel.ViewObjects.TextObjects;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.ViewModel.Text;
using Microsoft.Expression.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public sealed class RichTextBoxElement : BaseTextElement, ITextFlowSceneNode
  {
    public static readonly IPropertyId CaretBrushProperty = (IPropertyId) PlatformTypes.RichTextBox.GetMember(MemberType.LocalProperty, "CaretBrush", MemberAccessTypes.Public);
    public static readonly IPropertyId VerticalScrollBarVisibilityProperty = (IPropertyId) PlatformTypes.RichTextBox.GetMember(MemberType.LocalProperty, "VerticalScrollBarVisibility", MemberAccessTypes.Public);
    public static readonly IPropertyId HorizontalScrollBarVisibilityProperty = (IPropertyId) PlatformTypes.RichTextBox.GetMember(MemberType.LocalProperty, "HorizontalScrollBarVisibility", MemberAccessTypes.Public);
    public static readonly IPropertyId DocumentProperty = (IPropertyId) PlatformTypes.RichTextBox.GetMember(MemberType.LocalProperty, "Document", MemberAccessTypes.Public);
    public static readonly IPropertyId BlocksProperty = (IPropertyId) PlatformTypes.RichTextBox.GetMember(MemberType.LocalProperty, "Blocks", MemberAccessTypes.Public);
    public static readonly IPropertyId TextWrappingProperty = (IPropertyId) PlatformTypes.RichTextBox.GetMember(MemberType.LocalProperty, "TextWrapping", MemberAccessTypes.Public);
    public static readonly IPropertyId TextAlignmentProperty = (IPropertyId) PlatformTypes.RichTextBox.GetMember(MemberType.LocalProperty, "TextAlignment", MemberAccessTypes.Public);
    public static readonly RichTextBoxElement.ConcreteRichTextBoxElementFactory Factory = new RichTextBoxElement.ConcreteRichTextBoxElementFactory();
    private ReadOnlyCollection<IProperty> richTextBoxProperties;

    private IPropertyId[] ParagraphProperties
    {
      get
      {
        if (this.ProjectContext.IsCapabilitySet(PlatformCapability.IsWpf))
          return RichTextBoxParagraphsRangeElement.WpfTextParagraphProperties;
        return RichTextBoxParagraphsRangeElement.SilverlightTextParagraphProperties;
      }
    }

    public IPropertyId TextChildProperty
    {
      get
      {
        if (this.UsingFlowDocumentIndirection)
          return RichTextBoxElement.DocumentProperty;
        return RichTextBoxElement.BlocksProperty;
      }
    }

    private bool UsingFlowDocumentIndirection
    {
      get
      {
        return this.ProjectContext.IsCapabilitySet(PlatformCapability.SupportsRichTextBoxFlowDocument);
      }
    }

    public FlowDocumentElement FlowDocumentElement
    {
      get
      {
        if (!this.ProjectContext.IsCapabilitySet(PlatformCapability.SupportsRichTextBoxFlowDocument))
          return (FlowDocumentElement) null;
        return this.GetLocalValueAsSceneNode(RichTextBoxElement.DocumentProperty) as FlowDocumentElement;
      }
    }

    private IViewRichTextBox RichTextBox
    {
      get
      {
        return this.ViewObject as IViewRichTextBox;
      }
    }

    private IViewTextRange TextRange
    {
      get
      {
        if (this.RichTextBox == null)
          return (IViewTextRange) null;
        return this.RichTextBox.EntireRange;
      }
    }

    private RichTextBoxRangeElement TextRangeElement
    {
      get
      {
        return RichTextBoxRangeElement.Factory.Instantiate(this.TextRange, this.ViewModel, PlatformTypes.TextElement);
      }
    }

    public override bool IsContainer
    {
      get
      {
        return true;
      }
    }

    public override string Text
    {
      get
      {
        if (this.RichTextBox == null)
          return "";
        return this.TextRange.Text;
      }
      set
      {
        if (this.RichTextBox == null)
          return;
        DocumentCompositeNode documentCompositeNode = (DocumentCompositeNode) this.DocumentNode;
        documentCompositeNode.Properties[this.TextChildProperty] = (DocumentNode) null;
        IDocumentContext context = documentCompositeNode.Context;
        IViewRun run = this.Platform.ViewTextObjectFactory.CreateRun();
        run.Text = value;
        DocumentNode node = context.CreateNode(run.PlatformSpecificObject.GetType(), run.PlatformSpecificObject);
        this.EnsureTextParent();
        this.DefaultContent.Clear();
        this.AddInlineTextChild(node);
      }
    }

    public IViewTextSelection Selection
    {
      get
      {
        if (this.RichTextBox == null)
          return (IViewTextSelection) null;
        return this.RichTextBox.Selection;
      }
    }

    public IViewTextPointer ContentStart
    {
      get
      {
        if (this.RichTextBox == null)
          return (IViewTextPointer) null;
        return this.RichTextBox.BlockContainer.ContentStart;
      }
    }

    public IViewTextPointer ContentEnd
    {
      get
      {
        if (this.RichTextBox == null)
          return (IViewTextPointer) null;
        return this.RichTextBox.BlockContainer.ContentEnd;
      }
    }

    public ISceneNodeCollection<SceneNode> TextFlowCollectionForTextChildProperty
    {
      get
      {
        if (this.UsingFlowDocumentIndirection)
          return (ISceneNodeCollection<SceneNode>) new SceneNode.TextFlowSceneNodeCollection((BaseFrameworkElement) this, (SceneElement) this.FlowDocumentElement, (SceneElement) this, FlowDocumentElement.BlocksProperty);
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

    public override ISceneNodeCollection<SceneNode> GetCollectionForProperty(IPropertyId childProperty)
    {
      if (this.TextChildProperty.Equals((object) childProperty))
        return this.TextFlowCollectionForTextChildProperty;
      return base.GetCollectionForProperty(childProperty);
    }

    private bool ShouldForwardPropertyRequestsToRange(PropertyReference propertyReference)
    {
      if (this.ProjectContext.IsCapabilitySet(PlatformCapability.IsWpf) && (propertyReference[0].Equals((object) this.Platform.Metadata.ResolveProperty(RichTextBoxParagraphsRangeElement.TextAlignmentProperty)) || propertyReference[0].Equals((object) this.Platform.Metadata.ResolveProperty(RichTextBoxParagraphsRangeElement.AttachedTextAlignmentProperty)) || propertyReference[0].Equals((object) this.Platform.Metadata.ResolveProperty(RichTextBoxRangeElement.TextBlockTextAlignmentPropertyId)) || propertyReference[0].Equals((object) this.Platform.Metadata.ResolveProperty(RichTextBoxRangeElement.AttachedTextBlockTextAlignmentPropertyId)) || (propertyReference[0].Equals((object) this.Platform.Metadata.ResolveProperty(RichTextBoxParagraphsRangeElement.LineHeightProperty)) || propertyReference[0].Equals((object) this.Platform.Metadata.ResolveProperty(RichTextBoxParagraphsRangeElement.AttachedLineHeightProperty)))) || !this.IsViewObjectValid)
        return false;
      return RichTextBoxElement.IsParagraphProperty(propertyReference);
    }

    public override PropertyState IsSet(PropertyReference propertyReference)
    {
      if (this.ShouldForwardPropertyRequestsToRange(propertyReference))
        return this.TextRangeElement.IsValueSetOnTextRange(this.TextRange, propertyReference);
      return base.IsSet(propertyReference);
    }

    public override void InsertValue(PropertyReference propertyReference, int index, object valueToAdd)
    {
      if (this.ShouldForwardPropertyRequestsToRange(propertyReference))
      {
        this.TextRangeElement.InsertTextValue(this.TextRange, propertyReference, index, valueToAdd);
        this.UpdateDocumentModel();
      }
      else
        base.InsertValue(propertyReference, index, valueToAdd);
    }

    public override void SetValue(PropertyReference propertyReference, object valueToSet)
    {
      if (this.ShouldForwardPropertyRequestsToRange(propertyReference))
      {
        this.TextRangeElement.SetTextValue(this.TextRange, propertyReference, valueToSet);
        this.UpdateDocumentModel();
      }
      else
        base.SetValue(propertyReference, valueToSet);
    }

    public override void RemoveValueAt(PropertyReference propertyReference, int index)
    {
      if (this.ShouldForwardPropertyRequestsToRange(propertyReference))
      {
        this.TextRangeElement.RemoveTextValueAt(this.TextRange, propertyReference, index);
        this.UpdateDocumentModel();
      }
      else
        base.RemoveValueAt(propertyReference, index);
    }

    protected override object GetComputedValueInternal(PropertyReference propertyReference)
    {
      if (this.ShouldForwardPropertyRequestsToRange(propertyReference))
        return this.TextRangeElement.GetTextValue(this.TextRange, propertyReference);
      return base.GetComputedValueInternal(propertyReference);
    }

    public override void ClearValue(PropertyReference propertyReference)
    {
      if (this.ShouldForwardPropertyRequestsToRange(propertyReference))
      {
        this.TextRangeElement.ClearTextValue(this.TextRange, propertyReference);
        this.UpdateDocumentModel();
      }
      else
        base.ClearValue(propertyReference);
    }

    public override ReadOnlyCollection<IProperty> GetProperties()
    {
      if (this.richTextBoxProperties == null)
      {
        List<IProperty> list = new List<IProperty>();
        foreach (IProperty property in base.GetProperties())
          list.Add(property);
        foreach (IPropertyId propertyId in this.ParagraphProperties)
        {
          IProperty property = this.ProjectContext.ResolveProperty(propertyId);
          if (!list.Contains(property))
            list.Add(property);
        }
        this.richTextBoxProperties = new ReadOnlyCollection<IProperty>((IList<IProperty>) list);
      }
      return this.richTextBoxProperties;
    }

    public void AddInlineTextChild(DocumentNode child)
    {
      this.InsertInlineTextChild(-1, child);
    }

    public void InsertInlineTextChild(int index, DocumentNode child)
    {
      this.EnsureTextParent();
      if (this.UsingFlowDocumentIndirection)
      {
        if (index == -1)
          this.FlowDocumentElement.AddInlineTextChild(child);
        else
          this.FlowDocumentElement.InsertInlineTextChild(index, child);
      }
      else
      {
        DocumentCompositeNode documentCompositeNode1 = (DocumentCompositeNode) ((DocumentCompositeNode) this.DocumentNode).Properties[this.TextChildProperty];
        DocumentCompositeNode documentCompositeNode2 = (DocumentCompositeNode) null;
        if (documentCompositeNode1.Children.Count != 0)
          documentCompositeNode2 = (DocumentCompositeNode) documentCompositeNode1.Children[documentCompositeNode1.Children.Count - 1];
        if (documentCompositeNode2 == null || !PlatformTypes.Paragraph.IsAssignableFrom((ITypeId) documentCompositeNode2.Type))
        {
          documentCompositeNode2 = this.DocumentContext.CreateNode(PlatformTypes.Paragraph);
          documentCompositeNode1.Children.Add((DocumentNode) documentCompositeNode2);
        }
        DocumentCompositeNode documentCompositeNode3 = (DocumentCompositeNode) documentCompositeNode2.Properties[ParagraphElement.InlinesProperty];
        if (documentCompositeNode3 == null || !documentCompositeNode3.SupportsChildren)
        {
          documentCompositeNode3 = this.DocumentContext.CreateNode(PlatformTypes.InlineCollection);
          documentCompositeNode2.Properties[ParagraphElement.InlinesProperty] = (DocumentNode) documentCompositeNode3;
        }
        if (index == -1)
          documentCompositeNode3.Children.Add(child);
        else
          documentCompositeNode3.Children.Insert(index, child);
      }
    }

    public IViewTextPointer GetPositionFromPoint(Point point)
    {
      if (this.RichTextBox == null)
        return (IViewTextPointer) null;
      return this.RichTextBox.GetPositionFromPoint(point);
    }

    protected override void UpdateDocumentModelInternal()
    {
      if (this.RichTextBox == null)
        return;
      DocumentCompositeNode documentCompositeNode1 = (DocumentCompositeNode) this.DocumentNode;
      IDocumentContext context = documentCompositeNode1.Context;
      this.EnsureTextParent();
      object platformSpecificObject = this.RichTextBox.BlockContainer.PlatformSpecificObject;
      if (this.UsingFlowDocumentIndirection)
      {
        DocumentCompositeNode documentCompositeNode2 = (DocumentCompositeNode) context.CreateNode(platformSpecificObject.GetType(), platformSpecificObject);
        DocumentNode documentNode = documentCompositeNode2.Properties[FlowDocumentElement.BlocksProperty];
        documentCompositeNode2.Properties[FlowDocumentElement.BlocksProperty] = (DocumentNode) context.CreateNode(PlatformTypes.BlockCollection);
        ((DocumentCompositeNode) documentCompositeNode1.Properties[this.TextChildProperty]).Properties[FlowDocumentElement.BlocksProperty] = documentNode;
      }
      else
        documentCompositeNode1.Properties[this.TextChildProperty] = context.CreateNode(platformSpecificObject.GetType(), platformSpecificObject);
    }

    internal override object GetTextValueAtPoint(Point point, bool snapToText, PropertyReference propertyReference)
    {
      IViewTextPointer positionFromPoint = this.RichTextBox.GetPositionFromPoint(point);
      PropertyReference propertyReference1 = this.DesignerContext.PropertyManager.FilterProperty((ITypeResolver) this.ProjectContext, positionFromPoint.Parent.GetIType((ITypeResolver) this.ProjectContext), propertyReference);
      if (propertyReference1 == null)
        return this.GetComputedValue(propertyReference);
      object textPointerValue = RichTextBoxRangeElement.GetTextPointerValue(positionFromPoint, propertyReference1);
      if (DependencyProperty.UnsetValue == textPointerValue)
        return MixedProperty.Mixed;
      return textPointerValue;
    }

    public static bool IsParagraphProperty(PropertyReference propertyReference)
    {
      IPropertyId[] array = !propertyReference.PlatformMetadata.IsCapabilitySet(PlatformCapability.IsWpf) ? RichTextBoxParagraphsRangeElement.SilverlightTextParagraphProperties : RichTextBoxParagraphsRangeElement.WpfTextParagraphProperties;
      bool flag = false;
      if (Array.IndexOf<IPropertyId>(array, (IPropertyId) propertyReference[0]) != -1)
        flag = true;
      return flag;
    }

    public SceneElement EnsureTextParent()
    {
      DocumentCompositeNode documentCompositeNode = (DocumentCompositeNode) this.DocumentNode;
      DocumentNode documentNode1 = documentCompositeNode.Properties[this.TextChildProperty];
      if (this.UsingFlowDocumentIndirection)
      {
        if (documentNode1 == null || !PlatformTypes.FlowDocument.IsAssignableFrom((ITypeId) documentNode1.Type))
        {
          DocumentNode documentNode2 = (DocumentNode) documentCompositeNode.Context.CreateNode(PlatformTypes.FlowDocument);
          documentCompositeNode.Properties[this.TextChildProperty] = documentNode2;
        }
        return (SceneElement) this.FlowDocumentElement;
      }
      if (documentNode1 == null || !PlatformTypes.BlockCollection.IsAssignableFrom((ITypeId) documentNode1.Type))
      {
        DocumentNode documentNode2 = (DocumentNode) documentCompositeNode.Context.CreateNode(PlatformTypes.BlockCollection);
        documentCompositeNode.Properties[RichTextBoxElement.BlocksProperty] = documentNode2;
      }
      return (SceneElement) this;
    }

    public class ConcreteRichTextBoxElementFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new RichTextBoxElement();
      }
    }
  }
}
