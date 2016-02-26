// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.LayoutPathCollectionEditor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.CodeDom.Compiler;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class LayoutPathCollectionEditor : ComplexValueEditorBase, IPickWhipHost, IComponentConnector, IStyleConnector
  {
    private LayoutPathCollectionEditor.LayoutPathSelectionStrategy layoutPathSelectionStrategy;
    private SceneNodePropertyValue propertyValueCache;
    private int currentIndex;
    internal LayoutPathCollectionEditor LayoutPathsEditor;
    internal SingleSelectionListBox LayoutPathsListBox;
    private bool _contentLoaded;

    private PropertyValueCollection LayoutPaths
    {
      get
      {
        if (this.propertyValueCache != null)
          return this.propertyValueCache.Collection;
        return (PropertyValueCollection) null;
      }
    }

    public ICollectionView LayoutPathsView { get; private set; }

    public bool HasLayoutPaths
    {
      get
      {
        if (this.LayoutPathsView != null)
          return !this.LayoutPathsView.IsEmpty;
        return false;
      }
    }

    public FrameworkElement PropertyEditor
    {
      get
      {
        return (FrameworkElement) this;
      }
    }

    public Cursor PickWhipCursor
    {
      get
      {
        return ToolCursors.PickWhipCursor;
      }
    }

    public IElementSelectionStrategy ElementSelectionStrategy
    {
      get
      {
        return (IElementSelectionStrategy) this.layoutPathSelectionStrategy;
      }
    }

    public new SceneNodeProperty EditingProperty
    {
      get
      {
        return base.EditingProperty;
      }
    }

    public LayoutPathCollectionEditor()
    {
      this.layoutPathSelectionStrategy = new LayoutPathCollectionEditor.LayoutPathSelectionStrategy();
      this.layoutPathSelectionStrategy.TypeConstraint = PlatformTypes.FrameworkElement;
      this.layoutPathSelectionStrategy.Freeze();
      this.InitializeComponent();
    }

    private void MoveUp(object sender, RoutedEventArgs args)
    {
      this.ChangeOrder(-1);
    }

    private void MoveDown(object sender, RoutedEventArgs args)
    {
      this.ChangeOrder(1);
    }

    private void ChangeOrder(int increment)
    {
      if (this.LayoutPaths == null || this.LayoutPaths.Count == 0)
        return;
      int currentPosition = this.LayoutPathsView.CurrentPosition;
      int newIndex = currentPosition + increment;
      if (currentPosition == -1 || newIndex < 0 || newIndex >= this.LayoutPaths.Count)
        return;
      using (SceneEditTransaction editTransaction = this.ViewModel.CreateEditTransaction(StringTable.ChangeLayoutPathOrderUndo))
      {
        this.currentIndex = newIndex;
        this.LayoutPaths.SetIndex(currentPosition, newIndex);
        editTransaction.Commit();
      }
    }

    private void DeleteLayoutPath(object sender, RoutedEventArgs args)
    {
      SceneNodePropertyValue nodePropertyValue = ((FrameworkElement) sender).DataContext as SceneNodePropertyValue;
      if (nodePropertyValue == null || this.LayoutPaths == null || this.LayoutPaths.Count == 0)
        return;
      if (this.LayoutPathsView.CurrentItem as SceneNodePropertyValue == nodePropertyValue && this.LayoutPathsView.CurrentPosition == this.LayoutPaths.Count - 1)
        this.currentIndex = Math.Max(0, this.LayoutPaths.Count - 2);
      using (SceneEditTransaction editTransaction = this.ViewModel.CreateEditTransaction(StringTable.RemoveLayoutPathUndo))
      {
        using (this.ViewModel.ForceBaseValue())
        {
          this.LayoutPaths.Remove((Microsoft.Windows.Design.PropertyEditing.PropertyValue) nodePropertyValue);
          editTransaction.Commit();
        }
      }
    }

    protected override void Rebuild()
    {
      if (this.propertyValueCache == this.EditingValue)
        return;
      if (this.LayoutPathsView != null)
      {
        this.LayoutPathsView.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.LayoutPathsView_CollectionChanged);
        this.LayoutPathsView.CurrentChanged -= new EventHandler(this.LayoutPathsView_CurrentChanged);
      }
      this.propertyValueCache = this.EditingValue;
      this.currentIndex = 0;
      this.LayoutPathsView = CollectionViewSource.GetDefaultView((object) this.LayoutPaths);
      this.LayoutPathsView.CollectionChanged += new NotifyCollectionChangedEventHandler(this.LayoutPathsView_CollectionChanged);
      this.LayoutPathsView.CurrentChanged += new EventHandler(this.LayoutPathsView_CurrentChanged);
      this.OnPropertyChanged("LayoutPathsView");
      this.OnPropertyChanged("HasLayoutPaths");
    }

    private void LayoutPathsView_CurrentChanged(object sender, EventArgs e)
    {
      if (this.LayoutPathsView == null || this.LayoutPaths == null || (this.LayoutPaths.Count == 0 || this.LayoutPathsView.CurrentPosition == -1))
        return;
      this.currentIndex = this.LayoutPathsView.CurrentPosition;
    }

    private void LayoutPathsView_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.OnPropertyChanged("HasLayoutPaths");
      if (this.LayoutPathsView == null || this.LayoutPaths == null || this.LayoutPaths.Count == 0)
        return;
      if (this.currentIndex >= this.LayoutPaths.Count)
        this.currentIndex = this.LayoutPaths.Count - 1;
      this.LayoutPathsView.MoveCurrentToPosition(this.currentIndex);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/propertyinspector/complexvalueeditors/layoutpathcollectioneditor.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    internal Delegate _CreateDelegate(Type delegateType, string handler)
    {
      return Delegate.CreateDelegate(delegateType, (object) this, handler);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerNonUserCode]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.LayoutPathsEditor = (LayoutPathCollectionEditor) target;
          break;
        case 3:
          ((ButtonBase) target).Click += new RoutedEventHandler(this.MoveUp);
          break;
        case 4:
          ((ButtonBase) target).Click += new RoutedEventHandler(this.MoveDown);
          break;
        case 5:
          this.LayoutPathsListBox = (SingleSelectionListBox) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }

    [DebuggerNonUserCode]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IStyleConnector.Connect(int connectionId, object target)
    {
      if (connectionId != 2)
        return;
      ((ButtonBase) target).Click += new RoutedEventHandler(this.DeleteLayoutPath);
    }

    private class LayoutPathSelectionStrategy : ConstrainedElementSelectionStrategy
    {
      public override void SelectElement(SceneElement element, SceneNodeProperty editingProperty)
      {
        if (element == null || editingProperty == null || (editingProperty.SceneNodeObjectSet == null || editingProperty.IsMixedValue))
          return;
        SceneNodeObjectSet sceneNodeObjectSet = editingProperty.SceneNodeObjectSet;
        BindingSceneNode bindingSceneNode = sceneNodeObjectSet.ViewModel.CreateSceneNode(PlatformTypes.Binding) as BindingSceneNode;
        LayoutPathNode layoutPathNode = sceneNodeObjectSet.ViewModel.CreateSceneNode(ProjectNeutralTypes.LayoutPath) as LayoutPathNode;
        using (SceneEditTransaction editTransaction = sceneNodeObjectSet.ViewModel.CreateEditTransaction(StringTable.AddLayoutPathUndo))
        {
          using (sceneNodeObjectSet.ViewModel.ForceBaseValue())
          {
            element.EnsureNamed();
            editTransaction.Update();
            bindingSceneNode.ElementName = element.Name;
            layoutPathNode.SetValue(LayoutPathNode.SourceElementProperty, (object) bindingSceneNode.DocumentNode);
            editingProperty.AddValue((object) layoutPathNode.DocumentNode);
            editTransaction.Commit();
          }
        }
      }

      protected override Freezable CreateInstanceCore()
      {
        return (Freezable) new LayoutPathCollectionEditor.LayoutPathSelectionStrategy();
      }
    }
  }
}
