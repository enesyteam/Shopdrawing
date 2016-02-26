// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.DataSourceHost
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.DesignSurface.ViewModel.SceneNodeSearch;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class DataSourceHost : INotifyPropertyChanged
  {
    private DataSourceHost parent;
    private ObservableCollection<DataSourceHost> children;
    private ObservableCollection<DataSourceNode> dataSources;
    private DocumentNode documentNode;
    private SceneNode sceneNode;
    private SceneNodeSubscription<object, object> subscription;

    public DocumentNode DocumentNode
    {
      get
      {
        return this.documentNode;
      }
      internal set
      {
        this.documentNode = value;
      }
    }

    public DataSourceHost Parent
    {
      get
      {
        return this.parent;
      }
    }

    public ObservableCollection<DataSourceHost> Children
    {
      get
      {
        return this.children;
      }
    }

    public bool HasChildren
    {
      get
      {
        return this.children.Count != 0;
      }
    }

    public ObservableCollection<DataSourceNode> DataSources
    {
      get
      {
        return this.dataSources;
      }
    }

    public bool HasDataSources
    {
      get
      {
        return this.dataSources.Count != 0;
      }
    }

    public string Name
    {
      get
      {
        string name = this.DocumentNode.Name;
        if (!string.IsNullOrEmpty(name))
          return name;
        return "[" + this.DocumentNode.TargetType.Name + "]";
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public DataSourceHost(SceneNode sceneNode)
      : this()
    {
      this.sceneNode = sceneNode;
      this.documentNode = sceneNode.DocumentNode;
      this.InitializeSubscription();
    }

    public DataSourceHost()
    {
      this.children = (ObservableCollection<DataSourceHost>) new DataSourceHost.ChildrenDataCollection(this);
      this.dataSources = (ObservableCollection<DataSourceNode>) new DataSourceHost.DataSourcesDataCollection(this);
    }

    private void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    public DataSourceNode FindDataSourceByName(string name)
    {
      foreach (DataSourceNode dataSourceNode in (Collection<DataSourceNode>) this.dataSources)
      {
        if (dataSourceNode.Name == name)
          return dataSourceNode;
      }
      return (DataSourceNode) null;
    }

    public void Unload()
    {
      if (this.subscription == null)
        return;
      this.subscription.PathNodeInserted -= new SceneNodeSubscription<object, object>.PathNodeInsertedListener(this.Subscription_PathNodeInserted);
      this.subscription.PathNodeRemoved -= new SceneNodeSubscription<object, object>.PathNodeRemovedListener(this.Subscription_PathNodeRemoved);
      this.sceneNode.ViewModel.LateSceneUpdatePhase -= new SceneUpdatePhaseEventHandler(this.ViewModel_LateSceneUpdatePhase);
      this.subscription = (SceneNodeSubscription<object, object>) null;
    }

    private void InitializeSubscription()
    {
      this.subscription = new SceneNodeSubscription<object, object>();
      this.subscription.Path = new SearchPath(new SearchStep[1]
      {
        new SearchStep(new SearchAxis(this.sceneNode.NameProperty))
      });
      this.subscription.InsertBasisNode(this.sceneNode);
      this.subscription.PathNodeInserted += new SceneNodeSubscription<object, object>.PathNodeInsertedListener(this.Subscription_PathNodeInserted);
      this.subscription.PathNodeRemoved += new SceneNodeSubscription<object, object>.PathNodeRemovedListener(this.Subscription_PathNodeRemoved);
      this.sceneNode.ViewModel.LateSceneUpdatePhase += new SceneUpdatePhaseEventHandler(this.ViewModel_LateSceneUpdatePhase);
    }

    private void Subscription_PathNodeRemoved(object sender, SceneNode basisNode, object basisContent, SceneNode oldPathNode, object oldContent)
    {
      this.OnPropertyChanged("Name");
    }

    private void Subscription_PathNodeInserted(object sender, SceneNode basisNode, object basisContent, SceneNode newPathNode, object newContent)
    {
      this.OnPropertyChanged("Name");
    }

    private void ViewModel_LateSceneUpdatePhase(object sender, SceneUpdatePhaseEventArgs args)
    {
      this.subscription.Update(this.sceneNode.ViewModel, args.DocumentChanges, args.DocumentChangeStamp);
    }

    private abstract class UpdatingDataCollection<T> : ObservableCollection<T>
    {
      protected abstract void OnAddItem(T item);

      protected abstract void OnRemoveItem(T item);

      protected override void ClearItems()
      {
        ArrayList arrayList = new ArrayList((ICollection) this);
        base.ClearItems();
        foreach (T obj in arrayList)
          this.OnRemoveItem(obj);
      }

      protected override void InsertItem(int index, T item)
      {
        base.InsertItem(index, item);
        this.OnAddItem(item);
      }

      protected override void RemoveItem(int index)
      {
        T obj = this[index];
        base.RemoveItem(index);
        this.OnRemoveItem(obj);
      }

      protected override void SetItem(int index, T item)
      {
        T obj = this[index];
        base.SetItem(index, item);
        if (object.ReferenceEquals((object) obj, (object) item))
          return;
        this.OnRemoveItem(obj);
        this.OnAddItem(item);
      }
    }

    private class ChildrenDataCollection : DataSourceHost.UpdatingDataCollection<DataSourceHost>
    {
      private DataSourceHost host;

      public ChildrenDataCollection(DataSourceHost host)
      {
        this.host = host;
      }

      protected override void OnAddItem(DataSourceHost child)
      {
        child.parent = this.host;
        this.host.OnPropertyChanged("Children");
        if (this.Count != 1)
          return;
        this.host.OnPropertyChanged("HasChildren");
      }

      protected override void OnRemoveItem(DataSourceHost child)
      {
        child.parent = (DataSourceHost) null;
        this.host.OnPropertyChanged("Children");
        if (this.Count != 0)
          return;
        this.host.OnPropertyChanged("HasChildren");
      }
    }

    private class DataSourcesDataCollection : DataSourceHost.UpdatingDataCollection<DataSourceNode>
    {
      private DataSourceHost host;

      public DataSourcesDataCollection(DataSourceHost host)
      {
        this.host = host;
      }

      protected override void OnAddItem(DataSourceNode dataSource)
      {
        dataSource.Host = this.host;
        this.host.OnPropertyChanged("DataSources");
        if (this.Count != 1)
          return;
        this.host.OnPropertyChanged("HasDataSources");
      }

      protected override void OnRemoveItem(DataSourceNode dataSource)
      {
        dataSource.Host = (DataSourceHost) null;
        this.host.OnPropertyChanged("DataSources");
        if (this.Count != 0)
          return;
        this.host.OnPropertyChanged("HasDataSources");
      }
    }
  }
}
