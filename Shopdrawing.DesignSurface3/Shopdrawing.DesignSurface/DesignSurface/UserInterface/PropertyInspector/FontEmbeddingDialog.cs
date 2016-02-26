// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.FontEmbeddingDialog
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Controls;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class FontEmbeddingDialog : Dialog, IComponentConnector
  {
    private FontEmbeddingDialogModel model;
    internal FontEmbeddingDialog Root;
    internal ClearableTextBox SearchTextBox;
    internal Button AcceptButton;
    internal Button CancelButton;
    private bool _contentLoaded;

    public FontEmbeddingDialogModel Model
    {
      get
      {
        return this.model;
      }
    }

    internal FontEmbeddingDialog(SceneViewModel viewModel)
    {
      SourcedFontFamilyItem.DefaultPreviewFontFamilyName = ((FontFamily) this.FindResource((object) SystemFonts.MessageFontFamilyKey)).ToString();
      this.model = new FontEmbeddingDialogModel(viewModel);
      this.DataContext = (object) this.model;
      this.InitializeComponent();
      this.SizeToContent = SizeToContent.Manual;
      this.Width = 330.0;
      this.Height = 485.0;
      this.MinWidth = 225.0;
      this.MinHeight = 375.0;
      this.ResizeMode = ResizeMode.CanResizeWithGrip;
      this.Title = StringTable.FontEmbeddingDialogTitle;
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/propertyinspector/categoryeditors/text/fontembeddingdialog.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.Root = (FontEmbeddingDialog) target;
          break;
        case 2:
          this.SearchTextBox = (ClearableTextBox) target;
          break;
        case 3:
          this.AcceptButton = (Button) target;
          break;
        case 4:
          this.CancelButton = (Button) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
