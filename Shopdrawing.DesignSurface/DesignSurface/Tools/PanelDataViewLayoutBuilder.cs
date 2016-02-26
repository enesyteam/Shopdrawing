// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.PanelDataViewLayoutBuilder
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.Tools
{
  public class PanelDataViewLayoutBuilder : IDataViewLayoutBuilder
  {
    private int insertIndex;

    public PanelDataViewLayoutBuilder()
      : this(-1)
    {
    }

    public PanelDataViewLayoutBuilder(int insertIndex)
    {
      this.insertIndex = insertIndex;
    }

    public virtual void SetElementLayout(DataViewBuilderContext context)
    {
      IProperty defaultContentProperty = context.RootNode.Type.Metadata.DefaultContentProperty;
      DocumentCompositeNode contentNode = (DocumentCompositeNode) context.RootNode.Properties[(IPropertyId) defaultContentProperty];
      if (contentNode == null)
      {
        contentNode = context.RootNode.Context.CreateNode((ITypeId) defaultContentProperty.PropertyType);
        context.RootNode.Properties[PanelElement.ChildrenProperty] = (DocumentNode) contentNode;
      }
      if (context.CurrentLabelNode != null)
        this.AddChildToContent(contentNode, context.CurrentLabelNode);
      this.AddChildToContent(contentNode, context.CurrentFieldNode);
    }

    private void AddChildToContent(DocumentCompositeNode contentNode, DocumentCompositeNode childNode)
    {
      if (this.insertIndex >= 0)
        contentNode.Children.Insert(this.insertIndex++, (DocumentNode) childNode);
      else
        contentNode.Children.Add((DocumentNode) childNode);
    }
  }
}
