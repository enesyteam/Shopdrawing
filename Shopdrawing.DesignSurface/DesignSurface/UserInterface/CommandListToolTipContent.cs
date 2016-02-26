// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.CommandListToolTipContent
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.SceneCommands.ModalCommands;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  internal class CommandListToolTipContent : Control
  {
    private SceneViewModel viewModel;
    private ToolTip containingToolTip;
    private string exceptionMessage;
    private List<ContentControl> actions;

    static CommandListToolTipContent()
    {
      Style style = FileTable.GetStyle("Resources\\CommandListToolTipContent.xaml");
      FrameworkElement.StyleProperty.OverrideMetadata(typeof (CommandListToolTipContent), (PropertyMetadata) new FrameworkPropertyMetadata((object) style));
    }

    public CommandListToolTipContent(SceneViewModel viewModel, ToolTip containingToolTip, string exceptionMessage)
    {
      this.viewModel = viewModel;
      this.containingToolTip = containingToolTip;
      this.exceptionMessage = exceptionMessage;
      this.actions = new List<ContentControl>();
      this.containingToolTip.Opened += new RoutedEventHandler(this.ContainingToolTip_Opened);
    }

    public void AddAction(ModalCommandBase command)
    {
      this.actions.Add((ContentControl) new CommandListToolTipContent.InvokeActionControl(this.containingToolTip, (ICommand) command, command.Description));
    }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      ItemsControl itemsControl = this.GetTemplateChild("ActionsList") as ItemsControl;
      if (itemsControl != null)
      {
        foreach (ContentControl contentControl in this.actions)
          itemsControl.Items.Add((object) contentControl);
      }
      TextBlock textBlock = this.GetTemplateChild("ExceptionText") as TextBlock;
      if (textBlock == null)
        return;
      textBlock.Text = this.exceptionMessage;
    }

    private void ContainingToolTip_Opened(object sender, RoutedEventArgs e)
    {
      this.viewModel.LateSceneUpdatePhase += new SceneUpdatePhaseEventHandler(this.ViewModel_LateSceneUpdatePhase);
    }

    private void ViewModel_LateSceneUpdatePhase(object sender, SceneUpdatePhaseEventArgs args)
    {
      if (this.containingToolTip.IsOpen)
        this.containingToolTip.IsOpen = false;
      this.viewModel.LateSceneUpdatePhase -= new SceneUpdatePhaseEventHandler(this.ViewModel_LateSceneUpdatePhase);
    }

    private class InvokeActionControl : Button
    {
      private ToolTip containingToolTip;

      public InvokeActionControl(ToolTip containingToolTip, ICommand command, string text)
      {
        this.containingToolTip = containingToolTip;
        this.Command = command;
        this.Content = (object) text;
        this.IsEnabled = this.Command.CanExecute((object) null);
      }

      protected override void OnClick()
      {
        this.containingToolTip.IsOpen = false;
        this.Dispatcher.BeginInvoke(DispatcherPriority.Send, (Delegate) new DispatcherOperationCallback(this.ExecuteCommandCallback), (object) null);
      }

      private object ExecuteCommandCallback(object arg)
      {
        this.Command.Execute((object) null);
        return (object) null;
      }
    }
  }
}
