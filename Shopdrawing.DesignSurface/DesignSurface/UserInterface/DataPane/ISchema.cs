// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.ISchema
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System.ComponentModel;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public interface ISchema : INotifyPropertyChanged
  {
    DataSourceNode DataSource { get; }

    DataSchemaNode Root { get; }

    DataSchemaNode AbsoluteRoot { get; }

    string PathDescription { get; }

    string GetPath(DataSchemaNodePath nodePath);

    int GetCollectionDepth(DataSchemaNode endNode);

    DataSchemaNode GetEffectiveCollectionNode(DataSchemaNode endNode);

    DataSchemaNodePath GetNodePathFromPath(string path);

    DataSchemaNodePath CreateNodePath();

    ISchema MakeRelativeToNode(DataSchemaNode node);
  }
}
