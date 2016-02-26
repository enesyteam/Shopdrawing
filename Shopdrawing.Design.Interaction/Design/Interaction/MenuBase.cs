// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Interaction.MenuBase
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design;
using System.ComponentModel;

namespace Microsoft.Windows.Design.Interaction
{
  public abstract class MenuBase : INotifyPropertyChanged
  {
    private string _name;
    private string _displayName;
    private EditingContext _context;

    public EditingContext Context
    {
      get
      {
        return this._context;
      }
      internal set
      {
        if (this._context == value)
          return;
        this._context = value;
        this.OnPropertyChanged("Context");
      }
    }

    public string Name
    {
      get
      {
        return this._name;
      }
      set
      {
        if (!(this._name != value))
          return;
        this._name = value;
        this.OnPropertyChanged("Name");
      }
    }

    public string DisplayName
    {
      get
      {
        return this._displayName;
      }
      set
      {
        if (!(this._displayName != value))
          return;
        this._displayName = value;
        this.OnPropertyChanged("DisplayName");
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
