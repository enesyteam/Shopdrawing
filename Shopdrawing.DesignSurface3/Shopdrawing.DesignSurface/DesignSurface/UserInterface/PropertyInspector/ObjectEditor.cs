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
    public static readonly DependencyProperty PropertyValueProperty = DependencyProperty.Register("PropertyValue", typeof (Microsoft.Windows.Design.PropertyEditing.PropertyValue), typeof (ObjectEditor), new PropertyMetadata(new PropertyChangedCallback(ObjectEditor.PropertyValueChanged)));
    private bool _contentLoaded;

    public Microsoft.Windows.Design.PropertyEditing.PropertyValue PropertyValue
    {
      get
      {
        return (Microsoft.Windows.Design.PropertyEditing.PropertyValue) this.GetValue(ObjectEditor.PropertyValueProperty);
      }
      set
      {
        this.SetValue(ObjectEditor.PropertyValueProperty, (object) value);
      }
    }

    public ObjectEditor()
    {
      this.InitializeComponent();
      this.Unloaded += new RoutedEventHandler(this.ObjectEditor_Unloaded);
    }

    private void ObjectEditor_Unloaded(object sender, RoutedEventArgs e)
    {
      this.UpdateHandlers(this.PropertyValue, (Microsoft.Windows.Design.PropertyEditing.PropertyValue) null);
    }

    private static void PropertyValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      ObjectEditor objectEditor = (ObjectEditor) sender;
      Microsoft.Windows.Design.PropertyEditing.PropertyValue newPropertyValue = (Microsoft.Windows.Design.PropertyEditing.PropertyValue) e.NewValue;
      Microsoft.Windows.Design.PropertyEditing.PropertyValue oldPropertyValue = (Microsoft.Windows.Design.PropertyEditing.PropertyValue) e.OldValue;
      objectEditor.UpdateHandlers(oldPropertyValue, newPropertyValue);
      objectEditor.Recache();
    }

    private void UpdateHandlers(Microsoft.Windows.Design.PropertyEditing.PropertyValue oldPropertyValue, Microsoft.Windows.Design.PropertyEditing.PropertyValue newPropertyValue)
    {
      if (oldPropertyValue != null)
      {
        SceneNodeProperty sceneNodeProperty = oldPropertyValue.ParentProperty as SceneNodeProperty;
        if (sceneNodeProperty != null)
        {
          sceneNodeProperty.PropertyReferenceChanged -= new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnPropertyPropertyReferenceChanged);
          sceneNodeProperty.PropertyChanged -= new PropertyChangedEventHandler(this.OnPropertyPropertyChanged);
          this.ChangeActiveEditMode(PropertyContainerEditMode.Inline);
        }
      }
      if (newPropertyValue == null)
        return;
      SceneNodeProperty sceneNodeProperty1 = newPropertyValue.ParentProperty as SceneNodeProperty;
      if (sceneNodeProperty1 == null)
        return;
      sceneNodeProperty1.PropertyReferenceChanged += new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnPropertyPropertyReferenceChanged);
      sceneNodeProperty1.PropertyChanged += new PropertyChangedEventHandler(this.OnPropertyPropertyChanged);
    }

    private void ChangeActiveEditMode(PropertyContainerEditMode editMode)
    {
      PropertyContainer propertyContainer = (PropertyContainer) this.GetValue(PropertyContainer.OwningPropertyContainerProperty);
      if (propertyContainer == null)
        return;
      propertyContainer.ActiveEditMode = editMode;
    }

    private void OnPropertyPropertyReferenceChanged(object sender, PropertyReferenceChangedEventArgs e)
    {
      if (this.PropertyValue == null)
        return;
      SceneNodeProperty sceneNodeProperty = this.PropertyValue.ParentProperty as SceneNodeProperty;
      if (sceneNodeProperty.ValueSource.IsDefaultValue || sceneNodeProperty.ValueSource.IsInheritedValue)
        this.ChangeActiveEditMode(PropertyContainerEditMode.Inline);
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
        this.ItemsSource = (IEnumerable) this.PropertyValue.SubProperties;
      else
        this.ItemsSource = (IEnumerable) null;
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/propertyinspector/complexvalueeditors/objecteditor.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      this._contentLoaded = true;
    }
  }
}
