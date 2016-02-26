// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Triggers.TriggersView
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.UserInterface.Triggers
{
  public class TriggersView : Decorator
  {
    private TriggerModelManager triggersModel;

    internal TriggersView(DesignerContext designerContext)
    {
      this.triggersModel = new TriggerModelManager(designerContext);
    }

    protected override void OnInitialized(EventArgs e)
    {
      base.OnInitialized(e);
      this.triggersModel.Initialize();
      FrameworkElement element = FileTable.GetElement("Resources\\TriggersPane\\Triggers.xaml");
      element.DataContext = (object) this.triggersModel;
      this.Child = (UIElement) element;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
      if (Keyboard.Modifiers != ModifierKeys.None || e.Key != Key.Delete || !this.triggersModel.DeleteTriggerCommand.CanExecute((object) null))
        return;
      this.triggersModel.DeleteTriggerCommand.Execute((object) null);
      e.Handled = true;
    }
  }
}
