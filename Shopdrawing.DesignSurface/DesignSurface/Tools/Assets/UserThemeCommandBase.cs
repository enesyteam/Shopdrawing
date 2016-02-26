// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Assets.UserThemeCommandBase
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.Commands;

namespace Microsoft.Expression.DesignSurface.Tools.Assets
{
  internal abstract class UserThemeCommandBase : CheckCommandBase
  {
    public AssetLibrary Library { get; private set; }

    public string ProviderName { get; private set; }

    public IUserThemeProvider Provider
    {
      get
      {
        return this.Library.FindUserThemeProvider(this.ProviderName);
      }
    }

    public UserThemeCommandBase(IAssetLibrary library)
    {
      this.Library = (AssetLibrary) library;
    }

    public override object GetProperty(string propertyName)
    {
      if (propertyName == "Provider")
        return (object) this.ProviderName;
      return base.GetProperty(propertyName);
    }

    public override void SetProperty(string propertyName, object propertyValue)
    {
      if (propertyName == "Provider")
        this.ProviderName = (string) propertyValue;
      base.SetProperty(propertyName, propertyValue);
    }

    protected override bool IsChecked()
    {
      return false;
    }

    protected override void OnCheckedChanged(bool isChecked)
    {
    }
  }
}
