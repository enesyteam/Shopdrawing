// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.DesignDataConfigurationButton
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class DesignDataConfigurationButton : DataConfigurationButton
  {
    public bool ShouldDisplay
    {
      get
      {
        if (this.HasNoErrors && this.DataSchemaItem.DataSourceNode.IsDesignData)
        {
          IProperty property = DesignDataConfigurationButton.WritablePropertyFromSchemaItem(this.DataSchemaItem);
          if (property != null && PlatformTypes.String.Equals((object) property.PropertyType))
            return true;
        }
        return false;
      }
    }

    public bool HasNoErrors
    {
      get
      {
        return this.DataSchemaItem.HasNoErrors;
      }
    }

    protected override bool IsEnabledCore
    {
      get
      {
        if (this.HasNoErrors)
          return base.IsEnabledCore;
        return false;
      }
    }

    public static IProperty WritablePropertyFromSchemaItem(DataSchemaItem dataSchemaItem)
    {
      IProperty property = (IProperty) null;
      DataSchemaNode dataSchemaNode = dataSchemaItem.DataSchemaNode;
      if (dataSchemaNode.Parent != null && dataSchemaNode.IsProperty)
      {
        DataSchemaNode parent = dataSchemaNode.Parent;
        property = (IProperty) dataSchemaItem.DataSourceNode.DocumentNode.TypeResolver.GetType(parent.Type).GetMember(MemberType.Property, dataSchemaNode.PathName, MemberAccessTypes.All);
        if (!DesignDataGenerator.IsPropertyWritable(property, (ITypeResolver) dataSchemaItem.ViewModel.ProjectContext))
          property = (IProperty) null;
      }
      return property;
    }

    public override DataConfigurationPopup CreatePopup(DataSchemaItem dataSchemaItem)
    {
      return (DataConfigurationPopup) new DesignDataTypeConfigurationPopup(dataSchemaItem);
    }
  }
}
