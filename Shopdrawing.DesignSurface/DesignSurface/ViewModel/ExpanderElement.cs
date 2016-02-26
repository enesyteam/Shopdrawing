// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.ExpanderElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.Framework.UserInterface;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  internal class ExpanderElement : ContentControlElement, IExpandable
  {
    public static readonly IPropertyId IsExpandedProperty = (IPropertyId) ProjectNeutralTypes.Expander.GetMember(MemberType.LocalProperty, "IsExpanded", MemberAccessTypes.Public);
    public static readonly ExpanderElement.ConcreteExpanderElementFactory Factory = new ExpanderElement.ConcreteExpanderElementFactory();

    public IPropertyId ExpansionProperty
    {
      get
      {
        return ExpanderElement.IsExpandedProperty;
      }
    }

    public IPropertyId DesignTimeExpansionProperty
    {
      get
      {
        return DesignTimeProperties.DesignTimeExpanderIsExpandedProperty;
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

    public class ConcreteExpanderElementFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new ExpanderElement();
      }
    }
  }
}
