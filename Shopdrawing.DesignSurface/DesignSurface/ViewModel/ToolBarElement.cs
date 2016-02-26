// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.ToolBarElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.Framework.UserInterface;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class ToolBarElement : ItemsControlElement, IExpandable
  {
    public static readonly IPropertyId IsOverflowOpenProperty = (IPropertyId) PlatformTypes.ToolBar.GetMember(MemberType.LocalProperty, "IsOverflowOpen", MemberAccessTypes.Public);
    public static readonly ToolBarElement.ConcreteToolBarElementFactory Factory = new ToolBarElement.ConcreteToolBarElementFactory();

    public IPropertyId ExpansionProperty
    {
      get
      {
        return ToolBarElement.IsOverflowOpenProperty;
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
      if ((propertyReference.LastStep.Equals((object) ToolBarElement.IsOverflowOpenProperty) || DesignTimeProperties.IsPopupOpenProperty.Equals((object) propertyReference.LastStep)) && object.Equals(valueToSet, (object) true))
        this.EnsureExpandable();
      base.SetValue(propertyReference, valueToSet);
    }

    public override void AddCustomContextMenuCommands(ICommandBar contextMenu)
    {
      base.AddCustomContextMenuCommands(contextMenu);
      contextMenu.Items.AddCheckBox("Edit_ExpandControl");
    }

    public class ConcreteToolBarElementFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new ToolBarElement();
      }
    }
  }
}
