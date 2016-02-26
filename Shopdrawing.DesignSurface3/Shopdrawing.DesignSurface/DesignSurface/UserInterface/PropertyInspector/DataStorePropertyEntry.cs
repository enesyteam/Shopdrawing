// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.DataStorePropertyEntry
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.SampleData;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class DataStorePropertyEntry : Freezable
  {
    private string propertyName;

    public SampleDataSet DataSet { get; private set; }

    public bool Renamed { get; private set; }

    public bool IsCreateNewPropertyEntry { get; private set; }

    public string Name
    {
      get
      {
        return this.propertyName;
      }
      set
      {
        this.propertyName = value;
        this.Renamed = true;
      }
    }

    public string DataSetName
    {
      get
      {
        if (this.IsCreateNewPropertyEntry)
          return "";
        return this.DataSet.Name;
      }
    }

    public DataStorePropertyEntry(SampleDataSet dataSet, string propertyName, bool isCreateNewPropertyEntry)
    {
      this.DataSet = dataSet;
      this.propertyName = propertyName;
      this.IsCreateNewPropertyEntry = isCreateNewPropertyEntry;
    }

    protected override Freezable CreateInstanceCore()
    {
      return (Freezable) new DataStorePropertyEntry((SampleDataSet) null, (string) null, false);
    }

    public override string ToString()
    {
      return this.Name;
    }
  }
}
