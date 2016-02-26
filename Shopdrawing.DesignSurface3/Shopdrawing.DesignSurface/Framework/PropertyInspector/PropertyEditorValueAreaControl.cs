// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.PropertyInspector.PropertyEditorValueAreaControl
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Windows.Design.PropertyEditing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Microsoft.Expression.Framework.PropertyInspector
{
  public sealed class PropertyEditorValueAreaControl : ContentPresenter
  {
    public PropertyEditorValueAreaControl()
    {
      this.SetBinding(ContentPresenter.ContentProperty, (BindingBase) new Binding()
      {
        RelativeSource = RelativeSource.Self,
        Path = new PropertyPath("(0).(1).PropertyValue", (object[]) new DependencyProperty[2]
        {
          PropertyContainer.OwningPropertyContainerProperty,
          PropertyContainer.PropertyEntryProperty
        })
      });
    }
  }
}
