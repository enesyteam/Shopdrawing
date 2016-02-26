// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.DataSchemaNodePathCollection
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.UserInterface.DataPane;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.Tools
{
  [Serializable]
  internal class DataSchemaNodePathCollection : List<DataSchemaNodePath>
  {
    public DataSchemaNodePath PrimarySchemaNodePath
    {
      get
      {
        return this[0];
      }
    }

    public DataSchemaNodePath PrimaryAbsolutePath
    {
      get
      {
        return this[0].AbsolutePath;
      }
    }

    public ISchema PrimarySchema
    {
      get
      {
        return this.PrimarySchemaNodePath.Schema;
      }
    }

    public ISchema PrimaryAbsoluteSchema
    {
      get
      {
        return this.PrimaryAbsolutePath.Schema;
      }
    }

    public string PrimaryDataSourceName
    {
      get
      {
        return this.PrimarySchemaNodePath.Schema.DataSource.Name;
      }
    }

    public string PrimaryBindingPath
    {
      get
      {
        return this.PrimarySchemaNodePath.Path;
      }
    }

    public bool IsSingleSchema
    {
      get
      {
        ISchema schema = this[0].Schema;
        for (int index = 1; index < this.Count; ++index)
        {
          if (schema.AbsoluteRoot != this[index].Schema.AbsoluteRoot)
            return false;
        }
        return true;
      }
    }

    public bool IsCommonEffectiveCollectionNode
    {
      get
      {
        DataSchemaNode firstCollectionNode = this[0].EffectiveCollectionNode;
        return this.Find((Predicate<DataSchemaNodePath>) (path => path.EffectiveCollectionNode != firstCollectionNode)) == null;
      }
    }

    public bool IsCommonNodeType
    {
      get
      {
        SchemaNodeTypes firstNodeType = this[0].NodeType;
        return this.Find((Predicate<DataSchemaNodePath>) (path => path.NodeType != firstNodeType)) == null;
      }
    }

    public DataSchemaNodePathCollection(DataSchemaNodePath schemaNodePath)
    {
      this.Add(schemaNodePath);
    }

    public DataSchemaNodePathCollection(IEnumerable<DataSchemaNodePath> items)
    {
      this.AddRange(items);
    }
  }
}
