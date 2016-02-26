// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.BindingProxy`1
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.Data;
using System.ComponentModel;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class BindingProxy<T> : IDataBindingProxy<T>, INotifyPropertyChanged
  {
    private T storedValue;

    public T Value
    {
      get
      {
        return this.storedValue;
      }
      set
      {
        if (object.Equals((object) this.storedValue, (object) value))
          return;
        this.storedValue = value;
        if (this.PropertyChanged == null)
          return;
        this.PropertyChanged((object) this, new PropertyChangedEventArgs("Value"));
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;
  }
}
