// Decompiled with JetBrains decompiler
// Type: MS.Internal.Interaction.AdornerColorResourceDictionary
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design.Interaction;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace MS.Internal.Interaction
{
  internal partial class AdornerColorResourceDictionary : ResourceDictionary, IComponentConnector
  {
    private bool _contentLoaded;

    internal AdornerColorResourceDictionary()
    {
      if (SystemParameters.HighContrast)
      {
        this.Add((object) AdornerColors.AlignmentMarkColorKey, (object) SystemColors.ControlDarkColor);
        this.Add((object) AdornerColors.ElementBorderColorKey, (object) SystemColors.ControlLightLightColor);
        this.Add((object) AdornerColors.GlyphFillColorKey, (object) SystemColors.ControlDarkColor);
        this.Add((object) AdornerColors.HandleBorderColorKey, (object) SystemColors.ControlTextColor);
        this.Add((object) AdornerColors.HandleFillColorKey, (object) SystemColors.ControlColor);
        this.Add((object) AdornerColors.HandleEmptyFillColorKey, (object) Colors.Transparent);
        this.Add((object) AdornerColors.HandleFillHoverColorKey, (object) SystemColors.ActiveCaptionColor);
        this.Add((object) AdornerColors.HandleFillPressedColorKey, (object) SystemColors.ActiveBorderColor);
        this.Add((object) AdornerColors.MoveHandleContentColorKey, (object) SystemColors.ControlDarkColor);
        this.Add((object) AdornerColors.MoveHandleFillColorKey, (object) Colors.Transparent);
        this.Add((object) AdornerColors.MoveHandleFillHoverColorKey, (object) SystemColors.ControlColor);
        this.Add((object) AdornerColors.RailFillColorKey, (object) Colors.Transparent);
        this.Add((object) AdornerColors.SelectionFrameBorderColorKey, (object) SystemColors.ControlDarkColor);
        this.Add((object) AdornerColors.SelectionFrameFillColorKey, (object) Colors.Transparent);
        this.Add((object) AdornerColors.SimpleWashColorKey, (object) Colors.Transparent);
        this.Add((object) AdornerColors.ToggledGlyphFillColorKey, (object) SystemColors.ControlDarkDarkColor);
      }
      else
      {
        this.Add((object) AdornerColors.AlignmentMarkColorKey, (object) Color.FromArgb(byte.MaxValue, (byte) 99, (byte) 65, (byte) 24));
        this.Add((object) AdornerColors.ElementBorderColorKey, (object) Color.FromArgb((byte) 102, (byte) 116, (byte) 142, (byte) 170));
        this.Add((object) AdornerColors.GlyphFillColorKey, (object) SystemColors.ControlDarkColor);
        this.Add((object) AdornerColors.HandleBorderColorKey, (object) Color.FromRgb((byte) 116, (byte) 142, (byte) 170));
        this.Add((object) AdornerColors.HandleFillColorKey, (object) Color.FromRgb((byte) 203, (byte) 216, (byte) 221));
        this.Add((object) AdornerColors.HandleEmptyFillColorKey, (object) Colors.White);
        this.Add((object) AdornerColors.HandleFillHoverColorKey, (object) Color.FromRgb((byte) 247, (byte) 148, (byte) 28));
        this.Add((object) AdornerColors.HandleFillPressedColorKey, (object) Color.FromRgb((byte) 204, (byte) 0, (byte) 0));
        this.Add((object) AdornerColors.MoveHandleContentColorKey, (object) Colors.Transparent);
        this.Add((object) AdornerColors.MoveHandleFillColorKey, (object) Color.FromArgb((byte) 102, (byte) 154, (byte) 191, (byte) 229));
        this.Add((object) AdornerColors.MoveHandleFillHoverColorKey, (object) Color.FromArgb((byte) 102, (byte) 154, (byte) 191, (byte) 229));
        this.Add((object) AdornerColors.RailFillColorKey, (object) Color.FromArgb((byte) 102, (byte) 154, (byte) 191, (byte) 229));
        this.Add((object) AdornerColors.SelectionFrameBorderColorKey, (object) Color.FromArgb((byte) sbyte.MinValue, (byte) 116, (byte) 142, (byte) 170));
        this.Add((object) AdornerColors.SelectionFrameFillColorKey, (object) Color.FromArgb((byte) 127, (byte) 99, (byte) 65, (byte) 24));
        this.Add((object) AdornerColors.SimpleWashColorKey, (object) Color.FromArgb((byte) 51, (byte) 204, (byte) 204, (byte) 204));
        this.Add((object) AdornerColors.ToggledGlyphFillColorKey, (object) SystemColors.ControlDarkDarkColor);
      }
      this.InitializeComponent();
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Windows.Design.Interaction;component/ms/internal/interaction/adornercolorresourcedictionary.xaml", UriKind.Relative));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerNonUserCode]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      this._contentLoaded = true;
    }
  }
}
