// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.DataBindingDragDropHandler
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.UserInterface.DataPane;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal abstract class DataBindingDragDropHandler
  {
    public DataBindingDragDropModel DragModel { get; protected set; }

    public IProjectContext ProjectContext
    {
      get
      {
        if (this.DragModel != null && this.DragModel.TargetNode != null)
          return this.DragModel.TargetNode.ProjectContext;
        return (IProjectContext) null;
      }
    }

    public abstract bool CanHandle();

    public abstract bool Handle(Point artboardSnappedDropPoint);

    public IDisposable InitDragModel(DataBindingDragDropModel dragModel)
    {
      return (IDisposable) new DataBindingDragDropHandler.DataModelSetter(this, dragModel);
    }

    protected bool UpdateRelativeSchemaPath()
    {
      DataSchemaNodePath primaryAbsolutePath = this.DragModel.DataSource.PrimaryAbsolutePath;
      this.DragModel.RelativeDropSchemaPath = primaryAbsolutePath;
      DataContextInfo dataContextInfo = new DataContextEvaluator().Evaluate(this.DragModel.TargetNode);
      if (dataContextInfo != null && dataContextInfo.DataSource.IsValidWithSource)
      {
        ISchema schema = primaryAbsolutePath.Schema;
        if (DesignDataHelper.CompareDataSources(dataContextInfo.DataSource.SourceNode, schema.DataSource.DocumentNode))
        {
          DataSchemaNodePath nodePathFromPath = schema.GetNodePathFromPath(dataContextInfo.DataSource.Path);
          if (nodePathFromPath == null)
            return true;
          if (nodePathFromPath.IsMethod)
            return false;
          bool flag = nodePathFromPath.IsSubpathOf(this.DragModel.DataSource[0].AbsolutePath);
          for (int index = 1; index < this.DragModel.DataSource.Count; ++index)
          {
            if (nodePathFromPath.IsSubpathOf(this.DragModel.DataSource[index].AbsolutePath) != flag)
              return false;
          }
          if (flag)
            this.DragModel.RelativeDropSchemaPath = nodePathFromPath.GetRelativeNodePath(primaryAbsolutePath);
        }
      }
      return true;
    }

    protected BaseFrameworkElement GetReusableDetailsContainer(bool setDataContextIfMissing, bool canCheckParent)
    {
      SceneElement sceneElement = this.DragModel.TargetNode as SceneElement;
      if (sceneElement == null)
        return (BaseFrameworkElement) null;
      if (!this.IsSupportedDetailsContainer(sceneElement) && canCheckParent)
      {
        sceneElement = sceneElement.Parent as SceneElement;
        if (sceneElement == null || !this.IsSupportedDetailsContainer(sceneElement))
          return (BaseFrameworkElement) null;
      }
      DataContextInfo dataContextInfo = new DataContextEvaluator().Evaluate((SceneNode) sceneElement);
      if (dataContextInfo != null && dataContextInfo.RawDataSource != null)
      {
        DataSourceInfo other = new DataSourceInfo(this.DragModel.DetailsContainerSchemaPath);
        if (dataContextInfo.DataSource.CompareSources(other) == DataSourceMatchCriteria.Exact)
          return (BaseFrameworkElement) sceneElement;
      }
      DocumentCompositeNode documentCompositeNode = (DocumentCompositeNode) sceneElement.DocumentNode;
      if (documentCompositeNode.Properties[DesignTimeProperties.DesignDataContextProperty] != null || documentCompositeNode.Properties[BaseFrameworkElement.DataContextProperty] != null)
        return (BaseFrameworkElement) null;
      if (DataBindingDragDropHandler.ContainsDescendantsWithSourcelessBindings((DocumentNode) documentCompositeNode))
        return (BaseFrameworkElement) null;
      if (setDataContextIfMissing)
      {
        this.DragModel.ViewModel.BindingEditor.CreateAndSetBindingOrData((SceneNode) sceneElement, DesignTimeProperties.DesignDataContextProperty, this.DragModel.DetailsContainerSchemaPath);
        this.LinkDetailsWithMasterControl(sceneElement);
      }
      return (BaseFrameworkElement) sceneElement;
    }

    protected bool IsSupportedDetailsContainer(SceneElement container)
    {
      return PlatformTypes.Panel.IsAssignableFrom((ITypeId) container.Type) || this.DragModel.CheckDropFlags(DataBindingDragDropFlags.SetBinding) && (PlatformTypes.Border.IsAssignableFrom((ITypeId) container.Type) || PlatformTypes.Decorator.IsAssignableFrom((ITypeId) container.Type));
    }

    protected void LinkDetailsWithMasterControl(SceneElement detailsContainerElement)
    {
      BaseFrameworkElement masterControl = this.FindMasterControl(detailsContainerElement);
      if (masterControl == null)
        return;
      masterControl.EnsureNamed();
      BindingSceneNode binding = (BindingSceneNode) this.DragModel.ViewModel.CreateSceneNode(PlatformTypes.Binding);
      binding.ElementName = masterControl.Name;
      binding.SetPath("SelectedItem");
      detailsContainerElement.SetBinding(BaseFrameworkElement.DataContextProperty, binding);
    }

    protected BaseFrameworkElement FindMasterControl(SceneElement detailsContainerElement)
    {
      DataContextEvaluator evaluator = new DataContextEvaluator();
      DataSourceInfo targetDataSource = new DataSourceInfo(this.DragModel.DetailsContainerSchemaPath);
      ITypeId[] typeIdArray = new ITypeId[3]
      {
        PlatformTypes.Selector,
        ProjectNeutralTypes.DataGrid,
        ProjectNeutralTypes.TreeView
      };
      List<IProperty> properties = new List<IProperty>(typeIdArray.Length);
      for (int index = 0; index < typeIdArray.Length; ++index)
      {
        IProperty property = (IProperty) this.ProjectContext.ResolveType(typeIdArray[index]).GetMember(MemberType.LocalProperty, "SelectedItem", MemberAccessTypes.Public);
        if (property != null && !properties.Contains(property))
          properties.Add(property);
      }
      return this.FindMasterControl(this.DragModel.ViewModel.ActiveEditingContainer, properties, detailsContainerElement, targetDataSource, evaluator);
    }

    protected BaseFrameworkElement FindMasterControl(SceneNode sceneNode, List<IProperty> properties, SceneElement detailsContainerElement, DataSourceInfo targetDataSource, DataContextEvaluator evaluator)
    {
      if (sceneNode == null || sceneNode == detailsContainerElement)
        return (BaseFrameworkElement) null;
      for (int index = 0; index < properties.Count; ++index)
      {
        if (this.IsMasterControl(sceneNode, properties[index], targetDataSource, evaluator))
          return (BaseFrameworkElement) sceneNode;
      }
      IProperty defaultContentProperty = sceneNode.Type.Metadata.DefaultContentProperty;
      if (defaultContentProperty == null)
        return (BaseFrameworkElement) null;
      foreach (SceneNode sceneNode1 in (IEnumerable<SceneNode>) sceneNode.GetCollectionForProperty((IPropertyId) defaultContentProperty))
      {
        BaseFrameworkElement masterControl = this.FindMasterControl((SceneNode) (sceneNode1 as SceneElement), properties, detailsContainerElement, targetDataSource, evaluator);
        if (masterControl != null)
          return masterControl;
      }
      return (BaseFrameworkElement) null;
    }

    protected bool IsMasterControl(SceneNode sceneNode, IProperty selectedItemProperty, DataSourceInfo targetDataSource, DataContextEvaluator evaluator)
    {
      if (!selectedItemProperty.DeclaringType.IsAssignableFrom((ITypeId) sceneNode.Type))
        return false;
      DataContextInfo dataContextInfo = evaluator.Evaluate(sceneNode, (IPropertyId) selectedItemProperty, false);
      return targetDataSource.CompareSources(dataContextInfo.DataSource) == DataSourceMatchCriteria.Exact;
    }

    protected static bool ContainsDescendantsWithSourcelessBindings(DocumentNode documentNode)
    {
      DocumentCompositeNode documentCompositeNode = documentNode as DocumentCompositeNode;
      if (documentCompositeNode == null)
        return false;
      if (documentCompositeNode.Type.IsBinding)
        return documentCompositeNode.Properties[BindingSceneNode.SourceProperty] == null;
      foreach (DocumentNode documentNode1 in documentCompositeNode.ChildNodes)
      {
        if (DataBindingDragDropHandler.ContainsDescendantsWithSourcelessBindings(documentNode1))
          return true;
      }
      return false;
    }

    protected class DataModelSetter : IDisposable
    {
      protected DataBindingDragDropHandler handler;

      public DataModelSetter(DataBindingDragDropHandler handler, DataBindingDragDropModel dragModel)
      {
        this.handler = handler;
        this.handler.DragModel = dragModel;
      }

      public void Dispose()
      {
        this.handler = (DataBindingDragDropHandler) null;
        GC.SuppressFinalize((object) this);
      }
    }
  }
}
