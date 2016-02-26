// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.DataViewFactory
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.UserInterface.DataPane;
using Microsoft.Expression.DesignSurface.UserInterface.ResourcePane;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.Tools
{
  public static class DataViewFactory
  {
    public static DocumentCompositeNode GenerateDataView(IPlatform platform, IDocumentContext documentContext, IList<DataSchemaNodePath> entries, DataViewCategory category)
    {
      return DataViewFactory.GenerateDataView(platform, documentContext, entries, category, (DocumentCompositeNode) null, (IDataViewLayoutBuilder) null);
    }

    public static DocumentCompositeNode GenerateDataView(IPlatform platform, IDocumentContext documentContext, IList<DataSchemaNodePath> entries, DataViewCategory category, DocumentCompositeNode containerNode, IDataViewLayoutBuilder layoutBuilder)
    {
      DataViewBuilder dataViewBuilder = new DataViewBuilder(platform);
      return dataViewBuilder != null ? dataViewBuilder.GenerateDataView(documentContext, entries, category, containerNode, layoutBuilder) : (DocumentCompositeNode) null;
    }

    public static DocumentCompositeNode CreateDataTemplateResource(SceneNode targetElement, IPropertyId templateProperty, string resourceNameBase, IList<DataSchemaNodePath> entries, DataViewCategory category, ITypeId dataTemplateType)
    {
      SceneViewModel viewModel = targetElement.ViewModel;
      IPlatform platform = targetElement.Platform;
      IDocumentContext documentContext = viewModel.Document.DocumentContext;
      DocumentCompositeNode documentCompositeNode = DataViewFactory.GenerateDataView(platform, documentContext, entries, category);
      if (documentCompositeNode == null)
        return (DocumentCompositeNode) null;
      DocumentCompositeNode node = documentContext.CreateNode(dataTemplateType);
      node.Properties[(IPropertyId) node.Type.Metadata.DefaultContentProperty] = (DocumentNode) documentCompositeNode;
      CreateResourceModel createResourceModel = new CreateResourceModel(viewModel, viewModel.DesignerContext.ResourceManager, node.Type.RuntimeType, (Type) null, (string) null, (SceneElement) null, (SceneNode) null, CreateResourceModel.ContextFlags.None);
      createResourceModel.KeyString = resourceNameBase;
      bool useStaticResource = !JoltHelper.TypeSupported((ITypeResolver) viewModel.ProjectContext, PlatformTypes.DynamicResource);
      if (!useStaticResource && !PlatformTypes.FrameworkElement.IsAssignableFrom((ITypeId) targetElement.Type))
        useStaticResource = true;
      int index = useStaticResource ? createResourceModel.IndexInResourceSite(targetElement.DocumentNode) : -1;
      DocumentCompositeNode resource = createResourceModel.CreateResource((DocumentNode) node, (IPropertyId) null, index);
      if (templateProperty != null)
      {
        DocumentNode resourceReference = createResourceModel.CreateResourceReference(documentContext, resource, useStaticResource);
        using (viewModel.AnimationEditor.DeferKeyFraming())
          targetElement.SetValue(templateProperty, (object) resourceReference);
      }
      return (DocumentCompositeNode) resource.Properties[DictionaryEntryNode.ValueProperty];
    }

    public static DataViewTemplateEntry GetDataViewTemplateEntry(IPlatform platform, IType dataType, DataViewCategory category)
    {
      return new DataViewBuilder(platform).GetDataViewTemplateEntry(dataType, category);
    }
  }
}
