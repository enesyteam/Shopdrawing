// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.DataConfigurationPopup
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.Framework.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public abstract class DataConfigurationPopup : WorkaroundPopup
  {
    private ConfigurationPlaceholder rootConfig;

    public IPopupControlCallback PopupCallback { get; private set; }

    public ObservableCollection<IConfigurationOptionData> ConfigurationOptions { get; private set; }

    public bool IsPerformingSchemaChange { get; set; }

    protected DataConfigurationPopup(IPopupControlCallback popupCallback, ConfigurationPlaceholder rootConfig)
    {
      this.rootConfig = rootConfig;
      FrameworkElement element = FileTable.GetElement("Resources\\DataPane\\ConfigureSampleDataPopup.xaml");
      this.Child = (UIElement) element;
      element.DataContext = (object) this;
      this.Focusable = true;
      this.PopupCallback = popupCallback;
      this.ConfigurationOptions = new ObservableCollection<IConfigurationOptionData>();
      this.GenerateContent();
    }

    public void GenerateContent()
    {
      this.ConfigurationOptions.Clear();
      this.GenerateContentFrom(this.rootConfig);
    }

    internal virtual void MouseCaptureWorkaround(UIElement element)
    {
    }

    private void GenerateContentFrom(ConfigurationPlaceholder control)
    {
      switch (control.ControlType)
      {
        case ConfigurationControlType.ComboBox:
          this.ConfigurationOptions.Add(this.GenerateComboBox(control));
          SampleDataConfigurationOption configurationOption = this.PopupCallback.GetValue(control) as SampleDataConfigurationOption;
          if (configurationOption == null)
            break;
          using (IEnumerator<ConfigurationPlaceholder> enumerator = configurationOption.ChildControls.GetEnumerator())
          {
            while (enumerator.MoveNext())
              this.GenerateContentFrom(enumerator.Current);
            break;
          }
        case ConfigurationControlType.NumberSlider:
          this.ConfigurationOptions.Add(this.GenerateSlider(control));
          break;
        case ConfigurationControlType.DirectoryBrowser:
          this.ConfigurationOptions.Add(this.GenerateFileBrowser(control));
          break;
      }
    }

    private IConfigurationOptionData GenerateComboBox(ConfigurationPlaceholder control)
    {
      return (IConfigurationOptionData) new ComboBoxData(this.PopupCallback, control, this);
    }

    private IConfigurationOptionData GenerateFileBrowser(ConfigurationPlaceholder control)
    {
      return (IConfigurationOptionData) new FileBrowserData(this.PopupCallback, control);
    }

    private IConfigurationOptionData GenerateSlider(ConfigurationPlaceholder control)
    {
      return (IConfigurationOptionData) new NumberSliderData(this.PopupCallback, control);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
      base.OnKeyDown(e);
      if (e.Key != Key.Return)
        return;
      e.Handled = true;
      this.GenerateContent();
    }
  }
}
