// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Documents.MediaOpener
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Controls;
using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Documents
{
  public class MediaOpener
  {
    private string actualPath;
    private Uri uri;
    private IMessageDisplayService messageManager;
    private MediaOpener.MediaFileOpeningDialog openingDialog;
    private MediaPlayer player;

    public MediaPlayer Player
    {
      get
      {
        return this.player;
      }
    }

    public MediaOpener(string actualPath, Uri mediaUri, IMessageDisplayService messageManager)
    {
      this.actualPath = actualPath;
      this.uri = mediaUri;
      this.messageManager = messageManager;
    }

    public bool? OpenMedia()
    {
      this.openingDialog = new MediaOpener.MediaFileOpeningDialog(this.actualPath);
      MediaPlayer player = new MediaPlayer();
      this.AttachMediaPlayer(player);
      bool? nullable = new bool?();
      try
      {
        try
        {
          player.Open(this.uri);
        }
        catch (NotSupportedException ex)
        {
          int num = (int) this.messageManager.ShowMessage(StringTable.MediaPlayerTenRequired);
          return new bool?(false);
        }
        catch (COMException ex)
        {
          int num = (int) this.messageManager.ShowMessage(StringTable.MediaInsertOutOfResources);
          return new bool?(false);
        }
        if (this.openingDialog != null)
        {
          nullable = this.openingDialog.ShowDialog();
          if (this.openingDialog != null)
            this.openingDialog = (MediaOpener.MediaFileOpeningDialog) null;
        }
      }
      finally
      {
        this.DetachMediaPlayer(player);
      }
      if (nullable.HasValue && nullable.Value)
        this.player = player;
      return nullable;
    }

    private void AttachMediaPlayer(MediaPlayer player)
    {
      player.IsMuted = true;
      player.MediaOpened += new EventHandler(this.MediaOpened);
      player.MediaFailed += new EventHandler<ExceptionEventArgs>(this.MediaFailed);
    }

    private void DetachMediaPlayer(MediaPlayer player)
    {
      player.MediaOpened -= new EventHandler(this.MediaOpened);
      player.MediaFailed -= new EventHandler<ExceptionEventArgs>(this.MediaFailed);
    }

    private void MediaOpened(object sender, EventArgs args)
    {
      if (this.openingDialog == null)
        return;
      this.openingDialog.Close(new bool?(true));
      this.openingDialog = (MediaOpener.MediaFileOpeningDialog) null;
    }

    private void MediaFailed(object sender, ExceptionEventArgs args)
    {
      if (this.openingDialog == null)
        return;
      string targetPath = this.openingDialog.TargetPath;
      this.openingDialog.Close(new bool?(false));
      this.openingDialog = (MediaOpener.MediaFileOpeningDialog) null;
      this.messageManager.ShowError(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.MediaDocumentTypeMediaFailedDialogMessage, new object[2]
      {
        (object) targetPath,
        args == null || args.ErrorException == null ? (object) string.Empty : (object) args.ErrorException.Message
      }));
    }

    private class MediaFileOpeningDialog : Dialog
    {
      private string targetPath;

      public string TargetPath
      {
        get
        {
          return this.targetPath;
        }
      }

      public MediaFileOpeningDialog(string targetPath)
      {
        this.Title = StringTable.MediaDocumentTypeMediaFileOpeningDialogTitle;
        this.DialogContent = (UIElement) Microsoft.Expression.DesignSurface.FileTable.GetElement("Resources\\Documents\\MediaFileOpeningDialog.xaml");
        this.targetPath = targetPath;
        this.SizeToContent = SizeToContent.WidthAndHeight;
      }
    }
  }
}
