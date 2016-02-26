// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SchemaManager
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.InstanceBuilders;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.Text;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.SampleData;
using Microsoft.Expression.DesignSurface.UserInterface.DataPane;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Project;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace Microsoft.Expression.DesignSurface
{
  internal sealed class SchemaManager : IDisposable
  {
    private ProjectXamlContext projectContext;
    private SchemaManager.WeakDocumentNodeStore<ISchema> schemaCache;
    private DesignDataSchemaManager designDataManager;

    public event EventHandler ClrObjectSchemasInvalidated;

    public event SampleDataChangedEventHandler SampleTypesChanging;

    public SchemaManager(ProjectXamlContext projectContext)
    {
      this.projectContext = projectContext;
      this.designDataManager = new DesignDataSchemaManager((IProjectContext) projectContext);
      this.schemaCache = new SchemaManager.WeakDocumentNodeStore<ISchema>();
      this.projectContext.TypesChanged += new EventHandler<TypesChangedEventArgs>(this.ProjectContext_TypesChanged);
      this.projectContext.DocumentClosing += new EventHandler<ProjectDocumentEventArgs>(this.ProjectContext_DocumentClosing);
      this.projectContext.SampleData.SampleDataAdded += new EventHandler<SampleDataEventArgs>(this.SampleData_Added);
      this.projectContext.SampleData.SampleDataRemoving += new EventHandler<SampleDataEventArgs>(this.SampleData_Removing);
      foreach (SampleDataSet sampleDataSet in this.projectContext.SampleData.SampleDataSetCollection)
      {
        sampleDataSet.SampleTypesChanged += new SampleDataChangedEventHandler(this.SampleDataSet_TypesChanged);
        sampleDataSet.SampleTypesChanging += new SampleDataChangedEventHandler(this.SampleDataSet_TypesChanging);
      }
    }

    public ISchema GetSchemaForDesignDataFile(IProjectItem designDataFile)
    {
      return this.designDataManager.GetSchemaForDesignDataFile(designDataFile, (IProjectContext) this.projectContext);
    }

    public static ISchema GetSchemaForType(Type type, DocumentNode dataSourceNode)
    {
      return (ISchema) new ClrObjectSchema(type, dataSourceNode);
    }

    public static ISchema GetSchemaForDataSource(DocumentNode dataSource)
    {
      string errorMessage;
      return ProjectXamlContext.FromDocumentNode(dataSource).SchemaManager.GetSchemaForDataSourceInternal(dataSource, out errorMessage);
    }

    public static ISchema GetSchemaForDataSource(SceneNode dataSource)
    {
      string errorMessage;
      return ProjectXamlContext.FromSceneNode(dataSource).SchemaManager.GetSchemaForDataSourceInternal(dataSource.DocumentNode, out errorMessage);
    }

    public static ISchema GetSchemaForDataSourceInfo(DataSourceInfo dataSource)
    {
      if (dataSource == null || !dataSource.IsValidWithSource)
        return (ISchema) new EmptySchema();
      string errorMessage;
      ISchema schema = ProjectXamlContext.FromDocumentNode(dataSource.SourceNode).SchemaManager.GetSchemaForDataSourceInternal(dataSource.SourceNode, out errorMessage);
      if (schema != null && !string.IsNullOrEmpty(dataSource.Path))
      {
        DataSchemaNodePath nodePathFromPath = schema.GetNodePathFromPath(dataSource.Path);
        if (nodePathFromPath != null)
          schema = nodePathFromPath.RelativeSchema;
      }
      return schema;
    }

    public static ISchema GetSchemaForDataSource(DocumentNode dataSource, out string errorMessage)
    {
      return ProjectXamlContext.FromDocumentNode(dataSource).SchemaManager.GetSchemaForDataSourceInternal(dataSource, out errorMessage);
    }

    public ISchema GetSchemaForDataSourceInternal(DocumentNode dataSource, out string errorMessage)
    {
      errorMessage = string.Empty;
      ISchema schema = (ISchema) null;
      IProjectItem designDataFile = DesignDataHelper.GetDesignDataFile(dataSource);
      if (designDataFile != null)
        return this.GetSchemaForDesignDataFile(designDataFile);
      if (this.schemaCache.TryGetValue(dataSource, out schema))
      {
        ClrObjectSchema clrObjectSchema = schema as ClrObjectSchema;
        if (clrObjectSchema != null && clrObjectSchema.Root.Type != DataContextHelper.GetDataType(clrObjectSchema.DataSource.DocumentNode).RuntimeType)
        {
          this.schemaCache.Remove(dataSource);
          schema = (ISchema) null;
        }
        if (schema != null)
          return schema;
      }
      if (dataSource != null)
      {
        if (PlatformTypes.XmlDataProvider.IsAssignableFrom((ITypeId) dataSource.Type))
        {
          Microsoft.Expression.DesignSurface.UserInterface.DataPane.XmlSchema xmlSchema = (Microsoft.Expression.DesignSurface.UserInterface.DataPane.XmlSchema) null;
          try
          {
            Uri uriPropertyValue1 = this.GetUriPropertyValue(dataSource, DesignTimeProperties.SchemaUriProperty);
            if (uriPropertyValue1 != (Uri) null)
            {
              xmlSchema = this.GetXmlSchemaFromSchemaFile(uriPropertyValue1, dataSource, out errorMessage);
            }
            else
            {
              Uri uriPropertyValue2 = this.GetUriPropertyValue(dataSource, XmlDataProviderSceneNode.SourceProperty);
              if (uriPropertyValue2 != (Uri) null)
              {
                using (XmlReader xmlReader = XmlReader.Create(uriPropertyValue2.AbsoluteUri))
                  xmlSchema = this.InferXmlSchemaFromXmlFile(xmlReader, dataSource, out errorMessage);
              }
              else
              {
                string inlineXmlData = this.GetInlineXmlData(dataSource);
                if (inlineXmlData != null)
                {
                  using (StringReader stringReader = new StringReader(inlineXmlData))
                  {
                    using (XmlReader xmlReader = XmlReader.Create((TextReader) stringReader))
                      xmlSchema = this.InferXmlSchemaFromXmlFile(xmlReader, dataSource, out errorMessage);
                  }
                }
              }
            }
          }
          catch
          {
          }
          if (xmlSchema == null)
            xmlSchema = new Microsoft.Expression.DesignSurface.UserInterface.DataPane.XmlSchema(dataSource);
          schema = (ISchema) xmlSchema;
        }
        else
          schema = SchemaManager.GetSchemaForType(this.GetTypeForDataSource(dataSource), dataSource);
      }
      if (schema == null)
        schema = (ISchema) new EmptySchema();
      if (dataSource.DocumentRoot != null)
        this.schemaCache[dataSource] = schema;
      return schema;
    }

    private string GetInlineXmlData(DocumentNode dataSource)
    {
      DocumentCompositeNode documentCompositeNode1 = dataSource as DocumentCompositeNode;
      if (documentCompositeNode1 == null)
        return (string) null;
      DocumentCompositeNode documentCompositeNode2 = documentCompositeNode1.Properties[XmlDataProviderSceneNode.XmlSerializerProperty] as DocumentCompositeNode;
      if (documentCompositeNode2 == null)
        return (string) null;
      DocumentNode node = documentCompositeNode2.Properties[DesignTimeProperties.InlineXmlProperty];
      if (node == null)
        return (string) null;
      using (StandaloneInstanceBuilderContext instanceBuilderContext = new StandaloneInstanceBuilderContext(node.Context, (IViewRootResolver) null, (ITextBufferService) null))
      {
        using (instanceBuilderContext.DisablePostponedResourceEvaluation())
        {
          instanceBuilderContext.ViewNodeManager.RootNodePath = new DocumentNodePath(node.DocumentRoot.RootNode, node);
          instanceBuilderContext.ViewNodeManager.Instantiate(instanceBuilderContext.ViewNodeManager.Root);
        }
        return instanceBuilderContext.ViewNodeManager.Root.Instance as string;
      }
    }

    private Type GetTypeForDataSource(DocumentNode dataSource)
    {
      IType dataType = DataContextHelper.GetDataType(dataSource);
      if (dataType.RuntimeType == (Type) null)
        return typeof (object);
      return dataType.RuntimeType;
    }

    private Microsoft.Expression.DesignSurface.UserInterface.DataPane.XmlSchema InferXmlSchemaFromXmlFile(XmlReader xmlReader, DocumentNode dataSource, out string errorMessage)
    {
      errorMessage = string.Empty;
      Microsoft.Expression.DesignSurface.UserInterface.DataPane.XmlSchema xmlSchema = (Microsoft.Expression.DesignSurface.UserInterface.DataPane.XmlSchema) null;
      try
      {
        xmlSchema = new Microsoft.Expression.DesignSurface.UserInterface.DataPane.XmlSchema(new XmlSchemaInference().InferSchema(xmlReader), dataSource);
        return xmlSchema;
      }
      finally
      {
        if (xmlSchema == null)
          errorMessage = StringTable.XmlDataSourceSchemaInferenceError;
      }
    }

    private Microsoft.Expression.DesignSurface.UserInterface.DataPane.XmlSchema GetXmlSchemaFromSchemaFile(Uri schemaFile, DocumentNode dataSource, out string errorMessage)
    {
      errorMessage = string.Empty;
      Microsoft.Expression.DesignSurface.UserInterface.DataPane.XmlSchema xmlSchema = (Microsoft.Expression.DesignSurface.UserInterface.DataPane.XmlSchema) null;
      try
      {
        bool isSchemaValid = true;
        using (XmlReader reader = XmlReader.Create(schemaFile.ToString()))
        {
          System.Xml.Schema.XmlSchema schema = System.Xml.Schema.XmlSchema.Read(reader, (ValidationEventHandler) ((sender, e) => isSchemaValid = false));
          if (schema != null)
          {
            if (isSchemaValid)
            {
              XmlSchemaSet schemaSet = new XmlSchemaSet();
              schemaSet.Add(schema);
              schemaSet.Compile();
              xmlSchema = new Microsoft.Expression.DesignSurface.UserInterface.DataPane.XmlSchema(schemaSet, dataSource);
            }
          }
        }
      }
      finally
      {
        if (xmlSchema == null)
          errorMessage = StringTable.XmlDataSourceSchemaReadError;
      }
      return xmlSchema;
    }

    private Uri GetUriPropertyValue(DocumentNode node, IPropertyId propertyKey)
    {
      DocumentCompositeNode documentCompositeNode = node as DocumentCompositeNode;
      if (documentCompositeNode != null)
      {
        DocumentPrimitiveNode documentPrimitiveNode = documentCompositeNode.Properties[propertyKey] as DocumentPrimitiveNode;
        if (documentPrimitiveNode != null)
        {
          DocumentNodeStringValue documentNodeStringValue = documentPrimitiveNode.Value as DocumentNodeStringValue;
          if (documentNodeStringValue != null)
          {
            Uri result = (Uri) null;
            if (Uri.TryCreate(documentNodeStringValue.Value, UriKind.RelativeOrAbsolute, out result))
              return node.Context.MakeDesignTimeUri(result);
          }
        }
      }
      return (Uri) null;
    }

    private void ProjectContext_TypesChanged(object sender, TypesChangedEventArgs e)
    {
      this.Refresh((SampleDataSet) null);
    }

    private void Refresh(SampleDataSet changedSampleData)
    {
      List<DocumentNode> list = (List<DocumentNode>) null;
      foreach (KeyValuePair<DocumentNode, ISchema> keyValuePair in this.schemaCache)
      {
        ClrObjectSchema clrObjectSchema = keyValuePair.Value as ClrObjectSchema;
        if (clrObjectSchema != null)
        {
          Type typeForDataSource = this.GetTypeForDataSource(keyValuePair.Key);
          if (typeForDataSource != (Type) null && clrObjectSchema.Root.Type != typeForDataSource)
          {
            if (list == null)
              list = new List<DocumentNode>();
            list.Add(keyValuePair.Key);
          }
        }
      }
      if (list != null)
      {
        foreach (DocumentNode key in list)
          this.schemaCache.Remove(key);
      }
      this.designDataManager.Refresh(changedSampleData);
      if (this.ClrObjectSchemasInvalidated == null)
        return;
      this.ClrObjectSchemasInvalidated((object) this, EventArgs.Empty);
    }

    private void ProjectContext_DocumentClosing(object sender, ProjectDocumentEventArgs e)
    {
      if (e.Document == null || e.Document.DocumentRoot == null)
        return;
      this.schemaCache.Clear(e.Document.DocumentRoot);
    }

    private void SampleData_Added(object sender, SampleDataEventArgs args)
    {
      args.SampleData.SampleTypesChanged += new SampleDataChangedEventHandler(this.SampleDataSet_TypesChanged);
      args.SampleData.SampleTypesChanging += new SampleDataChangedEventHandler(this.SampleDataSet_TypesChanging);
    }

    private void SampleData_Removing(object sender, SampleDataEventArgs args)
    {
      args.SampleData.SampleTypesChanged -= new SampleDataChangedEventHandler(this.SampleDataSet_TypesChanged);
      args.SampleData.SampleTypesChanging -= new SampleDataChangedEventHandler(this.SampleDataSet_TypesChanging);
    }

    private void SampleDataSet_TypesChanged(SampleDataSet sender, EventArgs e)
    {
      this.Refresh(sender);
    }

    private void SampleDataSet_TypesChanging(SampleDataSet sender, EventArgs e)
    {
      if (this.SampleTypesChanging == null)
        return;
      this.SampleTypesChanging(sender, e);
    }

    public void Dispose()
    {
      if (this.projectContext != null)
      {
        this.projectContext.TypesChanged -= new EventHandler<TypesChangedEventArgs>(this.ProjectContext_TypesChanged);
        this.projectContext.DocumentClosing -= new EventHandler<ProjectDocumentEventArgs>(this.ProjectContext_DocumentClosing);
        this.projectContext.SampleData.SampleDataAdded -= new EventHandler<SampleDataEventArgs>(this.SampleData_Added);
        this.projectContext.SampleData.SampleDataRemoving -= new EventHandler<SampleDataEventArgs>(this.SampleData_Removing);
        foreach (SampleDataSet sampleDataSet in this.projectContext.SampleData.SampleDataSetCollection)
        {
          sampleDataSet.SampleTypesChanged -= new SampleDataChangedEventHandler(this.SampleDataSet_TypesChanged);
          sampleDataSet.SampleTypesChanging -= new SampleDataChangedEventHandler(this.SampleDataSet_TypesChanging);
        }
        this.projectContext = (ProjectXamlContext) null;
        this.designDataManager.Dispose();
      }
      GC.SuppressFinalize((object) this);
    }

    private sealed class WeakDocumentNodeStore<T> : IEnumerable<KeyValuePair<DocumentNode, T>>, IEnumerable where T : class
    {
      private Dictionary<IDocumentRoot, Dictionary<DocumentNode, WeakReference>> dictionary;

      public T this[DocumentNode key]
      {
        get
        {
          Dictionary<DocumentNode, WeakReference> targetDictionary;
          if (key.DocumentRoot != null && this.dictionary.TryGetValue(key.DocumentRoot, out targetDictionary))
            return this.EnsureValidOrRemove(targetDictionary, key, targetDictionary[key]);
          return default (T);
        }
        set
        {
          Dictionary<DocumentNode, WeakReference> dictionary;
          if (!this.dictionary.TryGetValue(key.DocumentRoot, out dictionary))
          {
            dictionary = new Dictionary<DocumentNode, WeakReference>();
            this.dictionary[key.DocumentRoot] = dictionary;
          }
          dictionary[key] = new WeakReference((object) value);
        }
      }

      public WeakDocumentNodeStore()
      {
        this.dictionary = new Dictionary<IDocumentRoot, Dictionary<DocumentNode, WeakReference>>();
      }

      public void Add(DocumentNode key, T value)
      {
        Dictionary<DocumentNode, WeakReference> dictionary;
        if (!this.dictionary.TryGetValue(key.DocumentRoot, out dictionary))
        {
          dictionary = new Dictionary<DocumentNode, WeakReference>();
          this.dictionary[key.DocumentRoot] = dictionary;
        }
        dictionary.Add(key, new WeakReference((object) value));
      }

      public bool ContainsKey(DocumentNode key)
      {
        Dictionary<DocumentNode, WeakReference> targetDictionary;
        WeakReference weakReference;
        if (key.DocumentRoot != null && this.dictionary.TryGetValue(key.DocumentRoot, out targetDictionary) && targetDictionary.TryGetValue(key, out weakReference))
          return (object) this.EnsureValidOrRemove(targetDictionary, key, weakReference) != null;
        return false;
      }

      public bool Remove(DocumentNode key)
      {
        Dictionary<DocumentNode, WeakReference> dictionary;
        if (key.DocumentRoot != null && this.dictionary.TryGetValue(key.DocumentRoot, out dictionary))
          return dictionary.Remove(key);
        return false;
      }

      public void Clear()
      {
        this.dictionary.Clear();
      }

      public void Clear(IDocumentRoot documentRoot)
      {
        this.dictionary.Remove(documentRoot);
      }

      public bool TryGetValue(DocumentNode key, out T value)
      {
        Dictionary<DocumentNode, WeakReference> targetDictionary;
        WeakReference weakReference;
        if (key.DocumentRoot != null && this.dictionary.TryGetValue(key.DocumentRoot, out targetDictionary) && (targetDictionary != null && targetDictionary.TryGetValue(key, out weakReference)))
        {
          value = this.EnsureValidOrRemove(targetDictionary, key, weakReference);
          return (object) value != null;
        }
        value = default (T);
        return false;
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return (IEnumerator) this.GetEnumerator();
      }

      public IEnumerator<KeyValuePair<DocumentNode, T>> GetEnumerator()
      {
        foreach (KeyValuePair<IDocumentRoot, Dictionary<DocumentNode, WeakReference>> keyValuePair1 in this.dictionary)
        {
          Dictionary<DocumentNode, WeakReference> documentDictionary = keyValuePair1.Value;
          List<DocumentNode> deadKeys = (List<DocumentNode>) null;
          foreach (KeyValuePair<DocumentNode, WeakReference> keyValuePair2 in documentDictionary)
          {
            if (!this.ValidateEntryCore(keyValuePair2.Key, keyValuePair2.Value))
            {
              if (deadKeys == null)
                deadKeys = new List<DocumentNode>();
              deadKeys.Add(keyValuePair2.Key);
            }
            else
              yield return new KeyValuePair<DocumentNode, T>(keyValuePair2.Key, keyValuePair2.Value.Target as T);
          }
          if (deadKeys != null)
          {
            foreach (DocumentNode key in deadKeys)
              documentDictionary.Remove(key);
          }
        }
      }

      private T EnsureValidOrRemove(Dictionary<DocumentNode, WeakReference> targetDictionary, DocumentNode key, WeakReference weakReference)
      {
        if (this.ValidateEntryCore(key, weakReference))
          return (T) weakReference.Target;
        targetDictionary.Remove(key);
        return default (T);
      }

      private bool ValidateEntryCore(DocumentNode key, WeakReference weakReference)
      {
        if (key.IsInDocument)
          return weakReference.IsAlive;
        return false;
      }
    }
  }
}
