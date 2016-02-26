// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.ModalCommands.ViewExceptionCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Controls;
using System;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.SceneCommands.ModalCommands
{
  internal sealed class ViewExceptionCommand : ModalCommandBase
  {
    private Exception exception;

    public override bool IsEnabled
    {
      get
      {
        return true;
      }
    }

    public override string Description
    {
      get
      {
        return StringTable.ViewExceptionDetailContextualDescription;
      }
    }

    public ViewExceptionCommand(Exception exception, SceneViewModel viewModel)
      : base(viewModel)
    {
      this.exception = exception;
    }

    public override void Execute()
    {
      new ViewExceptionCommand.ExceptionDialog(this.SceneViewModel.ProjectContext, this.exception).ShowDialog();
    }

    private class ExceptionDialog : Dialog
    {
      public ExceptionDialog(IProjectContext projectContext, Exception exception)
      {
        this.DialogContent = (UIElement) FileTable.GetElement("Resources\\ExceptionDialog.xaml");
        FormattedException formattedException = new ExceptionFormatter(projectContext).Format(exception);
        this.DataContext = (object) formattedException;
        this.Title = formattedException.TypeName;
        this.Width = 600.0;
        this.Height = 300.0;
        this.ResizeMode = ResizeMode.CanResizeWithGrip;
      }
    }
  }
}
