// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Text.TextToolBehavior
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.Tools.Text
{
  internal class TextToolBehavior : ElementCreateBehavior
  {
    private List<TextEditProxy> nonActiveEditProxies = new List<TextEditProxy>();
    private bool isCreatingText;
    private SceneElement selectedElementBeforeTextCreation;
    private ToolBehavior createToolBehavior;
    private bool isDropping;
    private TextEditProxy editProxy;
    private bool textChangesApplied;

    public override bool ShouldCapture
    {
      get
      {
        return false;
      }
    }

    public override bool UseDefaultEditingAdorners
    {
      get
      {
        return false;
      }
    }

    public BaseFrameworkElement TextSource
    {
      get
      {
        if (this.editProxy != null)
          return this.editProxy.TextSource;
        return (BaseFrameworkElement) null;
      }
    }

    public IViewTextBoxBase EditingElement
    {
      get
      {
        if (this.editProxy != null)
          return this.editProxy.EditingElement;
        return (IViewTextBoxBase) null;
      }
    }

    public TextToolBehavior(ToolBehaviorContext toolContext, ToolBehavior createToolBehavior)
      : base(toolContext)
    {
      this.createToolBehavior = createToolBehavior;
    }

    public bool TryEnterTextEditMode()
    {
      if (this.editProxy != null)
        return false;
      this.EditDifferentElement(this.Tool.ActiveSceneViewModel.ElementSelectionSet.PrimarySelection as BaseFrameworkElement);
      return true;
    }

    protected override void OnAttach()
    {
      base.OnAttach();
      this.ActiveSceneViewModel.RefreshSelection();
      this.ActiveSceneViewModel.EarlySceneUpdatePhase += new SceneUpdatePhaseEventHandler(this.ViewModel_EarlySceneUpdatePhase);
      this.ActiveSceneViewModel.LateSceneUpdatePhase += new SceneUpdatePhaseEventHandler(this.ViewModel_LateSceneUpdatePhase);
      if (this.createToolBehavior != null)
        return;
      this.EditDifferentElement(this.Tool.ActiveSceneViewModel.ElementSelectionSet.PrimarySelection as BaseFrameworkElement);
      if (this.editProxy == null)
        return;
      this.editProxy.EditingElement.Focus();
      this.editProxy.EditingElement.SelectAll();
    }

    protected override void OnResume()
    {
      if (this.isCreatingText)
      {
        SceneElement primarySelection = this.Tool.ActiveSceneViewModel.ElementSelectionSet.PrimarySelection;
        if ((this.editProxy == null || this.editProxy.TextSource != primarySelection) && (primarySelection != null && TextEditProxyFactory.IsEditableElement(primarySelection)) && this.selectedElementBeforeTextCreation != primarySelection)
        {
          this.EditDifferentElement(primarySelection as BaseFrameworkElement);
          if (this.ActiveSceneViewModel.TextSelectionSet.TextEditProxy != null)
            this.ActiveSceneViewModel.TextSelectionSet.TextEditProxy.EditingElement.SelectAll();
        }
        else
          this.EditDifferentElement((BaseFrameworkElement) null);
        this.isCreatingText = false;
      }
      base.OnResume();
    }

    protected override void OnDetach()
    {
      this.EditDifferentElement((BaseFrameworkElement) null);
      this.ClearNonActiveEditProxies();
      this.ActiveSceneViewModel.EarlySceneUpdatePhase -= new SceneUpdatePhaseEventHandler(this.ViewModel_EarlySceneUpdatePhase);
      this.ActiveSceneViewModel.LateSceneUpdatePhase -= new SceneUpdatePhaseEventHandler(this.ViewModel_LateSceneUpdatePhase);
      this.ActiveSceneViewModel.RefreshSelection();
      base.OnDetach();
    }

    protected override bool OnButtonDownOverNonAdorner(Point pointerPosition)
    {
      BaseFrameworkElement textElementAtPoint = this.GetEditableTextElementAtPoint(pointerPosition);
      if (textElementAtPoint != null)
      {
        if (textElementAtPoint != this.TextSource)
          this.EditDifferentElement(textElementAtPoint);
        return false;
      }
      if (this.createToolBehavior != null)
      {
        this.PushBehavior(this.createToolBehavior);
        this.isCreatingText = true;
        this.selectedElementBeforeTextCreation = this.Tool.ActiveSceneViewModel.ElementSelectionSet.PrimarySelection;
      }
      else
        this.PopSelfOrExitEditMode();
      return true;
    }

    protected override bool OnDrop(DragEventArgs args)
    {
      BaseFrameworkElement textElementAtPoint = this.GetEditableTextElementAtPoint(args.GetPosition((IInputElement) this.ActiveView.ViewRootContainer));
      if (textElementAtPoint != null)
      {
        if (textElementAtPoint != this.TextSource)
        {
          this.CommitCurrentEdit();
          if (this.editProxy != null)
            this.UnregisterEditingHandlers(this.editProxy);
          this.isDropping = true;
          Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Send, (Delegate) new DispatcherOperationCallback(this.OnDragDropDelayed), (object) this.editProxy);
        }
        TextEditProxy editingProxy = (TextEditProxy) null;
        foreach (TextEditProxy textEditProxy in this.nonActiveEditProxies)
        {
          if (textEditProxy.TextSource == textElementAtPoint)
            editingProxy = textEditProxy;
        }
        if (editingProxy != null)
        {
          this.nonActiveEditProxies.Remove(editingProxy);
          this.RegisterEditingHandlers(editingProxy);
          this.editProxy = editingProxy;
        }
      }
      return false;
    }

    protected override bool OnDragOver(DragEventArgs args)
    {
      base.OnDragOver(args);
      if (!this.IsSuspended)
      {
        BaseFrameworkElement textElementAtPoint = this.GetEditableTextElementAtPoint(args.GetPosition((IInputElement) this.ActiveView.ViewRootContainer));
        if (textElementAtPoint != null)
        {
          if (textElementAtPoint != this.TextSource)
          {
            bool flag = false;
            foreach (TextEditProxy textEditProxy in this.nonActiveEditProxies)
            {
              if (textEditProxy.TextSource == textElementAtPoint)
              {
                flag = true;
                break;
              }
            }
            if (!flag)
            {
              TextEditProxy editProxy = TextEditProxyFactory.CreateEditProxy(textElementAtPoint);
              this.AddEditProxyToScene(editProxy, true);
              this.nonActiveEditProxies.Add(editProxy);
            }
          }
        }
        else
          this.ClearNonActiveEditProxies();
      }
      return false;
    }

    protected override bool OnKey(KeyEventArgs args)
    {
      if (this.TextSource == null)
        return base.OnKey(args);
      if (args.Key == Key.Escape)
      {
        this.PopSelfOrExitEditMode();
        return true;
      }
      if (args.Key == Key.F5)
      {
        this.PopSelfOrExitEditMode();
        return false;
      }
      if (args.Key != Key.Return || !args.IsDown || (args.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.None)
        return false;
      this.PopSelfOrExitEditMode();
      return true;
    }

    public override void CommitCurrentEdit()
    {
      if (this.isDropping || this.editProxy == null || (this.editProxy.EditingElement == null || !this.editProxy.EditingElement.CheckAccess()) || this.editProxy.EditingElement.IsUndoEnabled && !this.editProxy.EditingElement.CanUndo)
        return;
      this.editProxy.Serialize();
      this.editProxy.UpdateDocumentModel();
      this.editProxy.EditingElement.IsUndoEnabled = false;
      this.editProxy.EditingElement.IsUndoEnabled = true;
    }

    protected override bool OnRightButtonDown(Point pointerPosition)
    {
      BaseFrameworkElement textElementAtPoint = this.GetEditableTextElementAtPoint(pointerPosition);
      if (textElementAtPoint != null)
      {
        if (textElementAtPoint == this.TextSource)
          return false;
        this.EditDifferentElement(textElementAtPoint);
      }
      this.PopSelfOrExitEditMode();
      return false;
    }

    protected override bool OnHoverOverAdorner(IAdorner adorner)
    {
      return base.OnHoverOverAdorner(adorner);
    }

    protected override bool OnHoverOverNonAdorner(Point pointerPosition)
    {
      BaseFrameworkElement textElementAtPoint = this.GetEditableTextElementAtPoint(pointerPosition);
      if (textElementAtPoint != null)
      {
        this.FindOrCreateEditProxy(textElementAtPoint, false);
        this.Cursor = Cursors.IBeam;
        this.PreviewInsertionPoint = (ISceneInsertionPoint) null;
        return true;
      }
      this.ClearNonActiveEditProxies();
      return base.OnHoverOverNonAdorner(pointerPosition);
    }

    public void PopSelfOrExitEditMode()
    {
      if (this.TryPopSelf())
        return;
      this.EditDifferentElement((BaseFrameworkElement) null);
    }

    private void ViewModel_EarlySceneUpdatePhase(object sender, SceneUpdatePhaseEventArgs args)
    {
      if (this.IsSuspended)
        return;
      if (args.IsDirtyViewState(SceneViewModel.ViewStateBits.ElementSelection))
      {
        SceneElementSelectionSet elementSelectionSet = this.ActiveSceneViewModel.ElementSelectionSet;
        BaseFrameworkElement frameworkElement = (BaseFrameworkElement) null;
        if (elementSelectionSet != null && elementSelectionSet.PrimarySelection != null && TextEditProxyFactory.IsEditableElement(elementSelectionSet.PrimarySelection))
          frameworkElement = elementSelectionSet.PrimarySelection as BaseFrameworkElement;
        if (this.TextSource != frameworkElement)
        {
          this.PopSelfOrExitEditMode();
          return;
        }
      }
      if (this.editProxy == null)
        return;
      if (args.IsDirtyViewState(SceneViewModel.ViewStateBits.EntireScene))
      {
        foreach (DocumentNodeChange change in args.DocumentChanges.DistinctChanges)
        {
          if ((change.ParentNode == this.TextSource.DocumentNode || change.OldChildNode == this.TextSource.DocumentNode) && !this.editProxy.IsTextChange(change))
          {
            this.PopSelfOrExitEditMode();
            return;
          }
        }
      }
      foreach (DocumentNodeChange change in args.DocumentChanges.DistinctChanges)
      {
        if ((change.ParentNode == this.TextSource.DocumentNode || change.OldChildNode == this.TextSource.DocumentNode) && this.editProxy.IsTextChange(change))
        {
          this.editProxy.ApplyPropertyChange(change);
          this.textChangesApplied = true;
        }
      }
      if (!this.editProxy.TextSource.IsViewObjectValid)
        return;
      this.SetIsTextEditingProperty(this.editProxy.TextSource, true);
    }

    private void ViewModel_LateSceneUpdatePhase(object sender, SceneUpdatePhaseEventArgs args)
    {
      if (!this.textChangesApplied)
        return;
      if (this.editProxy != null)
        this.editProxy.UpdateEditingElementLayout();
      this.textChangesApplied = false;
    }

    private void SetIsTextEditingProperty(BaseFrameworkElement textSource, bool value)
    {
      IViewObject visual = textSource.Visual;
      if (visual == null)
        return;
      (textSource.ProjectContext.ResolveProperty(DesignTimeProperties.IsTextEditingProperty) as DependencyPropertyReferenceStep).SetValue(visual.PlatformSpecificObject, (object) (bool) (value ? true : false));
    }

    private BaseFrameworkElement GetEditableTextElementAtPoint(Point point)
    {
      SceneElement elementAtPoint = this.ActiveView.GetElementAtPoint(point, new HitTestModifier(this.GetContainingTextElement), new InvisibleObjectHitTestModifier(SceneView.SmartInvisiblePanelSelect), (ICollection<BaseFrameworkElement>) null);
      BaseFrameworkElement frameworkElement = (BaseFrameworkElement) null;
      if (elementAtPoint != null && TextEditProxyFactory.IsEditableElement(elementAtPoint) && !elementAtPoint.IsLocked)
        frameworkElement = elementAtPoint as BaseFrameworkElement;
      return frameworkElement;
    }

    private SceneElement GetContainingTextElement(DocumentNodePath nodePath)
    {
      if (this.ActiveSceneViewModel == null || this.ActiveSceneViewModel.ActiveEditingContainer == null)
        return (SceneElement) null;
      DocumentNodePath editingContainer = this.ActiveSceneViewModel.GetAncestorInEditingContainer(nodePath, this.ActiveSceneViewModel.ActiveEditingContainer.DocumentNodePath, (DocumentNodePath) null);
      SceneElement sceneElement = (SceneElement) null;
      if (editingContainer != null)
        sceneElement = this.ActiveSceneViewModel.GetSceneNode(editingContainer.Node) as SceneElement;
      for (; sceneElement != null; sceneElement = sceneElement.ParentElement)
      {
        if (sceneElement.IsVisuallySelectable || sceneElement == this.ActiveSceneViewModel.ActiveEditingContainer)
        {
          if (!TextEditProxyFactory.IsEditableElement(sceneElement))
          {
            sceneElement = (SceneElement) null;
            break;
          }
          break;
        }
      }
      return sceneElement;
    }

    private TextEditProxy FindOrCreateEditProxy(BaseFrameworkElement textElement, bool active)
    {
      if (this.editProxy != null && this.editProxy.TextSource == textElement)
        return this.editProxy;
      TextEditProxy textEditProxy = Enumerable.FirstOrDefault<TextEditProxy>((IEnumerable<TextEditProxy>) this.nonActiveEditProxies, (Func<TextEditProxy, bool>) (proxy => proxy.TextSource == textElement));
      if (textEditProxy == null)
      {
        textEditProxy = TextEditProxyFactory.CreateEditProxy(textElement);
        this.AddEditProxyToScene(textEditProxy, active);
        if (active)
          this.editProxy = textEditProxy;
        else
          this.nonActiveEditProxies.Add(textEditProxy);
      }
      else if (active)
      {
        this.nonActiveEditProxies.Remove(textEditProxy);
        this.editProxy = textEditProxy;
        textEditProxy.EditingElement.Opacity = 1.0;
        if (textEditProxy.TextSource.IsViewObjectValid)
          this.SetIsTextEditingProperty(textEditProxy.TextSource, true);
      }
      return textEditProxy;
    }

    private void BeginTextEdit(BaseFrameworkElement textElement)
    {
      this.FindOrCreateEditProxy(textElement, true);
      if (this.editProxy == null)
        return;
      IViewTextBoxBase editingElement = this.editProxy.EditingElement;
      if (this.isCreatingText)
      {
        editingElement.Focus();
        editingElement.SelectAll();
      }
      this.RegisterEditingHandlers(this.editProxy);
    }

    private void AddEditProxyToScene(TextEditProxy textEditProxy, bool visible)
    {
      if (visible && textEditProxy.TextSource.IsViewObjectValid)
        this.SetIsTextEditingProperty(textEditProxy.TextSource, true);
      textEditProxy.EditingElement.GotKeyboardFocus += new KeyboardFocusChangedEventHandler(this.editingTextBox_GotKeyboardFocus);
      textEditProxy.AddToScene(visible);
    }

    private void editingTextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
      TextEditProxy textEditProxy = (TextEditProxy) ((IViewTextBoxBase) sender).TextEditProxyObject;
      if (textEditProxy == this.editProxy)
        return;
      this.EditDifferentElement(textEditProxy.TextSource);
    }

    private void RemoveEditProxyFromScene(TextEditProxy textEditProxy)
    {
      textEditProxy.RemoveFromScene();
      textEditProxy.EditingElement.GotKeyboardFocus -= new KeyboardFocusChangedEventHandler(this.editingTextBox_GotKeyboardFocus);
      if (textEditProxy.TextSource.DocumentNode.DocumentRoot == null || !textEditProxy.TextSource.IsViewObjectValid)
        return;
      this.SetIsTextEditingProperty(textEditProxy.TextSource, false);
    }

    private void editingTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
      PerformanceUtility.MeasurePerformanceUntilRender(PerformanceEvent.TextInsertLetter);
    }

    private void editingTextBox_SelectionChanged(object sender, RoutedEventArgs e)
    {
      this.ActiveSceneViewModel.TextSelectionSet.SetSelection(this.editProxy.TextSelection);
      this.ActiveSceneViewModel.RefreshCurrentValues();
    }

    private void editingTextBox_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
      this.CommitCurrentEdit();
    }

    private void editingTextBox_LostFocus(object sender, RoutedEventArgs e)
    {
      this.CommitCurrentEdit();
    }

    private void editingTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      this.ActiveDocument.SourceChanged();
      this.ActiveSceneViewModel.TextSelectionSet.SetSelection(this.editProxy.TextSelection);
    }

    private object OnDragDropDelayed(object arg)
    {
      this.isDropping = false;
      TextEditProxy textEditProxy = (TextEditProxy) arg;
      if (textEditProxy != null)
      {
        using (SceneEditTransaction editTransaction = this.Tool.ActiveSceneViewModel.CreateEditTransaction(StringTable.UndoUnitDragDropText))
        {
          if (this.editProxy != null)
          {
            this.ActiveSceneViewModel.ElementSelectionSet.SetSelection((SceneElement) this.editProxy.TextSource);
            TextSelectionSet textSelectionSet = this.ActiveSceneViewModel.TextSelectionSet;
            textSelectionSet.TextEditProxy = this.editProxy;
            textSelectionSet.IsActive = true;
          }
          textEditProxy.Serialize();
          textEditProxy.UpdateDocumentModel();
          this.RemoveEditProxyFromScene(textEditProxy);
          this.CommitCurrentEdit();
          editTransaction.Commit();
          if (this.editProxy != null)
          {
            if (this.editProxy.TextSource.IsViewObjectValid)
              this.SetIsTextEditingProperty(this.editProxy.TextSource, true);
          }
        }
      }
      return (object) null;
    }

    private void RegisterEditingHandlers(TextEditProxy editingProxy)
    {
      IViewTextBoxBase editingElement = editingProxy.EditingElement;
      editingElement.PreviewTextInput += new TextCompositionEventHandler(this.editingTextBox_PreviewTextInput);
      editingElement.TextChanged += new TextChangedEventHandler(this.editingTextBox_TextChanged);
      editingElement.SelectionChanged += new RoutedEventHandler(this.editingTextBox_SelectionChanged);
      editingElement.LostFocus += new RoutedEventHandler(this.editingTextBox_LostFocus);
      editingElement.PreviewLostKeyboardFocus += new KeyboardFocusChangedEventHandler(this.editingTextBox_PreviewLostKeyboardFocus);
    }

    private void UnregisterEditingHandlers(TextEditProxy editingProxy)
    {
      editingProxy.EditingElement.PreviewTextInput -= new TextCompositionEventHandler(this.editingTextBox_PreviewTextInput);
      editingProxy.EditingElement.TextChanged -= new TextChangedEventHandler(this.editingTextBox_TextChanged);
      editingProxy.EditingElement.SelectionChanged -= new RoutedEventHandler(this.editingTextBox_SelectionChanged);
      editingProxy.EditingElement.LostFocus -= new RoutedEventHandler(this.editingTextBox_LostFocus);
      editingProxy.EditingElement.PreviewLostKeyboardFocus -= new KeyboardFocusChangedEventHandler(this.editingTextBox_PreviewLostKeyboardFocus);
    }

    private void EndTextEdit()
    {
      if (this.editProxy == null)
        return;
      this.ActiveSceneViewModel.TextSelectionSet.Clear();
      this.CommitCurrentEdit();
      this.UnregisterEditingHandlers(this.editProxy);
      this.RemoveEditProxyFromScene(this.editProxy);
      this.editProxy = (TextEditProxy) null;
    }

    private void ClearNonActiveEditProxies()
    {
      foreach (TextEditProxy textEditProxy in this.nonActiveEditProxies)
        this.RemoveEditProxyFromScene(textEditProxy);
      this.nonActiveEditProxies.Clear();
    }

    private void EditDifferentElement(BaseFrameworkElement element)
    {
      this.EndTextEdit();
      if (element != null && !TextEditProxyFactory.IsEditableElement((SceneElement) element))
        element = (BaseFrameworkElement) null;
      bool flag1 = false;
      if (element != null && element.IsViewObjectValid)
      {
        SceneElementSelectionSet elementSelectionSet = this.ActiveSceneViewModel.ElementSelectionSet;
        if (elementSelectionSet != null)
          elementSelectionSet.SetSelection((SceneElement) element);
        flag1 = true;
        this.BeginTextEdit(element);
      }
      TextSelectionSet textSelectionSet = this.ActiveSceneViewModel.TextSelectionSet;
      bool flag2 = false;
      if (textSelectionSet != null)
      {
        flag2 = this.ActiveSceneViewModel.TextSelectionSet.IsActive;
        int num = flag1 ? true : false;
        textSelectionSet.TextEditProxy = this.editProxy;
        textSelectionSet.IsActive = flag1;
      }
      if (flag1 != flag2)
        this.ActiveSceneViewModel.RefreshSelection();
      if (flag1)
        return;
      this.ActiveView.ReturnFocus();
    }
  }
}
