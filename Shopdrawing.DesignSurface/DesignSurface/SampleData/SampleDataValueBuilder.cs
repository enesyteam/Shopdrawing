// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SampleData.SampleDataValueBuilder
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using System;

namespace Microsoft.Expression.DesignSurface.SampleData
{
  public sealed class SampleDataValueBuilder : SampleDataValueBuilderBase, IDisposable
  {
    private DocumentCompositeNode rootNode;

    public DocumentCompositeNode RootNode
    {
      get
      {
        if (this.rootNode == null)
          this.rootNode = this.DataSet.CreateEmptyRootNode();
        return this.rootNode;
      }
    }

    public SampleDataValueBuilder(SampleDataSet dataSet, IDocumentContext xamlDocumentContext)
      : base(dataSet, xamlDocumentContext)
    {
    }

    public void Dispose()
    {
      if (this.rootNode == null)
        return;
      this.DataSet.RootNode = this.rootNode;
      GC.SuppressFinalize((object) this);
    }

    public void Finalize(bool success)
    {
      if (!success)
        this.rootNode = (DocumentCompositeNode) null;
      this.Dispose();
    }

    public override DocumentCompositeNode CreateCompositeNode(SampleNonBasicType sampleType)
    {
      return sampleType != this.DataSet.RootType ? base.CreateCompositeNode(sampleType) : this.RootNode;
    }
  }
}
