// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.BrushEditor.ResourceEditorControl
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using Microsoft.Expression.DesignSurface.UserInterface.ResourcePane;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.PropertyInspector;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.BrushEditor
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  internal sealed class ResourceEditorControl : Border, IPropertyInspector, IDisposable, IComponentConnector
  {
    private DesignerContext designerContext;
    private SceneNodeProperty editingProperty;
    private PropertyValue editingValue;
    private EditResourceModel resourceModel;
    private PropertyEditingHelper transactionHelper;
    internal ResourceEditorControl ResourceEditor;
    private bool _contentLoaded;

    public PropertyValue EditingValue
    {
      get
      {
        return this.editingValue;
      }
    }

    public DataTemplate EditingTemplate
    {
      get
      {
        PropertyValueEditor propertyValueEditor1 = ((PropertyEntry) this.editingProperty).get_PropertyValueEditor();
        ExtendedPropertyValueEditor propertyValueEditor2 = propertyValueEditor1 as ExtendedPropertyValueEditor;
        if (propertyValueEditor2 != null && propertyValueEditor2.get_ExtendedEditorTemplate() != null)
          return propertyValueEditor2.get_ExtendedEditorTemplate();
        if (propertyValueEditor1 != null)
          return propertyValueEditor1.get_InlineEditorTemplate();
        return this.TryFindResource((object) "PropertyContainerDefaultInlineTemplate") as DataTemplate;
      }
    }

    public ResourceEditorControl(DesignerContext designerContext, SceneNodeProperty editingProperty)
    {
      this.designerContext = designerContext;
      this.editingProperty = editingProperty;
      this.resourceModel = this.BuildResourceModelFromPropertyReference();
      this.editingValue = this.resourceModel.EditorValue;
      this.transactionHelper = new PropertyEditingHelper(this.resourceModel.Document, (UIElement) this);
      this.DataContext = (object) this;
      this.CommandBindings.Add(new CommandBinding((ICommand) PropertyValueEditorCommands.get_FinishEditing(), new ExecutedRoutedEventHandler(this.OnFinishEditing)));
      PropertyInspectorHelper.SetOwningPropertyInspectorModel((DependencyObject) this, (IPropertyInspector) this);
      PropertyInspectorHelper.SetOwningPropertyInspectorElement((DependencyObject) this, (UIElement) this);
      this.InitializeComponent();
    }

    private EditResourceModel BuildResourceModelFromPropertyReference()
    {
      bool isMixed;
      DocumentCompositeNode findMe = (DocumentCompositeNode) this.editingProperty.GetLocalValueAsDocumentNode(false, out isMixed);
      ResourceManager resourceManager = this.editingProperty.SceneNodeObjectSet.DesignerContext.ResourceManager;
      DocumentCompositeNode documentCompositeNode = Microsoft.Expression.DesignSurface.Utility.ResourceHelper.LookupResource(this.editingProperty.SceneNodeObjectSet.ViewModel, findMe);
      DictionaryEntryNode resourceEntryNode = (DictionaryEntryNode) (documentCompositeNode.DocumentRoot != findMe.DocumentRoot ? ((ISceneViewHost) this.editingProperty.SceneNodeObjectSet.ViewModel.ProjectContext.GetService(typeof (ISceneViewHost))).OpenView(documentCompositeNode.DocumentRoot, false).ViewModel : this.editingProperty.SceneNodeObjectSet.ViewModel).GetSceneNode((DocumentNode) documentCompositeNode);
      SceneNode sceneNode = resourceEntryNode.Value;
      return new EditResourceModel(this.designerContext, resourceEntryNode, (IPropertyInspector) this);
    }

    private void OnFinishEditing(object sender, ExecutedRoutedEventArgs e)
    {
      this.designerContext.ActiveView.ReturnFocus();
      e.Handled = true;
    }

    public bool IsCategoryExpanded(string categoryName)
    {
      return true;
    }

    public void UpdateTransaction()
    {
      this.transactionHelper.UpdateTransaction();
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    private void Dispose(bool disposing)
    {
      if (!disposing || this.resourceModel == null)
        return;
      this.resourceModel.Dispose();
      this.resourceModel = (EditResourceModel) null;
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/propertyinspector/complexvalueeditors/resourceeditorcontrol.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    internal Delegate _CreateDelegate(Type delegateType, string handler)
    {
      return Delegate.CreateDelegate(delegateType, (object) this, handler);
    }

    [DebuggerNonUserCode]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      if (connectionId == 1)
        this.ResourceEditor = (ResourceEditorControl) target;
      else
        this._contentLoaded = true;
    }
  }
}
