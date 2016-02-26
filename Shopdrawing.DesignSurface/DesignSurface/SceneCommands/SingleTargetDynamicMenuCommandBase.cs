// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.SingleTargetDynamicMenuCommandBase
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.Diagnostics;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal abstract class SingleTargetDynamicMenuCommandBase : DynamicMenuCommand
  {
    private SceneViewModel viewModel;

    private ViewState RequiredSelectionViewState
    {
      get
      {
        return SceneCommandBase.DefaultRequiredSelectionViewState;
      }
    }

    public override bool IsEnabled
    {
      get
      {
        if (base.IsEnabled)
          return SceneCommandBase.SceneCommandIsEnabled(this.SceneView, this.RequiredSelectionViewState);
        return false;
      }
    }

    protected DesignerContext DesignerContext
    {
      get
      {
        return this.viewModel.DesignerContext;
      }
    }

    protected SceneViewModel ViewModel
    {
      get
      {
        return this.viewModel;
      }
    }

    protected SceneView SceneView
    {
      get
      {
        return this.viewModel.DefaultView;
      }
    }

    protected IType Type
    {
      get
      {
        if (this.HasValidTarget)
          return SingleTargetCommandBase.GetTypeOfElement(this.TargetElement);
        return (IType) null;
      }
    }

    protected bool HasValidTarget
    {
      get
      {
        if (this.viewModel.ElementSelectionSet.Count == 1)
          return this.TargetElement != null;
        return false;
      }
    }

    protected SceneElement TargetElement
    {
      get
      {
        return this.viewModel.ElementSelectionSet.PrimarySelection;
      }
    }

    public SingleTargetDynamicMenuCommandBase(SceneViewModel viewModel)
    {
      this.viewModel = viewModel;
    }

    public override void Execute()
    {
      throw new InvalidOperationException();
    }

    protected MenuItem CreateMenuItem(string name, string id, System.Windows.Input.ICommand command)
    {
      MenuItem menuItem = new MenuItem();
      menuItem.Header = (object) name;
      menuItem.SetValue(AutomationElement.IdProperty, (object) id);
      menuItem.Command = command;
      if (command != null)
        menuItem.SetBinding(UIElement.IsEnabledProperty, (BindingBase) new Binding("IsEnabled")
        {
          Source = (object) command,
          Mode = BindingMode.OneTime
        });
      return menuItem;
    }
  }
}
