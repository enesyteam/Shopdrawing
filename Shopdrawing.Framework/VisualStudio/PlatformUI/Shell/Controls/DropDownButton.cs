// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.Controls.DropDownButton
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
  public class DropDownButton : GlyphButton
  {
    public DropDownButton()
    {
      this.ContextMenuOpening += new ContextMenuEventHandler(this.OnContextMenuOpening);
      this.Click += new RoutedEventHandler(this.OnClick);
      this.SetResourceReference(FrameworkElement.StyleProperty, (object) typeof (GlyphButton));
    }

    private void OnClick(object sender, RoutedEventArgs e)
    {
      if (this.ContextMenu == null || this.ContextMenu.Items.Count == 0 && (this.ContextMenu.TemplatedParent == null || !(this.ContextMenu.TemplatedParent as ItemsControl).HasItems))
        return;
      CollectionView collectionView = this.ContextMenu.ItemsSource as CollectionView;
      if (collectionView != null)
        collectionView.Refresh();
      this.ContextMenu.Placement = PlacementMode.Bottom;
      this.ContextMenu.PlacementTarget = (UIElement) this;
      this.ContextMenu.IsOpen = true;
    }

    private void OnContextMenuOpening(object sender, ContextMenuEventArgs e)
    {
      this.ContextMenu.IsOpen = false;
      e.Handled = true;
    }
  }
}
