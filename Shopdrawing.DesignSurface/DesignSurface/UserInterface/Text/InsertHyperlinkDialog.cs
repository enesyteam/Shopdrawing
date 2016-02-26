// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Text.InsertHyperlinkDialog
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.Framework.Controls;
using System;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.UserInterface.Text
{
  public sealed class InsertHyperlinkDialog : Dialog
  {
    private InsertHyperlinkDialog.InsertHyperlinkDialogModel model;

    private InsertHyperlinkDialog(string navigateUri, string text)
    {
      FrameworkElement element = FileTable.GetElement("Resources\\TextPane\\InsertHyperlinkDialog.xaml");
      this.DialogContent = (UIElement) element;
      this.ResizeMode = ResizeMode.NoResize;
      this.SizeToContent = SizeToContent.WidthAndHeight;
      this.Title = StringTable.CreateHyperlinkDialogTitle;
      this.model = new InsertHyperlinkDialog.InsertHyperlinkDialogModel();
      this.model.NavigateUri = navigateUri;
      this.model.Text = text;
      element.DataContext = (object) this.model;
    }

    public static bool CreateHyperlink(ref Uri navigateUri, ref string hyperlinkText)
    {
      InsertHyperlinkDialog insertHyperlinkDialog = new InsertHyperlinkDialog(navigateUri != (Uri) null ? navigateUri.ToString() : string.Empty, hyperlinkText);
      bool? nullable = insertHyperlinkDialog.ShowDialog();
      if (!nullable.HasValue || !nullable.Value || !Uri.TryCreate(insertHyperlinkDialog.model.NavigateUri, UriKind.RelativeOrAbsolute, out navigateUri))
        return false;
      hyperlinkText = insertHyperlinkDialog.model.Text;
      return true;
    }

    private class InsertHyperlinkDialogModel
    {
      private string text;
      private string navigateUri;

      public string Text
      {
        get
        {
          return this.text;
        }
        set
        {
          this.text = value;
        }
      }

      public string NavigateUri
      {
        get
        {
          return this.navigateUri;
        }
        set
        {
          this.navigateUri = value;
        }
      }
    }
  }
}
