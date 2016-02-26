// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.EasingFunctionSelectionButton
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.ViewObjects;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class EasingFunctionSelectionButton : Button
  {
    public static readonly DependencyProperty EasingProperty = DependencyProperty.Register("Easing", typeof (EasingMode), typeof (EasingFunctionSelectionButton), (PropertyMetadata) new FrameworkPropertyMetadata((object) EasingMode.None, FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty SelectedEasingFunctionProperty = DependencyProperty.Register("SelectedEasingFunction", typeof (IEasingFunctionDefinition), typeof (EasingFunctionSelectionButton), (PropertyMetadata) new FrameworkPropertyMetadata(null, new PropertyChangedCallback(EasingFunctionSelectionButton.OnSelectedEasingFunctionChanged)));
    public static readonly DependencyProperty OwnEasingFunctionProperty = DependencyProperty.Register("OwnEasingFunction", typeof (IEasingFunctionDefinition), typeof (EasingFunctionSelectionButton), (PropertyMetadata) new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty CanBeEnabledProperty = DependencyProperty.Register("CanBeEnabled", typeof (bool), typeof (EasingFunctionSelectionButton), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, new PropertyChangedCallback(EasingFunctionSelectionButton.OnSelectedEasingFunctionChanged)));
    public static readonly DependencyProperty MatchesSelectionProperty = DependencyProperty.Register("MatchesSelection", typeof (bool), typeof (EasingFunctionSelectionButton), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty IsHighlightedProperty = DependencyProperty.Register("IsHighlighted", typeof (bool), typeof (EasingFunctionSelectionButton), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, FrameworkPropertyMetadataOptions.AffectsRender));

    public EasingMode Easing
    {
      get
      {
        return (EasingMode) this.GetValue(EasingFunctionSelectionButton.EasingProperty);
      }
      set
      {
        this.SetValue(EasingFunctionSelectionButton.EasingProperty, value);
      }
    }

    public IEasingFunctionDefinition SelectedEasingFunction
    {
      get
      {
        return (IEasingFunctionDefinition) this.GetValue(EasingFunctionSelectionButton.SelectedEasingFunctionProperty);
      }
      set
      {
        this.SetValue(EasingFunctionSelectionButton.SelectedEasingFunctionProperty, value);
      }
    }

    public IEasingFunctionDefinition OwnEasingFunction
    {
      get
      {
        return (IEasingFunctionDefinition) this.GetValue(EasingFunctionSelectionButton.OwnEasingFunctionProperty);
      }
      set
      {
        this.SetValue(EasingFunctionSelectionButton.OwnEasingFunctionProperty, value);
      }
    }

    public bool CanBeEnabled
    {
      get
      {
        return (bool) this.GetValue(EasingFunctionSelectionButton.CanBeEnabledProperty);
      }
      set
      {
        this.SetValue(EasingFunctionSelectionButton.CanBeEnabledProperty, (object) (bool) (value ? 1 : 0));
      }
    }

    public bool MatchesSelection
    {
      get
      {
        return (bool) this.GetValue(EasingFunctionSelectionButton.MatchesSelectionProperty);
      }
      set
      {
        this.SetValue(EasingFunctionSelectionButton.MatchesSelectionProperty, (object) (bool) (value ? 1 : 0));
      }
    }

    public bool IsHighlighted
    {
      get
      {
        return (bool) this.GetValue(EasingFunctionSelectionButton.IsHighlightedProperty);
      }
      set
      {
        this.SetValue(EasingFunctionSelectionButton.IsHighlightedProperty, (object) (bool) (value ? 1 : 0));
      }
    }

    public event RoutedEventHandler OnButtonPressed;

    public EasingFunctionSelectionButton()
    {
      this.AddHandler(UIElement.PreviewMouseLeftButtonDownEvent, (Delegate) new MouseButtonEventHandler(this.MouseLeftButtonDownHandler), true);
    }

    public void FireButtonPressed()
    {
      if (this.OnButtonPressed == null)
        return;
      this.OnButtonPressed(this, new RoutedEventArgs());
    }

    private static void OnSelectedEasingFunctionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      EasingFunctionSelectionButton functionSelectionButton = d as EasingFunctionSelectionButton;
      IEasingFunctionDefinition selectedEasingFunction = functionSelectionButton.SelectedEasingFunction;
      IEasingFunctionDefinition ownEasingFunction = functionSelectionButton.OwnEasingFunction;
      bool flag = functionSelectionButton.CanBeEnabled && (selectedEasingFunction == null || ownEasingFunction == null ? selectedEasingFunction == null && ownEasingFunction == null : ownEasingFunction.PlatformSpecificObject.GetType() == selectedEasingFunction.PlatformSpecificObject.GetType() && functionSelectionButton.Easing == selectedEasingFunction.EasingMode);
      functionSelectionButton.MatchesSelection = flag;
    }

    private void MouseLeftButtonDownHandler(object sender, MouseButtonEventArgs args)
    {
      this.FireButtonPressed();
    }
  }
}
