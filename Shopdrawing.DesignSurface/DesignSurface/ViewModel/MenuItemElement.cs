// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.MenuItemElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.Framework.UserInterface;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class MenuItemElement : ItemsControlElement, IExpandable
  {
    public static readonly IPropertyId IsSubmenuOpenProperty = (IPropertyId) PlatformTypes.MenuItem.GetMember(MemberType.LocalProperty, "IsSubmenuOpen", MemberAccessTypes.Public);
    public static readonly MenuItemElement.ConcreteMenuItemElementFactory Factory = new MenuItemElement.ConcreteMenuItemElementFactory();

    public IPropertyId ExpansionProperty
    {
      get
      {
        return DesignTimeProperties.IsPopupOpenProperty;
      }
    }

    public IPropertyId DesignTimeExpansionProperty
    {
      get
      {
        return DesignTimeProperties.IsPopupOpenProperty;
      }
    }

    public void EnsureExpandable()
    {
      this.ExpandDefaultStyle(BaseFrameworkElement.StyleProperty);
    }

    public override void SetValue(PropertyReference propertyReference, object valueToSet)
    {
      if ((MenuItemElement.IsSubmenuOpenProperty.Equals((object) propertyReference.LastStep) || DesignTimeProperties.IsPopupOpenProperty.Equals((object) propertyReference.LastStep)) && object.Equals(valueToSet, (object) true))
        this.EnsureExpandable();
      base.SetValue(propertyReference, valueToSet);
    }

    public override void AddCustomContextMenuCommands(ICommandBar contextMenu)
    {
      base.AddCustomContextMenuCommands(contextMenu);
      contextMenu.Items.AddButton("Edit_AddSeparator");
      contextMenu.Items.AddCheckBox("Edit_ExpandControl");
    }

    public class ConcreteMenuItemElementFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new MenuItemElement();
      }
    }
  }
}
