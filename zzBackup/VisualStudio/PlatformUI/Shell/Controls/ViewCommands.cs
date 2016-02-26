// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.Controls.ViewCommands
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System.Windows.Input;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
  public static class ViewCommands
  {
    public static readonly RoutedCommand HideViewCommand = new RoutedCommand("HideView", typeof (ViewCommands));
    public static readonly RoutedCommand HideViewInvertPreferenceCommand = new RoutedCommand("HideViewInvertPreference", typeof (ViewCommands));
    public static readonly RoutedCommand AutoHideViewCommand = new RoutedCommand("AutoHideView", typeof (ViewCommands));
    public static readonly RoutedCommand AutoHideViewInvertPreferenceCommand = new RoutedCommand("AutoHideViewInvertPreference", typeof (ViewCommands));
    public static readonly RoutedCommand ShowAutoHiddenView = new RoutedCommand("ShowAutoHiddenView", typeof (ViewCommands));
    public static readonly RoutedCommand ShowAndActivateAutoHiddenView = new RoutedCommand("ShowAndActivateAutoHiddenView", typeof (ViewCommands));
    public static readonly RoutedCommand ToggleDocked = new RoutedCommand("ToggleDocked", typeof (ViewCommands));
    public static readonly RoutedCommand NewHorizontalTabGroupCommand = new RoutedCommand("NewHorizontalTabGroup", typeof (ViewCommands));
    public static readonly RoutedCommand NewVerticalTabGroupCommand = new RoutedCommand("NewVerticalTabGroup", typeof (ViewCommands));
    public static readonly RoutedCommand MoveToNextTabGroupCommand = new RoutedCommand("MoveToNextTabGroup", typeof (ViewCommands));
    public static readonly RoutedCommand MoveToPreviousTabGroupCommand = new RoutedCommand("MoveToPreviousTabGroup", typeof (ViewCommands));
    public static readonly RoutedCommand ActivateDocumentViewCommand = new RoutedCommand("ActivateDocumentView", typeof (ViewCommands));
  }
}
