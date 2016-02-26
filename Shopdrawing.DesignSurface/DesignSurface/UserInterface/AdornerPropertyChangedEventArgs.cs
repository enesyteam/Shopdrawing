// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.AdornerPropertyChangedEventArgs
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  public class AdornerPropertyChangedEventArgs : EventArgs
  {
    private string propertyName;
    private object value;

    public string PropertyName
    {
      get
      {
        return this.propertyName;
      }
    }

    public object Value
    {
      get
      {
        return this.value;
      }
    }

    public AdornerPropertyChangedEventArgs(string propertyName, object value)
    {
      this.propertyName = propertyName;
      this.value = value;
    }
  }
}
