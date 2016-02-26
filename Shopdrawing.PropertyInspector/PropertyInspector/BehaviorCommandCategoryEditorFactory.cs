// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.BehaviorCommandCategoryEditorFactory
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Windows.Design.PropertyEditing;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public sealed class BehaviorCommandCategoryEditorFactory : CategoryEditorFactory
  {
    public override CategoryEditorSet GetCategoryEditors(ITypeId selectedType, SceneNodeCategory category)
    {
      CategoryEditorSet categoryEditorSet = new CategoryEditorSet();
      if (ProjectNeutralTypes.Behavior.IsAssignableFrom(selectedType))
      {
        using (IEnumerator<PropertyEntry> enumerator = ((CategoryEntry) category).get_Properties().GetEnumerator())
        {
          while (((IEnumerator) enumerator).MoveNext())
          {
            SceneNodeProperty sceneNodeProperty = (SceneNodeProperty) enumerator.Current;
            if (PlatformTypes.ICommand.IsAssignableFrom((ITypeId) sceneNodeProperty.PropertyTypeId))
            {
              categoryEditorSet.AddCategoryEditor((CategoryEditor) new BehaviorCommandCategoryEditor(category.get_CategoryName()), category.get_CategoryName());
              categoryEditorSet.AddCategoryEditor((CategoryEditor) new BehaviorCommandAdvancedCategoryEditor(category.get_CategoryName()), category.get_CategoryName());
              break;
            }
          }
        }
      }
      return categoryEditorSet;
    }
  }
}
