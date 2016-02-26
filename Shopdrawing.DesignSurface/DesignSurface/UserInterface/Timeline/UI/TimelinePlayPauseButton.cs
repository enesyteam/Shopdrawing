// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI.TimelinePlayPauseButton
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI
{
  public class TimelinePlayPauseButton : Button
  {
    protected override void OnLostMouseCapture(MouseEventArgs e)
    {
      KeyboardFocusChangedEventArgs e1 = new KeyboardFocusChangedEventArgs(Keyboard.PrimaryDevice, 0, (IInputElement) this, (IInputElement) this);
      e1.RoutedEvent = Keyboard.LostKeyboardFocusEvent;
      e1.Source = (object) this;
      this.OnLostKeyboardFocus(e1);
    }
  }
}
