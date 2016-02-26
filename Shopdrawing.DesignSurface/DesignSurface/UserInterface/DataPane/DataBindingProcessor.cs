// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.DataBindingProcessor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public abstract class DataBindingProcessor
  {
    private static readonly IPropertyId ChangePropetyActionTargetObject = (IPropertyId) ProjectNeutralTypes.ChangePropertyAction.GetMember(MemberType.Property, "TargetObject", MemberAccessTypes.Public);
    private Dictionary<DocumentNode, DataBindingProcessor.ResourceProcessingContext> processedResources = new Dictionary<DocumentNode, DataBindingProcessor.ResourceProcessingContext>();
    private Dictionary<IDocumentRoot, Dictionary<DocumentCompositeNode, DataBindingProcessingContext>> documenNamedElementDataContexts = new Dictionary<IDocumentRoot, Dictionary<DocumentCompositeNode, DataBindingProcessingContext>>();
    private List<DataBindingProcessor.PendingBindingInfo> pendingBindings = new List<DataBindingProcessor.PendingBindingInfo>();
    private Dictionary<DocumentCompositeNode, DataBindingProcessingContext> namedElementDataContexts;

    public DataContextEvaluator DataContextEvaluator { get; private set; }

    public ExpressionEvaluator ExpressionEvaluator { get; private set; }

    public IDocumentRoot DocumentRoot { get; private set; }

    public IProjectContext ProjectContext { get; private set; }

    public bool AllChangesCollected
    {
      get
      {
        return this.pendingBindings.Count == 0;
      }
    }

    protected abstract bool ShouldProcessDocumentNode(DataBindingProcessingContext context);

    protected abstract bool ShouldProcessDataSourceType(IType dataType);

    protected abstract void HandleBinding(DataBindingProcessingContext context, RawDataSourceInfoBase bindingInfo);

    protected abstract void HandleDataContextPathExtension(DataBindingProcessingContext context, RawDataSourceInfoBase bindingInfo);

    public void ProcessDocument(SceneDocument sceneDocument, DataBindingProcessingOptions options)
    {
      if (sceneDocument.DocumentRoot == null || sceneDocument.DocumentRoot.RootNode == null)
        return;
      this.EnsureInitialized(sceneDocument.ProjectContext);
      this.DocumentRoot = sceneDocument.DocumentRoot;
      if (!this.documenNamedElementDataContexts.TryGetValue(this.DocumentRoot, out this.namedElementDataContexts))
      {
        this.namedElementDataContexts = new Dictionary<DocumentCompositeNode, DataBindingProcessingContext>();
        this.documenNamedElementDataContexts[this.DocumentRoot] = this.namedElementDataContexts;
      }
      if ((options & DataBindingProcessingOptions.FirstPass) == DataBindingProcessingOptions.FirstPass)
      {
        DataBindingProcessingContext context = new DataBindingProcessingContext(sceneDocument.DocumentRoot.RootNode, (DataBindingProcessingContext) null);
        context.DataContext = this.InitDataContext(context);
        this.ProcessDocumentNode(context);
        this.ProcessPendingBindings();
      }
      if ((options & DataBindingProcessingOptions.SecondPass) != DataBindingProcessingOptions.SecondPass)
        return;
      this.ProcessPendingBindings();
    }

    private void EnsureInitialized(IProjectContext projectContext)
    {
      if (projectContext == this.ProjectContext)
        return;
      this.ProjectContext = projectContext;
      this.DataContextEvaluator = new DataContextEvaluator();
      this.ExpressionEvaluator = new ExpressionEvaluator((IDocumentRootResolver) projectContext);
    }

    private void ProcessDocumentNode(DataBindingProcessingContext context)
    {
      DocumentCompositeNode documentCompositeNode1 = context.DocumentCompositeNode;
      if (documentCompositeNode1.Type.IsBinding)
      {
        this.ProcessBinding(context);
      }
      else
      {
        if (!this.ShouldProcessDocumentNode(context))
          return;
        DocumentCompositeNode documentCompositeNode2 = this.ResolveResourceReferenceIfNeeded(documentCompositeNode1);
        if (documentCompositeNode2 != null)
        {
          if (PlatformTypes.DataTemplate.IsAssignableFrom((ITypeId) documentCompositeNode2.Type))
          {
            this.ProcessDataTemplate(context, documentCompositeNode2);
            return;
          }
          if (PlatformTypes.Style.IsAssignableFrom((ITypeId) documentCompositeNode2.Type))
          {
            this.ProcessStyle(context, documentCompositeNode2);
            return;
          }
          if (PlatformTypes.ControlTemplate.IsAssignableFrom((ITypeId) documentCompositeNode2.Type))
          {
            this.ProcessControlTemplate(context, documentCompositeNode2);
            return;
          }
          if (ProjectNeutralTypes.ChangePropertyAction.IsAssignableFrom((ITypeId) documentCompositeNode2.Type))
          {
            this.ProcessChangePropertyAction(context);
            return;
          }
          if (documentCompositeNode2 != documentCompositeNode1 && PlatformTypes.FrameworkElement.IsAssignableFrom((ITypeId) documentCompositeNode2.Type))
          {
            this.ProcessResource(context, documentCompositeNode2);
            return;
          }
        }
        this.ProcessDocumentNodeChildren(context);
      }
    }

    private void ProcessDocumentNodeChildren(DataBindingProcessingContext context)
    {
      DocumentCompositeNode documentCompositeNode = context.DocumentCompositeNode;
      if (documentCompositeNode == null)
        return;
      if (!string.IsNullOrEmpty(documentCompositeNode.Name))
        this.namedElementDataContexts[documentCompositeNode] = context;
      foreach (KeyValuePair<IProperty, DocumentNode> keyValuePair in (IEnumerable<KeyValuePair<IProperty, DocumentNode>>) documentCompositeNode.Properties)
      {
        DocumentCompositeNode childNode = keyValuePair.Value as DocumentCompositeNode;
        if (childNode != null)
          this.ProcessDocumentNodeChild(context, childNode, keyValuePair.Key);
        else
          this.ProcessDataContextPathExtension(context, keyValuePair.Value, keyValuePair.Key);
      }
      if (!documentCompositeNode.SupportsChildren)
        return;
      for (int index = 0; index < documentCompositeNode.Children.Count; ++index)
      {
        DocumentCompositeNode childNode = documentCompositeNode.Children[index] as DocumentCompositeNode;
        this.ProcessDocumentNodeChild(context, childNode, (IProperty) null);
      }
    }

    private void ProcessDocumentNodeChild(DataBindingProcessingContext context, DocumentCompositeNode childNode, IProperty property)
    {
      if (childNode == null)
        return;
      DataBindingProcessingContext context1 = new DataBindingProcessingContext(context, (DocumentNode) childNode, property);
      context1.DataContext = this.InitDataContext(context1);
      this.ProcessDocumentNode(context1);
    }

    private void ProcessPendingBindings()
    {
      List<DataBindingProcessor.PendingBindingInfo> list = new List<DataBindingProcessor.PendingBindingInfo>((IEnumerable<DataBindingProcessor.PendingBindingInfo>) this.pendingBindings);
      this.pendingBindings.Clear();
      foreach (DataBindingProcessor.PendingBindingInfo pendingBindingInfo in list)
      {
        RawDataSourceInfoBase dataSourceInfoBase = this.CombineDataSources((RawDataSourceInfoBase) null, (RawDataSourceInfoBase) pendingBindingInfo.BindingInfo);
        if (this.ShouldProcess(dataSourceInfoBase))
          this.HandleBinding(pendingBindingInfo.BindingContext, dataSourceInfoBase);
      }
      this.pendingBindings.Clear();
    }

    private RawDataSourceInfoBase InitDataContext(DataBindingProcessingContext context)
    {
      if (PlatformTypes.ResourceDictionary.IsAssignableFrom((ITypeId) context.DocumentNode.Type))
        return (RawDataSourceInfoBase) RawDataSourceInfo.NewEmpty;
      if (DataContextHelper.IsDataContextProperty((DocumentNode) context.ParentNode, (IPropertyId) context.Property))
        return context.GrandparentDataContext;
      DataBindingProcessor.DataContextWalker dataContextWalker = new DataBindingProcessor.DataContextWalker(context.ParentContext, context.Property, this.namedElementDataContexts);
      RawDataSourceInfoBase localDataContext = this.DataContextEvaluator.NavigateSpecialDataContext((IDataContextAncestorWalker) dataContextWalker, (RawDataSourceInfoBase) null);
      if (localDataContext == null)
      {
        RawDataSourceInfoBase rawDataContextInfo = this.DataContextEvaluator.GetRawDataContextInfo(context.DocumentCompositeNode);
        localDataContext = this.DataContextEvaluator.UnwindElementNameBinding((IDataContextAncestorWalker) dataContextWalker, rawDataContextInfo);
      }
      this.DataContextEvaluator.MoveUpIfDataContextLocation((IDataContextAncestorWalker) dataContextWalker);
      if (localDataContext is ElementDataSourceInfo)
        return localDataContext;
      if (dataContextWalker.CurrentContext != null)
        localDataContext = this.CombineDataSources(dataContextWalker.CurrentContext.DataContext, localDataContext);
      if (localDataContext == null)
        localDataContext = (RawDataSourceInfoBase) RawDataSourceInfo.NewEmpty;
      return localDataContext;
    }

    protected RawDataSourceInfoBase CombineDataSources(RawDataSourceInfoBase inheritedDataContext, RawDataSourceInfoBase localDataContext)
    {
      if (localDataContext == null)
        return inheritedDataContext;
      if (!localDataContext.IsValid || localDataContext.HasSource)
        return localDataContext;
      ElementDataSourceInfo elementDataSourceInfo = localDataContext as ElementDataSourceInfo;
      RawDataSourceInfoBase localSource;
      if (elementDataSourceInfo == null)
      {
        localSource = inheritedDataContext == null || !inheritedDataContext.IsValid ? localDataContext : inheritedDataContext.CombineWith(localDataContext);
      }
      else
      {
        DataBindingProcessingContext context;
        if (!this.namedElementDataContexts.TryGetValue(elementDataSourceInfo.RootElement, out context))
        {
          localSource = (RawDataSourceInfoBase) elementDataSourceInfo;
        }
        else
        {
          DataBindingProcessor.DataContextWalker dataContextWalker = new DataBindingProcessor.DataContextWalker(context, (IProperty) null, this.namedElementDataContexts);
          localSource = this.DataContextEvaluator.UnwindElementNameBinding((IDataContextAncestorWalker) dataContextWalker, (RawDataSourceInfoBase) elementDataSourceInfo);
          if (!(localSource is ElementDataSourceInfo))
          {
            this.DataContextEvaluator.MoveUpIfDataContextLocation((IDataContextAncestorWalker) dataContextWalker);
            if (dataContextWalker.CurrentContext != null)
              localSource = dataContextWalker.CurrentContext.DataContext.CombineWith(localSource);
          }
        }
      }
      return localSource;
    }

    private RawDataSourceInfoBase GetDataContextFromType(IType type, DocumentNode sourceNode)
    {
      if (type == null || !this.ShouldProcessDataSourceType(type))
        return (RawDataSourceInfoBase) null;
      return (RawDataSourceInfoBase) new RawDataSourceInfo(sourceNode, (string) null);
    }

    private RawDataSourceInfoBase GetDataSourceFromProperty(DataBindingProcessingContext context, string propertyName)
    {
      IType type = context.DocumentNode.Type;
      MemberAccessTypes allowableMemberAccess = TypeHelper.GetAllowableMemberAccess((ITypeResolver) this.ProjectContext, type);
      IProperty property = type.GetMember(MemberType.Property, propertyName, allowableMemberAccess) as IProperty;
      RawDataSourceInfoBase dataSourceInfoBase = (RawDataSourceInfoBase) null;
      if (property != null)
        dataSourceInfoBase = this.GetDataSourceFromProperty(context, property);
      return dataSourceInfoBase;
    }

    private RawDataSourceInfoBase GetDataSourceFromProperty(DataBindingProcessingContext context, IProperty property)
    {
      if (DataContextHelper.GetDataContextProperty(context.DocumentNode.Type) == property)
        return context.DataContext;
      DocumentNode dataSourceNode = context.DocumentCompositeNode.Properties[(IPropertyId) property];
      if (dataSourceNode == null)
        return (RawDataSourceInfoBase) null;
      RawDataSourceInfoBase rawDataSourceInfo = DataContextHelper.GetRawDataSourceInfo(dataSourceNode);
      return this.CombineDataSources(context.DataContext, rawDataSourceInfo);
    }

    private void ProcessDataTemplate(DataBindingProcessingContext context, DocumentCompositeNode resolvedTemplateNode)
    {
      IType targetType = this.GetTargetType(resolvedTemplateNode, DataTemplateElement.DataTypeProperty);
      if (targetType == null)
      {
        this.ProcessResource(context, resolvedTemplateNode);
      }
      else
      {
        if (resolvedTemplateNode != context.DocumentCompositeNode)
          return;
        RawDataSourceInfoBase dataContextFromType = this.GetDataContextFromType(targetType, resolvedTemplateNode.Properties[DataTemplateElement.DataTypeProperty]);
        if (dataContextFromType == null)
          return;
        this.ProcessDocumentNodeChildren(new DataBindingProcessingContext((DocumentNode) resolvedTemplateNode, (DataBindingProcessingContext) null)
        {
          DataContext = dataContextFromType
        });
      }
    }

    private void ProcessStyle(DataBindingProcessingContext context, DocumentCompositeNode styleNode)
    {
      if (!context.IsEmptyDataContext && !this.VerifyStyleTargetType(context, styleNode))
        return;
      this.ProcessResource(context, styleNode);
    }

    private bool VerifyStyleTargetType(DataBindingProcessingContext context, DocumentCompositeNode styleNode)
    {
      IType targetType = this.GetTargetType(styleNode, StyleNode.TargetTypeProperty);
      if (targetType == null)
        return true;
      if (context.Property == null)
        return false;
      DocumentCompositeNode parentNode = context.ParentNode;
      Type propertyTargetType = parentNode.Type.Metadata.GetStylePropertyTargetType((IPropertyId) context.Property);
      if (propertyTargetType == (Type) null)
        return context.Property.Equals((object) StyleNode.BasedOnProperty);
      IType type = parentNode.TypeResolver.GetType(propertyTargetType);
      return targetType.IsAssignableFrom((ITypeId) type);
    }

    private void ProcessControlTemplate(DataBindingProcessingContext context, DocumentCompositeNode templateNode)
    {
      if (!context.IsEmptyDataContext && !this.VerifyControlTemplateTargetType(context, templateNode))
        return;
      this.ProcessResource(context, templateNode);
    }

    private bool VerifyControlTemplateTargetType(DataBindingProcessingContext context, DocumentCompositeNode templateNode)
    {
      IType targetType = this.GetTargetType(templateNode, ControlTemplateElement.TargetTypeProperty);
      if (targetType == null)
        return true;
      DocumentCompositeNode parentNode = context.ParentNode;
      return parentNode != null && (targetType.IsAssignableFrom((ITypeId) parentNode.Type) || PlatformTypes.Setter.IsAssignableFrom((ITypeId) parentNode.Type) && context.Scope == ProcessingContextScope.Style);
    }

    private IType GetTargetType(DocumentCompositeNode documentNode, IPropertyId typeProperty)
    {
      IProperty property = documentNode.TypeResolver.ResolveProperty(typeProperty);
      if (property == null)
        return (IType) null;
      IType type = (IType) null;
      DocumentNode node = documentNode.Properties[(IPropertyId) property];
      if (node != null)
        type = DocumentPrimitiveNode.GetValueAsType(node);
      return type;
    }

    private void ProcessResource(DataBindingProcessingContext context, DocumentCompositeNode resourceNode)
    {
      DataBindingProcessor.ResourceProcessingContext processingContext;
      if (!this.processedResources.TryGetValue((DocumentNode) resourceNode, out processingContext))
      {
        processingContext = new DataBindingProcessor.ResourceProcessingContext();
        this.processedResources[(DocumentNode) resourceNode] = processingContext;
      }
      if (processingContext.IsProcessing || !processingContext.AddToProcessedContextsIfNeeded(context))
        return;
      using (processingContext.StartProcessing())
        this.ProcessDocumentNodeChildren(new DataBindingProcessingContext((DocumentNode) resourceNode, context));
    }

    private DocumentCompositeNode ResolveResourceReferenceIfNeeded(DocumentCompositeNode compositeNode)
    {
      if (compositeNode == null || !compositeNode.Type.IsResource)
        return compositeNode;
      return this.ExpressionEvaluator.EvaluateExpression(new DocumentNodePath(compositeNode.DocumentRoot.RootNode, (DocumentNode) compositeNode), (DocumentNode) compositeNode) as DocumentCompositeNode;
    }

    private void ProcessChangePropertyAction(DataBindingProcessingContext context)
    {
      INodeSourceContext containerContext = (INodeSourceContext) null;
      DocumentCompositeNode bindingNode = context.DocumentCompositeNode.GetValue(DataBindingProcessor.ChangePropetyActionTargetObject, out containerContext) as DocumentCompositeNode;
      if (bindingNode == null || !bindingNode.Type.IsBinding)
        return;
      RawDataSourceInfoBase sourceInfoFromBinding = DataContextHelper.GetDataSourceInfoFromBinding(bindingNode);
      RawDataSourceInfoBase bindingInfo = this.CombineDataSources(context.DataContext, sourceInfoFromBinding);
      if (!bindingInfo.IsValid)
        bindingInfo = this.GetRelativeSourceTargetBindingDataSource(context);
      if (bindingInfo == null || !bindingInfo.IsValid)
        return;
      this.HandleBinding(context, bindingInfo);
    }

    private void ProcessBinding(DataBindingProcessingContext context)
    {
      RawDataSourceInfoBase bindingAsDataSource = this.GetBindingAsDataSource(context);
      if (bindingAsDataSource == null || !bindingAsDataSource.IsValid)
        return;
      if (!bindingAsDataSource.HasSource && context.Scope != ProcessingContextScope.ResourceDictionary)
      {
        ElementDataSourceInfo bindingInfo = bindingAsDataSource as ElementDataSourceInfo;
        if (bindingInfo != null)
          this.pendingBindings.Add(new DataBindingProcessor.PendingBindingInfo(context, bindingInfo));
      }
      if (!this.ShouldProcessDataSourceType(bindingAsDataSource.SourceType))
        return;
      this.HandleBinding(context, bindingAsDataSource);
    }

    private RawDataSourceInfoBase GetBindingAsDataSource(DataBindingProcessingContext context)
    {
      RawDataSourceInfoBase sourceInfoFromBinding = DataContextHelper.GetDataSourceInfoFromBinding(context.DocumentCompositeNode);
      RawDataSourceInfoBase dataSourceInfoBase = this.CombineDataSources(context.DataContext, sourceInfoFromBinding);
      if (!dataSourceInfoBase.IsValid)
        dataSourceInfoBase = this.GetRelativeSourceTargetBindingDataSource(context);
      return dataSourceInfoBase;
    }

    private RawDataSourceInfoBase GetRelativeSourceTargetBindingDataSource(DataBindingProcessingContext context)
    {
      if (context.OuterContext == null || !context.IsStyleOrControlTemplateScope)
        return (RawDataSourceInfoBase) null;
      string bindingRelativeSource = this.GetBindingRelativeSource(context);
      if (string.IsNullOrEmpty(bindingRelativeSource))
        return (RawDataSourceInfoBase) null;
      DataBindingProcessingContext context1 = (DataBindingProcessingContext) null;
      if (context.Scope == ProcessingContextScope.Style && bindingRelativeSource == "Self")
      {
        if (context.OuterContext != null)
          context1 = context.OuterContext.ParentContext;
      }
      else if (context.Scope == ProcessingContextScope.ControlTemplate && bindingRelativeSource == "TemplatedParent")
        context1 = this.GetTemplatedParentContext(context);
      if (context1 == null || PlatformTypes.DictionaryEntry.IsAssignableFrom((ITypeId) context1.DocumentNode.Type))
        return (RawDataSourceInfoBase) null;
      string bindingPath = DataContextHelper.GetBindingPath(context.DocumentCompositeNode);
      if (string.IsNullOrEmpty(bindingPath))
        return (RawDataSourceInfoBase) null;
      IList<ClrPathPart> parts = ClrPropertyPathHelper.SplitPath(bindingPath);
      if (parts == null || parts.Count == 0)
        return (RawDataSourceInfoBase) null;
      ClrPathPart clrPathPart = parts[0];
      if (clrPathPart.Category != ClrPathPartCategory.PropertyName)
        return (RawDataSourceInfoBase) null;
      RawDataSourceInfoBase sourceFromProperty = this.GetDataSourceFromProperty(context1, clrPathPart.Path);
      if (sourceFromProperty == null || !sourceFromProperty.IsValid)
        return (RawDataSourceInfoBase) null;
      string path = ClrPropertyPathHelper.CombinePathParts(parts, 1);
      sourceFromProperty.AppendClrPath(path);
      return sourceFromProperty;
    }

    private DataBindingProcessingContext GetTemplatedParentContext(DataBindingProcessingContext context)
    {
      DataBindingProcessingContext outerContext;
      for (outerContext = context.OuterContext; outerContext != null && outerContext.Scope == ProcessingContextScope.Style; outerContext = outerContext.OuterContext)
      {
        DocumentCompositeNode parentNode = outerContext.ParentNode;
        if (parentNode == null || !PlatformTypes.Setter.IsAssignableFrom((ITypeId) parentNode.Type))
          break;
      }
      return outerContext.ParentContext;
    }

    private string GetBindingRelativeSource(DataBindingProcessingContext context)
    {
      DocumentCompositeNode documentCompositeNode = context.DocumentCompositeNode.Properties[BindingSceneNode.RelativeSourceProperty] as DocumentCompositeNode;
      if (documentCompositeNode == null || documentCompositeNode.Properties.Count != 1)
        return (string) null;
      DocumentPrimitiveNode documentPrimitiveNode = documentCompositeNode.Properties[0] as DocumentPrimitiveNode;
      if (documentPrimitiveNode == null || !PlatformTypes.RelativeSourceMode.IsAssignableFrom((ITypeId) documentPrimitiveNode.Type))
        return (string) null;
      return documentPrimitiveNode.GetValue<string>();
    }

    private void ProcessDataContextPathExtension(DataBindingProcessingContext context, DocumentNode childNode, IProperty property)
    {
      if (!PlatformTypes.String.IsAssignableFrom((ITypeId) childNode.Type) || DataContextMetadata.GetDataContextAttribute<DataContextPathExtensionAttribute>(property) == null)
        return;
      DataBindingProcessingContext context1 = new DataBindingProcessingContext(context, childNode, property);
      context1.DataContext = this.InitDataContext(context1);
      if (!this.ShouldProcess(context1.DataContext))
        return;
      this.HandleDataContextPathExtension(context1, context1.DataContext);
    }

    private bool ShouldProcess(RawDataSourceInfoBase dataSource)
    {
      if (dataSource == null || !dataSource.IsValidClr)
        return false;
      ElementDataSourceInfo elementDataSourceInfo = dataSource as ElementDataSourceInfo;
      return elementDataSourceInfo != null && elementDataSourceInfo.TargetProperty == null || (!dataSource.HasSource || this.ShouldProcessDataSourceType(dataSource.SourceType));
    }

    private class PendingBindingInfo
    {
      public DataBindingProcessingContext BindingContext { get; private set; }

      public ElementDataSourceInfo BindingInfo { get; private set; }

      public PendingBindingInfo(DataBindingProcessingContext bindingContext, ElementDataSourceInfo bindingInfo)
      {
        this.BindingContext = bindingContext;
        this.BindingInfo = bindingInfo;
      }

      public override string ToString()
      {
        return this.BindingInfo.ToString();
      }
    }

    private class DataContextWalker : IDataContextAncestorWalker
    {
      private Dictionary<DocumentCompositeNode, DataBindingProcessingContext> namedElementDataContexts;
      private DataBindingProcessor.DataContextWalker.ContextLocation first;
      private DataBindingProcessor.DataContextWalker.ContextLocation moveTo;
      private DataBindingProcessor.DataContextWalker.ContextLocation current;
      private bool endOfAncestorChain;

      public DataBindingProcessingContext CurrentContext
      {
        get
        {
          if (this.current != null)
            return this.current.Context;
          return (DataBindingProcessingContext) null;
        }
      }

      public DocumentCompositeNode CurrentNode
      {
        get
        {
          if (this.current != null)
            return this.current.Node;
          return (DocumentCompositeNode) null;
        }
      }

      public IProperty CurrentProperty
      {
        get
        {
          if (this.current != null)
            return this.current.Property;
          return (IProperty) null;
        }
      }

      public DataContextWalker(DataBindingProcessingContext context, IProperty property, Dictionary<DocumentCompositeNode, DataBindingProcessingContext> namedElementDataContexts)
      {
        this.namedElementDataContexts = namedElementDataContexts;
        this.current = this.first = new DataBindingProcessor.DataContextWalker.ContextLocation(context, property);
      }

      public void Reset()
      {
        this.endOfAncestorChain = false;
        this.moveTo = (DataBindingProcessor.DataContextWalker.ContextLocation) null;
        this.current = (DataBindingProcessor.DataContextWalker.ContextLocation) null;
      }

      public bool MoveNext()
      {
        if (this.endOfAncestorChain)
          return false;
        if (this.moveTo != null)
        {
          this.current = this.moveTo;
          this.moveTo = (DataBindingProcessor.DataContextWalker.ContextLocation) null;
          return true;
        }
        if (this.MoveNextInternal(ref this.current))
          return true;
        this.Reset();
        this.endOfAncestorChain = true;
        return false;
      }

      public bool MoveTo(DocumentCompositeNode targetNode, IProperty targetProperty, bool makeCurrent)
      {
        this.endOfAncestorChain = false;
        this.moveTo = (DataBindingProcessor.DataContextWalker.ContextLocation) null;
        if (this.CurrentNode == targetNode)
        {
          this.moveTo = new DataBindingProcessor.DataContextWalker.ContextLocation(this.CurrentContext, targetProperty);
        }
        else
        {
          DataBindingProcessingContext context;
          if (this.namedElementDataContexts.TryGetValue(targetNode, out context))
            this.moveTo = new DataBindingProcessor.DataContextWalker.ContextLocation(context, targetProperty);
        }
        if (this.moveTo == null)
        {
          this.moveTo = this.current == null || this.current.Node == null ? new DataBindingProcessor.DataContextWalker.ContextLocation(this.first.Context, this.first.Property) : new DataBindingProcessor.DataContextWalker.ContextLocation(this.current.Context, this.current.Property);
          while (this.moveTo.Node != targetNode)
          {
            if (!this.MoveNextInternal(ref this.moveTo))
            {
              this.Reset();
              this.endOfAncestorChain = true;
              return false;
            }
          }
          this.moveTo = new DataBindingProcessor.DataContextWalker.ContextLocation(this.moveTo.Context, targetProperty);
        }
        if (makeCurrent)
          return this.MoveNext();
        return true;
      }

      private bool MoveNextInternal(ref DataBindingProcessor.DataContextWalker.ContextLocation location)
      {
        if (location == null)
        {
          location = new DataBindingProcessor.DataContextWalker.ContextLocation(this.first.Context, this.first.Property);
          return true;
        }
        if (location.Context.ParentContext != null)
        {
          DataBindingProcessingContext parentContext = location.Context.ParentContext;
          location = new DataBindingProcessor.DataContextWalker.ContextLocation(parentContext, location.Context.Property);
        }
        else
        {
          if (location.Context.OuterContext == null)
            return false;
          DataBindingProcessingContext outerContext = location.Context.OuterContext;
          location = new DataBindingProcessor.DataContextWalker.ContextLocation(outerContext, outerContext.Property);
        }
        return true;
      }

      public override string ToString()
      {
        string str1 = string.Empty;
        if (this.CurrentNode != null)
        {
          string str2 = str1 + " Current: ";
          if (this.CurrentProperty != null)
            str1 = string.Concat(new object[4]
            {
              (object) str2,
              (object) this.CurrentNode,
              (object) "+",
              (object) this.CurrentProperty.Name
            });
          else
            str1 = str2 + (object) this.CurrentNode;
        }
        return str1;
      }

      private class ContextLocation
      {
        public DataBindingProcessingContext Context { get; private set; }

        public IProperty Property { get; private set; }

        public DocumentCompositeNode Node
        {
          get
          {
            if (this.Context != null)
              return this.Context.DocumentCompositeNode;
            return (DocumentCompositeNode) null;
          }
        }

        public ContextLocation(DataBindingProcessingContext context, IProperty property)
        {
          this.Context = context;
          this.Property = property;
        }
      }
    }

    private class ResourceProcessingContext
    {
      private List<DataBindingProcessor.ResourceProcessingContext.Context> visitedContexts = new List<DataBindingProcessor.ResourceProcessingContext.Context>();

      public bool IsProcessing { get; private set; }

      public bool AddToProcessedContextsIfNeeded(DataBindingProcessingContext context)
      {
        DataBindingProcessor.ResourceProcessingContext.Context context1 = new DataBindingProcessor.ResourceProcessingContext.Context(context.DataContext.SourceNode, context.DataContext.NormalizedClrPath);
        if (this.visitedContexts.BinarySearch(context1, (IComparer<DataBindingProcessor.ResourceProcessingContext.Context>) DataBindingProcessor.ResourceProcessingContext.ContextComparer.Instance) >= 0)
          return false;
        this.visitedContexts.Add(context1);
        this.visitedContexts.Sort((IComparer<DataBindingProcessor.ResourceProcessingContext.Context>) DataBindingProcessor.ResourceProcessingContext.ContextComparer.Instance);
        return true;
      }

      public IDisposable StartProcessing()
      {
        return (IDisposable) new DataBindingProcessor.ResourceProcessingContext.InProcessToken(this);
      }

      private class Context
      {
        public DocumentNode SourceNode { get; private set; }

        public string ClrPath { get; private set; }

        public Context(DocumentNode sourceNode, string clrPath)
        {
          this.SourceNode = sourceNode;
          this.ClrPath = clrPath ?? string.Empty;
        }
      }

      private class ContextComparer : IComparer<DataBindingProcessor.ResourceProcessingContext.Context>
      {
        private static DataBindingProcessor.ResourceProcessingContext.ContextComparer instance = new DataBindingProcessor.ResourceProcessingContext.ContextComparer();

        public static DataBindingProcessor.ResourceProcessingContext.ContextComparer Instance
        {
          get
          {
            return DataBindingProcessor.ResourceProcessingContext.ContextComparer.instance;
          }
        }

        public int Compare(DataBindingProcessor.ResourceProcessingContext.Context a, DataBindingProcessor.ResourceProcessingContext.Context b)
        {
          int num = string.CompareOrdinal(a.ClrPath, b.ClrPath);
          if (num != 0)
            return num;
          if (a.SourceNode == b.SourceNode)
            return 0;
          return (a.SourceNode != null ? a.SourceNode.GetHashCode() : 0) < (b.SourceNode != null ? b.SourceNode.GetHashCode() : 0) ? -1 : 1;
        }
      }

      private class InProcessToken : IDisposable
      {
        private DataBindingProcessor.ResourceProcessingContext owner;

        public InProcessToken(DataBindingProcessor.ResourceProcessingContext owner)
        {
          this.owner = owner;
          this.owner.IsProcessing = true;
        }

        public void Dispose()
        {
          this.owner.IsProcessing = false;
          GC.SuppressFinalize((object) this);
        }
      }
    }
  }
}
