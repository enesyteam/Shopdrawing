// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.UserInterface.GoToLineDialog
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.Expression.Code;
using Microsoft.Expression.Framework.Controls;
using System.ComponentModel;
using System.Windows;

namespace Microsoft.Expression.Code.UserInterface
{
  internal sealed class GoToLineDialog : Dialog, INotifyPropertyChanged
  {
    private string lineNumber = string.Empty;

    public string LineNumber
    {
      get
      {
        return this.lineNumber;
      }
      set
      {
        if (value != null)
          value = value.Trim();
        this.lineNumber = value;
        this.OnPropertyChanged("LineNumber");
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public GoToLineDialog()
    {
      this.DialogContent = (UIElement) FileTable.GetElement("UserInterface\\GoToLineDialog.xaml");
      this.Title = StringTable.GoToDialogTitle;
      this.SizeToContent = SizeToContent.WidthAndHeight;
      this.DataContext = (object) this;
    }

    private void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
