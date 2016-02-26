// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.FrameworkTemplateElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.UserInterface.ResourcePane;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class FrameworkTemplateElement : SceneElement, IStoryboardContainer, ITriggerContainer, IResourceContainer
  {
    public static readonly IPropertyId VisualTreeProperty = DesignTimeProperties.VisualTreeProperty;
    public static readonly IPropertyId ResourcesProperty = (IPropertyId) PlatformTypes.FrameworkTemplate.GetMember(MemberType.LocalProperty, "Resources", MemberAccessTypes.Public);
    public static readonly FrameworkTemplateElement.ConcreteFrameworkTemplateElementFactory Factory = new FrameworkTemplateElement.ConcreteFrameworkTemplateElementFactory();

    public override string DisplayName
    {
      get
      {
        return this.GetPropertyName(this.DocumentNodePath);
      }
    }

    public string Key
    {
      get
      {
        string str = (string) null;
        DictionaryEntryNode dictionaryEntryNode = this.FindNameBaseNode as DictionaryEntryNode;
        if (dictionaryEntryNode != null && dictionaryEntryNode.Key != null)
          str = dictionaryEntryNode.Key.ToString();
        return str;
      }
    }

    public override string ContainerDisplayName
    {
      get
      {
        string key = this.Key;
        string str = "Template";
        string name = this.TargetElementType.Name;
        if (key != null)
          return string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0} ({1} {2})", (object) key, (object) name, (object) str);
        return string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0} {1}", new object[2]
        {
          (object) name,
          (object) str
        });
      }
    }

    public override SceneNode FindNameBaseNode
    {
      get
      {
        SceneNode sceneNode = (SceneNode) this;
        DocumentNode documentNode = this.DocumentNode;
        if (documentNode != null)
        {
          DocumentCompositeNode parent = documentNode.Parent;
          if (parent != null)
          {
            DictionaryEntryNode dictionaryEntryNode = this.ViewModel.GetSceneNode((DocumentNode) parent) as DictionaryEntryNode;
            if (dictionaryEntryNode != null)
              return (SceneNode) dictionaryEntryNode;
            sceneNode = this.Parent;
            SceneElement sceneElement = sceneNode as SceneElement;
            if (sceneElement != null)
              return sceneElement.FindNameBaseNode;
          }
        }
        return (SceneNode) (sceneNode as BaseFrameworkElement);
      }
    }

    public override SceneNode Parent
    {
      get
      {
        SceneNode sceneNode = this.PathParent;
        if (sceneNode != null)
        {
          SetterSceneNode setterSceneNode = sceneNode as SetterSceneNode;
          if (setterSceneNode != null)
            sceneNode = setterSceneNode.Parent;
        }
        return sceneNode;
      }
    }

    public override IViewObject ViewTargetElement
    {
      get
      {
        foreach (IViewObject viewObject in (IEnumerable<IViewObject>) this.ViewModel.DefaultView.GetInstantiatedElements(this.DocumentNodePath))
        {
          if (PlatformTypes.FrameworkElement.IsAssignableFrom((ITypeId) viewObject.GetIType((ITypeResolver) this.ProjectContext)))
            return viewObject;
        }
        if (!this.ProjectContext.IsCapabilitySet(PlatformCapability.IsWpf) && this.VisualTreeRoot != null)
        {
          BaseFrameworkElement frameworkElement = (BaseFrameworkElement) null;
          for (SceneNode sceneNode = (SceneNode) this; frameworkElement == null && sceneNode != null && !(sceneNode is DictionaryEntryNode); sceneNode = sceneNode.Parent)
            frameworkElement = sceneNode as BaseFrameworkElement;
          IViewVisual viewVisual = frameworkElement != null ? frameworkElement.ViewTargetElement as IViewVisual : (IViewVisual) null;
          if (viewVisual != null)
          {
            foreach (IViewObject viewObject in (IEnumerable<IViewObject>) this.ViewModel.DefaultView.GetInstantiatedElements(this.VisualTreeRoot.DocumentNodePath))
            {
              IViewVisual visual = viewObject as IViewVisual;
              if (visual != null && viewVisual.IsAncestorOf(visual))
                return visual.VisualParent;
            }
          }
        }
        return base.ViewTargetElement;
      }
    }

    public BaseFrameworkElement TargetElement
    {
      get
      {
        BaseFrameworkElement frameworkElement = (BaseFrameworkElement) null;
        if (this.IsAttached)
        {
          SetterSceneNode setterSceneNode = this.Parent as SetterSceneNode;
          if (setterSceneNode != null)
          {
            SceneNode sceneNode = setterSceneNode.StoryboardContainer.ResolveTargetName(setterSceneNode.Target);
            if (sceneNode == setterSceneNode.StoryboardContainer)
              return ((IStoryboardContainer) sceneNode).TargetElement;
            return sceneNode as BaseFrameworkElement;
          }
          DocumentNodePath documentNodePath = this.DocumentNodePath;
          while (documentNodePath != null && !PlatformTypes.FrameworkElement.IsAssignableFrom((ITypeId) documentNodePath.Node.Type))
            documentNodePath = documentNodePath.GetContainerOwnerPath();
          if (documentNodePath != null)
            frameworkElement = (BaseFrameworkElement) this.ViewModel.GetSceneNode(documentNodePath.Node);
        }
        return frameworkElement;
      }
    }

    public BaseFrameworkElement VisualTreeRoot
    {
      get
      {
        ISceneNodeCollection<SceneNode> collectionForProperty = this.GetCollectionForProperty(FrameworkTemplateElement.VisualTreeProperty);
        if (collectionForProperty.Count > 0)
          return collectionForProperty[0] as BaseFrameworkElement;
        return (BaseFrameworkElement) null;
      }
      set
      {
        this.GetCollectionForProperty(FrameworkTemplateElement.VisualTreeProperty).Add((SceneNode) value);
      }
    }

    protected virtual IProperty TriggersProperty
    {
      get
      {
        return (IProperty) null;
      }
    }

    BaseFrameworkElement IStoryboardContainer.ScopeElement
    {
      get
      {
        return this.VisualTreeRoot;
      }
    }

    public IList<TriggerBaseNode> VisualTriggers
    {
      get
      {
        if (this.TriggersProperty == null)
          return (IList<TriggerBaseNode>) new SceneNode.EmptySceneNodeCollection<TriggerBaseNode>();
        return (IList<TriggerBaseNode>) new SceneNode.SceneNodeCollection<TriggerBaseNode>((SceneNode) this, (IPropertyId) this.TriggersProperty);
      }
    }

    public ResourceDictionaryNode Resources
    {
      get
      {
        if (this.ProjectContext.IsCapabilitySet(PlatformCapability.SupportsNonRootTemplateEditing))
          return ResourceManager.ProvideResourcesForElement((SceneNode) this);
        BaseFrameworkElement visualTreeRoot = this.VisualTreeRoot;
        if (visualTreeRoot != null)
          return visualTreeRoot.Resources;
        return (ResourceDictionaryNode) null;
      }
    }

    public bool AreResourcesSupported
    {
      get
      {
        return true;
      }
    }

    public bool CanEditTriggers
    {
      get
      {
        return this.TriggersProperty != null;
      }
    }

    public virtual bool CanEdit
    {
      get
      {
        return !this.IsLockedOrAncestorLocked && (this.ProjectContext.IsCapabilitySet(PlatformCapability.SupportsNonRootTemplateEditing) || this.VisualTreeRoot != null);
      }
    }

    public virtual Type TargetElementType
    {
      get
      {
        if (this.ViewTargetElement != null)
          return this.ViewTargetElement.TargetType;
        return typeof (FrameworkElement);
      }
    }

    public override string GetDisplayNameFromPath(DocumentNodePath documentNodePathOverride, bool includeTextContent)
    {
      return this.GetPropertyName(documentNodePathOverride);
    }

    private string GetPropertyName(DocumentNodePath documentNodePath)
    {
      SetterSceneNode setterSceneNode = (SetterSceneNode) null;
      DocumentNode documentNode = this.DocumentNode;
      if (documentNode.Parent == null || documentNode == documentNodePath.ContainerNode)
        setterSceneNode = this.ViewModel.GetSceneNode(documentNodePath.ContainerOwner) as SetterSceneNode;
      if (documentNode.IsChild)
        documentNode = (DocumentNode) documentNode.Parent;
      if (documentNode.Parent != null)
        setterSceneNode = this.ViewModel.GetSceneNode((DocumentNode) documentNode.Parent) as SetterSceneNode;
      if (setterSceneNode != null && setterSceneNode.Property != null)
        return setterSceneNode.Property.Name;
      return this.GetPropertyNameHelper(documentNodePath) ?? this.Type.Name;
    }

    public BaseFrameworkElement GetRelativeTargetElement(DocumentNodePath relativeEditingContainerPath)
    {
      if (relativeEditingContainerPath.ContainerNode != this.DocumentNode)
        return (BaseFrameworkElement) null;
      DocumentNodePath containerOwnerPath = relativeEditingContainerPath.GetContainerOwnerPath();
      if (containerOwnerPath == null)
        return (BaseFrameworkElement) null;
      SetterSceneNode setterSceneNode = this.ViewModel.GetSceneNode(containerOwnerPath.Node) as SetterSceneNode;
      if (setterSceneNode == null)
        return this.ViewModel.GetSceneNode(containerOwnerPath.Node) as BaseFrameworkElement;
      IStoryboardContainer storyboardContainer = (IStoryboardContainer) this.ViewModel.GetSceneNode(containerOwnerPath.ContainerNode);
      SceneNode sceneNode = storyboardContainer.ResolveTargetName(setterSceneNode.Target);
      if (sceneNode == storyboardContainer)
        return storyboardContainer.TargetElement;
      return sceneNode as BaseFrameworkElement;
    }

    SceneNode ITriggerContainer.ResolveTargetName(string name)
    {
      return !string.IsNullOrEmpty(name) ? SceneElementHelper.FindNode((SceneNode) this.VisualTreeRoot, name) : (SceneNode) this;
    }

    SceneNode ITriggerContainer.ResolveTargetName(string name, SceneNode relativeSource)
    {
      if (string.IsNullOrEmpty(name))
        return (SceneNode) (relativeSource as SceneElement ?? relativeSource.FindSceneNodeTypeAncestor<SceneElement>());
      return SceneElementHelper.FindNode((SceneNode) this, name);
    }

    string IStoryboardContainer.GetTargetName(SceneNode node)
    {
      if (this.TargetElement == node || node == this)
        return (string) null;
      if (node.StoryboardContainer != this)
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ExceptionStringTable.TimelineContainerDoesNotContainElement, new object[0]));
      return node.Name;
    }

    void IStoryboardContainer.AddResourceStoryboard(string name, StoryboardTimelineSceneNode storyboard)
    {
      this.AddResourceStoryboard(name, storyboard);
    }

    protected void AddResourceStoryboard(string name, StoryboardTimelineSceneNode storyboard)
    {
      (this.ProjectContext.IsCapabilitySet(PlatformCapability.SupportsNonRootTemplateEditing) ? Microsoft.Expression.DesignSurface.Utility.ResourceHelper.EnsureResourceDictionaryNode((SceneNode) this) : Microsoft.Expression.DesignSurface.Utility.ResourceHelper.EnsureResourceDictionaryNode((SceneNode) this.VisualTreeRoot)).Add(DictionaryEntryNode.Factory.Instantiate((object) name, (SceneNode) storyboard));
    }

    bool IStoryboardContainer.RemoveResourceStoryboard(StoryboardTimelineSceneNode storyboard)
    {
      return this.Resources.RemoveEntryWithValue((SceneNode) storyboard);
    }

    public void RenameTemplateIDs()
    {
      if (this.VisualTreeRoot == null)
        return;
      SceneNodeIDHelper sceneNodeIdHelper = new SceneNodeIDHelper(this.ViewModel, (SceneNode) this.VisualTreeRoot);
      Dictionary<string, string> renameTable = new Dictionary<string, string>();
      foreach (SceneElement sceneElement in SceneElementHelper.GetElementTree((SceneElement) this.VisualTreeRoot))
      {
        string name = sceneElement.Name;
        if (name == null || name.StartsWith("~ChildID"))
        {
          sceneNodeIdHelper.SetValidName((SceneNode) sceneElement, sceneElement.TargetType.Name);
          if (name != null && sceneElement.Name != name)
            renameTable.Add(name, sceneElement.Name);
        }
      }
      IStoryboardContainer storyboardContainer = (IStoryboardContainer) this;
      if (storyboardContainer == null)
        return;
      this.ViewModel.AnimationEditor.UpdateStoryboardOnElementRename(storyboardContainer, renameTable);
    }

    public class ConcreteFrameworkTemplateElementFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new FrameworkTemplateElement();
      }
    }
  }
}
