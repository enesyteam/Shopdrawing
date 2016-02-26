// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.BaseFrameworkElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.InstanceBuilders;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Designers;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.Tools.Transforms;
using Microsoft.Expression.DesignSurface.UserInterface.ResourcePane;
using Microsoft.Expression.DesignSurface.ViewModel.Text;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class BaseFrameworkElement : Base2DElement, IStoryboardContainer, ITriggerContainer, IResourceContainer
  {
    public static readonly IPropertyId FENameProperty = (IPropertyId) PlatformTypes.FrameworkElement.GetMember(MemberType.LocalProperty, "Name", MemberAccessTypes.Public);
    public static readonly IPropertyId ActualWidthProperty = (IPropertyId) PlatformTypes.FrameworkElement.GetMember(MemberType.LocalProperty, "ActualWidth", MemberAccessTypes.Public);
    public static readonly IPropertyId ActualHeightProperty = (IPropertyId) PlatformTypes.FrameworkElement.GetMember(MemberType.LocalProperty, "ActualHeight", MemberAccessTypes.Public);
    public static readonly IPropertyId CursorProperty = (IPropertyId) PlatformTypes.FrameworkElement.GetMember(MemberType.LocalProperty, "Cursor", MemberAccessTypes.Public);
    public static readonly IPropertyId WidthProperty = (IPropertyId) PlatformTypes.FrameworkElement.GetMember(MemberType.LocalProperty, "Width", MemberAccessTypes.Public);
    public static readonly IPropertyId HeightProperty = (IPropertyId) PlatformTypes.FrameworkElement.GetMember(MemberType.LocalProperty, "Height", MemberAccessTypes.Public);
    public static readonly IPropertyId MaxHeightProperty = (IPropertyId) PlatformTypes.FrameworkElement.GetMember(MemberType.LocalProperty, "MaxHeight", MemberAccessTypes.Public);
    public static readonly IPropertyId MaxWidthProperty = (IPropertyId) PlatformTypes.FrameworkElement.GetMember(MemberType.LocalProperty, "MaxWidth", MemberAccessTypes.Public);
    public static readonly IPropertyId MinHeightProperty = (IPropertyId) PlatformTypes.FrameworkElement.GetMember(MemberType.LocalProperty, "MinHeight", MemberAccessTypes.Public);
    public static readonly IPropertyId MinWidthProperty = (IPropertyId) PlatformTypes.FrameworkElement.GetMember(MemberType.LocalProperty, "MinWidth", MemberAccessTypes.Public);
    public static readonly IPropertyId StyleProperty = (IPropertyId) PlatformTypes.FrameworkElement.GetMember(MemberType.LocalProperty, "Style", MemberAccessTypes.Public);
    public static readonly IPropertyId ResourcesProperty = (IPropertyId) PlatformTypes.FrameworkElement.GetMember(MemberType.LocalProperty, "Resources", MemberAccessTypes.Public);
    public static readonly IPropertyId TriggersProperty = (IPropertyId) PlatformTypes.FrameworkElement.GetMember(MemberType.LocalProperty, "Triggers", MemberAccessTypes.Public);
    public static readonly IPropertyId FocusableProperty = (IPropertyId) PlatformTypes.FrameworkElement.GetMember(MemberType.LocalProperty, "Focusable", MemberAccessTypes.Public);
    public static readonly IPropertyId FocusVisualStyleProperty = (IPropertyId) PlatformTypes.FrameworkElement.GetMember(MemberType.LocalProperty, "FocusVisualStyle", MemberAccessTypes.Public);
    public static readonly IPropertyId OverridesDefaultStyleProperty = (IPropertyId) PlatformTypes.FrameworkElement.GetMember(MemberType.LocalProperty, "OverridesDefaultStyle", MemberAccessTypes.Public);
    public static readonly IPropertyId HorizontalAlignmentProperty = (IPropertyId) PlatformTypes.FrameworkElement.GetMember(MemberType.LocalProperty, "HorizontalAlignment", MemberAccessTypes.Public);
    public static readonly IPropertyId VerticalAlignmentProperty = (IPropertyId) PlatformTypes.FrameworkElement.GetMember(MemberType.LocalProperty, "VerticalAlignment", MemberAccessTypes.Public);
    public static readonly IPropertyId MarginProperty = (IPropertyId) PlatformTypes.FrameworkElement.GetMember(MemberType.LocalProperty, "Margin", MemberAccessTypes.Public);
    public static readonly IPropertyId IsEnabledProperty = (IPropertyId) PlatformTypes.FrameworkElement.GetMember(MemberType.LocalProperty, "IsEnabled", MemberAccessTypes.Public);
    public static readonly IPropertyId IsMouseOverProperty = (IPropertyId) PlatformTypes.FrameworkElement.GetMember(MemberType.LocalProperty, "IsMouseOver", MemberAccessTypes.Public);
    public static readonly IPropertyId LayoutTransformProperty = (IPropertyId) PlatformTypes.FrameworkElement.GetMember(MemberType.LocalProperty, "LayoutTransform", MemberAccessTypes.Public);
    public static readonly IPropertyId DataContextProperty = (IPropertyId) PlatformTypes.FrameworkElement.GetMember(MemberType.LocalProperty, "DataContext", MemberAccessTypes.Public);
    public static readonly IPropertyId FrameworkContentElementStyleProperty = (IPropertyId) PlatformTypes.FrameworkContentElement.GetMember(MemberType.LocalProperty, "Style", MemberAccessTypes.Public);
    public static readonly IPropertyId TagProperty = (IPropertyId) PlatformTypes.FrameworkElement.GetMember(MemberType.LocalProperty, "Tag", MemberAccessTypes.Public);
    public static readonly BaseFrameworkElement.ConcreteBaseFrameworkElementFactory Factory = new BaseFrameworkElement.ConcreteBaseFrameworkElementFactory();

    public double Width
    {
      get
      {
        return (double) this.GetComputedValue(BaseFrameworkElement.WidthProperty);
      }
      set
      {
        this.SetValue(BaseFrameworkElement.WidthProperty, (object) value);
      }
    }

    public double Height
    {
      get
      {
        return (double) this.GetComputedValue(BaseFrameworkElement.HeightProperty);
      }
      set
      {
        this.SetValue(BaseFrameworkElement.HeightProperty, (object) value);
      }
    }

    public StyleNode Style
    {
      get
      {
        return this.GetLocalValueAsSceneNode(BaseFrameworkElement.StyleProperty) as StyleNode;
      }
    }

    public ReadOnlyCollection<IPropertyId> StyleProperties
    {
      get
      {
        return this.Metadata.StyleProperties;
      }
    }

    public ILayoutDesigner LayoutDesigner
    {
      get
      {
        return LayoutDesignerFactory.Instantiate((SceneNode) this);
      }
    }

    BaseFrameworkElement IStoryboardContainer.ScopeElement
    {
      get
      {
        return this;
      }
    }

    BaseFrameworkElement IStoryboardContainer.TargetElement
    {
      get
      {
        return this;
      }
    }

    public bool CanEditTriggers
    {
      get
      {
        return true;
      }
    }

    IList<TriggerBaseNode> ITriggerContainer.VisualTriggers
    {
      get
      {
        IPropertyId collectionProperty = (IPropertyId) this.ProjectContext.ResolveProperty(BaseFrameworkElement.TriggersProperty);
        if (collectionProperty != null)
          return (IList<TriggerBaseNode>) new SceneNode.SceneNodeCollection<TriggerBaseNode>((SceneNode) this, collectionProperty);
        return (IList<TriggerBaseNode>) new EmptyList<TriggerBaseNode>();
      }
    }

    bool ITriggerContainer.CanEdit
    {
      get
      {
        return !this.IsLockedOrAncestorLocked;
      }
    }

    Type ITriggerContainer.TargetElementType
    {
      get
      {
        return this.TrueTargetType;
      }
    }

    public ResourceDictionaryNode Resources
    {
      get
      {
        return ResourceManager.ProvideResourcesForElement((SceneNode) this);
      }
      set
      {
        this.SetValueAsSceneNode(this.Type.Metadata.ResourcesProperty, (SceneNode) value);
      }
    }

    public bool AreResourcesSupported
    {
      get
      {
        return this.Platform.Metadata.ResolveProperty(BaseFrameworkElement.ResourcesProperty) != null;
      }
    }

    protected override object GetRawComputedValueInternal(PropertyReference propertyReference)
    {
      object obj = base.GetRawComputedValueInternal(propertyReference);
      if (obj == null && propertyReference.Count == 1 && BaseFrameworkElement.StyleProperty.Equals((object) propertyReference.FirstStep))
        obj = this.FindMissingImplicitStyle();
      return obj;
    }

    public virtual StyleNode ExpandDefaultStyle(IPropertyId propertyKey)
    {
      propertyKey = (IPropertyId) this.ProjectContext.ResolveProperty(propertyKey);
      if (this.IsSet(BaseFrameworkElement.StyleProperty) == PropertyState.Set)
        return (StyleNode) null;
      DocumentNode nodeForDefaultStyle = this.GetDocumentNodeForDefaultStyle(this.Type, propertyKey);
      StyleNode styleNode1 = (StyleNode) null;
      if (nodeForDefaultStyle != null)
      {
        if (!StyleNode.IsReferenceValue(nodeForDefaultStyle))
        {
          StyleNode styleNode2 = (StyleNode) this.ViewModel.GetSceneNode(nodeForDefaultStyle);
          foreach (SetterSceneNode setter in (IEnumerable<SceneNode>) styleNode2.Setters)
          {
            DependencyPropertyReferenceStep property = setter.Property;
            if (property != null && PlatformTypes.FrameworkTemplate.IsAssignableFrom((ITypeId) property.PropertyType))
              this.ResolveTemplate(setter);
          }
          if (styleNode2.VisualTriggers != null)
          {
            foreach (TriggerBaseNode triggerBaseNode in (IEnumerable<TriggerBaseNode>) styleNode2.VisualTriggers)
            {
              BaseTriggerNode baseTriggerNode = triggerBaseNode as BaseTriggerNode;
              if (baseTriggerNode != null)
              {
                foreach (SetterSceneNode setter in (IEnumerable<SceneNode>) baseTriggerNode.Setters)
                {
                  DependencyPropertyReferenceStep property = setter.Property;
                  if (property != null && PlatformTypes.FrameworkTemplate.IsAssignableFrom((ITypeId) property.PropertyType))
                    this.ResolveTemplate(setter);
                }
              }
            }
          }
        }
        styleNode1 = (StyleNode) this.ViewModel.GetSceneNode(nodeForDefaultStyle);
        using (this.ViewModel.ForceBaseValue())
          this.SetValueAsSceneNode(propertyKey, (SceneNode) styleNode1);
      }
      return styleNode1;
    }

    protected override object GetLocalValueInternal(PropertyReference propertyReference)
    {
      if (propertyReference.ReferenceSteps[0].Equals((object) BaseFrameworkElement.StyleProperty))
      {
        StyleNode style = this.Style;
        if (style != null && StyleNode.IsDefaultValue(style.DocumentNode))
          return (object) null;
      }
      return base.GetLocalValueInternal(propertyReference);
    }

    public virtual bool CanCloneStyle(IPropertyId propertyKey)
    {
      DocumentCompositeNode documentCompositeNode = (DocumentCompositeNode) this.DocumentNode;
      IType type = documentCompositeNode.Type;
      if (documentCompositeNode.Properties[propertyKey] == null)
        return this.ResolveDefaultStyleAsDocumentNode(type, propertyKey) != null;
      return true;
    }

    protected override void ModifyValue(PropertyReference propertyReference, object valueToSet, SceneNode.Modification modification, int index)
    {
      LayoutUtilities.DetectLayoutOverrides((SceneElement) this, propertyReference);
      if (this is ITextFlowSceneNode)
        this.ClearPropertyOnTextRuns(propertyReference);
      base.ModifyValue(propertyReference, valueToSet, modification, index);
    }

    private void ClearPropertyOnTextRuns(PropertyReference propertyReference)
    {
      if (!RichTextBoxRangeElement.ShouldClearPropertyOnTextRuns((SceneNode) this, propertyReference))
        return;
      propertyReference = propertyReference.Subreference(0, 0);
      foreach (SceneNode sceneNode in SceneElementHelper.GetElementTree((SceneElement) this))
      {
        TextElementSceneElement elementSceneElement = sceneNode as TextElementSceneElement;
        if (elementSceneElement != null)
        {
          PropertyReference propertyReference1 = this.DesignerContext.PropertyManager.FilterProperty((SceneNode) elementSceneElement, propertyReference);
          if (propertyReference1 != null)
            elementSceneElement.ClearValue(propertyReference1);
        }
      }
    }

    protected virtual DocumentNode ResolveDefaultStyleAsDocumentNode(IType targetType, IPropertyId propertyKey)
    {
      DocumentNode node = (DocumentNode) null;
      IViewStyle viewStyle = (IViewStyle) null;
      if (this.IsViewObjectValid)
      {
        ReferenceStep referenceStep = propertyKey as ReferenceStep;
        if (referenceStep != null && this.ProjectContext.IsCapabilitySet(PlatformCapability.IsWpf))
        {
          viewStyle = this.Platform.ViewObjectFactory.Instantiate(referenceStep.GetCurrentValue(this.ViewObject.PlatformSpecificObject)) as IViewStyle;
          if (viewStyle != null && viewStyle.StyleTargetType != targetType.RuntimeType)
            viewStyle = (IViewStyle) null;
          if (viewStyle != null)
          {
            node = this.ViewModel.DefaultView.GetCorrespondingDocumentNode((IViewObject) viewStyle, true);
            if (node == null && this.DesignerContext.DesignerDefaultPlatformService.DefaultPlatform == this.Platform)
              node = this.CreateNode(viewStyle.PlatformSpecificObject);
          }
        }
      }
      for (IType type = this.ProjectContext.GetType(this.MetadataFactory.GetMetadata(targetType.RuntimeType).GetStylePropertyTargetType(propertyKey)); node == null && type != null && !PlatformTypes.Object.Equals((object) type); type = type.BaseType)
      {
        IList<DocumentCompositeNode> auxillaryResources = (IList<DocumentCompositeNode>) null;
        if (PlatformTypes.IsPlatformType((ITypeId) type))
        {
          node = this.DesignerContext.ThemeContentProvider.GetThemeResourceFromPlatform(this.Platform, (object) type.RuntimeType, out auxillaryResources);
        }
        else
        {
          foreach (IProject project in this.DesignerContext.ProjectManager.CurrentSolution.Projects)
          {
            IProjectContext projectContext = (IProjectContext) ProjectXamlContext.GetProjectContext(project);
            if (projectContext != null && type.RuntimeAssembly.Equals((object) projectContext.ProjectAssembly))
            {
              node = this.DesignerContext.ThemeContentProvider.GetThemeResourceFromProject(project, (object) type.RuntimeType, out auxillaryResources);
              if (node == null && !this.ProjectContext.IsCapabilitySet(PlatformCapability.IsWpf))
                node = this.DesignerContext.ThemeContentProvider.GetThemeResourceFromAssembly(this.ProjectContext, type.RuntimeAssembly, type.RuntimeAssembly, (object) type.RuntimeType, out auxillaryResources);
            }
          }
        }
        if (node != null)
        {
          node = node.Clone(this.DocumentContext);
          if (auxillaryResources != null && auxillaryResources.Count > 0)
          {
            StyleNode styleNode = (StyleNode) this.ViewModel.GetSceneNode(node);
            if (styleNode.AreResourcesSupported)
            {
              if (styleNode.Resources == null)
                styleNode.Resources = (ResourceDictionaryNode) this.ViewModel.CreateSceneNode(PlatformTypes.ResourceDictionary);
              for (int index = auxillaryResources.Count - 1; index >= 0; --index)
              {
                DictionaryEntryNode dictionaryEntryNode = (DictionaryEntryNode) this.ViewModel.GetSceneNode(auxillaryResources[index].Clone(this.DocumentContext));
                styleNode.Resources.Insert(0, dictionaryEntryNode);
              }
            }
          }
        }
      }
      if (viewStyle == null && this.ProjectContext.IsCapabilitySet(PlatformCapability.IsWpf))
      {
        ReferenceStep referenceStep = propertyKey as ReferenceStep;
        if (referenceStep != null)
        {
          System.Windows.Style style = referenceStep.GetDefaultValue(targetType.RuntimeType) as System.Windows.Style;
          if (style != null)
            node = this.CreateNode((object) style);
        }
      }
      return node;
    }

    protected virtual DocumentNode GetDocumentNodeForDefaultStyle(IType targetType, IPropertyId propertyKey)
    {
      DocumentNode documentNode1 = this.ResolveDefaultStyleAsDocumentNode(targetType, propertyKey);
      DocumentNode documentNode2 = (DocumentNode) null;
      if (documentNode1 != null && documentNode1.DocumentRoot == this.ViewModel.DocumentRoot)
        documentNode2 = (DocumentNode) this.DocumentContext.CreateNode(PlatformTypes.Style, (IDocumentNodeValue) new DocumentNodeReferenceValue(documentNode1));
      if (documentNode2 == null)
      {
        if (documentNode1 != null)
        {
          try
          {
            documentNode2 = documentNode1.Clone(this.DocumentContext);
            ((DocumentCompositeNode) documentNode2).SetValue<bool>(DesignTimeProperties.IsDefaultStyleProperty, true);
          }
          catch (Exception ex)
          {
            return (DocumentNode) null;
          }
        }
      }
      return documentNode2;
    }

    BaseFrameworkElement IStoryboardContainer.GetRelativeTargetElement(DocumentNodePath relativeEditingContainerPath)
    {
      return this;
    }

    SceneNode ITriggerContainer.ResolveTargetName(string name)
    {
      if (string.IsNullOrEmpty(name))
        return (SceneNode) this;
      return SceneElementHelper.FindNode((SceneNode) this, name);
    }

    SceneNode ITriggerContainer.ResolveTargetName(string name, SceneNode relativeSource)
    {
      if (string.IsNullOrEmpty(name))
        return (SceneNode) (relativeSource as SceneElement ?? relativeSource.FindSceneNodeTypeAncestor<SceneElement>());
      return SceneElementHelper.FindNode((SceneNode) this, name);
    }

    string IStoryboardContainer.GetTargetName(SceneNode node)
    {
      if (node.StoryboardContainer != this && node.StoryboardContainer != null)
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ExceptionStringTable.TimelineContainerDoesNotContainElement, new object[0]));
      return node.Name;
    }

    void IStoryboardContainer.AddResourceStoryboard(string name, StoryboardTimelineSceneNode storyboard)
    {
      this.AddResourceStoryboard(name, storyboard);
    }

    protected void AddResourceStoryboard(string name, StoryboardTimelineSceneNode storyboard)
    {
      Microsoft.Expression.DesignSurface.Utility.ResourceHelper.EnsureResourceDictionaryNode((SceneNode) this).Add(DictionaryEntryNode.Factory.Instantiate((object) name, (SceneNode) storyboard));
    }

    bool IStoryboardContainer.RemoveResourceStoryboard(StoryboardTimelineSceneNode storyboard)
    {
      return this.Resources.RemoveEntryWithValue((SceneNode) storyboard);
    }

    private void ResolveTemplate(SetterSceneNode setter)
    {
      DocumentNode node1 = ((DocumentCompositeNode) setter.DocumentNode).Properties[SetterSceneNode.ValueProperty];
      if (node1 != null && !PlatformTypes.FrameworkTemplate.IsAssignableFrom((ITypeId) node1.Type))
      {
        DocumentCompositeNode node2 = node1 as DocumentCompositeNode;
        if (node2 != null)
        {
          DocumentNode resourceKey = ResourceNodeHelper.GetResourceKey(node2);
          if (resourceKey != null)
            node1 = this.CreateNode(this.ViewModel.FindResource(this.ViewModel.CreateInstance(new DocumentNodePath(resourceKey, resourceKey))));
        }
      }
      if (node1 == null)
        return;
      ControlTemplateElement controlTemplateElement = this.ViewModel.GetSceneNode(node1) as ControlTemplateElement;
      if (controlTemplateElement != null)
        controlTemplateElement.RenameTemplateIDs();
      ((DocumentCompositeNode) setter.DocumentNode).Properties[SetterSceneNode.ValueProperty] = node1;
    }

    internal object FindMissingImplicitStyle()
    {
      if (this.Platform.Metadata.IsCapabilitySet(PlatformCapability.IsWpf) || !this.Platform.Metadata.IsCapabilitySet(PlatformCapability.SupportsImplicitStyles))
        return (object) null;
      if (!PlatformTypes.FrameworkElement.IsAssignableFrom((ITypeId) this.Type))
        return (object) null;
      DocumentNode node = new ExpressionEvaluator(this.ViewModel.DefaultView.InstanceBuilderContext.DocumentRootResolver).EvaluateResource(this.DocumentNodePath, this.DocumentContext.CreateNode(typeof (Type), (object) this.Type.RuntimeType));
      if (node == null)
        return (object) null;
      DocumentNodePath nodePath = new DocumentNodePath(node.DocumentRoot.RootNode, node);
      IInstanceBuilderContext viewContext = this.ViewModel.DefaultView.InstanceBuilderContext.GetViewContext(node.DocumentRoot);
      if (viewContext == null || viewContext.ViewNodeManager.Root == null || viewContext.ViewNodeManager.Root.InstanceState == InstanceState.Invalid)
        return (object) null;
      ViewNode correspondingViewNode = viewContext.ViewNodeManager.GetCorrespondingViewNode(nodePath);
      if (correspondingViewNode == null || viewContext.ExceptionDictionary.Contains(correspondingViewNode))
        return (object) null;
      object instance = correspondingViewNode.Instance;
      if (instance == null || !PlatformTypes.Style.IsAssignableFrom((ITypeId) viewContext.DocumentContext.TypeResolver.GetType(instance.GetType())))
        return (object) null;
      return instance;
    }

    public class ConcreteBaseFrameworkElementFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new BaseFrameworkElement();
      }
    }
  }
}
