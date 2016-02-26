// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.DataBindingDialogModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.Diagnostics;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  internal class DataBindingDialogModel : BindingDialogModelBase
  {
    private string customBindingExpression = string.Empty;
    private const string PathPrefix = "Path=";
    private const string XPathPrefix = "XPath=";
    private bool useCustomBindingExpression;
    private ObservableCollection<BindingFilterMode> bindingFilterModes;
    private DataSourceBindingSourceModel dataSourceModel;
    private ElementPropertyBindingSourceModel elementPropertyModel;
    private ExplicitDataContextBindingSourceModel explicitDataContextModel;
    private ObservableCollection<IBindingSourceModel> bindingSources;
    private BindingProxy<IBindingSourceModel> bindingSourcesProxy;
    private CollectionView bindingSourcesView;
    private bool? canSetTwoWayBinding;
    private bool bindsTwoWayByDefault;
    private bool isBinding;
    private DataSchemaNodePath dataContext;
    private ICommand addXmlDataSourceCommand;
    private ICommand createSampleDataCommand;
    private ICommand createSampleDataFromXmlCommand;
    private ICommand addClrObjectDataSourceCommand;

    public CollectionView BindingSources
    {
      get
      {
        return this.bindingSourcesView;
      }
    }

    public IBindingSourceModel CurrentBindingSource
    {
      get
      {
        return this.bindingSourcesProxy.Value;
      }
      set
      {
        this.bindingSourcesProxy.Value = value;
      }
    }

    public bool CanUseCustomBindingExpression
    {
      get
      {
        if (this.UseCustomBindingExpression)
          return this.IsBinding;
        return false;
      }
    }

    public bool UseCustomBindingExpression
    {
      get
      {
        return this.useCustomBindingExpression;
      }
      set
      {
        this.useCustomBindingExpression = value;
        this.OnPropertyChanged("UseCustomBindingExpression");
        this.OnPropertyChanged("CanUseCustomBindingExpression");
        this.OnPropertyChanged("IsBindingLegal");
        this.UpdateBindingModes();
      }
    }

    public string CustomBindingExpression
    {
      get
      {
        return this.customBindingExpression;
      }
      set
      {
        if (!(this.customBindingExpression != value))
          return;
        this.customBindingExpression = value;
        this.OnPropertyChanged("CustomBindingExpression");
        this.UpdateBindingModes();
      }
    }

    public ObservableCollection<BindingFilterMode> BindingFilterModes
    {
      get
      {
        return this.bindingFilterModes;
      }
    }

    public BindingFilterMode CurrentBindingFilterMode
    {
      get
      {
        return (BindingFilterMode) CollectionViewSource.GetDefaultView((object) this.bindingFilterModes).CurrentItem;
      }
      set
      {
        CollectionViewSource.GetDefaultView((object) this.bindingFilterModes).MoveCurrentTo((object) value);
      }
    }

    public string BindingDialogHeader
    {
      get
      {
        string str = this.TargetElement.Name;
        if (string.IsNullOrEmpty(str))
          str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[{0}]", new object[1]
          {
            (object) this.TargetElement.TargetType.Name
          });
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, StringTable.BindingDialogHeader, new object[2]
        {
          (object) str,
          (object) this.TargetProperty.Name
        });
      }
    }

    public ICommand AddXmlDataSourceCommand
    {
      get
      {
        return this.addXmlDataSourceCommand;
      }
    }

    public ICommand CreateSampleDataCommand
    {
      get
      {
        return this.createSampleDataCommand;
      }
    }

    public ICommand CreateSampleDataFromXmlCommand
    {
      get
      {
        return this.createSampleDataFromXmlCommand;
      }
    }

    public ICommand AddClrObjectDataSourceCommand
    {
      get
      {
        return this.addClrObjectDataSourceCommand;
      }
    }

    public ICommand AddValueConverterCommand
    {
      get
      {
        return (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.OnAddValueConverter));
      }
    }

    public bool IsBinding
    {
      get
      {
        return this.isBinding;
      }
      private set
      {
        if (this.isBinding == value)
          return;
        this.isBinding = value;
        this.OnPropertyChanged("IsBinding");
        this.OnPropertyChanged("CanUseCustomBindingExpression");
      }
    }

    public bool IsBindingLegal
    {
      get
      {
        using (this.ViewModel.Document.CreateEditTransaction(string.Empty, true))
        {
          SceneNode bindingOrDataInternal = this.CreateBindingOrDataInternal(true);
          if (bindingOrDataInternal == null)
            return false;
          BindingSceneNode bindingSceneNode = bindingOrDataInternal as BindingSceneNode;
          if (bindingSceneNode == null)
            return true;
          string errorMessage;
          return this.UseCustomBindingExpression || bindingSceneNode.IsBindingLegal(this.TargetElement, this.TargetProperty, out errorMessage);
        }
      }
    }

    public override DataSchemaNodePath CurrentDataPath
    {
      get
      {
        if (this.CurrentBindingSource.SchemaItem == null)
          return (DataSchemaNodePath) null;
        DataSchemaNodePath dataSchemaNodePath = (DataSchemaNodePath) null;
        if (this.UseCustomBindingExpression)
          dataSchemaNodePath = this.CurrentBindingSource.SchemaItem.Schema.GetNodePathFromPath(this.CustomBindingExpression);
        else if (this.CurrentBindingSource.SchemaItem.SelectedItem != null)
          dataSchemaNodePath = this.CurrentBindingSource.SchemaItem.SelectedItem.DataSchemaNodePath;
        return dataSchemaNodePath;
      }
    }

    private DataSchemaNodePath CurrentRelativeSchemaPath
    {
      get
      {
        DataSchemaNodePath dataSchemaNodePath = this.CurrentDataPath;
        if (dataSchemaNodePath != null && this.dataContext != null && this.dataContext.IsSubpathOf(dataSchemaNodePath))
          dataSchemaNodePath = this.dataContext.GetRelativeNodePath(dataSchemaNodePath);
        return dataSchemaNodePath;
      }
    }

    public DataBindingDialogModel(DataPanelModel model, SceneNode targetElement, ReferenceStep targetProperty)
      : base(targetElement, targetProperty)
    {
      this.isBinding = true;
      this.addXmlDataSourceCommand = model.AddXmlDataSourceCommand;
      this.createSampleDataCommand = model.CreateSampleDataCommand;
      this.createSampleDataFromXmlCommand = model.CreateSampleDataFromXmlCommand;
      this.addClrObjectDataSourceCommand = model.AddClrObjectDataSourceCommand;
      DataContextInfo dataContextInfo = new DataContextEvaluator().Evaluate(targetElement, (IPropertyId) targetProperty, true);
      if (dataContextInfo.DataSource.IsValidWithSource)
      {
        ISchema schemaForDataSource = SchemaManager.GetSchemaForDataSource(dataContextInfo.DataSource.SourceNode);
        if (schemaForDataSource != null && !(schemaForDataSource is EmptySchema))
          this.dataContext = schemaForDataSource.GetNodePathFromPath(dataContextInfo.DataSource.Path);
      }
      this.bindsTwoWayByDefault = this.BindsTwoWayByDefault(targetElement, targetProperty);
      this.bindingFilterModes = new ObservableCollection<BindingFilterMode>();
      this.bindingFilterModes.Add(BindingFilterMode.None);
      this.bindingFilterModes.Add(BindingFilterMode.FilterByType);
      ICollectionView defaultView = CollectionViewSource.GetDefaultView((object) this.bindingFilterModes);
      defaultView.MoveCurrentTo((object) BindingFilterMode.FilterByType);
      this.bindingSources = new ObservableCollection<IBindingSourceModel>();
      this.dataSourceModel = new DataSourceBindingSourceModel(model, new DataSchemaItemFilter(this.BindingFilter));
      this.AddBindingSource((IBindingSourceModel) this.dataSourceModel);
      if (targetElement.ViewModel.ActiveEditingContainer is SceneElement)
      {
        this.elementPropertyModel = new ElementPropertyBindingSourceModel(targetElement, new DataSchemaItemFilter(this.BindingFilter));
        this.AddBindingSource((IBindingSourceModel) this.elementPropertyModel);
      }
      this.explicitDataContextModel = new ExplicitDataContextBindingSourceModel(targetElement, targetProperty, new DataSchemaItemFilter(this.BindingFilter));
      this.AddBindingSource((IBindingSourceModel) this.explicitDataContextModel);
      this.bindingSourcesProxy = new BindingProxy<IBindingSourceModel>();
      this.bindingSourcesProxy.PropertyChanged += new PropertyChangedEventHandler(this.BindingSourcesProxy_PropertyChanged);
      this.bindingSourcesView = (CollectionView) new DataBindingProxyCollectionView<IBindingSourceModel>(this.bindingSources, (IDataBindingProxy<IBindingSourceModel>) this.bindingSourcesProxy);
      BindingSceneNode binding = targetElement.GetBinding((IPropertyId) targetProperty);
      if (binding != null)
        this.SetExistingBindingValues(binding);
      else if (!(this.explicitDataContextModel.Schema is EmptySchema))
        this.bindingSourcesView.MoveCurrentTo((object) this.explicitDataContextModel);
      else
        this.bindingSourcesView.MoveCurrentTo((object) this.dataSourceModel);
      if (binding == null || !binding.IsModeSet)
        this.CurrentBindingMode = BindingPropertyHelper.GetDefaultBindingMode(this.TargetElement.DocumentNode, (IPropertyId) this.TargetProperty, this.CurrentDataPath).Mode;
      this.bindingSourcesView.CurrentChanging += new CurrentChangingEventHandler(this.BindingSourcesView_CurrentChanging);
      defaultView.CurrentChanged += new EventHandler(this.BindingFilterModesView_CurrentChanged);
      this.PropertyChanged += new PropertyChangedEventHandler(this.DataBindingDialogModel_PropertyChanged);
    }

    protected override void InitializeExpandedArea()
    {
      base.InitializeExpandedArea();
      if (this.ValueConverters == null)
        return;
      CollectionViewSource.GetDefaultView((object) this.ValueConverters).CurrentChanged += new EventHandler(this.valueConvertersView_CurrentChanged);
    }

    public void Teardown()
    {
      this.bindingSourcesProxy.PropertyChanged -= new PropertyChangedEventHandler(this.BindingSourcesProxy_PropertyChanged);
      this.bindingSourcesView.CurrentChanging -= new CurrentChangingEventHandler(this.BindingSourcesView_CurrentChanging);
      CollectionViewSource.GetDefaultView((object) this.bindingFilterModes).CurrentChanged -= new EventHandler(this.BindingFilterModesView_CurrentChanged);
      this.PropertyChanged -= new PropertyChangedEventHandler(this.DataBindingDialogModel_PropertyChanged);
      if (this.ValueConverters != null)
        CollectionViewSource.GetDefaultView((object) this.ValueConverters).CurrentChanged -= new EventHandler(this.valueConvertersView_CurrentChanged);
      foreach (IBindingSourceModel bindingSourceModel in (Collection<IBindingSourceModel>) this.bindingSources)
      {
        bindingSourceModel.PropertyChanged -= new PropertyChangedEventHandler(this.BindingSourceModel_PropertyChanged);
        bindingSourceModel.Unhook();
      }
      this.bindingSources.Clear();
    }

    public SceneNode CreateBindingOrData()
    {
      return this.CreateBindingOrDataInternal(false);
    }

    private SceneNode CreateBindingOrDataInternal(bool bindingNeededForVerification)
    {
      SceneNode sceneNode = (SceneNode) null;
      BindingSceneNode bindingSceneNode1 = (BindingSceneNode) null;
      if (this.CurrentBindingSource != null)
      {
        if (bindingNeededForVerification)
          sceneNode = this.CurrentBindingSource.CreateBindingOrData(this.TargetElement.ViewModel, this.TargetElement, (IProperty) this.TargetProperty);
        else if (this.useCustomBindingExpression)
        {
          sceneNode = this.CurrentBindingSource.CreateBindingOrData(this.TargetElement.ViewModel, this.customBindingExpression, this.TargetElement, (IProperty) this.TargetProperty);
          BindingSceneNode bindingSceneNode2 = sceneNode as BindingSceneNode;
          if (bindingSceneNode2 != null)
          {
            if (this.customBindingExpression.StartsWith("Path=", StringComparison.Ordinal))
            {
              bindingSceneNode2.ClearValue(BindingSceneNode.XPathProperty);
              bindingSceneNode1 = bindingSceneNode2.SetPath(this.customBindingExpression.Substring("Path=".Length));
            }
            else if (this.customBindingExpression.StartsWith("XPath=", StringComparison.Ordinal) && bindingSceneNode2.SupportsXPath)
            {
              bindingSceneNode2.XPath = this.customBindingExpression.Substring("XPath=".Length);
              bindingSceneNode2.ClearValue(BindingSceneNode.PathProperty);
            }
          }
        }
        else
          sceneNode = this.CurrentBindingSource.CreateBindingOrData(this.TargetElement.ViewModel, this.TargetElement, (IProperty) this.TargetProperty);
        BindingSceneNode binding = sceneNode as BindingSceneNode;
        if (binding != null)
          this.SetCommonBindingValues(binding);
      }
      return sceneNode;
    }

    private void OnAddValueConverter()
    {
      if (this.ValueConverters == null)
        return;
      using (SceneEditTransaction editTransaction = this.ViewModel.CreateEditTransaction(StringTable.AddValueConverterDialogTitle))
      {
        SceneNode valueConverter = AddValueConverterDialog.CreateValueConverter(this.ViewModel);
        if (valueConverter == null)
          return;
        ValueConverterModel valueConverterModel = ValueConverterModelFactory.CreateValueConverterModel(valueConverter);
        if (valueConverterModel == null)
          return;
        this.ValueConverters.Add(valueConverterModel);
        this.CurrentValueConverter = valueConverterModel;
        editTransaction.Commit();
      }
    }

    private void BindingSourcesProxy_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      this.OnPropertyChanged("CurrentBindingSource");
      this.CustomBindingExpression = this.bindingSourcesProxy.Value.Path;
      this.UpdateIsBinding();
      this.UpdateBindingModes();
      this.RefreshSchemaItemFilter();
      this.OnPropertyChanged("IsBindingLegal");
    }

    private void BindingSourcesView_CurrentChanging(object sender, CurrentChangingEventArgs e)
    {
      PerformanceUtility.MeasurePerformanceUntilRender(PerformanceEvent.SwitchDataSourceType);
    }

    private void DataBindingDialogModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "IsExpanded") || !this.IsExpanded)
        return;
      this.UpdateBindingModes();
    }

    private void BindingFilterModesView_CurrentChanged(object sender, EventArgs e)
    {
      this.RefreshSchemaItemFilter();
      this.OnPropertyChanged("CurrentBindingFilterMode");
    }

    private void BindingSourceModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      IBindingSourceModel bindingSourceModel = (IBindingSourceModel) sender;
      if (!(e.PropertyName == "Path"))
        return;
      this.CustomBindingExpression = bindingSourceModel.Path;
      this.UpdateIsBinding();
      this.UpdateBindingModes();
      this.OnPropertyChanged("IsBindingLegal");
    }

    private void valueConvertersView_CurrentChanged(object sender, EventArgs e)
    {
      this.RefreshSchemaItemFilter();
    }

    private void RefreshSchemaItemFilter()
    {
      if (this.CurrentBindingSource.SchemaItem == null)
        return;
      this.CurrentBindingSource.SchemaItem.RefreshDataSchemaItemFilter();
    }

    private void AddBindingSource(IBindingSourceModel bindingSourceModel)
    {
      bindingSourceModel.PropertyChanged += new PropertyChangedEventHandler(this.BindingSourceModel_PropertyChanged);
      this.bindingSources.Add(bindingSourceModel);
    }

    private void SetExistingBindingValues(BindingSceneNode existingBinding)
    {
      string path = existingBinding.PathOrXPath;
      DocumentNode node1 = (DocumentNode) null;
      DocumentCompositeNode node2 = existingBinding.SupportsSource ? existingBinding.Source as DocumentCompositeNode : (DocumentCompositeNode) null;
      if (node2 != null && node2.Type.IsResource)
        node1 = ResourceNodeHelper.GetResourceKey(node2);
      if (node1 != null)
        this.InitializeDataSource(DocumentPrimitiveNode.GetValueAsString(node1), path);
      else if (existingBinding.SupportsElementName && existingBinding.ElementName != null && this.elementPropertyModel != null)
        this.InitializeElementName(existingBinding.ElementName, path);
      else
        this.InitializeExplicitDataContext(path);
      if (this.bindingSourcesProxy.Value.Path != path)
      {
        if (this.CurrentBindingSource.Schema is EmptySchema && existingBinding.SupportsXPath && !string.IsNullOrEmpty(existingBinding.XPath))
          path = "XPath=" + path;
        this.CustomBindingExpression = path;
        this.UseCustomBindingExpression = true;
      }
      DocumentNode node3 = existingBinding.SupportsFallbackValue ? existingBinding.FallbackValue : (DocumentNode) null;
      if (node3 != null)
      {
        string valueAsString = DocumentPrimitiveNode.GetValueAsString(node3);
        if (valueAsString != null)
          this.BindingFallbackValue = valueAsString;
      }
      if (existingBinding.IsModeSet)
        this.CurrentBindingMode = existingBinding.Mode;
      if (!existingBinding.SupportsUpdateSourceTrigger)
        return;
      this.CurrentUpdateSourceTrigger = existingBinding.UpdateSourceTrigger;
    }

    private void InitializeDataSource(string dataSourceName, string path)
    {
      this.CurrentBindingSource = (IBindingSourceModel) this.dataSourceModel;
      DataSourceItem dataSourceItem1 = (DataSourceItem) null;
      foreach (DataSourceItem dataSourceItem2 in this.dataSourceModel.Model.DataSources)
      {
        if (dataSourceItem2.DataSourceNode.Name == dataSourceName)
        {
          dataSourceItem1 = dataSourceItem2;
          break;
        }
      }
      if (dataSourceItem1 == null)
        return;
      this.dataSourceModel.Model.SelectionContext.SetSelection((DataModelItemBase) dataSourceItem1);
      DataSchemaItem itemFromPath = dataSourceItem1.SchemaItem.GetItemFromPath(path);
      if (itemFromPath == null)
        return;
      dataSourceItem1.SchemaItem.SelectedItem = itemFromPath;
      itemFromPath.ExpandAncestors();
    }

    private void InitializeElementName(string elementName, string path)
    {
      this.CurrentBindingSource = (IBindingSourceModel) this.elementPropertyModel;
      if (elementName == null)
        return;
      ElementBindingSourceNode elementNodeByName = this.elementPropertyModel.FindElementNodeByName(elementName);
      if (elementNodeByName == null)
        return;
      elementNodeByName.ExpandAncestors();
      this.elementPropertyModel.SelectedNode = elementNodeByName;
      DataSchemaItem itemFromPath = elementNodeByName.SchemaItem.GetItemFromPath(path);
      if (itemFromPath == null)
        return;
      elementNodeByName.SchemaItem.SelectedItem = itemFromPath;
      itemFromPath.ExpandAncestors();
    }

    private void InitializeExplicitDataContext(string path)
    {
      this.CurrentBindingSource = (IBindingSourceModel) this.explicitDataContextModel;
      DataSchemaItem itemFromPath = this.explicitDataContextModel.SchemaItem.GetItemFromPath(path);
      if (itemFromPath == null)
        return;
      this.explicitDataContextModel.SchemaItem.SelectedItem = itemFromPath;
      itemFromPath.ExpandAncestors();
    }

    private void UpdateIsBinding()
    {
      if (this.CurrentBindingSource.SchemaItem == null)
      {
        this.IsBinding = true;
      }
      else
      {
        using (this.ViewModel.Document.CreateEditTransaction(string.Empty, true))
          this.IsBinding = this.CreateBindingOrDataInternal(true) is BindingSceneNode;
      }
    }

    private void UpdateBindingModes()
    {
      if (!this.IsBinding)
        return;
      if (!this.IsExpanded)
      {
        this.CurrentBindingMode = BindingPropertyHelper.GetDefaultBindingMode(this.TargetElement.DocumentNode, (IPropertyId) this.TargetProperty, this.CurrentDataPath).Mode;
      }
      else
      {
        DataSchemaNodePath relativeSchemaPath = this.CurrentRelativeSchemaPath;
        bool flag = false;
        if (relativeSchemaPath != null)
        {
          if (!relativeSchemaPath.Node.IsReadOnly && relativeSchemaPath.Node != relativeSchemaPath.Schema.Root)
            flag = true;
        }
        else if (this.UseCustomBindingExpression && !string.IsNullOrEmpty(this.CustomBindingExpression))
          flag = true;
        if (this.canSetTwoWayBinding.HasValue && this.canSetTwoWayBinding.Value == flag)
          return;
        this.canSetTwoWayBinding = new bool?(flag);
        if (flag)
        {
          if (!this.BindingModesCollection.Contains(BindingMode.TwoWay) && this.IsBindingModeSupported(BindingMode.TwoWay))
            this.BindingModesCollection.Add(BindingMode.TwoWay);
          if (!this.BindingModesCollection.Contains(BindingMode.OneWayToSource) && this.IsBindingModeSupported(BindingMode.OneWayToSource))
            this.BindingModesCollection.Add(BindingMode.OneWayToSource);
          if (this.BindingModesCollection.Contains(BindingMode.Default) || !this.IsBindingModeSupported(BindingMode.Default))
            return;
          this.BindingModesCollection.Insert(0, BindingMode.Default);
        }
        else
        {
          if (this.CurrentBindingMode == BindingMode.TwoWay || this.CurrentBindingMode == BindingMode.OneWayToSource || this.bindsTwoWayByDefault && this.CurrentBindingMode == BindingMode.Default)
          {
            if ((relativeSchemaPath != null ? relativeSchemaPath.Node : (DataSchemaNode) null) == null)
            {
              if (this.bindsTwoWayByDefault)
                this.CurrentBindingMode = BindingMode.OneWay;
              else
                this.CurrentBindingMode = (BindingMode) this.TargetElement.ProjectContext.GetCapabilityValue(PlatformCapability.DefaultBindingMode);
            }
            else
              this.CurrentBindingMode = BindingPropertyHelper.GetDefaultBindingMode(this.TargetElement.DocumentNode, (IPropertyId) this.TargetProperty, relativeSchemaPath).Mode;
          }
          this.BindingModesCollection.Remove(BindingMode.TwoWay);
          this.BindingModesCollection.Remove(BindingMode.OneWayToSource);
          if (!this.bindsTwoWayByDefault)
            return;
          this.BindingModesCollection.Remove(BindingMode.Default);
        }
      }
    }

    private bool BindingFilter(DataSchemaItem item)
    {
      switch (this.CurrentBindingFilterMode)
      {
        case BindingFilterMode.None:
          return true;
        case BindingFilterMode.FilterByType:
          DataSchemaItem dataSchemaItem = item;
          IProjectContext projectContext = this.TargetElement.ProjectContext;
          if (item.Parent != null && !BindingEditor.CanBindToSchemaNode(this.TargetElement, (IPropertyId) this.TargetProperty, dataSchemaItem.DataSchemaNode))
            return dataSchemaItem.HasChildren;
          return true;
        default:
          return true;
      }
    }
  }
}
