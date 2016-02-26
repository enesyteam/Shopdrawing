// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.DataContextInfo
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class DataContextInfo
  {
    private DataSourceInfo dataSource;
    private RawDataSourceInfoBase rawDataSource;

    public DocumentCompositeNode Owner { get; set; }

    public IProperty Property { get; set; }

    public DataSourceMatchCriteria DataSourceMatch { get; set; }

    public DocumentNode DataContext
    {
      get
      {
        DocumentNode documentNode = (DocumentNode) null;
        if (this.Owner != null && this.Property != null)
          documentNode = this.Owner.Properties[(IPropertyId) this.Property];
        return documentNode;
      }
    }

    public RawDataSourceInfoBase RawDataSource
    {
      get
      {
        return this.rawDataSource;
      }
      set
      {
        this.dataSource = (DataSourceInfo) null;
        this.rawDataSource = value;
      }
    }

    public DataSourceInfo DataSource
    {
      get
      {
        if (this.dataSource == null)
          this.dataSource = this.RawDataSource == null ? new DataSourceInfo((DocumentNode) null, (string) null, DataSourceCategory.Clr) : this.RawDataSource.DataSourceInfo;
        return this.dataSource;
      }
    }

    public bool IsValid
    {
      get
      {
        if (this.RawDataSource != null)
          return this.RawDataSource.IsValid;
        return true;
      }
    }

    public SceneNode GetOwnerSceneNode(SceneViewModel viewModel)
    {
      if (this.Owner == null)
        return (SceneNode) null;
      return viewModel.IsExternal((DocumentNode) this.Owner) ? viewModel.GetViewModel(this.Owner.DocumentRoot, false).GetSceneNode((DocumentNode) this.Owner) : viewModel.GetSceneNode((DocumentNode) this.Owner);
    }

    public void SetDataContextOwner(DocumentCompositeNode dataContextOwnerNode, IProperty dataContextProperty)
    {
      if (this.Owner != null)
        return;
      this.Owner = dataContextOwnerNode;
      this.Property = dataContextProperty;
    }

    public override string ToString()
    {
      string str = string.Empty;
      if (this.Owner != null)
      {
        str = this.Owner.ToString();
        if (this.Property != null)
          str = str + "[" + this.Property.Name + "]";
      }
      if (this.RawDataSource != null)
        str = str + (object) "; Source: " + (string) (object) this.RawDataSource;
      if (string.IsNullOrEmpty(str))
        str = "Empty";
      return str;
    }
  }
}
