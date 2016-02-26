// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.GridDataViewLayoutBuilder
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.Tools
{
  public class GridDataViewLayoutBuilder : IDataViewLayoutBuilder
  {
    private int row = -1;
    private DocumentCompositeNode rowDefinitionsNode;
    private DocumentCompositeNode contentNode;
    private DocumentCompositeNode firstLabel;
    private DocumentCompositeNode firstField;

    public void SetElementLayout(DataViewBuilderContext context)
    {
      if (this.row == -1)
        this.contentNode = (DocumentCompositeNode) context.RootNode.Properties[PanelElement.ChildrenProperty];
      ++this.row;
      if (this.row == 0)
      {
        if (context.CurrentLabelNode != null)
        {
          this.firstLabel = context.CurrentLabelNode;
          this.contentNode.Children.Add((DocumentNode) this.firstLabel);
        }
        this.firstField = context.CurrentFieldNode;
        this.contentNode.Children.Add((DocumentNode) this.firstField);
      }
      else
      {
        if (this.row == 1)
        {
          this.rowDefinitionsNode = context.DocumentContext.CreateNode(PlatformTypes.RowDefinitionCollection);
          context.RootNode.Properties[GridElement.RowDefinitionsProperty] = (DocumentNode) this.rowDefinitionsNode;
          this.AddRowDefinition();
          if (this.firstLabel != null)
            this.firstLabel.Properties[GridElement.RowProperty] = context.RootNode.Context.CreateNode(typeof (int), (object) 0);
          this.firstField.Properties[GridElement.RowProperty] = context.RootNode.Context.CreateNode(typeof (int), (object) 0);
        }
        this.AddRowDefinition();
        if (context.CurrentLabelNode != null)
        {
          context.CurrentLabelNode.Properties[GridElement.RowProperty] = context.RootNode.Context.CreateNode(typeof (int), (object) this.row);
          this.contentNode.Children.Add((DocumentNode) context.CurrentLabelNode);
        }
        context.CurrentFieldNode.Properties[GridElement.RowProperty] = context.RootNode.Context.CreateNode(typeof (int), (object) this.row);
        this.contentNode.Children.Add((DocumentNode) context.CurrentFieldNode);
      }
    }

    private void AddRowDefinition()
    {
      DocumentCompositeNode node = this.contentNode.Context.CreateNode(PlatformTypes.RowDefinition);
      node.Properties[RowDefinitionNode.HeightProperty] = (DocumentNode) this.contentNode.Context.CreateNode(PlatformTypes.GridLength, (IDocumentNodeValue) new DocumentNodeStringValue("Auto"));
      this.rowDefinitionsNode.Children.Add((DocumentNode) node);
    }
  }
}
