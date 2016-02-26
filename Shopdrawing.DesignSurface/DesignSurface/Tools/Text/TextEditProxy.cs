// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Text.TextEditProxy
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Geometry.Core;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.UserInterface;
using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Text
{
  public abstract class TextEditProxy
  {
    private BaseFrameworkElement textSource;

    public IPlatform ProxyPlatform { get; protected set; }

    public BaseFrameworkElement TextSource
    {
      get
      {
        return this.textSource;
      }
    }

    public abstract bool SupportsRangeProperties { get; }

    public abstract bool SupportsParagraphProperties { get; }

    public abstract IViewTextBoxBase EditingElement { get; }

    public abstract DesignTimeTextSelection TextSelection { get; }

    public bool ForceLoadOnInstantiate { get; set; }

    protected ITypeResolver TypeResolver
    {
      get
      {
        return this.textSource.DocumentNode.TypeResolver;
      }
    }

    protected TextEditProxy(BaseFrameworkElement textSource)
    {
      this.textSource = textSource;
    }

    private void DisableTabHandler(object sender, KeyEventArgs e)
    {
      if (e.Key != Key.Tab)
        return;
      e.Handled = true;
    }

    public virtual void Instantiate()
    {
      if (!this.TextSource.ViewModel.ProjectContext.IsCapabilitySet(PlatformCapability.SupportsTabInTextControl))
        this.EditingElement.KeyDown += new KeyEventHandler(this.DisableTabHandler);
      else
        this.EditingElement.AcceptsTab = true;
      this.EditingElement.SetValue(this.ProxyPlatform.Metadata.DefaultTypeResolver, DesignTimeProperties.IsTextEditProxyProperty, (object) true);
      this.EditingElement.IsSpellCheckEnabled = true;
      this.EditingElement.IsUndoEnabled = true;
      this.EditingElement.IsEnabled = true;
      this.EditingElement.AcceptsReturn = true;
      this.EditingElement.IsInputMethodEnabled = true;
      if (this.TextSource.ProjectContext.IsCapabilitySet(PlatformCapability.IsWpf) || !PlatformTypes.ContentControl.IsAssignableFrom((ITypeId) this.TextSource.Type))
      {
        this.CopyProperty(ControlElement.BackgroundProperty);
        this.CopyProperty(ControlElement.ForegroundProperty);
      }
      if (!this.TextSource.ProjectContext.IsCapabilitySet(PlatformCapability.IsWpf) && PlatformTypes.TextBlock.IsAssignableFrom((ITypeId) this.TextSource.Type))
      {
        TextBoxBase textBoxBase = this.EditingElement.PlatformSpecificObject as TextBoxBase;
        if (textBoxBase != null)
          textBoxBase.Background = (Brush) Brushes.Transparent;
      }
      IProperty property1 = this.TextSource.ProjectContext.ResolveProperty(TextOptionsProperties.TextFormattingModeProperty);
      if (property1 != null)
      {
        object computedValueAsWpf = this.TextSource.GetComputedValueAsWpf((IPropertyId) property1);
        TextBoxBase textBoxBase = this.EditingElement.PlatformSpecificObject as TextBoxBase;
        if (textBoxBase != null && computedValueAsWpf is TextFormattingMode)
          TextOptions.SetTextFormattingMode((DependencyObject) textBoxBase, (TextFormattingMode) computedValueAsWpf);
      }
      IProperty property2 = this.TextSource.ProjectContext.ResolveProperty(TextOptionsProperties.TextHintingModeProperty);
      if (property2 != null)
      {
        object computedValueAsWpf = this.TextSource.GetComputedValueAsWpf((IPropertyId) property2);
        TextBoxBase textBoxBase = this.EditingElement.PlatformSpecificObject as TextBoxBase;
        if (textBoxBase != null && computedValueAsWpf is TextHintingMode)
          TextOptions.SetTextHintingMode((DependencyObject) textBoxBase, (TextHintingMode) computedValueAsWpf);
      }
      this.CopyProperty(ControlElement.FontFamilyProperty);
      this.CopyProperty(ControlElement.FontSizeProperty);
      this.CopyProperty(ControlElement.BorderBrushProperty);
      this.CopyProperty(ControlElement.BorderThicknessProperty);
      this.CopyProperty(ControlElement.FontStretchProperty);
      this.CopyProperty(ControlElement.FontStyleProperty);
      this.CopyProperty(ControlElement.FontWeightProperty);
      this.CopyProperty(Base2DElement.OpacityProperty);
      this.CopyProperty(ControlElement.PaddingProperty);
      if (this.TextSource.ProjectContext.IsCapabilitySet(PlatformCapability.IsWpf))
        this.CopyProperty(DesignTimeProperties.InstanceBuilderContextProperty);
      else
        this.EditingElement.InstanceBuilderContext = (object) new WeakReference((object) this.textSource.ViewModel.DefaultView.InstanceBuilderContext);
      this.DisableUndo();
    }

    public abstract void Serialize();

    public abstract void UpdateDocumentModel();

    public virtual bool IsTextChange(DocumentNodeChange change)
    {
      if (!DesignTimeProperties.IsTextEditingProperty.Equals((object) change.PropertyKey))
        return this.IsTextPropertyChange(change);
      return true;
    }

    public virtual void ApplyPropertyChange(DocumentNodeChange change)
    {
      if (!this.IsTextPropertyChange(change))
        return;
      DependencyPropertyReferenceStep propertyReferenceStep = change.PropertyKey as DependencyPropertyReferenceStep;
      if (propertyReferenceStep == null)
        return;
      this.CopyProperty(this.TextSource.DesignerContext.PlatformConverter.ConvertToWpfPropertyKey((IProperty) propertyReferenceStep));
    }

    protected bool IsTextPropertyChange(DocumentNodeChange change)
    {
      if (change.PropertyKey == null)
        return false;
      if (!change.PropertyKey.Equals((object) this.ResolveProperty(ControlElement.FontFamilyProperty)) && !change.PropertyKey.Equals((object) this.ResolveProperty(ControlElement.FontSizeProperty)) && (!change.PropertyKey.Equals((object) this.ResolveProperty(ControlElement.FontWeightProperty)) && !change.PropertyKey.Equals((object) this.ResolveProperty(ControlElement.FontStyleProperty))) && (!change.PropertyKey.Equals((object) this.ResolveProperty(TextBoxElement.TextDecorationsProperty)) && !change.PropertyKey.Equals((object) this.ResolveProperty(ControlElement.ForegroundProperty)) && (!change.PropertyKey.Equals((object) this.ResolveProperty(ControlElement.BackgroundProperty)) && !change.PropertyKey.Equals((object) this.ResolveProperty(TextBoxElement.TextAlignmentProperty)))) && !change.PropertyKey.Equals((object) this.ResolveProperty(TextBlockElement.TextAlignmentProperty)))
        return change.PropertyKey.Equals((object) this.ResolveProperty(TextBlockElement.LineHeightProperty));
      return true;
    }

    protected IProperty ResolveProperty(IPropertyId propertyId)
    {
      IProperty property = this.TextSource.ProjectContext.ResolveProperty(propertyId);
      ReferenceStep referenceStep = property as ReferenceStep;
      if (referenceStep != null)
        property = (IProperty) this.TextSource.DesignerContext.PropertyManager.FilterProperty((SceneNode) this.TextSource, referenceStep);
      return property;
    }

    public abstract void DeleteSelection();

    public abstract void SelectNone();

    protected virtual void CopyProperty(IPropertyId propertyId)
    {
      DependencyPropertyReferenceStep propertyReferenceStep = this.TextSource.DesignerContext.DesignerDefaultPlatformService.DefaultPlatform.Metadata.ResolveProperty(propertyId) as DependencyPropertyReferenceStep;
      ReferenceStep referenceStep1 = PlatformTypes.IsExpressionInteractiveType(PlatformTypeHelper.GetDeclaringType((IMember) propertyReferenceStep)) ? (ReferenceStep) propertyReferenceStep : PlatformTypeHelper.GetProperty((ITypeResolver) this.TextSource.ProjectContext, (ITypeId) this.TextSource.Type, MemberType.LocalProperty, propertyReferenceStep.Name);
      if (referenceStep1 == null)
        return;
      object computedValueAsWpf = this.TextSource.GetComputedValueAsWpf((IPropertyId) referenceStep1);
      if (!this.TextSource.ProjectContext.IsCapabilitySet(PlatformCapability.IsWpf))
      {
        if (DesignTimeProperties.GetShadowProperty((IProperty) referenceStep1, (ITypeId) this.TextSource.Type) != null)
        {
          for (SceneNode sceneNode = (SceneNode) this.TextSource; sceneNode != null; sceneNode = sceneNode.Parent)
          {
            IProperty property = PlatformTypes.IsExpressionInteractiveType(PlatformTypeHelper.GetDeclaringType((IMember) referenceStep1)) ? (IProperty) referenceStep1 : (IProperty) PlatformTypeHelper.GetProperty((ITypeResolver) this.TextSource.ProjectContext, (ITypeId) sceneNode.Type, MemberType.LocalProperty, referenceStep1.Name);
            if (property != null && sceneNode.IsSet((IPropertyId) property) == PropertyState.Set)
            {
              computedValueAsWpf = sceneNode.GetComputedValueAsWpf((IPropertyId) property);
              break;
            }
          }
        }
      }
      try
      {
        ReferenceStep referenceStep2 = referenceStep1;
        if (!this.TextSource.ProjectContext.IsCapabilitySet(PlatformCapability.IsWpf))
          referenceStep2 = (ReferenceStep) propertyReferenceStep;
        referenceStep2.SetValue(this.EditingElement.PlatformSpecificObject, computedValueAsWpf);
      }
      catch
      {
      }
    }

    public void AddToScene(bool visible)
    {
      this.Instantiate();
      this.UpdateEditingElementSize();
      if (!visible)
        this.EditingElement.Opacity = 0.0;
      SceneView activeView = this.textSource.DesignerContext.ActiveView;
      activeView.FocusLiveControlLayer();
      activeView.AddLiveControl((IViewControl) this.EditingElement);
      if (visible)
        activeView.UpdateLayout();
      this.UpdateEditingElementTransform();
      this.EditingElement.TextEditProxyObject = (object) this;
      TextBoxBase textBoxBase = this.EditingElement.PlatformSpecificObject as TextBoxBase;
      if (textBoxBase == null)
        return;
      TextBoxHelper.SetPreventSelectAllOnFocus((DependencyObject) textBoxBase, true);
    }

    public void RemoveFromScene()
    {
      this.TextSource.DesignerContext.ActiveView.RemoveLiveControl((IViewControl) this.EditingElement);
    }

    public void UpdateEditingElementLayout()
    {
      this.UpdateEditingElementSize();
      this.UpdateEditingElementTransform();
    }

    private void UpdateEditingElementTransform()
    {
      SceneView activeView = this.textSource.DesignerContext.ActiveView;
      Matrix matrix = new Matrix();
      if (this.TextSource.Visual != null)
      {
        if (Adorner.NonAffineTransformInParentStack((SceneElement) this.TextSource))
        {
          Rect actualBounds = activeView.GetActualBounds(this.TextSource.Visual);
          Point point = activeView.TransformPoint(this.TextSource.Visual, (IViewObject) activeView.HitTestRoot, new Point(actualBounds.Width / 2.0, actualBounds.Height / 2.0));
          matrix.Translate(point.X - this.EditingElement.Width / 2.0, point.Y - this.EditingElement.Height / 2.0);
        }
        else
          matrix = activeView.GetComputedTransformToRoot(this.TextSource.Visual);
      }
      this.EditingElement.SetTransformMatrix(this.IsMatrixValidForTransform(matrix) ? matrix : Matrix.Identity);
    }

    private void UpdateEditingElementSize()
    {
      Rect childRect = this.TextSource.LayoutDesigner.GetChildRect(this.TextSource);
      this.EditingElement.Width = childRect.Width;
      this.EditingElement.Height = childRect.Height;
    }

    private bool IsMatrixValidForTransform(Matrix matrix)
    {
      if (FloatingPointArithmetic.IsFiniteDouble(matrix.M11) && FloatingPointArithmetic.IsFiniteDouble(matrix.M12) && (FloatingPointArithmetic.IsFiniteDouble(matrix.M21) && FloatingPointArithmetic.IsFiniteDouble(matrix.M22)) && FloatingPointArithmetic.IsFiniteDouble(matrix.OffsetX))
        return FloatingPointArithmetic.IsFiniteDouble(matrix.OffsetY);
      return false;
    }

    private void DisableUndo()
    {
      TextBoxBase textBoxBase = this.EditingElement.PlatformSpecificObject as TextBoxBase;
      if (textBoxBase == null)
        return;
      TextEditProxy.UndoOverrideCommand undoOverrideCommand = new TextEditProxy.UndoOverrideCommand(this.textSource.ViewModel.Document);
      CommandBinding commandBinding1 = new CommandBinding((ICommand) undoOverrideCommand);
      InputGesture gesture1 = (InputGesture) new KeyGesture(Key.Z, ModifierKeys.Control);
      InputBinding inputBinding1 = new InputBinding((ICommand) undoOverrideCommand, gesture1);
      textBoxBase.InputBindings.Add(inputBinding1);
      textBoxBase.CommandBindings.Add(commandBinding1);
      TextEditProxy.RedoOverrideCommand redoOverrideCommand = new TextEditProxy.RedoOverrideCommand(this.textSource.ViewModel.Document);
      CommandBinding commandBinding2 = new CommandBinding((ICommand) redoOverrideCommand);
      InputGesture gesture2 = (InputGesture) new KeyGesture(Key.Y, ModifierKeys.Control);
      InputBinding inputBinding2 = new InputBinding((ICommand) redoOverrideCommand, gesture2);
      textBoxBase.InputBindings.Add(inputBinding2);
      textBoxBase.CommandBindings.Add(commandBinding2);
    }

    private class UndoOverrideCommand : ICommand
    {
      private SceneDocument document;

      public event EventHandler CanExecuteChanged
      {
        add
        {
        }
        remove
        {
        }
      }

      public UndoOverrideCommand(SceneDocument document)
      {
        this.document = document;
      }

      public void Execute(object obj)
      {
        if (!this.document.CanUndo)
          return;
        this.document.Undo();
      }

      public bool CanExecute(object obj)
      {
        return true;
      }
    }

    private class RedoOverrideCommand : ICommand
    {
      private SceneDocument document;

      public event EventHandler CanExecuteChanged
      {
        add
        {
        }
        remove
        {
        }
      }

      public RedoOverrideCommand(SceneDocument document)
      {
        this.document = document;
      }

      public void Execute(object obj)
      {
        if (!this.document.CanRedo)
          return;
        this.document.Redo();
      }

      public bool CanExecute(object obj)
      {
        return true;
      }
    }
  }
}
