// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.UserInterface.FocusScopeManager
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Workspaces.Extension;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Microsoft.Expression.Framework.UserInterface
{
  public class FocusScopeManager
  {
    private List<WeakReference> scopes = new List<WeakReference>();
    private bool allowFocusChangeDenial = true;
    public static readonly DependencyProperty AllowedFocusProperty = DependencyProperty.RegisterAttached("AllowedFocus", typeof (bool), typeof (FocusScopeManager), (PropertyMetadata) new FrameworkPropertyMetadata((object) true, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(FocusScopeManager.AllowedFocusChanged)));
    private static int DefaultFocusScopePriority = int.MaxValue;
    public static readonly DependencyProperty FocusScopePriorityProperty = DependencyProperty.RegisterAttached("FocusScopePriority", typeof (int), typeof (FocusScopeManager), (PropertyMetadata) new FrameworkPropertyMetadata((object) FocusScopeManager.DefaultFocusScopePriority, new PropertyChangedCallback(FocusScopeManager.FocusScopePriorityChanged)));
    private bool listContainsDeadReferences;
    private WeakReference activeManagedFocusScope;
    private bool denyNextFocusChange;
    private bool handlingPointerButtonEvent;
    private static FocusScopeManager instance;
    private ReturnFocusCallback returnFocusCallback;
    private FocusScopeManagerActivateBehavior activateBehavior;

    public bool AllowFocusChangeDenial
    {
      get
      {
        return this.allowFocusChangeDenial;
      }
      set
      {
        this.allowFocusChangeDenial = value;
      }
    }

    public ReturnFocusCallback ReturnFocusCallback
    {
      get
      {
        return this.returnFocusCallback;
      }
      set
      {
        if (this.returnFocusCallback != null)
          return;
        this.returnFocusCallback = value;
      }
    }

    private UIElement ActiveManagedFocusScope
    {
      get
      {
        if (this.activeManagedFocusScope != null && this.activeManagedFocusScope.IsAlive)
          return this.activeManagedFocusScope.Target as UIElement;
        return (UIElement) null;
      }
      set
      {
        if (this.activeManagedFocusScope == null)
          this.activeManagedFocusScope = new WeakReference((object) value);
        else
          this.activeManagedFocusScope.Target = (object) value;
      }
    }

    public static FocusScopeManager Instance
    {
      get
      {
        if (FocusScopeManager.instance == null)
        {
          FocusScopeManager.instance = new FocusScopeManager();
          EventManager.RegisterClassHandler(typeof (Window), Keyboard.GotKeyboardFocusEvent, (Delegate) new KeyboardFocusChangedEventHandler(FocusScopeManager.HandleGotKeyboardFocusEvent), true);
          EventManager.RegisterClassHandler(typeof (Popup), Keyboard.GotKeyboardFocusEvent, (Delegate) new KeyboardFocusChangedEventHandler(FocusScopeManager.HandleGotKeyboardFocusEvent), true);
          EventManager.RegisterClassHandler(typeof (Window), Keyboard.PreviewGotKeyboardFocusEvent, (Delegate) new KeyboardFocusChangedEventHandler(FocusScopeManager.HandlePreviewGotKeyboardFocus), true);
          EventManager.RegisterClassHandler(typeof (Popup), Keyboard.PreviewGotKeyboardFocusEvent, (Delegate) new KeyboardFocusChangedEventHandler(FocusScopeManager.HandlePreviewGotKeyboardFocus), true);
        }
        return FocusScopeManager.instance;
      }
    }

    public static bool HasInstance
    {
      get
      {
        return FocusScopeManager.instance != null;
      }
    }

    private bool ShouldDenyFocusChange
    {
      get
      {
        if (this.AllowFocusChangeDenial && this.denyNextFocusChange)
          return !this.handlingPointerButtonEvent;
        return false;
      }
    }

    public FocusScopeManagerActivateBehavior ActivateBehavior
    {
      get
      {
        return this.activateBehavior;
      }
      set
      {
        this.activateBehavior = value;
      }
    }

    static FocusScopeManager()
    {
      FocusManager.FocusedElementProperty.OverrideMetadata(typeof (FrameworkElement), new PropertyMetadata((object) null, (PropertyChangedCallback) null, new CoerceValueCallback(FocusScopeManager.FocusManager_CoerceFocusedElement)));
    }

    private FocusScopeManager()
    {
    }

    private static void HandleGotKeyboardFocusEvent(object sender, KeyboardFocusChangedEventArgs e)
    {
      FocusScopeManager.SetFocusToFocusScope(e.NewFocus);
    }

    internal static void HandlePreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
      DependencyObject element = e.NewFocus as DependencyObject;
      Visual visual = element as Visual;
      if (FocusScopeManager.Instance.ShouldDenyFocusChange)
      {
        FocusScopeManager.Instance.EndDenyNextFocusChange();
        if (!(visual is ExpressionFloatingWindow))
        {
          e.Handled = true;
          return;
        }
      }
      if (element == null || FocusScopeManager.GetAllowedFocus(element))
        return;
      FocusScopeManager.Instance.ReturnFocus();
      e.Handled = true;
    }

    public static void SetFocusToFocusScope(IInputElement newFocus)
    {
      UIElement uiElement = newFocus as UIElement;
      if (uiElement == null)
        return;
      UIElement focusScope = FocusManager.GetFocusScope((DependencyObject) uiElement) as UIElement;
      int focusScopePriority = FocusScopeManager.GetFocusScopePriority((DependencyObject) focusScope);
      if (focusScopePriority == FocusScopeManager.DefaultFocusScopePriority)
        return;
      FocusScopeManager.Instance.OnScopeKeyboardFocusChanged(focusScope, focusScopePriority);
    }

    public static void DenyNextFocusChange()
    {
      FocusScopeManager.Instance.denyNextFocusChange = true;
      InputManager.Current.PreNotifyInput += new NotifyInputEventHandler(FocusScopeManager.Instance.InputManager_PreNotifyInput);
      InputManager.Current.PostNotifyInput += new NotifyInputEventHandler(FocusScopeManager.Instance.InputManager_PostNotifyInput);
      UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.Normal, (Action) (() =>
      {
        if (!FocusScopeManager.HasInstance)
          return;
        FocusScopeManager.Instance.EndDenyNextFocusChange();
      }));
    }

    private void EndDenyNextFocusChange()
    {
      this.denyNextFocusChange = false;
      this.handlingPointerButtonEvent = false;
      InputManager.Current.PreNotifyInput -= new NotifyInputEventHandler(this.InputManager_PreNotifyInput);
      InputManager.Current.PostNotifyInput -= new NotifyInputEventHandler(this.InputManager_PostNotifyInput);
    }

    private bool IsPointerButtonEventItem(StagingAreaInputItem stagingItem)
    {
      if (stagingItem == null)
        return false;
      if (!(stagingItem.Input is MouseButtonEventArgs))
        return stagingItem.Input is StylusButtonEventArgs;
      return true;
    }

    private void InputManager_PreNotifyInput(object sender, NotifyInputEventArgs e)
    {
      if (!this.IsPointerButtonEventItem(e.StagingItem))
        return;
      this.handlingPointerButtonEvent = true;
    }

    private void InputManager_PostNotifyInput(object sender, NotifyInputEventArgs e)
    {
      if (!this.IsPointerButtonEventItem(e.StagingItem))
        return;
      this.handlingPointerButtonEvent = false;
    }

    public static void SetFocusScopePriority(DependencyObject element, int value)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      if (!FocusManager.GetIsFocusScope(element))
        throw new ArgumentException(ExceptionStringTable.CanOnlySetFocusScopePriorityOnAnElementThatIsAFocusScope);
      element.SetValue(FocusScopeManager.FocusScopePriorityProperty, (object) value);
    }

    public static int GetFocusScopePriority(DependencyObject element)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      return (int) element.GetValue(FocusScopeManager.FocusScopePriorityProperty);
    }

    public static void SetAllowedFocus(DependencyObject element, bool value)
    {
        if (element == null)
        {
            throw new ArgumentNullException("element");
        }
        element.SetValue(FocusScopeManager.AllowedFocusProperty, value);
    }

    public static bool GetAllowedFocus(DependencyObject element)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      return (bool) element.GetValue(FocusScopeManager.AllowedFocusProperty);
    }

    private static void FocusScopePriorityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      FocusScopeManager.Instance.UpdateFocusScopePriorityForElement(d as UIElement, (int) e.OldValue, (int) e.NewValue);
    }

    private static object FocusManager_CoerceFocusedElement(DependencyObject d, object value)
    {
      if (d is UIElement)
        return FocusScopeManager.Instance.CoerceFocusedElement(value);
      return value;
    }

    private static void AllowedFocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      FrameworkElement element = d as FrameworkElement;
      if (element == null)
        return;
      bool newValue = (bool) e.NewValue;
      FocusScopeManager.Instance.OnAllowedFocusChanged(element, newValue);
    }

    private void UpdateFocusScopePriorityForElement(UIElement element, int oldValue, int newValue)
    {
      if (oldValue != FocusScopeManager.DefaultFocusScopePriority)
        this.RemoveScope(element);
      if (newValue == FocusScopeManager.DefaultFocusScopePriority)
        return;
      this.InsertScope(this.FindStartIndexForPriority(newValue), element);
    }

    private object CoerceFocusedElement(object newFocus)
    {
      UIElement uiElement = newFocus as UIElement;
      if (uiElement != null)
      {
        if (this.ShouldDenyFocusChange)
        {
          this.EndDenyNextFocusChange();
          return DependencyProperty.UnsetValue;
        }
        if (newFocus != null && !FocusScopeManager.GetAllowedFocus((DependencyObject) uiElement))
          return DependencyProperty.UnsetValue;
      }
      return newFocus;
    }

    private void OnAllowedFocusChanged(FrameworkElement element, bool newValue)
    {
      if (newValue || !element.IsKeyboardFocused)
        return;
      this.ReturnFocus();
    }

    internal void ReturnFocus()
    {
      if (this.ReturnFocusCallback != null)
        this.ReturnFocusCallback();
      else
        Keyboard.Focus((IInputElement) null);
    }

    private static DependencyObject GetParentFocusScope(DependencyObject element)
    {
      bool isFocusScope = FocusManager.GetIsFocusScope(element);
      if (isFocusScope)
        FocusManager.SetIsFocusScope(element, false);
      DependencyObject focusScope = FocusManager.GetFocusScope(element);
      if (isFocusScope)
        FocusManager.SetIsFocusScope(element, true);
      if (focusScope != element)
        return focusScope;
      return (DependencyObject) null;
    }

    private void OnScopeKeyboardFocusChanged(UIElement focusScope, int priority)
    {
      switch (this.ActivateBehavior)
      {
        case FocusScopeManagerActivateBehavior.ClearFocusedElementInLowerPriorityScopes:
          if (focusScope == this.ActiveManagedFocusScope)
            break;
          int indexForPriority = this.FindStartIndexForPriority(priority);
          if (!this.IsFocusScopeManaged(focusScope, priority, indexForPriority))
            break;
          for (int index = indexForPriority; index < this.scopes.Count; ++index)
          {
            WeakReference weakReference = this.scopes[index];
            if (weakReference.IsAlive)
            {
              UIElement uiElement = (UIElement) weakReference.Target;
              if (uiElement != focusScope)
                FocusManager.SetFocusedElement((DependencyObject) uiElement, (IInputElement) null);
            }
            else
              this.listContainsDeadReferences = true;
          }
          this.CleanUpDeadReferences();
          this.ActiveManagedFocusScope = focusScope;
          break;
        case FocusScopeManagerActivateBehavior.UpdateFocusedElementInAncestorFocusScopes:
          DependencyObject element = (DependencyObject) focusScope;
          while (true)
          {
            DependencyObject parentFocusScope = FocusScopeManager.GetParentFocusScope(element);
            if (parentFocusScope != null)
            {
              FocusManager.SetFocusedElement(parentFocusScope, (IInputElement) element);
              element = parentFocusScope;
            }
            else
              break;
          }
          break;
      }
    }

    private bool IsFocusScopeManaged(UIElement focusScope, int priority, int priorityStartIndex)
    {
      for (int index = priorityStartIndex; index < this.scopes.Count; ++index)
      {
        WeakReference weakReference = this.scopes[index];
        if (weakReference.IsAlive)
        {
          UIElement uiElement = (UIElement) weakReference.Target;
          if (FocusScopeManager.GetFocusScopePriority((DependencyObject) uiElement) > priority)
            return false;
          if (uiElement == focusScope)
            return true;
        }
      }
      return false;
    }

    private void CleanUpDeadReferences()
    {
      if (!this.listContainsDeadReferences)
        return;
      for (int index = this.scopes.Count - 1; index >= 0; --index)
      {
        if (!this.scopes[index].IsAlive)
          this.scopes.RemoveAt(index);
      }
      this.listContainsDeadReferences = false;
    }

    private int FindStartIndexForPriority(int priority)
    {
      int index;
      for (index = 0; index < this.scopes.Count; ++index)
      {
        WeakReference weakReference = this.scopes[index];
        if (weakReference.IsAlive)
        {
          if (FocusScopeManager.GetFocusScopePriority((DependencyObject) weakReference.Target) >= priority)
            break;
        }
        else
          this.listContainsDeadReferences = true;
      }
      return index;
    }

    private void InsertScope(int index, UIElement scope)
    {
      this.scopes.Insert(index, new WeakReference((object) scope));
    }

    private void RemoveScope(UIElement scope)
    {
      for (int index = this.scopes.Count - 1; index >= 0; --index)
      {
        WeakReference weakReference = this.scopes[index];
        if (!weakReference.IsAlive || weakReference.Target == scope)
          this.scopes.RemoveAt(index);
      }
    }
  }
}
