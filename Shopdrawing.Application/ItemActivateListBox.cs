// Decompiled with JetBrains decompiler
// Type: Shopdrawing.App.ItemActivateListBox
// Assembly: Shopdrawing.App, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: DDD2F1CF-BB6D-4068-A4D9-DDBDD16D6739
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Shopdrawing.App.dll

using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace Shopdrawing.App
{
  public class ItemActivateListBox : ListBox
  {
    public event EventHandler ItemActivate;

    protected override void OnSelectionChanged(SelectionChangedEventArgs e)
    {
      base.OnSelectionChanged(e);
      if (Mouse.LeftButton != MouseButtonState.Pressed)
        return;
      this.OnItemActivate(EventArgs.Empty);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
      if (Keyboard.Modifiers == ModifierKeys.None && (e.Key == Key.Space || e.Key == Key.Return))
      {
        e.Handled = true;
        this.OnItemActivate(EventArgs.Empty);
      }
      else
        base.OnKeyDown(e);
    }

    protected virtual void OnItemActivate(EventArgs e)
    {
      if (this.ItemActivate == null)
        return;
      this.ItemActivate((object) this, e);
    }
  }
}
