// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.UserInterface.PaletteRegistryEntry
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.Workspaces.Extension;
using Microsoft.VisualStudio.PlatformUI.Shell;
using System.Windows;
using System.Windows.Input;

namespace Microsoft.Expression.Framework.UserInterface
{
  public class PaletteRegistryEntry
  {
    private PaletteRegistry paletteRegistry;

    public string Name { get; private set; }

    public string CommandName
    {
      get
      {
        return "View_Palette_" + this.Name;
      }
    }

    public FrameworkElement Content { get; private set; }

    public string MenuText { get; private set; }

    public string Caption { get; private set; }

    public KeyBinding KeyBinding { get; private set; }

    public ExpressionViewProperties ViewProperties { get; private set; }

    public bool IsForcedInvisible
    {
      get
      {
        ExpressionView palette = this.Palette;
        if (palette != null)
          return palette.IsForcedInvisible;
        return false;
      }
    }

    public bool IsVisible
    {
      get
      {
        View view = (View) this.Palette;
        if (view != null)
          return view.IsVisible;
        return false;
      }
      set
      {
        ExpressionView palette = this.Palette;
        if (palette == null)
          return;
        palette.IsDesiredVisible = value;
      }
    }

    public bool IsVisibleAndSelected
    {
      get
      {
        ExpressionView palette = this.Palette;
        if (palette != null && palette.IsVisible)
          return palette.IsSelected;
        return false;
      }
      set
      {
        ExpressionView palette = this.Palette;
        if (palette == null)
          return;
        palette.IsDesiredVisible = value;
        palette.IsSelected = value;
      }
    }

    private ExpressionView Palette
    {
      get
      {
        if (this.paletteRegistry.WorkspaceService.ActiveWorkspace == null)
          return (ExpressionView) null;
        return this.paletteRegistry.WorkspaceService.ActiveWorkspace.FindPalette(this.Name) as ExpressionView;
      }
    }

    public PaletteRegistryEntry(PaletteRegistry paletteRegistry, string name, FrameworkElement content, string caption, KeyBinding keyBinding, ExpressionViewProperties viewProperties)
    {
      this.paletteRegistry = paletteRegistry;
      this.Name = name;
      this.Content = content;
      this.MenuText = caption;
      this.Caption = caption.Replace("_", string.Empty);
      this.KeyBinding = keyBinding;
      this.ViewProperties = viewProperties;
    }
  }
}
