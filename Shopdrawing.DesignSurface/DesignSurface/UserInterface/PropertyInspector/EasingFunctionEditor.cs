// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.EasingFunctionEditor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.PropertyInspector;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
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
  public sealed class EasingFunctionEditor : Grid, INotifyPropertyChanged, IComponentConnector
  {
    private List<SceneNodeProperty> subproperties = new List<SceneNodeProperty>();
    private List<IEasingFunctionDefinition> knownEasingFunctions = new List<IEasingFunctionDefinition>();
    private SceneNodeProperty easingFunctionProperty;
    private DesignerContext designerContext;
    private IProjectContext activeProjectContext;
    private PropertyEditingHelper propertyHelper;
    private bool isEasingFunctionNinched;
    private IEasingFunctionDefinition easingFunctionSelected;
    private ICollectionView knownEasingFunctionsView;
    internal EasingFunctionEditor EasingEditor;
    internal EasingFunctionComboBox EasingFunctionSelectionComboBox;
    internal ItemsControl SubPropertiesContainer;
    private bool _contentLoaded;

    public bool IsEasingFunctionNinched
    {
      get
      {
        return this.isEasingFunctionNinched;
      }
      private set
      {
        this.isEasingFunctionNinched = value;
        this.SendPropertyChanged("IsEasingFunctionNinched");
      }
    }

    public IEasingFunctionDefinition EasingFunctionSelected
    {
      get
      {
        return this.easingFunctionSelected;
      }
      private set
      {
        this.easingFunctionSelected = value;
        this.SendPropertyChanged("EasingFunctionSelected");
      }
    }

    public IEnumerable<PropertyEntry> SubProperties
    {
      get
      {
        if (this.easingFunctionProperty == null || this.IsEasingFunctionNinched || this.easingFunctionProperty.PropertyValue.SubProperties.Count <= 0)
          return (IEnumerable<PropertyEntry>) null;
        DependencyPropertyValueSource propertyValueSource = this.easingFunctionProperty.PropertyValue.Source as DependencyPropertyValueSource;
        if (propertyValueSource == null || !propertyValueSource.IsResource)
          return (IEnumerable<PropertyEntry>) this.easingFunctionProperty.PropertyValue.SubProperties;
        foreach (PropertyBase propertyBase in this.subproperties)
          propertyBase.OnRemoveFromCategory();
        this.subproperties.Clear();
        foreach (PropertyReferenceProperty referenceProperty in this.easingFunctionProperty.PropertyValue.SubProperties)
          this.subproperties.Add(this.easingFunctionProperty.SceneNodeObjectSet.CreateSceneNodeProperty(this.easingFunctionProperty.Reference.Append(referenceProperty.Reference), (AttributeCollection) null));
        return (IEnumerable<PropertyEntry>) this.subproperties.ToArray();
      }
    }

    public ICollectionView KnownEasingFunctions
    {
      get
      {
        return this.knownEasingFunctionsView;
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
          this.knownEasingFunctions.Clear();
        if (this.activeProjectContext != null)
          this.activeProjectContext.TypesChanged -= new EventHandler<TypesChangedEventArgs>(this.ProjectContext_TypesChanged);
        this.activeProjectContext = value;
        if (this.activeProjectContext == null)
          return;
        this.activeProjectContext.TypesChanged += new EventHandler<TypesChangedEventArgs>(this.ProjectContext_TypesChanged);
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public EasingFunctionEditor()
    {
      this.InitializeComponent();
      this.propertyHelper = new PropertyEditingHelper((UIElement) this.SubPropertiesContainer);
      this.DataContextChanged += new DependencyPropertyChangedEventHandler(this.EasingFunctionEditor_DataContextChanged);
      this.Loaded += new RoutedEventHandler(this.EasingFunctionEditor_Loaded);
      this.Unloaded += new RoutedEventHandler(this.EasingFunctionEditor_Unloaded);
    }

    private void OnEasingFunctionSelected(object sender, RoutedEventArgs args)
    {
      this.EasingFunctionSelectionComboBox.IsDropDownOpen = false;
      EasingFunctionSelectionButton functionSelectionButton = sender as EasingFunctionSelectionButton;
      if (functionSelectionButton == null || this.easingFunctionProperty == null)
        return;
      IEasingFunctionDefinition ownEasingFunction = functionSelectionButton.OwnEasingFunction;
      SceneViewModel viewModel = this.easingFunctionProperty.SceneNodeObjectSet.ViewModel;
      if (viewModel == null)
        return;
      foreach (SceneNode sceneNode in this.easingFunctionProperty.SceneNodeObjectSet.Objects)
      {
        if (!sceneNode.IsAttached)
          return;
      }
      using (SceneEditTransaction editTransaction = viewModel.CreateEditTransaction(StringTable.ChangeEasingFunctionUndoUnit, false))
      {
        if (ownEasingFunction != null)
        {
          ownEasingFunction.EasingMode = functionSelectionButton.Easing;
          this.easingFunctionProperty.PropertyValue.Value = ownEasingFunction.PlatformSpecificObject;
        }
        else
          this.easingFunctionProperty.PropertyValue.ClearValue();
        editTransaction.Commit();
      }
    }

    private void EasingFunctionEditor_Unloaded(object sender, RoutedEventArgs e)
    {
      this.Unhook();
      this.ActiveProjectContext = (IProjectContext) null;
    }

    private void EasingFunctionEditor_Loaded(object sender, RoutedEventArgs e)
    {
      this.UpdateFromDataContext();
    }

    private void EasingFunctionEditor_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      this.UpdateFromDataContext();
    }

    private void UpdateFromDataContext()
    {
      this.Unhook();
      Microsoft.Windows.Design.PropertyEditing.PropertyValue propertyValue = this.DataContext as Microsoft.Windows.Design.PropertyEditing.PropertyValue;
      if (propertyValue != null)
      {
        this.easingFunctionProperty = propertyValue.ParentProperty as SceneNodeProperty;
        if (this.easingFunctionProperty.SceneNodeObjectSet.Document == null || this.easingFunctionProperty.SceneNodeObjectSet.RepresentativeSceneNode == null)
        {
          this.easingFunctionProperty = (SceneNodeProperty) null;
          this.propertyHelper.ActiveDocument = (SceneDocument) null;
        }
        else
        {
          if (this.designerContext == null)
            this.designerContext = this.easingFunctionProperty.SceneNodeObjectSet.DesignerContext;
          this.ActiveProjectContext = this.easingFunctionProperty.SceneNodeObjectSet.ProjectContext;
          this.propertyHelper.ActiveDocument = this.easingFunctionProperty.SceneNodeObjectSet.Document;
          this.easingFunctionProperty.RemoveFromCategoryWhenDisassociated = true;
          this.easingFunctionProperty.PropertyReferenceChanged += new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.easingFunctionProperty_PropertyReferenceChanged);
          this.Rebuild();
        }
      }
      else
      {
        this.easingFunctionProperty = (SceneNodeProperty) null;
        this.propertyHelper.ActiveDocument = (SceneDocument) null;
      }
    }

    private void ProjectContext_TypesChanged(object sender, TypesChangedEventArgs e)
    {
      this.knownEasingFunctions.Clear();
      this.Rebuild();
    }

    private void easingFunctionProperty_PropertyReferenceChanged(object sender, PropertyReferenceChangedEventArgs e)
    {
      this.UpdateEasingFunctionSelected();
      this.SendPropertyChanged("SubProperties");
    }

    private void Rebuild()
    {
      if (this.knownEasingFunctions.Count == 0 && this.easingFunctionProperty != null)
      {
        this.easingFunctionProperty.Recache();
        SceneNode representativeSceneNode = this.easingFunctionProperty.SceneNodeObjectSet.RepresentativeSceneNode;
        SceneViewModel viewModel = this.easingFunctionProperty.SceneNodeObjectSet.ViewModel;
        if (representativeSceneNode == null || viewModel == null)
          return;
        if (viewModel.ProjectContext.ResolveType(PlatformTypes.EasingFunctionBase) != null)
        {
          foreach (IAssembly assembly in (IEnumerable<IAssembly>) viewModel.ProjectContext.AssemblyReferences)
            this.AddFoundEasingFunctionBaseTypes(assembly, viewModel.ProjectContext.Platform);
          this.knownEasingFunctions.Sort((Comparison<IEasingFunctionDefinition>) ((a, b) => a.Name.CompareTo(b.Name)));
          this.knownEasingFunctionsView = CollectionViewSource.GetDefaultView((object) this.knownEasingFunctions);
          PropertyGroupDescription groupDescription = new PropertyGroupDescription();
          groupDescription.PropertyName = "GroupName";
          this.knownEasingFunctionsView.GroupDescriptions.Clear();
          this.knownEasingFunctionsView.GroupDescriptions.Add((GroupDescription) groupDescription);
          this.SendPropertyChanged("KnownEasingFunctions");
        }
      }
      this.UpdateEasingFunctionSelected();
      this.SendPropertyChanged("SubProperties");
    }

    private void AddFoundEasingFunctionBaseTypes(IAssembly assembly, IPlatform platform)
    {
      if (assembly == null || !assembly.IsLoaded || platform == null)
        return;
      bool flag = false;
      Dictionary<string, List<IEasingFunctionDefinition>> orCreateCache = DesignSurfacePlatformCaches.GetOrCreateCache<Dictionary<string, List<IEasingFunctionDefinition>>>(platform.Metadata, DesignSurfacePlatformCaches.EasingFunctionsCache);
      IAssembly usingAssemblyName = platform.Metadata.GetPlatformAssemblyUsingAssemblyName(assembly);
      if (usingAssemblyName != null && AssemblyHelper.IsPlatformAssembly(usingAssemblyName))
      {
        if (orCreateCache.ContainsKey(assembly.FullName))
        {
          this.knownEasingFunctions.AddRange((IEnumerable<IEasingFunctionDefinition>) orCreateCache[assembly.FullName]);
          return;
        }
        flag = true;
      }
      bool supportInternal = this.ActiveProjectContext.ProjectAssembly.CompareTo(assembly);
      IType type1 = platform.Metadata.ResolveType(PlatformTypes.EasingFunctionBase);
      if (platform.Metadata.IsNullType((ITypeId) type1))
        return;
      Type runtimeType = type1.RuntimeType;
      Type[] types;
      try
      {
        types = AssemblyHelper.GetTypes(assembly);
      }
      catch (Exception ex)
      {
        return;
      }
      foreach (Type type2 in types)
      {
        if (runtimeType.IsAssignableFrom(type2))
        {
          ConstructorAccessibility accessibility;
          ConstructorInfo defaultConstructor = PlatformTypeHelper.GetDefaultConstructor(type2, supportInternal, out accessibility);
          if (defaultConstructor != (ConstructorInfo) null)
          {
            IEasingFunctionDefinition functionDefinition = platform.ViewObjectFactory.Instantiate(defaultConstructor.Invoke((object[]) null)) as IEasingFunctionDefinition;
            if (functionDefinition != null)
            {
              this.knownEasingFunctions.Add(functionDefinition);
              if (flag)
              {
                if (!orCreateCache.ContainsKey(assembly.FullName))
                  orCreateCache.Add(assembly.FullName, new List<IEasingFunctionDefinition>());
                orCreateCache[assembly.FullName].Add(functionDefinition);
              }
            }
          }
        }
      }
    }

    private void UpdateEasingFunctionSelected()
    {
      if (this.easingFunctionProperty != null && this.easingFunctionProperty.SceneNodeObjectSet.Count > 0)
      {
        foreach (SceneNode sceneNode in this.easingFunctionProperty.SceneNodeObjectSet.Objects)
        {
          if (!sceneNode.IsAttached)
            return;
        }
        SceneViewModel viewModel = this.easingFunctionProperty.SceneNodeObjectSet.ViewModel;
        if (viewModel == null)
          return;
        SceneNode sceneNode1 = (SceneNode) (this.easingFunctionProperty.SceneNodeObjectSet.RepresentativeSceneNode as DictionaryEntryNode);
        if (sceneNode1 != null)
        {
          this.IsEasingFunctionNinched = false;
          object computedValue = sceneNode1.GetComputedValue(this.easingFunctionProperty.Reference);
          this.EasingFunctionSelected = viewModel.ProjectContext.Platform.ViewObjectFactory.Instantiate(computedValue) as IEasingFunctionDefinition;
        }
        else
        {
          object platformObject = this.easingFunctionProperty.PropertyValue.Value;
          this.IsEasingFunctionNinched = platformObject == MixedProperty.Mixed;
          if (this.IsEasingFunctionNinched)
            return;
          this.EasingFunctionSelected = viewModel.ProjectContext.Platform.ViewObjectFactory.Instantiate(platformObject) as IEasingFunctionDefinition;
        }
      }
      else
      {
        this.IsEasingFunctionNinched = true;
        this.EasingFunctionSelected = (IEasingFunctionDefinition) null;
      }
    }

    private void Unhook()
    {
      if (this.easingFunctionProperty != null)
      {
        this.easingFunctionProperty.PropertyReferenceChanged -= new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.easingFunctionProperty_PropertyReferenceChanged);
        this.easingFunctionProperty = (SceneNodeProperty) null;
      }
      foreach (PropertyBase propertyBase in this.subproperties)
        propertyBase.OnRemoveFromCategory();
      this.subproperties.Clear();
    }

    private void SendPropertyChanged(string propertyName)
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
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/propertyinspector/categoryeditors/easing/easingfunctioneditor.xaml", UriKind.Relative));
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
      switch (connectionId)
      {
        case 1:
          this.EasingEditor = (EasingFunctionEditor) target;
          break;
        case 2:
          this.EasingFunctionSelectionComboBox = (EasingFunctionComboBox) target;
          break;
        case 3:
          this.SubPropertiesContainer = (ItemsControl) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
