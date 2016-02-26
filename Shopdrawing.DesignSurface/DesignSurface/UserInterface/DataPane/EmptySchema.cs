// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.EmptySchema
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System.ComponentModel;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class EmptySchema : ISchema, INotifyPropertyChanged
  {
    private DataSourceNode dataSource;

    public DataSourceNode DataSource
    {
      get
      {
        return this.dataSource;
      }
      set
      {
        this.dataSource = value;
      }
    }

    public DataSchemaNode Root
    {
      get
      {
        return (DataSchemaNode) null;
      }
    }

    public DataSchemaNode AbsoluteRoot
    {
      get
      {
        return (DataSchemaNode) null;
      }
    }

    public string PathDescription
    {
      get
      {
        return StringTable.UseCustomPropertyPathDescription;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged
    {
      add
      {
      }
      remove
      {
      }
    }

    public string GetPath(DataSchemaNodePath nodePath)
    {
      return string.Empty;
    }

    public int GetCollectionDepth(DataSchemaNode endNode)
    {
      return -1;
    }

    public DataSchemaNode GetEffectiveCollectionNode(DataSchemaNode endNode)
    {
      return (DataSchemaNode) null;
    }

    public DataSchemaNodePath CreateNodePath()
    {
      return (DataSchemaNodePath) null;
    }

    public ISchema MakeRelativeToNode(DataSchemaNode node)
    {
      return (ISchema) new EmptySchema();
    }

    public DataSchemaNodePath GetNodePathFromPath(string path)
    {
      return (DataSchemaNodePath) null;
    }
  }
}
