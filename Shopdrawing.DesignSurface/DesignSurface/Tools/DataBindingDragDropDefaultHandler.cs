// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.DataBindingDragDropDefaultHandler
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Tools.Assets;
using Microsoft.Expression.DesignSurface.UserInterface.DataPane;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal class DataBindingDragDropDefaultHandler : DataBindingDragDropHandler
  {
    private static readonly string SilverlightTreeControlAssemblyName = "System.Windows.Controls";
    private static readonly ITypeId TreeControlType = ProjectNeutralTypes.TreeView;
    private static readonly ITypeId ItemsControlType = PlatformTypes.ListBox;
    private static readonly IPropertyId HierarchicalItemsSourceProperty = (IPropertyId) ProjectNeutralTypes.HierarchicalDataTemplate.GetMember(MemberType.LocalProperty, "ItemsSource", MemberAccessTypes.Public);

    public override bool CanHandle()
    {
      this.DragModel.DropFlags = DataBindingDragDropFlags.None;
      if (this.DragModel.DataSource.FindIndex((Predicate<DataSchemaNodePath>) (schemaPath => schemaPath.IsMethod)) >= 0 || !this.DragModel.DataSource.IsSingleSchema || (!this.DragModel.DataSource.IsCommonEffectiveCollectionNode || !this.DragModel.DataSource.IsCommonNodeType))
        return false;
      IType type = this.DragModel.TargetNode.Type;
      if (PlatformTypes.FrameworkTemplate.IsAssignableFrom((ITypeId) type) || PlatformTypes.Style.IsAssignableFrom((ITypeId) type) || !this.UpdateRelativeSchemaPath())
        return false;
      bool flag = false;
      if (DataBindingModeModel.Instance.NormalizedMode == DataBindingMode.Default)
        flag = this.UpdateDragModelDefault();
      else if (DataBindingModeModel.Instance.NormalizedMode == DataBindingMode.Details)
        flag = this.UpdateDragModelDetails();
      if (flag)
      {
        this.Disambiguate();
        this.UpdateTooltip();
      }
      return flag;
    }

    public override bool Handle(Point artboardSnappedDropPoint)
    {
      return !this.DragModel.CheckDropFlags(DataBindingDragDropFlags.CreateElement) ? this.SetBindingOnExistingElement() : (DataBindingModeModel.Instance.NormalizedMode != DataBindingMode.Details ? (this.DragModel.NewElementType == DataBindingDragDropDefaultHandler.ItemsControlType || this.DragModel.NewElementType == DataBindingDragDropDefaultHandler.TreeControlType ? this.CreateItemsControl(artboardSnappedDropPoint) : this.CreateControls(artboardSnappedDropPoint)) : this.CreateDetailsVisualization(artboardSnappedDropPoint));
    }

    private void Disambiguate()
    {
      if (this.DragModel.CheckDropFlags(DataBindingDragDropFlags.SetBinding) && !this.DragModel.ViewModel.BindingEditor.CanCreateAndSetBindingOrData(this.DragModel.TargetNode, (IPropertyId) this.DragModel.TargetProperty, this.DragModel.DataSource.PrimaryAbsolutePath, true))
        this.DragModel.DropFlags &= ~DataBindingDragDropFlags.SetBinding;
      this.DragModel.DropFlags &= this.DragModel.DragFlags;
      DataBindingDragDropFlags flags = DataBindingDragDropFlags.SetBinding | DataBindingDragDropFlags.CreateElement;
      if (!this.DragModel.CheckDropFlags(flags))
        return;
      DataBindingDragDropFlags bindingDragDropFlags = this.DragModel.InsertionPoint.InsertIndex < 0 ? (this.DragModel.CheckDragFlags(DataBindingDragDropFlags.AutoPickProperty) ? DataBindingDragDropFlags.CreateElement : DataBindingDragDropFlags.SetBinding) : DataBindingDragDropFlags.CreateElement;
      this.DragModel.DropFlags &= ~flags;
      this.DragModel.DropFlags |= bindingDragDropFlags;
    }

    private bool UpdateDragModelDefault()
    {
      int collectionDepth = this.DragModel.RelativeDropSchemaPath.CollectionDepth;
      if (collectionDepth > 1 || collectionDepth > 0 && this.DragModel.RelativeDropSchemaPath.IsMethod)
        return false;
      DataBindingDragDropFlags dragFlags = this.DragModel.DragFlags;
      if (this.DragModel.DataSource.Count == 1 && (collectionDepth == 0 || this.DragModel.RelativeDropSchemaPath.IsCollection))
        this.DragModel.DropFlags |= DataBindingDragDropFlags.SetBinding;
      else if (this.DragModel.DataSource.Count > 1 && collectionDepth == 0)
        dragFlags &= ~(DataBindingDragDropFlags.SetBinding | DataBindingDragDropFlags.AutoPickProperty);
      DataSchemaNode effectiveCollectionNode = this.DragModel.RelativeDropSchemaPath.EffectiveCollectionNode;
      bool flag = effectiveCollectionNode != null && effectiveCollectionNode != this.DragModel.RelativeDropSchemaPath.Node;
      if (flag)
        this.DragModel.RelativeDropSchemaPath = new DataSchemaNodePath(this.DragModel.RelativeDropSchemaPath.Schema, effectiveCollectionNode);
      this.ApplyRules(dragFlags);
      if (this.DragModel.TargetPropertySpecialDataContext != null)
        this.DragModel.RelativeDropSchemaPath = this.DragModel.TargetPropertySpecialDataContext;
      else if (flag && this.DragModel.CheckDropFlags(DataBindingDragDropFlags.SetBinding) && !this.ShouldGenerateDetails(this.DragModel.TargetProperty))
      {
        this.DragModel.TargetProperty = (IProperty) null;
        this.DragModel.DropFlags &= ~DataBindingDragDropFlags.SetBinding;
      }
      return true;
    }

    private bool UpdateDragModelDetails()
    {
      this.DragModel.RelativeDropSchemaPath = this.DragModel.DataSource.PrimaryAbsolutePath;
      if (this.DragModel.RelativeDropSchemaPath.IsCollection)
        this.DragModel.RelativeDropSchemaPath = this.DragModel.RelativeDropSchemaPath.GetExtendedPath(this.DragModel.RelativeDropSchemaPath.EffectiveCollectionItemNode);
      if (this.DragModel.DataSource.Count == 1)
      {
        this.DragModel.DropFlags |= DataBindingDragDropFlags.SetBinding;
        this.ApplyRules(this.DragModel.DragFlags & ~DataBindingDragDropFlags.CreateElement);
        if (BaseFrameworkElement.DataContextProperty.Equals((object) this.DragModel.TargetProperty))
          this.DragModel.TargetProperty = this.ProjectContext.ResolveProperty(DesignTimeProperties.DesignDataContextProperty);
      }
      this.ApplyRules(this.DragModel.DragFlags & DataBindingDragDropFlags.CreateElement);
      return true;
    }

    private void ApplyRules(DataBindingDragDropFlags filteredDragFlags)
    {
      this.ApplyRulesInternal(this.DragModel, filteredDragFlags);
      if (!this.ShouldTryDropOnAncestorPanel(filteredDragFlags))
        return;
      DataBindingDragDropModel ancestorPanelModel = this.DragModel.AncestorPanelModel;
      if (ancestorPanelModel == null || ancestorPanelModel == this.DragModel)
        return;
      DataBindingDragDropFlags filteredDragFlags1 = filteredDragFlags & ~DataBindingDragDropFlags.SetBinding;
      this.ApplyRulesInternal(ancestorPanelModel, filteredDragFlags1);
      if (!ancestorPanelModel.CheckDropFlags(DataBindingDragDropFlags.CreateElement))
        return;
      this.DragModel = ancestorPanelModel;
    }

    private bool ShouldTryDropOnAncestorPanel(DataBindingDragDropFlags filteredDragFlags)
    {
      return (filteredDragFlags & DataBindingDragDropFlags.CreateElement) != DataBindingDragDropFlags.None && this.DragModel.CheckDragFlags(DataBindingDragDropFlags.AllowRetargetElement) && (this.DragModel.DropFlags == DataBindingDragDropFlags.None || this.DragModel.DataSource.PrimarySchemaNodePath.IsCollection && !this.DragModel.CheckDropFlags(DataBindingDragDropFlags.CreateElement) && (this.DragModel.TargetProperty == null || FrameworkElement.DataContextProperty.Equals((object) this.DragModel.TargetProperty)));
    }

    private void ApplyRulesInternal(DataBindingDragDropModel model, DataBindingDragDropFlags filteredDragFlags)
    {
      if ((filteredDragFlags & DataBindingDragDropFlags.SetBinding) == DataBindingDragDropFlags.SetBinding)
      {
        if (model.InsertionPoint.Property != null)
        {
          model.DropFlags &= ~DataBindingDragDropFlags.SetBinding;
          IType type = model.RelativeDropSchemaPath.Type;
          if (type != null && BindingPropertyHelper.GetPropertyCompatibility(model.InsertionPoint.Property, type, (ITypeResolver) model.TargetNode.ProjectContext) != BindingPropertyCompatibility.None)
          {
            model.TargetProperty = model.InsertionPoint.Property;
            model.DropFlags |= DataBindingDragDropFlags.SetBinding;
          }
        }
        else if ((filteredDragFlags & DataBindingDragDropFlags.AutoPickProperty) == DataBindingDragDropFlags.AutoPickProperty)
        {
          model.TargetProperty = this.GetSpecialDefaultBindingProperty(filteredDragFlags);
          if (model.TargetProperty == null)
            model.TargetProperty = this.GetDefaultBindingProperty(filteredDragFlags, model.RelativeDropSchemaPath);
          if (model.TargetProperty != null)
            model.DropFlags |= DataBindingDragDropFlags.SetBinding;
        }
      }
      if ((filteredDragFlags & DataBindingDragDropFlags.CreateElement) != DataBindingDragDropFlags.CreateElement || !PlatformTypes.Panel.IsAssignableFrom((ITypeId) model.TargetNode.Type))
        return;
      if (DataBindingModeModel.Instance.NormalizedMode == DataBindingMode.Default && model.RelativeDropSchemaPath.IsCollection)
      {
        model.NewElementType = !model.RelativeDropSchemaPath.IsHierarchicalCollection ? DataBindingDragDropDefaultHandler.ItemsControlType : DataBindingDragDropDefaultHandler.TreeControlType;
        model.NewElementProperty = (IPropertyId) model.NewElementType.GetMember(MemberType.LocalProperty, "ItemsSource", MemberAccessTypes.Public);
        model.DropFlags |= DataBindingDragDropFlags.CreateElement;
      }
      List<DataSchemaNode> toCreateElements = this.GetNodesToCreateElements();
      if (toCreateElements.Count < model.DataSource.Count)
        return;
      model.DropFlags |= DataBindingDragDropFlags.CreateElement;
      model.NodesToCreateElements = (IList<DataSchemaNode>) toCreateElements;
      if (model.NewElementType != null)
        return;
      IType dataType = toCreateElements[0].ResolveType(model.TargetNode.DocumentNode.TypeResolver);
      DataViewTemplateEntry viewTemplateEntry = DataViewFactory.GetDataViewTemplateEntry(model.Platform, dataType, DataViewCategory.Master);
      model.NewElementType = (ITypeId) viewTemplateEntry.FieldNode.Type;
      model.NewElementProperty = (IPropertyId) viewTemplateEntry.FieldValueProperty;
    }

    private List<DataSchemaNode> GetNodesToCreateElements()
    {
      IPlatform platform = this.DragModel.Platform;
      List<DataSchemaNode> list = new List<DataSchemaNode>();
      bool flag = DataBindingModeModel.Instance.NormalizedMode == DataBindingMode.Default;
      DataViewCategory category = flag ? DataViewCategory.Master : DataViewCategory.Details;
      if (flag && this.DragModel.DataSource.Count == 1 && this.DragModel.DataSource.PrimarySchemaNodePath.IsCollection)
      {
        DataSchemaNode collectionItemNode = this.DragModel.DataSource.PrimarySchemaNodePath.EffectiveCollectionItemNode;
        IType type = collectionItemNode.ResolveType(this.DragModel.TargetNode.DocumentNode.TypeResolver);
        if (PlatformTypes.IConvertible.IsAssignableFrom((ITypeId) type) || PlatformTypes.ImageSource.IsAssignableFrom((ITypeId) type))
        {
          list.Add(collectionItemNode);
        }
        else
        {
          foreach (DataSchemaNode dataSchemaNode in collectionItemNode.Children)
          {
            IType dataType = dataSchemaNode.ResolveType(this.DragModel.TargetNode.DocumentNode.TypeResolver);
            if (DataViewFactory.GetDataViewTemplateEntry(platform, dataType, category) != null)
              list.Add(dataSchemaNode);
          }
        }
      }
      else
      {
        for (int index = 0; index < this.DragModel.DataSource.Count; ++index)
        {
          DataSchemaNodePath dataSchemaNodePath = this.DragModel.DataSource[index];
          if (DataViewFactory.GetDataViewTemplateEntry(platform, dataSchemaNodePath.Type, category) != null)
            list.Add(dataSchemaNodePath.Node);
        }
      }
      list.Sort((Comparison<DataSchemaNode>) ((a, b) => StringLogicalComparer.Instance.Compare(a.PathName, b.PathName)));
      return list;
    }

    private IProperty GetSpecialDefaultBindingProperty(DataBindingDragDropFlags filteredDragFlags)
    {
      DataSchemaNodePath primaryAbsolutePath = this.DragModel.DataSource.PrimaryAbsolutePath;
      IProperty defaultBindingProperty = this.GetDefaultBindingProperty(filteredDragFlags, primaryAbsolutePath);
      if (!DataContextMetadata.HasDataContextAttributes(defaultBindingProperty))
        return (IProperty) null;
      DataContextInfo dataContextInfo = new DataContextEvaluator().Evaluate(this.DragModel.TargetNode, (IPropertyId) defaultBindingProperty, true);
      if (dataContextInfo.DataSource.SourceNode != primaryAbsolutePath.Schema.DataSource.DocumentNode)
        return (IProperty) null;
      DataSchemaNodePath nodePathFromPath = primaryAbsolutePath.Schema.GetNodePathFromPath(dataContextInfo.DataSource.Path);
      if (!nodePathFromPath.IsSubpathOf(primaryAbsolutePath))
        return (IProperty) null;
      DataSchemaNodePath relativeNodePath = nodePathFromPath.GetRelativeNodePath(primaryAbsolutePath);
      if (relativeNodePath.CollectionDepth != 0)
        return (IProperty) null;
      this.DragModel.TargetPropertySpecialDataContext = relativeNodePath;
      return defaultBindingProperty;
    }

    private IProperty GetDefaultBindingProperty(DataBindingDragDropFlags filteredDragFlags, DataSchemaNodePath schemaPath)
    {
      IType type = schemaPath.Type;
      IProperty property;
      if (!schemaPath.IsCollection || schemaPath.Node.Type == (Type) null || !(schemaPath.Schema is XmlSchema))
      {
        property = this.GetDefaultBindingProperty(filteredDragFlags, type);
      }
      else
      {
        IType dataType = this.ProjectContext.ResolveType(PlatformTypes.ICollection);
        property = !type.IsAssignableFrom((ITypeId) dataType) ? this.GetDefaultBindingProperty(filteredDragFlags, dataType) ?? this.GetDefaultBindingProperty(filteredDragFlags, type) : this.GetDefaultBindingProperty(filteredDragFlags, type);
      }
      return property;
    }

    private IProperty GetDefaultBindingProperty(DataBindingDragDropFlags filteredDragFlags, IType dataType)
    {
      if (dataType == null)
        return (IProperty) null;
      BindingPropertyMatchInfo bindingPropertyInfo = BindingPropertyHelper.GetDefaultBindingPropertyInfo(this.DragModel.TargetNode, dataType);
      if (bindingPropertyInfo.Property == null)
        return (IProperty) null;
      if (bindingPropertyInfo.Compatibility == BindingPropertyCompatibility.StringSpecial && !PlatformTypes.IConvertible.IsAssignableFrom((ITypeId) dataType))
        return (IProperty) null;
      if (bindingPropertyInfo.Compatibility == BindingPropertyCompatibility.DataContext)
      {
        if ((filteredDragFlags & DataBindingDragDropFlags.DiscourageDataContext) == DataBindingDragDropFlags.DiscourageDataContext)
          return (IProperty) null;
        SceneElement sceneElement = this.DragModel.TargetNode as SceneElement;
        if (sceneElement == null)
          return (IProperty) null;
        if (!sceneElement.IsContainer && !PlatformTypes.UserControl.IsAssignableFrom((ITypeId) sceneElement.Type) && (sceneElement.Parent != null && !PlatformTypes.FrameworkTemplate.IsAssignableFrom((ITypeId) sceneElement.Parent.Type)))
          return (IProperty) null;
        if (PlatformTypes.IConvertible.IsAssignableFrom((ITypeId) dataType))
          return (IProperty) null;
      }
      if (bindingPropertyInfo.Compatibility == BindingPropertyCompatibility.Assignable && bindingPropertyInfo.Property.PropertyType.RuntimeType == typeof (object) && !PlatformTypes.IConvertible.IsAssignableFrom((ITypeId) dataType))
        return (IProperty) null;
      if (PlatformTypes.IEnumerable.IsAssignableFrom((ITypeId) bindingPropertyInfo.Property.PropertyType) && !PlatformTypes.String.IsAssignableFrom((ITypeId) bindingPropertyInfo.Property.PropertyType) && PlatformTypes.String.IsAssignableFrom((ITypeId) dataType))
        return (IProperty) null;
      return bindingPropertyInfo.Property;
    }

    private bool CreateItemsControl(Point dropPoint)
    {
      if (this.DragModel.NewElementType == DataBindingDragDropDefaultHandler.TreeControlType && !this.EnsureTreeControlTypeAssembly())
        return false;
      BaseFrameworkElement child1 = (BaseFrameworkElement) null;
      using (SceneEditTransaction editTransaction = this.DragModel.Document.CreateEditTransaction(StringTable.UndoUnitDragDropCreateDataboundControl))
      {
        Rect rect = new Rect(dropPoint, new Point(double.PositiveInfinity, double.PositiveInfinity));
        child1 = (BaseFrameworkElement) new UserThemeTypeInstantiator(this.DragModel.ViewModel.DefaultView).CreateInstance(this.DragModel.NewElementType, (ISceneInsertionPoint) this.DragModel.InsertionPoint, rect, (OnCreateInstanceAction) null);
        double width = child1.Width;
        double height = child1.Height;
        if (!double.IsNaN(width) || !double.IsNaN(height))
        {
          BaseFrameworkElement child2 = (BaseFrameworkElement) this.DragModel.InsertionPoint.SceneNode;
          Rect childRect = this.DragModel.ViewModel.GetLayoutDesignerForChild((SceneElement) child2, true).GetChildRect(child2);
          rect.Width = Math.Min(childRect.Width, 200.0);
          rect.Height = Math.Min(childRect.Height, 300.0);
          if (rect.Width != width || rect.Height != height)
            this.DragModel.ViewModel.GetLayoutDesignerForChild((SceneElement) child1, true).SetChildRect(child1, rect);
        }
        IProperty targetProperty = this.ProjectContext.ResolveProperty(this.DragModel.NewElementProperty);
        if (this.SetBinding((SceneNode) child1, ref targetProperty))
        {
          editTransaction.Commit();
        }
        else
        {
          child1 = (BaseFrameworkElement) null;
          editTransaction.Cancel();
        }
      }
      return child1 != null;
    }

    private bool EnsureTreeControlTypeAssembly()
    {
      if (this.ProjectContext.IsCapabilitySet(PlatformCapability.IsWpf))
        return true;
      ProjectXamlContext projectXamlContext = ProjectXamlContext.FromProjectContext(this.ProjectContext);
      if (projectXamlContext == null)
        return false;
      projectXamlContext.EnsureAssemblyReferenced(DataBindingDragDropDefaultHandler.SilverlightTreeControlAssemblyName);
      this.DragModel.TargetNode.DesignerContext.ViewUpdateManager.RebuildPostponedViews();
      return !this.ProjectContext.PlatformMetadata.IsNullType((ITypeId) this.ProjectContext.ResolveType(DataBindingDragDropDefaultHandler.TreeControlType));
    }

    private bool CreateControls(Point dropPoint)
    {
      using (SceneEditTransaction editTransaction = this.DragModel.Document.CreateEditTransaction(StringTable.UndoUnitDragDropCreateDataboundControl))
      {
        DataContextInfo dataContextInfo = new DataContextEvaluator().Evaluate(this.DragModel.TargetNode);
        DataSourceInfo other = new DataSourceInfo(new DataSchemaNodePath(this.DragModel.DataSource.PrimaryAbsoluteSchema, this.GetRelativeSchema().Root));
        bool flag = false;
        if (dataContextInfo.DataSource == null || !dataContextInfo.DataSource.IsValidWithSource || dataContextInfo.DataSource.CompareSources(other) != DataSourceMatchCriteria.Exact)
          flag = true;
        IList<DataSchemaNodePath> relativeSchemaPaths = this.GetRelativeSchemaPaths();
        CanvasLikeDataViewLayoutBuilder viewLayoutBuilder = new CanvasLikeDataViewLayoutBuilder(this.DragModel.InsertionPoint.InsertIndex, false);
        DocumentCompositeNode containerNode = (DocumentCompositeNode) this.DragModel.TargetNode.DocumentNode;
        DataViewFactory.GenerateDataView(this.DragModel.Platform, this.DragModel.DocumentContext, relativeSchemaPaths, DataViewCategory.Master, containerNode, (IDataViewLayoutBuilder) viewLayoutBuilder);
        if (flag)
          viewLayoutBuilder.RebindFields(this.DragModel.ViewModel, this.DragModel.DataSource.PrimaryAbsoluteSchema, relativeSchemaPaths);
        editTransaction.Update();
        viewLayoutBuilder.ApplyActiveUserThemeStyle(this.DragModel.ViewModel);
        viewLayoutBuilder.CompleteLayout(this.DragModel.ViewModel, dropPoint);
        viewLayoutBuilder.SelectElements(this.DragModel.ViewModel);
        editTransaction.Commit();
      }
      return true;
    }

    private bool CreateDetailsVisualization(Point dropPoint)
    {
      using (SceneEditTransaction editTransaction = this.DragModel.Document.CreateEditTransaction(StringTable.UndoUnitDragDropCreateDataboundControl))
      {
        bool flag = true;
        DocumentCompositeNode containerNode = (DocumentCompositeNode) null;
        BaseFrameworkElement child = this.GetReusableDetailsContainer(true, this.DragModel.CheckDropFlags(DataBindingDragDropFlags.SetBinding));
        if (child != null)
        {
          containerNode = (DocumentCompositeNode) child.DocumentNode;
          flag = false;
        }
        IList<DataSchemaNodePath> relativeSchemaPaths = this.GetRelativeSchemaPaths();
        bool shouldCreateLabels = true;
        if (relativeSchemaPaths.Count == 1 && this.DragModel.DataSource.Count == 1 && relativeSchemaPaths[0].Node == this.DragModel.DataSource.PrimarySchemaNodePath.Node)
          shouldCreateLabels = false;
        CanvasLikeDataViewLayoutBuilder viewLayoutBuilder = new CanvasLikeDataViewLayoutBuilder(this.DragModel.InsertionPoint.InsertIndex, shouldCreateLabels);
        DocumentCompositeNode documentCompositeNode = DataViewFactory.GenerateDataView(this.DragModel.Platform, this.DragModel.DocumentContext, relativeSchemaPaths, DataViewCategory.Details, containerNode, (IDataViewLayoutBuilder) viewLayoutBuilder);
        if (flag)
        {
          this.DragModel.ViewModel.ElementSelectionSet.Clear();
          child = (BaseFrameworkElement) this.DragModel.ViewModel.GetSceneNode((DocumentNode) documentCompositeNode);
          this.DragModel.InsertionPoint.Insert((SceneNode) child);
          this.DragModel.ViewModel.BindingEditor.CreateAndSetBindingOrData((SceneNode) child, DesignTimeProperties.DesignDataContextProperty, this.DragModel.DetailsContainerSchemaPath);
        }
        editTransaction.Update();
        viewLayoutBuilder.ApplyActiveUserThemeStyle(this.DragModel.ViewModel);
        if (flag)
        {
          Size size = viewLayoutBuilder.CalcCombinedSize(this.DragModel.ViewModel);
          Rect rect = new Rect(dropPoint, new Size()
          {
            Width = Math.Max(size.Width, 200.0),
            Height = Math.Max(size.Height, 200.0)
          });
          this.DragModel.ViewModel.GetLayoutDesignerForChild((SceneElement) child, true).SetChildRect(child, rect);
          editTransaction.Update();
          this.DragModel.ViewModel.ElementSelectionSet.SetSelection((SceneElement) child);
          dropPoint = new Point();
        }
        viewLayoutBuilder.CompleteLayout(this.DragModel.ViewModel, dropPoint);
        if (!flag)
          viewLayoutBuilder.SelectElements(this.DragModel.ViewModel);
        this.LinkDetailsWithMasterControl((SceneElement) child);
        editTransaction.Commit();
      }
      return true;
    }

    private bool SetBindingOnExistingElement()
    {
      bool flag = false;
      using (SceneEditTransaction editTransaction = this.DragModel.Document.CreateEditTransaction(StringTable.UndoUnitDragDropCreateDataboundControl))
      {
        if (DataBindingModeModel.Instance.NormalizedMode == DataBindingMode.Details)
          this.GetReusableDetailsContainer(true, this.DragModel.CheckDropFlags(DataBindingDragDropFlags.SetBinding));
        IProperty targetProperty = this.DragModel.TargetProperty;
        if (this.SetBinding(this.DragModel.TargetNode, ref targetProperty))
        {
          if (DataBindingModeModel.Instance.NormalizedMode == DataBindingMode.Details && this.DragModel.InsertionPoint.SceneElement != null && DesignTimeProperties.DesignDataContextProperty.Equals((object) targetProperty))
          {
            DocumentCompositeNode documentCompositeNode = this.DragModel.TargetNode.DocumentNode as DocumentCompositeNode;
            if (documentCompositeNode != null && documentCompositeNode.Properties[BaseFrameworkElement.DataContextProperty] == null)
              this.LinkDetailsWithMasterControl(this.DragModel.InsertionPoint.SceneElement);
          }
          editTransaction.Commit();
          this.DragModel.OnUserSelectedProperty(targetProperty);
          flag = true;
        }
        else
          editTransaction.Cancel();
      }
      return flag;
    }

    private bool SetBinding(SceneNode targetNode, ref IProperty targetProperty)
    {
      ReferenceStep targetProperty1 = (ReferenceStep) null;
      if (targetProperty != null)
        targetProperty1 = this.DragModel.DocumentContext.TypeResolver.ResolveProperty((IPropertyId) targetProperty) as ReferenceStep;
      DataSchemaNodePath bindingPath = new DataSchemaNodePath(this.DragModel.DataSource.PrimaryAbsoluteSchema, this.DragModel.RelativeDropSchemaPath.Node);
      SceneNode setBindingOrData;
      if (targetProperty1 == null)
      {
        bool useDesignDataContext = DataBindingModeModel.Instance.NormalizedMode == DataBindingMode.Details;
        setBindingOrData = MiniBindingDialog.CreateAndSetBindingOrData(bindingPath, targetNode, useDesignDataContext, ref targetProperty1);
      }
      else
        setBindingOrData = this.DragModel.ViewModel.BindingEditor.CreateAndSetBindingOrData(targetNode, (IPropertyId) targetProperty1, bindingPath);
      if (setBindingOrData == null)
        return false;
      if (targetProperty != null)
        this.GenerateDetailsIfNeeded(targetNode, (IProperty) targetProperty1);
      targetProperty = (IProperty) targetProperty1;
      return true;
    }

    private bool ShouldGenerateDetails(IProperty targetProperty)
    {
      return ItemsControlElement.ItemsSourceProperty.Equals((object) targetProperty) || DataGridElement.ItemsSourceProperty.Equals((object) targetProperty);
    }

    private void GenerateDetailsIfNeeded(SceneNode targetNode, IProperty targetProperty)
    {
      if (!this.ShouldGenerateDetails(targetProperty))
        return;
      DataGridElement dataGridElement = targetNode as DataGridElement;
      if (dataGridElement != null)
        this.GenerateDataGridColumnsIfNeeded(dataGridElement, targetProperty);
      else if (ProjectNeutralTypes.TreeView.IsAssignableFrom((ITypeId) targetNode.Type))
        this.GenerateHierarchicalDataTemplateIfNeeded(targetNode);
      else
        this.GenerateDataTemplateIfNeeded(targetNode, PlatformTypes.DataTemplate);
    }

    private DocumentCompositeNode GenerateHierarchicalDataTemplateIfNeeded(SceneNode targetNode)
    {
      DocumentCompositeNode documentCompositeNode = this.GenerateDataTemplateIfNeeded(targetNode, ProjectNeutralTypes.HierarchicalDataTemplate);
      if (documentCompositeNode == null)
        return (DocumentCompositeNode) null;
      IType type = this.DragModel.RelativeDropSchemaPath.Type;
      IType typeId = type != null ? type.ItemType : (IType) null;
      if (typeId == null)
        return documentCompositeNode;
      string path;
      if (PlatformTypes.IEnumerable.IsAssignableFrom((ITypeId) typeId) && !PlatformTypes.String.IsAssignableFrom((ITypeId) typeId))
      {
        path = string.Empty;
      }
      else
      {
        IProperty property1 = (IProperty) null;
        MemberAccessTypes allowableMemberAccess = TypeHelper.GetAllowableMemberAccess((ITypeResolver) this.ProjectContext, typeId);
        List<IProperty> list = new List<IProperty>(typeId.GetProperties(allowableMemberAccess));
        list.Sort((Comparison<IProperty>) ((a, b) => StringLogicalComparer.Instance.Compare(a.Name, b.Name)));
        foreach (IProperty property2 in list)
        {
          IType propertyType = property2.PropertyType;
          if (propertyType == type)
          {
            property1 = property2;
            break;
          }
          if (property1 == null && PlatformTypes.IEnumerable.IsAssignableFrom((ITypeId) propertyType) && !PlatformTypes.String.IsAssignableFrom((ITypeId) propertyType))
            property1 = property2;
        }
        if (property1 == null)
          return documentCompositeNode;
        path = property1.Name;
      }
      BindingSceneNode bindingSceneNode = (BindingSceneNode) this.DragModel.ViewModel.CreateSceneNode(PlatformTypes.Binding);
      if (!string.IsNullOrEmpty(path))
        bindingSceneNode.SetPath(path);
      documentCompositeNode.Properties[DataBindingDragDropDefaultHandler.HierarchicalItemsSourceProperty] = bindingSceneNode.DocumentNode;
      return documentCompositeNode;
    }

    private DocumentCompositeNode GenerateDataTemplateIfNeeded(SceneNode targetNode, ITypeId dataTemplateType)
    {
      if (this.DragModel.RelativeDropSchemaPath.IsProperty)
        return (DocumentCompositeNode) null;
      IList<DataSchemaNodePath> relativeSchemaPaths = this.GetRelativeSchemaPaths();
      if (relativeSchemaPaths.Count == 0)
        return (DocumentCompositeNode) null;
      DocumentCompositeNode dataTemplateNode = (DocumentCompositeNode) null;
      using (SceneEditTransaction editTransaction = this.DragModel.Document.CreateEditTransaction(StringTable.UndoUnitCreateTemplate))
      {
        DataSchemaNode node = this.DragModel.RelativeDropSchemaPath.Node;
        DataSchemaNode dataSchemaNode = node.CollectionItem ?? node;
        string str = ((dataSchemaNode.Type != (Type) null ? dataSchemaNode.Type.Name : dataSchemaNode.PathName) + "Template").TrimStart('@', '/');
        if (!SceneNodeIDHelper.IsCSharpID(str))
          str = "DataTemplate";
        dataTemplateNode = DataViewFactory.CreateDataTemplateResource(targetNode, ItemsControlElement.ItemTemplateProperty, str, relativeSchemaPaths, DataViewCategory.DataTemplate, dataTemplateType);
        editTransaction.Update();
        this.ApplyActiveUserThemeStyleToDataTemplate(dataTemplateNode);
        editTransaction.Commit();
      }
      return dataTemplateNode;
    }

    private void ApplyActiveUserThemeStyleToDataTemplate(DocumentCompositeNode dataTemplateNode)
    {
      if (dataTemplateNode == null)
        return;
      DocumentCompositeNode documentCompositeNode1 = dataTemplateNode.Properties[FrameworkTemplateElement.VisualTreeProperty] as DocumentCompositeNode;
      if (documentCompositeNode1 == null || documentCompositeNode1.Type.Metadata.DefaultContentProperty == null)
        return;
      DocumentCompositeNode documentCompositeNode2 = documentCompositeNode1.Properties[(IPropertyId) documentCompositeNode1.Type.Metadata.DefaultContentProperty] as DocumentCompositeNode;
      if (documentCompositeNode2 == null || !documentCompositeNode2.SupportsChildren)
        return;
      foreach (DocumentNode node in (IEnumerable<DocumentNode>) documentCompositeNode2.Children)
        AssetLibrary.ApplyActiveUserThemeStyle(this.DragModel.ViewModel.GetSceneNode(node));
    }

    private void GenerateDataGridColumnsIfNeeded(DataGridElement dataGridElement, IProperty targetProperty)
    {
      if (!DataGridElement.ItemsSourceProperty.Equals((object) targetProperty) && !ItemsControlElement.ItemsSourceProperty.Equals((object) targetProperty) || this.DragModel.RelativeDropSchemaPath.IsProperty)
        return;
      IList<DataSchemaNodePath> relativeSchemaPaths = this.GetRelativeSchemaPaths();
      if (relativeSchemaPaths.Count == 0)
        return;
      using (SceneEditTransaction editTransaction = this.DragModel.Document.CreateEditTransaction(StringTable.UndoUnitCreateTemplate))
      {
        dataGridElement.SetValue(DataGridElement.AutoGenerateColumnsProperty, (object) false);
        ISceneNodeCollection<SceneNode> columnCollection = dataGridElement.ColumnCollection;
        for (int index = 0; index < relativeSchemaPaths.Count; ++index)
        {
          DataSchemaNodePath schemaPath = relativeSchemaPaths[index];
          IType type = schemaPath.Type;
          if (type.NullableType != null)
            type = type.NullableType;
          SceneNode sceneNode = !PlatformTypes.Boolean.IsAssignableFrom((ITypeId) type) ? (!PlatformTypes.IConvertible.IsAssignableFrom((ITypeId) type) ? this.CreateDataGridTemplateColumn(schemaPath) : this.CreateDataGridBoundColumn(schemaPath, ProjectNeutralTypes.DataGridTextColumn)) : this.CreateDataGridBoundColumn(schemaPath, ProjectNeutralTypes.DataGridCheckBoxColumn);
          columnCollection.Add(sceneNode);
        }
        editTransaction.Commit();
      }
    }

    private SceneNode CreateDataGridTemplateColumn(DataSchemaNodePath schemaPath)
    {
      DataGridColumnNode dataGridColumnNode = (DataGridColumnNode) this.DragModel.ViewModel.CreateSceneNode(ProjectNeutralTypes.DataGridTemplateColumn);
      string columnName = DataBindingDragDropDefaultHandler.GetColumnName(schemaPath);
      dataGridColumnNode.SetLocalValue(DataGridColumnNode.ColumnHeaderProperty, (object) columnName);
      string resourceNameBase = columnName + "Template";
      List<DataSchemaNodePath> list = new List<DataSchemaNodePath>()
      {
        schemaPath
      };
      DataViewFactory.CreateDataTemplateResource((SceneNode) dataGridColumnNode, DataGridColumnNode.TemplateColumnCellTemplateProperty, resourceNameBase, (IList<DataSchemaNodePath>) list, DataViewCategory.DataTemplate, PlatformTypes.DataTemplate);
      return (SceneNode) dataGridColumnNode;
    }

    private SceneNode CreateDataGridBoundColumn(DataSchemaNodePath schemaPath, ITypeId columnType)
    {
      DataGridColumnNode dataGridColumnNode = (DataGridColumnNode) this.DragModel.ViewModel.CreateSceneNode(columnType);
      string columnName = DataBindingDragDropDefaultHandler.GetColumnName(schemaPath);
      dataGridColumnNode.SetLocalValue(DataGridColumnNode.ColumnHeaderProperty, (object) columnName);
      BindingSceneNode bindingSceneNode = (BindingSceneNode) this.DragModel.ViewModel.CreateSceneNode(PlatformTypes.Binding);
      bindingSceneNode.SetPath(schemaPath.Path);
      BindingModeInfo defaultBindingMode = BindingPropertyHelper.GetDefaultBindingMode(dataGridColumnNode.DocumentNode, DataGridColumnNode.BoundColumnBindingProperty, schemaPath);
      if (!defaultBindingMode.IsOptional)
        bindingSceneNode.Mode = defaultBindingMode.Mode;
      dataGridColumnNode.SetValueAsSceneNode(DataGridColumnNode.BoundColumnBindingProperty, (SceneNode) bindingSceneNode);
      return (SceneNode) dataGridColumnNode;
    }

    private static string GetColumnName(DataSchemaNodePath schemaPath)
    {
      for (DataSchemaNode dataSchemaNode = schemaPath.Node; dataSchemaNode != null; dataSchemaNode = dataSchemaNode.Parent)
      {
        if (dataSchemaNode.PathName != DataSchemaNode.IndexNodePath)
          return dataSchemaNode.PathName;
      }
      return "_";
    }

    private ISchema GetRelativeSchema()
    {
      ISchema schema = this.DragModel.RelativeDropSchemaPath.Schema;
      DataSchemaNode collectionItemNode = this.DragModel.RelativeDropSchemaPath.EffectiveCollectionItemNode;
      if (collectionItemNode != null)
        schema = schema.MakeRelativeToNode(collectionItemNode);
      return schema;
    }

    private IList<DataSchemaNodePath> GetRelativeSchemaPaths()
    {
      ISchema relativeSchema = this.GetRelativeSchema();
      List<DataSchemaNodePath> list = new List<DataSchemaNodePath>();
      foreach (DataSchemaNode endNode in (IEnumerable<DataSchemaNode>) (this.DragModel.NodesToCreateElements ?? (IList<DataSchemaNode>) this.GetNodesToCreateElements()))
        list.Add(new DataSchemaNodePath(relativeSchema, endNode));
      return (IList<DataSchemaNodePath>) list;
    }

    private void UpdateTooltip()
    {
      string str = string.Empty;
      this.DragModel.Tooltip = string.Format((IFormatProvider) CultureInfo.InvariantCulture, !this.DragModel.CheckDropFlags(DataBindingDragDropFlags.CreateElement) ? (!string.IsNullOrEmpty(this.DragModel.TargetPropertyName) ? StringTable.ArtboardBindingTooltipBind : StringTable.ArtboardBindingTooltipBindNoProperty) : (DataBindingModeModel.Instance.NormalizedMode != DataBindingMode.Details || this.GetReusableDetailsContainer(false, this.DragModel.CheckDropFlags(DataBindingDragDropFlags.SetBinding)) != null ? StringTable.ArtboardBindingTooltipCreate : StringTable.ArtboardBindingTooltipCreateDetails), (object) this.DragModel.TargetNodeName, (object) this.DragModel.TargetPropertyName, (object) this.DragModel.SourceName);
    }
  }
}
