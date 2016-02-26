// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.ComboBoxData
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class ComboBoxData : IConfigurationOptionData
  {
    public IPopupControlCallback PopupCallback { get; private set; }

    public ConfigurationPlaceholder Control { get; private set; }

    public DataConfigurationPopup Popup { get; private set; }

    public string Label
    {
      get
      {
        return this.Control.Label;
      }
    }

    public string AutomationId
    {
      get
      {
        return this.Control.AutomationId;
      }
    }

    public IList<SampleDataConfigurationOption> ComboBoxOptions
    {
      get
      {
        return this.Control.ComboBoxOptions;
      }
    }

    public SampleDataConfigurationOption SelectedOption
    {
      get
      {
        foreach (SampleDataConfigurationOption configurationOption in (IEnumerable<SampleDataConfigurationOption>) this.ComboBoxOptions)
        {
          if (this.PopupCallback.GetValue(this.Control) == configurationOption)
            return configurationOption;
        }
        return (SampleDataConfigurationOption) null;
      }
      set
      {
        this.Popup.MouseCaptureWorkaround(this.Popup.Child);
        if (this.Control == ConfigurationPlaceholder.Type)
          this.Popup.IsPerformingSchemaChange = true;
        this.PopupCallback.SetValue(this.Control, (object) value);
        this.Popup.GenerateContent();
      }
    }

    public ComboBoxData(IPopupControlCallback popupCallback, ConfigurationPlaceholder control, DataConfigurationPopup popup)
    {
      this.PopupCallback = popupCallback;
      this.Control = control;
      this.Popup = popup;
    }
  }
}
