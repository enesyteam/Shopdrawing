// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.DataContextEvaluator
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.InstanceBuilders;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class DataContextEvaluator
  {
    private DataContextEvaluator.VisitedLocations visitedLocations = new DataContextEvaluator.VisitedLocations();
    private DataContextInfo evaluatingInfo;

    public DataContextInfo Evaluate(SceneNode target)
    {
      return this.Evaluate(target, (IPropertyId) null, true);
    }

    public DataContextInfo Evaluate(SceneNode target, IPropertyId targetProperty, bool isForSettingValue)
    {
      SceneNode targetNode = target;
      IProperty targetProperty1 = (IProperty) null;
      if (targetProperty != null)
      {
        targetProperty1 = target.ProjectContext.ResolveProperty(targetProperty);
        if (DataContextHelper.IsDataContextProperty(target.DocumentNode, (IPropertyId) targetProperty1))
        {
          targetNode = target.Parent;
          targetProperty1 = (IProperty) null;
          if (targetNode == null)
            return new DataContextInfo();
        }
      }
      return this.Evaluate((IDataContextAncestorWalker) new DataContextAncestorWalker(targetNode, targetProperty1), isForSettingValue);
    }

    public DataContextInfo Evaluate(IDataContextAncestorWalker walker, bool isForSettingValue)
    {
      this.visitedLocations.Reset();
      DataContextInfo dataContextInfo1 = new DataContextInfo();
      RawDataSourceInfoBase dataSourceInfoBase1 = (RawDataSourceInfoBase) null;
      bool flag = isForSettingValue;
      this.evaluatingInfo = dataContextInfo1;
      DataContextInfo dataContextInfo2 = (DataContextInfo) null;
      try
      {
        while (walker.MoveNext())
        {
          if (flag)
            flag = DataContextMetadata.GetDataContextAttribute<DualDataContextAttribute>(walker.CurrentProperty) != null;
          RawDataSourceInfoBase dataSourceInfoBase2 = !flag ? this.NavigateSpecialDataContextInternal(walker, dataSourceInfoBase1, true) : dataSourceInfoBase1;
          flag = false;
          if (dataSourceInfoBase2 != dataSourceInfoBase1)
          {
            dataSourceInfoBase1 = dataSourceInfoBase2;
            this.MoveUpIfDataContextLocation(walker);
          }
          if (dataSourceInfoBase1 != null)
          {
            if (!dataSourceInfoBase1.IsValid)
              break;
          }
          RawDataSourceInfoBase rawDataContextInfo = this.GetRawDataContextInfo(walker);
          if (rawDataContextInfo != null)
          {
            if (rawDataContextInfo.IsValid)
              dataSourceInfoBase1 = rawDataContextInfo.CombineWith(dataSourceInfoBase1);
            else if (dataSourceInfoBase1 != null)
              dataSourceInfoBase1.SetInvalid();
            else
              dataSourceInfoBase1 = rawDataContextInfo;
          }
          if (dataSourceInfoBase1 != null)
          {
            if (dataSourceInfoBase1.IsValid)
            {
              if (dataSourceInfoBase1.HasSource)
                break;
            }
            else
              break;
          }
          if (dataContextInfo2 == null)
            dataContextInfo2 = this.GetTypedTemplateDataContext(walker.CurrentNode, dataSourceInfoBase1);
        }
      }
      finally
      {
        this.evaluatingInfo = (DataContextInfo) null;
      }
      if (dataContextInfo2 != null && dataContextInfo2.DataSource.IsValidWithSource && (dataSourceInfoBase1 == null || !dataSourceInfoBase1.IsValid || !dataSourceInfoBase1.HasSource))
        dataContextInfo1 = dataContextInfo2;
      else
        dataContextInfo1.RawDataSource = dataSourceInfoBase1;
      return dataContextInfo1;
    }

    private DataContextInfo GetTypedTemplateDataContext(DocumentCompositeNode documentNode, RawDataSourceInfoBase localSource)
    {
      if (!PlatformTypes.DataTemplate.IsAssignableFrom((ITypeId) documentNode.Type))
        return (DataContextInfo) null;
      IProperty dataContextProperty = documentNode.TypeResolver.ResolveProperty(DataTemplateElement.DataTypeProperty);
      if (dataContextProperty == null)
        return (DataContextInfo) null;
      if (DocumentPrimitiveNode.GetValueAsType(documentNode.Properties[(IPropertyId) dataContextProperty]) == null)
        return (DataContextInfo) null;
      DataContextInfo dataContextInfo = new DataContextInfo();
      dataContextInfo.RawDataSource = new RawDataSourceInfo((DocumentNode) documentNode, (string) null).CombineWith(localSource);
      dataContextInfo.SetDataContextOwner(documentNode, dataContextProperty);
      return dataContextInfo;
    }

    public RawDataSourceInfoBase GetRawDataContextInfo(IDataContextAncestorWalker walker)
    {
      RawDataSourceInfoBase rawDataContextInfo = this.GetRawDataContextInfo(walker.CurrentNode);
      if (rawDataContextInfo == null || !rawDataContextInfo.IsValid)
        return rawDataContextInfo;
      return this.UnwindElementNameBindingInternal(walker, rawDataContextInfo);
    }

    public RawDataSourceInfoBase GetRawDataContextInfo(DocumentCompositeNode documentNode)
    {
      if (documentNode == null)
        return (RawDataSourceInfoBase) null;
      IProperty ptoperty = DesignTimeProperties.ResolveDesignTimePropertyKey(DesignTimeProperties.DesignDataContextProperty, documentNode.PlatformMetadata);
      DocumentNode dataSourceNode = documentNode.Properties[(IPropertyId) ptoperty];
      if (dataSourceNode == null)
      {
        ptoperty = DataContextHelper.GetDataContextProperty(documentNode.Type);
        if (ptoperty != null)
          dataSourceNode = documentNode.Properties[(IPropertyId) ptoperty];
      }
      if (dataSourceNode == null)
        return (RawDataSourceInfoBase) null;
      RawDataSourceInfoBase rawDataSourceInfo = DataContextHelper.GetRawDataSourceInfo(dataSourceNode);
      if (rawDataSourceInfo != null && rawDataSourceInfo.IsValid)
        this.OnDataSourceFound(documentNode, ptoperty);
      return rawDataSourceInfo;
    }

    public RawDataSourceInfoBase NavigateSpecialDataContext(IDataContextAncestorWalker walker, RawDataSourceInfoBase localDataSource)
    {
      this.visitedLocations.Reset();
      return this.NavigateSpecialDataContextInternal(walker, localDataSource, true);
    }

    private RawDataSourceInfoBase NavigateSpecialDataContextInternal(IDataContextAncestorWalker walker, RawDataSourceInfoBase localDataSource, bool unwindElementName)
    {
      RawDataSourceInfoBase localDataSource1 = unwindElementName ? this.UnwindElementNameBindingInternal(walker, localDataSource) : localDataSource;
      RawDataSourceInfoBase dataSourceInfoBase;
      do
      {
        dataSourceInfoBase = this.MoveToDataContextProperty(walker, localDataSource1) ?? this.MoveToSpecialDataContextPathExtension(walker, localDataSource1);
        if (dataSourceInfoBase != null)
          localDataSource1 = dataSourceInfoBase;
      }
      while (dataSourceInfoBase != null && dataSourceInfoBase.IsValid);
      return localDataSource1;
    }

    public void MoveUpIfDataContextLocation(IDataContextAncestorWalker walker)
    {
      if (!this.IsDataContextLocation(walker))
        return;
      walker.MoveNext();
    }

    public bool IsDataContextLocation(IDataContextAncestorWalker walker)
    {
      return DataContextHelper.IsDataContextProperty((DocumentNode) walker.CurrentNode, (IPropertyId) walker.CurrentProperty);
    }

    private RawDataSourceInfoBase MoveToDataContextProperty(IDataContextAncestorWalker walker, RawDataSourceInfoBase localDataSource)
    {
      if (walker.CurrentNode == null)
        return (RawDataSourceInfoBase) null;
      DataContextProperty dataContextProperty = DataContextMetadata.GetDataContextProperty(walker.CurrentNode, walker.CurrentProperty);
      if (dataContextProperty == null)
        return (RawDataSourceInfoBase) null;
      if (!dataContextProperty.IsValid)
        return (RawDataSourceInfoBase) RawDataSourceInfo.Invalid;
      if (this.visitedLocations.IsVisited(walker))
        return (RawDataSourceInfoBase) RawDataSourceInfo.Invalid;
      DocumentNode dataContextNode = dataContextProperty.DataContextNode;
      if (dataContextNode == null)
        return (RawDataSourceInfoBase) RawDataSourceInfo.Invalid;
      this.OnDataSourceFound(walker.CurrentNode, walker.CurrentProperty);
      RawDataSourceInfoBase rawDataSourceInfo = DataContextHelper.GetRawDataSourceInfo(dataContextNode);
      if (rawDataSourceInfo.IsValid && dataContextProperty.IsCollectionItem)
        rawDataSourceInfo.AppendIndexStep();
      RawDataSourceInfoBase localDataSource1 = rawDataSourceInfo.CombineWith(localDataSource);
      walker.MoveTo(dataContextProperty.SourceNode, dataContextProperty.Property, true);
      return this.UnwindElementNameBindingInternal(walker, localDataSource1);
    }

    private RawDataSourceInfoBase MoveToSpecialDataContextPathExtension(IDataContextAncestorWalker walker, RawDataSourceInfoBase localDataSource)
    {
      if (walker.CurrentNode == null)
        return (RawDataSourceInfoBase) null;
      DataContextPropertyPathExtension propertyPathExtension1 = DataContextMetadata.GetDataContextPropertyPathExtension(walker.CurrentNode, walker.CurrentProperty);
      if (propertyPathExtension1 == null)
        return (RawDataSourceInfoBase) null;
      string propertyPathExtension2 = this.GetPropertyPathExtension(walker.CurrentNode, walker.CurrentProperty, propertyPathExtension1);
      if (propertyPathExtension2 == null)
        return (RawDataSourceInfoBase) RawDataSourceInfo.Invalid;
      if (this.visitedLocations.IsVisited(walker))
        return (RawDataSourceInfoBase) RawDataSourceInfo.Invalid;
      DocumentNode dataSourceNode = walker.CurrentNode.Properties[(IPropertyId) propertyPathExtension1.Property];
      RawDataSourceInfoBase dataSourceInfoBase;
      if (dataSourceNode == null)
      {
        dataSourceInfoBase = (RawDataSourceInfoBase) RawDataSourceInfo.NewEmpty;
      }
      else
      {
        dataSourceInfoBase = DataContextHelper.GetRawDataSourceInfo(dataSourceNode);
        if (dataSourceInfoBase.IsValid)
          dataSourceInfoBase.AppendClrPath(propertyPathExtension2);
      }
      RawDataSourceInfoBase localDataSource1 = dataSourceInfoBase.CombineWith(localDataSource);
      walker.MoveTo(walker.CurrentNode, propertyPathExtension1.Property, true);
      return this.UnwindElementNameBindingInternal(walker, localDataSource1);
    }

    private string GetPropertyPathExtension(DocumentCompositeNode documentNode, IProperty targetProperty, DataContextPropertyPathExtension pathExtension)
    {
      if (!pathExtension.IsValid)
        return (string) null;
      DocumentNode node = documentNode.Properties[(IPropertyId) targetProperty];
      if (node == null || !PlatformTypes.String.IsAssignableFrom((ITypeId) node.Type))
        return string.Empty;
      string localPath = DocumentPrimitiveNode.GetValueAsString(node);
      if (pathExtension.IsCollectionItem)
        localPath = ClrObjectSchema.CombinePaths(DataSchemaNode.IndexNodePath, localPath);
      return localPath;
    }

    public RawDataSourceInfoBase UnwindElementNameBinding(IDataContextAncestorWalker walker, RawDataSourceInfoBase dataSource)
    {
      this.visitedLocations.Reset();
      return this.UnwindElementNameBindingInternal(walker, dataSource);
    }

    private RawDataSourceInfoBase UnwindElementNameBindingInternal(IDataContextAncestorWalker walker, RawDataSourceInfoBase localDataSource)
    {
      RawDataSourceInfoBase localDataSource1 = localDataSource;
      bool flag = false;
      while (true)
      {
        RawDataSourceInfoBase dataSourceInfoBase = this.MoveToElementNameBinding(walker, localDataSource1 as ElementDataSourceInfo);
        if (dataSourceInfoBase != null)
        {
          localDataSource1 = dataSourceInfoBase;
          if (localDataSource1.IsValid)
            flag = true;
          else
            break;
        }
        else
          goto label_5;
      }
      return localDataSource1;
label_5:
      if (flag)
        return this.NavigateSpecialDataContextInternal(walker, localDataSource1, false);
      return localDataSource1;
    }

    private RawDataSourceInfoBase MoveToElementNameBinding(IDataContextAncestorWalker walker, ElementDataSourceInfo elementBinding)
    {
      if (walker.CurrentNode == null || elementBinding == null || elementBinding.RootTargetProperty == null)
        return (RawDataSourceInfoBase) null;
      if (this.visitedLocations.IsVisited(walker))
        return (RawDataSourceInfoBase) RawDataSourceInfo.Invalid;
      this.OnDataSourceFound(walker.CurrentNode, walker.CurrentProperty);
      if (!walker.MoveTo(elementBinding.RootElement, elementBinding.RootTargetProperty, true))
        return (RawDataSourceInfoBase) null;
      DocumentNode dataSourceNode = elementBinding.RootElement.Properties[(IPropertyId) elementBinding.RootTargetProperty];
      RawDataSourceInfoBase dataSourceInfoBase = dataSourceNode != null ? DataContextHelper.GetRawDataSourceInfo(dataSourceNode) : (RawDataSourceInfoBase) RawDataSourceInfo.NewEmpty;
      if (dataSourceInfoBase.IsValid)
        dataSourceInfoBase.AppendClrPath(elementBinding.NormalizedClrPath);
      return dataSourceInfoBase;
    }

    private void OnDataSourceFound(DocumentCompositeNode documentNode, IProperty ptoperty)
    {
      if (this.evaluatingInfo == null)
        return;
      this.evaluatingInfo.SetDataContextOwner(documentNode, ptoperty);
    }

    public static object GetTemplateDataContext(DocumentNode rootNode, SceneViewModel viewModel)
    {
      if (!PlatformTypes.DataTemplate.IsAssignableFrom((ITypeId) rootNode.Type))
        return (object) null;
      DataContextInfo dataContextInfo = new DataContextEvaluator().Evaluate(viewModel.GetSceneNode(rootNode));
      if (dataContextInfo.DataSource == null || !dataContextInfo.DataSource.IsValidWithSource)
        return (object) null;
      object obj = (object) null;
      if (dataContextInfo.DataSource.Category == DataSourceCategory.Clr)
        obj = DataContextEvaluator.CreateClrDataContext(dataContextInfo.DataSource, viewModel);
      else if (dataContextInfo.DataSource.Category == DataSourceCategory.Xml)
        obj = DataContextEvaluator.CreateXmlDataContext(dataContextInfo.DataSource, viewModel);
      return obj;
    }

    private static object CreateXmlDataContext(DataSourceInfo dataSource, SceneViewModel viewModel)
    {
      DocumentNode sourceNode = dataSource.SourceNode;
      object obj = (object) null;
      using (StandaloneInstanceBuilderContext instanceBuilderContext = new StandaloneInstanceBuilderContext(viewModel.Document.DocumentContext, viewModel.DesignerContext))
      {
        try
        {
          IInstanceBuilder builder = instanceBuilderContext.InstanceBuilderFactory.GetBuilder(sourceNode.TargetType);
          ViewNode viewNode = builder.GetViewNode((IInstanceBuilderContext) instanceBuilderContext, sourceNode);
          obj = (object) (bool) (builder.Instantiate((IInstanceBuilderContext) instanceBuilderContext, viewNode) ? true : false);
        }
        catch
        {
        }
      }
      ReferenceStep referenceStep = (ReferenceStep) viewModel.ProjectContext.ResolveProperty(XmlDataProviderSceneNode.XPathProperty);
      string inheritedXPath = referenceStep.GetValue(obj) as string;
      if (string.IsNullOrEmpty(inheritedXPath))
        inheritedXPath = dataSource.Path;
      else if (!string.IsNullOrEmpty(dataSource.Path))
        inheritedXPath = XmlSchema.CombineXPaths(inheritedXPath, dataSource.Path);
      if (!string.IsNullOrEmpty(inheritedXPath))
        referenceStep.SetValue(obj, (object) inheritedXPath);
      return obj;
    }

    private static object CreateClrDataContext(DataSourceInfo dataSource, SceneViewModel viewModel)
    {
      DocumentNode sourceNode = dataSource.SourceNode;
      if (PlatformTypes.DataTemplate.IsAssignableFrom((ITypeId) sourceNode.Type))
        return (object) null;
      object obj1 = (object) null;
      using (StandaloneInstanceBuilderContext instanceBuilderContext = new StandaloneInstanceBuilderContext(viewModel.Document.DocumentContext, viewModel.DesignerContext))
      {
        try
        {
          IInstanceBuilder builder = instanceBuilderContext.InstanceBuilderFactory.GetBuilder(sourceNode.TargetType);
          ViewNode viewNode = builder.GetViewNode((IInstanceBuilderContext) instanceBuilderContext, sourceNode);
          if (builder.Instantiate((IInstanceBuilderContext) instanceBuilderContext, viewNode))
            obj1 = DataContextEvaluator.GetEvaluatedValue(viewNode.Instance);
        }
        catch
        {
        }
      }
      if (obj1 == null || string.IsNullOrEmpty(dataSource.Path))
        return obj1;
      object instance = obj1;
      try
      {
        IList<ClrPathPart> list1 = ClrPropertyPathHelper.SplitPath(dataSource.Path);
        if (list1 == null)
          return (object) null;
        for (int index = 0; index < list1.Count; ++index)
        {
          if (instance != null)
          {
            Type type = instance.GetType();
            object obj2 = (object) null;
            ClrPathPart clrPathPart = list1[index];
            if (clrPathPart.Category == ClrPathPartCategory.PropertyName)
            {
              PropertyInfo property = type.GetProperty(clrPathPart.Path);
              if (property != (PropertyInfo) null)
                obj2 = property.GetValue(instance, (object[]) null);
            }
            else
            {
              CollectionAdapterDescription adapterDescription = CollectionAdapterDescription.GetAdapterDescription(type);
              if (adapterDescription != null)
              {
                IList list2 = adapterDescription.GetCollectionAdapter(instance) as IList;
                if (list2 != null)
                {
                  int result = 0;
                  if (clrPathPart.Category == ClrPathPartCategory.IndexStep)
                  {
                    if (!int.TryParse(clrPathPart.Path.Trim('[', ']'), out result))
                      goto label_23;
                  }
                  obj2 = list2[result];
                }
              }
            }
label_23:
            instance = obj2;
          }
          else
            break;
        }
      }
      catch
      {
        instance = (object) null;
      }
      return instance;
    }

    private static object GetEvaluatedValue(object value)
    {
      MarkupExtension markupExtension = value as MarkupExtension;
      if (markupExtension != null)
        return markupExtension.ProvideValue((IServiceProvider) null);
      ObjectDataProvider objectDataProvider = value as ObjectDataProvider;
      if (objectDataProvider != null)
        return objectDataProvider.ObjectInstance ?? (object) null;
      return value;
    }

    private class VisitedLocations
    {
      private List<KeyValuePair<DocumentNode, IProperty>> locations;
      private int visitedCount;

      public bool IsVisited(IDataContextAncestorWalker walker)
      {
        return this.IsVisited((DocumentNode) walker.CurrentNode, walker.CurrentProperty);
      }

      public bool IsVisited(DocumentNode documentNode, IProperty property)
      {
        if (++this.visitedCount < 3)
          return false;
        if (this.locations == null)
          this.locations = new List<KeyValuePair<DocumentNode, IProperty>>();
        KeyValuePair<DocumentNode, IProperty> keyValuePair = new KeyValuePair<DocumentNode, IProperty>(documentNode, property);
        if (this.locations.Contains(keyValuePair))
          return true;
        this.locations.Add(keyValuePair);
        return false;
      }

      public void Reset()
      {
        this.visitedCount = 0;
        if (this.locations == null)
          return;
        this.locations.Clear();
      }
    }
  }
}
