// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.ControlStylingCommandBase
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal abstract class ControlStylingCommandBase : SingleTargetCommandBase, ICommand
  {
    private ISceneViewHost viewHost;
    private ReferenceStep targetProperty;
    private bool useRootTemplateProperty;

    protected ISceneViewHost ViewHost
    {
      get
      {
        return this.viewHost;
      }
    }

    protected virtual ReferenceStep TargetProperty
    {
      get
      {
        if (this.useRootTemplateProperty)
          return this.ActiveTemplateProperty;
        return this.targetProperty;
      }
    }

    protected PropertyReference TargetPropertyReference
    {
      get
      {
        return new PropertyReference(this.TargetProperty);
      }
    }

    protected ReferenceStep ActiveTemplateProperty
    {
      get
      {
        return this.ProvideTemplateProperty(this.Type);
      }
    }

    public event EventHandler CanExecuteChanged;

    public ControlStylingCommandBase(ISceneViewHost viewHost, SceneViewModel viewModel, IPropertyId targetProperty)
      : this(viewHost, viewModel, targetProperty, false)
    {
    }

    public ControlStylingCommandBase(ISceneViewHost viewHost, SceneViewModel viewModel, IPropertyId targetProperty, bool useRootTemplateProperty)
      : base(viewModel)
    {
      this.viewHost = viewHost;
      this.targetProperty = viewModel.ProjectContext.ResolveProperty(targetProperty) as ReferenceStep;
      this.useRootTemplateProperty = useRootTemplateProperty;
    }

    protected void OnCanExecuteChanged()
    {
      if (this.CanExecuteChanged == null)
        return;
      this.CanExecuteChanged((object) this, EventArgs.Empty);
    }

    protected ReferenceStep ProvideTemplateProperty(IType targetType)
    {
      IPropertyId propertyId = ControlElement.TemplateProperty;
      if (PlatformTypes.Page.IsAssignableFrom((ITypeId) targetType))
        propertyId = PageElement.TemplateProperty;
      return targetType.PlatformMetadata.ResolveProperty(propertyId) as ReferenceStep;
    }

    protected IPropertyId GetTargetTypeProperty(ITypeId styleOrTemplate)
    {
      IPropertyId propertyId = (IPropertyId) null;
      if (PlatformTypes.Style.IsAssignableFrom(styleOrTemplate))
        propertyId = StyleNode.TargetTypeProperty;
      else if (PlatformTypes.ControlTemplate.IsAssignableFrom(styleOrTemplate))
        propertyId = ControlTemplateElement.TargetTypeProperty;
      return propertyId;
    }

    void ICommand.Execute(object arg)
    {
      this.Execute();
    }

    bool ICommand.CanExecute(object arg)
    {
      return this.IsEnabled;
    }
  }
}
