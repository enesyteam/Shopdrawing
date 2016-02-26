// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.PropertyLookup
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.PropertyInspector;
using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  [MarkupExtensionReturnType(typeof (BindingExpression))]
  public class PropertyLookup : MarkupExtension
  {
    private string propertyName;

    public PropertyLookup(string propertyName)
    {
      this.propertyName = propertyName;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      Binding binding = new Binding();
      string path = "(0).SceneNodePropertyLookup[" + this.propertyName + "]";
      binding.Path = new PropertyPath(path, new object[1]
      {
        (object) PropertyInspectorHelper.OwningPropertyInspectorModelProperty
      });
      binding.RelativeSource = RelativeSource.Self;
      return binding.ProvideValue(serviceProvider);
    }
  }
}
