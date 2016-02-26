// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.PropertyEditing.ExtendedPropertyValueEditor
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using System.Windows;

namespace Microsoft.Windows.Design.PropertyEditing
{
  public class ExtendedPropertyValueEditor : PropertyValueEditor
  {
    private DataTemplate _extendedEditorTemplate;

    public DataTemplate ExtendedEditorTemplate
    {
      get
      {
        return this._extendedEditorTemplate;
      }
      set
      {
        this._extendedEditorTemplate = value;
      }
    }

    public ExtendedPropertyValueEditor()
      : this((DataTemplate) null, (DataTemplate) null)
    {
    }

    public ExtendedPropertyValueEditor(DataTemplate extendedEditorTemplate, DataTemplate inlineEditorTemplate)
      : base(inlineEditorTemplate)
    {
      this._extendedEditorTemplate = extendedEditorTemplate;
    }

    internal override DataTemplate GetPropertyValueEditor(PropertyContainerEditMode mode)
    {
      DataTemplate propertyValueEditor = base.GetPropertyValueEditor(mode);
      if (propertyValueEditor != null)
        return propertyValueEditor;
      if (mode != PropertyContainerEditMode.ExtendedPinned && mode != PropertyContainerEditMode.ExtendedPopup)
        return (DataTemplate) null;
      return this._extendedEditorTemplate;
    }
  }
}
