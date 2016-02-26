// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.DesignDataTypeConfigurationPopup
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.SampleData;
using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Expression.Project;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class DesignDataTypeConfigurationPopup : DataConfigurationPopup
  {
    public DesignDataTypeConfigurationPopup(DataSchemaItem dataSchemaItem)
      : base((IPopupControlCallback) new DesignDataTypeConfigurationPopup.DataPanePopupCallback(dataSchemaItem), ConfigurationPlaceholder.StringFormat)
    {
      this.IsPerformingSchemaChange = !DesignDataHelper.GetDesignDataFile(dataSchemaItem.DataSourceNode.DocumentNode).IsOpen;
    }

    private class DataPanePopupCallback : IPopupControlCallback
    {
      private IProperty property;
      private IProjectItem designDataFile;
      private SampleStringConfiguration stringConfigurator;

      public DataPanePopupCallback(DataSchemaItem dataSchemaItem)
      {
        DataSchemaNode parent = dataSchemaItem.DataSchemaNode.Parent;
        dataSchemaItem.DataSourceNode.DocumentNode.TypeResolver.GetType(parent.Type);
        this.property = DesignDataConfigurationButton.WritablePropertyFromSchemaItem(dataSchemaItem);
        this.designDataFile = DesignDataHelper.GetDesignDataFile(dataSchemaItem.DataSourceNode.DocumentNode);
        this.stringConfigurator = new SampleStringConfiguration(SampleDataFormatHelper.NormalizeFormat(SampleBasicType.String, (string) null, false), (string) null);
      }

      public object GetValue(ConfigurationPlaceholder control)
      {
        if (control == ConfigurationPlaceholder.Type)
          return (object) SampleDataConfigurationOption.TypeString;
        if (control == ConfigurationPlaceholder.RandomLatinWordCount || control == ConfigurationPlaceholder.RandomLatinWordLength || control == ConfigurationPlaceholder.StringFormat)
          return this.stringConfigurator.GetConfigurationValue(control);
        return (object) null;
      }

      public void SetValue(ConfigurationPlaceholder control, object value)
      {
        if (control != ConfigurationPlaceholder.RandomLatinWordCount && control != ConfigurationPlaceholder.RandomLatinWordLength && control != ConfigurationPlaceholder.StringFormat)
          return;
        this.stringConfigurator.SetConfigurationValue(control, value);
        using (TemporaryCursor.SetWaitCursor())
          DesignDataGenerator.UpdateDesignValues(this.designDataFile, this.property, (ISampleTypeConfiguration) this.stringConfigurator);
      }
    }
  }
}
