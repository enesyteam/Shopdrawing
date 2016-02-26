// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Controls.FocusDenyingTabControl
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.UserInterface;
using System.Windows.Controls;
using System.Windows.Input;

namespace Microsoft.Expression.Framework.Controls
{
  public class FocusDenyingTabControl : TabControl
  {
    protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
    {
      base.OnGotKeyboardFocus(e);
      if (!(e.NewFocus is TabItem))
        return;
      FocusScopeManager.Instance.ReturnFocus();
    }
  }
}
