// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Text.TextBlockEditProxy
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.InstanceBuilders;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Text
{
  public class TextBlockEditProxy : RichTextBoxEditProxyBase
  {
    public override bool SupportsParagraphProperties
    {
      get
      {
        return false;
      }
    }

    public override RichTextSerializationType SerializationType
    {
      get
      {
        if (JoltHelper.TypeSupported((ITypeResolver) this.TextSource.ViewModel.ProjectContext, PlatformTypes.FlowDocument))
          return RichTextSerializationType.FlowDocument;
        return JoltHelper.TypeSupported((ITypeResolver) this.TextSource.ViewModel.ProjectContext, PlatformTypes.InlineCollection) ? RichTextSerializationType.InlineCollection : RichTextSerializationType.None;
      }
    }

    public TextBlockEditProxy(BaseFrameworkElement textSource)
      : base(textSource)
    {
    }

    public override void Instantiate()
    {
      base.Instantiate();
      this.RichTextBox.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
      this.RichTextBox.SetValue(this.EditingElement.Platform.Metadata.DefaultTypeResolver, ControlElement.ForegroundProperty, this.TextSource.GetComputedValueAsWpf(TextBlockElement.ForegroundProperty));
      if (this.EditingElement.Platform.Metadata.ResolveProperty(TextBlockElement.BackgroundProperty) != null)
        this.RichTextBox.SetValue(this.EditingElement.Platform.Metadata.DefaultTypeResolver, ControlElement.BackgroundProperty, this.TextSource.GetComputedValueAsWpf(TextBlockElement.BackgroundProperty));
      TextAlignment textAlignment = (TextAlignment) this.TextSource.GetComputedValueAsWpf(TextBlockElement.TextAlignmentProperty);
      TextDecorationCollection decorationCollection = (TextDecorationCollection) this.TextSource.GetComputedValueAsWpf(TextBlockElement.TextDecorationsProperty);
      Paragraph paragraph = (Paragraph) ((FlowDocument) this.RichTextBox.Document.PlatformSpecificObject).Blocks.FirstBlock;
      paragraph.TextAlignment = textAlignment;
      paragraph.TextDecorations = decorationCollection;
      if (this.TextSource.IsSet(TextBlockElement.TextProperty) == PropertyState.Set)
      {
        Run run = new Run((string) this.TextSource.GetComputedValue(TextBlockElement.TextProperty));
        run.FlowDirection = (FlowDirection) FrameworkElement.FlowDirectionProperty.DefaultMetadata.DefaultValue;
        paragraph.Inlines.Add((Inline) run);
      }
      else if (this.TextSource.IsSet(TextBlockElement.InlinesProperty) == PropertyState.Set)
      {
        DocumentNodePath pathInContainer = this.TextSource.DocumentNodePath.GetPathInContainer(((DocumentCompositeNode) this.TextSource.DocumentNode).Properties[TextBlockElement.InlinesProperty]);
        SceneViewModel viewModel = this.TextSource.ViewModel;
        IDocumentContext documentContext = viewModel.Document.DocumentContext;
        if (!viewModel.ProjectContext.IsCapabilitySet(PlatformCapability.IsWpf))
        {
          foreach (SceneNode sceneNode in (IEnumerable<SceneNode>) this.TextSource.GetCollectionForProperty(TextBlockElement.InlinesProperty))
          {
            Inline inline = this.TextSource.ViewModel.DefaultView.ConvertToWpfValue((object) sceneNode.DocumentNode) as Inline;
            if (inline != null)
              paragraph.Inlines.Add(inline);
          }
        }
        else
        {
          using (InstanceBuilderContext instanceBuilderContext = new InstanceBuilderContext(viewModel.ProjectContext, viewModel, true, (Microsoft.Expression.DesignModel.DocumentModel.DocumentNode) null))
          {
            using (instanceBuilderContext.DisablePostponedResourceEvaluation())
            {
              instanceBuilderContext.ViewNodeManager.RootNodePath = pathInContainer;
              instanceBuilderContext.ViewNodeManager.Root.Instance = (object) paragraph.Inlines;
              instanceBuilderContext.ViewNodeManager.Instantiate(instanceBuilderContext.ViewNodeManager.Root);
            }
            this.UpdateUIChildrenInstances((IInstanceBuilderContext) instanceBuilderContext);
          }
        }
      }
      if (this.ForceLoadOnInstantiate)
      {
        this.EditingElement.UpdateLayout();
        this.AdjustPadding();
      }
      else
        this.RichTextBox.Loaded += new RoutedEventHandler(this.RichTextBox_Loaded);
      if (this.TextSource.ViewModel.ProjectContext.IsCapabilitySet(PlatformCapability.SupportsTabInTextControl))
      {
        RichTextBox richTextBox = this.RichTextBox.PlatformSpecificObject as RichTextBox;
        if (richTextBox != null)
          richTextBox.AddHandler(CommandManager.PreviewCanExecuteEvent, (Delegate) new CanExecuteRoutedEventHandler(TextBlockEditProxy.CanExecute));
      }
      this.RichTextBox.SelectionChanged += new RoutedEventHandler(this.RichTextBox_SelectionChanged);
    }

    public void UpdateCaretBrush()
    {
      RichTextBox richTextBox = this.RichTextBox.PlatformSpecificObject as RichTextBox;
      if (richTextBox == null)
        return;
      Brush brush = richTextBox.Selection.GetPropertyValue(TextElement.ForegroundProperty) as Brush;
      if (brush == null)
        return;
      richTextBox.CaretBrush = brush;
    }

    private void RichTextBox_SelectionChanged(object sender, RoutedEventArgs e)
    {
      this.UpdateCaretBrush();
    }

    private static void CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      if (e.Command != EditingCommands.IncreaseIndentation && e.Command != EditingCommands.DecreaseIndentation)
        return;
      e.CanExecute = false;
      e.ContinueRouting = false;
      e.Handled = true;
    }

    protected override void CopyProperty(IPropertyId propertyId)
    {
      base.CopyProperty(propertyId);
      if (!propertyId.Equals((object) this.ResolveProperty(TextBlockElement.ForegroundProperty)))
        return;
      this.UpdateCaretBrush();
    }

    private void RichTextBox_Loaded(object sender, RoutedEventArgs e)
    {
      this.RichTextBox.Loaded -= new RoutedEventHandler(this.RichTextBox_Loaded);
      this.AdjustPadding();
    }

    private void AdjustPadding()
    {
      Thickness thickness = (Thickness) this.TextSource.GetComputedValueAsWpf(TextBlockElement.PaddingProperty);
      thickness = new Thickness(RichTextBoxEditProxyBase.AdjustPaddingByOffset(thickness.Left, 0.0), RichTextBoxEditProxyBase.AdjustPaddingByOffset(thickness.Top, 0.0), RichTextBoxEditProxyBase.AdjustPaddingByOffset(thickness.Right, -1.0), RichTextBoxEditProxyBase.AdjustPaddingByOffset(thickness.Bottom, -1.0));
      this.RichTextBox.Padding = thickness;
      this.RichTextBox.BorderThickness = new Thickness(0.0);
      this.RichTextBox.Document.PagePadding = new Thickness(0.0);
      this.RichTextBox.Document.LineStackingStrategy = (LineStackingStrategy) this.TextSource.GetComputedValueAsWpf(TextBlockElement.LineStackingStrategyProperty);
      double num = (double) this.TextSource.GetComputedValue(TextBlockElement.LineHeightProperty);
      if (FlowDocument.LineHeightProperty.IsValidValue((object) num))
        this.RichTextBox.Document.LineHeight = num;
      Style style = new Style(typeof (Paragraph));
      style.Setters.Add((SetterBase) new Setter(Block.MarginProperty, (object) new Thickness(0.0)));
      style.Seal();
      ((FrameworkElement) this.RichTextBox.PlatformSpecificObject).Resources.Add((object) typeof (Paragraph), (object) style);
    }

    public override void ApplyPropertyChange(DocumentNodeChange change)
    {
      if (change.PropertyKey != null && change.PropertyKey.Equals((object) TextBlockElement.LineHeightProperty))
      {
        double num = (double) this.TextSource.GetComputedValue(TextBlockElement.LineHeightProperty);
        if (FlowDocument.LineHeightProperty.IsValidValue((object) num))
          this.RichTextBox.Document.LineHeight = num;
      }
      base.ApplyPropertyChange(change);
    }

    public override void UpdateDocumentModel()
    {
      if (this.ShouldSerializeAsText())
        this.SerializeAsTextProperty();
      else
        this.SerializeAsInlines();
    }

    public override bool IsTextChange(DocumentNodeChange change)
    {
      Microsoft.Expression.DesignModel.DocumentModel.DocumentNode documentNode = ((DocumentCompositeNode) this.TextSource.DocumentNode).Properties[TextBlockElement.InlinesProperty];
      if ((change.PropertyKey == null || !change.PropertyKey.Equals((object) TextBlockElement.TextProperty) && !change.PropertyKey.Equals((object) TextBlockElement.InlinesProperty) && !change.PropertyKey.Equals((object) TextBlockElement.LineHeightProperty)) && (documentNode == null || !documentNode.IsAncestorOf((Microsoft.Expression.DesignModel.DocumentModel.DocumentNode) change.ParentNode)))
        return base.IsTextChange(change);
      return true;
    }

    private bool ShouldSerializeAsText()
    {
      DocumentCompositeNode documentCompositeNode1 = (DocumentCompositeNode) this.TextSource.DocumentNode;
      DocumentCompositeNode documentCompositeNode2 = this.InlinesCollectionNode;
      if (this.FlowDocumentNode != null)
      {
        DocumentCompositeNode documentCompositeNode3 = (DocumentCompositeNode) this.FlowDocumentNode.Properties[FlowDocumentElement.BlocksProperty];
        if (documentCompositeNode3 == null || !documentCompositeNode3.SupportsChildren || documentCompositeNode3.Children.Count < 1)
          return true;
        documentCompositeNode2 = (DocumentCompositeNode) ((DocumentCompositeNode) documentCompositeNode3.Children[0]).Properties[ParagraphElement.InlinesProperty];
        if (documentCompositeNode3.Children.Count > 1)
          return false;
      }
      if (documentCompositeNode2 != null)
      {
        if (documentCompositeNode2 == null || !documentCompositeNode2.SupportsChildren || documentCompositeNode2.Children.Count > 1)
          return false;
        if (documentCompositeNode2.Children.Count == 1)
        {
          DocumentCompositeNode documentCompositeNode3 = (DocumentCompositeNode) documentCompositeNode2.Children[0];
          if (!PlatformTypes.Run.IsAssignableFrom((ITypeId) documentCompositeNode3.Type))
            return false;
          foreach (KeyValuePair<IProperty, Microsoft.Expression.DesignModel.DocumentModel.DocumentNode> keyValuePair in (IEnumerable<KeyValuePair<IProperty, Microsoft.Expression.DesignModel.DocumentModel.DocumentNode>>) documentCompositeNode3.Properties)
          {
            if (!keyValuePair.Key.Equals((object) TextElementSceneElement.ResourcesProperty) && !keyValuePair.Key.Equals((object) TextElementSceneElement.LanguageProperty) && !keyValuePair.Key.Equals((object) RunElement.TextProperty))
              return false;
          }
        }
      }
      return true;
    }

    private void SerializeAsTextProperty()
    {
      using (this.TextSource.ViewModel.AnimationEditor.DeferKeyFraming())
      {
        using (SceneEditTransaction editTransaction = this.TextSource.ViewModel.CreateEditTransaction(StringTable.TextEditUndo))
        {
          string str = string.Empty;
          Block firstBlock = ((FlowDocument) this.RichTextBox.Document.PlatformSpecificObject).Blocks.FirstBlock;
          if (firstBlock != null)
            str = new TextRange(firstBlock.ContentStart, firstBlock.ContentEnd).Text;
          this.TextSource.ClearLocalValue(TextBlockElement.InlinesProperty);
          editTransaction.Update();
          this.TextSource.SetValue(TextBlockElement.TextProperty, (object) str);
          editTransaction.Commit();
        }
      }
    }

    private void SerializeAsInlines()
    {
      DocumentCompositeNode documentCompositeNode1 = (DocumentCompositeNode) this.TextSource.DocumentNode;
      if (this.FlowDocumentNode != null)
      {
        DocumentCompositeNode documentCompositeNode2 = (DocumentCompositeNode) this.FlowDocumentNode.Properties[FlowDocumentElement.BlocksProperty];
        DocumentCompositeNode documentCompositeNode3 = (DocumentCompositeNode) documentCompositeNode2.Children[0];
        using (this.TextSource.ViewModel.AnimationEditor.DeferKeyFraming())
        {
          using (SceneEditTransaction editTransaction = this.TextSource.ViewModel.CreateEditTransaction(StringTable.TextEditUndo))
          {
            DocumentCompositeNode node = documentCompositeNode1.Context.CreateNode(typeof (InlineCollection));
            Block block = ((FlowDocument) this.RichTextBox.Document.PlatformSpecificObject).Blocks.FirstBlock;
            for (int index = 0; index < documentCompositeNode2.Children.Count; ++index)
            {
              foreach (Microsoft.Expression.DesignModel.DocumentModel.DocumentNode documentNode in this.BuildInlinesFromBlock(documentCompositeNode2.Children[index], block))
                node.Children.Add(documentNode);
              if (index < documentCompositeNode2.Children.Count - 1)
                node.Children.Add((Microsoft.Expression.DesignModel.DocumentModel.DocumentNode) node.Context.CreateNode(typeof (LineBreak)));
              block = block.NextBlock;
            }
            this.StripUnsupportedElements(node);
            this.TextSource.ClearLocalValue(TextBlockElement.TextProperty);
            editTransaction.Update();
            documentCompositeNode1.Properties[TextBlockElement.InlinesProperty] = (Microsoft.Expression.DesignModel.DocumentModel.DocumentNode) node;
            editTransaction.Commit();
          }
        }
      }
      else
      {
        using (this.TextSource.ViewModel.AnimationEditor.DeferKeyFraming())
        {
          using (SceneEditTransaction editTransaction = this.TextSource.ViewModel.CreateEditTransaction(StringTable.TextEditUndo))
          {
            this.StripUnsupportedElements(this.InlinesCollectionNode);
            this.TextSource.ClearLocalValue(TextBlockElement.TextProperty);
            editTransaction.Update();
            documentCompositeNode1.Properties[TextBlockElement.InlinesProperty] = (Microsoft.Expression.DesignModel.DocumentModel.DocumentNode) this.InlinesCollectionNode;
            editTransaction.Commit();
          }
        }
      }
    }

    private void StripUnsupportedElements(DocumentCompositeNode compositeNode)
    {
      IProjectContext projectContext = this.TextSource.ViewModel.ProjectContext;
      if (compositeNode.SupportsChildren)
      {
        for (int index = compositeNode.Children.Count - 1; index >= 0; --index)
        {
          if (!JoltHelper.TypeSupported((ITypeResolver) projectContext, (ITypeId) compositeNode.Children[index].Type))
            compositeNode.Children.RemoveAt(index);
        }
      }
      foreach (Microsoft.Expression.DesignModel.DocumentModel.DocumentNode documentNode in compositeNode.ChildNodes)
      {
        DocumentCompositeNode compositeNode1 = documentNode as DocumentCompositeNode;
        if (compositeNode1 != null)
          this.StripUnsupportedElements(compositeNode1);
      }
    }

    private List<Microsoft.Expression.DesignModel.DocumentModel.DocumentNode> BuildInlinesFromBlock(Microsoft.Expression.DesignModel.DocumentModel.DocumentNode blockNode, Block block)
    {
      List<Microsoft.Expression.DesignModel.DocumentModel.DocumentNode> list = new List<Microsoft.Expression.DesignModel.DocumentModel.DocumentNode>();
      if (typeof (Paragraph).IsAssignableFrom(blockNode.TargetType))
      {
        DocumentCompositeNode documentCompositeNode1 = (DocumentCompositeNode) ((DocumentCompositeNode) blockNode).Properties[ParagraphElement.InlinesProperty];
        if (documentCompositeNode1 != null && documentCompositeNode1.SupportsChildren)
        {
          foreach (DocumentCompositeNode documentCompositeNode2 in (IEnumerable<Microsoft.Expression.DesignModel.DocumentModel.DocumentNode>) documentCompositeNode1.Children)
            list.Add(documentCompositeNode2.Clone(documentCompositeNode2.Context));
        }
      }
      else
      {
        Run run = new Run(new TextRange(block.ContentStart, block.ContentEnd).Text);
        Microsoft.Expression.DesignModel.DocumentModel.DocumentNode node = blockNode.Context.CreateNode(typeof (Run), (object) run);
        list.Add(node);
      }
      return list;
    }
  }
}
