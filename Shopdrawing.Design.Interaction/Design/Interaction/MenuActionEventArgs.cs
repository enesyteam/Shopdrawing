// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Interaction.MenuActionEventArgs
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design;
using System;

namespace Microsoft.Windows.Design.Interaction
{
  public class MenuActionEventArgs : EventArgs
  {
    private EditingContext _context;
    private Selection _selection;

    public EditingContext Context
    {
      get
      {
        return this._context;
      }
    }

    public Selection Selection
    {
      get
      {
        return this._selection;
      }
    }

    public MenuActionEventArgs(EditingContext context)
    {
      if (context == null)
        throw new ArgumentNullException("context");
      this._context = context;
      this._selection = this._context.Items.GetValue<Selection>();
    }
  }
}
