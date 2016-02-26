// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.MediaElementThumnailGrabber
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Extensibility;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Documents;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface
{
  internal class MediaElementThumnailGrabber
  {
    private bool? supportsFirstFramePreview = new bool?();
    private Queue<MediaElementThumnailGrabber.ThumbnailWorkerParameters> workQueue = new Queue<MediaElementThumnailGrabber.ThumbnailWorkerParameters>();
    private MediaPlayer workerMediaPlayer;
    private DispatcherTimer frameRenderTimer;

    public IServices Services { get; private set; }

    private bool SupportsFirstFramePreview
    {
      get
      {
        if (!this.supportsFirstFramePreview.HasValue)
        {
          string versionedRegistryPath = this.Services.GetService<IExpressionInformationService>().VersionedRegistryPath;
          if (!string.IsNullOrEmpty(versionedRegistryPath))
            this.supportsFirstFramePreview = new bool?(!RegistryHelper.RetrieveCurrentUserRegistryValue<bool>(versionedRegistryPath, "DisableFFP"));
          this.supportsFirstFramePreview = new bool?(true);
        }
        return this.supportsFirstFramePreview.Value;
      }
    }

    public MediaElementThumnailGrabber(IServices services)
    {
      this.Services = services;
    }

    public void BeginGrabThumbnail(Uri source, Action<Uri, BitmapSource, string> callback)
    {
      if (!this.SupportsFirstFramePreview)
        return;
      MediaElementThumnailGrabber.ThumbnailWorkerParameters workerParameters = new MediaElementThumnailGrabber.ThumbnailWorkerParameters();
      workerParameters.Source = source;
      workerParameters.Callback = callback;
      bool flag = this.workQueue.Count > 0;
      this.workQueue.Enqueue(workerParameters);
      if (flag)
        return;
      UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.Normal, (Action) (() => this.ProcessNextQueueItem()));
    }

    private void ProcessNextQueueItem()
    {
      if (this.workQueue.Count == 0)
      {
        if (this.workerMediaPlayer == null)
          return;
        this.workerMediaPlayer.MediaOpened -= new EventHandler(this.OnMediaOpened);
        this.workerMediaPlayer.MediaFailed -= new EventHandler<ExceptionEventArgs>(this.OnMediaFailed);
        this.workerMediaPlayer.Close();
        this.workerMediaPlayer = (MediaPlayer) null;
      }
      else
      {
        MediaElementThumnailGrabber.ThumbnailWorkerParameters parameters = this.workQueue.Peek();
        if (MediaElementThumnailGrabber.ShouldSkipPreviewFor(parameters))
        {
          this.workQueue.Dequeue();
          UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.Normal, (Action) (() => this.ProcessNextQueueItem()));
        }
        else
        {
          if (this.workerMediaPlayer == null)
          {
            this.workerMediaPlayer = new MediaPlayer();
            this.workerMediaPlayer.MediaOpened += new EventHandler(this.OnMediaOpened);
            this.workerMediaPlayer.MediaFailed += new EventHandler<ExceptionEventArgs>(this.OnMediaFailed);
          }
          if (this.workerMediaPlayer.Source != parameters.Source)
          {
            try
            {
              this.workerMediaPlayer.ScrubbingEnabled = false;
              this.workerMediaPlayer.Open(parameters.Source);
            }
            catch (Exception ex)
            {
              this.HandleLoadFail();
            }
          }
          else
            this.MoveToNextWorkItem(this.RenderFrame(), (string) null);
        }
      }
    }

    private static bool ShouldSkipPreviewFor(MediaElementThumnailGrabber.ThumbnailWorkerParameters parameters)
    {
      if (parameters.Source == (Uri) null || !parameters.Source.IsAbsoluteUri)
        return true;
      string localPath = parameters.Source.LocalPath;
      return string.IsNullOrEmpty(localPath) || !PathHelper.FileExists(localPath);
    }

    private void MoveToNextWorkItem(BitmapSource image, string errorMessage)
    {
      MediaElementThumnailGrabber.ThumbnailWorkerParameters workerParameters = this.workQueue.Dequeue();
      workerParameters.Callback(workerParameters.Source, image, errorMessage);
      UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.Normal, (Action) (() => this.ProcessNextQueueItem()));
    }

    private void HandleLoadFail()
    {
      this.MoveToNextWorkItem((BitmapSource) null, StringTable.MediaElementFirstFramePreviewError);
    }

    private void OnMediaFailed(object sender, ExceptionEventArgs e)
    {
      this.HandleLoadFail();
    }

    private void OnMediaOpened(object sender, EventArgs e)
    {
      if (this.RenderFrame() == null)
      {
        this.MoveToNextWorkItem((BitmapSource) null, (string) null);
      }
      else
      {
        this.workerMediaPlayer.ScrubbingEnabled = true;
        this.workerMediaPlayer.Position = TimeSpan.FromSeconds(0.1);
        if (this.frameRenderTimer == null)
        {
          this.frameRenderTimer = new DispatcherTimer();
          this.frameRenderTimer.Interval = TimeSpan.FromSeconds(1.0);
        }
        this.frameRenderTimer.Tick += new EventHandler(this.FrameRenderTimerTick);
        this.frameRenderTimer.Start();
      }
    }

    private void FrameRenderTimerTick(object sender, EventArgs e)
    {
      if (this.frameRenderTimer != null)
      {
        this.frameRenderTimer.Stop();
        this.frameRenderTimer.Tick -= new EventHandler(this.FrameRenderTimerTick);
        this.frameRenderTimer = (DispatcherTimer) null;
      }
      if (this.workerMediaPlayer == null)
        return;
      this.MoveToNextWorkItem(this.RenderFrame(), (string) null);
    }

    private BitmapSource RenderFrame()
    {
      this.workQueue.Peek();
      if (!this.workerMediaPlayer.HasVideo || !this.workerMediaPlayer.CanPause)
        return (BitmapSource) null;
      RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(this.workerMediaPlayer.NaturalVideoWidth, this.workerMediaPlayer.NaturalVideoHeight, 96.0, 96.0, PixelFormats.Default);
      DrawingVisual drawingVisual = new DrawingVisual();
      using (DrawingContext drawingContext = drawingVisual.RenderOpen())
        drawingContext.DrawVideo(this.workerMediaPlayer, new Rect(0.0, 0.0, (double) this.workerMediaPlayer.NaturalVideoWidth, (double) this.workerMediaPlayer.NaturalVideoHeight));
      renderTargetBitmap.Render((Visual) drawingVisual);
      renderTargetBitmap.Freeze();
      return (BitmapSource) BitmapFrame.Create((BitmapSource) renderTargetBitmap);
    }

    private class ThumbnailWorkerParameters
    {
      public Uri Source;
      public Action<Uri, BitmapSource, string> Callback;
    }
  }
}
