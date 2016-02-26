// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.ContentControlElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.Framework.UserInterface;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class ContentControlElement : ControlElement
  {
    public static readonly IPropertyId ContentProperty = (IPropertyId) PlatformTypes.ContentControl.GetMember(MemberType.LocalProperty, "Content", MemberAccessTypes.Public);
    public static readonly IPropertyId ContentTemplateProperty = (IPropertyId) PlatformTypes.ContentControl.GetMember(MemberType.LocalProperty, "ContentTemplate", MemberAccessTypes.Public);
    public static readonly ContentControlElement.ConcreteContentControlElementFactory Factory = new ContentControlElement.ConcreteContentControlElementFactory();

    public override void AddCustomContextMenuCommands(ICommandBar contextMenu)
    {
      base.AddCustomContextMenuCommands(contextMenu);
      contextMenu.Items.AddButton("Edit_BindToData");
    }

    public class ConcreteContentControlElementFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new ContentControlElement();
      }
    }
  }
}
