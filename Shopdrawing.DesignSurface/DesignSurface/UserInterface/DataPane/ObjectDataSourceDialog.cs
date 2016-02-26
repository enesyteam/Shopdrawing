// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.ObjectDataSourceDialog
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  internal abstract class ObjectDataSourceDialog : ClrObjectDialogBase
  {
    private TextBox dataSourceNameTextBox;
    private string suggestedName;

    public DataPanelModel Model { get; private set; }

    public abstract string DataSourceNameSuffix { get; }

    public string DataSourceName
    {
      get
      {
        return this.dataSourceNameTextBox.Text;
      }
    }

    public virtual string DataSourceNameError
    {
      get
      {
        string str = (string) null;
        if (string.IsNullOrEmpty(this.DataSourceName))
          str = StringTable.DataSourceErrorEmptyName;
        else if (!ObjectDataSourceDialog.IsDataSourceNameValid(this.DataSourceName))
          str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.DataSourceNameErrorDescription, new object[1]
          {
            (object) this.DataSourceName
          });
        return str;
      }
    }

    protected ObjectDataSourceDialog(DataPanelModel model, string defaultName)
    {
      this.Model = model;
      FrameworkElement element = FileTable.GetElement("Resources\\DataPane\\ClrObjectDataSourceDialog.xaml");
      element.DataContext = (object) this;
      this.DialogContent = (UIElement) element;
      HeaderedContentControl headeredContentControl = (HeaderedContentControl) LogicalTreeHelper.FindLogicalNode((DependencyObject) this.DialogContent, "DataSourceNameContainer");
      headeredContentControl.ApplyTemplate();
      this.dataSourceNameTextBox = (TextBox) headeredContentControl.Template.FindName("DataSourceNameTextBox", (FrameworkElement) headeredContentControl);
      this.dataSourceNameTextBox.Text = defaultName;
      this.Initialize(this.Model.DesignerContext);
      this.ResizeMode = ResizeMode.CanResizeWithGrip;
      this.SizeToContent = SizeToContent.Manual;
      this.Width = 400.0;
      this.Height = 600.0;
      this.suggestedName = this.dataSourceNameTextBox.Text;
    }

    protected override void SelectionContextPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "PrimarySelection"))
        return;
      this.OnPropertyChanged("ObjectType");
      if (!(this.DataSourceName == this.suggestedName))
        return;
      this.suggestedName = this.ObjectType.Name + this.DataSourceNameSuffix;
      this.dataSourceNameTextBox.Text = this.suggestedName;
    }

    protected override void OnAcceptButtonExecute()
    {
      string dataSourceNameError = this.DataSourceNameError;
      if (!string.IsNullOrEmpty(dataSourceNameError))
        this.Model.DesignerContext.MessageDisplayService.ShowError(dataSourceNameError);
      else
        base.OnAcceptButtonExecute();
    }

    private static bool IsDataSourceNameValid(string name)
    {
      if (name == null)
        return false;
      name = name.Trim();
      if (name.Length == 0 || !char.IsLetter(name[0]))
        return false;
      foreach (char c in name)
      {
        if (!char.IsLetterOrDigit(c) && (int) c != 95)
          return false;
      }
      return true;
    }
  }
}
