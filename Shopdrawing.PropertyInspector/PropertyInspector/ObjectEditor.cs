// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.ObjectEditor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

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
  public sealed class ObjectEditor : ItemsControl, IComponentConnector
  {
    public static readonly DependencyProperty PropertyValueProperty = DependencyProperty.Register("PropertyValue", typeof (PropertyValue), typeof (ObjectEditor), new PropertyMetadata(new PropertyChangedCallback(ObjectEditor.PropertyValueChanged)));
    private bool _contentLoaded;

    public PropertyValue PropertyValue
    {
      get
      {
        return (PropertyValue) this.GetValue(ObjectEditor.PropertyValueProperty);
      }
      set
      {
        this.SetValue(ObjectEditor.PropertyValueProperty, value);
      }
    }

    public ObjectEditor()
    {
      this.InitializeComponent();
      this.Unloaded += new RoutedEventHandler(this.ObjectEditor_Unloaded);
    }

    private void ObjectEditor_Unloaded(object sender, RoutedEventArgs e)
    {
      this.UpdateHandlers(this.PropertyValue, (PropertyValue) null);
    }

    private static void PropertyValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      ObjectEditor objectEditor = (ObjectEditor) sender;
      PropertyValue newPropertyValue = (PropertyValue) e.NewValue;
      PropertyValue oldPropertyValue = (PropertyValue) e.OldValue;
      objectEditor.UpdateHandlers(oldPropertyValue, newPropertyValue);
      objectEditor.Recache();
    }

    private void UpdateHandlers(PropertyValue oldPropertyValue, PropertyValue newPropertyValue)
    {
      if (oldPropertyValue != null)
      {
        SceneNodeProperty sceneNodeProperty = oldPropertyValue.get_ParentProperty() as SceneNodeProperty;
        if (sceneNodeProperty != null)
        {
          sceneNodeProperty.PropertyReferenceChanged -= new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnPropertyPropertyReferenceChanged);
          sceneNodeProperty.remove_PropertyChanged(new PropertyChangedEventHandler(this.OnPropertyPropertyChanged));
          this.ChangeActiveEditMode((PropertyContainerEditMode) 0);
        }
      }
      if (newPropertyValue == null)
        return;
      SceneNodeProperty sceneNodeProperty1 = newPropertyValue.get_ParentProperty() as SceneNodeProperty;
      if (sceneNodeProperty1 == null)
        return;
      sceneNodeProperty1.PropertyReferenceChanged += new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnPropertyPropertyReferenceChanged);
      sceneNodeProperty1.add_PropertyChanged(new PropertyChangedEventHandler(this.OnPropertyPropertyChanged));
    }

    private void ChangeActiveEditMode(PropertyContainerEditMode editMode)
    {
      PropertyContainer propertyContainer = (PropertyContainer) this.GetValue((DependencyProperty) PropertyContainer.OwningPropertyContainerProperty);
      if (propertyContainer == null)
        return;
      propertyContainer.set_ActiveEditMode(editMode);
    }

    private void OnPropertyPropertyReferenceChanged(object sender, PropertyReferenceChangedEventArgs e)
    {
      if (this.PropertyValue == null)
        return;
      SceneNodeProperty sceneNodeProperty = this.PropertyValue.get_ParentProperty() as SceneNodeProperty;
      if (sceneNodeProperty.ValueSource.get_IsDefaultValue() || sceneNodeProperty.ValueSource.get_IsInheritedValue())
        this.ChangeActiveEditMode((PropertyContainerEditMode) 0);
      if (sceneNodeProperty == null || e.PropertyReference != sceneNodeProperty.Reference)
        return;
      this.Recache();
    }

    private void OnPropertyPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "SubProperties"))
        return;
      this.Recache();
    }

    private void Recache()
    {
      if (this.PropertyValue != null)
        this.ItemsSource = (IEnumerable) this.PropertyValue.get_SubProperties();
      else
        this.ItemsSource = (IEnumerable) null;
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent(this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/propertyinspector/complexvalueeditors/objecteditor.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      this._contentLoaded = true;
    }
  }
}
