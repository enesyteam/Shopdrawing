// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Interaction.CurrentDesignerView
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design;
using MS.Internal.Properties;
using System;

namespace Microsoft.Windows.Design.Interaction
{
  public sealed class CurrentDesignerView : ContextItem
  {
    private DesignerView _view;
    private bool _allowNullView;

    public DesignerView View
    {
      get
      {
        return this._view;
      }
    }

    public override Type ItemType
    {
      get
      {
        return typeof (CurrentDesignerView);
      }
    }

    public CurrentDesignerView()
    {
    }

    internal CurrentDesignerView(DesignerView view)
    {
      this._view = view;
      this._allowNullView = view == null;
    }

    protected override void OnItemChanged(EditingContext context, ContextItem previousItem)
    {
      if (!this._allowNullView && ((CurrentDesignerView) previousItem)._view != null && this._view == null)
        throw new InvalidOperationException(Resources.Error_ContextHasView);
      base.OnItemChanged(context, previousItem);
    }
  }
}
