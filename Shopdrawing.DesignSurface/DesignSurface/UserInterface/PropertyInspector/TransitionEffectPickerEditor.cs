// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.TransitionEffectPickerEditor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Tools.Assets;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.PropertyInspector;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class TransitionEffectPickerEditor : Grid, INotifyPropertyChanged, IComponentConnector
  {
    private static TransitionEffectPickerEditor.AlphabeticalTransitionEffectComparer alphabeticalTransitionEffectComparer = new TransitionEffectPickerEditor.AlphabeticalTransitionEffectComparer();
    private ObservableCollectionWorkaround<TransitionEffectInfo> transitionEffects = new ObservableCollectionWorkaround<TransitionEffectInfo>();
    private List<SceneNodeProperty> subproperties = new List<SceneNodeProperty>();
    private DesignerContext designerContext;
    private IProjectContext activeProjectContext;
    private ICollectionView transitionEffectsView;
    private bool hasLoadingPlaceholder;
    private SceneNodeProperty editingProperty;
    private PropertyEditingHelper propertyHelper;
    internal TransitionEffectPickerEditor TransitionEffectPickerEditorControl;
    internal ItemsControl SubPropertiesContainer;
    private bool _contentLoaded;

    private SceneNodeProperty EditingProperty
    {
      get
      {
        return this.editingProperty;
      }
      set
      {
        if (this.editingProperty != null)
          this.editingProperty.PropertyReferenceChanged -= new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.editingProperty_PropertyReferenceChanged);
        this.editingProperty = value;
        if (this.editingProperty == null)
          return;
        this.EditingProperty.PropertyReferenceChanged += new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.editingProperty_PropertyReferenceChanged);
      }
    }

    public ICollectionView TransitionEffects
    {
      get
      {
        return this.transitionEffectsView;
      }
    }

    public TransitionEffectInfo CurrentTransitionEffect
    {
      get
      {
        if (this.EditingProperty != null && this.EditingProperty.PropertyValue != null)
        {
          object obj = this.EditingProperty.PropertyValue.Value;
          string str = obj == null ? StringTable.TransitionEffectNone : obj.GetType().FullName;
          foreach (TransitionEffectInfo transitionEffectInfo in (Collection<TransitionEffectInfo>) this.transitionEffects)
          {
            if (transitionEffectInfo.TransitionEffectFullName == str)
              return transitionEffectInfo;
          }
        }
        return (TransitionEffectInfo) null;
      }
      set
      {
        if (this.EditingProperty.PropertyValue == null || this.EditingProperty.SceneNodeObjectSet.RepresentativeSceneNode == null)
          return;
        using (SceneEditTransaction editTransaction = this.EditingProperty.SceneNodeObjectSet.ViewModel.CreateEditTransaction(StringTable.ChangeTransitionEffectUndoUnit, false))
        {
          if (value.Asset == null)
          {
            this.EditingProperty.PropertyValue.Value = (object) null;
          }
          else
          {
            ProjectContext projectContext = ProjectContext.GetProjectContext(this.editingProperty.SceneNodeObjectSet.ProjectContext);
            if (((TypeAsset) value.Asset).EnsureTypeReferenced(projectContext))
            {
              bool supportInternal = true;
              ConstructorAccessibility accessibility;
              ConstructorInfo defaultConstructor = PlatformTypeHelper.GetDefaultConstructor(((TypeAsset) value.Asset).Type.RuntimeType, supportInternal, out accessibility);
              if (defaultConstructor != (ConstructorInfo) null)
                this.EditingProperty.PropertyValue.Value = defaultConstructor.Invoke((object[]) null);
            }
            if (this.EditingProperty.SceneNodeObjectSet.Count > 0)
            {
              VisualStateTransitionSceneNode transitionSceneNode = this.EditingProperty.SceneNodeObjectSet.RepresentativeSceneNode as VisualStateTransitionSceneNode;
              if (transitionSceneNode != null && transitionSceneNode.ViewModel != null && transitionSceneNode.ViewModel.ActiveEditContext != null)
                VisualStateManagerSceneNode.UpdateHasExtendedVisualStateManager(transitionSceneNode.ViewModel.ActiveEditContext.EditingContainer);
            }
          }
          editTransaction.Commit();
        }
      }
    }

    public IEnumerable<PropertyEntry> SubProperties
    {
      get
      {
        if (this.editingProperty == null || this.editingProperty.PropertyValue.SubProperties.Count <= 0)
          return (IEnumerable<PropertyEntry>) null;
        DependencyPropertyValueSource propertyValueSource = this.editingProperty.PropertyValue.Source as DependencyPropertyValueSource;
        if (propertyValueSource == null || !propertyValueSource.IsResource)
          return (IEnumerable<PropertyEntry>) this.editingProperty.PropertyValue.SubProperties;
        foreach (PropertyBase propertyBase in this.subproperties)
          propertyBase.OnRemoveFromCategory();
        this.subproperties.Clear();
        foreach (PropertyReferenceProperty referenceProperty in this.editingProperty.PropertyValue.SubProperties)
          this.subproperties.Add(this.editingProperty.SceneNodeObjectSet.CreateSceneNodeProperty(this.editingProperty.Reference.Append(referenceProperty.Reference), (AttributeCollection) null));
        return (IEnumerable<PropertyEntry>) this.subproperties.ToArray();
      }
    }

    private IProjectContext ActiveProjectContext
    {
      get
      {
        return this.activeProjectContext;
      }
      set
      {
        if (this.activeProjectContext != value)
        {
          this.transitionEffects.Clear();
          this.transitionEffects.Add(new TransitionEffectInfo((Asset) null));
          this.transitionEffects.Add(new TransitionEffectInfo((Asset) null)
          {
            IsLoadingPlaceholder = true
          });
          this.hasLoadingPlaceholder = true;
        }
        if (this.activeProjectContext != null)
          this.activeProjectContext.TypesChanged -= new EventHandler<TypesChangedEventArgs>(this.ProjectContext_TypesChanged);
        this.activeProjectContext = value;
        if (this.activeProjectContext == null)
          return;
        this.activeProjectContext.TypesChanged += new EventHandler<TypesChangedEventArgs>(this.ProjectContext_TypesChanged);
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public TransitionEffectPickerEditor()
    {
      this.InitializeComponent();
      this.propertyHelper = new PropertyEditingHelper((UIElement) this.SubPropertiesContainer);
      this.transitionEffects.Add(new TransitionEffectInfo((Asset) null));
      this.transitionEffects.Add(new TransitionEffectInfo((Asset) null)
      {
        IsLoadingPlaceholder = true
      });
      this.hasLoadingPlaceholder = true;
      this.DataContextChanged += new DependencyPropertyChangedEventHandler(this.TransitionEffectPickerEditor_DataContextChanged);
      this.Loaded += new RoutedEventHandler(this.TransitionEffectPickerEditor_Loaded);
      this.Unloaded += new RoutedEventHandler(this.TransitionEffectPickerEditor_Unloaded);
    }

    private void TransitionEffectPickerEditor_Unloaded(object sender, RoutedEventArgs e)
    {
      this.EditingProperty = (SceneNodeProperty) null;
      this.ActiveProjectContext = (IProjectContext) null;
      if (this.designerContext != null)
      {
        ((AssetLibrary) this.designerContext.AssetLibrary).OnClose();
        ((AssetLibrary) this.designerContext.AssetLibrary).AssetLibraryChanged -= new Action<AssetLibraryDamages>(this.TransitionEffectPickerEditor_AssetLibraryChanged);
      }
      foreach (PropertyBase propertyBase in this.subproperties)
        propertyBase.OnRemoveFromCategory();
      this.subproperties.Clear();
    }

    private void TransitionEffectPickerEditor_Loaded(object sender, RoutedEventArgs e)
    {
      this.UpdateFromDataContext();
    }

    private void UpdateFromDataContext()
    {
      this.EditingProperty = (SceneNodeProperty) null;
      Microsoft.Windows.Design.PropertyEditing.PropertyValue propertyValue = this.DataContext as Microsoft.Windows.Design.PropertyEditing.PropertyValue;
      if (propertyValue != null)
      {
        this.EditingProperty = propertyValue.ParentProperty as SceneNodeProperty;
        if (this.EditingProperty == null || this.EditingProperty.SceneNodeObjectSet.Document == null)
        {
          this.EditingProperty = (SceneNodeProperty) null;
          this.propertyHelper.ActiveDocument = (SceneDocument) null;
        }
        else
        {
          if (this.designerContext == null)
          {
            this.designerContext = this.EditingProperty.SceneNodeObjectSet.DesignerContext;
            ((AssetLibrary) this.designerContext.AssetLibrary).OnOpen();
            ((AssetLibrary) this.designerContext.AssetLibrary).AssetLibraryChanged += new Action<AssetLibraryDamages>(this.TransitionEffectPickerEditor_AssetLibraryChanged);
          }
          this.ActiveProjectContext = this.EditingProperty.SceneNodeObjectSet.ProjectContext;
          this.propertyHelper.ActiveDocument = this.EditingProperty.SceneNodeObjectSet.Document;
          this.EditingProperty.Recache();
          this.Rebuild();
        }
      }
      else
      {
        this.EditingProperty = (SceneNodeProperty) null;
        this.propertyHelper.ActiveDocument = (SceneDocument) null;
      }
    }

    private void TransitionEffectPickerEditor_AssetLibraryChanged(AssetLibraryDamages obj)
    {
      this.UpdateFromDataContext();
    }

    private void ProjectContext_TypesChanged(object sender, TypesChangedEventArgs e)
    {
      TransitionEffectInfo transitionEffect = this.CurrentTransitionEffect;
      this.transitionEffects.Clear();
      this.transitionEffects.Add(new TransitionEffectInfo((Asset) null));
      if (transitionEffect != null && transitionEffect.Asset != null)
      {
        this.transitionEffects.Add(transitionEffect);
        if (this.CurrentTransitionEffect == null)
          this.CurrentTransitionEffect = transitionEffect;
      }
      this.transitionEffects.Add(new TransitionEffectInfo((Asset) null)
      {
        IsLoadingPlaceholder = true
      });
      this.hasLoadingPlaceholder = true;
      this.UpdateFromDataContext();
    }

    private void editingProperty_PropertyReferenceChanged(object sender, PropertyReferenceChangedEventArgs e)
    {
      this.UpdateFromDataContext();
    }

    private void TransitionEffectPickerEditor_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      this.UpdateFromDataContext();
    }

    private void Rebuild()
    {
      if (this.EditingProperty == null)
        return;
      SceneNode representativeSceneNode = this.EditingProperty.SceneNodeObjectSet.RepresentativeSceneNode;
      if (representativeSceneNode == null || this.EditingProperty.SceneNodeObjectSet.ViewModel == null)
        return;
      foreach (Asset asset in (representativeSceneNode.DesignerContext.AssetLibrary as AssetLibrary).Assets)
      {
        TypeAsset typeAsset = asset as TypeAsset;
        if (typeAsset != null && ProjectNeutralTypes.TransitionEffect.IsAssignableFrom((ITypeId) asset.TargetType))
        {
          if (this.hasLoadingPlaceholder)
          {
            this.transitionEffects.RemoveAt(this.transitionEffects.Count - 1);
            this.hasLoadingPlaceholder = false;
          }
          TransitionEffectInfo transitionEffectInfo = new TransitionEffectInfo((Asset) typeAsset);
          int num = this.transitionEffects.BinarySearch(transitionEffectInfo, (IComparer<TransitionEffectInfo>) TransitionEffectPickerEditor.alphabeticalTransitionEffectComparer);
          if (num < 0)
            this.transitionEffects.Insert(~num, transitionEffectInfo);
        }
      }
      this.transitionEffectsView = CollectionViewSource.GetDefaultView((object) this.transitionEffects);
      this.OnPropertyChanged("CurrentTransitionEffect");
      this.OnPropertyChanged("TransitionEffects");
      this.OnPropertyChanged("SubProperties");
    }

    private void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/propertyinspector/complexvalueeditors/transitioneffectpickereditor.xaml", UriKind.Relative));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerNonUserCode]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.TransitionEffectPickerEditorControl = (TransitionEffectPickerEditor) target;
          break;
        case 2:
          this.SubPropertiesContainer = (ItemsControl) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }

    private class AlphabeticalTransitionEffectComparer : Comparer<TransitionEffectInfo>
    {
      public override int Compare(TransitionEffectInfo a, TransitionEffectInfo b)
      {
        if (a.Asset == null)
          return -1;
        if (b.Asset != null)
          return a.TransitionEffectName.CompareTo(b.TransitionEffectName);
        return 1;
      }
    }
  }
}
