// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.PropertyInspector.ObjectSet
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.Expression.Framework.PropertyInspector
{
  public abstract class ObjectSet : INotifyPropertyChanged
  {
    public abstract Type ObjectType { get; }

    public abstract int Count { get; }

    public abstract bool IsHomogenous { get; }

    public abstract string ObjectsName { get; }

    public abstract ObjectSet.PropertiesIterator Properties { get; }

    public event EventHandler SetChanged;

    public event EventHandler SetPropertiesChanged;

    public event EventHandler SetCategoriesChanged;

    public event EventHandler SetPropertyValuesChanged;

    public event PropertyChangedEventHandler PropertyChanged;

    public abstract bool IsCategoryPresent(CategoryEntry category);

    public abstract bool IsPropertyPresent(PropertyEntry property);

    protected void OnSetChanged()
    {
      if (this.SetChanged == null)
        return;
      this.SetChanged((object) this, EventArgs.Empty);
    }

    protected void OnSetPropertiesChanged()
    {
      if (this.SetPropertiesChanged == null)
        return;
      this.SetPropertiesChanged((object) this, EventArgs.Empty);
    }

    protected void OnSetCategoriesChanged()
    {
      if (this.SetCategoriesChanged == null)
        return;
      this.SetCategoriesChanged((object) this, EventArgs.Empty);
    }

    protected void OnSetPropertyValuesChanged()
    {
      if (this.SetPropertyValuesChanged == null)
        return;
      this.SetPropertyValuesChanged((object) this, EventArgs.Empty);
    }

    protected void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    public abstract class PropertiesIterator
    {
      public abstract int Length { get; }

      public abstract PropertyEntry this[string propertyName] { get; }

      public abstract IEnumerator<PropertyEntry> GetEnumerator();
    }
  }
}
