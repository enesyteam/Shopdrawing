// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.SampleDataEditorModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.SampleData;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.UserInterface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Media.Imaging;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public sealed class SampleDataEditorModel : NotifyingObject
  {
    private Dictionary<string, BitmapImage> imageCache = new Dictionary<string, BitmapImage>();
    private SampleCompositeType sampleCompositeType;
    private DataSchemaNodePath editingSchemaPath;
    private DocumentCompositeNode sourceCollectionNode;
    private DocumentCompositeNode editingCollectionNode;
    private DocumentCompositeNode editingRootNode;
    private int processedChanges;
    private bool isModified;
    private IMessageDisplayService messageService;
    private bool rowCountChanged;

    public SampleDataValueBuilderBase ValueBuilder { get; private set; }

    public IList<SampleDataProperty> SampleDataProperties { get; private set; }

    public ObservableCollection<SampleDataRow> SampleDataRows { get; private set; }

    public SampleDataSet SampleDataSet
    {
      get
      {
        return this.sampleCompositeType.DeclaringDataSet;
      }
    }

    public bool IsModified
    {
      get
      {
        return this.isModified;
      }
    }

    public DocumentCompositeNode CollectionNode
    {
      get
      {
        return this.editingCollectionNode ?? this.sourceCollectionNode;
      }
    }

    public int RowCount
    {
      get
      {
        return this.SampleDataRows.Count;
      }
      set
      {
        if (this.RowCount == value)
          return;
        this.SetModified();
        this.rowCountChanged = true;
        for (int count = this.editingCollectionNode.Children.Count; count < value; ++count)
          this.editingCollectionNode.Children.Add(this.ValueBuilder.GenerateValue((SampleType) this.sampleCompositeType));
        for (int rowCount = this.RowCount; rowCount < value; ++rowCount)
          this.SampleDataRows.Add(new SampleDataRow(this, rowCount));
        for (int rowCount = this.RowCount; rowCount > value; --rowCount)
          this.SampleDataRows.RemoveAt(rowCount - 1);
      }
    }

    public SampleDataEditorModel(DataSchemaNodePath schemaPath, IMessageDisplayService messageService)
    {
      this.messageService = messageService;
      SampleType sampleType = schemaPath.Node.SampleType;
      DataSchemaNode collectionItemNode = schemaPath.EffectiveCollectionItemNode;
      this.editingSchemaPath = new DataSchemaNodePath(schemaPath.Schema, collectionItemNode.Parent);
      this.sampleCompositeType = (SampleCompositeType) collectionItemNode.SampleType;
      this.ValueBuilder = new SampleDataValueBuilderBase(this.SampleDataSet, this.SampleDataSet.RootNode.Context);
      this.SampleDataProperties = (IList<SampleDataProperty>) new List<SampleDataProperty>();
      foreach (SampleProperty sampleProperty in (IEnumerable<SampleProperty>) this.sampleCompositeType.SampleProperties)
      {
        if (sampleProperty.IsBasicType)
          this.SampleDataProperties.Add(new SampleDataProperty(sampleProperty, this));
      }
      ((List<SampleDataProperty>) this.SampleDataProperties).Sort((Comparison<SampleDataProperty>) ((a, b) => StringLogicalComparer.Instance.Compare(a.SampleProperty.Name, b.SampleProperty.Name)));
      this.sourceCollectionNode = this.GetCollectionNode(this.SampleDataSet.RootNode, false);
      this.ValueBuilder.InitCollectionDepth(this.editingSchemaPath);
      List<SampleDataRow> rows = new List<SampleDataRow>();
      if (this.sourceCollectionNode != null)
      {
        int count = this.sourceCollectionNode.Children.Count;
        for (int rowNumber = 0; rowNumber < count; ++rowNumber)
          rows.Add(new SampleDataRow(this, rowNumber));
      }
      this.SampleDataRows = (ObservableCollection<SampleDataRow>) new SampleDataEditorModel.SampleDataRowCollection(rows);
    }

    public void SetModified()
    {
      if (this.isModified)
        return;
      this.isModified = true;
      this.editingRootNode = this.SampleDataSet.RootNode;
      this.editingRootNode = (DocumentCompositeNode) this.editingRootNode.Clone(this.editingRootNode.Context);
      this.editingCollectionNode = this.GetCollectionNode(this.editingRootNode, true);
    }

    public BitmapImage GetImage(string absolutePath)
    {
      BitmapImage bitmapImage = (BitmapImage) null;
      if (!string.IsNullOrEmpty(absolutePath) && !this.imageCache.TryGetValue(absolutePath, out bitmapImage))
      {
        bitmapImage = new BitmapImage();
        try
        {
          bitmapImage.BeginInit();
          bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
          bitmapImage.UriSource = new Uri(absolutePath, UriKind.Absolute);
          bitmapImage.EndInit();
        }
        catch
        {
          bitmapImage = (BitmapImage) null;
        }
        this.imageCache[absolutePath] = bitmapImage;
      }
      return bitmapImage;
    }

    public void UpdateEditingCollection()
    {
      SampleDataValueChangeProcessor.ApplyChanges(this.SampleDataSet.GetNormalizedChanges(this.processedChanges), this.editingRootNode, this.SampleDataSet.ProjectDocument, true);
      this.processedChanges = this.SampleDataSet.Changes.Count;
      ((SampleDataEditorModel.SampleDataRowCollection) this.SampleDataRows).NotifyReset();
    }

    public void CommitChanges()
    {
      if (!this.isModified)
        return;
      this.isModified = false;
      this.processedChanges = 0;
      for (int index = this.editingCollectionNode.Children.Count - 1; index >= this.RowCount; --index)
        this.editingCollectionNode.Children.RemoveAt(index);
      if (this.rowCountChanged)
        this.EqualizeSubCollectionItemCount(2);
      using (TemporaryCursor.SetWaitCursor())
        this.SampleDataSet.CommitChanges(this.editingRootNode, this.messageService);
    }

    public void CancelChanges()
    {
      if (!this.isModified)
        return;
      this.SampleDataSet.RollbackChanges();
    }

    private void EqualizeSubCollectionItemCount(int maxDepth)
    {
      DocumentCompositeNode ancestorCollection = (DocumentCompositeNode) null;
      Stack<List<IProperty>> ancestorProperties = new Stack<List<IProperty>>();
      List<IProperty> list = new List<IProperty>();
      int num = maxDepth;
      for (DocumentCompositeNode documentCompositeNode = this.editingCollectionNode; num > 0 && documentCompositeNode.Parent != null; documentCompositeNode = documentCompositeNode.Parent)
      {
        if (documentCompositeNode.IsProperty)
        {
          list.Add(documentCompositeNode.SitePropertyKey);
        }
        else
        {
          ancestorCollection = documentCompositeNode.Parent;
          list.Reverse();
          ancestorProperties.Push(list);
          --num;
          if (num != 0)
            list = new List<IProperty>();
          else
            break;
        }
      }
      if (ancestorCollection == null)
        return;
      this.EqualizeSubCollectionItemCount(ancestorCollection, ancestorProperties);
    }

    private void EqualizeSubCollectionItemCount(DocumentCompositeNode ancestorCollection, Stack<List<IProperty>> ancestorProperties)
    {
      if (ancestorCollection == null || !ancestorCollection.SupportsChildren || ancestorProperties.Count == 0)
        return;
      List<IProperty> list = ancestorProperties.Pop();
      for (int index1 = 0; index1 < ancestorCollection.Children.Count; ++index1)
      {
        DocumentCompositeNode documentCompositeNode = (DocumentCompositeNode) ancestorCollection.Children[index1];
        for (int index2 = list.Count - 1; documentCompositeNode != null && index2 >= 0; --index2)
          documentCompositeNode = documentCompositeNode.Properties[(IPropertyId) list[index2]] as DocumentCompositeNode;
        if (documentCompositeNode != null)
        {
          if (ancestorProperties.Count == 0)
            this.EnsureItemCount(documentCompositeNode, this.RowCount);
          else
            this.EqualizeSubCollectionItemCount(documentCompositeNode, ancestorProperties);
        }
      }
      ancestorProperties.Push(list);
    }

    private void EnsureItemCount(DocumentCompositeNode collectionNode, int itemCount)
    {
      if (collectionNode == null || !collectionNode.SupportsChildren || collectionNode.Children.Count == itemCount)
        return;
      for (int index = collectionNode.Children.Count - 1; index >= itemCount; --index)
        collectionNode.Children.RemoveAt(index);
      for (int count = collectionNode.Children.Count; count < itemCount; ++count)
      {
        DocumentNode documentNode = this.ValueBuilder.GenerateValue((SampleType) this.sampleCompositeType);
        collectionNode.Children.Add(documentNode);
      }
    }

    private DocumentCompositeNode GetCollectionNode(DocumentCompositeNode rootSampleDataNode, bool createMissingNodes)
    {
      DataSchemaNode node = this.editingSchemaPath.Node;
      List<DataSchemaNode> list = new List<DataSchemaNode>();
      for (DataSchemaNode dataSchemaNode = node; dataSchemaNode.Parent != null; dataSchemaNode = dataSchemaNode.Parent)
        list.Insert(0, dataSchemaNode);
      DocumentCompositeNode documentNode1 = rootSampleDataNode;
      for (int index = 0; index < list.Count; ++index)
      {
        DataSchemaNode dataSchemaNode = list[index];
        SampleNonBasicType sampleNonBasicType = (SampleNonBasicType) documentNode1.Type;
        DocumentNode documentNode2;
        if (sampleNonBasicType.IsCollection)
        {
          if (documentNode1.Children.Count > 0)
          {
            documentNode2 = documentNode1.Children[0];
          }
          else
          {
            if (!createMissingNodes)
              return (DocumentCompositeNode) null;
            documentNode2 = this.ValueBuilder.CreateNode(((SampleCollectionType) sampleNonBasicType).ItemSampleType);
            documentNode1.Children.Add(documentNode2);
          }
        }
        else
        {
          SampleProperty sampleProperty = ((SampleCompositeType) sampleNonBasicType).GetSampleProperty(dataSchemaNode.PathName);
          documentNode2 = documentNode1.Properties[(IPropertyId) sampleProperty];
          if (documentNode2 == null)
          {
            if (!createMissingNodes)
              return (DocumentCompositeNode) null;
            documentNode2 = this.ValueBuilder.CreatePropertyValue(documentNode1, sampleProperty);
            documentNode1.Properties[(IPropertyId) sampleProperty] = documentNode2;
          }
        }
        documentNode1 = (DocumentCompositeNode) documentNode2;
      }
      return documentNode1;
    }

    public override string ToString()
    {
      return "Edit Model: " + (object) this.SampleDataSet;
    }

    private class SampleDataRowCollection : ObservableCollection<SampleDataRow>
    {
      public SampleDataRowCollection(List<SampleDataRow> rows)
        : base(rows)
      {
      }

      public void NotifyReset()
      {
        this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
      }
    }
  }
}
