// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SampleData.SampleDataDocumentChangeProcessor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.UserInterface.DataPane;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.Expression.DesignSurface.SampleData
{
  public class SampleDataDocumentChangeProcessor : DataBindingProcessor
  {
    private static readonly IPropertyId ChangePropertyActionNameProperty = (IPropertyId) ProjectNeutralTypes.SetDataStoreValueAction.GetMember(MemberType.Property, "PropertyName", MemberAccessTypes.Public);
    private List<DocumentCompositeNode> nodesToInvalidate = new List<DocumentCompositeNode>();
    private List<PathChange> pathChanges = new List<PathChange>();
    private List<SamplePropertyRenamed> renamedPropertyChanges;
    private SampleDataChangeProcessor ownerProcessor;

    public SampleDataSet SampleData { get; private set; }

    public int RenameChangesCount
    {
      get
      {
        return this.renamedPropertyChanges.Count;
      }
    }

    public SampleDataDocumentChangeProcessor(SampleDataChangeProcessor ownerProcessor, SampleDataSet dataSet, IList<SampleDataChange> normalizedChanges)
    {
      this.ownerProcessor = ownerProcessor;
      this.SampleData = dataSet;
      this.renamedPropertyChanges = new List<SamplePropertyRenamed>();
      this.renamedPropertyChanges.AddRange(Enumerable.Select<SampleDataChange, SamplePropertyRenamed>(Enumerable.Where<SampleDataChange>((IEnumerable<SampleDataChange>) normalizedChanges, (Func<SampleDataChange, bool>) (change => change is SamplePropertyRenamed)), (Func<SampleDataChange, SamplePropertyRenamed>) (change => (SamplePropertyRenamed) change)));
    }

    protected override bool ShouldProcessDocumentNode(DataBindingProcessingContext context)
    {
      if (this.ownerProcessor.IsKilled)
        return false;
      if (this.SampleData.IsTypeOwner(DataContextHelper.GetDataType(context.DocumentNode)) && this.SampleData.RootType != context.DocumentNode.Type)
        this.nodesToInvalidate.Add(context.DocumentCompositeNode);
      return !(context.DocumentNode.Type is SampleNonBasicType);
    }

    protected override bool ShouldProcessDataSourceType(IType dataType)
    {
      return this.SampleData.IsTypeOwner(dataType);
    }

    private void HandleBindingOnChangePropertyAction(DataBindingProcessingContext context, RawDataSourceInfoBase bindingInfo)
    {
      string valueAsString = context.DocumentCompositeNode.GetValueAsString(SampleDataDocumentChangeProcessor.ChangePropertyActionNameProperty);
      if (string.IsNullOrEmpty(valueAsString))
        return;
      PathChangeInfo pathChange = this.GetPathChange(bindingInfo.SourceType, valueAsString, valueAsString, false);
      if (pathChange == null)
        return;
      this.pathChanges.Add((PathChange) new SampleDataDocumentChangeProcessor.ChangePropertyActionPathChange(pathChange, context.DocumentCompositeNode));
    }

    protected override void HandleBinding(DataBindingProcessingContext context, RawDataSourceInfoBase bindingInfo)
    {
      if (!bindingInfo.HasSource || !bindingInfo.IsValidClr)
        return;
      if (ProjectNeutralTypes.ChangePropertyAction.IsAssignableFrom((ITypeId) context.DocumentNode.Type))
      {
        this.HandleBindingOnChangePropertyAction(context, bindingInfo);
      }
      else
      {
        string bindingPath = DataContextHelper.GetBindingPath(context.DocumentCompositeNode);
        bool preservePropertyName = this.ShouldPreservePropertyName(context);
        PathChangeInfo change = this.GetPathChange(bindingInfo, bindingPath, preservePropertyName);
        if (change == null)
          return;
        if (change.BreakingChange && context.Scope == ProcessingContextScope.DataTemplate && this.ProjectContext.IsCapabilitySet(PlatformCapability.IsWpf))
        {
          RawDataSourceInfoBase bindingInfo1 = this.ConvertDataTemplateContextToCollectionItem(context, bindingInfo);
          if (bindingInfo1 != null)
          {
            PathChangeInfo pathChange = this.GetPathChange(bindingInfo1, bindingPath, preservePropertyName);
            if (pathChange == null)
              return;
            if (!pathChange.BreakingChange)
              change = pathChange;
          }
        }
        if (change == null)
          return;
        this.pathChanges.Add((PathChange) new SampleDataDocumentChangeProcessor.BindingPathChange(change, context.DocumentCompositeNode));
      }
    }

    protected override void HandleDataContextPathExtension(DataBindingProcessingContext context, RawDataSourceInfoBase bindingInfo)
    {
      string valueAsString = DocumentPrimitiveNode.GetValueAsString(context.DocumentNode);
      PathChangeInfo pathChange = this.GetPathChange(bindingInfo, valueAsString, false);
      if (pathChange == null)
        return;
      this.pathChanges.Add((PathChange) new SampleDataDocumentChangeProcessor.PrimitivePathChange(pathChange, context.ParentNode, context.Property));
    }

    public void ApplyChanges(SceneDocument sceneDocument)
    {
      List<PathChange> documentChanges = new List<PathChange>();
      for (int index = this.pathChanges.Count - 1; index >= 0; --index)
      {
        PathChange pathChange = this.pathChanges[index];
        if (pathChange.DocumentNode.DocumentRoot == this.DocumentRoot)
        {
          documentChanges.Add(pathChange);
          this.pathChanges.RemoveAt(index);
        }
      }
      this.ApplyChangesInternal(sceneDocument, documentChanges);
      this.ApplyChangesToExternalDocuments(sceneDocument.ProjectContext);
    }

    private void ApplyChangesInternal(SceneDocument sceneDocument, List<PathChange> documentChanges)
    {
      bool flag = true;
      foreach (PathChange pathChange in documentChanges)
      {
        if (!pathChange.Change.BreakingChange)
        {
          flag = false;
          break;
        }
      }
      if (flag)
        return;
      SceneView sceneView = this.GetSceneView(sceneDocument);
      string description = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.SampleDataUpdateBindingsTransaction, new object[1]
      {
        (object) this.SampleData.Name
      });
      using (SceneEditTransaction editTransaction = sceneDocument.CreateEditTransaction(description))
      {
        using (sceneView.ViewModel.AnimationEditor.DeferKeyFraming())
        {
          foreach (PathChange pathChange in documentChanges)
            pathChange.ApplyChange(sceneView.ViewModel);
        }
        editTransaction.Commit();
      }
    }

    private void ApplyChangesToExternalDocuments(IProjectContext projectContext)
    {
      Dictionary<IDocumentRoot, List<PathChange>> dictionary = new Dictionary<IDocumentRoot, List<PathChange>>();
      for (int index = this.pathChanges.Count - 1; index >= 0; --index)
      {
        PathChange pathChange = this.pathChanges[index];
        if ((IProjectContext) pathChange.DocumentNode.TypeResolver != projectContext)
        {
          List<PathChange> list;
          if (!dictionary.TryGetValue(pathChange.DocumentNode.DocumentRoot, out list))
          {
            list = new List<PathChange>();
            dictionary[pathChange.DocumentNode.DocumentRoot] = list;
          }
          list.Add(pathChange);
          this.pathChanges.RemoveAt(index);
        }
      }
      foreach (KeyValuePair<IDocumentRoot, List<PathChange>> keyValuePair in dictionary)
      {
        IDocumentRoot key = keyValuePair.Key;
        this.ApplyChangesInternal(((IProjectContext) key.DocumentContext.TypeResolver).OpenDocument(key.DocumentContext.DocumentUrl).Document as SceneDocument, keyValuePair.Value);
      }
    }

    protected SceneView GetSceneView(SceneDocument sceneDocument)
    {
      return ((ISceneViewHost) this.SampleData.ProjectContext.GetService(typeof (ISceneViewHost))).OpenView(sceneDocument.DocumentRoot, false);
    }

    public void InvalidateNodes(SceneDocument sceneDocument)
    {
      List<DocumentCompositeNode> list = new List<DocumentCompositeNode>();
      for (int index = this.nodesToInvalidate.Count - 1; index >= 0; --index)
      {
        DocumentCompositeNode documentCompositeNode = this.nodesToInvalidate[index];
        if (documentCompositeNode.DocumentRoot == this.DocumentRoot)
        {
          list.Add(documentCompositeNode);
          this.nodesToInvalidate.RemoveAt(index);
        }
      }
      string @namespace = this.SampleData.RootType.RuntimeType.Namespace;
      using (SceneEditTransaction editTransaction = sceneDocument.CreateEditTransaction(StringTable.ReferencesFixupEditTransaction, true))
      {
        foreach (DocumentCompositeNode documentCompositeNode in list)
          documentCompositeNode.Properties[DesignTimeProperties.SampleDataTagProperty] = (DocumentNode) documentCompositeNode.Context.CreateNode(@namespace);
        editTransaction.Commit();
      }
    }

    private bool ShouldPreservePropertyName(DataBindingProcessingContext context)
    {
      if (context.DocumentCompositeNode.Properties[BindingSceneNode.ElementNameProperty] != null || context.DocumentCompositeNode.Properties[BindingSceneNode.RelativeSourceProperty] != null)
        return true;
      ElementDataSourceInfo elementDataSourceInfo = context.DataContext as ElementDataSourceInfo;
      return elementDataSourceInfo != null && elementDataSourceInfo.TargetProperty == null;
    }

    private RawDataSourceInfoBase ConvertDataTemplateContextToCollectionItem(DataBindingProcessingContext context, RawDataSourceInfoBase dataSource)
    {
      RawDataSourceInfo normalizedDataSource = context.RootContext.DataContext.NormalizedDataSource;
      if (dataSource.SourceNode != normalizedDataSource.SourceNode)
        return (RawDataSourceInfoBase) null;
      string inheritedPath = normalizedDataSource.NormalizedClrPath ?? string.Empty;
      string str = dataSource.NormalizedClrPath ?? string.Empty;
      if (!str.StartsWith(inheritedPath, StringComparison.Ordinal))
        return (RawDataSourceInfoBase) null;
      string localPath = string.Empty;
      if (str.Length > inheritedPath.Length)
      {
        switch (str[inheritedPath.Length])
        {
          case '[':
            localPath = str.Substring(inheritedPath.Length);
            break;
          case '.':
          case '/':
            localPath = str.Substring(inheritedPath.Length + 1);
            break;
          default:
            return (RawDataSourceInfoBase) null;
        }
      }
      string clrPath = ClrPropertyPathHelper.CombinePaths(ClrPropertyPathHelper.CombinePaths(inheritedPath, DataSchemaNode.IndexNodePath), localPath);
      return (RawDataSourceInfoBase) new RawDataSourceInfo(dataSource.SourceNode, clrPath);
    }

    private PathChangeInfo GetPathChange(RawDataSourceInfoBase bindingInfo, string clrPath, bool preservePropertyName)
    {
      return this.GetPathChange(bindingInfo.SourceType, bindingInfo.NormalizedClrPath, clrPath, preservePropertyName);
    }

    private PathChangeInfo GetPathChange(IType sourceType, string normalizedClrPath, string clrPath, bool preservePropertyName)
    {
      if (string.IsNullOrEmpty(clrPath))
        return (PathChangeInfo) null;
      PathChangeInfo pathChangeInfo = new PathChangeInfo();
      pathChangeInfo.TargetType = sourceType;
      pathChangeInfo.OldPath = clrPath;
      pathChangeInfo.NewPath = string.Empty;
      IList<ClrPathPart> parts = ClrPropertyPathHelper.SplitPath(normalizedClrPath);
      if (parts == null)
        return (PathChangeInfo) null;
      bool flag = false;
      for (int index = 0; index < parts.Count; ++index)
      {
        ClrPathPart clrPathPart = parts[index];
        IType targetType = pathChangeInfo.TargetType;
        IType type;
        if (clrPathPart.Category == ClrPathPartCategory.CurrentItem || clrPathPart.Category == ClrPathPartCategory.IndexStep)
        {
          type = targetType.ItemType;
        }
        else
        {
          IProperty property = this.VerifyPropertyName(clrPathPart.Path, targetType);
          if (property == null)
          {
            type = (IType) null;
          }
          else
          {
            type = property.PropertyType;
            if (property.Name != clrPathPart.Path)
            {
              flag = true;
              clrPathPart.NewPath = property.Name;
            }
          }
        }
        pathChangeInfo.TargetType = type;
        if (type == null)
        {
          pathChangeInfo.BreakingChange = true;
          break;
        }
      }
      if (pathChangeInfo.BreakingChange)
        return pathChangeInfo;
      if (!flag)
        return (PathChangeInfo) null;
      if (preservePropertyName)
      {
        string[] strArray = ClrPropertyPathHelper.SplitAtFirstProperty(clrPath);
        string inheritedPath = strArray[0];
        int pathPartCount = ClrPropertyPathHelper.GetPathPartCount(strArray[1]);
        string localPath = ClrPropertyPathHelper.CombinePathParts(parts, parts.Count - pathPartCount);
        pathChangeInfo.NewPath = ClrPropertyPathHelper.CombinePaths(inheritedPath, localPath);
      }
      else
      {
        int pathPartCount = ClrPropertyPathHelper.GetPathPartCount(clrPath);
        pathChangeInfo.NewPath = ClrPropertyPathHelper.CombinePathParts(parts, parts.Count - pathPartCount);
      }
      return pathChangeInfo;
    }

    private IProperty VerifyPropertyName(string propertyName, IType type)
    {
      if (string.IsNullOrEmpty(propertyName))
        return (IProperty) null;
      if (type == null)
        return (IProperty) null;
      IProperty property = (IProperty) type.GetMember(MemberType.Property, propertyName, MemberAccessTypes.Public);
      if (property == null)
      {
        SampleCompositeType compositeType = type as SampleCompositeType;
        if (compositeType == null)
          return (IProperty) null;
        SamplePropertyRenamed samplePropertyRenamed = this.renamedPropertyChanges.Find((Predicate<SamplePropertyRenamed>) (renameChange =>
        {
          if (renameChange.SampleProperty.DeclaringSampleType != compositeType)
            return false;
          return renameChange.OldName == propertyName;
        }));
        if (samplePropertyRenamed == null)
          return (IProperty) null;
        property = (IProperty) samplePropertyRenamed.SampleProperty;
      }
      return property;
    }

    private class BindingPathChange : PathChange
    {
      public BindingPathChange(PathChangeInfo change, DocumentCompositeNode documentNode)
        : base(change, documentNode)
      {
      }

      public override void ApplyChange(SceneViewModel viewModel)
      {
        if (this.Change.BreakingChange)
          return;
        BindingSceneNode bindingSceneNode = viewModel.GetSceneNode((DocumentNode) this.DocumentNode) as BindingSceneNode;
        if (bindingSceneNode == null)
          return;
        bindingSceneNode.SetPath(this.Change.NewPath);
      }
    }

    private class ChangePropertyActionPathChange : PathChange
    {
      public ChangePropertyActionPathChange(PathChangeInfo change, DocumentCompositeNode documentNode)
        : base(change, documentNode)
      {
      }

      public override void ApplyChange(SceneViewModel viewModel)
      {
        if (this.Change.BreakingChange)
          return;
        SceneNode sceneNode = viewModel.GetSceneNode((DocumentNode) this.DocumentNode);
        if (sceneNode == null)
          return;
        sceneNode.SetLocalValue(SampleDataDocumentChangeProcessor.ChangePropertyActionNameProperty, (object) this.Change.NewPath);
      }
    }

    private class PrimitivePathChange : PathChange
    {
      public IProperty PathProperty { get; set; }

      public PrimitivePathChange(PathChangeInfo change, DocumentCompositeNode documentNode, IProperty pathProperty)
        : base(change, documentNode)
      {
        this.PathProperty = pathProperty;
      }

      public override void ApplyChange(SceneViewModel viewModel)
      {
        if (this.Change.BreakingChange)
          return;
        viewModel.GetSceneNode((DocumentNode) this.DocumentNode).SetLocalValue((IPropertyId) this.PathProperty, (object) this.Change.NewPath);
      }
    }
  }
}
