// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.UserInterface.ExpandoListItemModel
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.Configuration;
using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;

namespace Microsoft.Expression.Framework.UserInterface
{
  [ContentProperty("Content")]
  public class ExpandoListItemModel : INotifyPropertyChanged, IConfigurationSite
  {
    private bool isExpanded = true;
    private string title = string.Empty;
    private string identifier = string.Empty;
    private GridLength size = new GridLength(1.0, GridUnitType.Star);
    private bool isForcedInvisible;
    private object content;

    public bool IsExpanded
    {
      get
      {
        return this.isExpanded;
      }
      set
      {
        this.isExpanded = value;
        this.OnPropertyChanged("IsExpanded");
        this.OnPropertyChanged("AdjustedSize");
      }
    }

    public bool IsForcedInvisible
    {
      get
      {
        return this.isForcedInvisible;
      }
      set
      {
        if (this.isForcedInvisible == value)
          return;
        this.isForcedInvisible = value;
        this.OnPropertyChanged("IsForcedInvisible");
      }
    }

    public string Identifier
    {
      get
      {
        return this.identifier;
      }
      set
      {
        this.identifier = value;
        this.OnPropertyChanged("Identifier");
      }
    }

    public string Title
    {
      get
      {
        return this.title;
      }
      set
      {
        this.title = value;
        this.OnPropertyChanged("Title");
      }
    }

    public object Content
    {
      get
      {
        return this.content;
      }
      set
      {
        this.content = value;
        this.OnPropertyChanged("Content");
      }
    }

    public GridLength Size
    {
      get
      {
        return this.size;
      }
      set
      {
        this.size = value;
        this.OnPropertyChanged("Size");
        this.OnPropertyChanged("AdjustedSize");
      }
    }

    public GridLength AdjustedSize
    {
      get
      {
        if (!this.isExpanded)
          return GridLength.Auto;
        return this.size;
      }
      set
      {
        if (!this.isExpanded)
          return;
        this.Size = value;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public ExpandoListItemModel(bool isExpanded, string identifier, string title, object content, GridLength size)
    {
      this.isExpanded = isExpanded;
      this.identifier = identifier;
      this.title = title;
      this.content = content;
      this.size = size;
    }

    public ExpandoListItemModel()
    {
    }

    private void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    public void ReadFromConfiguration(IConfigurationObject configurationObject)
    {
      this.Size = (GridLength) configurationObject.GetProperty("Size", (object) GridLength.Auto);
      this.IsExpanded = (bool) configurationObject.GetProperty("IsExpanded", (object) true);
    }

    public void WriteToConfiguration(IConfigurationObject configurationObject)
    {
        configurationObject.SetProperty("Size", this.Size, GridLength.Auto);
        configurationObject.SetProperty("IsExpanded", this.IsExpanded, true);
    }
  }
}
