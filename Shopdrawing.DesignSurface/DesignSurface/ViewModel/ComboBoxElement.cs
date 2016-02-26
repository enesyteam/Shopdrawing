// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.ComboBoxElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.Framework.UserInterface;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class ComboBoxElement : ItemsControlElement, IExpandable
  {
    public static readonly IPropertyId IsDropDownOpenProperty = (IPropertyId) PlatformTypes.ComboBox.GetMember(MemberType.LocalProperty, "IsDropDownOpen", MemberAccessTypes.Public);
    public static readonly ComboBoxElement.ConcreteComboBoxElementFactory Factory = new ComboBoxElement.ConcreteComboBoxElementFactory();

    public IPropertyId ExpansionProperty
    {
      get
      {
        return DesignTimeProperties.IsExpandedProperty;
      }
    }

    public IPropertyId DesignTimeExpansionProperty
    {
      get
      {
        return DesignTimeProperties.IsExpandedProperty;
      }
    }

    public void EnsureExpandable()
    {
      if (this.ProjectContext.IsCapabilitySet(PlatformCapability.UsePlatformPopup))
        return;
      this.ExpandDefaultStyle(BaseFrameworkElement.StyleProperty);
    }

    public override void SetValue(PropertyReference propertyReference, object valueToSet)
    {
      ReferenceStep lastStep = propertyReference.LastStep;
      if ((ComboBoxElement.IsDropDownOpenProperty.Equals((object) lastStep) || DesignTimeProperties.IsPopupOpenProperty.Equals((object) lastStep) || DesignTimeProperties.IsExpandedProperty.Equals((object) lastStep)) && object.Equals(valueToSet, (object) true))
        this.EnsureExpandable();
      base.SetValue(propertyReference, valueToSet);
    }

    public override void AddCustomContextMenuCommands(ICommandBar contextMenu)
    {
      base.AddCustomContextMenuCommands(contextMenu);
      contextMenu.Items.AddCheckBox("Edit_ExpandControl");
    }

    public class ConcreteComboBoxElementFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new ComboBoxElement();
      }
    }
  }
}
