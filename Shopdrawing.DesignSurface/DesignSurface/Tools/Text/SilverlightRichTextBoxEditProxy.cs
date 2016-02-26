// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Text.SilverlightRichTextBoxEditProxy
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects.TextObjects;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Microsoft.Expression.DesignSurface.Tools.Text
{
  public class SilverlightRichTextBoxEditProxy : RichTextBoxEditProxyBase
  {
    private List<Microsoft.Expression.DesignModel.DocumentModel.DocumentNode> inlineUIElementStandIns;
    private List<Microsoft.Expression.DesignModel.DocumentModel.DocumentNode> inlineUIElementOriginals;
    private RenderTargetBitmap standInImageSource;

    public override RichTextSerializationType SerializationType
    {
      get
      {
        if (JoltHelper.TypeSupported((ITypeResolver) this.TextSource.ViewModel.ProjectContext, PlatformTypes.FlowDocument))
          return RichTextSerializationType.FlowDocument;
        return JoltHelper.TypeSupported((ITypeResolver) this.TextSource.ViewModel.ProjectContext, PlatformTypes.BlockCollection) ? RichTextSerializationType.BlockCollection : RichTextSerializationType.None;
      }
    }

    public SilverlightRichTextBoxEditProxy(BaseFrameworkElement textSource)
      : base(textSource)
    {
    }

    protected override void CopyProperty(IPropertyId propertyId)
    {
      if (propertyId.Equals((object) this.ResolveProperty(RichTextBoxElement.TextAlignmentProperty)))
      {
        RichTextBox richTextBox = this.RichTextBox.PlatformSpecificObject as RichTextBox;
        object computedValueAsWpf = this.TextSource.GetComputedValueAsWpf(RichTextBoxElement.TextAlignmentProperty);
        if (richTextBox != null && computedValueAsWpf is TextAlignment)
        {
          TextAlignment textAlignment = (TextAlignment) computedValueAsWpf;
          TextBlock.SetTextAlignment((DependencyObject) richTextBox, textAlignment);
          return;
        }
      }
      base.CopyProperty(propertyId);
    }

    public override void Instantiate()
    {
      base.Instantiate();
      this.CopyProperty(RichTextBoxElement.VerticalScrollBarVisibilityProperty);
      this.CopyProperty(RichTextBoxElement.HorizontalScrollBarVisibilityProperty);
      this.CopyProperty(ControlElement.ForegroundProperty);
      this.CopyProperty(ControlElement.BackgroundProperty);
      this.CopyProperty(ControlElement.BorderThicknessProperty);
      this.CopyProperty(RichTextBoxElement.TextAlignmentProperty);
      this.CopyProperty(RichTextBoxElement.CaretBrushProperty);
      if (this.EditingElement.Platform.Metadata.ResolveProperty(TextBlockElement.BackgroundProperty) != null)
        this.CopyProperty(ControlElement.BackgroundProperty);
      if (this.TextSource.IsSet(RichTextBoxElement.BlocksProperty) == PropertyState.Set)
      {
        this.TextSource.DocumentNodePath.GetPathInContainer(((DocumentCompositeNode) this.TextSource.DocumentNode).Properties[RichTextBoxElement.BlocksProperty]);
        IDocumentContext documentContext = this.TextSource.ViewModel.Document.DocumentContext;
        this.RichTextBox.Document.Blocks.Clear();
        this.inlineUIElementStandIns = new List<Microsoft.Expression.DesignModel.DocumentModel.DocumentNode>();
        this.inlineUIElementOriginals = new List<Microsoft.Expression.DesignModel.DocumentModel.DocumentNode>();
        this.standInImageSource = (RenderTargetBitmap) null;
        foreach (SceneNode sceneNode in (IEnumerable<SceneNode>) this.TextSource.GetCollectionForProperty(RichTextBoxElement.BlocksProperty))
        {
          Microsoft.Expression.DesignModel.DocumentModel.DocumentNode documentNode = this.TextSource.ViewModel.DefaultView.ConvertToWpfValueAsDocumentNode((object) this.PreProcessDocumentNodeForEditProxy(sceneNode.DocumentNode));
          Block block = (Block) this.TextSource.ViewModel.DefaultView.ConvertToWpfValue((object) documentNode);
          this.PostProcessInstanceForEditProxy((DependencyObject) block);
          this.UpdateUIChildrenInstances((DocumentCompositeNode) documentNode, (TextElement) block);
          this.RichTextBox.Document.Blocks.Add((IViewBlock) this.ProxyPlatform.ViewObjectFactory.Instantiate((object) block));
        }
      }
      if (this.ForceLoadOnInstantiate)
      {
        this.EditingElement.UpdateLayout();
        this.AdjustSpacing();
      }
      else
        this.RichTextBox.Loaded += new RoutedEventHandler(this.RichTextBox_Loaded);
      if (!this.TextSource.ViewModel.ProjectContext.IsCapabilitySet(PlatformCapability.SupportsTabInTextControl))
        return;
      TextBoxBase textBoxBase = this.RichTextBox.PlatformSpecificObject as TextBoxBase;
      if (textBoxBase == null)
        return;
      textBoxBase.AddHandler(CommandManager.PreviewCanExecuteEvent, (Delegate) new CanExecuteRoutedEventHandler(SilverlightRichTextBoxEditProxy.CanExecute));
    }

    private Microsoft.Expression.DesignModel.DocumentModel.DocumentNode PreProcessDocumentNodeForEditProxy(Microsoft.Expression.DesignModel.DocumentModel.DocumentNode node)
    {
      if (!Enumerable.Any<Microsoft.Expression.DesignModel.DocumentModel.DocumentNode>(node.DescendantNodes, (Func<Microsoft.Expression.DesignModel.DocumentModel.DocumentNode, bool>) (descendant => PlatformTypes.FrameworkElement.IsAssignableFrom((ITypeId) descendant.Type))))
        return node;
      Microsoft.Expression.DesignModel.DocumentModel.DocumentNode node1 = node.Clone(node.Context);
      List<Microsoft.Expression.DesignModel.DocumentModel.DocumentNode> originals = new List<Microsoft.Expression.DesignModel.DocumentModel.DocumentNode>();
      List<Microsoft.Expression.DesignModel.DocumentModel.DocumentNode> replacements = new List<Microsoft.Expression.DesignModel.DocumentModel.DocumentNode>();
      this.PreProcessDocumentNodeForEditProxyRecursive(node1, node, originals, replacements);
      this.UpdateReplacements(originals, replacements);
      return node1;
    }

    private void PreProcessDocumentNodeForEditProxyRecursive(Microsoft.Expression.DesignModel.DocumentModel.DocumentNode node, Microsoft.Expression.DesignModel.DocumentModel.DocumentNode nodeInDocument, List<Microsoft.Expression.DesignModel.DocumentModel.DocumentNode> originals, List<Microsoft.Expression.DesignModel.DocumentModel.DocumentNode> replacements)
    {
      if (PlatformTypes.FrameworkElement.IsAssignableFrom((ITypeId) node.Type))
      {
        BaseFrameworkElement frameworkElement = this.TextSource.ViewModel.GetSceneNode(nodeInDocument) as BaseFrameworkElement;
        if (frameworkElement != null)
        {
          DocumentCompositeNode node1 = node.Context.CreateNode(PlatformTypes.Rectangle);
          node1.Properties[BaseFrameworkElement.WidthProperty] = node.Context.CreateNode(typeof (double), (object) frameworkElement.RenderSize.Width);
          node1.Properties[BaseFrameworkElement.HeightProperty] = node.Context.CreateNode(typeof (double), (object) frameworkElement.RenderSize.Height);
          node1.Properties[BaseFrameworkElement.TagProperty] = node.Context.CreateNode(typeof (int), (object) this.inlineUIElementStandIns.Count);
          this.inlineUIElementStandIns.Add(node);
          this.inlineUIElementOriginals.Add(nodeInDocument);
          originals.Add(node);
          replacements.Add((Microsoft.Expression.DesignModel.DocumentModel.DocumentNode) node1);
          return;
        }
      }
      IEnumerator<Microsoft.Expression.DesignModel.DocumentModel.DocumentNode> enumerator = nodeInDocument.ChildNodes.GetEnumerator();
      foreach (Microsoft.Expression.DesignModel.DocumentModel.DocumentNode node1 in node.ChildNodes)
      {
        enumerator.MoveNext();
        this.PreProcessDocumentNodeForEditProxyRecursive(node1, enumerator.Current, originals, replacements);
      }
    }

    private void PostProcessInstanceForEditProxy(DependencyObject treeNode)
    {
      Rectangle rectangle = treeNode as Rectangle;
      if (rectangle != null)
      {
        if (!(rectangle.Tag is int))
          return;
        int index = (int) rectangle.Tag;
        if (index < 0 || index >= this.inlineUIElementOriginals.Count)
          return;
        Microsoft.Expression.DesignModel.DocumentModel.DocumentNode node = this.inlineUIElementOriginals[index];
        FrameworkElement frameworkElement = (FrameworkElement) ((SilverlightArtboard) this.TextSource.ViewModel.DefaultView.Artboard).SilverlightImageHost;
        if (this.standInImageSource == null)
        {
          this.standInImageSource = new RenderTargetBitmap((int) frameworkElement.RenderSize.Width, (int) frameworkElement.RenderSize.Height, 96.0, 96.0, PixelFormats.Default);
          this.standInImageSource.Render((Visual) frameworkElement);
        }
        ImageBrush imageBrush = new ImageBrush((ImageSource) this.standInImageSource);
        Size renderSize = frameworkElement.RenderSize;
        Matrix computedTransformToRoot = this.TextSource.ViewModel.DefaultView.GetComputedTransformToRoot(this.TextSource.ViewModel.GetSceneNode(node).ViewObject);
        imageBrush.Viewbox = new Rect(computedTransformToRoot.OffsetX / renderSize.Width, computedTransformToRoot.OffsetY / renderSize.Height, rectangle.Width / renderSize.Width, rectangle.Height / renderSize.Height);
        rectangle.Fill = (Brush) imageBrush;
      }
      else
      {
        foreach (object obj in LogicalTreeHelper.GetChildren(treeNode))
        {
          DependencyObject treeNode1 = obj as DependencyObject;
          if (treeNode1 != null)
            this.PostProcessInstanceForEditProxy(treeNode1);
        }
      }
    }

    protected override Microsoft.Expression.DesignModel.DocumentModel.DocumentNode PostProcessDocumentNodeFromEditProxy(Microsoft.Expression.DesignModel.DocumentModel.DocumentNode node)
    {
      List<Microsoft.Expression.DesignModel.DocumentModel.DocumentNode> originals = new List<Microsoft.Expression.DesignModel.DocumentModel.DocumentNode>();
      List<Microsoft.Expression.DesignModel.DocumentModel.DocumentNode> replacements = new List<Microsoft.Expression.DesignModel.DocumentModel.DocumentNode>();
      this.PostProcessDocumentNodeFromEditProxyRecursive(node, originals, replacements);
      this.UpdateReplacements(originals, replacements);
      return node;
    }

    private void PostProcessDocumentNodeFromEditProxyRecursive(Microsoft.Expression.DesignModel.DocumentModel.DocumentNode node, List<Microsoft.Expression.DesignModel.DocumentModel.DocumentNode> originals, List<Microsoft.Expression.DesignModel.DocumentModel.DocumentNode> replacements)
    {
      DocumentCompositeNode documentCompositeNode = node as DocumentCompositeNode;
      if (documentCompositeNode != null)
      {
        DocumentPrimitiveNode documentPrimitiveNode = documentCompositeNode.Properties[BaseFrameworkElement.TagProperty] as DocumentPrimitiveNode;
        if (documentPrimitiveNode != null && PlatformTypes.Int32.IsAssignableFrom((ITypeId) documentPrimitiveNode.Type))
        {
          int index = documentPrimitiveNode.GetValue<int>();
          if (index >= 0 && index < this.inlineUIElementStandIns.Count)
          {
            originals.Add(node);
            replacements.Add(this.inlineUIElementStandIns[index].Clone(node.Context));
            return;
          }
        }
      }
      foreach (Microsoft.Expression.DesignModel.DocumentModel.DocumentNode node1 in node.ChildNodes)
        this.PostProcessDocumentNodeFromEditProxyRecursive(node1, originals, replacements);
    }

    private void UpdateReplacements(List<Microsoft.Expression.DesignModel.DocumentModel.DocumentNode> originals, List<Microsoft.Expression.DesignModel.DocumentModel.DocumentNode> replacements)
    {
      for (int index = 0; index < originals.Count; ++index)
      {
        Microsoft.Expression.DesignModel.DocumentModel.DocumentNode documentNode1 = originals[index];
        Microsoft.Expression.DesignModel.DocumentModel.DocumentNode documentNode2 = replacements[index];
        if (documentNode1.SitePropertyKey != null)
          documentNode1.Parent.Properties[(IPropertyId) documentNode1.SitePropertyKey] = documentNode2;
        else
          documentNode1.Parent.Children[documentNode1.SiteChildIndex] = documentNode2;
      }
    }

    private void AdjustSpacing()
    {
      RichTextBox richTextBox = this.EditingElement.PlatformSpecificObject as RichTextBox;
      if (richTextBox == null)
        return;
      TextBlock.SetLineHeight((DependencyObject) richTextBox, 0.01);
      richTextBox.Document.PagePadding = new Thickness(0.0, 0.0, 1.0, 0.0);
    }

    private void RichTextBox_Loaded(object sender, RoutedEventArgs e)
    {
      this.RichTextBox.Loaded -= new RoutedEventHandler(this.RichTextBox_Loaded);
      this.AdjustSpacing();
    }

    public override bool IsTextChange(DocumentNodeChange change)
    {
      Microsoft.Expression.DesignModel.DocumentModel.DocumentNode documentNode = ((DocumentCompositeNode) this.TextSource.DocumentNode).Properties[RichTextBoxElement.BlocksProperty];
      if ((change.PropertyKey == null || !change.PropertyKey.Equals((object) RichTextBoxElement.BlocksProperty)) && (documentNode == null || !documentNode.IsAncestorOf((Microsoft.Expression.DesignModel.DocumentModel.DocumentNode) change.ParentNode)))
        return base.IsTextChange(change);
      return true;
    }

    private static void CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      if (e.Command != EditingCommands.IncreaseIndentation && e.Command != EditingCommands.DecreaseIndentation)
        return;
      e.CanExecute = false;
      e.ContinueRouting = false;
      e.Handled = true;
    }

    public override void UpdateDocumentModel()
    {
      IDocumentContext documentContext = this.TextSource.DocumentContext;
      using (SceneEditTransaction editTransaction = this.TextSource.ViewModel.CreateEditTransaction(StringTable.TextEditUndo))
      {
        DocumentCompositeNode documentCompositeNode = (DocumentCompositeNode) this.TextSource.DocumentNode;
        documentCompositeNode.Properties[RichTextBoxElement.BlocksProperty] = (Microsoft.Expression.DesignModel.DocumentModel.DocumentNode) null;
        new SceneNodeIDHelper(this.TextSource.ViewModel, (SceneNode) this.TextSource).FixNameConflicts((Microsoft.Expression.DesignModel.DocumentModel.DocumentNode) this.BlockCollectionNode);
        documentCompositeNode.Properties[RichTextBoxElement.BlocksProperty] = (Microsoft.Expression.DesignModel.DocumentModel.DocumentNode) this.BlockCollectionNode;
        editTransaction.Commit();
      }
    }
  }
}
