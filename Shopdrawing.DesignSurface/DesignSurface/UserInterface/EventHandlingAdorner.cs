// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.EventHandlingAdorner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  public abstract class EventHandlingAdorner : Adorner
  {
    protected EventHandlingAdorner(AdornerSet adornerSet)
      : base(adornerSet)
    {
      this.DesignerContext.ActiveView.AdornerLayer.AdornerPropertyChanged += new EventHandler<AdornerPropertyChangedEventArgs>(this.OnAdornerPropertyChanged);
    }

    private void Unhook()
    {
      if (this.DesignerContext == null)
        return;
      this.DesignerContext.ActiveView.AdornerLayer.AdornerPropertyChanged -= new EventHandler<AdornerPropertyChangedEventArgs>(this.OnAdornerPropertyChanged);
    }

    private void OnAdornerPropertyChanged(object sender, AdornerPropertyChangedEventArgs eventArgs)
    {
      if (!this.AdornerSet.ElementSet.IsAttached)
        this.Unhook();
      else
        this.HandleAdornerLayerEvent(eventArgs);
    }

    public override void OnRemove()
    {
      this.Unhook();
    }

    protected abstract void HandleAdornerLayerEvent(AdornerPropertyChangedEventArgs eventArgs);
  }
}
