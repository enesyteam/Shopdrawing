// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.BindToDataCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.UserInterface.DataPane;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Globalization;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal sealed class BindToDataCommand : SingleTargetCommandBase
  {
    public override bool IsEnabled
    {
      get
      {
        if (!base.IsEnabled || this.TargetProperty == null)
          return false;
        SceneElement targetElement = this.TargetElement;
        if (targetElement.Type.XamlSourcePath != null)
          return false;
        ITypeId type = (ITypeId) targetElement.TrueTargetTypeId;
        return !PlatformTypes.UserControl.IsAssignableFrom(type) && !PlatformTypes.Window.IsAssignableFrom(type);
      }
    }

    private ReferenceStep TargetProperty
    {
      get
      {
        BindingPropertyMatchInfo bindingPropertyInfo = BindingPropertyHelper.GetDefaultBindingPropertyInfo((SceneNode) this.TargetElement, this.TargetElement.Platform.Metadata.ResolveType(PlatformTypes.IEnumerable));
        if (bindingPropertyInfo.Compatibility == BindingPropertyCompatibility.Assignable)
          return (ReferenceStep) bindingPropertyInfo.Property;
        return (ReferenceStep) null;
      }
    }

    private string UndoString
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.BindToDataCommandText, new object[1]
        {
          (object) (this.TargetProperty != null ? this.TargetProperty.Name : string.Empty)
        });
      }
    }

    public BindToDataCommand(SceneViewModel viewModel)
      : base(viewModel)
    {
    }

    public override void Execute()
    {
      if (this.TargetProperty == null)
        return;
      DataBindingDialog.CreateAndSetBindingOrData(this.DesignerContext, (SceneNode) this.TargetElement, this.TargetProperty);
    }

    public override object GetProperty(string propertyName)
    {
      if (propertyName == "Text")
        return (object) this.UndoString;
      return base.GetProperty(propertyName);
    }
  }
}
