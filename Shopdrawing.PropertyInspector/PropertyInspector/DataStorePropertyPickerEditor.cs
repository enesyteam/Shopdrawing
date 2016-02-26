// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.DataStorePropertyPickerEditor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Shapes;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class DataStorePropertyPickerEditor : Grid, INotifyPropertyChanged, IComponentConnector
  {
    public static readonly DependencyProperty PropertyNameEntryProperty = DependencyProperty.Register("PropertyNameEntry", typeof (string), typeof (DataStorePropertyPickerEditor), (PropertyMetadata) new FrameworkPropertyMetadata((PropertyChangedCallback) null));
    private ICollectionView propertiesView;
    private DataStorePropertiesValueProvider dataStorePropertiesValueProvider;
    private SceneNodeProperty parentProperty;
    internal DataStorePropertyPickerEditor DataStorePropertyPickerEditorControl;
    private bool _contentLoaded;

    public string PropertyNameEntry
    {
      get
      {
        return (string) this.GetValue(DataStorePropertyPickerEditor.PropertyNameEntryProperty);
      }
      set
      {
        this.SetValue(DataStorePropertyPickerEditor.PropertyNameEntryProperty, value);
      }
    }

    public ICollectionView Properties
    {
      get
      {
        return this.propertiesView;
      }
    }

    public DataStorePropertyEntry SelectedProperty
    {
      get
      {
        return this.dataStorePropertiesValueProvider.SelectedProperty;
      }
      set
      {
        if (this.dataStorePropertiesValueProvider.SelectedProperty == value)
          return;
        this.dataStorePropertiesValueProvider.SelectedProperty = value;
        this.OnPropertyChanged("SelectedProperty");
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public DataStorePropertyPickerEditor()
    {
      this.dataStorePropertiesValueProvider = new DataStorePropertiesValueProvider((FrameworkElement) this);
      this.dataStorePropertiesValueProvider.Rebuilt += new EventHandler(this.Rebuild);
      this.Loaded += new RoutedEventHandler(this.OnLoaded);
      this.Unloaded += new RoutedEventHandler(this.OnUnloaded);
      this.DataContextChanged += new DependencyPropertyChangedEventHandler(this.OnDataContextChanged);
      this.dataStorePropertiesValueProvider.Rebuilt += new EventHandler(this.Rebuild);
      this.InitializeComponent();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
      this.Loaded -= new RoutedEventHandler(this.OnLoaded);
      this.Unloaded -= new RoutedEventHandler(this.OnUnloaded);
      this.DataContextChanged -= new DependencyPropertyChangedEventHandler(this.OnDataContextChanged);
      if (this.parentProperty == null)
        return;
      this.parentProperty.PropertyReferenceChanged -= new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnReferenceChanged);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
      this.DealWithValueAreaWrapperRectangle();
      this.dataStorePropertiesValueProvider.PropertyNameEntry = this.PropertyNameEntry;
      this.Rebuild();
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (this.parentProperty != null)
        this.parentProperty.PropertyReferenceChanged -= new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnReferenceChanged);
      SceneNodePropertyValue nodePropertyValue = this.DataContext as SceneNodePropertyValue;
      if (nodePropertyValue != null)
      {
        this.parentProperty = nodePropertyValue.get_ParentProperty() as SceneNodeProperty;
        this.parentProperty.PropertyReferenceChanged += new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnReferenceChanged);
      }
      this.Rebuild();
    }

    private void OnReferenceChanged(object sender, PropertyReferenceChangedEventArgs e)
    {
      this.Rebuild();
    }

    private void EditingPropertyChanged(object sender, EventArgs e)
    {
      this.Rebuild(this, EventArgs.Empty);
    }

    private void Rebuild()
    {
      this.dataStorePropertiesValueProvider.Rebuild();
      SceneNodePropertyValue nodePropertyValue = this.DataContext as SceneNodePropertyValue;
      if (nodePropertyValue == null)
        return;
      string propertyName = (string) null;
      if (this.PropertyNameEntry != null && this.parentProperty != null && (this.parentProperty.SceneNodeObjectSet != null && this.parentProperty.SceneNodeObjectSet.RepresentativeNode != null))
      {
        IType type = this.parentProperty.SceneNodeObjectSet.RepresentativeNode.Type;
        IProperty property;
        for (property = (IProperty) null; property == null && type != null; type = type.BaseType)
          property = type.GetMember(MemberType.LocalProperty, this.PropertyNameEntry, MemberAccessTypes.All) as IProperty;
        if (property != null)
          propertyName = (string) this.parentProperty.SceneNodeObjectSet.RepresentativeSceneNode.GetLocalValue((IPropertyId) property);
      }
      this.dataStorePropertiesValueProvider.EditingProperty = this.dataStorePropertiesValueProvider.FindMatchDataStorePropertyEntry(nodePropertyValue.get_ParentProperty() as SceneNodeProperty, propertyName);
      this.OnPropertyChanged("SelectedProperty");
    }

    private void Rebuild(object sender, EventArgs args)
    {
      this.propertiesView = (ICollectionView) null;
      if (this.dataStorePropertiesValueProvider.Properties != null)
      {
        List<DataStorePropertyEntry> list = new List<DataStorePropertyEntry>();
        foreach (DataStorePropertyEntry storePropertyEntry in (IEnumerable<DataStorePropertyEntry>) this.dataStorePropertiesValueProvider.Properties)
          list.Add(storePropertyEntry);
        this.propertiesView = CollectionViewSource.GetDefaultView((object) list);
        ((ListCollectionView) this.propertiesView).CustomSort = (IComparer) new DataStorePropertyEntrySorter();
        this.propertiesView.GroupDescriptions.Add((GroupDescription) new PropertyGroupDescription()
        {
          PropertyName = "DataSetName"
        });
      }
      this.OnPropertyChanged("CurrentProperty");
      this.OnPropertyChanged("Properties");
    }

    private void DealWithValueAreaWrapperRectangle()
    {
      PropertyContainer propertyContainer = (PropertyContainer) this.VisualParent.GetValue((DependencyProperty) PropertyContainer.OwningPropertyContainerProperty);
      if (propertyContainer == null)
        return;
      Rectangle rectangle = propertyContainer.get_InlineRowTemplate().FindName("ValueAreaWrapperRectangle", (FrameworkElement) propertyContainer) as Rectangle;
      if (rectangle == null)
        return;
      rectangle.Visibility = Visibility.Hidden;
    }

    private void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent(this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/propertyinspector/complexvalueeditors/datastorepropertypickereditor.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      if (connectionId == 1)
        this.DataStorePropertyPickerEditorControl = (DataStorePropertyPickerEditor) target;
      else
        this._contentLoaded = true;
    }
  }
}
