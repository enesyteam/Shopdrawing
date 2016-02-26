// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.PropertyEditing.DialogPropertyValueEditor
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using System.Windows;

namespace Microsoft.Windows.Design.PropertyEditing
{
  public class DialogPropertyValueEditor : PropertyValueEditor
  {
    private DataTemplate _dialogEditorTemplate;

    public DataTemplate DialogEditorTemplate
    {
      get
      {
        return this._dialogEditorTemplate;
      }
      set
      {
        this._dialogEditorTemplate = value;
      }
    }

    public DialogPropertyValueEditor()
      : this((DataTemplate) null, (DataTemplate) null)
    {
    }

    public DialogPropertyValueEditor(DataTemplate dialogEditorTemplate, DataTemplate inlineEditorTemplate)
      : base(inlineEditorTemplate)
    {
      this._dialogEditorTemplate = dialogEditorTemplate;
    }

    public virtual void ShowDialog(PropertyValue propertyValue, IInputElement commandSource)
    {
    }

    internal override DataTemplate GetPropertyValueEditor(PropertyContainerEditMode mode)
    {
      DataTemplate propertyValueEditor = base.GetPropertyValueEditor(mode);
      if (propertyValueEditor != null)
        return propertyValueEditor;
      if (mode != PropertyContainerEditMode.Dialog)
        return (DataTemplate) null;
      return this._dialogEditorTemplate;
    }
  }
}
