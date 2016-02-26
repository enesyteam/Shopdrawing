// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.DataSourcesCollection`1
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System.Collections.ObjectModel;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  internal sealed class DataSourcesCollection<T> : ObservableCollection<T> where T : DataSourceItem
  {
    private DataHostItem collectionHost;

    public DataSourcesCollection(DataHostItem collectionHost)
    {
      this.collectionHost = collectionHost;
    }

    public void InsertDataSource(int index, T dataSource)
    {
      this.InsertItem(index, dataSource);
      this.collectionHost.OnDataSourceAdded((DataSourceItem) dataSource);
    }

    public void AddDataSource(T dataSource)
    {
      this.Add(dataSource);
      this.collectionHost.OnDataSourceAdded((DataSourceItem) dataSource);
    }

    public void RemoveDataSource(T dataSource)
    {
      this.Remove(dataSource);
      this.collectionHost.OnDataSourceRemoved((DataSourceItem) dataSource, true);
    }

    public void ClearDataSources()
    {
      foreach (T obj in (Collection<T>) this)
        this.collectionHost.OnDataSourceRemoved((DataSourceItem) obj, false);
      this.Clear();
      this.collectionHost.OnDataSourcesCleared();
    }
  }
}
