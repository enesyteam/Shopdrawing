// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.DataViewBuilder
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
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.Expression.DesignSurface.Tools
{
  public class DataViewBuilder
  {
    public static readonly ReadOnlyCollection<ITypeId> StringLikeTypes = new ReadOnlyCollection<ITypeId>((IList<ITypeId>) new List<ITypeId>()
    {
      PlatformTypes.Byte,
      PlatformTypes.SByte,
      PlatformTypes.Char,
      PlatformTypes.DateTime,
      PlatformTypes.Decimal,
      PlatformTypes.Double,
      PlatformTypes.Single,
      PlatformTypes.Enum,
      PlatformTypes.Int16,
      PlatformTypes.Int32,
      PlatformTypes.Int64,
      PlatformTypes.UInt16,
      PlatformTypes.UInt32,
      PlatformTypes.UInt64,
      PlatformTypes.String
    });
    private List<DataViewTemplate> dataViewTemplates = new List<DataViewTemplate>();
    private IDataViewLayoutBuilder layoutBuilder;

    public IPlatform Platform { get; private set; }

    public DataViewBuilder(IPlatform platform)
    {
      this.Platform = platform;
    }

    public DocumentCompositeNode GenerateDataView(IDocumentContext documentContext, IList<DataSchemaNodePath> entries, DataViewCategory category)
    {
      return this.GenerateDataView(documentContext, entries, category, (DocumentCompositeNode) null, (IDataViewLayoutBuilder) null);
    }

    public DocumentCompositeNode GenerateDataView(IDocumentContext documentContext, IList<DataSchemaNodePath> entries, DataViewCategory category, DocumentCompositeNode containerNode, IDataViewLayoutBuilder layoutBuilder)
    {
      try
      {
        DataViewTemplate dataViewTemplate = this.GetDataViewTemplate(category);
        if (containerNode == null)
          containerNode = (DocumentCompositeNode) dataViewTemplate.RootNode.Clone(documentContext);
        this.layoutBuilder = layoutBuilder != null ? layoutBuilder : (!PlatformTypes.Grid.IsAssignableFrom((ITypeId) containerNode.Type) ? (IDataViewLayoutBuilder) new PanelDataViewLayoutBuilder() : (IDataViewLayoutBuilder) new GridDataViewLayoutBuilder());
        DataViewBuilderContext context = new DataViewBuilderContext(containerNode, category);
        for (int index = 0; index < entries.Count; ++index)
          this.ProcessSingleSchemaPath(context, entries[index], dataViewTemplate);
      }
      catch (Exception ex)
      {
      }
      return containerNode;
    }

    public DataViewTemplateEntry GetDataViewTemplateEntry(IType dataType, DataViewCategory category)
    {
      DataViewTemplate dataViewTemplate = this.GetDataViewTemplate(category);
      return this.GetDataViewTemplateEntryInternal(dataType, dataViewTemplate);
    }

    private DataViewTemplate GetDataViewTemplate(DataViewCategory category)
    {
      DataViewTemplate dataViewTemplate = this.dataViewTemplates.Find((Predicate<DataViewTemplate>) (t => t.Category == category));
      if (dataViewTemplate == null)
      {
        dataViewTemplate = new DataViewTemplate(this.Platform, category);
        this.dataViewTemplates.Add(dataViewTemplate);
      }
      return dataViewTemplate;
    }

    private void ProcessSingleSchemaPath(DataViewBuilderContext context, DataSchemaNodePath schemaPath, DataViewTemplate dataViewTemplate)
    {
      DataViewTemplateEntry templateEntryInternal = this.GetDataViewTemplateEntryInternal(schemaPath.Type, dataViewTemplate);
      if (templateEntryInternal == null)
        return;
      context.CurrentSchemaPath = schemaPath;
      context.CurrentLabelNode = (DocumentCompositeNode) null;
      context.CurrentFieldNode = (DocumentCompositeNode) null;
      this.CreateLabelElement(context, templateEntryInternal);
      this.CreateFieldElement(context, templateEntryInternal);
      this.layoutBuilder.SetElementLayout(context);
    }

    private DataViewTemplateEntry GetDataViewTemplateEntryInternal(IType platformType, DataViewTemplate dataViewTemplate)
    {
      if (platformType == null)
        return (DataViewTemplateEntry) null;
      DataViewTemplateEntry viewTemplateEntry = dataViewTemplate.TemplateEntries.Find((Predicate<DataViewTemplateEntry>) (entry => entry.DataType.IsAssignableFrom((ITypeId) platformType)));
      if (viewTemplateEntry == null)
      {
        if (!DataViewBuilder.ShouldTreatAsStringType(platformType))
          return (DataViewTemplateEntry) null;
        platformType = this.Platform.Metadata.ResolveType(PlatformTypes.String);
        viewTemplateEntry = dataViewTemplate.TemplateEntries.Find((Predicate<DataViewTemplateEntry>) (entry => entry.DataType.IsAssignableFrom((ITypeId) platformType)));
      }
      return viewTemplateEntry;
    }

    private void CreateLabelElement(DataViewBuilderContext context, DataViewTemplateEntry templateEntry)
    {
      if (templateEntry.LabelNode == null)
        return;
      context.CurrentLabelNode = (DocumentCompositeNode) templateEntry.LabelNode.Clone(context.DocumentContext);
      string str = context.CurrentSchemaPath.Node.PathName.TrimStart('@', '/');
      context.CurrentLabelNode.Properties[(IPropertyId) templateEntry.LabelValueProperty] = (DocumentNode) context.DocumentContext.CreateNode(str);
    }

    private void CreateFieldElement(DataViewBuilderContext context, DataViewTemplateEntry templateEntry)
    {
      context.CurrentFieldNode = (DocumentCompositeNode) templateEntry.FieldNode.Clone(context.DocumentContext);
      DocumentCompositeNode documentCompositeNode = (DocumentCompositeNode) context.CurrentFieldNode.Properties[(IPropertyId) templateEntry.FieldValueProperty];
      string path = context.CurrentSchemaPath.Path;
      if (context.CurrentSchemaPath.Schema is XmlSchema)
      {
        string str = path ?? string.Empty;
        documentCompositeNode.Properties[BindingSceneNode.XPathProperty] = (DocumentNode) context.DocumentContext.CreateNode(str);
      }
      else if (!string.IsNullOrEmpty(path))
      {
        object obj = this.Platform.Metadata.MakePropertyPath(path);
        documentCompositeNode.Properties[BindingSceneNode.PathProperty] = context.DocumentContext.CreateNode(obj.GetType(), obj);
      }
      BindingModeInfo defaultBindingMode = BindingPropertyHelper.GetDefaultBindingMode((DocumentNode) templateEntry.FieldNode, (IPropertyId) templateEntry.FieldValueProperty, context.CurrentSchemaPath);
      if (defaultBindingMode.IsOptional)
        return;
      DocumentPrimitiveNode node = context.DocumentContext.CreateNode(PlatformTypes.BindingMode, (IDocumentNodeValue) new DocumentNodeStringValue(defaultBindingMode.Mode.ToString()));
      documentCompositeNode.Properties[BindingSceneNode.ModeProperty] = (DocumentNode) node;
    }

    private static bool ShouldTreatAsStringType(IType dataType)
    {
      return Enumerable.FirstOrDefault<ITypeId>((IEnumerable<ITypeId>) DataViewBuilder.StringLikeTypes, (Func<ITypeId, bool>) (t => t.IsAssignableFrom((ITypeId) dataType))) != null;
    }
  }
}
