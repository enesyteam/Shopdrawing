// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SampleData.DesignTimeSampleCompositeType
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.ComponentModel;

namespace Microsoft.Expression.DesignSurface.SampleData
{
  public class DesignTimeSampleCompositeType : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    public void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    public override string ToString()
    {
      Type type = this.GetType();
      SampleDataSet sampleDataSet = SampleDataSet.SampleDataSetFromType(type);
      return sampleDataSet == null ? type.Name : sampleDataSet.ClrNamespace + "." + type.Name;
    }
  }
}
