// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Assets.UserThemeCommandHelper
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.UserInterface;
using System.Windows.Controls;

namespace Microsoft.Expression.DesignSurface.Tools.Assets
{
  internal static class UserThemeCommandHelper
  {
    internal static ContextMenu GetThemeContextMenu(AssetLibrary library, AssetCategory category)
    {
      ICommandBar commandBar = library.DesignerContext.CommandBarService.CommandBars.AddContextMenu("Assets_UserTheme");
      string lastStep = category.Path.LastStep;
      if (library.FindUserThemeProvider(lastStep) == null)
        return (ContextMenu) null;
      ICommandService commandService = library.DesignerContext.CommandService;
      commandService.SetCommandProperty("Assets_SetAsDefaultTheme", "Provider", (object) lastStep);
      commandService.SetCommandProperty("Assets_ResetSystemTheme", "Provider", (object) lastStep);
      commandService.SetCommandProperty("Assets_RestoreToDefaultContent", "Provider", (object) lastStep);
      commandBar.Items.AddCheckBox("Assets_SetAsDefaultTheme", StringTable.SetAsDefaultTheme);
      commandBar.Items.AddButton("Assets_ResetSystemTheme", StringTable.ResetSystemTheme);
      commandBar.Items.AddButton("Assets_RestoreToDefaultContent", StringTable.RestoreThemeContent);
      return (ContextMenu) commandBar;
    }
  }
}
