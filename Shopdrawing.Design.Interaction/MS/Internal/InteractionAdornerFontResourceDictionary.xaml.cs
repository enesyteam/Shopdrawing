// Decompiled with JetBrains decompiler
// Type: MS.Internal.Interaction.AdornerFontResourceDictionary
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
  internal partial class AdornerFontResourceDictionary : ResourceDictionary, IComponentConnector
  {
    private bool _contentLoaded;

    internal AdornerFontResourceDictionary()
    {
      this.Add((object) AdornerFonts.FontFamilyKey, (object) new FontFamily("Tahoma"));
      this.Add((object) AdornerFonts.FontSizeKey, (object) 8.4);
      this.InitializeComponent();
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Windows.Design.Interaction;component/ms/internal/interaction/adornerfontresourcedictionary.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      this._contentLoaded = true;
    }
  }
}
