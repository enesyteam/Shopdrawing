// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.SceneElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.Framework.UserInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public abstract class SceneElement : SceneNode
  {
    private const int TextContentMaxLength = 15;
    private ISceneInsertionPoint defaultInsertionPoint;
    private bool defaultInsertionPointInitialized;

    public SceneElement ParentElement
    {
      get
      {
        return this.Parent as SceneElement;
      }
    }

    public SceneElement VisualElementAncestor
    {
      get
      {
        SceneElement parentElement = this.ParentElement;
        while (parentElement != null && (parentElement.ViewObject == null || !PlatformTypes.Visual.IsAssignableFrom((ITypeId) parentElement.ViewObject.GetIType((ITypeResolver) this.ProjectContext))))
          parentElement = parentElement.ParentElement;
        return parentElement;
      }
    }

    private IViewObject FirstInstanceInArtboard
    {
      get
      {
        SceneView defaultView = this.ViewModel.DefaultView;
        if (defaultView != null && defaultView.Artboard != null)
        {
          foreach (IViewObject viewObject in (IEnumerable<IViewObject>) defaultView.GetInstantiatedElements(this.DocumentNodePath))
          {
            IViewVisual visual = viewObject as IViewVisual;
            if (visual != null && defaultView.Artboard.IsInArtboard(visual))
              return viewObject;
          }
        }
        return (IViewObject) null;
      }
    }

    public virtual IViewObject ViewTargetElement
    {
      get
      {
        if (this.DocumentNodePath.ContainerNode != this.DocumentNodePath.Node && PlatformTypes.FrameworkTemplate.IsAssignableFrom((ITypeId) this.DocumentNodePath.ContainerNode.Type))
        {
          IViewObject instanceInArtboard = this.FirstInstanceInArtboard;
          if (instanceInArtboard != null)
            return instanceInArtboard;
        }
        IType type1 = this.ProjectContext.ResolveType(PlatformTypes.FrameworkElement);
        IType type2 = this.ProjectContext.ResolveType(ProjectNeutralTypes.DataGridColumn);
        if (this.ViewObject != null && (type1.RuntimeType.IsAssignableFrom(this.ViewObject.TargetType) || type2.RuntimeType != (Type) null && type2.RuntimeType.IsAssignableFrom(this.ViewObject.TargetType)))
          return this.ViewObject;
        if (this.ViewModel.ViewRoot != this && this.ViewModel.ViewRoot != this.Parent)
          return (IViewObject) null;
        return this.ViewModel.DefaultView.ViewRoot;
      }
    }

    public IViewObject Visual
    {
      get
      {
        if (this.DocumentNodePath.ContainerNode != this.DocumentNodePath.Node && PlatformTypes.FrameworkTemplate.IsAssignableFrom((ITypeId) this.DocumentNodePath.ContainerNode.Type))
        {
          IViewObject instanceInArtboard = this.FirstInstanceInArtboard;
          if (instanceInArtboard != null)
            return instanceInArtboard;
        }
        IViewObject viewObject = this.ViewObject;
        IType type = this.ProjectContext.ResolveType(PlatformTypes.Visual);
        if (viewObject != null && type.RuntimeType.IsAssignableFrom(viewObject.TargetType))
          return viewObject;
        return this.ViewTargetElement;
      }
    }

    public virtual bool IsContainer
    {
      get
      {
        if (this.Type.XamlSourcePath != null && this.ViewModel != null && this.ViewModel.RootNode != this)
          return false;
        List<IPropertyId> list = new List<IPropertyId>((IEnumerable<IPropertyId>) this.ContentProperties);
        list.Insert(0, (IPropertyId) this.DefaultContentProperty);
        foreach (IPropertyId propertyId in list)
        {
          IProperty targetProperty = propertyId as IProperty;
          if (targetProperty != null)
          {
            PropertySceneInsertionPoint sceneInsertionPoint = new PropertySceneInsertionPoint(this, targetProperty);
            if (!this.ProjectContext.IsCapabilitySet(PlatformCapability.SupportsAutoTabItemWrapping) && ProjectNeutralTypes.TabControl.IsAssignableFrom((ITypeId) this.TrueTargetTypeId))
              return sceneInsertionPoint.CanInsert(ProjectNeutralTypes.TabItem);
            if (sceneInsertionPoint.CanInsert((ITypeId) (targetProperty.PropertyType.ItemType ?? targetProperty.PropertyType)))
              return true;
          }
        }
        return false;
      }
    }

    public override bool IsSelectable
    {
      get
      {
        if (this.IsLockedOrAncestorLocked || this.ViewTargetElement != null && ElementUtilities.HasVisualTreeAncestorOfType(this.ViewTargetElement.PlatformSpecificObject as DependencyObject, typeof (Viewport2DVisual3D)))
          return false;
        return base.IsSelectable;
      }
    }

    public bool IsVisuallySelectable
    {
      get
      {
        return this.GetIsVisuallySelectable(false);
      }
    }

    public bool IsEffectiveRoot
    {
      get
      {
        if (this.Parent == null || this.Parent is WindowElement || this.Parent is PageElement)
          return true;
        if (this.Parent.Parent != null)
          return false;
        if (!PlatformTypes.UserControl.IsAssignableFrom((ITypeId) this.Parent.Type))
          return this.Parent is ContentControlElement;
        return true;
      }
    }

    public bool IsPlaceholder
    {
      get
      {
        return (bool) this.GetLocalOrDefaultValue(DesignTimeProperties.IsPlaceholderProperty);
      }
      set
      {
        if (value)
          this.SetLocalValue(DesignTimeProperties.IsPlaceholderProperty, (object) (bool) (value ? true : false));
        else
          this.ClearLocalValue(DesignTimeProperties.IsPlaceholderProperty);
      }
    }

    public bool HasTextContent
    {
      get
      {
        return this.TextContent.Length > 0;
      }
    }

    public override bool ShouldClearAnimation
    {
      get
      {
        return true;
      }
    }

    public override string DisplayName
    {
      get
      {
        if (this.IsNamed || !this.HasTextContent)
          return this.DisplayNameNoTextContent;
        return this.DisplayNameNoTextContent + " " + this.TextContent;
      }
    }

    public string DisplayNameNoTextContent
    {
      get
      {
        return base.DisplayName;
      }
    }

    public virtual string ContainerDisplayName
    {
      get
      {
        return this.DisplayName;
      }
    }

    public virtual SceneNode FindNameBaseNode
    {
      get
      {
        return (SceneNode) (this as BaseFrameworkElement);
      }
    }

    public string TextContent
    {
      get
      {
        object obj = this.IsViewObjectValid ? this.ViewObject.PlatformSpecificObject : (object) null;
        if (obj == null)
          return string.Empty;
        string str = string.Empty;
        IProperty defaultContentProperty = this.DefaultContentProperty;
        RichTextBox richTextBox;
        if ((richTextBox = obj as RichTextBox) != null)
        {
          str = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd).Text;
        }
        else
        {
          TextBlock textBlock;
          if ((textBlock = obj as TextBlock) != null)
          {
            str = string.IsNullOrEmpty(textBlock.Text) ? new TextRange(textBlock.ContentStart, textBlock.ContentEnd).Text : textBlock.Text;
          }
          else
          {
            FlowDocumentScrollViewer documentScrollViewer;
            if ((documentScrollViewer = obj as FlowDocumentScrollViewer) != null)
            {
              FlowDocument document = documentScrollViewer.Document;
              if (document != null)
                str = new TextRange(document.ContentStart, document.ContentEnd).Text;
            }
            else
            {
              HeaderedContentControl headeredContentControl;
              if ((headeredContentControl = obj as HeaderedContentControl) != null)
              {
                str = headeredContentControl.Header as string;
              }
              else
              {
                HeaderedItemsControl headeredItemsControl;
                if ((headeredItemsControl = obj as HeaderedItemsControl) != null)
                {
                  str = headeredItemsControl.Header as string;
                }
                else
                {
                  ButtonBase buttonBase;
                  if ((buttonBase = obj as ButtonBase) != null)
                  {
                    str = buttonBase.Content as string;
                  }
                  else
                  {
                    MenuItem menuItem;
                    if ((menuItem = obj as MenuItem) != null)
                    {
                      str = menuItem.Header as string;
                    }
                    else
                    {
                      Label label;
                      if ((label = obj as Label) != null)
                      {
                        str = label.Content as string;
                      }
                      else
                      {
                        ImageElement imageElement;
                        if ((imageElement = this as ImageElement) != null)
                          str = Path.GetFileNameWithoutExtension(imageElement.Uri);
                      }
                    }
                  }
                }
              }
            }
          }
        }
        if (str == null)
          str = string.Empty;
        int length = str.IndexOf(Environment.NewLine);
        if (length != -1)
          str = str.Substring(0, length);
        if (str.Length > 15)
          str = str.Substring(0, 15) + "...";
        if (str.Length <= 0)
          return string.Empty;
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"{0}\"", new object[1]
        {
          (object) str
        });
      }
    }

    public bool IsHidden
    {
      get
      {
        if (PlatformTypes.UIElement.IsAssignableFrom((ITypeId) this.Type))
          return (bool) this.GetLocalOrDefaultValue(DesignTimeProperties.IsHiddenProperty);
        return false;
      }
      set
      {
        if (value)
          this.SetLocalValue(DesignTimeProperties.IsHiddenProperty, (object) true);
        else
          this.ClearLocalValue(DesignTimeProperties.IsHiddenProperty);
      }
    }

    public bool IsInstantiatedElementVisible
    {
      get
      {
        if (this.ViewObject != null && !this.IsHiddenOrCollapsedOrAncestorHiddenOrCollapsed)
          return this.ViewModel.DefaultView.IsDesignSurfaceVisible;
        return false;
      }
    }

    public bool IsHiddenOrAncestorHidden
    {
      get
      {
        if (this.IsHidden)
          return true;
        if (this != this.ViewModel.ActiveEditingContainer && this.ParentElement != null)
          return this.ParentElement.IsHiddenOrAncestorHidden;
        return false;
      }
    }

    public bool IsHiddenOrCollapsed
    {
      get
      {
        IViewVisual viewVisual = this.ViewObject as IViewVisual;
        if (viewVisual == null)
          return false;
        return !viewVisual.IsVisible;
      }
    }

    public bool IsHiddenOrCollapsedOrAncestorHiddenOrCollapsed
    {
      get
      {
        if (this.IsHiddenOrCollapsed)
          return true;
        if (this != this.ViewModel.ActiveEditingContainer && this.ParentElement != null)
          return this.ParentElement.IsHiddenOrCollapsedOrAncestorHiddenOrCollapsed;
        return false;
      }
    }

    public bool IsLocked
    {
      get
      {
        return (bool) this.GetLocalOrDefaultValue(DesignTimeProperties.IsLockedProperty);
      }
      set
      {
        bool flag = (bool) this.GetDefaultValue(DesignTimeProperties.IsLockedProperty);
        if (value == flag)
          this.ClearLocalValue(DesignTimeProperties.IsLockedProperty);
        else
          this.SetLocalValue(DesignTimeProperties.IsLockedProperty, (object) (bool) (value ? true : false));
        if (!value)
          return;
        this.DeselectSubtree();
        ISceneInsertionPoint lockedInsertionPoint = this.ViewModel.LockedInsertionPoint;
        if (lockedInsertionPoint == null || lockedInsertionPoint.SceneElement.IsSelectable)
          return;
        this.ViewModel.SetLockedInsertionPoint((SceneElement) null);
      }
    }

    public bool CanLock
    {
      get
      {
        return this != this.ViewModel.RootNode && !(this is StyleNode) && !(this is FrameworkTemplateElement);
      }
    }

    public bool CanHide
    {
      get
      {
        return this != this.ViewModel.RootNode && !(this is StyleNode) && (!(this is FrameworkTemplateElement) && PlatformTypes.UIElement.IsAssignableFrom((ITypeId) this.Type));
      }
    }

    public bool IsLockedOrAncestorLocked
    {
      get
      {
        if (this.IsLocked)
          return true;
        if (this != this.ViewModel.ActiveEditingContainer && this.ParentElement != null)
          return this.ParentElement.IsLockedOrAncestorLocked;
        return false;
      }
    }

    public virtual ISceneInsertionPoint DefaultInsertionPoint
    {
      get
      {
        if (!this.defaultInsertionPointInitialized)
        {
          if (this.IsContainer)
          {
            IProperty defaultContentProperty = this.DefaultContentProperty;
            if (defaultContentProperty != null)
              this.defaultInsertionPoint = (ISceneInsertionPoint) new PropertySceneInsertionPoint(this, defaultContentProperty);
          }
          this.defaultInsertionPointInitialized = true;
        }
        return this.defaultInsertionPoint;
      }
    }

    public Matrix TransformToRoot
    {
      get
      {
        return this.ViewModel.DefaultView.GetComputedTransformToRoot(this);
      }
    }

    public Matrix TransformFromRoot
    {
      get
      {
        return this.ViewModel.DefaultView.GetComputedTransformFromRoot(this);
      }
    }

    public Matrix TransformToArtboard
    {
      get
      {
        SceneView defaultView = this.ViewModel.DefaultView;
        return defaultView.GetComputedTransformToRoot(this) * defaultView.Artboard.CalculateTransformFromContentToArtboard().Value;
      }
    }

    public Matrix TransformFromArtboard
    {
      get
      {
        Matrix transformToArtboard = this.TransformToArtboard;
        transformToArtboard.Invert();
        return transformToArtboard;
      }
    }

    public Size RenderSize
    {
      get
      {
        return this.ViewModel.DefaultView.GetRenderSize(this.Visual);
      }
    }

    private IPlatform DesignerDefaultPlatform
    {
      get
      {
        return this.DesignerContext.DesignerDefaultPlatformService.DefaultPlatform;
      }
    }

    public SceneElement EffectiveParent
    {
      get
      {
        if (this.Parent == null)
          return (SceneElement) null;
        SceneNode parent = this.Parent;
        SceneElement sceneElement = (SceneElement) null;
        for (; parent != null; parent = parent.Parent)
        {
          sceneElement = parent as SceneElement;
          if (sceneElement != null && sceneElement.IsContainer)
            break;
        }
        return sceneElement;
      }
    }

    public virtual bool GetIsVisuallySelectable(bool includeEffectiveRoot)
    {
      if (this.IsSelectable && (includeEffectiveRoot || !this.IsEffectiveRoot))
        return !this.IsHiddenOrCollapsedOrAncestorHiddenOrCollapsed;
      return false;
    }

    public virtual string GetDisplayNameFromPath(DocumentNodePath documentNodePathOverride, bool includeTextContent)
    {
      if (includeTextContent)
        return this.DisplayName;
      return this.DisplayNameNoTextContent;
    }

    public bool IsTextContentChange(PropertyReference propertyReference)
    {
      ReferenceStep referenceStep = propertyReference[0];
      return propertyReference.Count == 1 && referenceStep == this.DefaultContentProperty || (referenceStep.Equals((object) HeaderedControlProperties.HeaderedContentHeaderProperty) || referenceStep.Equals((object) HeaderedControlProperties.HeaderedItemsHeaderProperty)) || (referenceStep.Equals((object) DataGridColumnNode.HeaderProperty) || referenceStep.Equals((object) TextBlockElement.TextProperty) || (referenceStep.Equals((object) TextBlockElement.InlinesProperty) || referenceStep.Equals((object) RichTextBoxElement.DocumentProperty))) || referenceStep.Equals((object) FlowDocumentScrollViewerElement.DocumentProperty);
    }

    public ISceneInsertionPoint InsertionPointForProperty(IProperty contentProperty)
    {
      if (contentProperty == null)
        return (ISceneInsertionPoint) null;
      if (contentProperty == this.DefaultContentProperty)
        return this.DefaultInsertionPoint;
      return (ISceneInsertionPoint) new PropertySceneInsertionPoint(this, contentProperty);
    }

    public override string ToString()
    {
      string str = (string) null;
      if (this.NameProperty != null)
        str = DocumentPrimitiveNode.GetValueAsString(((DocumentCompositeNode) this.DocumentNode).Properties[this.NameProperty]);
      if (str == null)
        str = "<no Name>";
      return this.Type.Name + ": " + str;
    }

    public virtual void AddCustomContextMenuCommands(ICommandBar contextMenu)
    {
    }

    public virtual SceneNode ClonePropertyValueAsSceneNode(PropertyReference propertyReference)
    {
      SceneNode valueAsSceneNode = this.GetLocalValueAsSceneNode(propertyReference);
      SceneNode sceneNode = (SceneNode) null;
      if (valueAsSceneNode == null)
      {
        object computedValue = this.GetComputedValue(propertyReference);
        if (computedValue != null)
        {
          Microsoft.Expression.DesignModel.DocumentModel.DocumentNode correspondingDocumentNode = this.ViewModel.DefaultView.GetCorrespondingDocumentNode(this.Platform.ViewObjectFactory.Instantiate(computedValue), true);
          if (correspondingDocumentNode != null)
            sceneNode = this.ViewModel.GetSceneNode(correspondingDocumentNode.Clone(this.DocumentContext));
          if (sceneNode == null)
            sceneNode = this.ViewModel.CreateSceneNode(computedValue);
        }
      }
      else
        sceneNode = this.ViewModel.GetSceneNode(valueAsSceneNode.DocumentNode.Clone(this.DocumentContext));
      return sceneNode;
    }

    public Matrix GetComputedTransformFromVisualParent()
    {
      TextElementSceneElement elementSceneElement = this.ParentElement as TextElementSceneElement;
      if (elementSceneElement != null)
        return elementSceneElement.HostElement.GetComputedTransformToElement(this);
      IViewVisual viewVisual = this.Visual as IViewVisual;
      if (viewVisual == null || viewVisual.VisualParent == null)
        return Matrix.Identity;
      MatrixTransform matrixTransform = this.ViewModel.DefaultView.ComputeTransformToVisual(viewVisual.VisualParent, (IViewObject) viewVisual) as MatrixTransform;
      if (matrixTransform == null)
        return Matrix.Identity;
      return matrixTransform.Matrix;
    }

    public Matrix GetComputedTransformToElement(SceneElement target)
    {
      if (this.Visual == null || target.Visual == null)
        return Matrix.Identity;
      return ((MatrixTransform) this.ViewModel.DefaultView.ComputeTransformToVisual(this.Visual, target.Visual)).Matrix;
    }

    protected static bool IsDescendantOf(SceneElement item, SceneElement putativeAncestor)
    {
      for (; item.Parent != null; item = (SceneElement) item.Parent)
      {
        if (item.Parent == putativeAncestor)
          return true;
      }
      return false;
    }

    protected void DeselectSubtree()
    {
      SceneElementSelectionSet elementSelectionSet = this.ViewModel.ElementSelectionSet;
      if (!elementSelectionSet.IsEmpty)
      {
        List<SceneElement> list = new List<SceneElement>();
        foreach (SceneElement sceneElement1 in elementSelectionSet.Selection)
        {
          for (SceneElement sceneElement2 = sceneElement1; sceneElement2 != null; sceneElement2 = sceneElement2.ParentElement)
          {
            if (sceneElement2 == this)
            {
              list.Add(sceneElement1);
              break;
            }
          }
        }
        if (list.Count > 0)
          elementSelectionSet.RemoveSelection((ICollection<SceneElement>) list);
      }
      KeyFrameSelectionSet frameSelectionSet = this.ViewModel.KeyFrameSelectionSet;
      if (!frameSelectionSet.IsEmpty)
      {
        List<KeyFrameSceneNode> list = new List<KeyFrameSceneNode>();
        foreach (KeyFrameSceneNode keyFrameSceneNode in frameSelectionSet.Selection)
        {
          for (SceneNode sceneNode = keyFrameSceneNode.TargetElement; sceneNode != null; sceneNode = sceneNode.Parent)
          {
            if (sceneNode == this)
            {
              list.Add(keyFrameSceneNode);
              break;
            }
          }
        }
        if (list.Count > 0)
          frameSelectionSet.RemoveSelection((ICollection<KeyFrameSceneNode>) list);
      }
      ChildPropertySelectionSet propertySelectionSet = this.ViewModel.ChildPropertySelectionSet;
      if (!propertySelectionSet.IsEmpty)
      {
        List<SceneNode> list = new List<SceneNode>();
        foreach (SceneNode sceneNode in propertySelectionSet.Selection)
        {
          for (SceneNode parent = sceneNode.Parent; parent != null; parent = parent.Parent)
          {
            if (parent == this)
            {
              list.Add(sceneNode);
              break;
            }
          }
        }
        if (list.Count > 0)
          propertySelectionSet.RemoveSelection((ICollection<SceneNode>) list);
      }
      BehaviorSelectionSet behaviorSelectionSet = this.ViewModel.BehaviorSelectionSet;
      if (behaviorSelectionSet.IsEmpty)
        return;
      List<BehaviorBaseNode> list1 = new List<BehaviorBaseNode>();
      foreach (BehaviorBaseNode behaviorBaseNode in behaviorSelectionSet.Selection)
      {
        for (SceneNode parent = behaviorBaseNode.Parent; parent != null; parent = parent.Parent)
        {
          if (parent == this)
          {
            list1.Add(behaviorBaseNode);
            break;
          }
        }
      }
      if (list1.Count <= 0)
        return;
      behaviorSelectionSet.RemoveSelection((ICollection<BehaviorBaseNode>) list1);
    }

    protected override void ModifyValue(PropertyReference propertyReference, object valueToSet, SceneNode.Modification modification, int index)
    {
      this.EnsureTransformIfNeeded(propertyReference);
      if (this == this.ViewModel.ActiveStoryboardContainer && this.ViewModel.ActiveVisualTrigger != null)
      {
        DependencyPropertyReferenceStep propertyReferenceStep = propertyReference.FirstStep as DependencyPropertyReferenceStep;
        if (propertyReferenceStep != null)
        {
          DependencyProperty dependencyProperty = propertyReferenceStep.DependencyProperty as DependencyProperty;
          if (dependencyProperty != null)
          {
            MultiTriggerNode multiTriggerNode = this.ViewModel.ActiveVisualTrigger as MultiTriggerNode;
            if (multiTriggerNode != null)
            {
              foreach (ConditionNode conditionNode in (IEnumerable<ConditionNode>) multiTriggerNode.Conditions)
              {
                if (conditionNode.PropertyKey == dependencyProperty)
                  return;
              }
            }
            else
            {
              TriggerNode triggerNode = this.ViewModel.ActiveVisualTrigger as TriggerNode;
              if (triggerNode != null && ((ITriggerConditionNode) triggerNode).PropertyKey == dependencyProperty)
                return;
            }
          }
        }
      }
      base.ModifyValue(propertyReference, valueToSet, modification, index);
    }

    protected override void OnChildAdding(SceneNode child)
    {
      if (this.IsAttached)
        this.ViewModel.OnNodeAdding((SceneNode) this, child);
      base.OnChildAdding(child);
    }

    protected override void OnChildAdded(SceneNode child)
    {
      if (this.IsAttached)
      {
        this.ViewModel.OnNodeAdded((SceneNode) this, child);
        if (this.ViewModel.StateEditTarget != null && this.ViewModel.StateEditTarget.IsInDocument && (this.ViewModel.AnimationEditor.IsRecording && !this.ViewModel.IsDisablingDrawIntoState))
        {
          Base2DElement base2Delement = child as Base2DElement;
          if (base2Delement != null)
          {
            IProperty propertyForChild = base2Delement.Parent.GetPropertyForChild((SceneNode) base2Delement);
            if (propertyForChild == base2Delement.Parent.DefaultContentProperty && propertyForChild.MemberType != MemberType.DesignTimeProperty)
            {
              ReferenceStep singleStep = this.ViewModel.ProjectContext.ResolveProperty(Base2DElement.OpacityProperty) as ReferenceStep;
              base2Delement.SetLocalValue(Base2DElement.OpacityProperty, (object) 0.0);
              this.ViewModel.AnimationEditor.AddPropertyAutoKeyframe((SceneElement) base2Delement, new PropertyReference(singleStep), this.ViewModel.AnimationEditor.AnimationTime, (object) 0.0, (object) 1.0);
            }
          }
        }
      }
      base.OnChildAdded(child);
    }

    protected override void OnChildRemoving(SceneNode child)
    {
      if (this.IsAttached)
        this.ViewModel.OnNodeRemoving((SceneNode) this, child);
      base.OnChildRemoving(child);
    }

    protected override void OnChildRemoved(SceneNode child)
    {
      if (this.IsAttached)
        this.ViewModel.OnNodeRemoved((SceneNode) this, child);
      base.OnChildRemoved(child);
    }

    protected string GetPropertyNameHelper(DocumentNodePath documentNodePath)
    {
      IProperty containerOwnerProperty = documentNodePath.ContainerOwnerProperty;
      if (containerOwnerProperty != null && PlatformTypeHelper.GetDeclaringType((IMember) containerOwnerProperty) != typeof (DictionaryEntry))
        return containerOwnerProperty.Name;
      if (this.DocumentNode.Parent != null && this.DocumentNode.IsProperty && PlatformTypeHelper.GetDeclaringType((IMember) this.DocumentNode.SitePropertyKey) != typeof (DictionaryEntry))
        return this.DocumentNode.SitePropertyKey.Name;
      return (string) null;
    }

    internal bool TryAnimationSetValue(PropertyReference propertyReference, object value)
    {
      bool flag1 = false;
      string path = propertyReference.Path;
      if (path.EndsWith("/ScR") || path.EndsWith("/ScG") || (path.EndsWith("/ScB") || path.EndsWith("/ScA")) || (path.EndsWith("/X") || path.EndsWith("/Y") || path.EndsWith("/Z")))
      {
        int num = 0;
        List<ReferenceStep> steps = new List<ReferenceStep>();
        foreach (ReferenceStep referenceStep in propertyReference.ReferenceSteps)
        {
          steps.Add(referenceStep);
          if (!PlatformTypes.Color.IsAssignableFrom((ITypeId) referenceStep.PropertyType))
          {
            if (!PlatformTypes.Vector3D.IsAssignableFrom((ITypeId) referenceStep.PropertyType))
            {
              if (!PlatformTypes.Point3D.IsAssignableFrom((ITypeId) referenceStep.PropertyType))
              {
                if (!PlatformTypes.Vector.IsAssignableFrom((ITypeId) referenceStep.PropertyType))
                {
                  if (!PlatformTypes.Point.IsAssignableFrom((ITypeId) referenceStep.PropertyType))
                    ++num;
                  else
                    break;
                }
                else
                  break;
              }
              else
                break;
            }
            else
              break;
          }
          else
            break;
        }
        PropertyReference propertyReference1 = new PropertyReference(steps);
        if (propertyReference1 != null)
        {
          object computedValue = this.GetComputedValue(propertyReference1);
          object obj = propertyReference.PartialSetValue(computedValue, value, num + 1, propertyReference.ReferenceSteps.Count - 1);
          flag1 = true;
          if (!object.Equals(computedValue, obj) || this.IsSet(propertyReference1) != PropertyState.Set)
          {
            this.SetValue(propertyReference1, obj);
            flag1 = true;
          }
        }
      }
      if (!flag1 && propertyReference.Count > 1 && !PlatformTypes.Freezable.IsAssignableFrom((ITypeId) propertyReference[0].PropertyType))
      {
        PropertyReference propertyReference1 = new PropertyReference(propertyReference[0]);
        object computedValue = this.GetComputedValue(propertyReference1);
        object valueToSet = propertyReference.PartialSetValue(computedValue, value, 1, propertyReference.Count - 1);
        this.SetValue(propertyReference1, valueToSet);
        flag1 = true;
      }
      if (!flag1 && PlatformTypes.Brush.IsAssignableFrom((ITypeId) propertyReference.ValueTypeId) && this.IsSet(propertyReference) == PropertyState.Set)
      {
        object computedValue = this.GetComputedValue(propertyReference);
        ITypeResolver typeResolver = (ITypeResolver) this.ViewModel.ProjectContext;
        if (PlatformTypes.IsInstance(value, PlatformTypes.SolidColorBrush, typeResolver) && PlatformTypes.IsInstance(computedValue, PlatformTypes.SolidColorBrush, typeResolver))
        {
          flag1 = true;
          if (!PropertyUtilities.Compare(value, computedValue, this.ViewModel.DefaultView))
          {
            ReferenceStep step = (ReferenceStep) this.ProjectContext.ResolveProperty(SolidColorBrushNode.ColorProperty);
            this.SetValue(propertyReference.Append(step), step.GetCurrentValue(value));
          }
        }
        else if (PlatformTypes.IsInstance(value, PlatformTypes.LinearGradientBrush, typeResolver) && PlatformTypes.IsInstance(computedValue, PlatformTypes.LinearGradientBrush, typeResolver))
        {
          bool flag2 = true;
          ReferenceStep step1 = (ReferenceStep) this.ProjectContext.ResolveProperty(LinearGradientBrushNode.StartPointProperty);
          ReferenceStep step2 = (ReferenceStep) this.ProjectContext.ResolveProperty(LinearGradientBrushNode.EndPointProperty);
          if (!PropertyUtilities.Compare(step1.GetCurrentValue(value), step1.GetCurrentValue(computedValue), this.ViewModel.DefaultView))
            this.SetValue(propertyReference.Append(step1), step1.GetCurrentValue(value));
          if (!PropertyUtilities.Compare(step2.GetCurrentValue(value), step2.GetCurrentValue(computedValue), this.ViewModel.DefaultView))
            this.SetValue(propertyReference.Append(step2), step2.GetCurrentValue(value));
          flag1 = flag2 & this.DiffBrushTransform(propertyReference, value, computedValue) & this.DiffGradientStops(propertyReference, value, computedValue);
        }
        else if (PlatformTypes.IsInstance(value, PlatformTypes.RadialGradientBrush, typeResolver) && PlatformTypes.IsInstance(computedValue, PlatformTypes.RadialGradientBrush, typeResolver))
        {
          bool flag2 = true;
          ReferenceStep step1 = (ReferenceStep) this.ProjectContext.ResolveProperty(RadialGradientBrushNode.CenterProperty);
          ReferenceStep step2 = (ReferenceStep) this.ProjectContext.ResolveProperty(RadialGradientBrushNode.RadiusXProperty);
          ReferenceStep step3 = (ReferenceStep) this.ProjectContext.ResolveProperty(RadialGradientBrushNode.RadiusYProperty);
          ReferenceStep step4 = (ReferenceStep) this.ProjectContext.ResolveProperty(RadialGradientBrushNode.GradientOriginProperty);
          if (!PropertyUtilities.Compare(step1.GetCurrentValue(value), step1.GetCurrentValue(computedValue), this.ViewModel.DefaultView))
            this.SetValue(propertyReference.Append(step1), step1.GetCurrentValue(value));
          if (!PropertyUtilities.Compare(step2.GetCurrentValue(value), step2.GetCurrentValue(computedValue), this.ViewModel.DefaultView))
            this.SetValue(propertyReference.Append(step2), step2.GetCurrentValue(value));
          if (!PropertyUtilities.Compare(step3.GetCurrentValue(value), step3.GetCurrentValue(computedValue), this.ViewModel.DefaultView))
            this.SetValue(propertyReference.Append(step3), step3.GetCurrentValue(value));
          if (!PropertyUtilities.Compare(step4.GetCurrentValue(value), step4.GetCurrentValue(computedValue), this.ViewModel.DefaultView))
            this.SetValue(propertyReference.Append(step4), step4.GetCurrentValue(value));
          flag1 = flag2 & this.DiffBrushTransform(propertyReference, value, computedValue) & this.DiffGradientStops(propertyReference, value, computedValue);
        }
      }
      else if (PlatformTypes.Transform.IsAssignableFrom((ITypeId) propertyReference.ValueTypeId))
      {
        flag1 = true;
        if (!propertyReference.IsSet(this.ViewObject.PlatformSpecificObject))
        {
          object platformTransform = new CanonicalTransform().GetPlatformTransform(this.Platform.GeometryHelper);
          using (this.ViewModel.ForceBaseValue())
            this.SetValue(propertyReference, platformTransform);
          this.EnsureViewTransform(propertyReference, platformTransform);
        }
        CanonicalTransform canonicalTransform1 = new CanonicalTransform((Transform) this.GetComputedValueAsWpf(propertyReference));
        CanonicalTransform canonicalTransform2 = new CanonicalTransform((Transform) this.ViewModel.DefaultView.ConvertToWpfValue(value));
        if (!object.Equals((object) canonicalTransform1.ScaleX, (object) canonicalTransform2.ScaleX))
          this.SetCanonicalTransformProperty(propertyReference, (object) RoundingHelper.RoundScale(canonicalTransform2.ScaleX), this.Platform.Metadata.CommonProperties.ScaleX);
        if (!object.Equals((object) canonicalTransform1.ScaleY, (object) canonicalTransform2.ScaleY))
          this.SetCanonicalTransformProperty(propertyReference, (object) RoundingHelper.RoundScale(canonicalTransform2.ScaleY), this.Platform.Metadata.CommonProperties.ScaleY);
        if (!object.Equals((object) canonicalTransform1.SkewX, (object) canonicalTransform2.SkewX))
          this.SetCanonicalTransformProperty(propertyReference, (object) RoundingHelper.RoundScale(canonicalTransform2.SkewX), this.Platform.Metadata.CommonProperties.SkewX);
        if (!object.Equals((object) canonicalTransform1.SkewY, (object) canonicalTransform2.SkewY))
          this.SetCanonicalTransformProperty(propertyReference, (object) RoundingHelper.RoundScale(canonicalTransform2.SkewY), this.Platform.Metadata.CommonProperties.SkewY);
        if (!object.Equals((object) canonicalTransform1.RotationAngle, (object) canonicalTransform2.RotationAngle))
          this.SetCanonicalTransformProperty(propertyReference, (object) RoundingHelper.RoundAngle(canonicalTransform2.RotationAngle), this.Platform.Metadata.CommonProperties.RotationAngle);
        if (!object.Equals((object) canonicalTransform1.TranslationX, (object) canonicalTransform2.TranslationX))
          this.SetCanonicalTransformProperty(propertyReference, (object) RoundingHelper.RoundLength(canonicalTransform2.TranslationX), this.Platform.Metadata.CommonProperties.TranslationX);
        if (!object.Equals((object) canonicalTransform1.TranslationY, (object) canonicalTransform2.TranslationY))
          this.SetCanonicalTransformProperty(propertyReference, (object) RoundingHelper.RoundLength(canonicalTransform2.TranslationY), this.Platform.Metadata.CommonProperties.TranslationY);
        if (!object.Equals((object) canonicalTransform1.CenterX, (object) canonicalTransform2.CenterX))
          this.SetCanonicalTransformProperty(propertyReference, (object) RoundingHelper.RoundLength(canonicalTransform2.CenterX), new PropertyReference(TransformProperties.GetCenterXStep(this.DesignerDefaultPlatform.Metadata)));
        if (!object.Equals((object) canonicalTransform1.CenterY, (object) canonicalTransform2.CenterY))
          this.SetCanonicalTransformProperty(propertyReference, (object) RoundingHelper.RoundLength(canonicalTransform2.CenterY), new PropertyReference(TransformProperties.GetCenterYStep(this.DesignerDefaultPlatform.Metadata)));
      }
      else if (propertyReference.Contains(Model3DElement.TransformProperty, (ITypeResolver) this.ProjectContext) || propertyReference.Contains(ModelVisual3DElement.TransformProperty, (ITypeResolver) this.ProjectContext) || propertyReference.Contains(CameraElement.TransformProperty, (ITypeResolver) this.ProjectContext))
      {
        int finalStepIndex = propertyReference.IndexOf(ModelVisual3DElement.TransformProperty, (ITypeResolver) this.ProjectContext);
        if (finalStepIndex < 0)
        {
          finalStepIndex = propertyReference.IndexOf(Model3DElement.TransformProperty, (ITypeResolver) this.ProjectContext);
          if (finalStepIndex < 0)
            finalStepIndex = propertyReference.IndexOf(CameraElement.TransformProperty, (ITypeResolver) this.ProjectContext);
        }
        if (!CanonicalTransform3D.IsCanonical(this.PartialGetValue(propertyReference, 0, finalStepIndex) as Transform3D))
        {
          CanonicalTransform3D canonicalTransform3D = new CanonicalTransform3D((Transform3D) this.PartialGetValue(propertyReference, 0, finalStepIndex));
          using (this.ViewModel.ForceBaseValue())
          {
            ReferenceStep[] steps = new ReferenceStep[finalStepIndex + 1];
            for (int index = 0; index <= finalStepIndex; ++index)
              steps[index] = propertyReference.ReferenceSteps[index];
            this.SetValue(PropertyReference.CreateNewPropertyReferenceFromStepsWithoutCopy(steps), (object) canonicalTransform3D.ToTransform());
          }
        }
        if (propertyReference.Count == 1)
        {
          flag1 = true;
          CanonicalTransform3D canonicalTransform3D1 = new CanonicalTransform3D((Transform3D) this.GetComputedValue(propertyReference));
          CanonicalTransform3D canonicalTransform3D2 = new CanonicalTransform3D((Transform3D) value);
          Base3DElement base3Delement = this as Base3DElement;
          if (base3Delement != null)
          {
            if (!Vector3D.Equals(canonicalTransform3D1.Scale, canonicalTransform3D2.Scale))
            {
              base3Delement.CanonicalScaleX = canonicalTransform3D2.ScaleX;
              base3Delement.CanonicalScaleY = canonicalTransform3D2.ScaleY;
              base3Delement.CanonicalScaleZ = canonicalTransform3D2.ScaleZ;
            }
            if (!Vector3D.Equals(canonicalTransform3D1.RotationAngles, canonicalTransform3D2.RotationAngles))
              base3Delement.CanonicalRotationAngles = canonicalTransform3D2.RotationAngles;
            if (!Vector3D.Equals(canonicalTransform3D1.Translation, canonicalTransform3D2.Translation))
            {
              base3Delement.CanonicalTranslationX = canonicalTransform3D2.TranslationX;
              base3Delement.CanonicalTranslationY = canonicalTransform3D2.TranslationY;
              base3Delement.CanonicalTranslationZ = canonicalTransform3D2.TranslationZ;
            }
          }
        }
      }
      else
      {
        ReferenceStep referenceStep = (ReferenceStep) this.ProjectContext.ResolveProperty(BrushNode.RelativeTransformProperty);
        if (propertyReference.ReferenceSteps.Contains(referenceStep))
        {
          int endIndex = propertyReference.ReferenceSteps.IndexOf(referenceStep);
          Transform wpfTransform = Transform.Identity;
          if (!this.TryGetCanonicalTransform(propertyReference.Subreference(0, endIndex), out wpfTransform, true, false))
          {
            CanonicalTransform canonicalTransform = new CanonicalTransform(wpfTransform);
            using (this.ViewModel.AnimationEditor.DeferKeyFraming())
            {
              ReferenceStep[] steps = new ReferenceStep[endIndex + 1];
              for (int index = 0; index <= endIndex; ++index)
                steps[index] = propertyReference.ReferenceSteps[index];
              PropertyReference stepsWithoutCopy = PropertyReference.CreateNewPropertyReferenceFromStepsWithoutCopy(steps);
              object platformTransform = canonicalTransform.GetPlatformTransform(this.Platform.GeometryHelper);
              this.SetValue(stepsWithoutCopy, platformTransform);
              this.EnsureViewTransform(stepsWithoutCopy, platformTransform);
            }
          }
        }
      }
      return flag1;
    }

    private void SetTransformCenterProperties(PropertyReference basePropertyReference, object value, ReferenceStep centerProperty)
    {
      if (this.Platform.Metadata.IsCapabilitySet(PlatformCapability.SupportsCompositeTransform))
      {
        using (this.ViewModel.ForceBaseValue())
        {
          if (centerProperty.Equals((object) TransformProperties.GetCenterXStep(this.DesignerDefaultPlatform.Metadata)))
            this.SetValue(basePropertyReference.Append(CompositeTransformNode.CenterXProperty), value);
          else
            this.SetValue(basePropertyReference.Append(CompositeTransformNode.CenterYProperty), value);
        }
      }
      else
      {
        IPropertyId propertyKey1;
        IPropertyId propertyKey2;
        IPropertyId propertyKey3;
        if (centerProperty.Equals((object) TransformProperties.GetCenterXStep(this.DesignerDefaultPlatform.Metadata)))
        {
          propertyKey1 = RotateTransformNode.CenterXProperty;
          propertyKey2 = SkewTransformNode.CenterXProperty;
          propertyKey3 = ScaleTransformNode.CenterXProperty;
        }
        else
        {
          propertyKey1 = RotateTransformNode.CenterYProperty;
          propertyKey2 = SkewTransformNode.CenterYProperty;
          propertyKey3 = ScaleTransformNode.CenterYProperty;
        }
        using (this.ViewModel.ForceBaseValue())
        {
          using (SceneNode.DisableEnsureTransform(true))
          {
            PropertyReference propertyReference = basePropertyReference.Append(TransformGroupNode.ChildrenProperty);
            ReferenceStep step1 = (ReferenceStep) IndexedClrPropertyReferenceStep.GetReferenceStep((IPlatformMetadata) this.Platform.Metadata, PlatformTypes.TransformCollection, CanonicalTransformOrder.RotateIndex);
            this.SetValue(propertyReference.Append(step1).Append(propertyKey1), value);
            ReferenceStep step2 = (ReferenceStep) IndexedClrPropertyReferenceStep.GetReferenceStep((IPlatformMetadata) this.Platform.Metadata, PlatformTypes.TransformCollection, CanonicalTransformOrder.SkewIndex);
            this.SetValue(propertyReference.Append(step2).Append(propertyKey2), value);
            ReferenceStep step3 = (ReferenceStep) IndexedClrPropertyReferenceStep.GetReferenceStep((IPlatformMetadata) this.Platform.Metadata, PlatformTypes.TransformCollection, CanonicalTransformOrder.ScaleIndex);
            this.SetValue(propertyReference.Append(step3).Append(propertyKey3), value);
          }
        }
      }
    }

    private void SetCanonicalTransformProperty(PropertyReference propertyReference, object value, PropertyReference canonicalReference)
    {
      PropertyReference propertyReference1 = canonicalReference;
      ReferenceStep centerXstep = TransformProperties.GetCenterXStep(this.DesignerDefaultPlatform.Metadata);
      ReferenceStep centerYstep = TransformProperties.GetCenterYStep(this.DesignerDefaultPlatform.Metadata);
      if (centerXstep.Equals((object) canonicalReference.LastStep))
        this.SetTransformCenterProperties(propertyReference, value, centerXstep);
      else if (centerYstep.Equals((object) canonicalReference.LastStep))
        this.SetTransformCenterProperties(propertyReference, value, centerYstep);
      else
        this.SetValue(propertyReference.Append(propertyReference1), value);
    }

    private bool DiffBrushTransform(PropertyReference propertyReference, object value, object oldValue)
    {
      bool flag = false;
      PropertyReference propertyReference1 = new PropertyReference((ReferenceStep) this.ProjectContext.ResolveProperty(BrushNode.RelativeTransformProperty));
      PropertyReference propertyReference2 = propertyReference.Append(propertyReference1);
      Transform transform1 = this.ViewModel.DefaultView.ConvertToWpfValue(propertyReference1.GetCurrentValue(oldValue)) as Transform;
      Transform transform2 = this.ViewModel.DefaultView.ConvertToWpfValue(propertyReference1.GetCurrentValue(value)) as Transform;
      if (transform1 != null && transform2 != null)
      {
        CanonicalTransform canonicalTransform1 = new CanonicalTransform(transform1);
        CanonicalTransform canonicalTransform2 = new CanonicalTransform(transform2);
        flag = true;
        if (canonicalTransform1.CenterX != canonicalTransform2.CenterX)
          this.SetValue(propertyReference2.Append(TransformProperties.GetCenterXStep(this.DesignerDefaultPlatform.Metadata)), (object) canonicalTransform2.CenterX);
        if (canonicalTransform1.CenterY != canonicalTransform2.CenterY)
          this.SetValue(propertyReference2.Append(TransformProperties.GetCenterYStep(this.DesignerDefaultPlatform.Metadata)), (object) canonicalTransform2.CenterY);
        if (canonicalTransform1.ScaleX != canonicalTransform2.ScaleX)
          this.SetValue(propertyReference2.Append(this.Platform.Metadata.CommonProperties.ScaleX), (object) canonicalTransform2.ScaleX);
        if (canonicalTransform1.ScaleY != canonicalTransform2.ScaleY)
          this.SetValue(propertyReference2.Append(this.Platform.Metadata.CommonProperties.ScaleY), (object) canonicalTransform2.ScaleY);
        if (canonicalTransform1.SkewX != canonicalTransform2.SkewX)
          this.SetValue(propertyReference2.Append(this.Platform.Metadata.CommonProperties.SkewX), (object) canonicalTransform2.SkewX);
        if (canonicalTransform1.SkewY != canonicalTransform2.SkewY)
          this.SetValue(propertyReference2.Append(this.Platform.Metadata.CommonProperties.SkewY), (object) canonicalTransform2.SkewY);
        if (canonicalTransform1.TranslationX != canonicalTransform2.TranslationX)
          this.SetValue(propertyReference2.Append(this.Platform.Metadata.CommonProperties.TranslationX), (object) canonicalTransform2.TranslationX);
        if (canonicalTransform1.TranslationY != canonicalTransform2.TranslationY)
          this.SetValue(propertyReference2.Append(this.Platform.Metadata.CommonProperties.TranslationY), (object) canonicalTransform2.TranslationY);
        if (canonicalTransform1.RotationAngle != canonicalTransform2.RotationAngle)
          this.SetValue(propertyReference2.Append(this.Platform.Metadata.CommonProperties.RotationAngle), (object) canonicalTransform2.RotationAngle);
      }
      return flag;
    }

    private bool DiffGradientStops(PropertyReference propertyReference, object value, object oldValue)
    {
      ITypeResolver defaultTypeResolver = this.Platform.Metadata.DefaultTypeResolver;
      PropertyReference propertyReference1 = new PropertyReference((ReferenceStep) defaultTypeResolver.ResolveProperty(GradientBrushNode.GradientStopsProperty));
      GradientStopCollection gradientStopCollection1 = (GradientStopCollection) this.ViewModel.DefaultView.ConvertToWpfValue(propertyReference1.GetCurrentValue(oldValue));
      GradientStopCollection gradientStopCollection2 = (GradientStopCollection) this.ViewModel.DefaultView.ConvertToWpfValue(propertyReference1.GetCurrentValue(value));
      if (gradientStopCollection1.Count != gradientStopCollection2.Count)
        return false;
      PropertyReference propertyReference2 = propertyReference.Append(GradientBrushNode.GradientStopsProperty);
      for (int index = 0; index < gradientStopCollection1.Count; ++index)
      {
        ReferenceStep step = (ReferenceStep) IndexedClrPropertyReferenceStep.GetReferenceStep(defaultTypeResolver, PlatformTypes.GradientStopCollection, index);
        if (gradientStopCollection1[index].Offset != gradientStopCollection2[index].Offset)
          this.SetValueAsWpf(propertyReference2.Append(step).Append(GradientStopNode.OffsetProperty), (object) gradientStopCollection2[index].Offset);
        if (gradientStopCollection1[index].Color != gradientStopCollection2[index].Color)
          this.SetValueAsWpf(propertyReference2.Append(step).Append(GradientStopNode.ColorProperty), (object) gradientStopCollection2[index].Color);
      }
      return true;
    }
  }
}
