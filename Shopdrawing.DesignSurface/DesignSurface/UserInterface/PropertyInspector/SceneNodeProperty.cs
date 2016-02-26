// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.SceneNodeProperty
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Markup;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.Tools.Text;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.UserInterface.DataPane;
using Microsoft.Expression.DesignSurface.UserInterface.ResourcePane;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.DesignSurface.ViewModel.Extensibility;
using Microsoft.Expression.DesignSurface.ViewModel.Text;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.PropertyInspector;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Xml;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class SceneNodeProperty : PropertyReferenceProperty
  {
    private static readonly IPropertyId ResourceKeyProperty = (IPropertyId) PlatformTypes.DynamicResource.GetMember(MemberType.LocalProperty, "ResourceKey", MemberAccessTypes.Public);
    private static object invalidCacheValue = new object();
    private DependencyPropertyValueSource valueSource = DependencyPropertyValueSource.LocalValue;
    private const string LocalResourceKey = "LocalResources";
    private const string SystemResourceKey = "SystemResources";
    private object valueCache;
    private ResourceList virtualizingResourceList;
    private uint lastPropertyReferenceChangeStamp;
    private string expressionAsString;
    private string valueException;

    public SceneNodeObjectSet SceneNodeObjectSet
    {
      get
      {
        return (SceneNodeObjectSet) this.ObjectSet;
      }
    }

    public override bool IsReadOnly
    {
      get
      {
        bool isReadOnly = base.IsReadOnly;
        if (!isReadOnly && this.Attributes != null)
        {
          foreach (System.Attribute attribute in this.Attributes)
          {
            ReadOnlyAttribute readOnlyAttribute = attribute as ReadOnlyAttribute;
            if (readOnlyAttribute != null)
              return readOnlyAttribute.IsReadOnly;
          }
        }
        return isReadOnly;
      }
    }

    public override string DisplayName
    {
      get
      {
        if (this.Attributes != null)
        {
          DisplayNameAttribute displayNameAttribute = (DisplayNameAttribute) this.Attributes[typeof (DisplayNameAttribute)];
          if (displayNameAttribute != null && !string.IsNullOrEmpty(displayNameAttribute.DisplayName))
            return displayNameAttribute.DisplayName;
        }
        return base.DisplayName;
      }
    }

    public override EditingContext Context
    {
      get
      {
        return this.SceneNodeObjectSet.ViewModel.ExtensibilityManager.EditingContext;
      }
    }

    public override IEnumerable<ModelProperty> ModelProperties
    {
      get
      {
        if (this.Reference.Count != 1)
          return Enumerable.Empty<ModelProperty>();
        List<ModelProperty> list = new List<ModelProperty>();
        foreach (object obj in this.SceneNodeObjectSet.Objects)
        {
          SceneNode sceneNode = obj as SceneNode;
          if (sceneNode != null)
            list.Add((ModelProperty) new SceneNodeModelProperty(sceneNode.ModelItem, (IProperty) this.Reference[0]));
        }
        return (IEnumerable<ModelProperty>) list;
      }
    }

    public DependencyPropertyValueSource ValueSource
    {
      get
      {
        return this.valueSource;
      }
      set
      {
        if (this.valueSource == value)
          return;
        DependencyPropertyValueSource valueSourceThatChanged = this.valueSource;
        this.valueSource = value;
        this.OnPropertyChanged("ValueSource");
        this.FireOnValueSourceChangedEvent(valueSourceThatChanged);
        this.FireOnValueSourceChangedEvent(value);
        this.OnPropertyChanged("IsEnabledDatabind");
        this.OnPropertyChanged("IsEnabledMakeNewResource");
        this.OnPropertyChanged("IsEnabledClearValue");
        this.OnPropertyChanged("IsEnabledSetLocal");
        this.OnPropertyChanged("IsEnabledEditCustomExpression");
        this.OnPropertyChanged("IsEnabledEditResource");
        this.OnPropertyChanged("IsEnabledLocalResource");
        this.OnPropertyChanged("IsEnabledSystemResource");
        this.OnPropertyChanged("IsEnabledTemplateBinding");
        this.OnPropertyChanged("IsExpression");
        this.OnPropertyChanged("IsResource");
      }
    }

    public bool IsExpression
    {
      get
      {
        return this.valueSource.IsMarkupExtension;
      }
    }

    public virtual bool IsEnabledDatabind
    {
      get
      {
        if (this.SceneNodeObjectSet.IsTextRange || !this.CanSetBindingExpression || this.SceneNodeObjectSet.Count != 1)
          return false;
        return BindingPropertyHelper.IsPropertyBindable(this.SceneNodeObjectSet.Objects[0], this.Reference);
      }
    }

    public bool IsResource
    {
      get
      {
        if (!this.valueSource.IsResource)
          return this.valueSource.IsStatic;
        return true;
      }
    }

    public virtual bool IsEnabledSystemResource
    {
      get
      {
        if (!this.SceneNodeObjectSet.ProjectContext.IsCapabilitySet(PlatformCapability.SupportsSystemResources) || this.IsEditingUriOnMediaElementWithOwningTimeline || !this.CanSetDynamicExpression)
          return false;
        return this.SystemResources.GetEnumerator().MoveNext();
      }
    }

    private bool IsEditingUriOnMediaElementWithOwningTimeline
    {
      get
      {
        if (typeof (Uri).IsAssignableFrom(this.PropertyType) && typeof (MediaElement).IsAssignableFrom(this.SceneNodeObjectSet.ObjectType))
        {
          foreach (MediaSceneElement mediaSceneElement in this.SceneNodeObjectSet.Objects)
          {
            if (mediaSceneElement.OwningTimeline != null)
              return true;
          }
        }
        return false;
      }
    }

    private bool IsEditingKeySplineInSilverlight
    {
      get
      {
        if (this.SceneNodeObjectSet.ProjectContext.IsCapabilitySet(PlatformCapability.DisableKeySplineResources))
          return PlatformTypes.KeySpline.IsAssignableFrom((ITypeId) this.PropertyTypeId);
        return false;
      }
    }

    public virtual bool IsEnabledLocalResource
    {
      get
      {
        if (!this.IsEditingKeySplineInSilverlight && !this.IsEditingUriOnMediaElementWithOwningTimeline && this.CanSetExpression)
          return this.LocalResources.GetEnumerator().MoveNext();
        return false;
      }
    }

    public virtual bool IsEnabledLocalOrSystemResource
    {
      get
      {
        if (!this.IsEnabledLocalResource)
          return this.IsEnabledSystemResource;
        return true;
      }
    }

    public virtual bool IsEnabledTemplateBinding
    {
      get
      {
        if (this.CanSetDynamicExpression && this.TemplateBindableProperties.GetEnumerator().MoveNext() && this.ViewModel != null)
          return this.ViewModel.AnimationEditor.ActiveVisualTrigger == null;
        return false;
      }
    }

    public bool IsEnabledSetLocal
    {
      get
      {
        if (!this.IsValueLocal && this.CanSetLocalValue)
          return !this.IsMixedValue;
        return false;
      }
    }

    public virtual bool IsEnabledClearValue
    {
      get
      {
        if ((this.IsValueInherited || this.IsValueDefaultValue) && (!this.IsMixedValue && !this.SceneNodeObjectSet.IsTextRange))
          return this.IsTextRangeProperty;
        return true;
      }
    }

    public bool IsTextRangeProperty
    {
      get
      {
        BaseFrameworkElement textElement = this.SceneNodeObjectSet.RepresentativeSceneNode as BaseFrameworkElement;
        if (textElement != null && RichTextBoxRangeElement.IsTextProperty((SceneNode) textElement, this.Reference) && TextEditProxyFactory.IsEditableElement((SceneElement) textElement))
          return TextEditProxyFactory.CreateEditProxy(textElement).SupportsRangeProperties;
        return false;
      }
    }

    public virtual bool IsEnabledMakeNewResource
    {
      get
      {
        if (this.IsEditingKeySplineInSilverlight)
          return false;
        try
        {
          this.GetValue();
        }
        catch
        {
          return false;
        }
        IType computedValueTypeId = this.ComputedValueTypeId;
        if (computedValueTypeId == null || !JoltHelper.CanCreateTypeInXaml(this.SceneNodeObjectSet.ProjectContext, computedValueTypeId) || computedValueTypeId.IsExpression)
          return false;
        if (this.IsValueLocalResource || this.IsValueSystemResource)
          return true;
        if (this.SceneNodeObjectSet.ObjectType != (Type) null && PlatformTypes.Window.IsAssignableFrom((ITypeId) this.SceneNodeObjectSet.ObjectTypeId) && PlatformTypes.Style.IsAssignableFrom((ITypeId) this.PropertyTypeId) || (PlatformTypes.Model3DCollection.IsAssignableFrom((ITypeId) this.PropertyTypeId) || PlatformTypes.Model3D.IsAssignableFrom((ITypeId) this.PropertyTypeId) || (PlatformTypes.ResourceDictionary.IsAssignableFrom((ITypeId) this.PropertyTypeId) || this.IsEditingUriOnMediaElementWithOwningTimeline)) || (this.SceneNodeObjectSet.IsTextRange || this.IsValueTemplateBinding || (this.IsValueDataBound || this.IsMixedValue)))
          return false;
        return this.CanSetExpression;
      }
    }

    public virtual bool IsEnabledEditResource
    {
      get
      {
        if (!this.IsValueLocalResource)
          return false;
        ITypeId type = (ITypeId) this.ComputedValueTypeId;
        return type != null && !PlatformTypes.Transform.IsAssignableFrom(type) && !PlatformTypes.Transform3D.IsAssignableFrom(type);
      }
    }

    public bool IsEnabledEditCustomExpression
    {
      get
      {
        return this.CanSetExpression;
      }
    }

    public bool IsEnabledRecordCurrentValue
    {
      get
      {
        bool flag = false;
        if (this.SceneNodeObjectSet.ShouldAllowAnimation && this.ViewModel.AnimationEditor.IsKeyFraming)
        {
          SceneNode representativeSceneNode = this.SceneNodeObjectSet.RepresentativeSceneNode;
          if (representativeSceneNode != null)
            flag = this.ViewModel.AnimationEditor.GetAnimationProperty(representativeSceneNode, this.Reference) != null;
        }
        return flag;
      }
    }

    protected virtual bool CanSetBindingExpression
    {
      get
      {
        return this.SceneNodeObjectSet.CanSetBindingExpression;
      }
    }

    protected virtual bool CanSetDynamicExpression
    {
      get
      {
        bool flag;
        if (this.Reference.Count < 2)
        {
          flag = this.SceneNodeObjectSet.CanSetDynamicExpression;
        }
        else
        {
          PropertyReferenceProperty referenceProperty = new PropertyReferenceProperty(this.ObjectSet, this.Reference.Subreference(0, this.Reference.Count - 2), (AttributeCollection) null);
          Type runtimeType1 = this.SceneNodeObjectSet.ProjectContext.ResolveType(PlatformTypes.DependencyObject).RuntimeType;
          Type runtimeType2 = this.SceneNodeObjectSet.ProjectContext.ResolveType(PlatformTypes.Style).RuntimeType;
          object obj = referenceProperty.GetValue();
          Type c = obj == null || obj == MixedProperty.Mixed ? PlatformTypeHelper.GetPropertyType((IProperty) this.Reference[this.Reference.Count - 2]) : obj.GetType();
          flag = typeof (CanonicalTransform).IsAssignableFrom(c) || typeof (CanonicalTransform3D).IsAssignableFrom(c) || runtimeType1 != (Type) null && runtimeType1.IsAssignableFrom(c) || runtimeType2 != (Type) null && runtimeType2.IsAssignableFrom(c);
        }
        if (!flag || !this.CanSetExpression || !(this.Reference.LastStep is DependencyPropertyReferenceStep))
          return false;
        if (this.SceneNodeObjectSet.ShouldAllowAnimation && this.IsEnabledRecordCurrentValue)
          return !this.SceneNodeObjectSet.ViewModel.AnimationEditor.IsRecording;
        return true;
      }
    }

    private bool CanSetExpression
    {
      get
      {
        if (!this.IsRuntimeNameProperty && !this.SceneNodeObjectSet.IsTextRange)
          return !this.IsReadOnly;
        return false;
      }
    }

    private bool CanSetLocalValue
    {
      get
      {
        if (this.IsReadOnly)
          return false;
        if (this.SceneNodeObjectSet.IsTextRange)
          return false;
        object obj;
        try
        {
          obj = this.GetValue();
        }
        catch
        {
          return false;
        }
        if (obj is XmlNode || obj is ObservableCollection<XmlNode>)
          return false;
        if (obj == null)
          return true;
        IType type = this.SceneNodeObjectSet.ProjectContext.GetType(obj.GetType());
        return !PlatformTypes.FrameworkElement.IsAssignableFrom((ITypeId) type);
      }
    }

    private SceneViewModel ViewModel
    {
      get
      {
        return this.SceneNodeObjectSet.ViewModel;
      }
    }

    private bool IsRuntimeNameProperty
    {
      get
      {
        if (!(this.SceneNodeObjectSet.ObjectType != (Type) null))
          return false;
        IPropertyId propertyId = (IPropertyId) this.SceneNodeObjectSet.ProjectContext.MetadataFactory.GetMetadata(this.SceneNodeObjectSet.ObjectType).NameProperty;
        if (propertyId != null)
          return propertyId.Name == this.PropertyName;
        return false;
      }
    }

    private bool Inherits
    {
      get
      {
        DependencyPropertyReferenceStep propertyReferenceStep = this.Reference.LastStep as DependencyPropertyReferenceStep;
        if (propertyReferenceStep != null)
        {
          Type objectType = this.SceneNodeObjectSet.ObjectType;
          if (objectType != (Type) null)
            return propertyReferenceStep.Inherits(objectType);
        }
        return false;
      }
    }

    public bool IsValueDataBound
    {
      get
      {
        return this.valueSource.IsBinding;
      }
    }

    public bool IsValueSystemResource
    {
      get
      {
        return this.valueSource.IsStatic;
      }
    }

    public bool IsValueLocalResource
    {
      get
      {
        return this.valueSource.IsResource;
      }
    }

    public bool IsValueTemplateBinding
    {
      get
      {
        return this.valueSource.IsTemplateBinding;
      }
    }

    public bool IsValueCustomMarkupExtension
    {
      get
      {
        return this.valueSource.IsCustomMarkupExtension;
      }
    }

    public bool IsValueLocal
    {
      get
      {
        return this.valueSource.IsLocalValue;
      }
    }

    public bool IsValueDefaultValue
    {
      get
      {
        return this.valueSource.IsDefaultValue;
      }
    }

    public bool IsValueInherited
    {
      get
      {
        return this.valueSource.IsInheritedValue;
      }
    }

    public object ToolTip
    {
      get
      {
        string result;
        PlatformNeutralAttributeHelper.TryGetAttributeValue<string>((IEnumerable) this.Attributes, PlatformTypes.DescriptionAttribute, "Description", out result);
        return (object) new DocumentationEntry(this.Reference.TargetType.Name, this.DisplayName, PlatformTypeHelper.GetPropertyType((IProperty) this.Reference.LastStep), result);
      }
    }

    public override bool IsDefaultValue
    {
      get
      {
        return this.valueSource.IsDefaultValue;
      }
    }

    public override string CategoryName
    {
      get
      {
        string result;
        if (PlatformNeutralAttributeHelper.TryGetAttributeValue<string>((IEnumerable) this.Attributes, PlatformTypes.CategoryAttribute, "Category", out result) && !string.IsNullOrEmpty(result))
          return result;
        return CategoryAttribute.Default.Category;
      }
    }

    public override bool IsAdvanced
    {
      get
      {
        object result;
        if (PlatformNeutralAttributeHelper.TryGetAttributeValue<object>((IEnumerable) this.Attributes, PlatformTypes.EditorBrowsableAttribute, "State", out result))
          return (int) result == 2;
        return false;
      }
    }

    public ReadOnlyObservableCollection<ResourceVirtualizingTreeItem> VirtualizingResources
    {
      get
      {
        if (this.virtualizingResourceList == null)
          this.virtualizingResourceList = new ResourceList();
        if (this.IsEnabledLocalResource)
        {
          this.virtualizingResourceList.EnsureCategoryExists("LocalResources", string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.LocalResourceHeader, new object[1]
          {
            (object) this.PropertyType.Name
          }));
          this.virtualizingResourceList.UpdateCategory<LocalResourceModel>("LocalResources", this.LocalResources);
        }
        if (this.IsEnabledSystemResource)
        {
          this.virtualizingResourceList.EnsureCategoryExists("SystemResources", string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.SystemResourceHeader, new object[1]
          {
            (object) this.PropertyType.Name
          }));
          this.virtualizingResourceList.UpdateCategory<SystemResourceModel>("SystemResources", this.SystemResources);
        }
        return this.virtualizingResourceList.ItemList;
      }
    }

    public IEnumerable<SelectablePropertyModel<LocalResourceModel>> LocalResources
    {
      get
      {
        if (this.SceneNodeObjectSet.ProjectContext != null)
        {
          ExpressionEvaluator eval = new ExpressionEvaluator((IDocumentRootResolver) this.SceneNodeObjectSet.ProjectContext);
          DocumentNode selectedResource = (DocumentNode) null;
          if (this.ValueSource.IsResource)
          {
            bool isMixed;
            DocumentCompositeNode node = this.GetLocalValueAsDocumentNode(false, out isMixed) as DocumentCompositeNode;
            this.SceneNodeObjectSet.InvalidateLocalResourcesCache(false);
            if (node != null)
            {
              DocumentNode resourceKey = ResourceNodeHelper.GetResourceKey(node);
              if (resourceKey != null && resourceKey.DocumentRoot != null && resourceKey.DocumentRoot.RootNode != null)
              {
                ResourceReferenceType resourceType = ResourceNodeHelper.GetResourceType((DocumentNode) node);
                selectedResource = eval.EvaluateResource(new DocumentNodePath(resourceKey.DocumentRoot.RootNode, resourceKey), resourceType, resourceKey);
              }
            }
          }
          IEnumerable<LocalResourceModel> unfilteredList = (IEnumerable<LocalResourceModel>) this.SceneNodeObjectSet.LocalResources;
          foreach (LocalResourceModel propertyModel in unfilteredList)
          {
            if (this.ProxyPropertyType.IsAssignableFrom(propertyModel.ResourceType))
            {
              if (PlatformTypes.Style.IsAssignableFrom((ITypeId) this.PropertyTypeId) || PlatformTypes.FrameworkTemplate.IsAssignableFrom((ITypeId) this.PropertyTypeId))
              {
                IType objectTypeId = this.SceneNodeObjectSet.ObjectTypeId;
                if (objectTypeId != null)
                {
                  IType styleOrTemplateType;
                  IType type = this.SceneNodeObjectSet.ProjectContext.ResolveType(DocumentNodeUtilities.GetStyleOrTemplateTypeAndTargetType(propertyModel.ResourceNode, out styleOrTemplateType));
                  IPropertyId propertyKey = (IPropertyId) this.Reference.LastStep;
                  Type propertyTargetType = objectTypeId.Metadata.GetStylePropertyTargetType(propertyKey);
                  if (propertyTargetType != (Type) null && (type == null || type.RuntimeType == (Type) null || !type.RuntimeType.IsAssignableFrom(propertyTargetType)))
                    continue;
                }
                else
                  continue;
              }
              bool isSelected = selectedResource != null && selectedResource.DocumentRoot == propertyModel.ResourceNode.DocumentRoot && propertyModel.ResourceNode.Marker.CompareTo((object) selectedResource.Marker) == 0;
              yield return new SelectablePropertyModel<LocalResourceModel>(propertyModel, isSelected);
            }
          }
        }
      }
    }

    internal VirtualizingResourceItem<LocalResourceModel> SelectedLocalResourceModel
    {
      get
      {
        if (this.IsValueLocalResource)
        {
          foreach (SelectablePropertyModel<LocalResourceModel> resource in this.LocalResources)
          {
            if (resource.IsSelected)
              return this.virtualizingResourceList.FindItemInCategory<LocalResourceModel>("LocalResources", resource);
          }
        }
        return (VirtualizingResourceItem<LocalResourceModel>) null;
      }
    }

    internal VirtualizingResourceItem<SystemResourceModel> SelectedSystemResourceModel
    {
      get
      {
        if (this.IsValueSystemResource)
        {
          foreach (SelectablePropertyModel<SystemResourceModel> resource in this.SystemResources)
          {
            if (resource.IsSelected)
              return this.virtualizingResourceList.FindItemInCategory<SystemResourceModel>("SystemResources", resource);
          }
        }
        return (VirtualizingResourceItem<SystemResourceModel>) null;
      }
    }

    public IEnumerable<SelectablePropertyModel<SystemResourceModel>> SystemResources
    {
      get
      {
        IEnumerable<SystemResourceModel> unfilteredList = (IEnumerable<SystemResourceModel>) this.SceneNodeObjectSet.SystemResources;
        string selectedResourceName = (string) null;
        if (this.ValueSource.IsStatic)
        {
          bool isMixed;
          DocumentCompositeNode documentCompositeNode1 = this.GetLocalValueAsDocumentNode(false, out isMixed) as DocumentCompositeNode;
          if (documentCompositeNode1 != null)
          {
            DocumentCompositeNode documentCompositeNode2 = documentCompositeNode1.Properties[SceneNodeProperty.ResourceKeyProperty] as DocumentCompositeNode;
            if (documentCompositeNode2 != null)
            {
              DocumentPrimitiveNode documentPrimitiveNode = documentCompositeNode2.Properties[StaticExtensionProperties.MemberProperty] as DocumentPrimitiveNode;
              if (documentPrimitiveNode != null)
              {
                ReferenceStep referenceStep = DocumentPrimitiveNode.GetValueAsMember((DocumentNode) documentPrimitiveNode) as ReferenceStep;
                if (referenceStep != null)
                  selectedResourceName = referenceStep.UniqueName;
              }
            }
          }
        }
        foreach (SystemResourceModel propertyModel in unfilteredList)
        {
          if (this.ProxyPropertyType.IsAssignableFrom(propertyModel.ResourceType))
            yield return new SelectablePropertyModel<SystemResourceModel>(propertyModel, selectedResourceName == propertyModel.ResourceName);
        }
      }
    }

    public IEnumerable<SelectablePropertyModel<TemplateBindablePropertyModel>> TemplateBindableProperties
    {
      get
      {
        IEnumerable<TemplateBindablePropertyModel> unfilteredList = (IEnumerable<TemplateBindablePropertyModel>) this.SceneNodeObjectSet.TemplateBindableProperties;
        DependencyPropertyReferenceStep referenceStep = (DependencyPropertyReferenceStep) null;
        if (this.ValueSource.IsTemplateBinding)
        {
          bool isMixed;
          DocumentCompositeNode documentCompositeNode = this.GetLocalValueAsDocumentNode(false, out isMixed) as DocumentCompositeNode;
          if (documentCompositeNode != null)
          {
            DocumentPrimitiveNode documentPrimitiveNode = documentCompositeNode.Properties[TemplateBindablePropertyModel.PropertyProperty] as DocumentPrimitiveNode;
            if (documentPrimitiveNode != null)
              referenceStep = DocumentPrimitiveNode.GetValueAsMember((DocumentNode) documentPrimitiveNode) as DependencyPropertyReferenceStep;
          }
        }
        Type propertyType = this.PropertyType;
        if (typeof (string).Equals(propertyType))
          propertyType = typeof (object);
        foreach (TemplateBindablePropertyModel propertyModel in unfilteredList)
        {
          if (propertyType.IsAssignableFrom(propertyModel.PropertyType))
            yield return new SelectablePropertyModel<TemplateBindablePropertyModel>(propertyModel, propertyModel.ReferenceStep == referenceStep);
        }
      }
    }

    public string ExpressionAsString
    {
      get
      {
        return this.expressionAsString;
      }
    }

    public override bool ShouldClearValueWhenSettingNull
    {
      get
      {
        if (base.ShouldClearValueWhenSettingNull)
          return !this.IsEnabledRecordCurrentValue;
        return false;
      }
    }

    public SceneNodeProperty(SceneNodeObjectSet objectSet, PropertyReference propertyReference, AttributeCollection attributes)
      : this(objectSet, propertyReference, attributes, (Microsoft.Windows.Design.PropertyEditing.PropertyValue) null, (Type) null)
    {
    }

    public SceneNodeProperty(SceneNodeObjectSet objectSet, PropertyReference propertyReference, AttributeCollection attributes, Microsoft.Windows.Design.PropertyEditing.PropertyValue parentValue)
      : this(objectSet, propertyReference, attributes, parentValue, (Type) null)
    {
    }

    public SceneNodeProperty(SceneNodeObjectSet objectSet, PropertyReference propertyReference, AttributeCollection attributes, Microsoft.Windows.Design.PropertyEditing.PropertyValue parentValue, Type proxyType)
      : base((ObjectSetBase) objectSet, propertyReference, attributes, parentValue, proxyType)
    {
      this.SceneNodeObjectSet.PropertyChanged += new PropertyChangedEventHandler(this.SceneNodeProperty_PropertyTargetPropertyChanged);
      this.SceneNodeObjectSet.RegisterPropertyChangedHandler(this.Reference, new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.SceneNodeProperty_PropertyReferenceChanged));
    }

    private void SceneNodeProperty_PropertyTargetPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "LocalResources")
      {
        this.OnPropertyChanged("LocalResources");
        this.OnPropertyChanged("VirtualizingResources");
        this.OnPropertyChanged("IsEnabledLocalResource");
        this.OnPropertyChanged("IsEnabledLocalOrSystemResource");
      }
      else
      {
        if (!(e.PropertyName == "TemplateBindableProperties"))
          return;
        this.OnPropertyChanged("TemplateBindableProperties");
        this.OnPropertyChanged("IsEnabledTemplateBinding");
      }
    }

    private void SceneNodeProperty_PropertyReferenceChanged(object sender, PropertyReferenceChangedEventArgs e)
    {
      if (this.ViewModel == null)
        return;
      if (this.SceneNodeObjectSet.IsValidForUpdate)
      {
        if (this.SceneNodeObjectSet.PropertyUpdatesLocked || this.SceneNodeObjectSet.IsSelectionInvalid || (this.SceneNodeObjectSet.ProjectContext == null || this.SceneNodeObjectSet.ProjectContext.PlatformMetadata != this.Reference.PlatformMetadata))
          return;
        if ((int) this.lastPropertyReferenceChangeStamp != (int) e.ChangeStamp)
        {
          this.Recache();
          this.lastPropertyReferenceChangeStamp = e.ChangeStamp;
        }
        this.OnPropertyReferenceChanged(e);
      }
      else
        this.OnRemoveFromCategory();
    }

    private bool IsValueLegal(object value, out string errorMessage)
    {
      errorMessage = string.Empty;
      bool flag = true;
      if (string.IsNullOrEmpty(value as string) && this.IsRuntimeNameProperty)
      {
        if (this.SceneNodeObjectSet.IsMultiSelection)
        {
          errorMessage = StringTable.CannotSetSameValueForNameErrorMessage;
          return false;
        }
        if (this.SceneNodeObjectSet.IsTargetedByAnimation)
        {
          errorMessage = StringTable.CannotSetTargetedNameToBlank;
          return false;
        }
      }
      if (DictionaryEntryNode.ValueProperty.Equals((object) this.Reference.LastStep) && value == null)
      {
        errorMessage = StringTable.InvalidResourceValueMessage;
        return false;
      }
      DocumentNode documentNode = value as DocumentNode;
      if (documentNode != null && DocumentNodeUtilities.IsMarkupExtension(documentNode))
      {
        if (!this.CanSetExpression)
        {
          errorMessage = StringTable.ExpressionTargetErrorMessage;
          flag = false;
        }
        else if (DocumentNodeUtilities.IsBinding(documentNode))
          flag = this.IsBindingLegal((DocumentCompositeNode) documentNode, out errorMessage);
        else if (DocumentNodeUtilities.IsTemplateBinding(documentNode))
          flag = this.IsAliasLegal(documentNode, out errorMessage);
        else if (DocumentNodeUtilities.IsDynamicResource(documentNode))
          flag = this.IsDynamicResourceLegal((DocumentCompositeNode) documentNode, out errorMessage);
        else if (DocumentNodeUtilities.IsStaticResource(documentNode))
          flag = this.IsStaticResourceLegal((DocumentCompositeNode) documentNode, out errorMessage);
      }
      return flag;
    }

    private bool IsBindingLegal(DocumentCompositeNode binding, out string errorMessage)
    {
      if (!this.IsEnabledDatabind)
      {
        errorMessage = StringTable.DatabindingTargetErrorMessage;
        return false;
      }
      BindingSceneNode bindingSceneNode = this.ViewModel.GetSceneNode((DocumentNode) binding) as BindingSceneNode;
      if (bindingSceneNode != null)
      {
        SceneNode targetElement = this.SceneNodeObjectSet.Objects[0];
        return bindingSceneNode.IsBindingLegal(targetElement, this.Reference.FirstStep, out errorMessage);
      }
      errorMessage = (string) null;
      return true;
    }

    private bool IsAliasLegal(DocumentNode templateBinding, out string errorMessage)
    {
      errorMessage = string.Empty;
      return true;
    }

    private bool IsDynamicResourceLegal(DocumentCompositeNode resource, out string errorMessage)
    {
      if (resource.Properties[SceneNodeProperty.ResourceKeyProperty] == null)
      {
        errorMessage = StringTable.ResourceExpressionCannotHaveEmptyKey;
        return false;
      }
      errorMessage = string.Empty;
      return true;
    }

    private bool IsStaticResourceLegal(DocumentCompositeNode resource, out string errorMessage)
    {
      errorMessage = string.Empty;
      return true;
    }

    private void FireOnValueSourceChangedEvent(DependencyPropertyValueSource valueSourceThatChanged)
    {
      if (valueSourceThatChanged.IsBinding)
        this.OnPropertyChanged("IsValueDataBound");
      else if (valueSourceThatChanged.IsStatic)
      {
        this.OnPropertyChanged("SystemResources");
        this.OnPropertyChanged("VirtualizingResources");
        this.OnPropertyChanged("IsValueSystemResource");
      }
      else if (valueSourceThatChanged.IsResource)
      {
        this.OnPropertyChanged("LocalResources");
        this.OnPropertyChanged("VirtualizingResources");
        this.OnPropertyChanged("IsValueLocalResource");
      }
      else if (valueSourceThatChanged.IsTemplateBinding)
        this.OnPropertyChanged("IsValueTemplateBinding");
      else if (valueSourceThatChanged.IsCustomMarkupExtension)
        this.OnPropertyChanged("IsValueCustomMarkupExtension");
      else if (valueSourceThatChanged.IsLocalValue)
        this.OnPropertyChanged("IsValueLocal");
      else if (valueSourceThatChanged.IsDefaultValue)
      {
        this.OnPropertyChanged("IsValueDefaultValue");
      }
      else
      {
        if (!valueSourceThatChanged.IsInheritedValue)
          return;
        this.OnPropertyChanged("IsValueInherited");
      }
    }

    private bool IsSystemResource(DocumentCompositeNode keyNode)
    {
      DocumentPrimitiveNode documentPrimitiveNode = keyNode.Properties[StaticExtensionProperties.MemberProperty] as DocumentPrimitiveNode;
      if (documentPrimitiveNode != null)
      {
        IProperty property = DocumentPrimitiveNode.GetValueAsMember((DocumentNode) documentPrimitiveNode) as IProperty;
        if (property != null)
        {
          Type declaringType = PlatformTypeHelper.GetDeclaringType((IMember) property);
          if (typeof (SystemColors).IsAssignableFrom(declaringType) || typeof (SystemFonts).IsAssignableFrom(declaringType) || typeof (SystemParameters).IsAssignableFrom(declaringType))
            return true;
        }
      }
      return false;
    }

    private void ExplicitRecache()
    {
      this.Recache();
      this.lastPropertyReferenceChangeStamp = this.ViewModel.ChangeStamp;
      this.OnPropertyReferenceChanged(new PropertyReferenceChangedEventArgs(SceneViewModel.ViewStateBits.CurrentValues, this.Reference));
    }

    public override void Recache()
    {
      this.valueCache = SceneNodeProperty.invalidCacheValue;
      base.Recache();
      bool isMixed;
      DocumentNode valueAsDocumentNode = this.SceneNodeObjectSet.GetLocalValueAsDocumentNode(this, GetLocalValueFlags.SkipCheckIfMixed | GetLocalValueFlags.CheckKeyframes, out isMixed);
      if (valueAsDocumentNode == null)
      {
        if (this.Inherits)
          this.ValueSource = DependencyPropertyValueSource.InheritedValue;
        else
          this.ValueSource = DependencyPropertyValueSource.DefaultValue;
      }
      else if (DocumentNodeUtilities.IsBinding(valueAsDocumentNode))
        this.ValueSource = DependencyPropertyValueSource.Binding;
      else if (DocumentNodeUtilities.IsTemplateBinding(valueAsDocumentNode))
        this.ValueSource = DependencyPropertyValueSource.TemplateBinding;
      else if (DocumentNodeUtilities.IsStaticExtension(valueAsDocumentNode))
        this.ValueSource = DependencyPropertyValueSource.LocalValue;
      else if (DocumentNodeUtilities.IsDynamicResource(valueAsDocumentNode) || DocumentNodeUtilities.IsStaticResource(valueAsDocumentNode))
      {
        DocumentCompositeNode keyNode = ResourceNodeHelper.GetResourceKey((DocumentCompositeNode) valueAsDocumentNode) as DocumentCompositeNode;
        if (keyNode != null && DocumentNodeUtilities.IsStaticExtension((DocumentNode) keyNode) && this.IsSystemResource(keyNode))
        {
          this.ValueSource = DependencyPropertyValueSource.Static;
          this.OnPropertyChanged("SelectedSystemResourceModel");
        }
        else
        {
          this.ValueSource = DocumentNodeUtilities.IsDynamicResource(valueAsDocumentNode) ? DependencyPropertyValueSource.DynamicResource : DependencyPropertyValueSource.StaticResource;
          this.OnPropertyChanged("SelectedLocalResourceModel");
        }
        this.OnPropertyChanged("VirtualizingResources");
      }
      else if (DocumentNodeUtilities.IsMarkupExtension(valueAsDocumentNode))
        this.ValueSource = DependencyPropertyValueSource.CustomMarkupExtension;
      else
        this.ValueSource = DependencyPropertyValueSource.LocalValue;
    }

    protected override void ModifyValue(object valueToSet, PropertyReferenceProperty.Modification modification, int index)
    {
      string errorMessage;
      if ((modification == PropertyReferenceProperty.Modification.SetValue || modification == PropertyReferenceProperty.Modification.ClearValue) && !this.IsValueLegal(valueToSet, out errorMessage))
      {
        this.ShowMessage(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.InvalidPropertyValueErrorMessage, new object[1]
        {
          (object) errorMessage
        }), MessageBoxButton.OK, MessageBoxImage.Hand);
      }
      else
      {
        base.ModifyValue(valueToSet, modification, index);
        if (valueToSet is DocumentNode || modification != PropertyReferenceProperty.Modification.SetValue)
          this.valueCache = SceneNodeProperty.invalidCacheValue;
        else
          this.valueCache = valueToSet;
      }
    }

    public DocumentNode GetLocalValueAsDocumentNode(bool resolved, out bool isMixed)
    {
      return this.SceneNodeObjectSet.GetLocalValueAsDocumentNode(this, (GetLocalValueFlags) ((resolved ? true : false) | 4), out isMixed);
    }

    public override object GetValue()
    {
      if (this.valueCache == SceneNodeProperty.invalidCacheValue)
      {
        if (!this.Associated)
          return (object) null;
        this.valueCache = base.GetValue();
        if (this.valueCache is double)
          this.valueCache = (object) JoltHelper.RoundDouble(this.SceneNodeObjectSet.ProjectContext, (double) this.valueCache);
        else if (this.valueCache is GridLength)
          this.valueCache = (object) JoltHelper.RoundGridLength(this.SceneNodeObjectSet.ProjectContext, (GridLength) this.valueCache);
      }
      return this.valueCache;
    }

    public override void OnRemoveFromCategory()
    {
      if (this.Reference != null)
      {
        this.SceneNodeObjectSet.UnregisterPropertyChangedHandler(this.Reference, new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.SceneNodeProperty_PropertyReferenceChanged));
        this.SceneNodeObjectSet.PropertyChanged -= new PropertyChangedEventHandler(this.SceneNodeProperty_PropertyTargetPropertyChanged);
        ((PropertyReferencePropertyValue) this.PropertyValue).Unhook();
      }
      base.OnRemoveFromCategory();
    }

    private void ShowError(string message)
    {
      this.SceneNodeObjectSet.DesignerContext.MessageDisplayService.ShowError(message);
    }

    private void ShowMessage(string message, MessageBoxButton button, MessageBoxImage image)
    {
      int num = (int) this.SceneNodeObjectSet.DesignerContext.MessageDisplayService.ShowMessage(new MessageBoxArgs()
      {
        Message = message,
        Button = button,
        Image = image
      });
    }

    public void DoClearValue()
    {
      this.ClearValue();
      this.ExplicitRecache();
    }

    public void DoSetLocalValue()
    {
      bool isMixed;
      DocumentNode valueAsDocumentNode = this.GetLocalValueAsDocumentNode(true, out isMixed);
      if (isMixed)
        return;
      DocumentNode documentNode = valueAsDocumentNode == null || valueAsDocumentNode.Type.IsExpression ? this.SceneNodeObjectSet.GetRawValue(this.ViewModel.Document.DocumentContext, this.Reference, PropertyReference.GetValueFlags.Computed) : valueAsDocumentNode.Clone(this.ViewModel.Document.DocumentContext);
      using (documentNode != null && this.SceneNodeObjectSet.ShouldAllowAnimation ? (IDisposable) null : this.ViewModel.AnimationEditor.DeferKeyFraming())
      {
        if (documentNode == null)
          this.ClearValue();
        else
          this.SetValue((object) documentNode);
      }
      this.ExplicitRecache();
    }

    public void DoSetDataBinding()
    {
      this.SceneNodeObjectSet.CreateAndSetBindingOrData(this);
      this.ExplicitRecache();
    }

    public void DoSetToLocalResource(LocalResourceModel localResource)
    {
      this.SceneNodeObjectSet.SetValueToLocalResource(this, localResource);
      this.ExplicitRecache();
    }

    public void DoSetToSystemResource(SystemResourceModel systemResource)
    {
      this.SceneNodeObjectSet.SetValueToSystemResource(this, systemResource);
      this.ExplicitRecache();
    }

    public void DoSetToTemplateBinding(TemplateBindablePropertyModel templateBindProperty)
    {
      this.SceneNodeObjectSet.SetValueToTemplateBinding(this, (ReferenceStep) templateBindProperty.ReferenceStep);
      this.ExplicitRecache();
    }

    public bool SetExpressionAsString(string text)
    {
      if (text.Length > 0 && (int) text[0] == 123)
      {
        if (text.StartsWith("{}", StringComparison.Ordinal))
        {
          text = text.Substring(2);
        }
        else
        {
          DocumentNode representativeNode = this.SceneNodeObjectSet.RepresentativeNode;
          IList<XamlParseError> errors;
          DocumentNode expressionFromString = XamlExpressionSerializer.GetExpressionFromString(text, representativeNode, this.PropertyType, out errors);
          if (errors != null && errors.Count > 0)
          {
            this.ShowError(errors[0].FullMessage);
            return false;
          }
          this.SetValue((object) expressionFromString);
          return true;
        }
      }
      this.valueException = (string) null;
      this.PropertyValue.PropertyValueException += new EventHandler<PropertyValueExceptionEventArgs>(this.OnPropertyValueException);
      this.PropertyValue.StringValue = text;
      this.PropertyValue.PropertyValueException -= new EventHandler<PropertyValueExceptionEventArgs>(this.OnPropertyValueException);
      if (this.valueException == null)
        return true;
      this.ShowError(this.valueException);
      return false;
    }

    public void DoEditCustomExpression(FrameworkElement target)
    {
      this.expressionAsString = (string) null;
      if (this.IsExpression)
      {
        bool isMixed;
        DocumentNode valueAsDocumentNode = this.GetLocalValueAsDocumentNode(false, out isMixed);
        if (!isMixed)
          this.expressionAsString = this.ConvertExpressionToString((object) valueAsDocumentNode);
      }
      if (this.expressionAsString == null)
      {
        object expressionObject = (object) null;
        try
        {
          expressionObject = this.GetValue();
        }
        catch
        {
        }
        if (expressionObject != MixedProperty.Mixed)
          this.expressionAsString = this.ConvertExpressionToString(expressionObject);
      }
      CustomExpressionEditor expressionEditor = new CustomExpressionEditor(this);
      expressionEditor.PlacementTarget = (UIElement) target;
      expressionEditor.IsOpen = true;
    }

    private void OnPropertyValueException(object sender, PropertyValueExceptionEventArgs e)
    {
      this.valueException = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.InvalidValueErrorMessage, new object[1]
      {
        (object) e.Exception.Message
      });
    }

    private string ConvertExpressionToString(object expressionObject)
    {
      string str1 = "";
      DocumentNode expression;
      if ((expression = expressionObject as DocumentNode) != null)
      {
        DocumentNode representativeNode = this.SceneNodeObjectSet.RepresentativeNode;
        str1 = XamlExpressionSerializer.GetStringFromExpression(expression, representativeNode);
      }
      else
      {
        string str2 = expressionObject as string;
        if (str2 != null)
        {
          if (str2.StartsWith("{", StringComparison.Ordinal))
            str2 = "{}" + str2;
          return str2;
        }
        try
        {
          str1 = this.PropertyValue.StringValue;
        }
        catch
        {
        }
      }
      return str1;
    }

    public void DoEditResource()
    {
      DesignerContext designerContext = this.SceneNodeObjectSet.DesignerContext;
      IViewService viewService = designerContext.ViewService;
      ResourceManager resourceManager = designerContext.ResourceManager;
      bool isMixed;
      DocumentCompositeNode findMe = (DocumentCompositeNode) this.GetLocalValueAsDocumentNode(false, out isMixed);
      DocumentCompositeNode documentCompositeNode = (DocumentCompositeNode) null;
      if (findMe != null)
        documentCompositeNode = Microsoft.Expression.DesignSurface.Utility.ResourceHelper.LookupResource(this.ViewModel, findMe);
      if (documentCompositeNode == null)
      {
        this.ShowError(StringTable.PropertyCannotFindResource);
      }
      else
      {
        SceneViewModel sceneViewModel = documentCompositeNode.DocumentRoot != findMe.DocumentRoot ? ((ISceneViewHost) this.ViewModel.ProjectContext.GetService(typeof (ISceneViewHost))).OpenView(documentCompositeNode.DocumentRoot, false).ViewModel : this.ViewModel;
        DictionaryEntryNode resourceEntryNode = (DictionaryEntryNode) sceneViewModel.GetSceneNode((DocumentNode) documentCompositeNode);
        SceneNode sceneNode = resourceEntryNode.Value;
        if (PlatformTypes.Style.IsAssignableFrom((ITypeId) sceneNode.Type) || PlatformTypes.FrameworkTemplate.IsAssignableFrom((ITypeId) sceneNode.Type))
        {
          bool flag = sceneViewModel != this.ViewModel;
          SceneViewModel viewModel = this.ViewModel;
          if (flag)
            viewService.ActiveView = (IView) sceneViewModel.DefaultView;
          sceneViewModel.SetViewRoot(viewModel.DefaultView, (SceneElement) null, (IPropertyId) null, sceneNode.DocumentNode, Size.Empty);
          SceneElement selectionToSet = sceneNode as SceneElement;
          if (selectionToSet == null)
            return;
          sceneViewModel.ElementSelectionSet.SetSelection(selectionToSet);
        }
        else
        {
          using (SceneEditTransaction editTransaction = sceneViewModel.CreateEditTransaction(StringTable.UndoUnitEditResource))
          {
            EditResourceDialog editResourceDialog = new EditResourceDialog(this.ViewModel.DesignerContext);
            using (EditResourceModel editResourceModel = new EditResourceModel(this.ViewModel.DesignerContext, resourceEntryNode, (IPropertyInspector) editResourceDialog))
            {
              editResourceDialog.Model = editResourceModel;
              bool? nullable = editResourceDialog.ShowDialog();
              if ((nullable.GetValueOrDefault() ? 0 : (nullable.HasValue ? true : false)) != 0)
              {
                editTransaction.Cancel();
                return;
              }
              editTransaction.Commit();
            }
          }
          this.ExplicitRecache();
          this.SceneNodeObjectSet.InvalidateLocalResourcesCache();
        }
      }
    }

    private bool MakeNewResource(DocumentNode resourceNode, Type resourceType, Type targetType, IPropertyId targetTypeProperty)
    {
      SceneViewModel viewModel = this.ViewModel;
      SceneNode representativeSceneNode = this.SceneNodeObjectSet.RepresentativeSceneNode;
      CreateResourceModel model = new CreateResourceModel(viewModel, this.SceneNodeObjectSet.DesignerContext.ResourceManager, resourceType, targetType, this.PropertyName, (SceneElement) null, representativeSceneNode, CreateResourceModel.ContextFlags.None);
      bool? nullable = new CreateResourceDialog(this.SceneNodeObjectSet.DesignerContext, model).ShowDialog();
      if ((!nullable.GetValueOrDefault() ? 1 : (!nullable.HasValue ? true : false)) != 0)
        return false;
      string description = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.PropertyCreateResourceUndoDescription, new object[1]
      {
        (object) this.PropertyName
      });
      using (SceneEditTransaction editTransaction = viewModel.CreateEditTransaction(description))
      {
        try
        {
          bool flag = typeof (KeySpline).IsAssignableFrom(resourceType);
          int index = flag ? 0 : -1;
          foreach (SceneNode sceneNode in this.SceneNodeObjectSet.Objects)
          {
            if (!flag)
            {
              DocumentNode documentNode = sceneNode.DocumentNode;
              int num = model.IndexInResourceSite(documentNode);
              if (index < 0 || num < index)
                index = num;
            }
            SceneElement element = sceneNode.GetLocalValueAsSceneNode(this.Reference) as SceneElement;
            if (element != null)
              viewModel.AnimationEditor.DeleteAllAnimationsInSubtree(element);
          }
          if (this.IsEnabledRecordCurrentValue && this.ViewModel.AnimationEditor.IsRecording && this.ViewModel.ActiveStoryboardTimeline != null)
          {
            int num = model.IndexInResourceSite(this.ViewModel.ActiveStoryboardTimeline.DocumentNode);
            if (num >= 0 && (index < 0 || num < index))
              index = num;
          }
          DocumentCompositeNode resource = model.CreateResource(resourceNode, targetTypeProperty, index);
          if (resource != null)
          {
            DocumentNode resourceReference = model.CreateResourceReference(viewModel.Document.DocumentContext, resource, !this.CanSetDynamicExpression || !JoltHelper.TypeSupported((ITypeResolver) viewModel.ProjectContext, PlatformTypes.DynamicResource));
            using (this.SceneNodeObjectSet.ShouldAllowAnimation ? (IDisposable) null : viewModel.AnimationEditor.DeferKeyFraming())
              this.SetValue((object) resourceReference);
            editTransaction.Commit();
            this.ExplicitRecache();
            this.SceneNodeObjectSet.InvalidateLocalResourcesCache();
          }
          else
            editTransaction.Cancel();
        }
        catch (Exception ex)
        {
          if (editTransaction != null)
            editTransaction.Cancel();
          this.ShowMessage(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.InvalidPropertyValueErrorMessage, new object[1]
          {
            (object) ex.Message
          }), MessageBoxButton.OK, MessageBoxImage.Hand);
        }
      }
      return true;
    }

    public bool DoConvertToResource()
    {
      Type propertyType = this.PropertyType;
      bool isMixed;
      DocumentNode valueAsDocumentNode = this.GetLocalValueAsDocumentNode(true, out isMixed);
      if (isMixed)
        return false;
      DocumentPrimitiveNode documentPrimitiveNode = valueAsDocumentNode as DocumentPrimitiveNode;
      double result;
      DocumentNode resourceNode = valueAsDocumentNode == null || !propertyType.IsAssignableFrom(valueAsDocumentNode.TargetType) || valueAsDocumentNode.Type.IsExpression || documentPrimitiveNode != null && documentPrimitiveNode.Value is DocumentNodeStringValue && (!(documentPrimitiveNode.TargetType == typeof (double)) || !double.TryParse(documentPrimitiveNode.Value.ToString(), out result)) && (!PlatformTypes.Point.IsAssignableFrom((ITypeId) this.PropertyTypeId) && !PlatformTypes.Point3D.IsAssignableFrom((ITypeId) this.PropertyTypeId) && (!PlatformTypes.Thickness.IsAssignableFrom((ITypeId) this.PropertyTypeId) && !PlatformTypes.Vector.IsAssignableFrom((ITypeId) this.PropertyTypeId))) && !PlatformTypes.Vector3D.IsAssignableFrom((ITypeId) this.PropertyTypeId) ? this.SceneNodeObjectSet.GetRawValue(this.ViewModel.Document.DocumentContext, this.Reference, PropertyReference.GetValueFlags.Computed) : valueAsDocumentNode.Clone(valueAsDocumentNode.Context);
      Type targetType = (Type) null;
      IPropertyId targetTypeProperty = (IPropertyId) null;
      ITargetTypeMetadata targetTypeMetadata = this.PropertyTypeId.Metadata as ITargetTypeMetadata;
      if (targetTypeMetadata != null)
      {
        targetTypeProperty = targetTypeMetadata.TargetTypeProperty;
        IProperty property = (IProperty) this.Reference.LastStep;
        targetType = this.Reference.Count != 1 || this.SceneNodeObjectSet.ObjectTypeId == null ? property.DeclaringType.Metadata.GetStylePropertyTargetType((IPropertyId) property) : this.SceneNodeObjectSet.ObjectTypeId.Metadata.GetStylePropertyTargetType((IPropertyId) property);
      }
      return this.MakeNewResource(resourceNode, propertyType, targetType, targetTypeProperty);
    }

    public void DoRecordCurrentValue()
    {
      bool isRecording = this.ViewModel.AnimationEditor.IsRecording;
      this.ViewModel.AnimationEditor.IsRecording = true;
      try
      {
        this.SetValue(this.GetValue());
      }
      finally
      {
        this.ViewModel.AnimationEditor.IsRecording = isRecording;
      }
      this.ExplicitRecache();
    }

    public void UpdateResourceAndTemplateBindingSelection()
    {
      this.OnPropertyChanged("LocalResources");
      this.OnPropertyChanged("SystemResources");
      this.OnPropertyChanged("TemplateBindableProperties");
      this.OnPropertyChanged("VirtualizingResources");
    }

    internal override bool UpdateAndRefresh(PropertyReference propertyReference, AttributeCollection attributes, Type proxyType)
    {
      bool flag1 = !object.Equals((object) this.Reference, (object) propertyReference);
      if (flag1 && this.Reference != null)
        this.SceneNodeObjectSet.UnregisterPropertyChangedHandler(this.Reference, new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.SceneNodeProperty_PropertyReferenceChanged));
      bool flag2 = base.UpdateAndRefresh(propertyReference, attributes, proxyType);
      if (flag1 && this.Reference != null)
        this.SceneNodeObjectSet.RegisterPropertyChangedHandler(this.Reference, new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.SceneNodeProperty_PropertyReferenceChanged));
      return flag2;
    }

    protected object ConvertFromWpfValue(object value)
    {
      return this.ViewModel.DefaultView.ConvertFromWpfValue(value);
    }

    protected object ConvertToWpfValue(object value)
    {
      return this.SceneNodeObjectSet.DesignerContext.PlatformConverter.ConvertToWpf(this.SceneNodeObjectSet.DocumentContext, value);
    }

    public void SetValueAsWpf(object value)
    {
      this.SetValue(this.ConvertFromWpfValue(value));
    }

    public object GetValueAsWpf()
    {
      return this.ConvertToWpfValue(this.GetValue());
    }

    protected override Microsoft.Windows.Design.PropertyEditing.PropertyValue CreatePropertyValueInstance()
    {
      return (Microsoft.Windows.Design.PropertyEditing.PropertyValue) new SceneNodePropertyValue(this);
    }
  }
}
