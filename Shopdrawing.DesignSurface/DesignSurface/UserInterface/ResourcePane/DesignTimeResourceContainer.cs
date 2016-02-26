// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.ResourcePane.DesignTimeResourceContainer
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System.ComponentModel;

namespace Microsoft.Expression.DesignSurface.UserInterface.ResourcePane
{
  internal class DesignTimeResourceContainer : INotifyPropertyChanged
  {
    private string projectName;
    private string containerName;
    private DocumentResourceContainer container;

    public string ProjectName
    {
      get
      {
        return this.projectName;
      }
      set
      {
        this.projectName = value;
        this.OnPropertyChanged("ProjectName");
      }
    }

    public string ContainerName
    {
      get
      {
        return this.containerName;
      }
      set
      {
        this.containerName = value;
        this.OnPropertyChanged("ContainerName");
      }
    }

    public string FullPath
    {
      get
      {
        return this.container.DocumentReference.Path;
      }
    }

    public DocumentResourceContainer Container
    {
      get
      {
        return this.container;
      }
      set
      {
        this.container = value;
        this.OnPropertyChanged("Container");
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public DesignTimeResourceContainer(DocumentResourceContainer container)
    {
      this.Container = container;
      this.ContainerName = container.Name;
      this.ProjectName = container.ProjectItem.Project.Name;
    }

    public override string ToString()
    {
      return this.ContainerName;
    }

    protected void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
