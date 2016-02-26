// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.EasingFunctionComboBox
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.Framework.Controls;
using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class EasingFunctionComboBox : ComboBox
  {
    private ArrayList buttonOrder = new ArrayList((ICollection) Enum.GetValues(typeof (EasingMode)));
    private EasingFunctionSelectionButton nullButton;
    private EasingFunctionSelectionButton lastHighlightedButton;

    private EasingFunctionSelectionButton LastHighlightedButton
    {
      get
      {
        return this.lastHighlightedButton;
      }
      set
      {
        if (this.lastHighlightedButton != null)
          this.lastHighlightedButton.IsHighlighted = false;
        this.lastHighlightedButton = value;
        if (this.lastHighlightedButton == null)
          return;
        this.lastHighlightedButton.IsHighlighted = true;
      }
    }

    public EasingFunctionComboBox()
    {
      this.AddHandler(UIElement.PreviewKeyDownEvent, (Delegate) new KeyEventHandler(this.KeyDownEventHandler), true);
      this.AddHandler(ExpressionPopup.PopupClosedEvent, (Delegate) new RoutedEventHandler(this.HandlePopupClosed));
      EventManager.RegisterClassHandler(typeof (EasingFunctionSelectionButton), UIElement.MouseEnterEvent, new RoutedEventHandler(this.HandleMouseOverChanged));
      EventManager.RegisterClassHandler(typeof (EasingFunctionSelectionButton), UIElement.MouseLeaveEvent, new RoutedEventHandler(this.HandleMouseOverChanged));
    }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      this.nullButton = this.Template.FindName("PopUpDisplayNullEasingFunctionButton", (FrameworkElement) this) as EasingFunctionSelectionButton;
    }

    private void HandlePopupClosed(object sender, RoutedEventArgs args)
    {
      this.LastHighlightedButton = (EasingFunctionSelectionButton) null;
    }

    private void KeyDownEventHandler(object sender, KeyEventArgs args)
    {
      if (!this.IsDropDownOpen)
        return;
      if (args.Key == Key.Left || args.Key == Key.Right || (args.Key == Key.Down || args.Key == Key.Up))
      {
        if (this.lastHighlightedButton == null)
        {
          IEasingFunctionDefinition functionDefinition = this.Tag as IEasingFunctionDefinition;
          if (functionDefinition == null)
          {
            this.LastHighlightedButton = this.nullButton;
          }
          else
          {
            foreach (object obj in (IEnumerable) this.Items)
            {
              IEasingFunctionDefinition targetFunction = obj as IEasingFunctionDefinition;
              if (targetFunction != null && targetFunction.PlatformSpecificObject.GetType() == functionDefinition.PlatformSpecificObject.GetType())
              {
                this.MoveHighlight(targetFunction, this.buttonOrder.IndexOf(functionDefinition.EasingMode));
                break;
              }
            }
          }
        }
        if (this.LastHighlightedButton == this.nullButton)
        {
          if (args.Key != Key.Down)
            return;
          this.MoveHighlight(this.Items[0] as IEasingFunctionDefinition, this.buttonOrder.IndexOf(EasingMode.In));
        }
        else if (args.Key == Key.Left || args.Key == Key.Right)
        {
          this.MoveHighlight(this.LastHighlightedButton.DataContext as IEasingFunctionDefinition, this.buttonOrder.IndexOf(this.LastHighlightedButton.Easing) + (args.Key == Key.Left ? -1 : 1));
        }
        else
        {
          int index = this.Items.IndexOf(this.LastHighlightedButton.DataContext) + (args.Key == Key.Down ? 1 : -1);
          if (index < 0)
          {
            this.LastHighlightedButton = this.nullButton;
          }
          else
          {
            if (index == this.Items.Count)
              index = this.Items.Count - 1;
            this.MoveHighlight(this.Items[index] as IEasingFunctionDefinition, this.buttonOrder.IndexOf(this.LastHighlightedButton.Easing));
          }
        }
      }
      else
      {
        if (args.Key != Key.Return && args.Key != Key.Space)
          return;
        if (this.LastHighlightedButton == null)
          this.nullButton.FireButtonPressed();
        else
          this.LastHighlightedButton.FireButtonPressed();
      }
    }

    private void MoveHighlight(IEasingFunctionDefinition targetFunction, int buttonNumber)
    {
      if (targetFunction == null)
        return;
      ComboBoxItem comboBoxItem = (ComboBoxItem) this.ItemContainerGenerator.ContainerFromItem((object) targetFunction);
      if (comboBoxItem == null)
        return;
      ContentPresenter contentPresenter = Enumerable.FirstOrDefault<ContentPresenter>(Enumerable.OfType<ContentPresenter>(ElementUtilities.GetVisualTree((Visual) comboBoxItem)));
      DataTemplate contentTemplate = contentPresenter.ContentTemplate;
      if (buttonNumber < 0 || buttonNumber >= this.buttonOrder.Count)
        return;
      EasingFunctionSelectionButton functionSelectionButton = contentTemplate.FindName(this.buttonOrder[buttonNumber].ToString() + "Button", (FrameworkElement) contentPresenter) as EasingFunctionSelectionButton;
      if (functionSelectionButton == null)
        return;
      this.LastHighlightedButton = functionSelectionButton;
    }

    private void HandleMouseOverChanged(object sender, RoutedEventArgs args)
    {
      EasingFunctionSelectionButton functionSelectionButton = args.OriginalSource as EasingFunctionSelectionButton;
      if (functionSelectionButton != null)
      {
        if (functionSelectionButton.IsMouseOver)
          this.LastHighlightedButton = functionSelectionButton;
        functionSelectionButton.IsHighlighted = functionSelectionButton.IsMouseOver;
      }
      args.Handled = true;
    }
  }
}
