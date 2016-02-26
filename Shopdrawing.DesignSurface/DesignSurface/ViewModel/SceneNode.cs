// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.SceneNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.InstanceBuilders;
using Microsoft.Expression.DesignModel.Markup;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.NodeBuilders;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignModel.ViewObjects.TextObjects;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.DocumentProcessors;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel.Extensibility;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.UserInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class SceneNode
  {
    private static bool allowEnsureTransform = true;
    private static bool allowRenameAsyncDialog = true;
    public static readonly SceneNode.ConcreteSceneNodeFactory Factory = new SceneNode.ConcreteSceneNodeFactory();
    private Microsoft.Expression.DesignModel.DocumentModel.DocumentNode node;
    private SceneViewModel viewModel;
    private SceneNodeModelItem modelItem;
    private DocumentNodePath lastKnownDocumentNodePath;
    private DocumentNodePath lastKnownEditingContainerPath;
    private IViewObject cachedViewObject;

    public IType Type
    {
      get
      {
        return this.node.Type;
      }
    }

    public System.Type TargetType
    {
      get
      {
        return this.DocumentNode.TargetType;
      }
    }

    public System.Type TrueTargetType
    {
      get
      {
        ITypeId typeId = (ITypeId) this.TrueTargetTypeId;
        IReflectionType reflectionType = typeId as IReflectionType;
        if (reflectionType != null)
          return reflectionType.ReflectionType;
        return (typeId as IType ?? this.ProjectContext.ResolveType(typeId)).RuntimeType;
      }
    }

    public IType TrueTargetTypeId
    {
      get
      {
        IType type = this.DocumentNode.Type;
        if (this.ViewModel.RootNode == this)
        {
          XamlDocument xamlDocument = (XamlDocument) this.ViewModel.XamlDocument;
          if (xamlDocument != null)
          {
            IType codeBehindClass = xamlDocument.CodeBehindClass;
            if (codeBehindClass != null && codeBehindClass.IsResolvable && codeBehindClass.IsBuilt)
            {
              codeBehindClass.InitializeClass();
              type = codeBehindClass;
            }
          }
        }
        return type;
      }
    }

    public Microsoft.Expression.DesignModel.DocumentModel.DocumentNode DocumentNode
    {
      get
      {
        return this.node;
      }
    }

    public bool IsInDocument
    {
      get
      {
        return this.node.DocumentRoot != null;
      }
    }

    public IPlatform Platform
    {
      get
      {
        return this.ProjectContext.Platform;
      }
    }

    public DocumentNodePath DocumentNodePath
    {
      get
      {
        if (this == this.viewModel.ActiveEditingContainer)
          return this.viewModel.ActiveEditingContainerPath;
        if (!this.IsAttached)
          return new DocumentNodePath(this.RootDocumentNode, this.node);
        if (this.viewModel.ActiveEditingContainerPath == null)
          return new DocumentNodePath(this.viewModel.DocumentRoot.RootNode, this.node);
        if (this.LastKnownDocumentNodePathIsStillGood())
          return this.lastKnownDocumentNodePath;
        DocumentNodePath documentNodePath = this.viewModel.ActiveEditingContainerPath.GetPathInContainer(this.node);
        while (documentNodePath != null)
        {
          if (documentNodePath.IsValid())
          {
            this.lastKnownDocumentNodePath = documentNodePath;
            this.lastKnownEditingContainerPath = this.viewModel.ActiveEditingContainerPath;
            return documentNodePath;
          }
          documentNodePath = documentNodePath.GetContainerOwnerPath();
          if (documentNodePath != null)
            documentNodePath = documentNodePath.GetPathInContainer(this.node);
        }
        return this.viewModel.ActiveEditingContainerPath.GetPathInContainer(this.node);
      }
    }

    public SceneViewModel ViewModel
    {
      get
      {
        return this.viewModel;
      }
    }

    internal DesignerContext DesignerContext
    {
      get
      {
        return this.viewModel.DesignerContext;
      }
    }

    public IDocumentContext DocumentContext
    {
      get
      {
        return this.node.Context;
      }
    }

    public IProjectContext ProjectContext
    {
      get
      {
        return this.viewModel.ProjectContext;
      }
    }

    public ITypeMetadataFactory MetadataFactory
    {
      get
      {
        return this.ProjectContext.MetadataFactory;
      }
    }

    public ITypeMetadata Metadata
    {
      get
      {
        return this.Type.Metadata;
      }
    }

    internal bool IsAttached
    {
      get
      {
        return this.RootDocumentNode == this.viewModel.DocumentRoot.RootNode;
      }
    }

    public virtual SceneNode Parent
    {
      get
      {
        Microsoft.Expression.DesignModel.DocumentModel.DocumentNode nodeParent = this.NodeParent;
        if (nodeParent == null)
          return (SceneNode) null;
        return this.viewModel.GetSceneNode(nodeParent);
      }
    }

    public bool IsSubclassDefinition
    {
      get
      {
        return this.DocumentNode.IsSubclassDefinition;
      }
    }

    public virtual IPropertyId NameProperty
    {
      get
      {
        return (IPropertyId) this.Metadata.NameProperty;
      }
    }

    public virtual string Name
    {
      get
      {
        DocumentCompositeNode documentCompositeNode = this.DocumentNode as DocumentCompositeNode;
        if (documentCompositeNode != null)
          return DocumentPrimitiveNode.GetValueAsString(documentCompositeNode.Properties[this.NameProperty]);
        return (string) null;
      }
      set
      {
        PropertyReference propertyReference = new PropertyReference((ReferenceStep) this.NameProperty);
        if (string.IsNullOrEmpty(value))
        {
          this.ClearValue(propertyReference);
        }
        else
        {
          string name1 = this.Name;
          this.SetValue(propertyReference, (object) value);
          string name2 = this.Name;
          this.viewModel.BindingEditor.UpdateElementNameBindings(this.DocumentNode as DocumentCompositeNode, name1, name2);
        }
      }
    }

    public virtual bool ShouldClearAnimation
    {
      get
      {
        return false;
      }
    }

    public virtual string DisplayName
    {
      get
      {
        if (this.IsNamed)
          return this.Name;
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, StringTable.ElementTimelineItemBracketedName, new object[1]
        {
          (object) this.Type.Name
        });
      }
    }

    public bool IsNamed
    {
      get
      {
        if (this.Name != null && this.Name.Length > 0)
          return !this.Name.StartsWith("~", StringComparison.Ordinal);
        return false;
      }
    }

    public virtual string UniqueID
    {
      get
      {
        List<string> list = new List<string>();
        if (this.Name != null)
          list.Add(this.Name);
        else if (this.IsSet(DesignTimeProperties.TestNameProperty) == PropertyState.Set)
        {
          string str = this.GetLocalValue(DesignTimeProperties.TestNameProperty) as string;
          if (str != null)
            list.Add(str);
        }
        else if (this.ViewModel.RootNode == this)
          list.Add(this.Type.Name);
        DocumentNodePath containerOwnerPath;
        for (DocumentNodePath documentNodePath = this.DocumentNodePath; documentNodePath != null; documentNodePath = containerOwnerPath)
        {
          containerOwnerPath = documentNodePath.GetContainerOwnerPath();
          if (containerOwnerPath != null)
          {
            SceneElement sceneElement = this.ViewModel.GetSceneNode(containerOwnerPath.Node) as SceneElement;
            string str1 = string.Empty;
            if (sceneElement != null)
            {
              if (sceneElement.Name != null)
              {
                str1 = sceneElement.Name;
              }
              else
              {
                string str2 = sceneElement.GetLocalValue(DesignTimeProperties.TestNameProperty) as string;
                if (str2 != null)
                  str1 = str2;
              }
            }
            string str3 = str1 + "." + documentNodePath.ContainerOwnerProperty.Name;
            list.Add(str3);
          }
        }
        StringBuilder stringBuilder = new StringBuilder();
        for (int index = list.Count - 1; index >= 0; --index)
        {
          stringBuilder.Append(list[index]);
          if (index > 0 && !list[index - 1].StartsWith("."))
            stringBuilder.Append("\\");
        }
        return stringBuilder.ToString();
      }
    }

    public virtual IProperty DefaultContentProperty
    {
      get
      {
        return this.Metadata.DefaultContentProperty;
      }
    }

    public ReadOnlyCollection<IPropertyId> ContentProperties
    {
      get
      {
        return this.Metadata.ContentProperties;
      }
    }

    public bool CanAccessProperties
    {
      get
      {
        return this.DocumentNode is DocumentCompositeNode;
      }
    }

    public virtual ISceneNodeCollection<SceneNode> DefaultContent
    {
      get
      {
        IProperty defaultContentProperty = this.DefaultContentProperty;
        return defaultContentProperty == null ? (ISceneNodeCollection<SceneNode>) new SceneNode.EmptySceneNodeCollection<SceneNode>() : this.GetCollectionForProperty((IPropertyId) defaultContentProperty);
      }
    }

    public virtual IProperty InsertionTargetProperty
    {
      get
      {
        return this.DefaultContentProperty;
      }
    }

    internal SceneNodeModelItem ModelItem
    {
      get
      {
        if (this.modelItem == null)
          this.modelItem = new SceneNodeModelItem(this);
        return this.modelItem;
      }
    }

    internal bool IsModelItemCreated
    {
      get
      {
        return this.modelItem != null;
      }
    }

    public virtual IStoryboardContainer StoryboardContainer
    {
      get
      {
        if (!this.IsAttached)
          return this.viewModel.GetSceneNode(this.RootDocumentNode) as IStoryboardContainer;
        for (DocumentNodePath documentNodePath = this.DocumentNodePath; documentNodePath != null; documentNodePath = documentNodePath.GetContainerOwnerPath())
        {
          IType type1 = this.Platform.Metadata.ResolveType(PlatformTypes.Style);
          IType type2 = this.Platform.Metadata.ResolveType(PlatformTypes.FrameworkTemplate);
          Microsoft.Expression.DesignModel.DocumentModel.DocumentNode containerNode = documentNodePath.ContainerNode;
          for (Microsoft.Expression.DesignModel.DocumentModel.DocumentNode node = documentNodePath.Node; node != containerNode; node = (Microsoft.Expression.DesignModel.DocumentModel.DocumentNode) node.Parent)
          {
            if (type1.IsAssignableFrom((ITypeId) node.Type) || type2.IsAssignableFrom((ITypeId) node.Type))
            {
              IStoryboardContainer storyboardContainer = this.ViewModel.GetSceneNode(node) as IStoryboardContainer;
              if (storyboardContainer != null)
                return storyboardContainer;
            }
          }
          IStoryboardContainer storyboardContainer1 = this.ViewModel.GetSceneNode(documentNodePath.ContainerNode) as IStoryboardContainer;
          if (storyboardContainer1 != null)
            return storyboardContainer1;
        }
        return (IStoryboardContainer) null;
      }
    }

    public bool IsLogicalTreeNode
    {
      get
      {
        if (this.Metadata.ContentProperties.Count > 0)
          return true;
        Microsoft.Expression.DesignModel.DocumentModel.DocumentNode documentNode = this.DocumentNode;
        if (documentNode.Parent == null || this.Metadata.ContentProperties.Count > 0)
          return true;
        if (documentNode.IsChild)
          documentNode = (Microsoft.Expression.DesignModel.DocumentModel.DocumentNode) documentNode.Parent;
        if (documentNode.Parent == null)
          return false;
        if (documentNode.SitePropertyKey == null)
          throw new InvalidOperationException(ExceptionStringTable.SceneNodeNotPropertyNode);
        return this.MetadataFactory.GetMetadata(documentNode.Parent.TargetType).ContentProperties.Contains((IPropertyId) documentNode.SitePropertyKey);
      }
    }

    public virtual bool ShouldSerialize
    {
      get
      {
        return (bool) this.GetLocalOrDefaultValue(DesignTimeProperties.ShouldSerializeProperty);
      }
      set
      {
        if (!value)
          this.SetLocalValue(DesignTimeProperties.ShouldSerializeProperty, (object) (bool) (value ? true : false));
        else
          this.ClearLocalValue(DesignTimeProperties.ShouldSerializeProperty);
      }
    }

    public virtual bool IsSelectable
    {
      get
      {
        AnimationEditor animationEditor = this.ViewModel.AnimationEditor;
        if ((animationEditor.ActiveStoryboardTimeline != null || animationEditor.ActiveVisualTrigger != null) && this.StoryboardContainer != animationEditor.ActiveStoryboardContainer)
          return !((SceneNode) animationEditor.ActiveStoryboardContainer).IsAncestorOf(this);
        return true;
      }
    }

    public bool CanNameElement
    {
      get
      {
        if (!(this is StyleNode))
          return !(this is FrameworkTemplateElement);
        return false;
      }
    }

    public virtual IViewObject ViewObject
    {
      get
      {
        if (!this.viewModel.InViewObjectCacheScope)
          return this.ViewModel.GetViewObject(this.DocumentNodePath);
        if (this.cachedViewObject == null)
        {
          this.cachedViewObject = this.ViewModel.GetViewObject(this.DocumentNodePath);
          this.ViewModel.AddViewObjectCachedNode(this);
        }
        return this.cachedViewObject;
      }
    }

    public bool IsViewObjectValid
    {
      get
      {
        if (this.ViewObject != null)
          return this.CheckViewObjectType(this.ViewObject.TargetType);
        return false;
      }
    }

    private Microsoft.Expression.DesignModel.DocumentModel.DocumentNode NodeParent
    {
      get
      {
        Microsoft.Expression.DesignModel.DocumentModel.DocumentNode documentNode = this.DocumentNode;
        if (documentNode.Parent == null)
          return (Microsoft.Expression.DesignModel.DocumentModel.DocumentNode) null;
        if (documentNode.IsChild)
          documentNode = (Microsoft.Expression.DesignModel.DocumentModel.DocumentNode) documentNode.Parent;
        return (Microsoft.Expression.DesignModel.DocumentModel.DocumentNode) documentNode.Parent;
      }
    }

    protected SceneNode PathParent
    {
      get
      {
        SceneNode sceneNode = (SceneNode) null;
        if (this.IsAttached)
        {
          DocumentNodePath documentNodePath = this.DocumentNodePath;
          Microsoft.Expression.DesignModel.DocumentModel.DocumentNode node = this.DocumentNode == documentNodePath.ContainerNode ? documentNodePath.ContainerOwner : this.NodeParent;
          if (node != null)
            sceneNode = this.ViewModel.GetSceneNode(node);
        }
        return sceneNode;
      }
    }

    private Microsoft.Expression.DesignModel.DocumentModel.DocumentNode RootDocumentNode
    {
      get
      {
        Microsoft.Expression.DesignModel.DocumentModel.DocumentNode documentNode = this.node;
        while (documentNode.Parent != null)
          documentNode = (Microsoft.Expression.DesignModel.DocumentModel.DocumentNode) documentNode.Parent;
        return documentNode;
      }
    }

    protected SceneNode()
    {
    }

    private void Initialize(SceneViewModel viewModel, Microsoft.Expression.DesignModel.DocumentModel.DocumentNode node)
    {
      SceneNode sceneNode = (SceneNode) node.SceneNode;
      if (sceneNode != null && sceneNode.ViewModel == viewModel)
        throw new InvalidOperationException(ExceptionStringTable.DocumentNodeIsAlreadyAssociatedWithASceneNode);
      if (viewModel.IsExternal(node))
        throw new InvalidOperationException(ExceptionStringTable.DocumentNodeIsFromADifferentDocument);
      this.viewModel = viewModel;
      this.node = node;
      this.node.SceneNode = (object) this;
    }

    private void Initialize(SceneViewModel viewModel, ITypeId targetType)
    {
      this.Initialize(viewModel, (Microsoft.Expression.DesignModel.DocumentModel.DocumentNode) viewModel.Document.DocumentContext.CreateNode(targetType));
    }

    private bool LastKnownDocumentNodePathIsStillGood()
    {
      DocumentNodePath editingContainerPath = this.viewModel.ActiveEditingContainerPath;
      return editingContainerPath != null && editingContainerPath.Equals((object) this.lastKnownEditingContainerPath);
    }

    public List<SceneNode> GetAllContent()
    {
      List<SceneNode> list = new List<SceneNode>();
      ReadOnlyCollection<IPropertyId> contentProperties = this.ContentProperties;
      for (int index = 0; index < contentProperties.Count; ++index)
      {
        IPropertyId childProperty = contentProperties[index];
        list.AddRange((IEnumerable<SceneNode>) this.GetCollectionForProperty(childProperty));
      }
      return list;
    }

    public virtual ISceneNodeCollection<SceneNode> GetCollectionContainer()
    {
      Microsoft.Expression.DesignModel.DocumentModel.DocumentNode documentNode = this.DocumentNode;
      if (documentNode.Parent != null && documentNode.IsProperty && this.Parent.IsCollectionProperty((IPropertyId) documentNode.SitePropertyKey))
        return (ISceneNodeCollection<SceneNode>) new SceneNode.SceneNodeSingletonCollection<SceneNode>(this.Parent, this.CreateLocalDocumentPropertyNodeReference(documentNode.Parent, (IPropertyId) documentNode.SitePropertyKey));
      SceneNode parent = this.Parent;
      if (parent == null)
        return (ISceneNodeCollection<SceneNode>) null;
      IProperty propertyForChild = parent.GetPropertyForChild(this);
      return parent.GetCollectionForProperty((IPropertyId) propertyForChild);
    }

    public object CreateInstance()
    {
      return this.ViewModel.CreateInstance(this.DocumentNodePath);
    }

    public bool IsCollectionProperty(IPropertyId propertyKey)
    {
      System.Type propertyType = PlatformTypeHelper.GetPropertyType(this.ProjectContext.ResolveProperty(propertyKey));
      IDocumentNodeChildBuilder childBuilder = this.Platform.DocumentNodeChildBuilderFactory.GetChildBuilder(propertyType);
      if (childBuilder != null)
        return childBuilder.GetChildType(propertyType) != (System.Type) null;
      return false;
    }

    public virtual ISceneNodeCollection<SceneNode> GetCollectionForProperty(IPropertyId childProperty)
    {
      DocumentCompositeNode parent = this.DocumentNode as DocumentCompositeNode;
      if (parent == null || childProperty == null)
        return (ISceneNodeCollection<SceneNode>) new SceneNode.EmptySceneNodeCollection<SceneNode>();
      if (this.IsCollectionProperty(childProperty))
        return (ISceneNodeCollection<SceneNode>) new SceneNode.SceneNodeCollection<SceneNode>(this, childProperty);
      return (ISceneNodeCollection<SceneNode>) new SceneNode.SceneNodeSingletonCollection<SceneNode>(this, this.CreateLocalDocumentPropertyNodeReference(parent, childProperty));
    }

    public virtual ISceneNodeCollection<SceneNode> GetCollectionForChild(SceneNode child)
    {
      return this.GetCollectionForProperty((IPropertyId) this.GetPropertyForChild(child));
    }

    public ISceneNodeCollection<SceneNode> GetChildren()
    {
      return (ISceneNodeCollection<SceneNode>) new SceneNode.SceneNodeChildrenCollection<SceneNode>(this);
    }

    public virtual IProperty GetPropertyForChild(SceneNode child)
    {
      if (child == null)
        throw new ArgumentNullException("child");
      DocumentCompositeNode documentCompositeNode = this.DocumentNode as DocumentCompositeNode;
      if (documentCompositeNode == null)
        throw new InvalidOperationException(ExceptionStringTable.SceneNodeMustBeCompositeForLogicalChild);
      DocumentNodePath documentNodePath = child.DocumentNodePath;
      IProperty property;
      if (documentNodePath.ContainerNode == documentNodePath.Node)
      {
        property = documentCompositeNode.TypeResolver.ResolveProperty((IPropertyId) documentNodePath.ContainerOwnerProperty);
      }
      else
      {
        Microsoft.Expression.DesignModel.DocumentModel.DocumentNode documentNode = child.DocumentNode;
        if (documentNode.IsChild && !PlatformTypes.UIElement.IsAssignableFrom((ITypeId) documentNode.Parent.Type) && !PlatformTypes.FrameworkContentElement.IsAssignableFrom((ITypeId) documentNode.Parent.Type))
          documentNode = (Microsoft.Expression.DesignModel.DocumentModel.DocumentNode) documentNode.Parent;
        property = documentNode.SitePropertyKey;
      }
      return property;
    }

    public void Remove()
    {
      ISceneNodeCollection<SceneNode> collectionContainer = this.GetCollectionContainer();
      if (collectionContainer == null)
        throw new InvalidOperationException(ExceptionStringTable.SceneNodeNotChildOfCollection);
      collectionContainer.Remove(this);
    }

    public virtual ReadOnlyCollection<IProperty> GetProperties()
    {
      return this.Metadata.Properties;
    }

    public virtual PropertyState IsSet(IPropertyId propertyKey)
    {
      PropertyReference propertyReference = this.PropertyReferenceFromPropertyKey(propertyKey);
      if (propertyReference == null)
        return PropertyState.Unset;
      return this.IsSet(propertyReference);
    }

    public virtual PropertyState IsSet(PropertyReference propertyReference)
    {
      if (this.DocumentNode is DocumentPrimitiveNode)
        return PropertyState.Unset;
      int nextReferenceStepIndex;
      DocumentPropertyNodeReferenceBase propertyReference1 = this.GetDeepestCompositeOrCollectionNodePropertyReference(propertyReference, false, false, out nextReferenceStepIndex);
      if (propertyReference1 == null || nextReferenceStepIndex != propertyReference.Count - 1)
        return PropertyState.Unset;
      DocumentIndexedPropertyNodeReference propertyNodeReference = propertyReference1 as DocumentIndexedPropertyNodeReference;
      return propertyNodeReference != null && propertyNodeReference.Index >= propertyNodeReference.Parent.Children.Count || (propertyReference1 == null || propertyReference1.Node == null) ? PropertyState.Unset : PropertyState.Set;
    }

    public object GetLocalValue(PropertyReference propertyReference)
    {
      return this.ValidateValueOfType(this.GetLocalValueInternal(propertyReference), propertyReference);
    }

    protected virtual object GetLocalValueInternal(PropertyReference propertyReference)
    {
      return this.GetLocalValueInternal(propertyReference, PropertyContext.None);
    }

    public object GetLocalValue(PropertyReference propertyReference, PropertyContext context)
    {
      object localValueInternal = this.GetLocalValueInternal(propertyReference, context);
      if ((context & PropertyContext.AsDocumentNodes) != PropertyContext.None)
        return localValueInternal;
      return this.ValidateValueOfType(localValueInternal, propertyReference);
    }

    protected virtual object GetLocalValueInternal(PropertyReference propertyReference, PropertyContext context)
    {
      int nextReferenceStepIndex = 0;
      DocumentPropertyNodeReferenceBase propertyReference1 = this.GetDeepestCompositeOrCollectionNodePropertyReference(propertyReference, false, !this.ViewModel.IsForcingBaseValue, out nextReferenceStepIndex);
      if (propertyReference1 == null)
        return (object) null;
      if (nextReferenceStepIndex == propertyReference.Count - 1)
      {
        object obj = (object) null;
        Microsoft.Expression.DesignModel.DocumentModel.DocumentNode node = propertyReference1.Node;
        if (node != null)
          obj = (context & PropertyContext.AsDocumentNodes) == PropertyContext.None ? this.BuildLocalValue(node) : (object) node;
        return obj;
      }
      if ((context & PropertyContext.AsDocumentNodes) != PropertyContext.None)
        return (object) null;
      DocumentPrimitiveNode documentPrimitiveNode = propertyReference1.Node as DocumentPrimitiveNode;
      object target = (object) null;
      if (documentPrimitiveNode == null)
      {
        target = this.BuildLocalValue(propertyReference1.Node);
      }
      else
      {
        DocumentNodeStringValue documentNodeStringValue = documentPrimitiveNode.Value as DocumentNodeStringValue;
        if (documentNodeStringValue != null && documentNodeStringValue.Value != null)
        {
          TypeConverter valueConverter = documentPrimitiveNode.ValueConverter;
          if (valueConverter != null)
          {
            if (valueConverter.CanConvertFrom(typeof (string)))
            {
              try
              {
                target = valueConverter.ConvertFromInvariantString(documentNodeStringValue.Value);
              }
              catch (Exception ex)
              {
                return (object) null;
              }
            }
          }
        }
      }
      if (target != null && this.Platform.Metadata.IsExpression(target.GetType()))
        return (object) null;
      return propertyReference.PartialGetValue(target, nextReferenceStepIndex + 1, propertyReference.Count - 1);
    }

    public object GetLocalValue(IPropertyId propertyKey)
    {
      return this.GetLocalValue(propertyKey, PropertyContext.None);
    }

    public object GetLocalValue(IPropertyId propertyKey, PropertyContext propertyContext)
    {
      object obj = (object) null;
      DocumentCompositeNode documentCompositeNode = this.DocumentNode as DocumentCompositeNode;
      if (documentCompositeNode == null)
        throw new InvalidOperationException(ExceptionStringTable.SceneNodeMustBeCompositeForProperties);
      Microsoft.Expression.DesignModel.DocumentModel.DocumentNode valueNode = documentCompositeNode.Properties[propertyKey];
      if ((propertyContext & PropertyContext.AsDocumentNodes) != PropertyContext.None)
        obj = (object) valueNode;
      else if (valueNode != null)
        obj = !valueNode.Type.IsExpression ? this.BuildLocalValue(valueNode) : (object) valueNode;
      return this.ValidateValueOfType(obj, propertyKey);
    }

    public bool IsValueExpression(IPropertyId propertyKey)
    {
      DocumentCompositeNode documentCompositeNode = this.DocumentNode as DocumentCompositeNode;
      if (documentCompositeNode == null)
        throw new InvalidOperationException(ExceptionStringTable.SceneNodeMustBeCompositeForProperties);
      Microsoft.Expression.DesignModel.DocumentModel.DocumentNode documentNode = documentCompositeNode.Properties[propertyKey];
      return documentNode != null && documentNode.Type.IsExpression;
    }

    public object GetLocalOrDefaultValue(IPropertyId propertyKey)
    {
      IPropertyId propertyKey1 = (IPropertyId) this.ProjectContext.ResolveProperty(propertyKey);
      DocumentCompositeNode documentCompositeNode = this.DocumentNode as DocumentCompositeNode;
      if (documentCompositeNode == null)
        throw new InvalidOperationException(ExceptionStringTable.SceneNodeMustBeCompositeForProperties);
      if (documentCompositeNode.Properties[propertyKey1] != null)
        return this.GetLocalValue(propertyKey1);
      return this.GetDefaultValue(propertyKey1);
    }

    public object GetLocalOrDefaultValueAsWpf(IPropertyId propertyKey)
    {
      return this.ConvertToWpfValue(this.GetLocalOrDefaultValue(propertyKey));
    }

    public object GetLocalOrDefaultValue(PropertyReference propertyReference)
    {
      object target;
      if (this.IsSet(propertyReference) == PropertyState.Set)
      {
        target = this.GetLocalValue(propertyReference);
      }
      else
      {
        target = this.GetDefaultValue((IPropertyId) propertyReference[0]);
        if (propertyReference.Count > 1)
          target = propertyReference.PartialGetValue(target, 1, propertyReference.Count - 1);
        if (target == null)
          target = this.GetDefaultValue((IPropertyId) propertyReference[propertyReference.Count - 1]);
      }
      return this.ValidateValueOfType(target, propertyReference);
    }

    public object GetLocalOrDefaultValueAsWpf(PropertyReference propertyReference)
    {
      return this.ConvertToWpfValue(this.GetLocalOrDefaultValue(propertyReference));
    }

    public object GetDefaultValue(IPropertyId propertyKey)
    {
      return this.ValidateValueOfType(this.GetDefaultValueInternal(propertyKey), propertyKey);
    }

    protected virtual object GetDefaultValueInternal(IPropertyId propertyKey)
    {
      IProperty resolvedProperty = this.ProjectContext.ResolveProperty(propertyKey);
      ReferenceStep referenceStep = resolvedProperty as ReferenceStep;
      if (referenceStep != null)
        return referenceStep.GetDefaultValue(this.TargetType);
      if (resolvedProperty.MemberType == MemberType.DesignTimeProperty)
        return DesignTimeProperties.GetDocumentOnlyDefaultValue(resolvedProperty);
      throw new ArgumentException(ExceptionStringTable.SceneNodeCannotGetDefaultValue);
    }

    public virtual void SetValue(IPropertyId propertyKey, object valueToSet)
    {
      this.SetValue(this.PropertyReferenceFromPropertyKey(propertyKey), valueToSet);
    }

    public virtual void SetValue(PropertyReference propertyReference, object valueToSet)
    {
      this.ModifyValue(propertyReference, valueToSet, SceneNode.Modification.SetValue, -1);
    }

    public virtual void InsertValue(PropertyReference propertyReference, int index, object valueToAdd)
    {
      this.ModifyValue(propertyReference, valueToAdd, SceneNode.Modification.InsertValue, index);
    }

    public virtual void RemoveValueAt(PropertyReference propertyReference, int index)
    {
      this.ModifyValue(propertyReference, (object) null, SceneNode.Modification.RemoveValue, index);
    }

    public void SetLocalValueAsWpf(IPropertyId propertyKey, object valueToSet)
    {
      Microsoft.Expression.DesignModel.DocumentModel.DocumentNode valueNode = this.ConvertFromWpfValueToDocumentNode(valueToSet);
      this.SetLocalValue(propertyKey, valueNode);
    }

    public void SetLocalValue(IPropertyId propertyKey, object valueToSet)
    {
      if (propertyKey == null)
        throw new ArgumentNullException("propertyKey");
      Microsoft.Expression.DesignModel.DocumentModel.DocumentNode valueNode = valueToSet == null ? this.DocumentContext.CreateNode(PlatformTypeHelper.GetPropertyType(this.ProjectContext.ResolveProperty(propertyKey)), (object) null) : this.DocumentContext.CreateNode(valueToSet.GetType(), valueToSet);
      this.SetLocalValue(propertyKey, valueNode);
    }

    public void SetLocalValue(IPropertyId propertyKey, Microsoft.Expression.DesignModel.DocumentModel.DocumentNode valueNode)
    {
      if (propertyKey == null)
        throw new ArgumentNullException("propertyKey");
      DocumentCompositeNode documentCompositeNode = this.DocumentNode as DocumentCompositeNode;
      if (documentCompositeNode == null)
        throw new InvalidOperationException(ExceptionStringTable.SceneNodeMustBeCompositeForProperties);
      Microsoft.Expression.DesignModel.DocumentModel.DocumentNode nodeBeingRemoved = documentCompositeNode.Properties[propertyKey];
      if (nodeBeingRemoved != null)
        this.ViewModel.UpdateViewModelStateForNodeRemoving(this.DocumentNodePath, nodeBeingRemoved);
      documentCompositeNode.Properties[propertyKey] = valueNode;
    }

    public virtual void ClearValue(IPropertyId propertyKey)
    {
      this.ClearValue(this.PropertyReferenceFromPropertyKey(propertyKey));
    }

    public virtual void ClearValue(PropertyReference propertyReference)
    {
      this.ModifyValue(propertyReference, (object) null, SceneNode.Modification.ClearValue, -1);
    }

    public virtual void ClearLocalValue(IPropertyId propertyKey)
    {
      if (propertyKey == null)
        throw new ArgumentNullException("propertyKey");
      DocumentCompositeNode documentCompositeNode = this.DocumentNode as DocumentCompositeNode;
      if (documentCompositeNode == null)
        throw new InvalidOperationException(ExceptionStringTable.SceneNodeMustBeCompositeForProperties);
      documentCompositeNode.Properties[propertyKey] = (Microsoft.Expression.DesignModel.DocumentModel.DocumentNode) null;
    }

    public BindingSceneNode GetBinding(IPropertyId propertyKey)
    {
      return this.GetLocalValueAsSceneNode(propertyKey) as BindingSceneNode;
    }

    public void SetBinding(IPropertyId propertyKey, BindingSceneNode binding)
    {
      this.SetValue(propertyKey, (object) binding.DocumentNode);
    }

    public virtual void EnsureNodeTree(PropertyReference propertyReference, bool allowFinalExpressions, bool ensureDeepestPossible)
    {
      this.EnsureTransformIfNeeded(propertyReference);
      int nextReferenceStepIndex;
      DocumentPropertyNodeReferenceBase propertyReference1 = this.GetDeepestCompositeOrCollectionNodePropertyReference(propertyReference, true, false, out nextReferenceStepIndex);
      while (nextReferenceStepIndex < propertyReference.Count - 1 && this.EnsureNode(propertyReference, ensureDeepestPossible, nextReferenceStepIndex, propertyReference1))
        propertyReference1 = this.GetDeepestCompositeOrCollectionNodePropertyReference(propertyReference, true, false, out nextReferenceStepIndex);
      if (allowFinalExpressions || propertyReference1.Node == null || !propertyReference1.Node.Type.IsExpression)
        return;
      this.EnsureNode(propertyReference, ensureDeepestPossible, nextReferenceStepIndex, propertyReference1);
    }

    protected virtual object PartialGetValue(PropertyReference propertyReference, int initialStepIndex, int finalStepIndex)
    {
      object target = this.ViewObject != null ? this.ViewObject.PlatformSpecificObject : (object) null;
      if (!this.IsViewObjectValid)
        target = !(target is Microsoft.Expression.DesignModel.DocumentModel.DocumentNode) ? (object) null : this.ViewModel.CreateInstance(this.DocumentNodePath);
      propertyReference = DesignTimeProperties.GetAppliedShadowPropertyReference(propertyReference, (ITypeId) this.Type);
      return propertyReference.PartialGetValue(target, initialStepIndex, finalStepIndex);
    }

    private bool EnsureNode(PropertyReference propertyReference, bool ensureDeepestPossible, int nextReferenceStepIndex, DocumentPropertyNodeReferenceBase reference)
    {
      if (reference.Node != null && !reference.Node.Type.IsExpression && !ensureDeepestPossible)
        return false;
      object obj = this.PartialGetValue(propertyReference, 0, nextReferenceStepIndex);
      Microsoft.Expression.DesignModel.DocumentModel.DocumentNode documentNode = (Microsoft.Expression.DesignModel.DocumentModel.DocumentNode) null;
      PropertyReference propertyReference1 = (PropertyReference) null;
      if (PlatformTypes.IsInstance(obj, PlatformTypes.ImageBrush, (ITypeResolver) this.ProjectContext) && this.IsValueExpression((IPropertyId) propertyReference.FirstStep))
      {
        propertyReference1 = propertyReference.Subreference(0, nextReferenceStepIndex);
        SceneNode valueAsSceneNode = this.GetLocalValueAsSceneNode(propertyReference1, true);
        if (valueAsSceneNode != null)
        {
          DocumentNodePath valueAsDocumentNode = valueAsSceneNode.GetLocalValueAsDocumentNode(ImageBrushNode.ImageSourceProperty);
          if (valueAsDocumentNode != null && valueAsDocumentNode.Node is DocumentPrimitiveNode)
            documentNode = valueAsDocumentNode.Node;
        }
      }
      Microsoft.Expression.DesignModel.DocumentModel.DocumentNode node = this.DocumentContext.CreateNode(obj == null ? PlatformTypeHelper.GetPropertyType((IProperty) propertyReference[nextReferenceStepIndex]) : obj.GetType(), obj);
      reference.Node = node;
      if (documentNode != null)
        this.SetValue(propertyReference1.Append(ImageBrushNode.ImageSourceProperty), (object) documentNode);
      return true;
    }

    public SceneNode GetCommonAncestor(SceneNode other)
    {
      List<SceneNode> ancestors1 = this.GetAncestors(true);
      List<SceneNode> ancestors2 = other.GetAncestors(true);
      SceneNode sceneNode = (SceneNode) null;
      int index1 = ancestors1.Count - 1;
      for (int index2 = ancestors2.Count - 1; index1 >= 0 && index2 >= 0 && ancestors1[index1] == ancestors2[index2]; --index2)
      {
        sceneNode = ancestors1[index1];
        --index1;
      }
      return sceneNode;
    }

    internal List<SceneNode> GetAncestors(bool includeSelf)
    {
      List<SceneNode> list = new List<SceneNode>();
      for (SceneNode sceneNode = includeSelf ? this : this.Parent; sceneNode != null; sceneNode = sceneNode.Parent)
        list.Add(sceneNode);
      return list;
    }

    public bool IsAncestorOf(SceneNode descendant)
    {
      for (; descendant != null; descendant = descendant.Parent)
      {
        if (descendant == this || descendant.DocumentNode.Parent != null && descendant.DocumentNode.Parent == this.DocumentNode)
          return true;
      }
      return false;
    }

    public SceneNode FindTargetTypeAncestor(ITypeId type)
    {
      for (SceneNode parent = this.Parent; parent != null; parent = parent.Parent)
      {
        if (type.IsAssignableFrom((ITypeId) parent.Type))
          return parent;
      }
      return (SceneNode) null;
    }

    public SceneNode FindSceneNodeTypeAncestor(System.Type nodeType)
    {
      for (SceneNode parent = this.Parent; parent != null; parent = parent.Parent)
      {
        if (nodeType.IsAssignableFrom(parent.GetType()))
          return parent;
      }
      return (SceneNode) null;
    }

    public T FindSceneNodeTypeAncestor<T>() where T : SceneNode
    {
      return (T) this.FindSceneNodeTypeAncestor(typeof (T));
    }

    public void EnsureNamed()
    {
      if (!string.IsNullOrEmpty(this.Name) && !this.Name.StartsWith("~", StringComparison.Ordinal) || !this.CanNameElement)
        return;
      this.Name = new SceneNodeIDHelper(this.ViewModel, this.StoryboardContainer as SceneNode ?? this.ViewModel.GetSceneNode(this.RootDocumentNode)).GetValidElementID(this, SceneNodeIDHelper.DefaultNamePrefixForType((ITypeId) this.Type));
    }

    public void FlushViewObjectCache()
    {
      this.cachedViewObject = (IViewObject) null;
    }

    private bool CheckViewObjectType(System.Type type)
    {
      if (this.viewModel.DefaultView == null)
        return false;
      IInstanceBuilder builder = this.Platform.InstanceBuilderFactory.GetBuilder(this.TargetType);
      IUserControlInstanceBuilder controlInstanceBuilder = builder as IUserControlInstanceBuilder;
      System.Type replacementType = builder.ReplacementType;
      System.Type type1 = !(replacementType != (System.Type) null) ? (!this.IsSubclassDefinition ? this.TrueTargetType : (this.viewModel.DefaultView.InstanceBuilderContext.RootTargetTypeReplacement == null || !(this.TargetType == this.viewModel.DefaultView.InstanceBuilderContext.RootTargetTypeReplacement.SourceType.RuntimeType) ? this.TargetType : this.viewModel.DefaultView.InstanceBuilderContext.RootTargetTypeReplacement.ReplacementType.RuntimeType)) : replacementType;
      if (type == type1)
        return true;
      if (controlInstanceBuilder != null)
        return type == controlInstanceBuilder.PreviewControlType;
      return false;
    }

    public object GetComputedValue(PropertyReference propertyReference)
    {
      return this.ValidateValueOfType(this.GetComputedValueInternal(propertyReference), propertyReference);
    }

    protected virtual object GetComputedValueInternal(PropertyReference propertyReference)
    {
      if (!this.IsAttached)
        throw new InvalidOperationException(ExceptionStringTable.SceneNodeNoDocumentTreeForComputedValue);
      ReferenceStep firstStep = propertyReference.FirstStep;
      StyleNode styleNode = this as StyleNode;
      System.Type type = styleNode != null ? styleNode.StyleTargetType : this.TargetType;
      System.Type replacementType = this.Platform.InstanceBuilderFactory.GetBuilder(type).ReplacementType;
      if (replacementType != (System.Type) null && !firstStep.DeclaringType.IsAssignableFrom((ITypeId) this.Platform.Metadata.GetType(replacementType)) && DesignTimeProperties.GetShadowProperty((IProperty) propertyReference[0], (ITypeId) this.Type) == null)
        return this.GetLocalOrDefaultValue(propertyReference);
      if (this.ViewModel != null && this.ViewModel.DefaultView != null && (this.ViewModel.DefaultView.InstanceBuilderContext != null && this.ViewModel.DefaultView.InstanceBuilderContext.RootTargetTypeReplacement != null) && type == this.ViewModel.DefaultView.InstanceBuilderContext.RootTargetTypeReplacement.SourceType.RuntimeType)
      {
        ReferenceStep referenceStep1 = this.ViewModel.DefaultView.InstanceBuilderContext.RootTargetTypeReplacement.GetReplacementProperty((IProperty) firstStep) as ReferenceStep;
        if (referenceStep1 != firstStep && referenceStep1 != null)
        {
          List<ReferenceStep> steps = new List<ReferenceStep>();
          foreach (ReferenceStep referenceStep2 in propertyReference.ReferenceSteps)
            steps.Add(referenceStep2);
          steps[0] = referenceStep1;
          return this.GetRawComputedValue(new PropertyReference(steps));
        }
      }
      return this.GetRawComputedValue(propertyReference);
    }

    public object GetComputedValue(IPropertyId propertyKey)
    {
      PropertyReference propertyReference = this.PropertyReferenceFromPropertyKey(propertyKey);
      if (propertyReference == null)
        return (object) null;
      return this.GetComputedValue(propertyReference);
    }

    public object GetRawComputedValue(PropertyReference propertyReference)
    {
      return this.ValidateValueOfType(this.GetRawComputedValueInternal(propertyReference), propertyReference);
    }

    protected object GetCurrentValueFromPropertyReference(PropertyReference propertyReference, object viewObject)
    {
      for (int endIndex = 0; endIndex < propertyReference.Count - 1; ++endIndex)
      {
        IType propertyType = propertyReference[endIndex].PropertyType;
        if (PlatformTypes.Transform.IsAssignableFrom((ITypeId) propertyType))
        {
          if (endIndex - 2 >= 0 && PlatformTypes.TransformCollection.IsAssignableFrom((ITypeId) propertyReference[endIndex].DeclaringType))
            endIndex -= 2;
          Transform wpfTransform = Transform.Identity;
          if (!this.TryGetCanonicalTransform(propertyReference.Subreference(0, endIndex), out wpfTransform, true, false))
          {
            CanonicalTransform canonicalTransform = new CanonicalTransform(wpfTransform);
            return propertyReference.Subreference(endIndex + 1).GetCurrentValue(canonicalTransform.GetPlatformTransform(this.Platform.GeometryHelper));
          }
          break;
        }
        if (PlatformTypes.Transform3D.IsAssignableFrom((ITypeId) propertyType))
        {
          Transform3D transform = (Transform3D) this.GetComputedValueAsWpf(propertyReference.Subreference(0, endIndex));
          if (!CanonicalTransform3D.IsCanonical(transform))
          {
            CanonicalTransform3D canonicalTransform3D = new CanonicalTransform3D(transform);
            return this.ConvertFromWpfValue(this.ConvertToWpfPropertyReference(propertyReference.Subreference(endIndex + 1)).GetCurrentValue((object) canonicalTransform3D.ToTransform()));
          }
          break;
        }
      }
      return propertyReference.GetCurrentValue(viewObject);
    }

    protected virtual object GetRawComputedValueInternal(PropertyReference propertyReference)
    {
      if (this.ViewModel.DefaultView.IsValid)
      {
        object obj = this.ViewObject != null ? this.ViewObject.PlatformSpecificObject : (object) null;
        if (!this.IsViewObjectValid)
        {
          if (obj is Microsoft.Expression.DesignModel.DocumentModel.DocumentNode)
          {
            using (this.viewModel.ForceUseShadowProperties())
              obj = this.viewModel.CreateInstance(this.DocumentNodePath);
            if (obj != null && !this.CheckViewObjectType(obj.GetType()))
              obj = (object) null;
          }
          else
            obj = (object) null;
        }
        if (obj != null)
        {
          PropertyReference propertyReference1 = propertyReference;
          if (this.ProjectContext.IsCapabilitySet(PlatformCapability.IsWpf) || this.viewModel.ActiveStoryboardTimeline == null || !this.viewModel.AnimationEditor.IsCurrentlyAnimated(this, propertyReference, propertyReference.Count) || DesignTimeProperties.IsShadowedInSilverlightAnimation((IProperty) propertyReference.LastStep))
            propertyReference1 = DesignTimeProperties.GetAppliedShadowPropertyReference(propertyReference, (ITypeId) this.Type);
          object propertyReference2 = this.GetCurrentValueFromPropertyReference(propertyReference1, obj);
          if (propertyReference1 == propertyReference || propertyReference1.Count != 1)
            return propertyReference2;
          if (!DesignTimeProperties.UseShadowPropertyForInstanceBuilding(this.DocumentContext.TypeResolver, (IPropertyId) propertyReference1[0]))
            return SceneNode.GetComputedValueWithShadowCoercion(propertyReference, propertyReference1, obj);
          DependencyPropertyReferenceStep propertyReferenceStep = propertyReference[0] as DependencyPropertyReferenceStep;
          if (propertyReferenceStep != null)
          {
            switch (propertyReferenceStep.GetValueSource(obj))
            {
              case BaseValueSource.DefaultStyle:
              case BaseValueSource.DefaultStyleTrigger:
              case BaseValueSource.Style:
              case BaseValueSource.TemplateTrigger:
              case BaseValueSource.StyleTrigger:
              case BaseValueSource.ImplicitStyleReference:
              case BaseValueSource.ParentTemplate:
              case BaseValueSource.ParentTemplateTrigger:
                return propertyReferenceStep.GetValue(obj);
            }
          }
          ReferenceStep referenceStep1 = propertyReference1[0];
          if (referenceStep1.IsSet(obj))
            return propertyReference2;
          object currentValue = propertyReference.GetCurrentValue(obj);
          ReferenceStep referenceStep2 = (ReferenceStep) DesignTimeProperties.GetShadowPeerProperty((IProperty) referenceStep1);
          if (referenceStep2 == null)
          {
            if (DesignTimeProperties.IsShadowValuePreferred((IProperty) referenceStep1))
              return propertyReference2;
            return currentValue;
          }
          if (!referenceStep2.IsSet(obj))
            return currentValue;
          return propertyReference2;
        }
      }
      object obj1 = this.GetLocalOrDefaultValue(propertyReference);
      PlatformTypes platformTypes = (PlatformTypes) this.Platform.Metadata;
      if (obj1 != null && platformTypes.IsExpression(obj1.GetType()))
        obj1 = this.GetDefaultValue((IPropertyId) propertyReference[propertyReference.Count - 1]);
      return obj1;
    }

    internal static object GetComputedValueWithShadowCoercion(PropertyReference propertyReference, PropertyReference shadowReference, object viewObject)
    {
      DependencyPropertyReferenceStep propertyReferenceStep1 = shadowReference[0] as DependencyPropertyReferenceStep;
      DependencyPropertyReferenceStep propertyReferenceStep2 = propertyReference[0] as DependencyPropertyReferenceStep;
      if (propertyReferenceStep1 == null || propertyReferenceStep2 == null)
        return shadowReference.GetValue(viewObject);
      BaseValueSource valueSource1 = propertyReferenceStep1.GetValueSource(viewObject);
      BaseValueSource valueSource2 = propertyReferenceStep2.GetValueSource(viewObject);
      if (valueSource1 != BaseValueSource.Default && valueSource1 != BaseValueSource.Inherited || (valueSource2 == BaseValueSource.Default || valueSource2 == BaseValueSource.Inherited) && valueSource1 != BaseValueSource.Default)
        return shadowReference.GetValue(viewObject);
      return propertyReference.GetValue(viewObject);
    }

    public object GetComputedValueAsWpf(IPropertyId propertyKey)
    {
      return this.ValidateValueOfType(this.GetComputedValueAsWpfInternal(propertyKey), propertyKey);
    }

    protected virtual object GetComputedValueAsWpfInternal(IPropertyId propertyKey)
    {
      return this.ConvertToWpfValue(this.GetComputedValue(propertyKey));
    }

    public object GetComputedValueAsWpf(PropertyReference propertyReference)
    {
      if (!this.ProjectContext.IsCapabilitySet(PlatformCapability.IsWpf))
        return this.ConvertToWpfValue(this.GetComputedValue(this.ConvertFromWpfPropertyReference(propertyReference)));
      return this.GetComputedValue(propertyReference);
    }

    public object GetDefaultValueAsWpf(IPropertyId propertyKey)
    {
      object obj = this.GetDefaultValue((IPropertyId) this.ProjectContext.ResolveProperty(propertyKey));
      if (!this.ProjectContext.IsCapabilitySet(PlatformCapability.IsWpf))
        obj = this.ConvertToWpfValue(obj);
      return obj;
    }

    public object GetLocalValueAsWpf(IPropertyId propertyKey)
    {
      return this.ConvertToWpfValue(this.GetLocalValue(propertyKey));
    }

    public object GetLocalValueAsWpf(PropertyReference propertyReference)
    {
      if (!this.ProjectContext.IsCapabilitySet(PlatformCapability.IsWpf))
        return this.ConvertToWpfValue(this.GetLocalValue(propertyReference));
      return this.GetLocalValue(propertyReference);
    }

    public void SetValueAsWpf(IPropertyId propertyKey, object valueToSet)
    {
      this.SetValue(propertyKey, this.ConvertFromWpfValue(valueToSet));
    }

    public void SetValueAsWpf(PropertyReference propertyReference, object valueToSet)
    {
      if (!this.ProjectContext.IsCapabilitySet(PlatformCapability.IsWpf))
      {
        PropertyReference propertyReference1 = propertyReference;
        Microsoft.Expression.DesignModel.DocumentModel.DocumentNode documentNode = valueToSet as Microsoft.Expression.DesignModel.DocumentModel.DocumentNode;
        if (documentNode == null || documentNode.Type.PlatformMetadata != this.ProjectContext.PlatformMetadata)
          valueToSet = this.ConvertFromWpfValue(valueToSet);
        this.SetValue(this.ConvertFromWpfPropertyReference(propertyReference1), valueToSet);
      }
      else
        this.SetValue(propertyReference, valueToSet);
    }

    public void ClearValueAsWpf(PropertyReference propertyReference)
    {
      if (!this.ProjectContext.IsCapabilitySet(PlatformCapability.IsWpf))
        this.ClearValue(this.ConvertFromWpfPropertyReference(propertyReference));
      else
        this.ClearValue(propertyReference);
    }

    public void InsertValueAsWpf(PropertyReference propertyReference, int index, object valueToAdd)
    {
      if (!this.ProjectContext.IsCapabilitySet(PlatformCapability.IsWpf))
      {
        PropertyReference propertyReference1 = propertyReference;
        Microsoft.Expression.DesignModel.DocumentModel.DocumentNode documentNode = valueToAdd as Microsoft.Expression.DesignModel.DocumentModel.DocumentNode;
        if (documentNode == null || documentNode.Type.PlatformMetadata != this.ProjectContext.PlatformMetadata)
          valueToAdd = this.ConvertFromWpfValue(valueToAdd);
        this.InsertValue(this.ConvertFromWpfPropertyReference(propertyReference1), index, valueToAdd);
      }
      else
        this.InsertValue(propertyReference, index, valueToAdd);
    }

    protected object ConvertToWpfValue(object value)
    {
      return this.ViewModel.DefaultView.ConvertToWpfValue(value);
    }

    protected Microsoft.Expression.DesignModel.DocumentModel.DocumentNode ConvertFromWpfValueToDocumentNode(object value)
    {
      return this.ViewModel.DefaultView.ConvertFromWpfValueAsDocumentNode(value);
    }

    protected object ConvertFromWpfValue(object value)
    {
      return this.ViewModel.DefaultView.ConvertFromWpfValue(value);
    }

    protected PropertyReference ConvertFromWpfPropertyReference(PropertyReference propertyReference)
    {
      return this.ViewModel.DefaultView.ConvertFromWpfPropertyReference(propertyReference);
    }

    protected PropertyReference ConvertToWpfPropertyReference(PropertyReference propertyReference)
    {
      return this.ViewModel.DefaultView.ConvertToWpfPropertyReference(propertyReference);
    }

    protected Microsoft.Expression.DesignModel.DocumentModel.DocumentNode CreateNode(object value)
    {
      Microsoft.Expression.DesignModel.DocumentModel.DocumentNode documentNode = value as Microsoft.Expression.DesignModel.DocumentModel.DocumentNode;
      if (documentNode == null)
        return this.DocumentContext.CreateNode(value != null ? value.GetType() : typeof (object), value);
      if (documentNode.Parent == null && this.DocumentContext == documentNode.Context)
        return documentNode;
      return documentNode.Clone(this.DocumentContext);
    }

    protected DocumentPropertyNodeReferenceBase GetDeepestCompositeOrCollectionNodePropertyReference(PropertyReference propertyReference, bool visualTriggerOnly, bool evaluateExpressions, out int nextReferenceStepIndex)
    {
      DocumentNodePath parentPath;
      return this.GetDeepestCompositeOrCollectionNodePropertyReference(propertyReference, visualTriggerOnly, evaluateExpressions, out nextReferenceStepIndex, out parentPath);
    }

    protected DocumentPropertyNodeReferenceBase GetDeepestCompositeOrCollectionNodePropertyReference(PropertyReference propertyReference, bool visualTriggerOnly, bool evaluateExpressions, out int nextReferenceStepIndex, out DocumentNodePath parentPath)
    {
      if (this.DocumentNode is DocumentPrimitiveNode)
        throw new InvalidOperationException(ExceptionStringTable.SceneNodeMustBeCompositeForProperties);
      DocumentPropertyNodeReferenceBase nodeReferenceBase = (DocumentPropertyNodeReferenceBase) null;
      parentPath = this.DocumentNodePath;
      nextReferenceStepIndex = 0;
      if (propertyReference.Count > 1)
        parentPath = this.ResolveValue(parentPath, propertyReference.GetPropertyKeys(), propertyReference.Count - 1, evaluateExpressions, visualTriggerOnly, out nextReferenceStepIndex);
      Microsoft.Expression.DesignModel.DocumentModel.DocumentNode node1 = parentPath.Node;
      ReferenceStep referenceStep = (ReferenceStep) null;
      DependencyPropertyReferenceStep propertyReferenceStep1 = propertyReference[nextReferenceStepIndex] as DependencyPropertyReferenceStep;
      if (propertyReferenceStep1 != null && propertyReferenceStep1.TargetType != (System.Type) null && !propertyReferenceStep1.TargetType.IsAssignableFrom(node1.TargetType))
      {
        DependencyPropertyReferenceStep propertyReferenceStep2 = node1.Type.GetMember(MemberType.DependencyProperty, propertyReferenceStep1.Name, MemberAccessTypes.Public) as DependencyPropertyReferenceStep;
        if (propertyReferenceStep2 != null && propertyReferenceStep2.DependencyProperty == propertyReferenceStep1.DependencyProperty)
          referenceStep = (ReferenceStep) propertyReferenceStep2;
      }
      if (referenceStep == null)
        referenceStep = propertyReference[nextReferenceStepIndex];
      if (nextReferenceStepIndex > 0 && (node1 is DocumentPrimitiveNode || referenceStep.TargetType != (System.Type) null && !referenceStep.TargetType.IsAssignableFrom(node1.TargetType)))
      {
        Microsoft.Expression.DesignModel.DocumentModel.DocumentNode node2;
        do
        {
          node2 = parentPath.Node;
          parentPath = parentPath.GetParent();
          --nextReferenceStepIndex;
        }
        while (nextReferenceStepIndex > 0 && parentPath != null && (node2.Parent != null && !node2.IsProperty));
        node1 = parentPath.Node;
        IProperty sitePropertyKey = node2.SitePropertyKey;
        if (sitePropertyKey != null)
          nodeReferenceBase = DocumentNodeResolver.CreateCompositeOrCollectionNodePropertyReference(this, (DocumentCompositeNode) node1, (IPropertyId) sitePropertyKey, this.GetActiveLocalPropertyTrigger(), new DocumentNodeResolver.ShouldUseTrigger(this.ShouldUseTrigger), visualTriggerOnly);
      }
      if (nodeReferenceBase == null)
        nodeReferenceBase = DocumentNodeResolver.CreateCompositeOrCollectionNodePropertyReference(this, (DocumentCompositeNode) node1, (IPropertyId) referenceStep, this.GetActiveLocalPropertyTrigger(), new DocumentNodeResolver.ShouldUseTrigger(this.ShouldUseTrigger), visualTriggerOnly);
      return nodeReferenceBase;
    }

    protected virtual void OnChildAdding(SceneNode child)
    {
    }

    protected virtual void OnChildAdded(SceneNode child)
    {
    }

    protected virtual void OnChildRemoving(SceneNode child)
    {
      this.ViewModel.UpdateViewModelStateForNodeRemoving(child.DocumentNodePath, child.DocumentNode);
    }

    protected virtual void OnChildRemoved(SceneNode child)
    {
    }

    protected virtual void ModifyValue(PropertyReference propertyReference, object valueToSet, SceneNode.Modification modification, int index)
    {
      modification = this.VerifyModificationOperator(ref propertyReference, valueToSet, modification, index);
      if (modification == SceneNode.Modification.NoOP)
        return;
      this.InternalModifyValue(propertyReference, valueToSet, modification, index, true);
    }

    protected SceneNode.Modification VerifyModificationOperator(ref PropertyReference propertyReference, object valueToSet, SceneNode.Modification modification, int index)
    {
      AnimationEditor animationEditor = this.ViewModel.AnimationEditor;
      ViewState viewState1 = this.ViewModel.DefaultView.GetViewState(this);
      ViewState viewState2 = ViewState.ElementValid | ViewState.AncestorValid | ViewState.SubtreeValid;
      if (this.IsAttached && (viewState1 & viewState2) == viewState2)
      {
        if (modification == SceneNode.Modification.SetValue && this.ViewObject != null && (!Base2DElement.RenderTransformOriginProperty.Equals((object) propertyReference.LastStep) && !this.ViewModel.IsForcingDefaultSetValue) && (this.ViewModel.IsForcingBaseValue || !this.ViewModel.AnimationEditor.IsRecording && this.ViewModel.ActiveVisualTrigger == null))
        {
          object objB = (object) null;
          bool flag1 = false;
          if (propertyReference.Count == 1)
          {
            objB = this.GetDefaultValue((IPropertyId) propertyReference.FirstStep);
          }
          else
          {
            object computedValue = this.GetComputedValue(propertyReference.Subreference(0, propertyReference.Count - 2));
            if (computedValue != null)
            {
              System.Type type = computedValue.GetType();
              if (type.IsValueType)
                flag1 = true;
              else
                objB = propertyReference.LastStep.GetDefaultValue(type);
            }
          }
          if (!flag1 && object.Equals(valueToSet, objB))
          {
            if (!PlatformTypes.Style.IsAssignableFrom((ITypeId) this.Type))
            {
              try
              {
                PropertyReference propertyReference1 = DesignTimeProperties.GetAppliedShadowPropertyReference(propertyReference, (ITypeId) this.Type);
                bool flag2 = propertyReference1 != propertyReference;
                this.ViewObject.ClearValue(propertyReference);
                if (flag2)
                  this.ViewObject.ClearValue(propertyReference1);
                this.ViewModel.DefaultView.InstanceBuilderContext.ViewNodeManager.Invalidate(this.DocumentNode, new InstanceState((IProperty) propertyReference[0]));
                object currentValue = this.ViewObject.GetCurrentValue(propertyReference);
                object objA = flag2 ? this.ViewObject.GetCurrentValue(propertyReference1) : (object) null;
                if (object.Equals(currentValue, valueToSet) && (!flag2 || object.Equals(objA, valueToSet)))
                {
                  object objToInspect = propertyReference.Count <= 1 ? Activator.CreateInstance(this.ViewObject.TargetType) : Activator.CreateInstance(this.ViewObject.GetCurrentValue(propertyReference1.Subreference(0, propertyReference.Count - 2)).GetType());
                  if (object.Equals(propertyReference.LastStep.GetCurrentValue(objToInspect), valueToSet))
                    modification = SceneNode.Modification.ClearValue;
                }
                else
                {
                  object valueToSet1 = !object.Equals(currentValue, valueToSet) ? currentValue : objA;
                  propertyReference.SetValue(this.ViewObject.PlatformSpecificObject, valueToSet1);
                  if (flag2)
                    propertyReference1.SetValue(this.ViewObject.PlatformSpecificObject, valueToSet1);
                }
              }
              catch
              {
              }
            }
          }
        }
        if (modification == SceneNode.Modification.ClearValue && this.ViewObject != null && !PlatformTypes.Style.IsAssignableFrom((ITypeId) this.Type) && (PlatformTypes.Transform.Equals((object) propertyReference.FirstStep.PropertyType) || PlatformTypes.Projection.IsAssignableFrom((ITypeId) propertyReference.FirstStep.PropertyType)) && (this.ViewModel.IsForcingBaseValue || !this.ViewModel.AnimationEditor.IsRecording && this.ViewModel.ActiveVisualTrigger == null))
        {
          PropertyReference propertyReference1 = DesignTimeProperties.GetAppliedShadowPropertyReference(propertyReference, (ITypeId) this.Type);
          this.ViewObject.ClearValue(propertyReference1);
          PropertyReference propertyReference2 = new PropertyReference(propertyReference.FirstStep);
          if (PlatformTypes.Transform.Equals((object) propertyReference.FirstStep.PropertyType))
          {
            Transform transform1 = this.ConvertToWpfValue(this.ViewObject.GetCurrentValue((IProperty) propertyReference1.FirstStep)) as Transform;
            if ((transform1 == null || transform1.Value.IsIdentity) && !this.ViewModel.AnimationEditor.HasAnimations(this, propertyReference2.Path))
            {
              this.ViewObject.ClearValue((IProperty) propertyReference1.FirstStep);
              Transform transform2 = this.ConvertToWpfValue(this.ViewObject.GetCurrentValue((IProperty) propertyReference1.FirstStep)) as Transform;
              if (transform2 == null || transform2.Value.IsIdentity)
                propertyReference = propertyReference2;
            }
          }
          else if (PlatformTypes.Projection.IsAssignableFrom((ITypeId) propertyReference.FirstStep.PropertyType))
          {
            IViewPlaneProjection viewPlaneProjection1 = this.Platform.ViewObjectFactory.Instantiate(this.ViewObject.GetCurrentValue((IProperty) propertyReference1.FirstStep)) as IViewPlaneProjection;
            if ((viewPlaneProjection1 == null || viewPlaneProjection1.IsIdentity()) && !this.ViewModel.AnimationEditor.HasAnimations(this, propertyReference2.Path))
            {
              this.ViewObject.ClearValue((IProperty) propertyReference1.FirstStep);
              IViewPlaneProjection viewPlaneProjection2 = this.Platform.ViewObjectFactory.Instantiate(this.ViewObject.GetCurrentValue((IProperty) propertyReference1.FirstStep)) as IViewPlaneProjection;
              if (viewPlaneProjection2 == null || viewPlaneProjection2.IsIdentity())
                propertyReference = propertyReference2;
            }
          }
        }
        SceneElement element = this as SceneElement;
        if (modification == SceneNode.Modification.SetValue && this.StoryboardContainer == animationEditor.ActiveStoryboardContainer && element != null)
        {
          animationEditor.ValidateAnimations(this, propertyReference, valueToSet);
          if (animationEditor.IsKeyFraming && !this.ViewModel.IsForcingBaseValue)
          {
            if (!(valueToSet is Microsoft.Expression.DesignModel.DocumentModel.DocumentNode) && element.TryAnimationSetValue(propertyReference, valueToSet))
              return SceneNode.Modification.NoOP;
            if (animationEditor.IsRecording || animationEditor.IsCurrentlyAnimated(this, propertyReference, 1))
            {
              PropertyReference animationProperty = animationEditor.GetAnimationProperty(this, propertyReference);
              if (animationProperty != null)
              {
                animationEditor.SetValue(element, animationProperty, valueToSet);
                return SceneNode.Modification.NoOP;
              }
            }
          }
          if (this.IsAttached && this.ViewModel.ActiveVisualTrigger != null && (!this.ViewModel.IsForcingBaseValue && propertyReference.LastStep != this.NameProperty))
            this.EnsureNamed();
        }
        else if (modification == SceneNode.Modification.InsertValue || modification == SceneNode.Modification.RemoveValue)
          animationEditor.ValidateAnimations(this, propertyReference, index, modification == SceneNode.Modification.InsertValue);
        else if (modification == SceneNode.Modification.ClearValue)
        {
          if (!this.ProjectContext.IsCapabilitySet(PlatformCapability.SupportsAutoAndNan) && !this.ViewModel.IsForcingBaseValue && propertyReference.ReferenceSteps.Count == 1 && (animationEditor.IsRecording || animationEditor.IsCurrentlyAnimated(this, propertyReference, 1)))
          {
            object defaultValue = this.GetDefaultValue((IPropertyId) propertyReference[0]);
            if (defaultValue is double && double.IsNaN((double) defaultValue) && !animationEditor.CanAnimateLayout)
            {
              animationEditor.PostAutoErrorDialog(AutoDialogType.KeyFrame);
              return SceneNode.Modification.NoOP;
            }
          }
          animationEditor.ValidateAnimations(this, propertyReference, DependencyProperty.UnsetValue);
        }
      }
      return modification;
    }

    private void InternalModifyValue(PropertyReference propertyReference, object valueToSet, SceneNode.Modification modification, int index, bool checkName)
    {
      this.EnsureNodeTree(propertyReference, modification != SceneNode.Modification.InsertValue && modification != SceneNode.Modification.RemoveValue, valueToSet is Microsoft.Expression.DesignModel.DocumentModel.DocumentNode);
      if (checkName && propertyReference[0].Equals((object) this.NameProperty) && this.IsAttached)
      {
        string str = valueToSet as string;
        if (str != null)
        {
          SceneNodeIDHelper sceneNodeIdHelper = new SceneNodeIDHelper(this.ViewModel, (SceneNode) this.StoryboardContainer);
          bool isNamed = this.IsNamed;
          string name = this.Name;
          if (!sceneNodeIdHelper.IsValidElementID(this, str))
            str = sceneNodeIdHelper.GetValidElementID(this, str);
          if (!(str != name))
            return;
          using (this.ViewModel.ForceBaseValue())
            this.InternalModifyValue(propertyReference, (object) str, modification, index, false);
          if (!isNamed)
            return;
          IStoryboardContainer storyboardContainer = this.StoryboardContainer;
          if (storyboardContainer != null && storyboardContainer.ResolveTargetName(name) == null)
            this.ViewModel.AnimationEditor.UpdateStoryboardOnElementRename(storyboardContainer, new Dictionary<string, string>()
            {
              {
                name,
                str
              }
            });
          if (SceneNode.allowRenameAsyncDialog)
          {
            new AsyncProcessDialog((AsyncProcess) new ReferenceRepairProcessor(this.DesignerContext, (ReferenceChangeModel) new NameChangeModel(name, str, this)), this.DesignerContext.ExpressionInformationService).ShowDialog();
            return;
          }
          new SynchronousReferenceRepairProcessor(this.DesignerContext, (ReferenceChangeModel) new NameChangeModel(name, str, this)).Begin();
          return;
        }
        if (valueToSet == null && this.Name != null && this.StoryboardContainer != null)
          this.ViewModel.AnimationEditor.DeleteAllAnimations(this);
      }
      int nextReferenceStepIndex;
      DocumentNodePath parentPath;
      DocumentPropertyNodeReferenceBase propertyReference1 = this.GetDeepestCompositeOrCollectionNodePropertyReference(propertyReference, true, false, out nextReferenceStepIndex, out parentPath);
      if (nextReferenceStepIndex == propertyReference.Count - 1)
      {
        if (modification == SceneNode.Modification.SetValue || modification == SceneNode.Modification.InsertValue)
        {
          Microsoft.Expression.DesignModel.DocumentModel.DocumentNode node1 = valueToSet == null ? (Microsoft.Expression.DesignModel.DocumentModel.DocumentNode) this.DocumentContext.CreateNode((ITypeId) this.DocumentContext.TypeResolver.ResolveProperty(propertyReference1.PropertyKey).PropertyType, (IDocumentNodeValue) null) : this.CreateNode(valueToSet);
          if (modification == SceneNode.Modification.SetValue)
          {
            this.ViewModel.UpdateViewModelStateForNodeRemoving(parentPath, propertyReference1.Node);
            propertyReference1.Node = node1;
          }
          else
          {
            if (modification != SceneNode.Modification.InsertValue)
              return;
            Microsoft.Expression.DesignModel.DocumentModel.DocumentNode node2 = propertyReference1.Node;
            ISceneNodeCollection<SceneNode> sceneNodeCollection = node2 == null ? this.viewModel.GetSceneNode((Microsoft.Expression.DesignModel.DocumentModel.DocumentNode) propertyReference1.Parent).GetCollectionForProperty(propertyReference1.PropertyKey) : this.viewModel.GetSceneNode(node2).GetChildren();
            SceneNode sceneNode = this.viewModel.GetSceneNode(node1);
            if (index == -1)
              sceneNodeCollection.Add(sceneNode);
            else
              sceneNodeCollection.Insert(index, sceneNode);
          }
        }
        else if (modification == SceneNode.Modification.ClearValue)
        {
          this.ViewModel.UpdateViewModelStateForNodeRemoving(parentPath, propertyReference1.Node);
          propertyReference1.Node = (Microsoft.Expression.DesignModel.DocumentModel.DocumentNode) null;
        }
        else
        {
          if (modification != SceneNode.Modification.RemoveValue)
            return;
          DocumentCompositeNode documentCompositeNode = (DocumentCompositeNode) propertyReference1.Node;
          if (!documentCompositeNode.SupportsChildren)
            return;
          this.ViewModel.UpdateViewModelStateForNodeRemoving(parentPath, documentCompositeNode.Children[index]);
          documentCompositeNode.Children.RemoveAt(index);
        }
      }
      else
      {
        Microsoft.Expression.DesignModel.DocumentModel.DocumentNode node1 = propertyReference1.Node;
        if (node1 == null)
          throw new InvalidOperationException(ExceptionStringTable.SceneNodeCannotResolvePropertyOrIndexReference);
        object target = this.ViewModel.CreateInstance(this.DocumentNodePath.GetPathInContainer(node1));
        Freezable freezable = target as Freezable;
        if (freezable != null && freezable.IsFrozen)
          target = (object) freezable.Clone();
        if (modification == SceneNode.Modification.InsertValue)
          target = propertyReference.PartialAdd(target, index, valueToSet, nextReferenceStepIndex + 1, propertyReference.Count - 1);
        else if (modification == SceneNode.Modification.SetValue)
          target = propertyReference.PartialSetValue(target, valueToSet, nextReferenceStepIndex + 1, propertyReference.Count - 1);
        else if (modification == SceneNode.Modification.ClearValue)
          propertyReference.PartialClearValue(target, nextReferenceStepIndex + 1, propertyReference.Count - 1);
        else if (modification == SceneNode.Modification.RemoveValue)
          target = propertyReference.PartialRemoveAt(target, nextReferenceStepIndex + 1, propertyReference.Count - 1, index);
        Microsoft.Expression.DesignModel.DocumentModel.DocumentNode node2 = this.CreateNode(target);
        propertyReference1.Node = node2;
      }
    }

    public SceneNode GetLocalValueAsSceneNode(PropertyReference propertyReference)
    {
      return this.GetLocalValueAsSceneNode(propertyReference, false);
    }

    public SceneNode GetLocalValueAsSceneNode(PropertyReference propertyReference, bool forceEvaluateExpressions)
    {
      return this.GetLocalValueAsSceneNode(propertyReference.GetPropertyKeys(), forceEvaluateExpressions);
    }

    public SceneNode GetLocalValueAsSceneNode(IPropertyId propertyKey)
    {
      return this.GetLocalValueAsSceneNode(propertyKey, false);
    }

    public SceneNode GetLocalValueAsSceneNode(IPropertyId propertyKey, bool forceEvaluateExpressions)
    {
      return this.GetLocalValueAsSceneNode(new IProperty[1]
      {
        this.ProjectContext.ResolveProperty(propertyKey)
      }, forceEvaluateExpressions);
    }

    protected SceneNode GetLocalValueAsSceneNode(IProperty[] propertyPath, bool forceEvaluateExpressions)
    {
      DocumentNodePath valueAsDocumentNode = this.GetLocalValueAsDocumentNode(propertyPath, forceEvaluateExpressions);
      if (valueAsDocumentNode != null && !this.ViewModel.IsExternal(valueAsDocumentNode.Node))
        return this.ViewModel.GetSceneNode(valueAsDocumentNode.Node);
      return (SceneNode) null;
    }

    public DocumentNodePath GetLocalValueAsDocumentNode(PropertyReference propertyReference)
    {
      return this.GetLocalValueAsDocumentNode(propertyReference, false);
    }

    public DocumentNodePath GetLocalValueAsDocumentNode(PropertyReference propertyReference, bool forceEvaluateExpressions)
    {
      return this.GetLocalValueAsDocumentNode(propertyReference.GetPropertyKeys(), forceEvaluateExpressions);
    }

    public DocumentNodePath GetLocalValueAsDocumentNode(IPropertyId propertyKey)
    {
      return this.GetLocalValueAsDocumentNode(propertyKey, false);
    }

    public DocumentNodePath GetLocalValueAsDocumentNode(IPropertyId propertyKey, bool forceEvaluateExpressions)
    {
      return this.GetLocalValueAsDocumentNode(new IProperty[1]
      {
        this.ProjectContext.ResolveProperty(propertyKey)
      }, forceEvaluateExpressions);
    }

    protected virtual DocumentNodePath GetLocalValueAsDocumentNode(IProperty[] propertyPath, bool forceEvaluateExpressions)
    {
      int nextResolvedStepIndex = 0;
      DocumentNodePath documentNodePath = this.ResolveValue(this.DocumentNodePath, propertyPath, propertyPath.Length, forceEvaluateExpressions || !this.ViewModel.IsForcingBaseValue, false, out nextResolvedStepIndex);
      if (nextResolvedStepIndex == propertyPath.Length)
        return documentNodePath;
      return (DocumentNodePath) null;
    }

    public void SetValueAsSceneNode(PropertyReference propertyReference, SceneNode value)
    {
      this.EnsureNodeTree(propertyReference, true, true);
      int nextReferenceStepIndex;
      DocumentNodePath parentPath;
      DocumentPropertyNodeReferenceBase propertyReference1 = this.GetDeepestCompositeOrCollectionNodePropertyReference(propertyReference, true, false, out nextReferenceStepIndex, out parentPath);
      if (nextReferenceStepIndex != propertyReference.Count - 1)
        throw new InvalidOperationException(ExceptionStringTable.SceneNodeNoPropertyValue);
      propertyReference1.Node = value.DocumentNode;
    }

    public void SetValueAsSceneNode(IPropertyId propertyKey, SceneNode value)
    {
      DocumentNodeReference documentNodeReference = (DocumentNodeReference) DocumentNodeResolver.CreateCompositeOrCollectionNodePropertyReference(this, (DocumentCompositeNode) this.DocumentNode, (IPropertyId) this.ProjectContext.ResolveProperty(propertyKey), this.GetActiveLocalPropertyTrigger(), new DocumentNodeResolver.ShouldUseTrigger(this.ShouldUseTrigger), false);
      if (documentNodeReference.Node != null)
        this.ViewModel.UpdateViewModelStateForNodeRemoving(this.DocumentNodePath, documentNodeReference.Node);
      documentNodeReference.Node = value.DocumentNode;
    }

    public virtual DocumentPropertyNodeReferenceBase CreateLocalDocumentPropertyNodeReference(DocumentCompositeNode parent, IPropertyId propertyKey)
    {
      return (DocumentPropertyNodeReferenceBase) new DocumentPropertyNodeReference(parent, propertyKey);
    }

    private static IEnumerable<Microsoft.Expression.DesignModel.DocumentModel.DocumentNode> GetDocumentNodes<T>(IEnumerable<T> collection) where T : SceneNode
    {
      foreach (T obj in collection)
        yield return obj.DocumentNode;
    }

    public static DocumentNodeMarkerSortedList GetMarkerList<T>(IEnumerable<T> collection, bool sorted) where T : SceneNode
    {
      DocumentNodeMarkerSortedList markerSortedList = new DocumentNodeMarkerSortedList();
      markerSortedList.CopyFrom(SceneNode.GetDocumentNodes<T>(collection), sorted, true);
      return markerSortedList;
    }

    public static List<T> FromMarkerList<T>(DocumentNodeMarkerSortedListBase markers, SceneViewModel viewModel) where T : SceneNode
    {
      List<T> list = new List<T>(markers.Count);
      foreach (DocumentNodeMarker documentNodeMarker in markers.Markers)
      {
        Microsoft.Expression.DesignModel.DocumentModel.DocumentNode node = documentNodeMarker.Node;
        if (node != null)
        {
          T obj = viewModel.GetSceneNode(node) as T;
          if ((object) obj == null)
            throw new ArgumentException(ExceptionStringTable.NodeForMarkerIsNotOfSpecificType);
          list.Add(obj);
        }
      }
      return list;
    }

    public static T FromMarker<T>(DocumentNodeMarker marker, SceneViewModel viewModel) where T : SceneNode
    {
      if (marker == null)
        return default (T);
      Microsoft.Expression.DesignModel.DocumentModel.DocumentNode node = marker.Node;
      if (node == null)
        return default (T);
      T obj = viewModel.GetSceneNode(node) as T;
      if ((object) obj == null)
        throw new ArgumentException(ExceptionStringTable.NodeForMarkerIsNotOfSpecificType);
      return obj;
    }

    public static int MarkerCompare(SceneNode x, SceneNode y)
    {
      return x.DocumentNode.Marker.CompareTo((object) y.DocumentNode.Marker);
    }

    public static IDisposable DisableEnsureTransform(bool disable)
    {
      return (IDisposable) new SceneNode.DisableEnsureTransformToken(disable);
    }

    public static IDisposable DisableAsyncRenameDialog()
    {
      return (IDisposable) new SceneNode.DisableAsyncRenameDialogToken();
    }

    public static bool CopyPropertyValue(SceneNode fromNode, IPropertyId fromProperty, SceneNode toNode, IPropertyId toProperty)
    {
      if (fromNode == null || fromProperty == null || (toNode == null || toProperty == null))
        return false;
      using (fromNode.ViewModel.ForceBaseValue())
      {
        DocumentNodePath valueAsDocumentNode1 = fromNode.GetLocalValueAsDocumentNode(fromProperty);
        Microsoft.Expression.DesignModel.DocumentModel.DocumentNode documentNode1 = valueAsDocumentNode1 != null ? valueAsDocumentNode1.Node : (Microsoft.Expression.DesignModel.DocumentModel.DocumentNode) null;
        DocumentNodePath valueAsDocumentNode2 = toNode.GetLocalValueAsDocumentNode(toProperty);
        Microsoft.Expression.DesignModel.DocumentModel.DocumentNode documentNode2 = valueAsDocumentNode2 != null ? valueAsDocumentNode2.Node : (Microsoft.Expression.DesignModel.DocumentModel.DocumentNode) null;
        if (!object.Equals((object) documentNode1, (object) documentNode2))
        {
          toNode.SetLocalValue(toProperty, documentNode1 != null ? documentNode1.Clone(toNode.DocumentContext) : (Microsoft.Expression.DesignModel.DocumentModel.DocumentNode) null);
          return true;
        }
      }
      return false;
    }

    protected void EnsureTransformIfNeeded(PropertyReference propertyReference)
    {
      if (!SceneNode.allowEnsureTransform || propertyReference.Count <= 1)
        return;
      for (int endIndex = 0; endIndex < propertyReference.Count - 1; ++endIndex)
      {
        ITypeId type = (ITypeId) propertyReference[endIndex].PropertyType;
        if (PlatformTypes.Transform.IsAssignableFrom(type) || PlatformTypes.Transform3D.IsAssignableFrom(type))
        {
          this.EnsureTransform(propertyReference.Subreference(0, endIndex));
          break;
        }
        if (PlatformTypes.Projection.IsAssignableFrom(type))
        {
          this.EnsureProjection(propertyReference.Subreference(0, endIndex));
          break;
        }
      }
    }

    protected void EnsureProjection(PropertyReference projectionProperty)
    {
      if (this.ViewObject == null)
        return;
      using (this.ViewModel.ForceBaseValue())
      {
        if (!PlatformTypes.Projection.IsAssignableFrom((ITypeId) projectionProperty.LastStep.PropertyType) || this.IsSet(projectionProperty) == PropertyState.Set)
          return;
        SceneNode sceneNode = this.viewModel.CreateSceneNode(PlatformTypes.PlaneProjection);
        this.SetValueAsSceneNode(projectionProperty, sceneNode);
        if (this is StyleNode)
          return;
        object instance = sceneNode.CreateInstance();
        if (!this.IsViewObjectValid)
          return;
        this.ViewObject.SetValue((ITypeResolver) this.ProjectContext, projectionProperty, instance);
      }
    }

    protected void EnsureTransform(PropertyReference transformProperty)
    {
      if (!SceneNode.allowEnsureTransform || this.ViewObject == null)
        return;
      using (this.ViewModel.ForceBaseValue())
      {
        if (PlatformTypes.Transform.IsAssignableFrom((ITypeId) transformProperty.LastStep.PropertyType))
        {
          Transform wpfTransform = Transform.Identity;
          bool useComputedValue = false;
          bool ignoreCenter = true;
          if (this.TryGetCanonicalTransform(transformProperty, out wpfTransform, useComputedValue, ignoreCenter))
            return;
          if (!this.ProjectContext.IsCapabilitySet(PlatformCapability.SupportsCompositeTransform))
            this.ViewModel.AnimationEditor.DeleteAllAnimations(this, transformProperty.Path);
          else
            this.ViewModel.AnimationEditor.UpdateTransformReferences(this, transformProperty.Path);
          CanonicalTransform canonicalTransform = new CanonicalTransform(this.GetComputedValueAsWpf(transformProperty) as Transform);
          if (this.IsSet(transformProperty) == PropertyState.Unset)
          {
            if (transformProperty.LastStep.Equals((object) Base2DElement.RenderTransformProperty))
            {
              Base2DElement base2Delement = (Base2DElement) this;
              if (this.IsSet(Base2DElement.RenderTransformOriginProperty) == PropertyState.Unset)
              {
                Point point = (Point) this.GetComputedValueAsWpf(Base2DElement.RenderTransformOriginProperty);
                Rect computedTightBounds = base2Delement.GetComputedTightBounds();
                canonicalTransform.UpdateForNewOrigin(new Point(point.X * computedTightBounds.Width, point.Y * computedTightBounds.Height), new Point(0.5 * computedTightBounds.Width, 0.5 * computedTightBounds.Height));
                base2Delement.SetValueAsWpf(Base2DElement.RenderTransformOriginProperty, (object) new Point(0.5, 0.5));
              }
            }
            else if (transformProperty.LastStep.Equals((object) BrushNode.RelativeTransformProperty))
              canonicalTransform.UpdateCenter(new Point(0.5, 0.5));
          }
          object platformTransform = canonicalTransform.GetPlatformTransform(this.Platform.GeometryHelper);
          if (!this.UpdateCompositeTransform(transformProperty))
            this.SetValue(transformProperty, platformTransform);
          this.EnsureViewTransform(transformProperty, platformTransform);
        }
        else
        {
          if (!typeof (Transform3D).IsAssignableFrom(PlatformTypeHelper.GetPropertyType((IProperty) transformProperty.LastStep)) || CanonicalTransform3D.IsCanonical(this.GetLocalValueAsWpf(transformProperty) as Transform3D))
            return;
          Transform3D transform3D = (Transform3D) new CanonicalTransform3D(this.GetComputedValueAsWpf(transformProperty) as Transform3D).ToTransform();
          this.SetValueAsWpf(transformProperty, (object) transform3D);
          this.EnsureViewTransform(transformProperty, (object) transform3D);
        }
      }
    }

    protected virtual void EnsureViewTransform(PropertyReference transformProperty, object value)
    {
      if (!this.IsViewObjectValid)
        return;
      this.ViewObject.SetValue((ITypeResolver) this.ProjectContext, transformProperty, value);
    }

    protected bool TryGetCanonicalTransform(PropertyReference transformProperty, out Transform wpfTransform, bool useComputedValue = true, bool ignoreCenter = false)
    {
      wpfTransform = Transform.Identity;
      if (transformProperty == null || this.IsSet(transformProperty) != PropertyState.Set)
        return false;
      object obj = !useComputedValue ? this.GetLocalValue(transformProperty) : this.GetComputedValue(transformProperty);
      wpfTransform = this.ConvertToWpfValue(obj) as Transform;
      if (wpfTransform == null)
      {
        wpfTransform = Transform.Identity;
        return false;
      }
      bool flag = CanonicalTransform.IsCanonical(wpfTransform, ignoreCenter);
      if (flag && transformProperty.PlatformMetadata.IsCapabilitySet(PlatformCapability.SupportsCompositeTransform) && !PlatformTypes.CompositeTransform.IsAssignableFrom((ITypeId) this.ProjectContext.GetType(obj.GetType())))
        flag = false;
      return flag;
    }

    private bool UpdateCompositeTransform(PropertyReference transformProperty)
    {
      if (!this.ProjectContext.IsCapabilitySet(PlatformCapability.SupportsCompositeTransform))
        return false;
      TransformNode transformNode = this.GetLocalValueAsSceneNode(transformProperty) as TransformNode;
      if (transformNode != null && !(transformNode is CompositeTransformNode))
      {
        CompositeTransformNode compositeTransform = (CompositeTransformNode) this.ViewModel.CreateSceneNode(PlatformTypes.CompositeTransform);
        if (transformNode.CopyToCompositeTransform(compositeTransform))
        {
          this.SetValue(transformProperty, (object) compositeTransform.DocumentNode);
          return true;
        }
      }
      return false;
    }

    private object ValidateValueOfType(object value, PropertyReference propertyReference)
    {
      if (value == null && propertyReference != null && propertyReference.ValueType.IsValueType)
        return Activator.CreateInstance(propertyReference.ValueType);
      return value;
    }

    private object ValidateValueOfType(object value, IPropertyId propertyKey)
    {
      if (value == null)
      {
        IProperty property = this.ProjectContext.ResolveProperty(propertyKey);
        if (property != null && property.TargetType.IsValueType)
          return Activator.CreateInstance(property.TargetType);
      }
      return value;
    }

    private object BuildLocalValue(Microsoft.Expression.DesignModel.DocumentModel.DocumentNode valueNode)
    {
      if (this.DesignerContext == null || this.Platform.InstanceBuilderFactory == null)
        return (object) null;
      return this.viewModel.CreateInstance(this.DocumentNodePath.GetPathInContainer(valueNode));
    }

    private bool IsValidPath(DocumentNodePath nodePath)
    {
      for (; nodePath != null; nodePath = nodePath.GetContainerOwnerPath())
      {
        Microsoft.Expression.DesignModel.DocumentModel.DocumentNode containerNode = nodePath.ContainerNode;
        Microsoft.Expression.DesignModel.DocumentModel.DocumentNode documentNode = nodePath.Node;
        while (documentNode != null && documentNode != containerNode)
          documentNode = (Microsoft.Expression.DesignModel.DocumentModel.DocumentNode) documentNode.Parent;
        if (documentNode != containerNode)
          return false;
      }
      return true;
    }

    private DocumentNodePath ResolveValue(DocumentNodePath parentPath, IProperty[] propertyKeys, int numSteps, bool evaluateExpressions, bool visualTriggerOnly, out int nextResolvedStepIndex)
    {
      BaseTriggerNode trigger = this.ViewModel.IsForcingBaseValue ? (BaseTriggerNode) null : this.GetActiveLocalPropertyTrigger();
      return DocumentNodeResolver.ResolveValue(parentPath, (IList<IProperty>) propertyKeys, this, numSteps, evaluateExpressions, visualTriggerOnly, trigger, new DocumentNodeResolver.ShouldUseTrigger(this.ShouldUseTrigger), out nextResolvedStepIndex);
    }

    protected virtual bool ShouldUseTrigger(BaseTriggerNode trigger, IPropertyId propertyKey)
    {
      return trigger.ShouldRecordPropertyChange(this, propertyKey);
    }

    private BaseTriggerNode GetActiveLocalPropertyTrigger()
    {
      if (this.viewModel == null)
        return (BaseTriggerNode) null;
      AnimationEditor animationEditor = this.viewModel.AnimationEditor;
      if (animationEditor != null)
      {
        BaseTriggerNode baseTriggerNode = animationEditor.ActiveVisualTrigger as BaseTriggerNode;
        if (baseTriggerNode != null)
        {
          IStoryboardContainer storyboardContainer = this.StoryboardContainer;
          if (storyboardContainer != null && animationEditor.ActiveStoryboardContainer != null && (storyboardContainer == animationEditor.ActiveStoryboardContainer || this == animationEditor.ActiveStoryboardContainer.TargetElement))
            return baseTriggerNode;
        }
      }
      return (BaseTriggerNode) null;
    }

    protected PropertyReference PropertyReferenceFromPropertyKey(IPropertyId propertyKey)
    {
      IPropertyId propertyId = (IPropertyId) this.ProjectContext.ResolveProperty(propertyKey);
      if (propertyId == null)
        return (PropertyReference) null;
      return new PropertyReference(propertyId as ReferenceStep);
    }

    public class ConcreteSceneNodeFactory
    {
      protected virtual SceneNode Instantiate()
      {
        return new SceneNode();
      }

      public SceneNode Instantiate(SceneViewModel viewModel, Microsoft.Expression.DesignModel.DocumentModel.DocumentNode node)
      {
        SceneNode sceneNode = this.Instantiate();
        sceneNode.Initialize(viewModel, node);
        return sceneNode;
      }

      public virtual SceneNode Instantiate(SceneViewModel viewModel, ITypeId targetType)
      {
        SceneNode sceneNode = this.Instantiate();
        sceneNode.Initialize(viewModel, targetType);
        return sceneNode;
      }
    }

    public class MarkerComparer<T> : IComparer<T> where T : SceneNode
    {
      public int Compare(T x, T y)
      {
        return SceneNode.MarkerCompare((SceneNode) x, (SceneNode) y);
      }
    }

    protected enum Modification
    {
      SetValue,
      ClearValue,
      InsertValue,
      RemoveValue,
      NoOP,
    }

    private class DisableEnsureTransformToken : IDisposable
    {
      private bool originalAllowEnsureTransform;

      public DisableEnsureTransformToken(bool disable)
      {
        this.originalAllowEnsureTransform = SceneNode.allowEnsureTransform;
        SceneNode.allowEnsureTransform = !disable;
      }

      public void Dispose()
      {
        SceneNode.allowEnsureTransform = this.originalAllowEnsureTransform;
        GC.SuppressFinalize((object) this);
      }
    }

    private class DisableAsyncRenameDialogToken : IDisposable
    {
      private static int disableAsyncRenameDialogTokenRefCount;

      public DisableAsyncRenameDialogToken()
      {
        SceneNode.allowRenameAsyncDialog = false;
        ++SceneNode.DisableAsyncRenameDialogToken.disableAsyncRenameDialogTokenRefCount;
      }

      public void Dispose()
      {
        if (--SceneNode.DisableAsyncRenameDialogToken.disableAsyncRenameDialogTokenRefCount != 0)
          return;
        SceneNode.allowRenameAsyncDialog = true;
      }
    }

    protected abstract class SceneNodeCollectionBase
    {
      private SceneNode parentSceneNode;
      private SceneViewModel viewModel;

      protected SceneNode ParentSceneNode
      {
        get
        {
          return this.parentSceneNode;
        }
      }

      protected SceneViewModel ViewModel
      {
        get
        {
          return this.viewModel;
        }
      }

      protected DocumentCompositeNode CompositeParent
      {
        get
        {
          return (DocumentCompositeNode) this.parentSceneNode.DocumentNode;
        }
      }

      protected SceneNodeCollectionBase(SceneNode parentSceneNode)
      {
        this.parentSceneNode = parentSceneNode;
        this.viewModel = parentSceneNode.ViewModel;
      }

      protected static void VerifyDetached(SceneNode item)
      {
        if (item == null)
          throw new ArgumentNullException("item");
        if (item.IsAttached)
          throw new InvalidOperationException(ExceptionStringTable.SceneNodeCollectionCannotPerformOperationOnElementInTree);
      }
    }

    protected sealed class EmptySceneNodeCollection<T> : ISceneNodeCollection<T>, IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable where T : SceneNode
    {
      public bool IsReadOnly
      {
        get
        {
          return true;
        }
      }

      public int? FixedCapacity
      {
        get
        {
          return new int?();
        }
      }

      public T this[int index]
      {
        get
        {
          throw new ArgumentOutOfRangeException("index");
        }
        set
        {
          throw new ArgumentOutOfRangeException("index");
        }
      }

      public int Count
      {
        get
        {
          return 0;
        }
      }

      public bool Contains(T item)
      {
        return false;
      }

      public bool Remove(T item)
      {
        return false;
      }

      public void RemoveAt(int index)
      {
        throw new ArgumentOutOfRangeException("index");
      }

      public void Clear()
      {
      }

      public void Add(T item)
      {
        throw new InvalidOperationException();
      }

      public void Insert(int index, T item)
      {
        throw new InvalidOperationException();
      }

      public int IndexOf(T item)
      {
        return -1;
      }

      public void CopyTo(T[] array, int arrayIndex)
      {
      }

      public IEnumerator<T> GetEnumerator()
      {
        return (IEnumerator<T>) new SceneNode.ListEnumerator<T>((IList<T>) this);
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return (IEnumerator) this.GetEnumerator();
      }
    }

    protected abstract class SceneNodePropertyCollectionBase : SceneNode.SceneNodeCollectionBase
    {
      private IProperty collectionProperty;

      protected IProperty CollectionProperty
      {
        get
        {
          return this.collectionProperty;
        }
      }

      protected SceneNodePropertyCollectionBase(SceneNode parentSceneNode, IPropertyId collectionProperty)
        : base(parentSceneNode)
      {
        this.collectionProperty = parentSceneNode.ProjectContext.ResolveProperty(collectionProperty);
      }
    }

    protected class SceneNodeCollection<T> : SceneNode.SceneNodePropertyCollectionBase, ISceneNodeCollection<T>, IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable where T : SceneNode
    {
      private DocumentCompositeNode collectionParent;

      public bool IsReadOnly
      {
        get
        {
          return false;
        }
      }

      public virtual int? FixedCapacity
      {
        get
        {
          return new int?();
        }
      }

      public T this[int index]
      {
        get
        {
          if (this.collectionParent != null)
            return this.ViewModel.GetSceneNode(this.collectionParent.Children[index]) as T;
          throw new InvalidOperationException();
        }
        set
        {
          SceneNode.SceneNodeCollectionBase.VerifyDetached((SceneNode) value);
          if (this.collectionParent == null)
            throw new InvalidOperationException();
          this.RemoveAt(index);
          this.Insert(index, value);
        }
      }

      public int Count
      {
        get
        {
          int num = 0;
          if (this.collectionParent != null)
            num = this.collectionParent.Children.Count;
          return num;
        }
      }

      public SceneNodeCollection(SceneNode parentSceneNode, IPropertyId collectionProperty)
        : base(parentSceneNode, collectionProperty)
      {
        this.collectionParent = (DocumentCompositeNode) this.CompositeParent.Properties[(IPropertyId) this.CollectionProperty];
        if (this.collectionParent == null || this.collectionParent.SupportsChildren)
          return;
        this.collectionParent = (DocumentCompositeNode) null;
      }

      public bool Contains(T item)
      {
        return this.IndexOf(item) != -1;
      }

      public bool Remove(T item)
      {
        if (!this.Contains(item))
          return false;
        this.RemoveInternal(item);
        return true;
      }

      public void RemoveAt(int index)
      {
        this.RemoveInternal(this[index]);
      }

      public void Clear()
      {
        if (this.collectionParent == null)
          return;
        this.ViewModel.UpdateViewModelStateForNodeRemoving(this.ParentSceneNode.DocumentNodePath, (Microsoft.Expression.DesignModel.DocumentModel.DocumentNode) this.collectionParent);
        this.CompositeParent.Properties[(IPropertyId) this.CollectionProperty] = (Microsoft.Expression.DesignModel.DocumentModel.DocumentNode) null;
      }

      public virtual void Add(T item)
      {
        SceneNode.SceneNodeCollectionBase.VerifyDetached((SceneNode) item);
        this.ParentSceneNode.OnChildAdding((SceneNode) item);
        if (this.collectionParent == null)
        {
          this.collectionParent = this.ParentSceneNode.DocumentContext.CreateNode(PlatformTypeHelper.GetPropertyType(this.CollectionProperty));
          this.CompositeParent.Properties[(IPropertyId) this.CollectionProperty] = (Microsoft.Expression.DesignModel.DocumentModel.DocumentNode) this.collectionParent;
        }
        this.collectionParent.Children.Add(item.DocumentNode);
        this.ParentSceneNode.OnChildAdded((SceneNode) item);
      }

      public virtual void Insert(int index, T item)
      {
        SceneNode.SceneNodeCollectionBase.VerifyDetached((SceneNode) item);
        this.ParentSceneNode.OnChildAdding((SceneNode) item);
        if (this.collectionParent == null)
        {
          this.collectionParent = this.ParentSceneNode.DocumentContext.CreateNode(PlatformTypeHelper.GetPropertyType(this.CollectionProperty));
          this.CompositeParent.Properties[(IPropertyId) this.CollectionProperty] = (Microsoft.Expression.DesignModel.DocumentModel.DocumentNode) this.collectionParent;
        }
        this.collectionParent.Children.Insert(index, item.DocumentNode);
        this.ParentSceneNode.OnChildAdded((SceneNode) item);
      }

      public int IndexOf(T item)
      {
        if ((object) item == null)
          throw new ArgumentNullException("item");
        int num = -1;
        if (this.collectionParent != null && this.ParentSceneNode.DocumentNodePath.GetContainerNodePath().Equals((object) item.DocumentNodePath.GetContainerNodePath()))
          num = this.collectionParent.Children.IndexOf(item.DocumentNode);
        return num;
      }

      public void CopyTo(T[] array, int arrayIndex)
      {
        for (int index = 0; index < this.Count; ++index)
          array[arrayIndex + index] = this[index];
      }

      public IEnumerator<T> GetEnumerator()
      {
        return (IEnumerator<T>) new SceneNode.ListEnumerator<T>((IList<T>) this);
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return (IEnumerator) this.GetEnumerator();
      }

      private void RemoveInternal(T item)
      {
        this.ParentSceneNode.OnChildRemoving((SceneNode) item);
        this.collectionParent.Children.Remove(item.DocumentNode);
        this.ParentSceneNode.OnChildRemoved((SceneNode) item);
      }
    }

    protected sealed class SceneNodeSingletonCollection<T> : SceneNode.SceneNodeCollectionBase, ISceneNodeCollection<T>, IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable where T : SceneNode
    {
      private DocumentPropertyNodeReferenceBase nodeReference;

      public bool IsReadOnly
      {
        get
        {
          return false;
        }
      }

      public int? FixedCapacity
      {
        get
        {
          return new int?(1);
        }
      }

      public T this[int index]
      {
        get
        {
          if (index == 0 && this.Count == 1)
            return (T) this.ViewModel.GetSceneNode(this.nodeReference.Node);
          throw new ArgumentOutOfRangeException();
        }
        set
        {
          if (index != 0)
            throw new ArgumentOutOfRangeException();
          this.Clear();
          this.AddInternal(value);
        }
      }

      public int Count
      {
        get
        {
          return this.nodeReference.Node == null ? 0 : 1;
        }
      }

      public SceneNodeSingletonCollection(SceneNode parentSceneNode, IProperty collectionProperty)
        : base(parentSceneNode)
      {
        this.nodeReference = (DocumentPropertyNodeReferenceBase) new DocumentPropertyNodeReference(this.CompositeParent, (IPropertyId) collectionProperty);
      }

      public SceneNodeSingletonCollection(SceneNode parentSceneNode, DocumentPropertyNodeReferenceBase reference)
        : base(parentSceneNode)
      {
        this.nodeReference = reference;
      }

      public bool Contains(T item)
      {
        if (this.Count > 0)
          return this[0].DocumentNode == item.DocumentNode;
        return false;
      }

      public bool Remove(T item)
      {
        if (!this.Contains(item))
          return false;
        this.RemoveInternal(item);
        return true;
      }

      public void RemoveAt(int index)
      {
        if (index != 0 || this.Count != 1)
          throw new InvalidOperationException();
        this.RemoveInternal(this[0]);
      }

      public void Clear()
      {
        if (this.Count != 1)
          return;
        SceneElement element = (object) this[0] as SceneElement;
        if (element != null)
          this.ViewModel.AnimationEditor.DeleteAllAnimationsInSubtree(element);
        this.RemoveInternal(this[0]);
      }

      public void Add(T item)
      {
        this.Clear();
        this[0] = item;
      }

      public void Insert(int index, T item)
      {
        if (index != 0)
          throw new ArgumentOutOfRangeException();
        this.Clear();
        this[0] = item;
      }

      public int IndexOf(T item)
      {
        return !this.Contains(item) ? -1 : 0;
      }

      public void CopyTo(T[] array, int arrayIndex)
      {
        for (int index = 0; index < this.Count; ++index)
          array[arrayIndex + index] = this[index];
      }

      public IEnumerator<T> GetEnumerator()
      {
        return (IEnumerator<T>) new SceneNode.ListEnumerator<T>((IList<T>) this);
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return (IEnumerator) this.GetEnumerator();
      }

      private void AddInternal(T item)
      {
        SceneNode.SceneNodeCollectionBase.VerifyDetached((SceneNode) item);
        this.ParentSceneNode.OnChildAdding((SceneNode) item);
        this.nodeReference.Node = item.DocumentNode;
        this.ParentSceneNode.OnChildAdded((SceneNode) item);
      }

      private void RemoveInternal(T item)
      {
        this.ParentSceneNode.OnChildRemoving((SceneNode) item);
        this.nodeReference.Node = (Microsoft.Expression.DesignModel.DocumentModel.DocumentNode) null;
        this.ParentSceneNode.OnChildRemoved((SceneNode) item);
      }
    }

    protected sealed class SceneNodeChildrenCollection<T> : SceneNode.SceneNodeCollectionBase, ISceneNodeCollection<T>, IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable where T : SceneNode
    {
      public bool IsReadOnly
      {
        get
        {
          return false;
        }
      }

      public int? FixedCapacity
      {
        get
        {
          return new int?();
        }
      }

      public T this[int index]
      {
        get
        {
          return (T) this.ViewModel.GetSceneNode(this.CompositeParent.Children[index]);
        }
        set
        {
          SceneNode.SceneNodeCollectionBase.VerifyDetached((SceneNode) value);
          this.RemoveAt(index);
          this.Insert(index, value);
        }
      }

      public int Count
      {
        get
        {
          return this.CompositeParent.Children.Count;
        }
      }

      public SceneNodeChildrenCollection(SceneNode parentSceneNode)
        : base(parentSceneNode)
      {
      }

      public bool Contains(T item)
      {
        return this.IndexOf(item) != -1;
      }

      public bool Remove(T item)
      {
        if (!this.Contains(item))
          return false;
        this.ParentSceneNode.OnChildRemoving((SceneNode) item);
        this.CompositeParent.Children.Remove(item.DocumentNode);
        this.ParentSceneNode.OnChildRemoved((SceneNode) item);
        return true;
      }

      public void RemoveAt(int index)
      {
        this.Remove(this[index]);
      }

      public void Clear()
      {
        this.CompositeParent.Children.Clear();
      }

      public void Add(T item)
      {
        SceneNode.SceneNodeCollectionBase.VerifyDetached((SceneNode) item);
        this.ParentSceneNode.OnChildAdding((SceneNode) item);
        this.CompositeParent.Children.Add(item.DocumentNode);
        this.ParentSceneNode.OnChildAdded((SceneNode) item);
      }

      public void Insert(int index, T item)
      {
        SceneNode.SceneNodeCollectionBase.VerifyDetached((SceneNode) item);
        this.ParentSceneNode.OnChildAdding((SceneNode) item);
        this.CompositeParent.Children.Insert(index, item.DocumentNode);
        this.ParentSceneNode.OnChildAdded((SceneNode) item);
      }

      public int IndexOf(T item)
      {
        return this.CompositeParent.Children.IndexOf(item.DocumentNode);
      }

      public void CopyTo(T[] array, int arrayIndex)
      {
        for (int index = 0; index < this.Count; ++index)
          array[arrayIndex + index] = this[index];
      }

      public IEnumerator<T> GetEnumerator()
      {
        return (IEnumerator<T>) new SceneNode.ListEnumerator<T>((IList<T>) this);
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return (IEnumerator) this.GetEnumerator();
      }
    }

    private sealed class ListEnumerator<T> : IEnumerator<T>, IDisposable, IEnumerator
    {
      private IList<T> collection;
      private int currentIndex;

      public T Current
      {
        get
        {
          return this.collection[this.currentIndex];
        }
      }

      object IEnumerator.Current
      {
        get
        {
          return (object) this.Current;
        }
      }

      public ListEnumerator(IList<T> collection)
      {
        this.currentIndex = -1;
        this.collection = collection;
      }

      public void Reset()
      {
        this.currentIndex = -1;
      }

      public bool MoveNext()
      {
        ++this.currentIndex;
        while (this.currentIndex < this.collection.Count && (object) this.collection[this.currentIndex] == null)
          ++this.currentIndex;
        return this.currentIndex < this.collection.Count;
      }

      public void Dispose()
      {
      }
    }

    protected class TextFlowSceneNodeCollection : ITextFlowSceneNodeCollection<SceneNode>, ISceneNodeCollection<SceneNode>, IList<SceneNode>, ICollection<SceneNode>, IEnumerable<SceneNode>, IEnumerable
    {
      private SceneViewModel viewModel;
      private BaseFrameworkElement textElement;
      private SceneElement parentElement;
      private SceneElement collectionParent;
      private IPropertyId collectionProperty;

      private DocumentCompositeNode CollectionParent
      {
        get
        {
          if (this.parentElement != null)
          {
            DocumentCompositeNode documentCompositeNode = this.parentElement.DocumentNode as DocumentCompositeNode;
            if (documentCompositeNode != null)
              return documentCompositeNode.Properties[this.collectionProperty] as DocumentCompositeNode;
          }
          return (DocumentCompositeNode) null;
        }
      }

      public SceneNode this[int index]
      {
        get
        {
          this.EnsureParentElement();
          return this.parentElement.ViewModel.GetSceneNode(this.CollectionParent.Children[index]);
        }
        set
        {
          this.RemoveAt(index);
          this.Insert(index, value);
        }
      }

      public int Count
      {
        get
        {
          if (this.CollectionParent == null || !this.CollectionParent.SupportsChildren)
            return 0;
          return this.CollectionParent.Children.Count;
        }
      }

      public bool IsReadOnly
      {
        get
        {
          return false;
        }
      }

      public int? FixedCapacity
      {
        get
        {
          return new int?();
        }
      }

      private IViewTextPointer ContentStart
      {
        get
        {
          this.EnsureParentElement();
          return ((ITextFlowSceneNode) this.parentElement).ContentStart;
        }
      }

      private IViewTextPointer ContentEnd
      {
        get
        {
          this.EnsureParentElement();
          return ((ITextFlowSceneNode) this.parentElement).ContentEnd;
        }
      }

      internal TextFlowSceneNodeCollection(BaseFrameworkElement textElement, SceneElement parentElement, SceneElement collectionParent, IPropertyId collectionProperty)
      {
        this.textElement = textElement;
        this.parentElement = parentElement;
        this.collectionProperty = collectionProperty;
        this.collectionParent = collectionParent;
        this.viewModel = this.collectionParent.ViewModel;
      }

      public void InsertAtTextIndex(int index, SceneElement element)
      {
        IViewTextPointer insertionLocation = (IViewTextPointer) null;
        if (this.ContentStart != null)
          insertionLocation = this.ContentStart.GetPositionAtOffset(index, LogicalDirection.Forward);
        if (insertionLocation == null && this.ContentEnd != null)
          insertionLocation = this.ContentEnd.GetInsertionPosition(LogicalDirection.Forward);
        this.InsertInternal((SceneNode) element, insertionLocation);
      }

      public int IndexOf(SceneNode item)
      {
        if (item == null)
          throw new ArgumentNullException("item");
        int num = -1;
        if (this.parentElement != null && this.parentElement.DocumentNodePath.GetContainerNodePath().Equals((object) item.DocumentNodePath.GetContainerNodePath()))
        {
          Microsoft.Expression.DesignModel.DocumentModel.DocumentNode documentNode = item.DocumentNode;
          DocumentCompositeNode collectionParent = this.CollectionParent;
          if (collectionParent != null && collectionParent.SupportsChildren)
            num = collectionParent.Children.IndexOf(documentNode);
        }
        return num;
      }

      public void Insert(int index, SceneNode sceneNode)
      {
        int num = 0;
        IViewTextPointer insertionLocation;
        for (insertionLocation = this.ContentStart; insertionLocation.CompareTo(this.ContentEnd) < 0 && index != num; insertionLocation = insertionLocation.GetPositionAtOffset(1))
        {
          IViewObject adjacentElement = insertionLocation.GetAdjacentElement(LogicalDirection.Forward);
          if (adjacentElement != null && adjacentElement is IViewVisual)
            ++num;
        }
        this.InsertInternal(sceneNode, insertionLocation);
      }

      public void RemoveAt(int index)
      {
        this.RemoveInternal(this[index]);
      }

      public void Add(SceneNode sceneNode)
      {
        this.viewModel.Document.OnUpdatedEditTransaction();
        this.InsertInternal(sceneNode, this.ContentEnd);
      }

      public void Clear()
      {
        while (this.Count > 0)
          this.RemoveAt(0);
      }

      public bool Contains(SceneNode sceneNode)
      {
        return this.IndexOf(sceneNode) != -1;
      }

      public void CopyTo(SceneNode[] array, int arrayIndex)
      {
        for (int index = 0; index < this.Count; ++index)
          array[arrayIndex + index] = this[index];
      }

      public bool Remove(SceneNode sceneNode)
      {
        this.RemoveInternal(sceneNode);
        return true;
      }

      public IEnumerator<SceneNode> GetEnumerator()
      {
        return (IEnumerator<SceneNode>) new SceneNode.ListEnumerator<SceneNode>((IList<SceneNode>) this);
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return (IEnumerator) this.GetEnumerator();
      }

      private void EnsureParentElement()
      {
        if (this.CollectionParent != null && !DocumentNodeUtilities.IsMarkupExtension((Microsoft.Expression.DesignModel.DocumentModel.DocumentNode) this.CollectionParent) && this.CollectionParent.SupportsChildren)
          return;
        this.parentElement = ((ITextFlowSceneNode) this.textElement).EnsureTextParent();
        this.viewModel.Document.OnUpdatedEditTransaction();
      }

      private string[] BreakRun(IViewRun run, IViewTextPointer splitPoint)
      {
        if (run.ContentStart.CompareTo(splitPoint) >= 0)
          return new string[2]
          {
            "",
            run.Text
          };
        if (run.ContentEnd.CompareTo(splitPoint) <= 0)
          return new string[2]
          {
            run.Text,
            ""
          };
        int offsetToPosition = splitPoint.GetOffsetToPosition(run.ContentEnd);
        return new string[2]
        {
          run.Text.Substring(0, run.Text.Length - offsetToPosition),
          run.Text.Substring(run.Text.Length - offsetToPosition)
        };
      }

      private void InsertInternal(SceneNode sceneNode, IViewTextPointer insertionLocation)
      {
        this.EnsureParentElement();
        if (!PlatformTypes.UIElement.IsAssignableFrom((ITypeId) sceneNode.Type))
        {
          new SceneNode.SceneNodeCollection<SceneNode>((SceneNode) this.collectionParent, this.collectionProperty)
          {
            sceneNode
          };
        }
        else
        {
          IDocumentContext documentContext = this.parentElement.DocumentContext;
          Microsoft.Expression.DesignModel.DocumentModel.DocumentNode node1 = (Microsoft.Expression.DesignModel.DocumentModel.DocumentNode) null;
          IViewRun run = (IViewRun) null;
          if (insertionLocation != null)
          {
            insertionLocation = insertionLocation.GetInsertionPosition(LogicalDirection.Forward);
            node1 = this.viewModel.DefaultView.GetCorrespondingDocumentNode(insertionLocation.Parent, false);
            run = insertionLocation.Parent as IViewRun;
          }
          DocumentCompositeNode node2 = documentContext.CreateNode(PlatformTypes.InlineUIContainer);
          InlineUIContainerElement containerElement = (InlineUIContainerElement) this.viewModel.GetSceneNode((Microsoft.Expression.DesignModel.DocumentModel.DocumentNode) node2);
          if (run != null && node1 != null)
          {
            string[] strArray = this.BreakRun(run, insertionLocation);
            DocumentCompositeNode documentCompositeNode1 = (DocumentCompositeNode) documentContext.CreateNode(run.PlatformSpecificObject.GetType(), run.PlatformSpecificObject);
            string str1 = strArray[0];
            Microsoft.Expression.DesignModel.DocumentModel.DocumentNode documentNode1 = (Microsoft.Expression.DesignModel.DocumentModel.DocumentNode) documentContext.CreateNode(str1);
            documentCompositeNode1.Properties[RunElement.TextProperty] = documentNode1;
            DocumentCompositeNode documentCompositeNode2 = (DocumentCompositeNode) documentContext.CreateNode(run.PlatformSpecificObject.GetType(), run.PlatformSpecificObject);
            string str2 = strArray[1];
            Microsoft.Expression.DesignModel.DocumentModel.DocumentNode documentNode2 = (Microsoft.Expression.DesignModel.DocumentModel.DocumentNode) documentContext.CreateNode(str2);
            documentCompositeNode2.Properties[RunElement.TextProperty] = documentNode2;
            IList<Microsoft.Expression.DesignModel.DocumentModel.DocumentNode> children = node1.Parent.Children;
            int index = children.IndexOf(node1);
            children.RemoveAt(index);
            if (str2.Length > 0)
              children.Insert(index, (Microsoft.Expression.DesignModel.DocumentModel.DocumentNode) documentCompositeNode2);
            children.Insert(index, (Microsoft.Expression.DesignModel.DocumentModel.DocumentNode) node2);
            if (str1.Length > 0)
              children.Insert(index, (Microsoft.Expression.DesignModel.DocumentModel.DocumentNode) documentCompositeNode1);
          }
          else
          {
            ITextFlowSceneNode textFlowSceneNode = (ITextFlowSceneNode) null;
            if (node1 != null)
              textFlowSceneNode = this.viewModel.GetSceneNode(node1) as ITextFlowSceneNode;
            if (textFlowSceneNode != null)
            {
              bool flag = false;
              IProperty property = this.viewModel.ProjectContext.ResolveProperty(textFlowSceneNode.TextChildProperty);
              if (property != null && PlatformTypes.InlineCollection.IsAssignableFrom((ITypeId) property.PropertyType))
              {
                ISceneNodeCollection<SceneNode> textChildProperty = textFlowSceneNode.CollectionForTextChildProperty;
                if (textChildProperty != null)
                {
                  IViewObject adjacentElement = insertionLocation.GetAdjacentElement(LogicalDirection.Forward);
                  if (adjacentElement != null)
                  {
                    for (int index = 0; index < textChildProperty.Count; ++index)
                    {
                      if (textChildProperty[index].ViewObject != null && textChildProperty[index].ViewObject.PlatformSpecificObject == adjacentElement.PlatformSpecificObject)
                      {
                        textFlowSceneNode.InsertInlineTextChild(index, (Microsoft.Expression.DesignModel.DocumentModel.DocumentNode) node2);
                        flag = true;
                        break;
                      }
                    }
                  }
                }
              }
              if (!flag)
                textFlowSceneNode.AddInlineTextChild((Microsoft.Expression.DesignModel.DocumentModel.DocumentNode) node2);
            }
            else
              ((ITextFlowSceneNode) this.textElement).AddInlineTextChild((Microsoft.Expression.DesignModel.DocumentModel.DocumentNode) node2);
          }
          containerElement.DefaultContent.Add(sceneNode);
          this.viewModel.Document.OnUpdatedEditTransaction();
        }
      }

      private void RemoveInternal(SceneNode sceneNode)
      {
        Microsoft.Expression.DesignModel.DocumentModel.DocumentNode documentNode = sceneNode.DocumentNode;
        SceneNode parent1 = sceneNode.Parent;
        DocumentCompositeNode parent2 = documentNode.Parent;
        parent1.OnChildRemoving(sceneNode);
        if (documentNode.IsProperty)
          parent2.Properties[(IPropertyId) documentNode.SitePropertyKey] = (Microsoft.Expression.DesignModel.DocumentModel.DocumentNode) null;
        else
          parent2.Children.Remove(sceneNode.DocumentNode);
        parent1.OnChildRemoved(sceneNode);
        this.viewModel.Document.OnUpdatedEditTransaction();
      }
    }
  }
}
