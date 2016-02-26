// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.GridLengthEditor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class GridLengthEditor : Grid, INotifyPropertyChanged, IComponentConnector
  {
    private PropertyReferenceProperty editingProperty;
    private double value;
    private GridUnitType gridUnitType;
    internal GridLengthEditor UserControlSelf;
    private bool _contentLoaded;

    public double Value
    {
      get
      {
        return this.value;
      }
      set
      {
        this.value = value;
        this.SetGridLength();
      }
    }

    public GridUnitType GridUnitType
    {
      get
      {
        return this.gridUnitType;
      }
      set
      {
        this.gridUnitType = value;
        this.SetGridLength();
      }
    }

    public ICollection GridUnitValueTypes
    {
      get
      {
        return Microsoft.Expression.DesignModel.Metadata.MetadataStore.GetTypeConverter(typeof (GridUnitType)).GetStandardValues();
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public GridLengthEditor()
    {
      this.DataContextChanged += new DependencyPropertyChangedEventHandler(this.OnDataContextChangedRebuild);
      this.InitializeComponent();
    }

    private void OnDataContextChangedRebuild(object sender, DependencyPropertyChangedEventArgs e)
    {
      PropertyValue propertyValue = this.DataContext as PropertyValue;
      if (this.editingProperty != null)
      {
        this.editingProperty.PropertyReferenceChanged -= new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnEditingPropertyChanged);
        this.editingProperty = (PropertyReferenceProperty) null;
      }
      if (propertyValue != null)
        this.editingProperty = (PropertyReferenceProperty) propertyValue.get_ParentProperty();
      if (this.editingProperty == null)
        return;
      this.editingProperty.PropertyReferenceChanged += new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnEditingPropertyChanged);
      this.Rebuild();
    }

    private void SetGridLength()
    {
      this.editingProperty.SetValue(((SceneNodeProperty) this.editingProperty).SceneNodeObjectSet.ViewModel.DefaultView.ConvertFromWpfValue(new GridLength(this.value, this.gridUnitType)));
    }

    private void OnEditingPropertyChanged(object sender, PropertyReferenceChangedEventArgs e)
    {
      this.Rebuild();
    }

    private void Rebuild()
    {
      if (!this.editingProperty.IsMixedValue)
      {
        object obj = this.editingProperty.GetValue();
        if (PlatformTypes.IsInstance(obj, PlatformTypes.GridLength, (ITypeResolver) ((SceneNodeProperty) this.editingProperty).SceneNodeObjectSet.ProjectContext))
        {
          SceneNodeObjectSet sceneNodeObjectSet = ((SceneNodeProperty) this.editingProperty).SceneNodeObjectSet;
          GridLength gridLength = (GridLength) sceneNodeObjectSet.DesignerContext.PlatformConverter.ConvertToWpf(((SceneNodeProperty) this.editingProperty).SceneNodeObjectSet.DocumentContext, obj);
          this.value = JoltHelper.RoundDouble(sceneNodeObjectSet.ProjectContext, gridLength.Value);
          this.gridUnitType = gridLength.GridUnitType;
          this.OnPropertyChanged("Value");
        }
      }
      else
        this.gridUnitType = GridUnitType.Star;
      this.OnPropertyChanged("GridUnitType");
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
      Application.LoadComponent(this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/propertyinspector/complexvalueeditors/gridlengtheditor.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      if (connectionId == 1)
        this.UserControlSelf = (GridLengthEditor) target;
      else
        this._contentLoaded = true;
    }
  }
}
