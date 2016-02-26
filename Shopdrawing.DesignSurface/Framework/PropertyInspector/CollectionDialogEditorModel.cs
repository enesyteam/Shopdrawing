// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.PropertyInspector.CollectionDialogEditorModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Expression.Framework.PropertyInspector
{
  public abstract class CollectionDialogEditorModel : IPropertyInspector
  {
    private CollectionDialogEditor host;
    private IList<CategoryBase> categories;

    public abstract bool CanMoveItem { get; }

    public abstract bool CanRemoveItem { get; }

    public virtual CollectionDialogEditor Host
    {
      get
      {
        return this.host;
      }
      set
      {
        this.host = value;
      }
    }

    public abstract IMessageLoggingService MessageLoggingService { get; }

    public IList<CategoryBase> Categories
    {
      get
      {
        return this.categories;
      }
    }

    protected CollectionDialogEditorModel()
    {
      this.categories = (IList<CategoryBase>) new ObservableCollection<CategoryBase>();
    }

    public abstract void PropertyValueChanged();

    public abstract CategoryBase FindOrCreateCategory(string categoryName, Type selectedType);

    public abstract object CreateType(Type type);

    public abstract IList<NewItemTypesAttribute> GetNewItemTypesAttributes();

    public virtual void OnRebuildComplete(PropertyValue activePropertyValue)
    {
    }

    public bool IsCategoryExpanded(string categoryName)
    {
      return true;
    }

    public virtual void UpdateTransaction()
    {
    }
  }
}
