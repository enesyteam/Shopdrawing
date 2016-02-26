// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.XmlDataSourceDialog
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.UserInterface;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public sealed class XmlDataSourceDialog : Dialog
  {
    private XmlDataSourceDialogModel model;
    private MessageBubbleHelper controlNameMessageBubble;

    public XmlDataSourceDialogModel Model
    {
      get
      {
        return this.model;
      }
    }

    private string DialogTitle
    {
      get
      {
        switch (this.model.Mode)
        {
          case DataSourceDialogMode.LiveXmlData:
            return StringTable.AddXmlDataSourceDialogTitle;
          case DataSourceDialogMode.DefineNewSampleData:
            return StringTable.DefineNewSampleData;
          case DataSourceDialogMode.SampleDataFromXml:
            return StringTable.ImportSampleDataFromXMLSource;
          case DataSourceDialogMode.DefineNewDataStore:
            return StringTable.DefineNewDataStoreTitle;
          default:
            return (string) null;
        }
      }
    }

    public XmlDataSourceDialog(DataPanelModel dataPanelModel, string initialDataSourceName, DataSourceDialogMode mode)
    {
      this.model = new XmlDataSourceDialogModel(dataPanelModel, initialDataSourceName, mode);
      FrameworkElement element = FileTable.GetElement("Resources\\DataPane\\XmlDataSourceDialog.xaml");
      ((UIElement) element.FindName("XmlDataSourceUrlTextBox")).LostFocus += new RoutedEventHandler(this.DataSourceUrlTextBox_LostFocus);
      element.DataContext = (object) this.model;
      this.DialogContent = (UIElement) element;
      this.Title = this.DialogTitle;
      this.SizeToContent = SizeToContent.WidthAndHeight;
      this.controlNameMessageBubble = new MessageBubbleHelper((UIElement) (element.FindName("DataSourceNameTextBox") as TextBox), (IMessageBubbleValidator) new DataSourceNameValidator(this.model));
    }

    private void DataSourceUrlTextBox_LostFocus(object sender, RoutedEventArgs e)
    {
      this.model.UpdateDataSourceToNormalizedDataSource();
    }
  }
}
