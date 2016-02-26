// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.MakeControlCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface.ResourcePane;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal class MakeControlCommand : SceneCommandBase
  {
    private bool showUI = true;

    public bool ShowUI
    {
      get
      {
        return this.showUI;
      }
      set
      {
        this.showUI = value;
      }
    }

    protected virtual string UndoString
    {
      get
      {
        return StringTable.UndoUnitMakeControl;
      }
    }

    protected virtual IType DefaultContainerType
    {
      get
      {
        return this.SceneViewModel.ProjectContext.ResolveType(PlatformTypes.Button);
      }
    }

    protected BaseFrameworkElement SelectedElement
    {
      get
      {
        return this.SceneViewModel.ElementSelectionSet.PrimarySelection as BaseFrameworkElement;
      }
    }

    private static IEnumerable<IPropertyId> RenderTransformsProperties
    {
      get
      {
        return (IEnumerable<IPropertyId>) new IPropertyId[3]
        {
          Base2DElement.RenderTransformProperty,
          Base2DElement.RenderTransformOriginProperty,
          BaseFrameworkElement.LayoutTransformProperty
        };
      }
    }

    public override bool IsEnabled
    {
      get
      {
        if (base.IsEnabled && this.SceneViewModel.ElementSelectionSet.Count == 1 && this.SelectedElement != null)
          return this.SelectedElement.Parent != null;
        return false;
      }
    }

    public MakeControlCommand(SceneViewModel viewModel)
      : base(viewModel)
    {
    }

    protected virtual CreateResourceDialog CreateDialog(CreateResourceModel createResourceModel)
    {
      return (CreateResourceDialog) new CreateControlDialog(this.DesignerContext, createResourceModel);
    }

    public virtual void PostProcessing(SceneNode oldElement, SceneNode newElement, DocumentCompositeNode styleNode)
    {
      SceneViewModel viewModel = newElement.ViewModel;
      if (!PlatformTypes.TextBox.IsAssignableFrom((ITypeId) newElement.Type) && !PlatformTypes.ContentControl.IsAssignableFrom((ITypeId) newElement.Type))
        return;
      StyleNode styleElement;
      DocumentNode templateNode = MakeControlCommand.GetTemplateNode(newElement, styleNode, out styleElement);
      if (templateNode == null)
        return;
      DocumentCompositeNode documentCompositeNode1 = templateNode.FindFirst(new Predicate<DocumentNode>(this.SelectTextBlockPredicate)) as DocumentCompositeNode;
      if (documentCompositeNode1 == null)
        return;
      TextBlockElement textBlockSceneNode = styleElement.ViewModel.GetSceneNode((DocumentNode) documentCompositeNode1) as TextBlockElement;
      TextBoxElement textBoxElement = newElement as TextBoxElement;
      ContentControlElement contentControlElement1 = newElement as ContentControlElement;
      using (SceneEditTransaction editTransaction = styleElement.ViewModel.CreateEditTransaction(this.UndoString))
      {
        DocumentCompositeNode documentCompositeNode2 = templateNode.FindFirst(new Predicate<DocumentNode>(this.SelectContentPresenterPredicate)) as DocumentCompositeNode;
        if (textBoxElement != null)
        {
          Dictionary<IPropertyId, IPropertyId> properties = new Dictionary<IPropertyId, IPropertyId>()
          {
            {
              TextBlockElement.FontFamilyProperty,
              ControlElement.FontFamilyProperty
            },
            {
              TextBlockElement.FontWeightProperty,
              ControlElement.FontWeightProperty
            },
            {
              TextBlockElement.FontSizeProperty,
              ControlElement.FontSizeProperty
            },
            {
              TextBlockElement.FontStyleProperty,
              ControlElement.FontStyleProperty
            },
            {
              TextBlockElement.TextAlignmentProperty,
              TextBoxElement.TextAlignmentProperty
            },
            {
              TextBlockElement.PaddingProperty,
              ControlElement.PaddingProperty
            }
          };
          if (viewModel.ProjectContext.ResolveProperty(TextBoxElement.TextDecorationsProperty) != null && viewModel.ProjectContext.ResolveProperty(TextBlockElement.TextDecorationsProperty) != null)
            properties.Add(TextBlockElement.TextDecorationsProperty, TextBoxElement.TextDecorationsProperty);
          this.SetTextPropertiesOnStyle(viewModel, styleElement, textBlockSceneNode, properties);
          ContentControlElement contentControlElement2 = (ContentControlElement) styleElement.ViewModel.CreateSceneNode(PlatformTypes.ScrollViewer);
          if (viewModel.ProjectContext.IsCapabilitySet(PlatformCapability.IsWpf))
          {
            contentControlElement2.Name = "PART_ContentHost";
            DocumentNode documentNode = (DocumentNode) DocumentNodeUtilities.NewTemplateBindingNode((DocumentNode) documentCompositeNode1, Base2DElement.SnapsToDevicePixelsProperty);
            contentControlElement2.SetValue(Base2DElement.SnapsToDevicePixelsProperty, (object) documentNode);
          }
          else
          {
            contentControlElement2.Name = "ContentElement";
            DocumentNode documentNode = (DocumentNode) DocumentNodeUtilities.NewTemplateBindingNode((DocumentNode) documentCompositeNode1, ControlElement.PaddingProperty);
            contentControlElement2.SetValue(ControlElement.PaddingProperty, (object) documentNode);
            contentControlElement2.SetLocalValueAsWpf(ControlElement.BorderThicknessProperty, (object) new Thickness(0.0));
            contentControlElement2.SetLocalValue(ControlElement.IsTabStopProperty, (object) false);
          }
          textBoxElement.SetValue(TextBoxElement.TextProperty, textBlockSceneNode.GetLocalOrDefaultValue(TextBlockElement.TextProperty));
          if (PlatformTypes.TextBox.IsAssignableFrom((ITypeId) newElement.Type) && PlatformTypes.TextBlock.IsAssignableFrom((ITypeId) oldElement.Type))
          {
            this.TransferLayoutProperties(MakeControlCommand.RenderTransformsProperties, (BaseFrameworkElement) textBlockSceneNode, (SceneElement) textBoxElement);
            bool addRenderTransforms = true;
            this.ClearLayoutProperties(this.GetLayoutProperties((SceneElement) textBlockSceneNode, addRenderTransforms), (SceneElement) contentControlElement2);
          }
          else
          {
            bool addRenderTransforms = true;
            this.TransferLayoutProperties(this.GetLayoutProperties((SceneElement) textBlockSceneNode, addRenderTransforms), (BaseFrameworkElement) textBlockSceneNode, (SceneElement) contentControlElement2);
          }
          SceneNode parent = textBlockSceneNode.Parent;
          IProperty propertyForChild = parent.GetPropertyForChild((SceneNode) textBlockSceneNode);
          ISceneNodeCollection<SceneNode> collectionForProperty = parent.GetCollectionForProperty((IPropertyId) propertyForChild);
          int index = collectionForProperty.IndexOf((SceneNode) textBlockSceneNode);
          collectionForProperty[index] = (SceneNode) contentControlElement2;
          if (documentCompositeNode2 != null)
            (styleElement.ViewModel.GetSceneNode((DocumentNode) documentCompositeNode2) as ContentPresenterElement).Remove();
        }
        else
        {
          Dictionary<IPropertyId, IPropertyId> properties = new Dictionary<IPropertyId, IPropertyId>()
          {
            {
              TextBlockElement.FontFamilyProperty,
              ControlElement.FontFamilyProperty
            },
            {
              TextBlockElement.FontWeightProperty,
              ControlElement.FontWeightProperty
            },
            {
              TextBlockElement.FontSizeProperty,
              ControlElement.FontSizeProperty
            },
            {
              TextBlockElement.FontStyleProperty,
              ControlElement.FontStyleProperty
            }
          };
          this.SetTextPropertiesOnStyle(viewModel, styleElement, textBlockSceneNode, properties);
          contentControlElement1.SetValue((IPropertyId) contentControlElement1.DefaultContentProperty, textBlockSceneNode.GetLocalOrDefaultValue(TextBlockElement.TextProperty));
          if (documentCompositeNode2 != null)
          {
            ContentPresenterElement presenterElement = (ContentPresenterElement) styleElement.ViewModel.GetSceneNode((DocumentNode) documentCompositeNode2);
            if (PlatformTypes.ContentControl.IsAssignableFrom((ITypeId) newElement.Type) && PlatformTypes.TextBlock.IsAssignableFrom((ITypeId) oldElement.Type))
            {
              this.TransferLayoutProperties(MakeControlCommand.RenderTransformsProperties, (BaseFrameworkElement) textBlockSceneNode, (SceneElement) newElement);
              bool addRenderTransforms = true;
              this.ClearLayoutProperties(this.GetLayoutProperties((SceneElement) textBlockSceneNode, addRenderTransforms), (SceneElement) presenterElement);
            }
            else
            {
              bool addRenderTransforms = true;
              this.TransferLayoutProperties(this.GetLayoutProperties((SceneElement) textBlockSceneNode, addRenderTransforms), (BaseFrameworkElement) textBlockSceneNode, (SceneElement) presenterElement);
            }
          }
          textBlockSceneNode.Remove();
        }
        if (textBlockSceneNode.IsSet(TextBlockElement.ForegroundProperty) == PropertyState.Set)
          styleElement.SetValue(ControlElement.ForegroundProperty, textBlockSceneNode.GetLocalValue(TextBlockElement.ForegroundProperty));
        editTransaction.Commit();
      }
    }

    private IEnumerable<IPropertyId> GetLayoutProperties(SceneElement element, bool addRenderTransforms)
    {
      IEnumerable<IPropertyId> layoutProperties = element.ViewModel.GetLayoutDesignerForChild(element, true).GetLayoutProperties();
      if (addRenderTransforms)
        return Enumerable.Concat<IPropertyId>(layoutProperties, MakeControlCommand.RenderTransformsProperties);
      return layoutProperties;
    }

    private void ClearLayoutProperties(IEnumerable<IPropertyId> properties, SceneElement element)
    {
      DocumentCompositeNode documentCompositeNode = element.DocumentNode as DocumentCompositeNode;
      foreach (IPropertyId propertyId in properties)
      {
        IProperty property = element.ViewModel.ProjectContext.ResolveProperty(propertyId);
        if (property != null)
          documentCompositeNode.ClearValue((IPropertyId) property);
      }
    }

    private void TransferLayoutProperties(IEnumerable<IPropertyId> properties, BaseFrameworkElement source, SceneElement target)
    {
      DocumentCompositeNode documentCompositeNode1 = source.DocumentNode as DocumentCompositeNode;
      DocumentCompositeNode documentCompositeNode2 = target.DocumentNode as DocumentCompositeNode;
      if (documentCompositeNode1 == null || documentCompositeNode2 == null)
        return;
      foreach (IPropertyId propertyId in properties)
      {
        IProperty propertyKey = source.ViewModel.ProjectContext.ResolveProperty(propertyId);
        if (propertyKey != null)
        {
          if (documentCompositeNode1.Properties.Contains(propertyKey))
            documentCompositeNode2.Properties[(IPropertyId) propertyKey] = documentCompositeNode1.Properties[(IPropertyId) propertyKey].Clone(target.DocumentContext);
          else
            documentCompositeNode2.ClearValue((IPropertyId) propertyKey);
        }
      }
    }

    private void SetTextPropertiesOnStyle(SceneViewModel viewModel, StyleNode styleElement, TextBlockElement textBlockSceneNode, Dictionary<IPropertyId, IPropertyId> properties)
    {
      foreach (KeyValuePair<IPropertyId, IPropertyId> keyValuePair in properties)
      {
        IProperty propertyKey = viewModel.ProjectContext.ResolveProperty(keyValuePair.Key);
        if (((DocumentCompositeNode) textBlockSceneNode.DocumentNode).Properties.Contains(propertyKey))
          styleElement.SetValue(keyValuePair.Value, textBlockSceneNode.GetLocalOrDefaultValue(keyValuePair.Key));
      }
    }

    private static DocumentNode GetTemplateNode(SceneNode newElement, DocumentCompositeNode styleNode, out StyleNode styleElement)
    {
      DocumentCompositeNode documentCompositeNode1 = styleNode.Properties[DictionaryEntryNode.ValueProperty] as DocumentCompositeNode;
      SceneViewModel viewModel = newElement.ViewModel.GetViewModel(documentCompositeNode1.DocumentRoot, false);
      if (viewModel == null)
      {
        styleElement = (StyleNode) null;
        return (DocumentNode) null;
      }
      styleElement = viewModel.GetSceneNode((DocumentNode) documentCompositeNode1) as StyleNode;
      DependencyPropertyReferenceStep propertyReferenceStep = (DependencyPropertyReferenceStep) newElement.ViewModel.ProjectContext.ResolveProperty(ControlElement.TemplateProperty);
      if (documentCompositeNode1 != null)
      {
        DocumentCompositeNode documentCompositeNode2 = documentCompositeNode1.Properties[StyleNode.SettersProperty] as DocumentCompositeNode;
        if (documentCompositeNode2 != null)
        {
          foreach (DocumentNode documentNode1 in (IEnumerable<DocumentNode>) documentCompositeNode2.Children)
          {
            DocumentCompositeNode documentCompositeNode3 = documentNode1 as DocumentCompositeNode;
            if (documentCompositeNode3 != null)
            {
              IMemberId memberId = (IMemberId) DocumentPrimitiveNode.GetValueAsMember(documentCompositeNode3.Properties[SetterSceneNode.PropertyProperty]);
              DocumentNode documentNode2 = documentCompositeNode3.Properties[SetterSceneNode.ValueProperty];
              if (memberId != null && documentNode2 != null && propertyReferenceStep.Equals((object) memberId))
                return documentNode2;
            }
          }
        }
      }
      return (DocumentNode) null;
    }

    public override void Execute()
    {
      BaseFrameworkElement selectedElement = this.SelectedElement;
      if (selectedElement == null)
        return;
      IType type1 = this.SceneViewModel.ProjectContext.ResolveType(PlatformTypes.Style);
      CreateResourceModel.ContextFlags contextFlags = this.SceneViewModel.ProjectContext.IsCapabilitySet(PlatformCapability.SupportsImplicitStyles) ? CreateResourceModel.ContextFlags.CanApplyAutomatically : CreateResourceModel.ContextFlags.None;
      if (this.SceneViewModel.ProjectContext.IsCapabilitySet(PlatformCapability.SupportsImplicitStyles))
        contextFlags = CreateResourceModel.ContextFlags.CanApplyAutomatically;
      CreateResourceModel createResourceModel = new CreateResourceModel(this.SceneViewModel, this.DesignerContext.ResourceManager, type1.RuntimeType, this.DefaultContainerType.RuntimeType, PlatformTypes.Style.Name, (SceneElement) null, (SceneNode) selectedElement, contextFlags);
      if (this.ShowUI)
      {
        bool? nullable = this.CreateDialog(createResourceModel).ShowDialog();
        if ((!nullable.GetValueOrDefault() ? 1 : (!nullable.HasValue ? true : false)) != 0)
          return;
      }
      using (this.SceneViewModel.DisableUpdateChildrenOnAddAndRemove())
      {
        using (SceneEditTransaction editTransaction1 = this.SceneViewModel.CreateEditTransaction(this.UndoString, false))
        {
          using (this.SceneViewModel.ForceBaseValue())
          {
            using (this.SceneViewModel.DisableDrawIntoState())
            {
              this.SceneViewModel.AnimationEditor.DeleteAllAnimationsInSubtree((SceneElement) selectedElement);
              this.SceneViewModel.ElementSelectionSet.Clear();
              IDocumentContext documentContext = this.SceneViewModel.Document.DocumentContext;
              IProjectContext projectContext = this.SceneViewModel.Document.ProjectContext;
              Type type2 = (Type) null;
              if (createResourceModel.TargetTypeAsset != null && createResourceModel.TargetTypeAsset.EnsureTypeReferenced(this.SceneViewModel.ProjectContext as ProjectContext))
                type2 = createResourceModel.TargetTypeAsset.Type.RuntimeType;
              if (type2 == (Type) null)
                type2 = createResourceModel.TargetType;
              IType type3 = projectContext.GetType(type2);
              DocumentCompositeNode documentCompositeNode1 = (DocumentCompositeNode) selectedElement.DocumentNode;
              this.SceneViewModel.GetLayoutDesignerForChild((SceneElement) selectedElement, true).ClearUnusedLayoutProperties(selectedElement);
              DocumentCompositeNode visualTreeNode = documentContext.CreateNode((ITypeId) documentCompositeNode1.Type);
              Dictionary<IProperty, DocumentNode> dictionary = new Dictionary<IProperty, DocumentNode>();
              bool addRenderTransforms = false;
              foreach (IPropertyId propertyId in this.GetLayoutProperties((SceneElement) selectedElement, addRenderTransforms))
              {
                IProperty property = this.DesignerContext.ActiveSceneViewModel.ProjectContext.ResolveProperty(propertyId);
                if (property != null && documentCompositeNode1.Properties.Contains(property))
                {
                  dictionary.Add(property, documentCompositeNode1.Properties[(IPropertyId) property].Clone(documentContext));
                  documentCompositeNode1.ClearValue((IPropertyId) property);
                }
              }
              foreach (KeyValuePair<IProperty, DocumentNode> keyValuePair in (IEnumerable<KeyValuePair<IProperty, DocumentNode>>) documentCompositeNode1.Properties)
              {
                IPropertyId index = (IPropertyId) keyValuePair.Key;
                DependencyPropertyReferenceStep propertyReferenceStep = index as DependencyPropertyReferenceStep;
                if ((propertyReferenceStep == null || !propertyReferenceStep.IsAttachable || propertyReferenceStep.MemberType == MemberType.DesignTimeProperty) && (!index.Equals((object) BaseFrameworkElement.WidthProperty) && !index.Equals((object) BaseFrameworkElement.HeightProperty)))
                  visualTreeNode.Properties[index] = keyValuePair.Value.Clone(documentContext);
              }
              if (documentCompositeNode1.SupportsChildren)
              {
                foreach (DocumentNode documentNode in (IEnumerable<DocumentNode>) documentCompositeNode1.Children)
                  visualTreeNode.Children.Add(documentNode.Clone(documentContext));
              }
              if (!PlatformTypes.Panel.IsAssignableFrom((ITypeId) documentCompositeNode1.Type))
              {
                GridElement gridElement = (GridElement) this.SceneViewModel.CreateSceneNode(PlatformTypes.Grid);
                SceneNode sceneNode = this.SceneViewModel.GetSceneNode((DocumentNode) visualTreeNode);
                gridElement.Children.Add(sceneNode);
                visualTreeNode = (DocumentCompositeNode) gridElement.DocumentNode;
              }
              StyleNode styleNode = (StyleNode) this.SceneViewModel.CreateSceneNode(PlatformTypes.Style);
              styleNode.StyleTargetTypeId = type3;
              SetterSceneNode setterSceneNode = (SetterSceneNode) this.SceneViewModel.CreateSceneNode(PlatformTypes.Setter);
              DependencyPropertyReferenceStep propertyReferenceStep1 = (DependencyPropertyReferenceStep) this.SceneViewModel.ProjectContext.ResolveProperty(ControlElement.TemplateProperty);
              setterSceneNode.Property = propertyReferenceStep1;
              BaseFrameworkElement frameworkElement = (BaseFrameworkElement) this.SceneViewModel.CreateSceneNode(type2);
              DocumentCompositeNode documentCompositeNode2 = (DocumentCompositeNode) frameworkElement.DocumentNode;
              this.AddPresenterIfNecessary(visualTreeNode, (SceneElement) frameworkElement);
              ControlTemplateElement controlTemplateElement = (ControlTemplateElement) this.SceneViewModel.CreateSceneNode(PlatformTypes.ControlTemplate);
              controlTemplateElement.ControlTemplateTargetTypeId = (ITypeId) type3;
              controlTemplateElement.DefaultInsertionPoint.Insert(this.SceneViewModel.GetSceneNode((DocumentNode) visualTreeNode));
              if (PlatformTypes.Button.Equals((object) type3) && controlTemplateElement.CanEditTriggers && this.SceneViewModel.ProjectContext.IsCapabilitySet(PlatformCapability.IsWpf))
              {
                DocumentCompositeNode node = documentContext.CreateNode(typeof (TriggerCollection));
                node.Children.Add((DocumentNode) this.CreatePropertyTrigger(documentContext, ButtonProperties.IsFocusedProperty, (object) true));
                node.Children.Add((DocumentNode) this.CreatePropertyTrigger(documentContext, ButtonProperties.IsDefaultedProperty, (object) true));
                node.Children.Add((DocumentNode) this.CreatePropertyTrigger(documentContext, BaseFrameworkElement.IsMouseOverProperty, (object) true));
                node.Children.Add((DocumentNode) this.CreatePropertyTrigger(documentContext, ButtonProperties.IsPressedProperty, (object) true));
                node.Children.Add((DocumentNode) this.CreatePropertyTrigger(documentContext, BaseFrameworkElement.IsEnabledProperty, (object) false));
                controlTemplateElement.SetLocalValue(ControlTemplateElement.ControlTemplateTriggersProperty, (DocumentNode) node);
              }
              setterSceneNode.SetValueAsSceneNode(SetterSceneNode.ValueProperty, (SceneNode) controlTemplateElement);
              styleNode.Setters.Add((SceneNode) setterSceneNode);
              bool useStaticResource = !JoltHelper.TypeSupported((ITypeResolver) this.SceneViewModel.ProjectContext, PlatformTypes.DynamicResource);
              int index1 = -1;
              if (useStaticResource && selectedElement.DocumentContext == createResourceModel.CurrentResourceSite.DocumentContext)
                index1 = createResourceModel.IndexInResourceSite(selectedElement.DocumentNode);
              IList<DocumentCompositeNode> referencedResources = Microsoft.Expression.DesignSurface.Utility.ResourceHelper.FindReferencedResources((DocumentNode) documentCompositeNode1);
              foreach (KeyValuePair<IProperty, DocumentNode> keyValuePair in dictionary)
                documentCompositeNode2.Properties[(IPropertyId) keyValuePair.Key] = keyValuePair.Value;
              IList<SceneNode> nodes = (IList<SceneNode>) new List<SceneNode>();
              nodes.Add((SceneNode) frameworkElement);
              SceneNode parent = selectedElement.Parent;
              IProperty propertyForChild = parent.GetPropertyForChild((SceneNode) selectedElement);
              ISceneNodeCollection<SceneNode> collectionForProperty = parent.GetCollectionForProperty((IPropertyId) propertyForChild);
              int index2 = collectionForProperty.IndexOf((SceneNode) selectedElement);
              collectionForProperty[index2] = (SceneNode) frameworkElement;
              if (createResourceModel.SelectedResourceDictionary != null)
              {
                ResourceContainer instance = createResourceModel.SelectedResourceDictionary.Instance;
                if (instance != null && instance.DocumentNode == documentCompositeNode1)
                  createResourceModel = new CreateResourceModel(this.SceneViewModel, this.DesignerContext.ResourceManager, type1.RuntimeType, type2, PlatformTypes.Style.Name, (SceneElement) frameworkElement, (SceneNode) null, contextFlags);
              }
              if (createResourceModel.CurrentResourceSite != null && !PlatformTypes.PlatformsCompatible(createResourceModel.CurrentResourceSite.DocumentContext.TypeResolver.PlatformMetadata, styleNode.DocumentNode.PlatformMetadata))
              {
                editTransaction1.Cancel();
                return;
              }
              DocumentCompositeNode resource = createResourceModel.CreateResource(styleNode.DocumentNode, StyleNode.TargetTypeProperty, index1);
              if (resource == null)
              {
                editTransaction1.Cancel();
                return;
              }
              DocumentNode resourceReference = createResourceModel.CreateResourceReference(this.SceneViewModel.Document.DocumentContext, resource, useStaticResource);
              DefaultTypeInstantiator typeInstantiator = new DefaultTypeInstantiator(this.SceneView);
              if (resourceReference != null)
                documentCompositeNode2.Properties[BaseFrameworkElement.StyleProperty] = resourceReference;
              foreach (KeyValuePair<IProperty, DocumentNode> keyValuePair in (IEnumerable<KeyValuePair<IProperty, DocumentNode>>) documentCompositeNode1.Properties)
              {
                IPropertyId index3 = (IPropertyId) keyValuePair.Key;
                DependencyPropertyReferenceStep propertyReferenceStep2 = index3 as DependencyPropertyReferenceStep;
                if (propertyReferenceStep2 != null && propertyReferenceStep2.IsAttachable && propertyReferenceStep2.MemberType != MemberType.DesignTimeProperty)
                  documentCompositeNode2.Properties[index3] = keyValuePair.Value.Clone(documentContext);
              }
              DocumentCompositeNode hostNode = createResourceModel.CurrentResourceSite.HostNode;
              SceneViewModel viewModel = this.SceneViewModel.GetViewModel(hostNode.DocumentRoot, false);
              using (SceneEditTransaction editTransaction2 = viewModel.CreateEditTransaction(this.UndoString))
              {
                Microsoft.Expression.DesignSurface.Utility.ResourceHelper.CopyResourcesToNewResourceSite(referencedResources, viewModel, hostNode, resource, createResourceModel.IndexInResourceSite((DocumentNode) resource));
                editTransaction2.Commit();
              }
              editTransaction1.Update();
              if (this.SceneView.IsValid)
                typeInstantiator.ApplyAfterInsertionDefaultsToElements(nodes, (SceneNode) null);
              this.SceneView.CandidateEditingContainer = frameworkElement.DocumentNodePath;
              editTransaction1.Update();
              this.SceneViewModel.ElementSelectionSet.ExtendSelection((SceneElement) frameworkElement);
              this.PostProcessing((SceneNode) selectedElement, (SceneNode) frameworkElement, resource);
              if (frameworkElement.GetComputedValue(ControlElement.TemplateProperty) != null)
                this.ActivateTemplateEditingMode((SceneElement) frameworkElement);
              else
                UIThreadDispatcherHelper.BeginInvoke(DispatcherPriority.ApplicationIdle, (Delegate) new Action<SceneElement>(this.ActivateTemplateEditingMode), (object) frameworkElement);
              this.SceneView.CandidateEditingContainer = (DocumentNodePath) null;
            }
            editTransaction1.Commit();
          }
        }
      }
    }

    private void ActivateTemplateEditingMode(SceneElement targetElement)
    {
      object computedValue = targetElement.GetComputedValue(ControlElement.TemplateProperty);
      if (computedValue == null)
        return;
      DocumentNodePath correspondingNodePath = this.SceneView.GetCorrespondingNodePath(this.SceneViewModel.ProjectContext.Platform.ViewObjectFactory.Instantiate(computedValue), true);
      if (correspondingNodePath == null)
        return;
      SceneViewModel viewModel = this.SceneViewModel.GetViewModel(correspondingNodePath.Node.DocumentRoot, true);
      if (this.SceneViewModel == viewModel)
      {
        viewModel.ActiveEditingContainerPath = correspondingNodePath;
      }
      else
      {
        IPropertyId ancestorPropertyKey = (IPropertyId) (this.SceneViewModel.ProjectContext.ResolveProperty(BaseFrameworkElement.StyleProperty) as ReferenceStep);
        BaseFrameworkElement frameworkElement = targetElement as BaseFrameworkElement;
        Size preferredSize = frameworkElement == null || !frameworkElement.IsViewObjectValid ? Size.Empty : frameworkElement.GetComputedBounds((Base2DElement) frameworkElement).Size;
        viewModel.SetViewRoot(this.SceneViewModel.DefaultView, targetElement, ancestorPropertyKey, correspondingNodePath.Node, preferredSize);
        viewModel.DefaultView.EnsureDesignSurfaceVisible();
      }
      if (!viewModel.IsEditable)
        return;
      SceneElement selectionToSet = viewModel.GetSceneNode(correspondingNodePath.Node) as SceneElement;
      viewModel.ElementSelectionSet.SetSelection(selectionToSet);
    }

    private bool SelectContentPresenterPredicate(DocumentNode node)
    {
      DocumentCompositeNode documentCompositeNode = node as DocumentCompositeNode;
      return documentCompositeNode != null && PlatformTypes.ContentPresenter.IsAssignableFrom((ITypeId) documentCompositeNode.Type);
    }

    private bool SelectTextBlockPredicate(DocumentNode node)
    {
      DocumentCompositeNode documentCompositeNode = node as DocumentCompositeNode;
      return documentCompositeNode != null && PlatformTypes.TextBlock.IsAssignableFrom((ITypeId) documentCompositeNode.Type);
    }

    private DocumentCompositeNode CreatePropertyTrigger(IDocumentContext documentContext, IPropertyId propertyId, object value)
    {
      DocumentCompositeNode node = documentContext.CreateNode(typeof (Trigger));
      IProperty property = documentContext.TypeResolver.ResolveProperty(propertyId);
      node.Properties[TriggerNode.PropertyProperty] = (DocumentNode) documentContext.CreateNode(PlatformTypes.DependencyProperty, (IDocumentNodeValue) new DocumentNodeMemberValue((IMember) property));
      node.Properties[TriggerNode.ValueProperty] = documentContext.CreateNode(value.GetType(), value);
      return node;
    }

    private void AddPresenterIfNecessary(DocumentCompositeNode visualTreeNode, SceneElement newElement)
    {
      Type targetType = (Type) null;
      if (PlatformTypes.ContentControl.IsAssignableFrom((ITypeId) newElement.Type))
        targetType = newElement.ViewModel.ProjectContext.ResolveType(PlatformTypes.ContentPresenter).RuntimeType;
      else if (PlatformTypes.ItemsControl.IsAssignableFrom((ITypeId) newElement.Type))
        targetType = newElement.ViewModel.ProjectContext.ResolveType(PlatformTypes.ItemsPresenter).RuntimeType;
      if (!(targetType != (Type) null))
        return;
      DocumentNode node = visualTreeNode.SelectFirstDescendantNode(targetType);
      if (node == null)
      {
        PanelElement panelElement = (PanelElement) this.SceneViewModel.GetSceneNode((DocumentNode) visualTreeNode);
        SceneElement presenterElement = (SceneElement) this.SceneViewModel.CreateSceneNode(targetType);
        SceneElement sceneElement = this.PreparePresenter(visualTreeNode, presenterElement);
        GridElement gridElement = panelElement as GridElement;
        if (gridElement != null)
        {
          int count1 = gridElement.ColumnDefinitions.Count;
          int count2 = gridElement.RowDefinitions.Count;
          if (count1 > 0)
            sceneElement.SetLocalValue(GridElement.ColumnSpanProperty, (object) count1);
          if (count2 > 0)
            sceneElement.SetLocalValue(GridElement.RowSpanProperty, (object) count2);
        }
        panelElement.Children.Add((SceneNode) sceneElement);
      }
      else
      {
        SceneElement presenterElement = this.SceneViewModel.GetSceneNode(node) as SceneElement;
        if (presenterElement == null)
          return;
        this.PreparePresenter(visualTreeNode, presenterElement);
      }
    }

    private SceneElement PreparePresenter(DocumentCompositeNode visualTreeNode, SceneElement presenterElement)
    {
      if (presenterElement.Type.Equals((object) PlatformTypes.ContentPresenter))
        ContentPresenterElement.PrepareContentPresenter((ContentPresenterElement) presenterElement);
      else if (presenterElement.Type.Equals((object) PlatformTypes.ItemsPresenter) && presenterElement.Parent == null)
      {
        ContentControlElement contentControlElement = (ContentControlElement) presenterElement.ViewModel.CreateSceneNode(PlatformTypes.ScrollViewer);
        if (!presenterElement.ViewModel.ProjectContext.IsCapabilitySet(PlatformCapability.IsWpf))
        {
          DocumentNode first = visualTreeNode.FindFirst((Predicate<DocumentNode>) (node =>
          {
            if (node.Name != null)
              return node.Name.Equals(PlatformTypes.ScrollViewer.Name);
            return false;
          }));
          if (first != null)
            presenterElement.ViewModel.GetSceneNode(first).Name = (string) null;
          contentControlElement.Name = PlatformTypes.ScrollViewer.Name;
        }
        contentControlElement.DefaultInsertionPoint.Insert((SceneNode) presenterElement);
        return (SceneElement) contentControlElement;
      }
      return presenterElement;
    }
  }
}
