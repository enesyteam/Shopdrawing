// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.ComplexValueEditorBase
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Windows.Design.PropertyEditing;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class ComplexValueEditorBase : Grid, INotifyPropertyChanged
  {
    protected SceneNodePropertyValue EditingValue { get; private set; }

    protected SceneNodeProperty EditingProperty { get; private set; }

    protected IProjectContext ProjectContext
    {
      get
      {
        if (this.EditingProperty != null)
          return this.EditingProperty.SceneNodeObjectSet.ProjectContext;
        return (IProjectContext) null;
      }
    }

    protected SceneViewModel ViewModel
    {
      get
      {
        if (this.EditingProperty != null)
          return this.EditingProperty.SceneNodeObjectSet.ViewModel;
        return (SceneViewModel) null;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public ComplexValueEditorBase()
    {
      this.DataContextChanged += new DependencyPropertyChangedEventHandler(this.OnDataContextChangedImpl);
    }

    private void OnDataContextChangedImpl(object sender, DependencyPropertyChangedEventArgs args)
    {
      this.EditingValue = this.DataContext as SceneNodePropertyValue;
      if (this.EditingProperty != null)
      {
        this.EditingProperty.PropertyReferenceChanged -= new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnEditingPropertyChangedImpl);
        this.EditingProperty = (SceneNodeProperty) null;
      }
      if (this.EditingValue != null)
        this.EditingProperty = (SceneNodeProperty) this.EditingValue.ParentProperty;
      if (this.EditingProperty != null)
      {
        this.EditingProperty.PropertyReferenceChanged += new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnEditingPropertyChangedImpl);
        this.Rebuild();
      }
      this.OnDataContextChanged();
    }

    protected virtual void OnEditingPropertyChanged()
    {
    }

    private void OnEditingPropertyChangedImpl(object sender, PropertyReferenceChangedEventArgs args)
    {
      this.Rebuild();
      this.OnEditingPropertyChanged();
    }

    protected virtual void OnDataContextChanged()
    {
    }

    protected virtual void Rebuild()
    {
    }

    protected void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    protected void SuppressValueAreaWrapper()
    {
      SceneNodeProperty editingProperty = this.EditingProperty;
      PropertyContainer propertyContainer = (PropertyContainer) this.VisualParent.GetValue(PropertyContainer.OwningPropertyContainerProperty);
      if (propertyContainer == null)
        return;
      Rectangle rectangle = propertyContainer.InlineRowTemplate.FindName("ValueAreaWrapperRectangle", (FrameworkElement) propertyContainer) as Rectangle;
      if (rectangle == null)
        return;
      if (editingProperty.IsValueDataBound || !editingProperty.IsExpression)
        rectangle.Visibility = Visibility.Hidden;
      else
        rectangle.Visibility = Visibility.Visible;
    }
  }
}
