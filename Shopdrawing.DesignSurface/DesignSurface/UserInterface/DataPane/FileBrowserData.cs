// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.FileBrowserData
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.Documents;
using System.Windows.Forms;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class FileBrowserData : IConfigurationOptionData
  {
    private IPopupControlCallback popupCallback;
    private ConfigurationPlaceholder control;

    public string Label
    {
      get
      {
        return this.control.Label;
      }
    }

    public string AutomationId
    {
      get
      {
        return this.control.AutomationId;
      }
    }

    public ICommand SetNewFolderCommand
    {
      get
      {
        return (ICommand) new ConfigureSampleDataPopupFileBrowserControl.SetNewFolderCommand(new ConfigureSampleDataPopupFileBrowserControl.SetNewFolderCommand.StringDelegateHandler(this.SetNewFolder));
      }
    }

    public ICommand BrowseForFolderCommand
    {
      get
      {
        return (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.BrowseForFolder));
      }
    }

    public string DirectoryDisplayText { get; set; }

    public FileBrowserData(IPopupControlCallback popupCallback, ConfigurationPlaceholder control)
    {
      this.popupCallback = popupCallback;
      this.control = control;
      object obj = this.popupCallback.GetValue(control);
      this.DirectoryDisplayText = obj != null ? obj.ToString() : (string) null;
    }

    private void SetNewFolder(string path)
    {
      if (!PathHelper.DirectoryExists(path))
        return;
      this.popupCallback.SetValue(this.control, (object) path);
    }

    private void BrowseForFolder()
    {
      bool flag = false;
      string path = this.DirectoryDisplayText;
      string folderBrowserDialog1 = StringTable.SampleDataConfigurationImageFolderBrowserDialog;
      if (ExpressionFileDialog.CanPickFolders)
      {
        ExpressionOpenFileDialog expressionOpenFileDialog = new ExpressionOpenFileDialog();
        expressionOpenFileDialog.Title = folderBrowserDialog1;
        expressionOpenFileDialog.InitialDirectory = path;
        expressionOpenFileDialog.PickFolders = true;
        bool? nullable = expressionOpenFileDialog.ShowDialog();
        if (nullable.HasValue && nullable.Value)
        {
          flag = true;
          path = expressionOpenFileDialog.FileName;
        }
      }
      else
      {
        using (new ModalDialogHelper())
        {
          FolderBrowserDialog folderBrowserDialog2 = new FolderBrowserDialog();
          folderBrowserDialog2.Description = folderBrowserDialog1;
          folderBrowserDialog2.SelectedPath = path;
          if (folderBrowserDialog2.ShowDialog() == DialogResult.OK)
          {
            flag = true;
            path = folderBrowserDialog2.SelectedPath;
          }
        }
      }
      if (!flag)
        return;
      this.SetNewFolder(path);
    }
  }
}
