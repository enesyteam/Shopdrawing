// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.DataGridElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.Framework.UserInterface;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class DataGridElement : BaseFrameworkElement
  {
    public static readonly IPropertyId AutoGenerateColumnsProperty = (IPropertyId) ProjectNeutralTypes.DataGrid.GetMember(MemberType.LocalProperty, "AutoGenerateColumns", MemberAccessTypes.Public);
    public static readonly IPropertyId CurrentItemProperty = (IPropertyId) ProjectNeutralTypes.DataGrid.GetMember(MemberType.LocalProperty, "CurrentItem", MemberAccessTypes.Public);
    public static readonly IPropertyId ColumnsProperty = (IPropertyId) ProjectNeutralTypes.DataGrid.GetMember(MemberType.LocalProperty, "Columns", MemberAccessTypes.Public);
    public static readonly IPropertyId ItemsSourceProperty = (IPropertyId) ProjectNeutralTypes.DataGrid.GetMember(MemberType.LocalProperty, "ItemsSource", MemberAccessTypes.Public);
    private static DataGridElement.ConcreteDataGridElementFactory factory = new DataGridElement.ConcreteDataGridElementFactory();

    public static SceneNode.ConcreteSceneNodeFactory Factory
    {
      get
      {
        return (SceneNode.ConcreteSceneNodeFactory) DataGridElement.factory;
      }
    }

    public IType ColumnBaseType
    {
      get
      {
        return this.ProjectContext.ResolveProperty(DataGridElement.ColumnsProperty).PropertyType.ItemType;
      }
    }

    public ISceneNodeCollection<SceneNode> ColumnCollection
    {
      get
      {
        return this.GetCollectionForProperty(DataGridElement.ColumnsProperty);
      }
    }

    public override IProperty InsertionTargetProperty
    {
      get
      {
        return this.Metadata.TypeResolver.ResolveProperty(DataGridElement.ColumnsProperty);
      }
    }

    public override void AddCustomContextMenuCommands(ICommandBar contextMenu)
    {
      base.AddCustomContextMenuCommands(contextMenu);
      contextMenu.Items.AddButton("Edit_BindToData");
      contextMenu.Items.AddDynamicMenu("Edit_AddDataGridColumns");
    }

    protected override void ModifyValue(PropertyReference propertyReference, object valueToSet, SceneNode.Modification modification, int index)
    {
      if (PlatformTypes.ItemsControl.IsAssignableFrom((ITypeId) this.Type) && (modification == SceneNode.Modification.SetValue || modification == SceneNode.Modification.InsertValue))
        ItemsControlElement.ClearMutuallyExclusivePropertyIfNeeded((SceneElement) this, propertyReference);
      base.ModifyValue(propertyReference, valueToSet, modification, index);
    }

    protected override void OnChildAdded(SceneNode child)
    {
      if (PlatformTypes.ItemsControl.IsAssignableFrom((ITypeId) this.Type))
        ItemsControlElement.OnChildAddedHelper((SceneElement) this, child);
      base.OnChildAdded(child);
    }

    private class ConcreteDataGridElementFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new DataGridElement();
      }
    }
  }
}
