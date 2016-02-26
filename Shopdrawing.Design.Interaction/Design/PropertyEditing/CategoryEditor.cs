// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.PropertyEditing.CategoryEditor
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using MS.Internal.Properties;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;

namespace Microsoft.Windows.Design.PropertyEditing
{
  public abstract class CategoryEditor
  {
    public abstract string TargetCategory { get; }

    public abstract DataTemplate EditorTemplate { get; }

    public abstract bool ConsumesProperty(PropertyEntry propertyEntry);

    public abstract object GetImage(Size desiredSize);

    public static EditorAttribute CreateEditorAttribute(CategoryEditor editor)
    {
      if (editor == null)
        throw new ArgumentNullException("editor");
      return CategoryEditor.CreateEditorAttribute(editor.GetType());
    }

    public static EditorAttribute CreateEditorAttribute(Type categoryEditorType)
    {
      if (categoryEditorType == null)
        throw new ArgumentNullException("categoryEditorType");
      if (!typeof (CategoryEditor).IsAssignableFrom(categoryEditorType))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_ArgIncorrectType, new object[2]
        {
           "categoryEditorType",
          typeof (CategoryEditor).Name
        }));
      return new EditorAttribute(categoryEditorType, categoryEditorType);
    }
  }
}
