// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI.ActiveStoryboardControl
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.ValueEditors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI
{
  public class ActiveStoryboardControl : ContentPresenter
  {
    public static readonly DependencyProperty EditCommandProperty = DependencyProperty.Register("EditCommand", typeof (ICommand), typeof (ActiveStoryboardControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) DeadCommand.Instance));

    public ICommand EditCommand
    {
      get
      {
        return (ICommand) this.GetValue(ActiveStoryboardControl.EditCommandProperty);
      }
      set
      {
        this.SetValue(ActiveStoryboardControl.EditCommandProperty, (object) value);
      }
    }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      InlineStringEditor inlineStringEditor = this.GetTemplateChild("StoryboardName") as InlineStringEditor;
      if (inlineStringEditor != null)
        this.EditCommand = inlineStringEditor.EditCommand;
      else
        this.ClearValue(ActiveStoryboardControl.EditCommandProperty);
    }
  }
}
