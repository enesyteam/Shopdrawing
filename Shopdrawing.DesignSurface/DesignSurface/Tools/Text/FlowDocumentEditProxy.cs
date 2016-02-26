// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Text.FlowDocumentEditProxy
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.InstanceBuilders;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects.TextObjects;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.DesignSurface.ViewModel.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Text
{
  public class FlowDocumentEditProxy : RichTextBoxEditProxyBase
  {
    public FlowDocumentEditProxy(BaseFrameworkElement textSource)
      : base(textSource)
    {
    }

    public override void Instantiate()
    {
      base.Instantiate();
      this.CopyProperty(RichTextBoxElement.VerticalScrollBarVisibilityProperty);
      this.CopyProperty(RichTextBoxElement.HorizontalScrollBarVisibilityProperty);
      this.CopyProperty(RichTextBoxRangeElement.TextBlockTextAlignmentPropertyId);
      if (this.TextSource.ProjectContext.ResolveProperty(RichTextBoxElement.CaretBrushProperty) != null)
        this.CopyProperty(RichTextBoxElement.CaretBrushProperty);
      ITextFlowSceneNode textFlowSceneNode = (ITextFlowSceneNode) this.TextSource;
      DocumentNodePath documentNodePath = this.TextSource.DocumentNodePath;
      SceneViewModel viewModel = this.TextSource.ViewModel;
      IDocumentContext documentContext = viewModel.Document.DocumentContext;
      IProjectContext projectContext = viewModel.Document.ProjectContext;
      using (InstanceBuilderContext instanceBuilderContext = new InstanceBuilderContext(projectContext, viewModel, true, (Microsoft.Expression.DesignModel.DocumentModel.DocumentNode) null))
      {
        using (instanceBuilderContext.DisablePostponedResourceEvaluation())
        {
          instanceBuilderContext.ViewNodeManager.RootNodePath = documentNodePath;
          instanceBuilderContext.ViewNodeManager.Instantiate(instanceBuilderContext.ViewNodeManager.Root);
        }
        this.UpdateUIChildrenInstances((IInstanceBuilderContext) instanceBuilderContext);
        ReferenceStep referenceStep = (ReferenceStep) projectContext.ResolveProperty(textFlowSceneNode.TextChildProperty);
        bool flag1 = false;
        bool flag2 = false;
        bool flag3 = false;
        FlowDocument flowDocument = (FlowDocument) referenceStep.GetCurrentValue(instanceBuilderContext.ViewNodeManager.Root.Instance);
        DependencyPropertyReferenceStep shadowProperty = DesignTimeProperties.GetShadowProperty(projectContext.ResolveProperty(TextElementSceneElement.FontFamilyProperty), (ITypeId) null);
        if (flowDocument == null)
        {
          flowDocument = new FlowDocument();
        }
        else
        {
          flag1 = ((ReferenceStep) projectContext.ResolveProperty(TextElementSceneElement.FontSizeProperty)).IsSet((object) flowDocument);
          flag2 = ((ReferenceStep) projectContext.ResolveProperty(ParagraphElement.TextAlignmentProperty)).IsSet((object) flowDocument);
          flag3 = shadowProperty.IsSet((object) flowDocument);
        }
        double fontSize = flowDocument.FontSize;
        FontFamily fontFamily = (FontFamily) shadowProperty.GetValue((object) flowDocument);
        Thickness pagePadding = flowDocument.PagePadding;
        TextAlignment textAlignment = flowDocument.TextAlignment;
        referenceStep.SetValue(instanceBuilderContext.ViewNodeManager.Root.Instance, (object) new FlowDocument());
        this.RichTextBox.Document = (IViewFlowDocument) this.TextSource.Platform.ViewObjectFactory.Instantiate((object) flowDocument);
        if (flag1)
          flowDocument.FontSize = fontSize;
        if (flag3)
        {
          if (!DesignTimeProperties.UseShadowPropertyForInstanceBuilding(this.TypeResolver, ControlElement.FontFamilyProperty))
            flowDocument.FontFamily = fontFamily;
          else
            shadowProperty.SetValue((object) flowDocument, (object) fontFamily);
        }
        if (flag2)
          flowDocument.TextAlignment = textAlignment;
        flowDocument.PagePadding = pagePadding;
      }
    }

    public override void UpdateDocumentModel()
    {
      IDocumentContext documentContext = this.TextSource.DocumentContext;
      using (SceneEditTransaction editTransaction = this.TextSource.ViewModel.CreateEditTransaction(StringTable.TextEditUndo))
      {
        DocumentCompositeNode documentCompositeNode1 = (DocumentCompositeNode) this.TextSource.DocumentNode;
        IPropertyId textChildProperty = ((ITextFlowSceneNode) this.TextSource).TextChildProperty;
        DocumentCompositeNode documentCompositeNode2 = documentCompositeNode1.Properties[textChildProperty] as DocumentCompositeNode;
        if (documentCompositeNode2 == null || !typeof (FlowDocument).IsAssignableFrom(documentCompositeNode2.TargetType))
        {
          documentCompositeNode2 = documentContext.CreateNode(typeof (FlowDocument));
          documentCompositeNode1.Properties[textChildProperty] = (Microsoft.Expression.DesignModel.DocumentModel.DocumentNode) documentCompositeNode2;
        }
        Microsoft.Expression.DesignModel.DocumentModel.DocumentNode node = this.FlowDocumentNode.Properties[FlowDocumentElement.BlocksProperty];
        this.FlowDocumentNode.Properties[FlowDocumentElement.BlocksProperty] = (Microsoft.Expression.DesignModel.DocumentModel.DocumentNode) documentContext.CreateNode(typeof (BlockCollection));
        documentCompositeNode2.Properties[FlowDocumentElement.BlocksProperty] = (Microsoft.Expression.DesignModel.DocumentModel.DocumentNode) null;
        new SceneNodeIDHelper(this.TextSource.ViewModel, (SceneNode) this.TextSource).FixNameConflicts(node);
        documentCompositeNode2.Properties[FlowDocumentElement.BlocksProperty] = node;
        editTransaction.Commit();
      }
    }

    public override bool IsTextChange(DocumentNodeChange change)
    {
      if (base.IsTextChange(change))
        return true;
      DocumentCompositeNode documentCompositeNode1 = (DocumentCompositeNode) this.TextSource.DocumentNode;
      IPropertyId textChildProperty = ((ITextFlowSceneNode) this.TextSource).TextChildProperty;
      if (textChildProperty != null)
      {
        if (textChildProperty.Equals((object) change.PropertyKey))
          return true;
        DocumentCompositeNode documentCompositeNode2 = documentCompositeNode1.Properties[textChildProperty] as DocumentCompositeNode;
        if (documentCompositeNode2 != null && documentCompositeNode2.IsAncestorOf((Microsoft.Expression.DesignModel.DocumentModel.DocumentNode) change.ParentNode))
          return true;
      }
      return false;
    }
  }
}
