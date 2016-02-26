// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Text.RichTextBoxEditProxyBase
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.InstanceBuilders;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignModel.ViewObjects.TextObjects;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.DesignSurface.ViewModel.Text;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Microsoft.Expression.DesignSurface.Tools.Text
{
  public abstract class RichTextBoxEditProxyBase : TextEditProxy
  {
    private static readonly IPropertyId BlockUIContainerChildProperty = (IPropertyId) PlatformTypes.BlockUIContainer.GetMember(MemberType.LocalProperty, "Child", MemberAccessTypes.Public);
    private static readonly IPropertyId InlineUIContainerChildProperty = (IPropertyId) PlatformTypes.InlineUIContainer.GetMember(MemberType.LocalProperty, "Child", MemberAccessTypes.Public);
    private static readonly IPropertyId ParagraphInlinesProperty = (IPropertyId) PlatformTypes.Paragraph.GetMember(MemberType.LocalProperty, "Inlines", MemberAccessTypes.Public);
    private static readonly IPropertyId SpanInlinesProperty = (IPropertyId) PlatformTypes.Span.GetMember(MemberType.LocalProperty, "Inlines", MemberAccessTypes.Public);
    private static readonly IPropertyId SectionBlocksProperty = (IPropertyId) PlatformTypes.Section.GetMember(MemberType.LocalProperty, "Blocks", MemberAccessTypes.Public);
    private DocumentCompositeNode flowDocumentNode;
    private DocumentCompositeNode inlinesCollectionNode;
    private DocumentCompositeNode blockCollectionNode;
    private IViewFlowDocumentEditor richTextBox;
    private Dictionary<object, Microsoft.Expression.DesignModel.DocumentModel.DocumentNode> instanceDictionary;

    public override bool SupportsRangeProperties
    {
      get
      {
        return JoltHelper.TypeSupported((ITypeResolver) this.TextSource.ViewModel.ProjectContext, PlatformTypes.Inline);
      }
    }

    public override bool SupportsParagraphProperties
    {
      get
      {
        return true;
      }
    }

    public override IViewTextBoxBase EditingElement
    {
      get
      {
        return (IViewTextBoxBase) this.richTextBox;
      }
    }

    protected IViewFlowDocumentEditor RichTextBox
    {
      get
      {
        return this.richTextBox;
      }
    }

    protected DocumentCompositeNode FlowDocumentNode
    {
      get
      {
        return this.flowDocumentNode;
      }
    }

    protected DocumentCompositeNode InlinesCollectionNode
    {
      get
      {
        return this.inlinesCollectionNode;
      }
    }

    protected DocumentCompositeNode BlockCollectionNode
    {
      get
      {
        return this.blockCollectionNode;
      }
    }

    public virtual RichTextSerializationType SerializationType
    {
      get
      {
        if (JoltHelper.TypeSupported((ITypeResolver) this.TextSource.ViewModel.ProjectContext, PlatformTypes.FlowDocument))
          return RichTextSerializationType.FlowDocument;
        if (JoltHelper.TypeSupported((ITypeResolver) this.TextSource.ViewModel.ProjectContext, PlatformTypes.BlockCollection))
          return RichTextSerializationType.BlockCollection;
        return JoltHelper.TypeSupported((ITypeResolver) this.TextSource.ViewModel.ProjectContext, PlatformTypes.InlineCollection) ? RichTextSerializationType.InlineCollection : RichTextSerializationType.None;
      }
    }

    public override DesignTimeTextSelection TextSelection
    {
      get
      {
        return new DesignTimeTextSelection((SceneElement) this.TextSource, this.richTextBox);
      }
    }

    protected RichTextBoxEditProxyBase(BaseFrameworkElement textSource)
      : base(textSource)
    {
      this.ProxyPlatform = textSource.DesignerContext.DesignerDefaultPlatformService.DefaultPlatform;
      this.richTextBox = (IViewFlowDocumentEditor) this.ProxyPlatform.ViewTextObjectFactory.CreateRichTextBox();
      this.instanceDictionary = new Dictionary<object, Microsoft.Expression.DesignModel.DocumentModel.DocumentNode>();
    }

    protected void UpdateUIChildrenInstances(IInstanceBuilderContext context)
    {
      this.UpdateUIChildrenInstancesInternal(context.ViewNodeManager.Root);
    }

    protected void UpdateUIChildrenInstances(DocumentCompositeNode rootNode, TextElement rootInstance)
    {
      BlockUIContainer blockUIContainerInstance = rootInstance as BlockUIContainer;
      if (blockUIContainerInstance != null && blockUIContainerInstance.Child != null)
        this.UpdateUIChildInstance(rootNode, blockUIContainerInstance);
      InlineUIContainer inlineUIContainerInstance = rootInstance as InlineUIContainer;
      if (inlineUIContainerInstance != null && inlineUIContainerInstance.Child != null)
        this.UpdateUIChildInstance(rootNode, inlineUIContainerInstance);
      Paragraph paragraph = rootInstance as Paragraph;
      if (paragraph != null && paragraph.Inlines != null)
      {
        DocumentCompositeNode documentCompositeNode = (DocumentCompositeNode) rootNode.Properties[RichTextBoxEditProxyBase.ParagraphInlinesProperty];
        int index = 0;
        foreach (Inline inline in (TextElementCollection<Inline>) paragraph.Inlines)
        {
          this.UpdateUIChildrenInstances((DocumentCompositeNode) documentCompositeNode.Children[index], (TextElement) inline);
          ++index;
        }
      }
      Section section = rootInstance as Section;
      if (section != null && section.Blocks != null)
      {
        DocumentCompositeNode documentCompositeNode = (DocumentCompositeNode) rootNode.Properties[RichTextBoxEditProxyBase.SectionBlocksProperty];
        int index = 0;
        foreach (Block block in (TextElementCollection<Block>) section.Blocks)
        {
          this.UpdateUIChildrenInstances((DocumentCompositeNode) documentCompositeNode.Children[index], (TextElement) block);
          ++index;
        }
      }
      Span span = rootInstance as Span;
      if (span == null || span.Inlines == null)
        return;
      DocumentCompositeNode documentCompositeNode1 = (DocumentCompositeNode) rootNode.Properties[RichTextBoxEditProxyBase.SpanInlinesProperty];
      int index1 = 0;
      foreach (Inline inline in (TextElementCollection<Inline>) span.Inlines)
      {
        this.UpdateUIChildrenInstances((DocumentCompositeNode) documentCompositeNode1.Children[index1], (TextElement) inline);
        ++index1;
      }
    }

    private void UpdateUIChildrenInstancesInternal(ViewNode root)
    {
      InlineUIContainer inlineUIContainerInstance = root.Instance as InlineUIContainer;
      if (inlineUIContainerInstance != null && inlineUIContainerInstance.Child != null)
        this.UpdateUIChildInstance((DocumentCompositeNode) root.DocumentNode, inlineUIContainerInstance);
      BlockUIContainer blockUIContainerInstance = root.Instance as BlockUIContainer;
      if (blockUIContainerInstance != null && blockUIContainerInstance.Child != null)
        this.UpdateUIChildInstance((DocumentCompositeNode) root.DocumentNode, blockUIContainerInstance);
      foreach (ViewNode root1 in (IEnumerable<ViewNode>) root.Children)
        this.UpdateUIChildrenInstancesInternal(root1);
      foreach (ViewNode root1 in (IEnumerable<ViewNode>) root.Properties.Values)
        this.UpdateUIChildrenInstancesInternal(root1);
    }

    private void UpdateUIChildInstance(DocumentCompositeNode inlineUIContainerNode, InlineUIContainer inlineUIContainerInstance)
    {
      Microsoft.Expression.DesignModel.DocumentModel.DocumentNode documentNode = inlineUIContainerNode.Properties[RichTextBoxEditProxyBase.InlineUIContainerChildProperty];
      this.instanceDictionary.Add((object) inlineUIContainerInstance.Child, documentNode);
    }

    private void UpdateUIChildInstance(DocumentCompositeNode blockUIContainerNode, BlockUIContainer blockUIContainerInstance)
    {
      Microsoft.Expression.DesignModel.DocumentModel.DocumentNode documentNode = blockUIContainerNode.Properties[RichTextBoxEditProxyBase.BlockUIContainerChildProperty];
      this.instanceDictionary.Add((object) blockUIContainerInstance.Child, documentNode);
    }

    protected override void CopyProperty(IPropertyId propertyId)
    {
      if (propertyId.Equals((object) this.ResolveProperty(RichTextBoxRangeElement.TextBlockLineHeightPropertyId)))
      {
        System.Windows.Controls.RichTextBox richTextBox = this.RichTextBox.PlatformSpecificObject as System.Windows.Controls.RichTextBox;
        object computedValueAsWpf = this.TextSource.GetComputedValueAsWpf(RichTextBoxRangeElement.TextBlockLineHeightPropertyId);
        if (richTextBox == null || !(computedValueAsWpf is double))
          return;
        double num = (double) computedValueAsWpf;
        TextBlock.SetLineHeight((DependencyObject) richTextBox, num);
      }
      else if (propertyId.Equals((object) this.ResolveProperty(RichTextBoxRangeElement.TextBlockTextAlignmentPropertyId)))
      {
        System.Windows.Controls.RichTextBox richTextBox = this.RichTextBox.PlatformSpecificObject as System.Windows.Controls.RichTextBox;
        object computedValueAsWpf = this.TextSource.GetComputedValueAsWpf(RichTextBoxRangeElement.TextBlockTextAlignmentPropertyId);
        if (richTextBox == null || !(computedValueAsWpf is TextAlignment))
          return;
        TextAlignment textAlignment = (TextAlignment) computedValueAsWpf;
        TextBlock.SetTextAlignment((DependencyObject) richTextBox, textAlignment);
      }
      else
        base.CopyProperty(propertyId);
    }

    public override bool IsTextChange(DocumentNodeChange change)
    {
      if (base.IsTextChange(change))
        return true;
      if (change.PropertyKey != null)
      {
        ReferenceStep singleStep = this.ResolveProperty((IPropertyId) change.PropertyKey) as ReferenceStep;
        if (singleStep != null)
          return RichTextBoxRangeElement.IsTextProperty((SceneNode) this.TextSource, new PropertyReference(singleStep));
      }
      return false;
    }

    public override void Serialize()
    {
      IDocumentContext documentContext = this.TextSource.DocumentContext;
      IPlatformTypes metadata = this.ProxyPlatform.Metadata;
      DependencyPropertyReferenceStep propertyReferenceStep = DesignTimeProperties.ResolveDesignTimeReferenceStep(DesignTimeProperties.UpdateContextProperty, (IPlatformMetadata) metadata);
      switch (this.SerializationType)
      {
        case RichTextSerializationType.FlowDocument:
          this.richTextBox.SetValue(metadata.DefaultTypeResolver, (IProperty) propertyReferenceStep, (object) this.instanceDictionary);
          this.flowDocumentNode = (DocumentCompositeNode) documentContext.CreateNode(typeof (FlowDocument), (object) (FlowDocument) this.richTextBox.Document.PlatformSpecificObject);
          this.richTextBox.ClearValue((IProperty) propertyReferenceStep);
          break;
        case RichTextSerializationType.BlockCollection:
          this.richTextBox.SetValue(metadata.DefaultTypeResolver, (IProperty) propertyReferenceStep, (object) this.instanceDictionary);
          this.blockCollectionNode = documentContext.CreateNode(PlatformTypes.BlockCollection);
          foreach (IViewBlock viewBlock in (IEnumerable<IViewBlock>) this.richTextBox.BlockContainer.Blocks)
          {
            IViewParagraph viewParagraph = viewBlock as IViewParagraph;
            if (viewParagraph == null)
            {
              IViewTextRange textRange = this.ProxyPlatform.ViewTextObjectFactory.CreateTextRange(this.richTextBox.BlockContainer, viewBlock.ContentStart, viewBlock.ContentEnd);
              viewParagraph = this.ProxyPlatform.ViewTextObjectFactory.CreateParagraph();
              IViewRun run = this.ProxyPlatform.ViewTextObjectFactory.CreateRun();
              run.Text = textRange.Text;
              viewParagraph.Inlines.Add((IViewInline) run);
            }
            this.blockCollectionNode.Children.Add(this.PostProcessDocumentNodeFromEditProxy(this.TextSource.ViewModel.DefaultView.ConvertFromWpfValueAsDocumentNode(viewParagraph.PlatformSpecificObject)));
          }
          this.richTextBox.ClearValue((IProperty) propertyReferenceStep);
          break;
        case RichTextSerializationType.InlineCollection:
          this.inlinesCollectionNode = documentContext.CreateNode(PlatformTypes.InlineCollection);
          using (IEnumerator<IViewBlock> enumerator = this.richTextBox.Document.Blocks.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              IViewBlock current = enumerator.Current;
              IViewParagraph viewParagraph = current as IViewParagraph;
              if (viewParagraph != null)
              {
                this.ProcessInlinesForSerialization(viewParagraph.Inlines);
                if (current.PlatformSpecificObject != ((FlowDocument) this.richTextBox.Document.PlatformSpecificObject).Blocks.LastBlock)
                  this.inlinesCollectionNode.Children.Add(this.TextSource.ViewModel.DefaultView.ConvertFromWpfValueAsDocumentNode((object) new LineBreak()));
              }
              else
              {
                Microsoft.Expression.DesignModel.DocumentModel.DocumentNode documentNode = this.TextSource.ViewModel.DefaultView.ConvertFromWpfValueAsDocumentNode((object) new Run(new TextRange(((TextElement) current.PlatformSpecificObject).ContentStart, ((TextElement) current.PlatformSpecificObject).ContentEnd).Text));
                if (documentNode != null)
                  this.inlinesCollectionNode.Children.Add(documentNode);
              }
            }
            break;
          }
      }
    }

    protected virtual Microsoft.Expression.DesignModel.DocumentModel.DocumentNode PostProcessDocumentNodeFromEditProxy(Microsoft.Expression.DesignModel.DocumentModel.DocumentNode node)
    {
      return node;
    }

    private void ProcessInlinesForSerialization(ICollection<IViewInline> inlineCollection)
    {
      IDocumentContext documentContext = this.TextSource.DocumentContext;
      foreach (IViewInline viewInline in (IEnumerable<IViewInline>) inlineCollection)
      {
        Microsoft.Expression.DesignModel.DocumentModel.DocumentNode documentNode = this.TextSource.ViewModel.DefaultView.ConvertFromWpfValueAsDocumentNode(viewInline.PlatformSpecificObject);
        if (documentNode != null)
        {
          this.inlinesCollectionNode.Children.Add(documentNode);
        }
        else
        {
          IViewSpan viewSpan;
          if ((viewSpan = viewInline as IViewSpan) != null)
            this.ProcessInlinesForSerialization(viewSpan.Inlines);
        }
      }
    }

    public override void DeleteSelection()
    {
      if (this.richTextBox.Selection.IsEmpty)
      {
        IViewTextPointer insertionPosition = this.richTextBox.Selection.End.GetNextInsertionPosition(LogicalDirection.Forward);
        if (insertionPosition != null)
          this.richTextBox.Selection.Select(this.richTextBox.Selection.Start, insertionPosition);
      }
      this.richTextBox.Selection.Text = string.Empty;
    }

    public override void SelectNone()
    {
      this.richTextBox.Selection.Select(this.richTextBox.Selection.Start, this.richTextBox.Selection.Start);
    }

    public static double AdjustPaddingByOffset(double padding, double offset)
    {
      if (!double.IsNaN(padding))
        return padding + offset;
      return offset;
    }

    public override void ApplyPropertyChange(DocumentNodeChange change)
    {
      base.ApplyPropertyChange(change);
      if (!RichTextBoxRangeElement.ShouldClearPropertyOnTextRuns((SceneNode) this.TextSource, new PropertyReference((ReferenceStep) change.PropertyKey)))
        return;
      this.ClearTextProperty(change.PropertyKey);
    }

    public void ClearTextProperty(IProperty property)
    {
      DependencyPropertyReferenceStep propertyReferenceStep = (DependencyPropertyReferenceStep) this.TypeResolver.ResolveProperty(this.TextSource.DesignerContext.PlatformConverter.ConvertToWpfPropertyKey(property));
      DependencyPropertyReferenceStep shadowProperty = DesignTimeProperties.GetShadowProperty((IProperty) propertyReferenceStep, (ITypeId) this.TextSource.Type);
      if (shadowProperty != null && DesignTimeProperties.UseShadowPropertyForInstanceBuilding(this.TypeResolver, (IPropertyId) shadowProperty))
        propertyReferenceStep = shadowProperty;
      DependencyProperty dependencyProperty = (DependencyProperty) propertyReferenceStep.DependencyProperty;
      foreach (TextElement textElement in (TextElementCollection<Block>) ((FlowDocument) this.RichTextBox.Document.PlatformSpecificObject).Blocks)
        this.ClearPropertyOnInlines(textElement, dependencyProperty);
      ((DependencyObject) this.richTextBox.Document.PlatformSpecificObject).ClearValue(dependencyProperty);
    }

    private void ClearPropertyOnInlines(TextElement textElement, DependencyProperty dependencyProperty)
    {
      textElement.ClearValue(dependencyProperty);
      foreach (object obj in LogicalTreeHelper.GetChildren((FrameworkContentElement) textElement))
      {
        TextElement textElement1 = obj as TextElement;
        if (textElement1 != null)
          this.ClearPropertyOnInlines(textElement1, dependencyProperty);
      }
    }
  }
}
