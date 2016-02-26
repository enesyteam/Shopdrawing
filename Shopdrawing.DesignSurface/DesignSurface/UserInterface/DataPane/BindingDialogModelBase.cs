// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.BindingDialogModelBase
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.UserInterface.ResourcePane;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  internal abstract class BindingDialogModelBase : INotifyPropertyChanged
  {
    private BindingMode[] bindingModes = new BindingMode[5]
    {
      BindingMode.Default,
      BindingMode.OneTime,
      BindingMode.OneWay,
      BindingMode.TwoWay,
      BindingMode.OneWayToSource
    };
    private ObservableCollection<BindingMode> bindingModesCollection;
    private BindingProxy<BindingMode> bindingModeProxy;
    private CollectionView bindingModesView;
    private string[] supportedBindingModeNames;
    private BindingProxy<object> updateSourceTriggerProxy;
    private CollectionView updateSourceTriggersView;
    private ObservableCollection<ValueConverterModel> valueConverters;
    private DocumentNode originalValueConverter;
    private string valueConverterParameter;
    private string bindingFallbackValue;
    private bool isExpanded;
    private bool isExpandedAreaInitialized;

    public abstract DataSchemaNodePath CurrentDataPath { get; }

    public SceneNode TargetElement { get; private set; }

    public ReferenceStep TargetProperty { get; protected set; }

    public ObservableCollection<BindingMode> BindingModesCollection
    {
      get
      {
        return this.bindingModesCollection;
      }
    }

    public CollectionView BindingModes
    {
      get
      {
        return this.bindingModesView;
      }
    }

    public BindingMode CurrentBindingMode
    {
      get
      {
        return this.bindingModeProxy.Value;
      }
      set
      {
        if (this.bindingModeProxy.Value == value)
          return;
        this.bindingModeProxy.Value = value;
        this.OnPropertyChanged("CurrentBindingMode");
      }
    }

    public ObservableCollection<ValueConverterModel> ValueConverters
    {
      get
      {
        return this.valueConverters;
      }
    }

    public ValueConverterModel CurrentValueConverter
    {
      get
      {
        if (this.ValueConverters != null)
          return (ValueConverterModel) CollectionViewSource.GetDefaultView((object) this.ValueConverters).CurrentItem;
        return (ValueConverterModel) null;
      }
      set
      {
        CollectionViewSource.GetDefaultView((object) this.ValueConverters).MoveCurrentTo((object) value);
      }
    }

    public string ValueConverterParameter
    {
      get
      {
        return this.valueConverterParameter;
      }
      set
      {
        this.valueConverterParameter = value;
        this.OnPropertyChanged("ValueConverterParameter");
      }
    }

    public CollectionView UpdateSourceTriggers
    {
      get
      {
        return this.updateSourceTriggersView;
      }
    }

    public object CurrentUpdateSourceTrigger
    {
      get
      {
        return this.updateSourceTriggerProxy.Value;
      }
      set
      {
        this.updateSourceTriggerProxy.Value = value;
      }
    }

    public bool IsUpdateSourceTriggerSupported
    {
      get
      {
        return BindingSceneNode.IsUpdateSourceTriggerSupported(this.ViewModel);
      }
    }

    public string BindingFallbackValue
    {
      get
      {
        return this.bindingFallbackValue;
      }
      set
      {
        this.bindingFallbackValue = value;
        this.OnPropertyChanged("BindingFallbackValue");
      }
    }

    public bool IsBindingFallbackValueSupported
    {
      get
      {
        return BindingSceneNode.IsFallbackValueSupported(this.ViewModel);
      }
    }

    public bool IsExpanded
    {
      get
      {
        return this.isExpanded;
      }
      set
      {
        if (value && !this.isExpandedAreaInitialized)
        {
          this.isExpandedAreaInitialized = true;
          this.InitializeExpandedArea();
        }
        this.isExpanded = value;
        this.OnPropertyChanged("IsExpanded");
        this.OnPropertyChanged("ExpanderText");
      }
    }

    public string ExpanderText
    {
      get
      {
        if (this.isExpanded)
          return StringTable.BindingDialogFewerOptionsText;
        return StringTable.BindingDialogMoreOptionsText;
      }
    }

    public SceneViewModel ViewModel
    {
      get
      {
        return this.TargetElement.ViewModel;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public BindingDialogModelBase(SceneNode targetElement, ReferenceStep targetProperty)
    {
      this.TargetElement = targetElement;
      this.TargetProperty = targetProperty;
      IType type1 = this.ViewModel.ProjectContext.ResolveType(PlatformTypes.BindingMode);
      this.supportedBindingModeNames = Enum.GetNames(type1.RuntimeType);
      this.bindingModeProxy = new BindingProxy<BindingMode>();
      this.bindingModesCollection = new ObservableCollection<BindingMode>();
      foreach (BindingMode bindingMode in this.bindingModes)
      {
        if (this.IsBindingModeSupported(bindingMode))
          this.bindingModesCollection.Add(bindingMode);
      }
      this.bindingModesView = (CollectionView) new DataBindingProxyCollectionView<BindingMode>(this.bindingModesCollection, (IDataBindingProxy<BindingMode>) this.bindingModeProxy);
      this.bindingModesView.MoveCurrentToFirst();
      this.bindingModeProxy.Value = BindingPropertyHelper.GetDefaultBindingMode(this.TargetElement.DocumentNode, (IPropertyId) this.TargetProperty, (DataSchemaNodePath) null).Mode;
      if (this.TargetProperty != null)
      {
        BindingSceneNode binding = this.TargetElement.GetBinding((IPropertyId) this.TargetProperty);
        if (binding != null)
          this.originalValueConverter = binding.Converter;
      }
      this.updateSourceTriggerProxy = new BindingProxy<object>();
      ObservableCollection<object> collection = new ObservableCollection<object>();
      IType type2 = this.ViewModel.ProjectContext.ResolveType(PlatformTypes.UpdateSourceTrigger);
      if (!this.ViewModel.ProjectContext.PlatformMetadata.IsNullType((ITypeId) type2))
      {
        IProperty property = this.ViewModel.ProjectContext.ResolveProperty(BindingSceneNode.UpdateSourceTriggerProperty);
        if (property != null)
          this.updateSourceTriggerProxy.Value = property.GetDefaultValue(type1.RuntimeType);
        foreach (object obj in (IEnumerable) MetadataStore.GetTypeConverter(type2.RuntimeType).GetStandardValues())
          collection.Add(obj);
      }
      this.updateSourceTriggersView = (CollectionView) new DataBindingProxyCollectionView<object>(collection, (IDataBindingProxy<object>) this.updateSourceTriggerProxy);
    }

    protected void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    protected void SetCommonBindingValues(BindingSceneNode binding)
    {
      IDocumentContext documentContext = binding.DocumentContext;
      bool flag = !string.IsNullOrEmpty(binding.Path) || binding.SupportsXPath && !string.IsNullOrEmpty(binding.XPath);
      BindingMode bindingMode = this.CurrentBindingMode;
      if (!flag && (bindingMode == BindingMode.Default || bindingMode == BindingMode.OneWayToSource || bindingMode == BindingMode.TwoWay))
        bindingMode = BindingMode.OneWay;
      if (bindingMode == BindingMode.TwoWay && binding.SupportsUpdateSourceTrigger)
      {
        if (binding.GetDefaultValue(BindingSceneNode.UpdateSourceTriggerProperty) == this.CurrentUpdateSourceTrigger)
          binding.ClearLocalValue(BindingSceneNode.UpdateSourceTriggerProperty);
        else
          binding.UpdateSourceTrigger = this.CurrentUpdateSourceTrigger;
      }
      BindingModeInfo defaultBindingMode = BindingPropertyHelper.GetDefaultBindingMode(this.TargetElement.DocumentNode, (IPropertyId) this.TargetProperty, this.CurrentDataPath);
      if (defaultBindingMode.IsOptional && defaultBindingMode.Mode == bindingMode)
        binding.ClearLocalValue(BindingSceneNode.ModeProperty);
      else
        binding.Mode = bindingMode;
      if (!string.IsNullOrEmpty(this.BindingFallbackValue))
        binding.FallbackValue = (DocumentNode) documentContext.CreateNode(this.BindingFallbackValue);
      SceneNode sceneNode = (SceneNode) null;
      if (this.CurrentValueConverter != null)
        sceneNode = this.CurrentValueConverter.GenerateConverter();
      else if (this.originalValueConverter != null)
        sceneNode = this.ViewModel.GetSceneNode(this.originalValueConverter.Clone(binding.DocumentContext));
      if (sceneNode != null)
        binding.SetValueAsSceneNode(BindingSceneNode.ConverterProperty, sceneNode);
      if (string.IsNullOrEmpty(this.valueConverterParameter))
        return;
      binding.ConverterParameter = (object) this.valueConverterParameter;
    }

    protected bool BindsTwoWayByDefault(SceneNode targetNode, ReferenceStep targetProperty)
    {
      DependencyPropertyReferenceStep propertyReferenceStep = targetProperty as DependencyPropertyReferenceStep;
      if (propertyReferenceStep == null)
        return false;
      Type targetType = targetNode.TargetType;
      if (DocumentNodeUtilities.IsStyleOrTemplate(targetNode.Type))
        targetType = DocumentNodeUtilities.GetStyleOrTemplateTargetType(targetNode.DocumentNode).RuntimeType;
      return propertyReferenceStep.BindsTwoWayByDefault(targetType);
    }

    protected virtual void InitializeExpandedArea()
    {
      this.InitializeValueConverters();
      this.InitializeCurrentValueConverter();
    }

    protected void InitializeValueConverters()
    {
      this.valueConverterParameter = string.Empty;
      List<ValueConverterModel> list = new List<ValueConverterModel>();
      List<SceneDocument> visitedDocuments = new List<SceneDocument>();
      for (SceneNode sceneNode = this.TargetElement; sceneNode != null; sceneNode = sceneNode.Parent)
        this.AddSceneNodeValueConverters(sceneNode, list, visitedDocuments);
      foreach (SceneDocument document in this.ViewModel.Document.DesignTimeResourceDocuments)
        this.AddDocumentConverters(document, list, visitedDocuments);
      list.Sort((Comparison<ValueConverterModel>) ((a, b) => string.Compare(a.DisplayName, b.DisplayName, StringComparison.OrdinalIgnoreCase)));
      list.Insert(0, (ValueConverterModel) new ValueConverterNullModel());
      this.valueConverters = new ObservableCollection<ValueConverterModel>(list);
      this.InitializeCurrentValueConverter();
      this.OnPropertyChanged("ValueConverters");
      this.OnPropertyChanged("CurrentValueConverter");
    }

    private void AddSceneNodeValueConverters(SceneNode sceneNode, List<ValueConverterModel> converters, List<SceneDocument> visitedDocuments)
    {
      ResourceDictionaryNode resourceDictionary = ResourceManager.ProvideResourcesForElement(sceneNode);
      if (resourceDictionary == null)
        return;
      this.AddValueConverters(resourceDictionary, converters, visitedDocuments);
    }

    private void AddDocumentConverters(SceneDocument document, List<ValueConverterModel> converters, List<SceneDocument> visitedDocuments)
    {
      if (document == null || document.DocumentRoot == null || (document.DocumentRoot.RootNode == null || visitedDocuments.Contains(document)))
        return;
      visitedDocuments.Add(document);
      this.AddSceneNodeValueConverters(this.ViewModel.GetViewModel(document.DocumentRoot, false).GetSceneNode(document.DocumentRoot.RootNode), converters, visitedDocuments);
    }

    private void AddValueConverters(ResourceDictionaryNode resourceDictionary, List<ValueConverterModel> converters, List<SceneDocument> visitedDocuments)
    {
      foreach (DictionaryEntryNode dictionaryEntryNode in resourceDictionary)
      {
        ValueConverterModel valueConverter = ValueConverterModelFactory.CreateValueConverterModel((SceneNode) dictionaryEntryNode);
        if (valueConverter != null && converters.Find((Predicate<ValueConverterModel>) (vc => vc.DisplayName == valueConverter.DisplayName)) == null)
          converters.Add(valueConverter);
      }
      foreach (ResourceDictionaryNode resourceDictionaryNode in (IEnumerable<ResourceDictionaryNode>) resourceDictionary.MergedDictionaries)
      {
        string designTimeSource = resourceDictionaryNode.DesignTimeSource;
        if (!string.IsNullOrEmpty(designTimeSource))
        {
          IProjectDocument projectDocument = this.TargetElement.ProjectContext.OpenDocument(designTimeSource);
          if (projectDocument != null)
            this.AddDocumentConverters(projectDocument.Document as SceneDocument, converters, visitedDocuments);
        }
      }
    }

    protected void InitializeCurrentValueConverter()
    {
      if (this.TargetProperty == null)
        return;
      BindingSceneNode binding = this.TargetElement.GetBinding((IPropertyId) this.TargetProperty);
      if (binding == null)
        return;
      string str = binding.ConverterParameter as string;
      if (str != null)
        this.ValueConverterParameter = str;
      DocumentCompositeNode node = binding.Converter as DocumentCompositeNode;
      if (node == null)
        return;
      ValueConverterModel valueConverterModel = (ValueConverterModel) null;
      if (node.Type.IsResource)
      {
        string key = DocumentPrimitiveNode.GetValueAsString(ResourceNodeHelper.GetResourceKey(node));
        if (string.IsNullOrEmpty(key))
          return;
        valueConverterModel = Enumerable.FirstOrDefault<ValueConverterModel>((IEnumerable<ValueConverterModel>) this.ValueConverters, (Func<ValueConverterModel, bool>) (vc => vc.DisplayName == key));
      }
      else
      {
        SceneNode sceneNode = this.ViewModel.GetSceneNode((DocumentNode) node);
        if (sceneNode != null)
        {
          valueConverterModel = ValueConverterModelFactory.CreateValueConverterModel(sceneNode);
          if (valueConverterModel != null)
            this.ValueConverters.Add(valueConverterModel);
        }
      }
      if (valueConverterModel == null)
        return;
      this.CurrentValueConverter = valueConverterModel;
    }

    protected bool IsBindingModeSupported(BindingMode bindingMode)
    {
      return Array.IndexOf<string>(this.supportedBindingModeNames, Enum.GetName(typeof (BindingMode), (object) bindingMode)) != -1;
    }
  }
}
