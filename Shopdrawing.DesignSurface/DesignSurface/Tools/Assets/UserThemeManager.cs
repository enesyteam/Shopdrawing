// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Assets.UserThemeManager
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.Project;

namespace Microsoft.Expression.DesignSurface.Tools.Assets
{
  internal class UserThemeManager : FolderBasedThemeManager
  {
    private static readonly string ThemeFolderName = "AppThemes";

    public override string ThemeFolder
    {
      get
      {
        return TemplateManager.TranslatedFolder(UserThemeManager.ThemeFolderName);
      }
    }

    public override string CurrentTheme
    {
      get
      {
        return string.Empty;
      }
    }

    public UserThemeManager(IPlatform platform)
      : base(platform)
    {
    }
  }
}
