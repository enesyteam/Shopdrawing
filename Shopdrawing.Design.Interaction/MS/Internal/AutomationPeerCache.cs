// Decompiled with JetBrains decompiler
// Type: MS.Internal.AutomationPeerCache
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Data;

namespace MS.Internal
{
  internal static class AutomationPeerCache
  {
    public static readonly DependencyProperty AutomationPeerProperty = DependencyProperty.RegisterAttached("AutomationPeer", typeof (AutomationPeer), typeof (AutomationPeerCache), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty IsAutomationFocusedProperty = DependencyProperty.RegisterAttached("IsAutomationFocused", typeof (bool), typeof (AutomationPeerCache), (PropertyMetadata) new UIPropertyMetadata((object) false, new PropertyChangedCallback(AutomationPeerCache.OnIsAutomationFocusedChanged)));
    private static DependencyObject _objThatIsBinding;

    public static AutomationPeer GetAutomationPeer(DependencyObject obj)
    {
      return (AutomationPeer) obj.GetValue(AutomationPeerCache.AutomationPeerProperty);
    }

    public static void SetAutomationPeer(DependencyObject obj, AutomationPeer value)
    {
      obj.SetValue(AutomationPeerCache.AutomationPeerProperty, (object) value);
    }

    public static void RegisterFocusEvents(DependencyObject obj)
    {
      if (AutomationPeerCache.GetIsAutomationFocused(obj))
        return;
      AutomationPeerCache.RegisterFocusEvents(obj, "IsKeyboardFocused");
    }

    public static void RegisterFocusEvents(DependencyObject obj, AutomationPeer associatedPeer)
    {
      if (AutomationPeerCache.GetIsAutomationFocused(obj))
        return;
      if (AutomationPeerCache.GetAutomationPeer(obj) == null && UIElementAutomationPeer.FromElement(obj as UIElement) == null)
        AutomationPeerCache.SetAutomationPeer(obj, associatedPeer);
      AutomationPeerCache.RegisterFocusEvents(obj, "IsKeyboardFocused");
    }

    public static void RegisterFocusEvents(DependencyObject obj, string propertyName)
    {
      if (!(obj is UIElement))
        return;
      try
      {
        AutomationPeerCache._objThatIsBinding = obj;
        BindingOperations.SetBinding(obj, AutomationPeerCache.IsAutomationFocusedProperty, (BindingBase) new Binding(propertyName)
        {
          RelativeSource = RelativeSource.Self
        });
      }
      finally
      {
        AutomationPeerCache._objThatIsBinding = (DependencyObject) null;
      }
    }

    public static bool GetIsAutomationFocused(DependencyObject obj)
    {
      return (bool) obj.GetValue(AutomationPeerCache.IsAutomationFocusedProperty);
    }

    public static void SetIsAutomationFocused(DependencyObject obj, bool value)
    {
      obj.SetValue(AutomationPeerCache.IsAutomationFocusedProperty, (object) (bool) (value ? true : false));
    }

    private static void OnIsAutomationFocusedChanged(DependencyObject dobj, DependencyPropertyChangedEventArgs e)
    {
      if (AutomationPeerCache._objThatIsBinding == dobj)
        return;
      AutomationPeer automationPeer = AutomationPeerCache.GetAutomationPeer(dobj) ?? UIElementAutomationPeer.FromElement(dobj as UIElement);
      if (automationPeer == null)
        return;
      if (AutomationPeer.ListenerExists(AutomationEvents.PropertyChanged))
      {
        automationPeer.RaisePropertyChangedEvent(SelectionItemPatternIdentifiers.IsSelectedProperty, e.OldValue, e.NewValue);
        automationPeer.RaisePropertyChangedEvent(AutomationElementIdentifiers.HasKeyboardFocusProperty, e.OldValue, e.NewValue);
      }
      if (AutomationPeer.ListenerExists(AutomationEvents.AutomationFocusChanged))
        automationPeer.RaiseAutomationEvent(AutomationEvents.AutomationFocusChanged);
      if (!(automationPeer is ItemAutomationPeer) || !AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementSelected))
        return;
      automationPeer.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementSelected);
    }

    public static T Create<T>(UIElement element, params object[] args) where T : AutomationPeer
    {
      AutomationPeer automationPeer = AutomationPeerCache.GetAutomationPeer((DependencyObject) element);
      if (automationPeer == null)
      {
        if (args.Length == 0)
          automationPeer = (AutomationPeer) Activator.CreateInstance(typeof (T), new object[1]
          {
            (object) element
          });
        else
          automationPeer = (AutomationPeer) Activator.CreateInstance(typeof (T), args);
        if (automationPeer != null)
        {
          AutomationPeerCache.SetAutomationPeer((DependencyObject) element, automationPeer);
          AutomationPeerCache.RegisterFocusEvents((DependencyObject) element);
        }
      }
      return automationPeer as T;
    }
  }
}
