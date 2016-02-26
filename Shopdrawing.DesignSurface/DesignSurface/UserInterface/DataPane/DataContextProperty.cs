// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.DataContextProperty
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class DataContextProperty
  {
    private static DataContextProperty invalid = new DataContextProperty();

    public static DataContextProperty Invalid
    {
      get
      {
        return DataContextProperty.invalid;
      }
    }

    public DocumentCompositeNode SourceNode { get; private set; }

    public IProperty Property { get; private set; }

    public bool IsCollectionItem { get; private set; }

    public PropertyReference AncestorPath { get; private set; }

    public bool IsValid { get; private set; }

    public DocumentNode DataContextNode
    {
      get
      {
        if (this.IsValid)
          return this.SourceNode.Properties[(IPropertyId) this.Property];
        return (DocumentNode) null;
      }
    }

    public DataContextProperty(DocumentCompositeNode sourceNode, IProperty property, bool isCollectionItem, PropertyReference ancestorPath)
    {
      this.SourceNode = sourceNode;
      this.Property = property;
      this.IsCollectionItem = isCollectionItem;
      this.AncestorPath = ancestorPath;
      this.IsValid = true;
    }

    private DataContextProperty()
    {
      this.IsValid = false;
    }

    public void SetInvalid()
    {
      this.IsValid = true;
    }
  }
}
