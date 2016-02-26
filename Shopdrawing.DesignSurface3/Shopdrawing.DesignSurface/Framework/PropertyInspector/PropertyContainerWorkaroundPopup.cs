// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.PropertyInspector.PropertyContainerWorkaroundPopup
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.Controls;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Microsoft.Expression.Framework.PropertyInspector
{
  public class PropertyContainerWorkaroundPopup : WorkaroundPopup
  {
    public static readonly RoutedCommand OnBeginExtendedEdit = new RoutedCommand("OnBeginExtendedEdit", typeof (PropertyContainerWorkaroundPopup));
    public static readonly RoutedCommand OnEndExtendedEdit = new RoutedCommand("OnEndExtendedEdit", typeof (PropertyContainerWorkaroundPopup));

    public static CustomPopupPlacementCallback RightAlignedPopupPlacement
    {
      get
      {
        return new CustomPopupPlacementCallback(PropertyContainerWorkaroundPopup.RightAlignedPopupPlacementCallback);
      }
    }

    protected override void OnOpened(EventArgs e)
    {
      PropertyContainer propertyContainer = (PropertyContainer) this.GetValue(PropertyContainer.OwningPropertyContainerProperty);
      PropertyContainerWorkaroundPopup.OnBeginExtendedEdit.Execute((object) this, (IInputElement) propertyContainer);
      base.OnOpened(e);
    }

    protected override void OnClosed(EventArgs e)
    {
      base.OnClosed(e);
      PropertyContainer propertyContainer = (PropertyContainer) this.GetValue(PropertyContainer.OwningPropertyContainerProperty);
      if (propertyContainer != null && propertyContainer.ActiveEditMode == PropertyContainerEditMode.ExtendedPopup)
      {
        DependencyObject descendant = Mouse.Captured as DependencyObject;
        if (descendant != null && propertyContainer.IsAncestorOf(descendant))
          Mouse.Capture((IInputElement) null);
        propertyContainer.ActiveEditMode = PropertyContainerEditMode.Inline;
      }
      PropertyContainerWorkaroundPopup.OnEndExtendedEdit.Execute((object) this, (IInputElement) propertyContainer);
    }

    public static CustomPopupPlacement[] RightAlignedPopupPlacementCallback(Size popupSize, Size targetSize, Point offset)
    {
      return new CustomPopupPlacement[1]
      {
        new CustomPopupPlacement(new Point(targetSize.Width - popupSize.Width, targetSize.Height), PopupPrimaryAxis.Horizontal)
      };
    }
  }
}
