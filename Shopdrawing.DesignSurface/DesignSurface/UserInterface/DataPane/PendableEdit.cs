// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.PendableEdit
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.ValueEditors;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public static class PendableEdit
  {
    public static readonly DependencyProperty IsPendingEditProperty = DependencyProperty.RegisterAttached("IsPendingEdit", typeof (bool), typeof (PendableEdit), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, new PropertyChangedCallback(PendableEdit.OnIsPendingEditChanged)));

    public static bool GetIsPendingEdit(StringEditor editor)
    {
      return (bool) editor.GetValue(PendableEdit.IsPendingEditProperty);
    }

    public static void SetIsPendingEdit(StringEditor editor, bool value)
    {
      editor.SetValue(PendableEdit.IsPendingEditProperty, (object) (bool) (value ? true : false));
    }

    private static void OnIsPendingEditChanged(object sender, DependencyPropertyChangedEventArgs args)
    {
      StringEditor stringEditor = sender as StringEditor;
      if (stringEditor == null || !(bool) args.NewValue)
        return;
      stringEditor.SetValue(PendableEdit.IsPendingEditProperty, (object) false);
      stringEditor.IsEditing = true;
    }
  }
}
