// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.PropertyPickerEditor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class PropertyPickerEditor : Grid, INotifyPropertyChanged, IComponentConnector
  {
    private ICollectionView propertiesView;
    private EditingPropertyManager contextHelper;
    internal PropertyPickerEditor PropertyPickerEditorControl;
    private bool _contentLoaded;

    public ICollectionView Properties
    {
      get
      {
        return this.propertiesView;
      }
    }

    public IPropertyInformation CurrentProperty
    {
      get
      {
        return this.contextHelper.CurrentProperty;
      }
      set
      {
        this.contextHelper.CurrentProperty = value;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public PropertyPickerEditor()
    {
      this.contextHelper = new EditingPropertyManager((FrameworkElement) this);
      this.contextHelper.Rebuilt += new EventHandler(this.Rebuild);
      this.InitializeComponent();
    }

    private void Rebuild(object sender, EventArgs args)
    {
      this.propertiesView = (ICollectionView) null;
      if (this.contextHelper.Properties != null)
      {
        this.propertiesView = CollectionViewSource.GetDefaultView((object) this.contextHelper.Properties);
        this.propertiesView.GroupDescriptions.Add((GroupDescription) new PropertyGroupDescription()
        {
          PropertyName = "GroupBy"
        });
        this.propertiesView.SortDescriptions.Add(new SortDescription());
      }
      PropertyContainer propertyContainer = (PropertyContainer) this.GetValue(PropertyContainer.OwningPropertyContainerProperty);
      if (propertyContainer != null)
      {
        propertyContainer.SetValue(EditingPropertyManager.EditingPropertyManagerProperty, (object) this.contextHelper);
        propertyContainer.ActiveEditMode = PropertyContainerEditMode.ExtendedPinned;
      }
      this.OnPropertyChanged("CurrentProperty");
      this.OnPropertyChanged("Properties");
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
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/propertyinspector/complexvalueeditors/propertypickereditor.xaml", UriKind.Relative));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerNonUserCode]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      if (connectionId == 1)
        this.PropertyPickerEditorControl = (PropertyPickerEditor) target;
      else
        this._contentLoaded = true;
    }
  }
}
