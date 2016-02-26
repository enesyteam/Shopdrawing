// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.NotifyingObject
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Expression.Framework
{
  public abstract class NotifyingObject : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    [Conditional("DEBUG")]
    public static void VerifyPropertyExists(object instance, string propertyName)
    {
    }

    [Conditional("DEBUG")]
    public static void VerifyPropertyExists(Type type, string propertyName)
    {
    }

    public bool AssignAndNotify<T>(ref T target, T value, params string[] propertyNames)
    {
      if (object.Equals((object) target, (object) value))
        return false;
      target = value;
      foreach (string propertyName in propertyNames)
        this.OnPropertyChanged(propertyName);
      return true;
    }
  }
}
