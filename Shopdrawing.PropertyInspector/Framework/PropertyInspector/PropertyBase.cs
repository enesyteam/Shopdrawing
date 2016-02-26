// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.PropertyInspector.PropertyBase
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.ValueEditors;
using Microsoft.Windows.Design.PropertyEditing;
using System.ComponentModel;

namespace Microsoft.Expression.Framework.PropertyInspector
{
  public abstract class PropertyBase : PropertyEntry
  {
    private TypeConverter typeConverter;
    private IValueValidator valueValidator;
    private string propertyName;
    private bool associated;
    private bool removeFromCategoryWhenDisassociated;

    public abstract ValueEditorParameters ValueEditorParameters { get; }

    public virtual string PropertyName
    {
      get
      {
        return this.propertyName;
      }
    }

    public virtual TypeConverter Converter
    {
      get
      {
        return this.typeConverter;
      }
    }

    public IValueValidator Validator
    {
      get
      {
        return this.valueValidator;
      }
      set
      {
        if (this.valueValidator == value)
          return;
        this.valueValidator = value;
        this.OnPropertyChanged("Validator");
      }
    }

    public bool Associated
    {
      get
      {
        return this.associated;
      }
      set
      {
        if (this.associated == value)
          return;
        this.associated = value;
        this.OnPropertyChanged("Associated");
      }
    }

    public bool RemoveFromCategoryWhenDisassociated
    {
      get
      {
        return this.removeFromCategoryWhenDisassociated;
      }
      set
      {
        if (this.removeFromCategoryWhenDisassociated == value)
          return;
        this.removeFromCategoryWhenDisassociated = value;
        this.OnPropertyChanged("RemoveFromCategoryWhenDisassociated");
      }
    }

    protected PropertyBase()
    {
      base.\u002Ector();
    }

    protected PropertyBase(PropertyValue parentValue)
    {
      base.\u002Ector(parentValue);
    }

    public abstract void SetValue(object value);

    public abstract void ClearValue();

    public abstract object GetValue();

    public abstract void AddValue(object value);

    public abstract void InsertValue(int index, object value);

    public abstract void RemoveValueAt(int index);

    public abstract void OnRemoveFromCategory();

    public virtual void Recache()
    {
    }

    protected void SetName(string propertyName)
    {
      this.propertyName = propertyName;
    }
  }
}
