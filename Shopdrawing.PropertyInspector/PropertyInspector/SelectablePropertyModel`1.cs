// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.SelectablePropertyModel`1
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.UserInterface;
using System;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class SelectablePropertyModel<T>
  {
    private T propertyModel;
    private bool isSelected;

    public T PropertyModel
    {
      get
      {
        return this.propertyModel;
      }
    }

    public bool IsSelected
    {
      get
      {
        return this.isSelected;
      }
    }

    public SelectablePropertyModel(T propertyModel, bool isSelected)
    {
      this.propertyModel = propertyModel;
      this.isSelected = isSelected;
    }

    public override bool Equals(object obj)
    {
      SelectablePropertyModel<T> selectablePropertyModel = obj as SelectablePropertyModel<T>;
      if (selectablePropertyModel != null && this.isSelected == selectablePropertyModel.isSelected)
        return this.propertyModel.Equals((object) selectablePropertyModel.propertyModel);
      return false;
    }

    public override int GetHashCode()
    {
      return this.isSelected.GetHashCode() ^ this.propertyModel.GetHashCode();
    }

    public int CompareTo(object obj)
    {
      if (obj == null)
        return 1;
      SelectablePropertyModel<T> selectablePropertyModel = obj as SelectablePropertyModel<T>;
      if (selectablePropertyModel != null)
      {
        BaseResourceModel baseResourceModel1 = this.propertyModel as BaseResourceModel;
        BaseResourceModel baseResourceModel2 = (object) selectablePropertyModel.propertyModel as BaseResourceModel;
        if (baseResourceModel1 != null && baseResourceModel2 != null)
          return baseResourceModel1.CompareTo((object) baseResourceModel2);
      }
      throw new ArgumentException(ExceptionStringTable.CompareToMustBeCalledWithASelectablePropertyModelTOrNull, "obj");
    }
  }
}
