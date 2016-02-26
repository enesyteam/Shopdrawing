// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Assets.AssetPopup
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.Configuration;
using Microsoft.Expression.Framework.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.Tools.Assets
{
  internal class AssetPopup : ResizablePopup
  {
    private static readonly Size AssetPopupMinSize = new Size(285.0, 235.0);
    private static readonly Size AssetPopupDefaultSize = new Size(600.0, 400.0);

    protected override Size MinSize
    {
      get
      {
        return AssetPopup.AssetPopupMinSize;
      }
    }

    public AssetView AssetView
    {
      get
      {
        return ((AssetPopupContent) this.ContentControl).AssetView;
      }
    }

    public AssetPopup(IAssetLibrary library, IConfigurationObject configuration)
      : base((ContentControl) new AssetPopupContent(), configuration, "AssetPopup", AssetPopup.AssetPopupDefaultSize)
    {
      this.ContentControl.DataContext = (object) library;
      this.ContentControl.Focusable = false;
      this.RedirectFocusOnOpen = false;
    }

    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
      if (e.Key == Key.Escape && string.IsNullOrEmpty(this.AssetView.SearchString))
      {
        this.IsOpen = false;
        e.Handled = true;
      }
      else if (e.Key == Key.Tab)
      {
        this.IsOpen = false;
        e.Handled = true;
      }
      else
        base.OnPreviewKeyDown(e);
    }
  }
}
