// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.UserInterface.ShellOptionsModel
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System.ComponentModel;

namespace Microsoft.Expression.Framework.UserInterface
{
  internal sealed class ShellOptionsModel : INotifyPropertyChanged
  {
    private IWindowService windowService;
    private string previousActiveTheme;

    public ITheme ActiveTheme
    {
      get
      {
        for (int index = 0; index < this.windowService.Themes.Count; ++index)
        {
          if (this.windowService.Themes[index].Name == this.windowService.ActiveTheme)
            return this.windowService.Themes[index];
        }
        return (ITheme) null;
      }
      set
      {
        this.windowService.ActiveTheme = value.Name;
        this.OnPropertyChanged("ActiveTheme");
      }
    }

    public IThemeCollection Themes
    {
      get
      {
        return this.windowService.Themes;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public ShellOptionsModel(IWindowService windowService)
    {
      this.windowService = windowService;
      this.previousActiveTheme = this.windowService.ActiveTheme;
    }

    public void Cancel()
    {
      this.windowService.ActiveTheme = this.previousActiveTheme;
    }

    private void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
