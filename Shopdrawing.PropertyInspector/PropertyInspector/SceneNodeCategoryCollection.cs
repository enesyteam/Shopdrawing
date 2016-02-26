// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.SceneNodeCategoryCollection
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.Data;
using System;
using System.Collections.ObjectModel;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class SceneNodeCategoryCollection : ObservableCollection<SceneNodeCategory>
  {
    internal int FindCategoryForName(string categoryName)
    {
      return OrderedListExtensions.GenericBinarySearch<SceneNodeCategory, string>(this.Items, categoryName, (Func<string, SceneNodeCategory, int>) ((name, category) => name.CompareTo(category.get_CategoryName())));
    }
  }
}
