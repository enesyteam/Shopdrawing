// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.DataGridColumnNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.Framework.UserInterface;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class DataGridColumnNode : SceneElement
  {
    public static readonly IPropertyId CellStyleProperty = (IPropertyId) ProjectNeutralTypes.DataGridColumn.GetMember(MemberType.LocalProperty, "CellStyle", MemberAccessTypes.Public);
    public static readonly IPropertyId HeaderProperty = (IPropertyId) ProjectNeutralTypes.DataGridColumn.GetMember(MemberType.LocalProperty, "Header", MemberAccessTypes.Public);
    public static readonly IPropertyId WidthProperty = (IPropertyId) ProjectNeutralTypes.DataGridColumn.GetMember(MemberType.LocalProperty, "Width", MemberAccessTypes.Public);
    public static readonly IPropertyId ColumnHeaderProperty = (IPropertyId) ProjectNeutralTypes.DataGridColumn.GetMember(MemberType.LocalProperty, "Header", MemberAccessTypes.Public);
    public static readonly IPropertyId TemplateColumnCellTemplateProperty = (IPropertyId) ProjectNeutralTypes.DataGridTemplateColumn.GetMember(MemberType.LocalProperty, "CellTemplate", MemberAccessTypes.Public);
    public static readonly IPropertyId BoundColumnBindingProperty = (IPropertyId) ProjectNeutralTypes.DataGridBoundColumn.GetMember(MemberType.LocalProperty, "Binding", MemberAccessTypes.Public);
    private static DataGridColumnNode.ConcreteDataGridElementFactory factory = new DataGridColumnNode.ConcreteDataGridElementFactory();

    public static SceneNode.ConcreteSceneNodeFactory Factory
    {
      get
      {
        return (SceneNode.ConcreteSceneNodeFactory) DataGridColumnNode.factory;
      }
    }

    public string Header
    {
      get
      {
        string str = (string) null;
        DocumentPrimitiveNode documentPrimitiveNode = ((DocumentCompositeNode) this.DocumentNode).Properties[DataGridColumnNode.HeaderProperty] as DocumentPrimitiveNode;
        if (documentPrimitiveNode != null)
          str = documentPrimitiveNode.GetValue<string>();
        return str;
      }
    }

    public override string DisplayName
    {
      get
      {
        string str = base.DisplayName;
        if (!this.IsNamed)
        {
          string header = this.Header;
          if (!string.IsNullOrEmpty(header))
            str = str + " \"" + header + "\"";
        }
        return str;
      }
    }

    public override void AddCustomContextMenuCommands(ICommandBar contextMenu)
    {
      base.AddCustomContextMenuCommands(contextMenu);
      if (ProjectNeutralTypes.DataGridBoundColumn.IsAssignableFrom((ITypeId) this.Type))
        contextMenu.Items.AddButton("Edit_BindToData");
      contextMenu.Items.AddDynamicMenu("Edit_EditStyles", StringTable.DataGridColumnContextMenuEditStyles);
      if (!ProjectNeutralTypes.DataGridTemplateColumn.IsAssignableFrom((ITypeId) this.Type))
        return;
      contextMenu.Items.AddDynamicMenu("Edit_EditTemplates", StringTable.DataGridColumnContextMenuEditTemplates);
    }

    private class ConcreteDataGridElementFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new DataGridColumnNode();
      }
    }
  }
}
