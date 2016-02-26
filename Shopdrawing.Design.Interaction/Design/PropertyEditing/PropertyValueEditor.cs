// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.PropertyEditing.PropertyValueEditor
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
  public class PropertyValueEditor
  {
    private DataTemplate _inlineEditorTemplate;

    public DataTemplate InlineEditorTemplate
    {
      get
      {
        return this._inlineEditorTemplate;
      }
      set
      {
        this._inlineEditorTemplate = value;
      }
    }

    public PropertyValueEditor()
    {
    }

    public PropertyValueEditor(DataTemplate inlineEditorTemplate)
    {
      this._inlineEditorTemplate = inlineEditorTemplate;
    }

    internal virtual DataTemplate GetPropertyValueEditor(PropertyContainerEditMode mode)
    {
      if (mode != PropertyContainerEditMode.Inline)
        return (DataTemplate) null;
      return this._inlineEditorTemplate;
    }

    public static EditorAttribute CreateEditorAttribute(PropertyValueEditor editor)
    {
      if (editor == null)
        throw new ArgumentNullException("editor");
      return PropertyValueEditor.CreateEditorAttribute(editor.GetType());
    }

    public static EditorAttribute CreateEditorAttribute(Type propertyValueEditorType)
    {
      if (propertyValueEditorType == null)
        throw new ArgumentNullException("propertyValueEditorType");
      if (!typeof (PropertyValueEditor).IsAssignableFrom(propertyValueEditorType))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_ArgIncorrectType, new object[2]
        {
          (object) "propertyValueEditorType",
          (object) typeof (PropertyValueEditor).Name
        }));
      return new EditorAttribute(propertyValueEditorType, typeof (PropertyValueEditor));
    }
  }
}
