// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.DataViewBuilderContext
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignSurface.UserInterface.DataPane;

namespace Microsoft.Expression.DesignSurface.Tools
{
  public class DataViewBuilderContext
  {
    public DocumentCompositeNode RootNode { get; private set; }

    public DataViewCategory Category { get; private set; }

    public DataSchemaNodePath CurrentSchemaPath { get; set; }

    public DocumentCompositeNode CurrentLabelNode { get; set; }

    public DocumentCompositeNode CurrentFieldNode { get; set; }

    public IDocumentContext DocumentContext
    {
      get
      {
        return this.RootNode.Context;
      }
    }

    public DataViewBuilderContext(DocumentCompositeNode rootNode, DataViewCategory category)
    {
      this.RootNode = rootNode;
      this.Category = category;
    }
  }
}
