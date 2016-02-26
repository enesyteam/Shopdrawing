// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.SceneNodeObjectSetBase
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.Tools.Layout;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.UserInterface.DataPane;
using Microsoft.Expression.DesignSurface.UserInterface.ResourcePane;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.DesignSurface.ViewModel.Text;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.PropertyInspector;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public abstract class SceneNodeObjectSetBase : SceneNodeObjectSet
  {
    private bool? isViewRepresentationValid = new bool?();
    private DesignerContext designerContext;
    private ObservableCollectionWorkaround<LocalResourceModel> localResources;
    private ObservableCollectionWorkaround<TemplateBindablePropertyModel> templateBindableProperties;
    private ObservableCollectionWorkaround<SystemResourceModel> systemResources;

    public override bool PropertyUpdatesLocked { get; set; }

    public override bool IsViewRepresentationValid
    {
      get
      {
        if (!this.isViewRepresentationValid.HasValue)
        {
          this.isViewRepresentationValid = new bool?(true);
          foreach (SceneNode sceneNode in this.Objects)
          {
            if (!sceneNode.IsViewObjectValid)
            {
              this.isViewRepresentationValid = new bool?(false);
              break;
            }
          }
        }
        return this.isViewRepresentationValid.Value;
      }
    }

    public override bool CanSetBindingExpression
    {
      get
      {
        return true;
      }
    }

    public override bool CanSetDynamicExpression
    {
      get
      {
        foreach (SceneNode sceneNode in this.Objects)
        {
          if (!PlatformTypes.DependencyObject.IsAssignableFrom((ITypeId) sceneNode.Type) && !PlatformTypes.Style.IsAssignableFrom((ITypeId) sceneNode.Type) || PlatformTypes.IKeyFrame.IsAssignableFrom((ITypeId) sceneNode.Type))
            return false;
        }
        return true;
      }
    }

    public override bool IsTextRange
    {
      get
      {
        if (this.Count == 0)
          return false;
        foreach (SceneNode sceneNode in this.Objects)
        {
          if (!(sceneNode is TextRangeElement))
            return false;
        }
        return true;
      }
    }

    internal override DesignerContext DesignerContext
    {
      get
      {
        return this.designerContext;
      }
    }

    public override SceneDocument Document
    {
      get
      {
        if (this.ViewModel == null)
          return (SceneDocument) null;
        return this.ViewModel.Document;
      }
    }

    public override IDocumentContext DocumentContext
    {
      get
      {
        if (this.Document == null)
          return (IDocumentContext) null;
        return this.Document.DocumentContext;
      }
    }

    public override IProjectContext ProjectContext
    {
      get
      {
        if (this.Document == null)
          return (IProjectContext) null;
        return this.Document.ProjectContext;
      }
    }

    public override DocumentNode RepresentativeNode
    {
      get
      {
        if (this.Count > 0)
          return this.Objects[0].DocumentNode;
        return (DocumentNode) null;
      }
    }

    public override SceneNode RepresentativeSceneNode
    {
      get
      {
        if (this.Count > 0)
          return this.Objects[0];
        return (SceneNode) null;
      }
    }

    public override Type ObjectType
    {
      get
      {
        IType objectTypeId = this.ObjectTypeId;
        if (objectTypeId == null)
          return (Type) null;
        return objectTypeId.RuntimeType;
      }
    }

    public override IType ObjectTypeId
    {
      get
      {
        IType type1 = (IType) null;
        SceneNode[] objects = this.Objects;
        if (objects.Length > 0)
        {
          type1 = SceneNodeObjectSetBase.GetStyleTargetTypeId(objects[0]);
          for (int index = 1; index < objects.Length; ++index)
          {
            ITypeId type2 = (ITypeId) SceneNodeObjectSetBase.GetStyleTargetTypeId(objects[index]);
            while (type1 != null && !type1.IsAssignableFrom(type2))
              type1 = type1.BaseType;
            if (type1 == null)
              break;
          }
        }
        return type1;
      }
    }

    public override bool IsTargetedByAnimation
    {
      get
      {
        if (this.Count == 1)
        {
          SceneElement element = this.Objects[0] as SceneElement;
          if (element != null && element.ViewModel.AnimationEditor.IsTargetedByName(element))
            return true;
        }
        return false;
      }
    }

    public override ObservableCollection<LocalResourceModel> LocalResources
    {
      get
      {
        if (this.localResources == null)
          this.localResources = this.RecalculateLocalResources((ObservableCollectionWorkaround<LocalResourceModel>) null);
        return (ObservableCollection<LocalResourceModel>) this.localResources;
      }
    }

    public override ObservableCollection<TemplateBindablePropertyModel> TemplateBindableProperties
    {
      get
      {
        if (this.templateBindableProperties == null)
        {
          this.templateBindableProperties = new ObservableCollectionWorkaround<TemplateBindablePropertyModel>();
          this.RecalculateTemplateBindableProperties();
        }
        return (ObservableCollection<TemplateBindablePropertyModel>) this.templateBindableProperties;
      }
    }

    public override ObservableCollection<SystemResourceModel> SystemResources
    {
      get
      {
        if (this.systemResources == null)
        {
          this.systemResources = new ObservableCollectionWorkaround<SystemResourceModel>();
          SceneNodeObjectSetBase.CollectResourceKeyProperties(typeof (SystemColors), (ObservableCollection<SystemResourceModel>) this.systemResources);
          SceneNodeObjectSetBase.CollectResourceKeyProperties(typeof (SystemFonts), (ObservableCollection<SystemResourceModel>) this.systemResources);
          SceneNodeObjectSetBase.CollectResourceKeyProperties(typeof (SystemParameters), (ObservableCollection<SystemResourceModel>) this.systemResources);
          this.systemResources.Sort((Comparison<SystemResourceModel>) ((left, right) => string.Compare(left.ResourceName, right.ResourceName, StringComparison.CurrentCulture)));
        }
        return (ObservableCollection<SystemResourceModel>) this.systemResources;
      }
    }

    internal SceneNodeObjectSetBase(DesignerContext designerContext, IPropertyInspector transactionContext)
      : base(transactionContext)
    {
      this.designerContext = designerContext;
      this.designerContext.SelectionManager.LateActiveSceneUpdatePhase += new SceneUpdatePhaseEventHandler(this.SelectionManager_LateActiveSceneUpdatePhase);
    }

    private void SelectionManager_LateActiveSceneUpdatePhase(object sender, SceneUpdatePhaseEventArgs args)
    {
      if (this.Document == null || this.Document.HasOpenTransaction)
        return;
      this.isViewRepresentationValid = new bool?();
      this.OnPropertyChanged("IsViewRepresentationValid");
    }

    public virtual void Dispose()
    {
      this.designerContext.SelectionManager.LateActiveSceneUpdatePhase -= new SceneUpdatePhaseEventHandler(this.SelectionManager_LateActiveSceneUpdatePhase);
    }

    public override SceneNode CreateAndSetBindingOrData(SceneNodeProperty property)
    {
      PerformanceUtility.MeasurePerformanceUntilRender(PerformanceEvent.DatabindCommand);
      SceneNode sceneNode = (SceneNode) ((object[]) this.Objects)[0];
      SceneNode target;
      if (property.Reference.Count == 1 || this.IsTextRange)
      {
        target = sceneNode;
      }
      else
      {
        using (SceneEditTransaction editTransaction = sceneNode.ViewModel.CreateEditTransaction("Build local value tree", true))
        {
          sceneNode.EnsureNodeTree(property.Reference, true, true);
          editTransaction.Commit();
        }
        PropertyReference propertyReference = property.Reference.Subreference(0, property.Reference.Count - 2);
        target = sceneNode.GetLocalValueAsSceneNode(propertyReference);
      }
      ReferenceStep lastStep = property.Reference.LastStep;
      return DataBindingDialog.CreateAndSetBindingOrData(this.designerContext, target, lastStep);
    }

    public override void SetValueToLocalResource(SceneNodeProperty propertyKey, LocalResourceModel localResource)
    {
      DocumentNode resourceKey = localResource.ResourceKey;
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.ApplyResource, "Apply resource.");
      SceneViewModel viewModel = this.ViewModel;
      if (viewModel != null)
      {
        IDocumentContext documentContext = viewModel.Document.DocumentContext;
        IProjectContext projectContext = viewModel.ProjectContext;
        DocumentNode keyNode = resourceKey.Clone(documentContext);
        bool flag = this.CanSetDynamicExpression && JoltHelper.TypeSupported((ITypeResolver) projectContext, PlatformTypes.DynamicResource);
        if (flag && this.ShouldAllowAnimation && (propertyKey.IsEnabledRecordCurrentValue && this.ViewModel.AnimationEditor.IsRecording))
          flag = false;
        DocumentNode documentNode = !flag ? (DocumentNode) DocumentNodeUtilities.NewStaticResourceNode(documentContext, keyNode) : (DocumentNode) DocumentNodeUtilities.NewDynamicResourceNode(documentContext, keyNode);
        using (this.ShouldAllowAnimation ? (IDisposable) null : viewModel.AnimationEditor.DeferKeyFraming())
        {
          string description = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.PropertyChangeUndoDescription, new object[1]
          {
            (object) ((PropertyEntry) propertyKey).get_PropertyName()
          });
          if (viewModel.Document.IsEditable)
          {
            using (SceneEditTransaction editTransaction = viewModel.CreateEditTransaction(description))
            {
              this.SetValue((PropertyReferenceProperty) propertyKey, (object) documentNode);
              bool isMixed;
              DocumentNode valueAsDocumentNode = this.GetLocalValueAsDocumentNode(propertyKey, GetLocalValueFlags.CheckKeyframes, out isMixed);
              if (!isMixed && valueAsDocumentNode != null)
                Microsoft.Expression.DesignSurface.Utility.ResourceHelper.EnsureReferencedResourcesAreReachable(localResource.ResourceNode, valueAsDocumentNode);
              editTransaction.Commit();
            }
          }
        }
      }
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.ApplyResource, "Apply resource.");
    }

    public override void SetValueToSystemResource(SceneNodeProperty propertyKey, SystemResourceModel systemResource)
    {
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.ApplyResource, "Apply system resource.");
      SceneViewModel viewModel = this.ViewModel;
      if (viewModel != null)
      {
        IDocumentContext documentContext = viewModel.Document.DocumentContext;
        IType type = viewModel.ProjectContext.ProjectNamespaces.GetType((IXmlNamespace) XmlNamespace.AvalonXmlNamespace, systemResource.CollectionName);
        if (type != null)
        {
          IMember memberId = (IMember) type.GetMember(MemberType.LocalProperty | MemberType.Field, systemResource.ResourceName, MemberAccessTypes.Public);
          DocumentNode documentNode = (DocumentNode) null;
          if (memberId != null)
            documentNode = (DocumentNode) DocumentNodeUtilities.NewDynamicResourceNode(documentContext, (DocumentNode) DocumentNodeUtilities.NewStaticNode(documentContext, memberId));
          using (this.ShouldAllowAnimation ? (IDisposable) null : viewModel.AnimationEditor.DeferKeyFraming())
            this.SetValue((PropertyReferenceProperty) propertyKey, (object) documentNode);
        }
      }
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.ApplyResource, "Apply resource.");
    }

    public override void SetValueToTemplateBinding(SceneNodeProperty propertyKey, ReferenceStep referenceStep)
    {
      SceneViewModel viewModel = this.ViewModel;
      if (viewModel == null)
        return;
      DocumentNode documentNode = (DocumentNode) DocumentNodeUtilities.NewTemplateBindingNode(this.RepresentativeNode, (IPropertyId) referenceStep, propertyKey.Reference);
      using (viewModel.AnimationEditor.DeferKeyFraming())
        this.SetValue((PropertyReferenceProperty) propertyKey, (object) documentNode);
    }

    public override DocumentNode GetLocalValueAsDocumentNode(SceneNodeProperty property, GetLocalValueFlags flags, out bool isMixed)
    {
      DocumentNode documentNode = (DocumentNode) null;
      bool flag = false;
      foreach (SceneNode sceneNode1 in this.Objects)
      {
        PropertyReference propertyReference1 = SceneNodeObjectSet.FilterProperty(sceneNode1, property.Reference);
        if (propertyReference1 != null)
        {
          DocumentNode other = (DocumentNode) null;
          if ((flags & GetLocalValueFlags.CheckKeyframes) != GetLocalValueFlags.None && sceneNode1.ViewModel.AnimationEditor.ActiveStoryboardTimeline != null && this.ShouldAllowAnimation)
          {
            SceneNode ancestor = (SceneNode) null;
            PropertyReference propertyReference2 = propertyReference1;
            SceneNode sceneNode2 = sceneNode1;
            if (this.FindAncestor(sceneNode1, out ancestor, ref propertyReference2, new Predicate<SceneNode>(SceneNodeObjectSetBase.IsAnimationParent)))
              sceneNode2 = ancestor;
            foreach (TimelineSceneNode timelineSceneNode in (IEnumerable<TimelineSceneNode>) sceneNode1.ViewModel.AnimationEditor.ActiveStoryboardTimeline.Children)
            {
              TimelineSceneNode.PropertyNodePair elementAndProperty = timelineSceneNode.TargetElementAndProperty;
              if (elementAndProperty.SceneNode == sceneNode2 && elementAndProperty.PropertyReference != null)
              {
                PropertyReference propertyReference3 = SceneNodeObjectSet.FilterProperty(elementAndProperty.SceneNode, elementAndProperty.PropertyReference);
                if (propertyReference2.Equals((object) propertyReference3))
                {
                  KeyFrameAnimationSceneNode animationSceneNode1 = timelineSceneNode as KeyFrameAnimationSceneNode;
                  FromToAnimationSceneNode animationSceneNode2 = timelineSceneNode as FromToAnimationSceneNode;
                  if (animationSceneNode1 != null)
                  {
                    KeyFrameSceneNode keyFrameAtTime = animationSceneNode1.GetKeyFrameAtTime(sceneNode1.ViewModel.AnimationEditor.AnimationTime);
                    if (keyFrameAtTime != null)
                    {
                      using ((flags & GetLocalValueFlags.Resolve) == GetLocalValueFlags.None ? this.ViewModel.ForceBaseValue() : (IDisposable) null)
                      {
                        other = keyFrameAtTime.ValueNode;
                        break;
                      }
                    }
                    else
                      break;
                  }
                  else if (animationSceneNode2 != null)
                  {
                    double animationTime = sceneNode1.ViewModel.AnimationEditor.AnimationTime;
                    using ((flags & GetLocalValueFlags.Resolve) == GetLocalValueFlags.None ? this.ViewModel.ForceBaseValue() : (IDisposable) null)
                    {
                      DocumentNodePath documentNodePath = (DocumentNodePath) null;
                      if (animationTime == animationSceneNode2.Begin + animationSceneNode2.Duration)
                        documentNodePath = animationSceneNode2.GetLocalValueAsDocumentNode(animationSceneNode2.ToProperty);
                      else if (animationTime == animationSceneNode2.Begin)
                        documentNodePath = animationSceneNode2.GetLocalValueAsDocumentNode(animationSceneNode2.FromProperty);
                      other = documentNodePath != null ? documentNodePath.Node : (DocumentNode) null;
                      break;
                    }
                  }
                  else
                    break;
                }
              }
            }
          }
          if (other == null)
          {
            if ((flags & GetLocalValueFlags.Resolve) != GetLocalValueFlags.None)
            {
              DocumentNodePath valueAsDocumentNode = sceneNode1.GetLocalValueAsDocumentNode(propertyReference1);
              if (valueAsDocumentNode != null)
                other = valueAsDocumentNode.Node;
            }
            else
              other = (DocumentNode) sceneNode1.GetLocalValue(propertyReference1, PropertyContext.AsDocumentNodes);
          }
          if (!flag)
          {
            if (other == null && (flags & GetLocalValueFlags.SkipCheckIfMixed) != GetLocalValueFlags.None)
            {
              isMixed = false;
              return (DocumentNode) null;
            }
            flag = true;
            documentNode = other;
          }
          else if (documentNode == null && other != null || documentNode != null && !documentNode.Equals(other))
          {
            isMixed = true;
            return (DocumentNode) null;
          }
        }
      }
      isMixed = false;
      return documentNode;
    }

    public override DocumentNode GetRawValue(IDocumentContext documentContext, PropertyReference propertyReference, PropertyReference.GetValueFlags getValueFlags)
    {
      object obj = this.GetValue(propertyReference, getValueFlags);
      if (obj != null)
        return documentContext.CreateNode(obj.GetType(), obj);
      return (DocumentNode) null;
    }

    public override object GetValue(PropertyReference propertyReference, PropertyReference.GetValueFlags getValueFlags)
    {
      object second = null;
      bool flag = false;
      foreach (SceneNode sceneNode in this.Objects)
      {
        PropertyReference propertyReference1 = SceneNodeObjectSet.FilterProperty(sceneNode, propertyReference);
        if (propertyReference1 != null)
        {
          object first;
          if ((getValueFlags & PropertyReference.GetValueFlags.Computed) != PropertyReference.GetValueFlags.Local)
          {
            if (SceneNodeObjectSetBase.IsValidForGetComputedValue(sceneNode))
            {
              first = sceneNode.GetComputedValue(propertyReference1);
            }
            else
            {
              SceneNode ancestor = (SceneNode) null;
              PropertyReference propertyReference2 = propertyReference1;
              first = !this.ShouldWalkParentsForGetValue || !this.FindAncestor(sceneNode, out ancestor, ref propertyReference2, new Predicate<SceneNode>(SceneNodeObjectSetBase.IsValidForGetComputedValue)) ? sceneNode.GetLocalOrDefaultValue(propertyReference1) : ancestor.GetComputedValue(propertyReference2);
            }
          }
          else
            first = sceneNode.GetLocalOrDefaultValue(propertyReference1);
          if (!flag)
          {
            second = first;
            flag = true;
          }
          else if (!PropertyUtilities.Compare(first, second, sceneNode.ViewModel.DefaultView))
          {
            second = MixedProperty.Mixed;
            break;
          }
        }
      }
      if (!flag)
      {
        ReferenceStep referenceStep = propertyReference[propertyReference.Count - 1];
        if (this.designerContext.ActiveView != null)
          second = referenceStep.GetDefaultValue(referenceStep.TargetType);
      }
      return second;
    }

    protected override void ModifyValue(PropertyReferenceProperty property, object valueToSet, Modification modification, int index)
    {
      if (this.ViewModel == null)
        return;
      string description = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.PropertyChangeUndoDescription, new object[1]
      {
        (object) ((PropertyEntry) property).get_PropertyName()
      });
      if (!this.ViewModel.DefaultView.Document.IsEditable)
        return;
      using (SceneEditTransaction editTransaction = this.ViewModel.CreateEditTransaction(description, false, SceneEditTransactionType.NestedInAutoClosing))
      {
        try
        {
          foreach (SceneNode sceneNode1 in this.Objects)
          {
            PropertyReference propertyReference1 = SceneNodeObjectSet.FilterProperty(sceneNode1, property.Reference);
            if (propertyReference1 != null)
            {
              SceneNode sceneNode2 = sceneNode1;
              if (this.ShouldAllowAnimation)
              {
                SceneNode ancestor = (SceneNode) null;
                PropertyReference propertyReference2 = propertyReference1;
                if (this.FindAncestor(sceneNode1, out ancestor, ref propertyReference2, new Predicate<SceneNode>(SceneNodeObjectSetBase.IsAnimationParent)))
                {
                  sceneNode2 = ancestor;
                  propertyReference1 = propertyReference2;
                }
              }
              using (SceneNode.DisableEnsureTransform(PlatformTypes.TransformGroup.IsAssignableFrom((ITypeId) propertyReference1.LastStep.DeclaringType)))
              {
                if (modification == Modification.InsertValue)
                  sceneNode2.InsertValue(propertyReference1, index, valueToSet);
                else if (modification == Modification.ClearValue)
                {
                  this.ClearAnimations(sceneNode2, propertyReference1);
                  sceneNode2.ClearValue(propertyReference1);
                }
                else if (modification == Modification.RemoveValue)
                {
                  SceneElement element = sceneNode2.GetLocalValueAsSceneNode(propertyReference1).GetChildren()[index] as SceneElement;
                  if (element != null)
                    element.ViewModel.AnimationEditor.DeleteAllAnimationsInSubtree(element);
                  sceneNode2.RemoveValueAt(propertyReference1, index);
                }
                else if (modification == Modification.SetValue)
                {
                  this.ClearAnimations(sceneNode2, propertyReference1);
                  using (LayoutRoundingHelper.IsUseLayoutRoundingProperty(sceneNode2, (IProperty) propertyReference1.LastStep) ? sceneNode2.ViewModel.ForceDefaultSetValue() : (IDisposable) null)
                    sceneNode2.SetValue(propertyReference1, valueToSet);
                }
                LayoutRoundingHelper.ExplicitlyChangeLayoutRounding(sceneNode2, propertyReference1);
              }
            }
          }
          editTransaction.Commit();
        }
        catch (Exception ex)
        {
          if (editTransaction != null)
            editTransaction.Cancel();
          int num = (int) this.DesignerContext.MessageDisplayService.ShowMessage(new MessageBoxArgs()
          {
            Message = StringTable.InvalidPropertyValueErrorNoException,
            Button = MessageBoxButton.OK,
            Image = MessageBoxImage.Hand
          });
        }
      }
      this.TransactionContext.UpdateTransaction();
    }

    private void ClearAnimations(SceneNode node, PropertyReference property)
    {
      node.ViewModel.AnimationEditor.DeleteAllAnimationsInPropertySubtree(node, property);
    }

    private static bool IsAnimationParent(SceneNode sceneNode)
    {
      return sceneNode is SceneElement;
    }

    private static bool IsValidForGetComputedValue(SceneNode node)
    {
      if (!(node is SceneElement) && !(node is StoryboardTimelineSceneNode) && (!(node is KeyFrameSceneNode) && !(node is FromToAnimationSceneNode)) && (!(node is BehaviorBaseNode) && !(node is BehaviorTriggerBaseNode) && (!(node is VisualStateTransitionSceneNode) && !(node is ComparisonConditionNode))) && !PlatformTypes.EasingFunctionBase.IsAssignableFrom((ITypeId) node.Type))
        return ProjectNeutralTypes.TransitionEffect.IsAssignableFrom((ITypeId) node.Type);
      return true;
    }

    private bool FindAncestor(SceneNode child, out SceneNode ancestor, ref PropertyReference propertyReference, Predicate<SceneNode> predicate)
    {
      ancestor = child;
      if (predicate(ancestor))
        return true;
      SceneNode sceneNode = child;
      while (sceneNode != null && !predicate(sceneNode))
        sceneNode = sceneNode.Parent;
      ancestor = sceneNode;
      if (ancestor == null || ancestor.DocumentNode.Marker == null || ancestor.DocumentNode.DocumentRoot != child.DocumentNode.DocumentRoot)
        return false;
      Stack<ReferenceStep> input = DocumentNodeMarkerUtilities.PropertyReferencePath(child.DocumentNode.Marker, ancestor.DocumentNode.Marker);
      propertyReference = new PropertyReference(input).Append(propertyReference);
      return true;
    }

    public override void InvalidateLocalResourcesCache(bool firePropertyChanged)
    {
      if (this.localResources == null)
        return;
      ObservableCollectionWorkaround<LocalResourceModel> collectionWorkaround = this.RecalculateLocalResources(this.localResources);
      if (collectionWorkaround == this.localResources)
        return;
      this.localResources = collectionWorkaround;
      if (!firePropertyChanged)
        return;
      this.OnPropertyChanged("LocalResources");
    }

    protected virtual ObservableCollectionWorkaround<LocalResourceModel> RecalculateLocalResources(ObservableCollectionWorkaround<LocalResourceModel> currentResources)
    {
      return currentResources;
    }

    protected void InvalidateTemplateBindableProperties()
    {
      this.templateBindableProperties = (ObservableCollectionWorkaround<TemplateBindablePropertyModel>) null;
    }

    protected void RecalculateTemplateBindableProperties()
    {
      this.templateBindableProperties.Clear();
      SceneNode[] objects = this.Objects;
      if (objects.Length != 1)
        return;
      SceneNode sceneNode = objects[0];
      if (sceneNode == null || sceneNode.StoryboardContainer == null || !typeof (ControlTemplateElement).IsAssignableFrom(sceneNode.StoryboardContainer.GetType()) || !sceneNode.Platform.Metadata.IsCapabilitySet(PlatformCapability.SupportNonFrameworkElementTemplateBinding) && !PlatformTypes.FrameworkElement.IsAssignableFrom((ITypeId) sceneNode.Type))
        return;
      Type targetElementType = sceneNode.StoryboardContainer.TargetElementType;
      if (!(targetElementType != (Type) null))
        return;
      IList<TargetedReferenceStep> mergedProperties = PropertyMerger.GetMergedProperties((IEnumerable<SceneNode>) new SceneNode[1]
      {
        sceneNode.ViewModel.CreateSceneNode(targetElementType)
      });
      IProjectContext projectContext = sceneNode.ProjectContext;
      foreach (TargetedReferenceStep targetedReferenceStep in (IEnumerable<TargetedReferenceStep>) mergedProperties)
      {
        DependencyPropertyReferenceStep referenceStep = targetedReferenceStep.ReferenceStep as DependencyPropertyReferenceStep;
        if (referenceStep != null)
        {
          bool showReadOnly = true;
          if (PropertyInspectorModel.IsPropertyBrowsable(objects, targetedReferenceStep, showReadOnly) && PropertyInspectorModel.IsAttachedPropertyBrowsable(objects, this.ObjectTypeId, targetedReferenceStep, (ITypeResolver) projectContext))
            this.templateBindableProperties.Add(new TemplateBindablePropertyModel(referenceStep));
        }
      }
      this.templateBindableProperties.Sort((Comparison<TemplateBindablePropertyModel>) ((left, right) => string.Compare(left.PropertyName, right.PropertyName, StringComparison.CurrentCulture)));
      this.OnPropertyChanged("TemplateBindableProperties");
    }

    protected virtual ObservableCollectionWorkaround<LocalResourceModel> ProvideLocalResources(List<ResourceContainer> activeContainers)
    {
      ObservableCollectionWorkaround<LocalResourceModel> collectionWorkaround = new ObservableCollectionWorkaround<LocalResourceModel>();
      foreach (DocumentCompositeNode entryNode in this.DesignerContext.ResourceManager.GetResourcesInElementsScope((IList<ResourceContainer>) activeContainers, PlatformTypes.Object, ResourceResolutionFlags.IncludeApplicationResources | ResourceResolutionFlags.UniqueKeysOnly))
      {
        DocumentNode resourceEntryKey = ResourceNodeHelper.GetResourceEntryKey(entryNode);
        DocumentNode node = entryNode.Properties[DictionaryEntryNode.ValueProperty];
        if (resourceEntryKey != null)
        {
          DelayedEvaluationLocalResourceModel localResourceModel = new DelayedEvaluationLocalResourceModel(this.DocumentContext, this.DesignerContext, resourceEntryKey, node.TargetType, node);
          collectionWorkaround.Add((LocalResourceModel) localResourceModel);
        }
      }
      collectionWorkaround.Sort((Comparison<LocalResourceModel>) ((left, right) => string.Compare(left.ResourceName, right.ResourceName, StringComparison.CurrentCulture)));
      return collectionWorkaround;
    }

    private static void CollectResourceKeyProperties(Type staticType, ObservableCollection<SystemResourceModel> systemResources)
    {
      foreach (PropertyInfo propertyInfo in staticType.GetProperties(BindingFlags.Static | BindingFlags.Public))
      {
        if (typeof (ResourceKey).Equals(propertyInfo.PropertyType))
        {
          Type type = typeof (object);
          string str = string.Empty;
          object obj = null;
          string name = propertyInfo.Name.Substring(0, propertyInfo.Name.Length - 3);
          PropertyInfo property = staticType.GetProperty(name);
          if (property != (PropertyInfo) null)
          {
            type = property.PropertyType;
            obj = property.GetValue(null, (object[]) null);
          }
          if (obj != null)
            systemResources.Add(new SystemResourceModel(staticType.Name, propertyInfo.Name, type, obj));
        }
      }
    }

    private static IType GetStyleTargetTypeId(SceneNode node)
    {
      StyleNode styleNode = node as StyleNode;
      if (styleNode == null)
        return node.Type;
      return styleNode.StyleTargetTypeId;
    }
  }
}
