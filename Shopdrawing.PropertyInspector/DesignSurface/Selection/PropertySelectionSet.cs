// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Selection.PropertySelectionSet
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Data;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.Selection
{
  public class PropertySelectionSet : SelectionSet<PropertyReference, OrderedList<PropertyReference>>
  {
    public override bool IsExclusive
    {
      get
      {
        return false;
      }
    }

    public PropertySelectionSet(SceneViewModel viewModel)
      : base(viewModel, (ISelectionSetNamingHelper) new PropertySelectionSet.PropertyNamingHelper(), (SelectionSet<PropertyReference, OrderedList<PropertyReference>>.IStorageProvider) new BasicSelectionSetStorageProvider<PropertyReference>((IComparer<PropertyReference>) new PropertyReference.Comparer()))
    {
    }

    private class PropertyNamingHelper : ISelectionSetNamingHelper
    {
      public string Name
      {
        get
        {
          return StringTable.UndoUnitPropertyName;
        }
      }

      public string GetUndoString(object obj)
      {
        PropertyReference propertyReference = obj as PropertyReference;
        if (propertyReference != null)
          return propertyReference.ShortPath;
        return "";
      }
    }
  }
}
