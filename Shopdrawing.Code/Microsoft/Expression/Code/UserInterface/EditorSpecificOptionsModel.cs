// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.UserInterface.EditorSpecificOptionsModel
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.Framework.Configuration;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Media;

namespace Microsoft.Expression.Code.UserInterface
{
  public class EditorSpecificOptionsModel : INotifyPropertyChanged
  {
    private const bool DefaultConvertTabsToSpace = false;
    private const int DefaultTabSize = 4;
    private const double DefaultFontSize = 10.0;
    private const bool DefaultWordWrap = false;
    private string displayName;
    private string prefix;
    private bool convertTabsToSpace;
    private int tabSize;
    private string fontFamily;
    private string unescapedFontFamily;
    private double fontSize;
    private bool wordWrap;

    public string DisplayName
    {
      get
      {
        return this.displayName;
      }
    }

    public bool ConvertTabsToSpace
    {
      get
      {
        return this.convertTabsToSpace;
      }
      set
      {
        this.convertTabsToSpace = value;
        this.OnPropertyChanged("ConvertTabsToSpace");
      }
    }

    public int TabSize
    {
      get
      {
        return this.tabSize;
      }
      set
      {
        this.tabSize = value;
        this.OnPropertyChanged("TabSize");
      }
    }

    public string FontFamily
    {
      get
      {
        return this.fontFamily;
      }
    }

    public string UnescapedFontFamily
    {
      get
      {
        return this.unescapedFontFamily;
      }
      set
      {
        if (!(this.unescapedFontFamily != value))
          return;
        this.unescapedFontFamily = value;
        this.fontFamily = FontFamilyHelper.EnsureFamilyName(value);
        this.OnPropertyChanged("FontFamily");
        this.OnPropertyChanged("UnescapedFontFamily");
      }
    }

    public double FontSize
    {
      get
      {
        return this.fontSize;
      }
      set
      {
        if (value >= 1.0 && value <= 100.0)
          this.fontSize = value;
        this.OnPropertyChanged("FontSize");
      }
    }

    public bool WordWrap
    {
      get
      {
        return this.wordWrap;
      }
      set
      {
        this.wordWrap = value;
        this.OnPropertyChanged("WordWrap");
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public EditorSpecificOptionsModel(string displayName, string prefix)
    {
      this.displayName = displayName;
      this.prefix = prefix;
    }

    public void Load(IConfigurationObject value)
    {
      this.ConvertTabsToSpace = (bool) value.GetProperty(this.prefix + "ConvertTabsToSpace", (object) false);
      this.TabSize = (int) value.GetProperty(this.prefix + "TabSize", (object) 4);
      this.UnescapedFontFamily = (string) value.GetProperty(this.prefix + "FontFamily", (object) EditorSpecificOptionsModel.GetDefaultFont());
      this.FontSize = (double) value.GetProperty(this.prefix + "FontSize", (object) 10.0);
      this.WordWrap = (bool) value.GetProperty(this.prefix + "WordWrap", (object) false);
    }

    public void Save(IConfigurationObject value)
    {
        value.SetProperty(string.Concat(this.prefix, "ConvertTabsToSpace"), this.ConvertTabsToSpace, false);
        value.SetProperty(string.Concat(this.prefix, "TabSize"), this.TabSize, 4);
        value.SetProperty(string.Concat(this.prefix, "FontFamily"), this.UnescapedFontFamily, EditorSpecificOptionsModel.GetDefaultFont());
        value.SetProperty(string.Concat(this.prefix, "FontSize"), this.FontSize, 10);
        value.SetProperty(string.Concat(this.prefix, "WordWrap"), this.WordWrap, false);
    }

    internal static string GetDefaultFont()
    {
      switch (CultureInfo.CurrentCulture.ThreeLetterWindowsLanguageName)
      {
        case "ENU":
          return Fonts.SystemFontFamilies.Contains(new System.Windows.Media.FontFamily("Consolas")) ? "Consolas" : "Courier New";
        case "JPN":
          return "MS Gothic";
        case "KOR":
          return "Dotum Che";
        case "CHS":
          return "NSimSun";
        case "CHT":
          return "MingLiu";
        default:
          return "Consolas";
      }
    }

    private void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
