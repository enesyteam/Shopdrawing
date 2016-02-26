// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.ResourcePane.EditResourceDialog
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.PropertyInspector;
using Microsoft.Expression.Framework.ValueEditors;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignSurface.UserInterface.ResourcePane
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class EditResourceDialog : Dialog, IPropertyInspector, IComponentConnector
  {
    private EditResourceModel model;
    private DesignerContext designerContext;
    private PropertyEditingHelper transactionHelper;
    internal EditResourceDialog UserControlSelf;
    internal Label TextBlock2;
    internal TextBox KeyText;
    internal Button AcceptButton;
    internal Button CancelButton;
    private bool _contentLoaded;

    internal EditResourceModel Model
    {
      set
      {
        this.model = value;
        this.DataContext = (object) value;
        this.transactionHelper.ActiveDocument = this.model.Document;
      }
    }

    public DataTemplate EditorTemplate
    {
      get
      {
        PropertyValueEditor propertyValueEditor1 = this.model.EditingProperty.PropertyValueEditor;
        ExtendedPropertyValueEditor propertyValueEditor2 = propertyValueEditor1 as ExtendedPropertyValueEditor;
        if (propertyValueEditor2 != null && propertyValueEditor2.ExtendedEditorTemplate != null)
          return propertyValueEditor2.ExtendedEditorTemplate;
        if (propertyValueEditor1 != null)
          return propertyValueEditor1.InlineEditorTemplate;
        return this.TryFindResource((object) "PropertyContainerDefaultInlineTemplate") as DataTemplate;
      }
    }

    internal EditResourceDialog(DesignerContext designerContext)
    {
      this.designerContext = designerContext;
      this.Title = StringTable.EditResourceDialogTitle;
      this.transactionHelper = new PropertyEditingHelper((UIElement) this);
      this.CommandBindings.Add(new CommandBinding((ICommand) PropertyValueEditorCommands.FinishEditing, new ExecutedRoutedEventHandler(this.OnPropertyContainerCommandsFinishEditing)));
      PropertyInspectorHelper.SetOwningPropertyInspectorModel((DependencyObject) this, (IPropertyInspector) this);
      PropertyInspectorHelper.SetOwningPropertyInspectorElement((DependencyObject) this, (UIElement) this);
      ValueEditorUtils.SetHandlesCommitKeys((DependencyObject) this, true);
      this.InitializeComponent();
    }

    public bool IsCategoryExpanded(string categoryName)
    {
      return true;
    }

    public void UpdateTransaction()
    {
      this.transactionHelper.UpdateTransaction();
    }

    protected override void OnAcceptButtonExecute()
    {
      if (this.model == null)
        return;
      if (!this.model.KeyStringIsValid)
        this.designerContext.MessageDisplayService.ShowError(StringTable.EditResourceInvalidKey);
      if (!this.model.ExceptionOccurred)
        base.OnAcceptButtonExecute();
      this.model.ClearExceptionOccurred();
    }

    private void OnPropertyContainerCommandsFinishEditing(object sender, ExecutedRoutedEventArgs eventArgs)
    {
      this.AcceptButton.Focus();
      eventArgs.Handled = true;
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/resourcepane/editresourcedialog.xaml", UriKind.Relative));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerNonUserCode]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.UserControlSelf = (EditResourceDialog) target;
          break;
        case 2:
          this.TextBlock2 = (Label) target;
          break;
        case 3:
          this.KeyText = (TextBox) target;
          break;
        case 4:
          this.AcceptButton = (Button) target;
          break;
        case 5:
          this.CancelButton = (Button) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
