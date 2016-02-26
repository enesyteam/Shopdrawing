// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.PopupElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.Framework.UserInterface;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class PopupElement : BaseFrameworkElement, IExpandable
  {
    public static readonly IPropertyId IsOpenProperty = (IPropertyId) PlatformTypes.Page.GetMember(MemberType.LocalProperty, "IsOpen", MemberAccessTypes.Public);
    public static readonly PopupElement.ConcretePopupElementFactory Factory = new PopupElement.ConcretePopupElementFactory();

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
    }

    public override void AddCustomContextMenuCommands(ICommandBar contextMenu)
    {
      base.AddCustomContextMenuCommands(contextMenu);
      contextMenu.Items.AddCheckBox("Edit_ExpandControl");
    }

    public class ConcretePopupElementFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new PopupElement();
      }
    }
  }
}
