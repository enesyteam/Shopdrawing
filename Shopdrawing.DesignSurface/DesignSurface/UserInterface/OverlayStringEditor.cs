// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.OverlayStringEditor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.ValueEditors;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  public class OverlayStringEditor : StringEditor
  {
    public static readonly DependencyProperty OverlayTextProperty = OverlayTextBox.OverlayTextProperty.AddOwner(typeof (OverlayStringEditor));
    private static readonly DependencyPropertyKey HasTextPropertyKey = DependencyProperty.RegisterReadOnly("HasText", typeof (bool), typeof (OverlayStringEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
    public static readonly DependencyProperty HasTextProperty = OverlayStringEditor.HasTextPropertyKey.DependencyProperty;

    public string OverlayText
    {
      get
      {
        return (string) this.GetValue(OverlayStringEditor.OverlayTextProperty);
      }
      set
      {
        this.SetValue(OverlayStringEditor.OverlayTextProperty, (object) value);
      }
    }

    public bool HasText
    {
      get
      {
        return (bool) this.GetValue(OverlayStringEditor.HasTextProperty);
      }
    }

    static OverlayStringEditor()
    {
      TextBox.TextProperty.OverrideMetadata(typeof (OverlayStringEditor), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(OverlayStringEditor.OnTextChangedCallback)));
    }

    private static void OnTextChangedCallback(object sender, DependencyPropertyChangedEventArgs args)
    {
      OverlayStringEditor overlayStringEditor = sender as OverlayStringEditor;
      if (overlayStringEditor == null)
        return;
      bool flag = !string.IsNullOrEmpty(overlayStringEditor.Text);
      if (flag == overlayStringEditor.HasText)
        return;
      overlayStringEditor.SetValue(OverlayStringEditor.HasTextPropertyKey, (object) (bool) (flag ? true : false));
    }
  }
}
