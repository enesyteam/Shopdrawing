// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.UserInterface.IWindowService
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.Workspaces.Extension;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Microsoft.Expression.Framework.UserInterface
{
  public interface IWindowService
  {
    PaletteRegistry PaletteRegistry { get; }

    double WorkspaceZoom { get; set; }

    string ActiveTheme { get; set; }

    IThemeCollection Themes { get; }

    Window MainWindow { get; }

    string Title { get; set; }

    bool IsEnabled { get; set; }

    bool IsVisible { get; set; }

    event CancelEventHandler Closing;

    event EventHandler ClosingCanceled;

    event EventHandler Closed;

    event EventHandler ThemeChanged;

    event EventHandler StateChanged;

    event EventHandler Initialized;

    PaletteRegistryEntry RegisterPalette(string identifier, FrameworkElement content, string caption);

    PaletteRegistryEntry RegisterPalette(string identifier, FrameworkElement content, string caption, KeyBinding keyBinding);

    PaletteRegistryEntry RegisterPalette(string identifier, FrameworkElement content, string caption, KeyBinding keyBinding, ExpressionViewProperties viewProperties);

    void UnregisterPalette(string paletteName);

    void AddResourceDictionary(ResourceDictionary dictionary);

    void RemoveResourceDictionary(ResourceDictionary dictionary);

    void ReturnFocus();

    IDisposable SuppressViewActivationOnGotFocus();

    void Focus();

    void UpdateActiveTheme();

    void ShowSwitchToDialog();
  }
}
