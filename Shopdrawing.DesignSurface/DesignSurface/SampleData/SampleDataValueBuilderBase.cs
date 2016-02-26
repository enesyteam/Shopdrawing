// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SampleData.SampleDataValueBuilderBase
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.UserInterface.DataPane;
using Microsoft.Expression.Framework.Extensions.Enumerable;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.Expression.DesignSurface.SampleData
{
  public class SampleDataValueBuilderBase
  {
    private static ReadOnlyCollection<int> DefaultCollectionCounts = new ReadOnlyCollection<int>((IList<int>) new List<int>()
    {
      10,
      5,
      2,
      0
    });
    private SampleDataValueBuilderBase.BuildingStack buildingStack = new SampleDataValueBuilderBase.BuildingStack();
    private int collectionDepth = -1;
    private ReadOnlyCollection<int> collectionCounts = SampleDataValueBuilderBase.DefaultCollectionCounts;
    private RandomValueGenerator valueGenerator;

    public SampleDataSet DataSet { get; private set; }

    public IDocumentContext XamlDocumentContext { get; private set; }

    private int CollectionCount
    {
      get
      {
        return this.collectionCounts[Math.Max(0, Math.Min(this.collectionDepth, this.collectionCounts.Count - 1))];
      }
    }

    public ReadOnlyCollection<int> CollectionCounts
    {
      get
      {
        return this.collectionCounts;
      }
      set
      {
        if (value == null || value.Count == 0)
          this.collectionCounts = SampleDataValueBuilderBase.DefaultCollectionCounts;
        else
          this.collectionCounts = value;
      }
    }

    public SampleDataValueBuilderBase(SampleDataSet dataSet, IDocumentContext xamlDocumentContext)
    {
      this.DataSet = dataSet;
      this.XamlDocumentContext = xamlDocumentContext;
      this.valueGenerator = new RandomValueGenerator(dataSet);
    }

    public void InitCollectionDepth(DocumentNode documentNode)
    {
      this.collectionDepth = 0;
      for (DocumentNode documentNode1 = documentNode; documentNode1.Parent != null; documentNode1 = (DocumentNode) documentNode1.Parent)
      {
        if (documentNode1.IsChild)
          ++this.collectionDepth;
      }
    }

    public void InitCollectionDepth(DataSchemaNodePath schemaPath)
    {
      this.collectionDepth = schemaPath.CollectionDepth;
    }

    public void AutoGenerateValues()
    {
      this.collectionDepth = 0;
      this.GenerateValue((SampleType) this.DataSet.RootType);
    }

    public DocumentNode GeneratePropertyValue(SampleProperty sampleProperty)
    {
      return !sampleProperty.IsBasicType ? this.GenerateNonBasicValue((SampleNonBasicType) sampleProperty.PropertySampleType) : this.GenerateBasicValue((SampleBasicType) sampleProperty.PropertySampleType, sampleProperty.Format, sampleProperty.FormatParameters);
    }

    public DocumentNode GenerateValue(SampleType sampleType)
    {
      return !sampleType.IsBasicType ? this.GenerateNonBasicValue((SampleNonBasicType) sampleType) : this.GenerateBasicValue((SampleBasicType) sampleType, (string) null, (string) null);
    }

    private DocumentNode GenerateBasicValue(SampleBasicType sampleType, string format, string formatParameters)
    {
      string randomValue = this.valueGenerator.GetRandomValue(sampleType, format, formatParameters);
      return randomValue != null ? (DocumentNode) this.CreateBasicNode(sampleType, randomValue) : (DocumentNode) null;
    }

    private DocumentNode GenerateNonBasicValue(SampleNonBasicType sampleType)
    {
      DocumentCompositeNode compositeNode = this.CreateCompositeNode(sampleType);
      if (sampleType.IsCollection)
      {
        SampleCollectionType sampleCollectionType = (SampleCollectionType) sampleType;
        if (this.buildingStack.PushCollectionItemType((SampleType) sampleCollectionType))
        {
          int collectionCount = this.CollectionCount;
          ++this.collectionDepth;
          for (int index = 0; index < collectionCount; ++index)
          {
            DocumentNode documentNode = this.GenerateValue(sampleCollectionType.ItemSampleType);
            compositeNode.Children.Add(documentNode);
          }
          --this.collectionDepth;
          this.buildingStack.Pop();
        }
      }
      else
      {
        foreach (SampleProperty sampleProperty in (IEnumerable<SampleProperty>) ((SampleCompositeType) sampleType).SampleProperties)
        {
          if (this.buildingStack.PushProperty(sampleProperty))
          {
            DocumentNode documentNode = this.GeneratePropertyValue(sampleProperty);
            if (documentNode != null)
              compositeNode.Properties[(IPropertyId) sampleProperty] = documentNode;
            this.buildingStack.Pop();
          }
        }
      }
      return (DocumentNode) compositeNode;
    }

    public DocumentNode CreatePropertyValue(DocumentCompositeNode documentNode, string propertyName)
    {
      return this.CreatePropertyValue(documentNode, propertyName, (string) null);
    }

    public DocumentNode CreatePropertyValue(DocumentCompositeNode documentNode, string propertyName, string value)
    {
      SampleCompositeType sampleCompositeType = documentNode.Type as SampleCompositeType;
      if (sampleCompositeType == null)
        return (DocumentNode) null;
      SampleProperty sampleProperty = sampleCompositeType.GetSampleProperty(propertyName);
      if (sampleProperty == null)
        return (DocumentNode) null;
      return this.CreatePropertyValueInternal(documentNode, sampleProperty, value);
    }

    public DocumentNode CreatePropertyValue(DocumentCompositeNode documentNode, SampleProperty sampleProperty)
    {
      return this.CreatePropertyValue(documentNode, sampleProperty, (string) null);
    }

    public DocumentNode CreatePropertyValue(DocumentCompositeNode documentNode, SampleProperty sampleProperty, string value)
    {
      SampleCompositeType sampleCompositeType = documentNode.Type as SampleCompositeType;
      if (sampleCompositeType == null)
        return (DocumentNode) null;
      if (sampleProperty.DeclaringSampleType != sampleCompositeType)
        return (DocumentNode) null;
      return this.CreatePropertyValueInternal(documentNode, sampleProperty, value);
    }

    private DocumentNode CreatePropertyValueInternal(DocumentCompositeNode documentNode, SampleProperty sampleProperty, string value)
    {
      DocumentNode documentNode1;
      if (sampleProperty.PropertySampleType.IsBasicType)
      {
        SampleBasicType sampleType = (SampleBasicType) sampleProperty.PropertySampleType;
        if (value == null)
          value = this.valueGenerator.GetRandomValue(sampleType, sampleProperty.Format, sampleProperty.FormatParameters);
        documentNode1 = (DocumentNode) this.CreateBasicNode(sampleType, value);
      }
      else
        documentNode1 = (DocumentNode) this.CreateCompositeNode((SampleNonBasicType) sampleProperty.PropertySampleType);
      documentNode.Properties[(IPropertyId) sampleProperty] = documentNode1;
      return documentNode1;
    }

    public DocumentNode CreateCollectionItem(DocumentCompositeNode collectionNode)
    {
      return this.CreateCollectionItemInternal(collectionNode, (string) null);
    }

    public DocumentNode CreateCollectionItem(DocumentCompositeNode collectionNode, string value)
    {
      return this.CreateCollectionItemInternal(collectionNode, value);
    }

    public DocumentNode CreateCollectionItemForProperty(DocumentCompositeNode documentNode, string collectionPropertyName)
    {
      return this.CreateCollectionItemForProperty(documentNode, collectionPropertyName, (string) null);
    }

    public DocumentNode CreateCollectionItemForProperty(DocumentCompositeNode documentNode, string collectionPropertyName, string value)
    {
      SampleCompositeType sampleCompositeType = documentNode.Type as SampleCompositeType;
      if (sampleCompositeType == null)
        return (DocumentNode) null;
      SampleProperty sampleProperty = sampleCompositeType.GetSampleProperty(collectionPropertyName);
      if (sampleProperty == null)
        return (DocumentNode) null;
      return this.CreateCollectionItemForPropertyInternal(documentNode, sampleProperty, value);
    }

    public DocumentNode CreateCollectionItemForProperty(DocumentCompositeNode documentNode, SampleProperty collectionProperty)
    {
      return this.CreateCollectionItemForProperty(documentNode, collectionProperty, (string) null);
    }

    public DocumentNode CreateCollectionItemForProperty(DocumentCompositeNode documentNode, SampleProperty collectionProperty, string value)
    {
      SampleCompositeType sampleCompositeType = documentNode.Type as SampleCompositeType;
      if (sampleCompositeType == null)
        return (DocumentNode) null;
      if (collectionProperty.DeclaringSampleType != sampleCompositeType)
        return (DocumentNode) null;
      return this.CreateCollectionItemForPropertyInternal(documentNode, collectionProperty, value);
    }

    private DocumentNode CreateCollectionItemForPropertyInternal(DocumentCompositeNode documentNode, SampleProperty collectionProperty, string value)
    {
      SampleCollectionType sampleCollectionType = collectionProperty.PropertySampleType as SampleCollectionType;
      if (sampleCollectionType == null)
        return (DocumentNode) null;
      DocumentCompositeNode collectionNode = documentNode.Properties[(IPropertyId) collectionProperty] as DocumentCompositeNode;
      if (collectionNode == null)
      {
        collectionNode = this.CreateCompositeNode((SampleNonBasicType) sampleCollectionType);
        documentNode.Properties[(IPropertyId) collectionProperty] = (DocumentNode) collectionNode;
      }
      return this.CreateCollectionItemInternal(collectionNode, value);
    }

    private DocumentNode CreateCollectionItemInternal(DocumentCompositeNode collectionNode, string value)
    {
      SampleCollectionType sampleCollectionType = collectionNode.Type as SampleCollectionType;
      if (sampleCollectionType == null)
        return (DocumentNode) null;
      DocumentNode documentNode;
      if (sampleCollectionType.ItemSampleType.IsBasicType)
      {
        SampleBasicType sampleType = (SampleBasicType) sampleCollectionType.ItemSampleType;
        if (value == null)
          value = this.valueGenerator.GetRandomValue(sampleType, (string) null, (string) null);
        documentNode = (DocumentNode) this.CreateBasicNode(sampleType, value);
      }
      else
        documentNode = (DocumentNode) this.CreateCompositeNode((SampleNonBasicType) sampleCollectionType.ItemSampleType);
      collectionNode.Children.Add(documentNode);
      return documentNode;
    }

    public DocumentNode CreateNode(SampleType sampleType)
    {
      return !sampleType.IsBasicType ? (DocumentNode) this.CreateCompositeNode((SampleNonBasicType) sampleType) : (DocumentNode) this.CreateBasicNode((SampleBasicType) sampleType, (string) null);
    }

    public DocumentPrimitiveNode CreateBasicNode(SampleBasicType sampleType, string value)
    {
      string str = value;
      if (sampleType == SampleBasicType.Image)
        str = this.DataSet.AddOrUpdateAssetFile(value);
      DocumentNodeStringValue documentNodeStringValue = new DocumentNodeStringValue(str);
      return this.XamlDocumentContext.CreateNode(sampleType.TypeId, (IDocumentNodeValue) documentNodeStringValue);
    }

    public virtual DocumentCompositeNode CreateCompositeNode(SampleNonBasicType sampleType)
    {
      return this.XamlDocumentContext.CreateNode((ITypeId) sampleType);
    }

    private class BuildingItem
    {
      public SampleType Type { get; private set; }

      public string Identity { get; private set; }

      public BuildingItem(SampleType type, string identity)
      {
        this.Type = type;
        this.Identity = identity;
      }

      public static bool operator ==(SampleDataValueBuilderBase.BuildingItem left, SampleDataValueBuilderBase.BuildingItem right)
      {
        return left.Type == right.Type && !(left.Identity != right.Identity);
      }

      public static bool operator !=(SampleDataValueBuilderBase.BuildingItem left, SampleDataValueBuilderBase.BuildingItem right)
      {
        return !(left == right);
      }

      public override string ToString()
      {
        if (string.IsNullOrEmpty(this.Identity))
          return "Coll<" + (object) this.Type + ">";
        return this.Identity + (object) ": " + (string) (object) this.Type;
      }

      public override int GetHashCode()
      {
        return this.Type.GetHashCode() ^ this.Identity.GetHashCode();
      }

      public override bool Equals(object obj)
      {
        SampleDataValueBuilderBase.BuildingItem buildingItem = obj as SampleDataValueBuilderBase.BuildingItem;
        if (buildingItem == (SampleDataValueBuilderBase.BuildingItem) null)
          return false;
        return this == buildingItem;
      }
    }

    private class BuildingStack
    {
      private static int recursionLimit = 2;
      private List<SampleDataValueBuilderBase.BuildingItem> stack = new List<SampleDataValueBuilderBase.BuildingItem>();

      public bool PushProperty(SampleProperty sampleProperty)
      {
        return this.Push(sampleProperty.PropertySampleType, sampleProperty.Name);
      }

      public bool PushCollectionItemType(SampleType itemType)
      {
        return this.Push(itemType, string.Empty);
      }

      public void Pop()
      {
        this.stack.RemoveAt(this.stack.Count - 1);
      }

      private bool Push(SampleType sampleType, string identity)
      {
        SampleDataValueBuilderBase.BuildingItem item = new SampleDataValueBuilderBase.BuildingItem(sampleType, identity);
        if (!EnumerableExtensions.CountIsLessThan<SampleDataValueBuilderBase.BuildingItem>(Enumerable.Where<SampleDataValueBuilderBase.BuildingItem>((IEnumerable<SampleDataValueBuilderBase.BuildingItem>) this.stack, (Func<SampleDataValueBuilderBase.BuildingItem, bool>) (i => i == item)), SampleDataValueBuilderBase.BuildingStack.recursionLimit))
          return false;
        this.stack.Add(item);
        return true;
      }
    }
  }
}
