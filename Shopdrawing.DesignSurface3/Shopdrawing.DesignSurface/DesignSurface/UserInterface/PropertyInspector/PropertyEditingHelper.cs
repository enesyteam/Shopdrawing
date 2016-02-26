// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.PropertyEditingHelper
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.PropertyInspector;
using Microsoft.Expression.Framework.ValueEditors;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class PropertyEditingHelper
  {
    public static readonly DependencyProperty SupportedPropertyCommandsProperty = DependencyProperty.RegisterAttached("SupportedPropertyCommands", typeof (SupportedPropertyCommands), typeof (PropertyEditingHelper), (PropertyMetadata) new FrameworkPropertyMetadata((object) SupportedPropertyCommands.All, FrameworkPropertyMetadataOptions.Inherits));
    private Stack<SceneEditTransaction> editTransactions = new Stack<SceneEditTransaction>();
    private Dictionary<FrameworkElement, PropertyEditingHelper.CommandBindingHolder> commandBindingReferences = new Dictionary<FrameworkElement, PropertyEditingHelper.CommandBindingHolder>();
    private SceneDocument activeDocument;
    private UIElement host;

    public virtual SceneDocument ActiveDocument
    {
      get
      {
        return this.activeDocument;
      }
      set
      {
        if (value == this.activeDocument)
          return;
        this.activeDocument = value;
      }
    }

    private SceneEditTransaction CurrentTransaction
    {
      get
      {
        if (this.editTransactions.Count == 0)
          return (SceneEditTransaction) null;
        return this.editTransactions.Peek();
      }
    }

    public PropertyEditingHelper(UIElement host)
      : this((SceneDocument) null, host)
    {
    }

    public PropertyEditingHelper(SceneDocument activeDocument, UIElement host)
    {
      this.activeDocument = activeDocument;
      this.host = host;
      if (this.host == null)
        return;
      this.AddCommandBindings(host, SupportedPropertyCommands.All);
    }

    public void AddCommandBindings(UIElement host, SupportedPropertyCommands supportedCommands)
    {
      host.SetValue(ExpressionPropertyValueEditorCommands.ExpressionPropertyValueEditorCommandsProperty, (object) new ExpressionPropertyValueEditorCommands()
      {
        BeginTransaction = new ExpressionPropertyValueEditorCommand((ExpressionPropertyValueEditorCommandHandler) (obj => this.OnBeginTransaction(obj))),
        CommitTransaction = new ExpressionPropertyValueEditorCommand((ExpressionPropertyValueEditorCommandHandler) (obj => this.OnCommitTransaction(obj))),
        AbortTransaction = new ExpressionPropertyValueEditorCommand((ExpressionPropertyValueEditorCommandHandler) (obj => this.OnAbortTransaction(obj)))
      });
      if ((supportedCommands & SupportedPropertyCommands.Transactions) == SupportedPropertyCommands.Transactions)
      {
        host.CommandBindings.Add(new CommandBinding((ICommand) PropertyValueEditorCommands.BeginTransaction, new ExecutedRoutedEventHandler(this.OnBeginTransaction)));
        host.CommandBindings.Add(new CommandBinding((ICommand) PropertyValueEditorCommands.CommitTransaction, new ExecutedRoutedEventHandler(this.OnCommitTransaction)));
        host.CommandBindings.Add(new CommandBinding((ICommand) PropertyValueEditorCommands.AbortTransaction, new ExecutedRoutedEventHandler(this.OnAbortTransaction)));
      }
      if ((supportedCommands & SupportedPropertyCommands.DialogEditor) == SupportedPropertyCommands.DialogEditor)
      {
        host.CommandBindings.Add(new CommandBinding((ICommand) PropertyContainer.OpenDialogWindow, new ExecutedRoutedEventHandler(this.OnOpenDialogWindow), new CanExecuteRoutedEventHandler(this.OnCanOpenDialogWindow)));
        host.CommandBindings.Add(new CommandBinding((ICommand) PropertyValueEditorCommands.ShowDialogEditor, new ExecutedRoutedEventHandler(this.OnShowDialogEditor)));
      }
      if ((supportedCommands & SupportedPropertyCommands.PinnedEditor) == SupportedPropertyCommands.PinnedEditor)
        host.CommandBindings.Add(new CommandBinding((ICommand) PropertyValueEditorCommands.ShowExtendedPinnedEditor, new ExecutedRoutedEventHandler(this.OnShowExtendedPinnedEditor)));
      if ((supportedCommands & SupportedPropertyCommands.PopupEditor) == SupportedPropertyCommands.PopupEditor)
        host.CommandBindings.Add(new CommandBinding((ICommand) PropertyValueEditorCommands.ShowExtendedPopupEditor, new ExecutedRoutedEventHandler(this.OnShowExtendedPopupEditor)));
      host.CommandBindings.Add(new CommandBinding((ICommand) PropertyValueEditorCommands.ShowInlineEditor, new ExecutedRoutedEventHandler(this.OnShowInlineEditor)));
      host.SetValue(PropertyEditingHelper.SupportedPropertyCommandsProperty, (object) supportedCommands);
    }

    public void CancelOutstandingTransactions()
    {
      while (this.editTransactions.Count > 0)
      {
        this.editTransactions.Peek().Cancel();
        this.editTransactions.Pop();
      }
    }

    public bool CommitOutstandingTransactions(int remaining)
    {
      bool flag = this.editTransactions.Count > remaining;
      while (this.editTransactions.Count > remaining)
      {
        this.editTransactions.Peek().Commit();
        this.editTransactions.Pop();
      }
      return flag;
    }

    public void UpdateTransaction()
    {
      if (this.ActiveDocument == null)
        return;
      this.ActiveDocument.OnUpdatedEditTransaction();
    }

    public static PropertyContainer GetPropertyContainer(object element)
    {
      FrameworkElement frameworkElement = element as FrameworkElement;
      if (frameworkElement != null)
        return PropertyContainer.GetOwningPropertyContainer((DependencyObject) frameworkElement);
      return (PropertyContainer) null;
    }

    private void HookEditEndingHandlers(FrameworkElement element)
    {
      PropertyEditingHelper.CommandBindingHolder commandBindingHolder = (PropertyEditingHelper.CommandBindingHolder) null;
      UIElement uiElement = (UIElement) element.GetValue(PropertyInspectorHelper.OwningPropertyInspectorElementProperty);
      if (this.host == null || this.host != uiElement)
        this.host = uiElement;
      if (!this.commandBindingReferences.TryGetValue(element, out commandBindingHolder))
      {
        commandBindingHolder = new PropertyEditingHelper.CommandBindingHolder();
        commandBindingHolder.AbortCommandBinding = new CommandBinding((ICommand) PropertyValueEditorCommands.AbortTransaction, new ExecutedRoutedEventHandler(this.OnAbortOrCommitTransaction));
        commandBindingHolder.CommitCommandBinding = new CommandBinding((ICommand) PropertyValueEditorCommands.CommitTransaction, new ExecutedRoutedEventHandler(this.OnCommitTransaction));
        element.CommandBindings.Add(commandBindingHolder.AbortCommandBinding);
        element.CommandBindings.Add(commandBindingHolder.CommitCommandBinding);
        this.commandBindingReferences.Add(element, commandBindingHolder);
      }
      ++commandBindingHolder.RefCount;
    }

    private FrameworkElement FindKeyForElementGivenEventType(FrameworkElement sourceElement, RoutingStrategy routingStrategy)
    {
      if (routingStrategy != RoutingStrategy.Direct)
      {
        foreach (FrameworkElement frameworkElement in this.commandBindingReferences.Keys)
        {
          if (routingStrategy == RoutingStrategy.Bubble && frameworkElement.IsAncestorOf((DependencyObject) sourceElement) || routingStrategy == RoutingStrategy.Tunnel && frameworkElement.IsDescendantOf((DependencyObject) sourceElement))
            return frameworkElement;
        }
      }
      return sourceElement;
    }

    private void UnhookEditEndingHandlers(FrameworkElement element)
    {
      PropertyEditingHelper.CommandBindingHolder commandBindingHolder;
      if (!this.commandBindingReferences.TryGetValue(element, out commandBindingHolder))
        return;
      --commandBindingHolder.RefCount;
      if (commandBindingHolder.RefCount != 0)
        return;
      element.CommandBindings.Remove(commandBindingHolder.AbortCommandBinding);
      element.CommandBindings.Remove(commandBindingHolder.CommitCommandBinding);
      this.commandBindingReferences.Remove(element);
    }

    private void OnAbortOrCommitTransaction(object sender, ExecutedRoutedEventArgs eventArgs)
    {
      if (this.ActiveDocument == null)
        return;
      FrameworkElement frameworkElement = eventArgs.OriginalSource as FrameworkElement;
      if (frameworkElement != null && this.host != null && this.host.IsAncestorOf((DependencyObject) frameworkElement))
        this.OnAbortTransaction(sender, eventArgs);
      else
        this.OnCommitTransaction(sender, eventArgs);
    }

    private void OnBeginTransaction(object sender, ExecutedRoutedEventArgs eventArgs)
    {
      this.OnBeginTransactionCore(eventArgs.OriginalSource, eventArgs.Parameter);
      eventArgs.Handled = true;
    }

    private void OnBeginTransaction(ExpressionValueEditorCommandArgs commandArgs)
    {
      this.OnBeginTransactionCore(commandArgs != null ? (object) commandArgs.InputElement : (object) (IInputElement) null, commandArgs != null ? commandArgs.Parameter : (object) null);
    }

    private void OnBeginTransactionCore(object originalSource, object parameter)
    {
      if (this.ActiveDocument == null)
        return;
      FrameworkElement element = originalSource as FrameworkElement;
      if (element != null)
        this.HookEditEndingHandlers(element);
      string description = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.PropertyChangeUndoDescription, new object[1]
      {
        (object) "Property"
      });
      string str = parameter as string;
      PropertyTransactionParameters transactionParameters = parameter as PropertyTransactionParameters;
      SceneEditTransactionType transactionType = SceneEditTransactionType.AutoCommitting;
      if (transactionParameters != null)
      {
        description = transactionParameters.TransactionDescription;
        transactionType = transactionParameters.TransactionType;
      }
      else if (str != null)
      {
        description = str;
      }
      else
      {
        PropertyContainer propertyContainer;
        if ((propertyContainer = PropertyEditingHelper.GetPropertyContainer(originalSource)) != null)
          description = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.PropertyChangeUndoDescription, new object[1]
          {
            (object) propertyContainer.PropertyEntry.PropertyName
          });
      }
      this.editTransactions.Push(this.ActiveDocument.CreateEditTransaction(description, false, transactionType));
    }

    private void OnCommitTransaction(ExpressionValueEditorCommandArgs commandArgs)
    {
      this.OnCommitTransactionCore(commandArgs.InputElement as FrameworkElement, RoutingStrategy.Direct);
    }

    private void OnCommitTransaction(object sender, ExecutedRoutedEventArgs eventArgs)
    {
      if (this.ActiveDocument == null)
        return;
      this.OnCommitTransactionCore(eventArgs.OriginalSource as FrameworkElement, eventArgs.RoutedEvent.RoutingStrategy);
      eventArgs.Handled = true;
    }

    private void OnCommitTransactionCore(FrameworkElement sourceElement, RoutingStrategy routingStrategy)
    {
      if (sourceElement != null)
        this.UnhookEditEndingHandlers(this.FindKeyForElementGivenEventType(sourceElement, routingStrategy));
      if (this.CurrentTransaction == null)
        return;
      this.CurrentTransaction.Commit();
      this.editTransactions.Pop();
    }

    private void OnAbortTransaction(ExpressionValueEditorCommandArgs commandArgs)
    {
      this.OnAbortTransactionCore(commandArgs.InputElement as FrameworkElement, RoutingStrategy.Direct);
    }

    private void OnAbortTransaction(object sender, ExecutedRoutedEventArgs eventArgs)
    {
      if (this.ActiveDocument == null)
        return;
      this.OnAbortTransactionCore(eventArgs.OriginalSource as FrameworkElement, eventArgs.RoutedEvent.RoutingStrategy);
      eventArgs.Handled = true;
    }

    private void OnAbortTransactionCore(FrameworkElement sourceElement, RoutingStrategy routingStrategy)
    {
      if (sourceElement != null)
        this.UnhookEditEndingHandlers(this.FindKeyForElementGivenEventType(sourceElement, routingStrategy));
      if (this.CurrentTransaction != null)
      {
        this.CurrentTransaction.Cancel();
        this.UpdateTransaction();
        this.editTransactions.Pop();
      }
      if (this.editTransactions.Count <= 0)
        return;
      this.UpdateTransaction();
    }

    private void OnShowInlineEditor(object sender, ExecutedRoutedEventArgs eventArgs)
    {
      this.SwitchPropertyContainerMode(eventArgs, PropertyContainerEditMode.Inline);
    }

    private void OnShowExtendedPopupEditor(object sender, ExecutedRoutedEventArgs eventArgs)
    {
      this.SwitchPropertyContainerMode(eventArgs, PropertyContainerEditMode.ExtendedPopup);
    }

    private void OnShowExtendedPinnedEditor(object sender, ExecutedRoutedEventArgs eventArgs)
    {
      this.SwitchPropertyContainerMode(eventArgs, PropertyContainerEditMode.ExtendedPinned);
    }

    private void OnShowDialogEditor(object sender, ExecutedRoutedEventArgs eventArgs)
    {
      this.SwitchPropertyContainerMode(eventArgs, PropertyContainerEditMode.Dialog);
    }

    private void OnCanOpenDialogWindow(object sender, CanExecuteRoutedEventArgs eventArgs)
    {
      DataTemplate dialogEditorTemplate = PropertyEditingHelper.GetDialogEditorTemplate(PropertyEditingHelper.GetEditedProperty(eventArgs.OriginalSource, eventArgs.Parameter));
      eventArgs.CanExecute = dialogEditorTemplate != null;
      eventArgs.Handled = true;
    }

    private void OnOpenDialogWindow(object sender, ExecutedRoutedEventArgs eventArgs)
    {
      PropertyReferenceProperty referenceProperty = (PropertyReferenceProperty) PropertyEditingHelper.GetEditedProperty(eventArgs.OriginalSource, eventArgs.Parameter);
      DataTemplate dialogEditorTemplate = PropertyEditingHelper.GetDialogEditorTemplate((PropertyEntry) referenceProperty);
      if (dialogEditorTemplate == null)
        return;
      Dialog dialog = (Dialog) new DialogValueEditorHost(referenceProperty.PropertyValue, dialogEditorTemplate);
      int remaining = 0;
      SceneEditTransaction sceneEditTransaction = (SceneEditTransaction) null;
      if (this.ActiveDocument != null)
      {
        this.CommitOutstandingTransactions(0);
        sceneEditTransaction = this.ActiveDocument.CreateEditTransaction(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.DialogValueEditorUndoDescription, new object[1]
        {
          (object) referenceProperty.DisplayName
        }));
        this.editTransactions.Push(sceneEditTransaction);
        remaining = this.editTransactions.Count;
      }
      bool? nullable = dialog.ShowDialog();
      if (this.ActiveDocument != null)
      {
        this.CommitOutstandingTransactions(remaining);
        if (nullable.HasValue && nullable.Value)
          sceneEditTransaction.Commit();
        else
          sceneEditTransaction.Cancel();
        this.editTransactions.Pop();
      }
      eventArgs.Handled = true;
    }

    private static PropertyEntry GetEditedProperty(object showDialogCommandSource, object commandParamater)
    {
      PropertyEntry propertyEntry = commandParamater as PropertyEntry;
      if (propertyEntry != null)
        return propertyEntry;
      DependencyObject dependencyObject = showDialogCommandSource as DependencyObject;
      if (dependencyObject != null)
      {
        PropertyContainer propertyContainer = PropertyContainer.GetOwningPropertyContainer(dependencyObject);
        if (propertyContainer != null)
          return propertyContainer.PropertyEntry;
      }
      return (PropertyEntry) null;
    }

    private static DataTemplate GetDialogEditorTemplate(PropertyEntry property)
    {
      if (property == null)
        return (DataTemplate) null;
      DialogPropertyValueEditor propertyValueEditor = property.PropertyValueEditor as DialogPropertyValueEditor;
      if (propertyValueEditor == null)
        return (DataTemplate) null;
      return propertyValueEditor.DialogEditorTemplate;
    }

    private void SwitchPropertyContainerMode(ExecutedRoutedEventArgs eventArgs, PropertyContainerEditMode newMode)
    {
      PropertyContainer propertyContainer = PropertyEditingHelper.GetPropertyContainer(eventArgs.OriginalSource);
      if (propertyContainer == null)
        return;
      propertyContainer.ActiveEditMode = newMode;
      eventArgs.Handled = true;
    }

    private class CommandBindingHolder
    {
      public CommandBinding AbortCommandBinding { get; set; }

      public CommandBinding CommitCommandBinding { get; set; }

      public int RefCount { get; set; }
    }
  }
}
