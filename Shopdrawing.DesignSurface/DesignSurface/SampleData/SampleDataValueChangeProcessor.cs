// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SampleData.SampleDataValueChangeProcessor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Documents;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.SampleData
{
  public class SampleDataValueChangeProcessor
  {
    private int changeCount;
    private SampleDataValueBuilderBase valueBuilder;

    private SampleDataValueChangeProcessor()
    {
    }

    public static DocumentCompositeNode ApplyChanges(IList<SampleDataChange> normalizedChanges, DocumentCompositeNode rootNode, IProjectDocument projectDocument, bool editInPlace)
    {
      if (normalizedChanges.Count == 0 || rootNode == null)
        return (DocumentCompositeNode) null;
      return new SampleDataValueChangeProcessor().ApplyChangesInternal(normalizedChanges, rootNode, projectDocument, editInPlace);
    }

    private DocumentCompositeNode ApplyChangesInternal(IList<SampleDataChange> normalizedChanges, DocumentCompositeNode rootNode, IProjectDocument projectDocument, bool editInPlace)
    {
      this.valueBuilder = new SampleDataValueBuilderBase(normalizedChanges[0].DeclaringDataSet, rootNode.Context);
      DocumentCompositeNode rootNode1 = rootNode;
      using (((SceneDocument) projectDocument.Document).CreateEditTransaction("", true))
      {
        this.ProcessDeletedTypes(normalizedChanges, rootNode1);
        this.ProcessDeletedProperties(normalizedChanges, rootNode1);
        this.ProcessCreatedProperties(normalizedChanges, rootNode1);
        this.ProcessPropertyTypeOrFormatChanges(normalizedChanges, rootNode1);
        if (!editInPlace)
        {
          if (this.changeCount > 0)
          {
            rootNode1 = (DocumentCompositeNode) rootNode.Clone(rootNode1.Context);
            rootNode1.SourceContext = (INodeSourceContext) null;
          }
        }
      }
      return rootNode1;
    }

    private void ProcessDeletedTypes(IList<SampleDataChange> normalizedChanges, DocumentCompositeNode rootNode)
    {
      for (int index = 0; index < normalizedChanges.Count; ++index)
      {
        SampleTypeDeleted sampleTypeDeleted = normalizedChanges[index] as SampleTypeDeleted;
        if (sampleTypeDeleted != null)
          this.RemoveNodesByType(rootNode, sampleTypeDeleted.SampleType);
      }
    }

    private void RemoveNodesByType(DocumentCompositeNode compositeNode, SampleNonBasicType deletedType)
    {
      if (compositeNode == null)
        return;
      List<IProperty> list = new List<IProperty>((IEnumerable<IProperty>) compositeNode.Properties.Keys);
      for (int index = 0; index < list.Count; ++index)
      {
        IProperty property = list[index];
        DocumentNode documentNode = compositeNode.Properties[(IPropertyId) property];
        if (documentNode.Type == deletedType)
        {
          compositeNode.ClearValue((IPropertyId) property);
          ++this.changeCount;
        }
        else
          this.RemoveNodesByType(documentNode as DocumentCompositeNode, deletedType);
      }
      if (!compositeNode.SupportsChildren)
        return;
      for (int index = 0; index < compositeNode.Children.Count; ++index)
        this.RemoveNodesByType(compositeNode.Children[index] as DocumentCompositeNode, deletedType);
    }

    private void ProcessDeletedProperties(IList<SampleDataChange> normalizedChanges, DocumentCompositeNode rootNode)
    {
      for (int index = 0; index < normalizedChanges.Count; ++index)
      {
        SamplePropertyDeleted samplePropertyDeleted = normalizedChanges[index] as SamplePropertyDeleted;
        if (samplePropertyDeleted != null)
          this.RemoveNodesByProperty(rootNode, samplePropertyDeleted.SampleProperty);
      }
    }

    private void RemoveNodesByProperty(DocumentCompositeNode compositeNode, SampleProperty deletedProperty)
    {
      if (compositeNode == null)
        return;
      if (compositeNode.Type == deletedProperty.DeclaringSampleType)
      {
        if (this.changeCount == 0 && compositeNode.Properties[(IPropertyId) deletedProperty] != null)
          ++this.changeCount;
        compositeNode.ClearValue((IPropertyId) deletedProperty);
      }
      for (int index = compositeNode.Properties.Count - 1; index >= 0; --index)
        this.RemoveNodesByProperty(compositeNode.Properties[index] as DocumentCompositeNode, deletedProperty);
      if (!compositeNode.SupportsChildren)
        return;
      for (int index = 0; index < compositeNode.Children.Count; ++index)
        this.RemoveNodesByProperty(compositeNode.Children[index] as DocumentCompositeNode, deletedProperty);
    }

    private void ProcessCreatedProperties(IList<SampleDataChange> normalizedChanges, DocumentCompositeNode rootNode)
    {
      for (int index = 0; index < normalizedChanges.Count; ++index)
      {
        SamplePropertyCreated samplePropertyCreated = normalizedChanges[index] as SamplePropertyCreated;
        if (samplePropertyCreated != null)
          this.AddPropertyValues(rootNode, samplePropertyCreated.SampleProperty);
      }
    }

    private void AddPropertyValues(DocumentCompositeNode compositeNode, SampleProperty newProperty)
    {
      if (compositeNode == null)
        return;
      if (compositeNode.SupportsChildren)
      {
        for (int index = 0; index < compositeNode.Children.Count; ++index)
          this.AddPropertyValues(compositeNode.Children[index] as DocumentCompositeNode, newProperty);
      }
      for (int index = 0; index < compositeNode.Properties.Count; ++index)
        this.AddPropertyValues(compositeNode.Properties[index] as DocumentCompositeNode, newProperty);
      if (compositeNode.Type != newProperty.DeclaringSampleType)
        return;
      this.CreatePropertyValue(compositeNode, newProperty);
      ++this.changeCount;
    }

    private void ProcessPropertyTypeOrFormatChanges(IList<SampleDataChange> normalizedChanges, DocumentCompositeNode rootNode)
    {
      for (int index = 0; index < normalizedChanges.Count; ++index)
      {
        SamplePropertyTypeOrFormatChanged change = normalizedChanges[index] as SamplePropertyTypeOrFormatChanged;
        if (change != null)
          this.UpdatePropertyValues(rootNode, change);
      }
    }

    private void UpdatePropertyValues(DocumentCompositeNode compositeNode, SamplePropertyTypeOrFormatChanged change)
    {
      if (compositeNode == null)
        return;
      SampleProperty sampleProperty = change.SampleProperty;
      if (compositeNode.Type == sampleProperty.DeclaringSampleType)
      {
        if (change.OldType == SampleBasicType.Image)
        {
          DocumentPrimitiveNode documentPrimitiveNode = compositeNode.Properties[(IPropertyId) sampleProperty] as DocumentPrimitiveNode;
          if (documentPrimitiveNode != null)
          {
            string relativePath = documentPrimitiveNode.GetValue<string>();
            sampleProperty.DeclaringDataSet.UnuseAssetFile(relativePath);
          }
        }
        this.CreatePropertyValue(compositeNode, sampleProperty);
        ++this.changeCount;
      }
      for (int index = 0; index < compositeNode.Properties.Count; ++index)
        this.UpdatePropertyValues(compositeNode.Properties[index] as DocumentCompositeNode, change);
      if (!compositeNode.SupportsChildren)
        return;
      for (int index = 0; index < compositeNode.Children.Count; ++index)
        this.UpdatePropertyValues(compositeNode.Children[index] as DocumentCompositeNode, change);
    }

    private void CreatePropertyValue(DocumentCompositeNode compositeNode, SampleProperty sampleProperty)
    {
      this.valueBuilder.InitCollectionDepth((DocumentNode) compositeNode);
      DocumentNode documentNode = this.valueBuilder.GeneratePropertyValue(sampleProperty);
      compositeNode.Properties[(IPropertyId) sampleProperty] = documentNode;
    }
  }
}
