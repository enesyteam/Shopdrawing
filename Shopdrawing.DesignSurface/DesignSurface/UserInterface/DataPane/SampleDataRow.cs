// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.SampleDataRow
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.Framework;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public sealed class SampleDataRow : NotifyingObject
  {
    public SampleDataEditorModel Model { get; private set; }

    public int RowNumber { get; private set; }

    public DocumentCompositeNode RowNode
    {
      get
      {
        return (DocumentCompositeNode) this.Model.CollectionNode.Children[this.RowNumber];
      }
    }

    public List<SampleDataCellValue> Cells { get; private set; }

    public SampleDataRow(SampleDataEditorModel model, int rowNumber)
    {
      this.Model = model;
      this.RowNumber = rowNumber;
      this.Cells = new List<SampleDataCellValue>(this.Model.SampleDataProperties.Count);
      foreach (SampleDataProperty property in (IEnumerable<SampleDataProperty>) this.Model.SampleDataProperties)
        this.Cells.Add(new SampleDataCellValue(this, property));
    }

    public SampleDataCellValue GetCell(SampleDataProperty property)
    {
      return this.Cells.Find((Predicate<SampleDataCellValue>) (cell => cell.Property == property));
    }

    public override string ToString()
    {
      return this.RowNode.ToString() + (object) " - " + (string) (object) this.RowNumber;
    }
  }
}
