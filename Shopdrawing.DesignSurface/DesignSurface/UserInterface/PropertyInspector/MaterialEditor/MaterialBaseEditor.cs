// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.MaterialEditor.MaterialBaseEditor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using System;
using System.ComponentModel;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.MaterialEditor
{
  public abstract class MaterialBaseEditor : INotifyPropertyChanged
  {
    private SceneNodeProperty materialProperty;
    private SceneNodeProperty brushProperty;
    private SceneNodeProperty colorProperty;

    public abstract string Name { get; }

    public abstract string ColorName { get; }

    public abstract Type Type { get; }

    public DataTemplate EditorTemplate
    {
      get
      {
        DataTemplate dataTemplate = new DataTemplate();
        dataTemplate.VisualTree = new FrameworkElementFactory(typeof (MaterialBaseEditorControl));
        return dataTemplate;
      }
    }

    public SceneNodeProperty MaterialProperty
    {
      get
      {
        return this.materialProperty;
      }
    }

    public SceneNodeProperty BrushProperty
    {
      get
      {
        return this.brushProperty;
      }
      protected set
      {
        this.brushProperty = value;
        this.OnPropertyChanged("BrushProperty");
      }
    }

    public SceneNodeProperty ColorProperty
    {
      get
      {
        return this.colorProperty;
      }
      protected set
      {
        this.colorProperty = value;
        this.OnPropertyChanged("ColorProperty");
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected MaterialBaseEditor(SceneNodeObjectSet objectSet, PropertyReference materialReference)
    {
      this.materialProperty = objectSet.CreateSceneNodeProperty(materialReference, (AttributeCollection) null);
    }

    public virtual void Unhook()
    {
      if (this.materialProperty != null)
      {
        this.materialProperty.OnRemoveFromCategory();
        this.materialProperty = (SceneNodeProperty) null;
      }
      if (this.BrushProperty != null)
      {
        this.BrushProperty.OnRemoveFromCategory();
        this.BrushProperty = (SceneNodeProperty) null;
      }
      if (this.ColorProperty == null)
        return;
      this.ColorProperty.OnRemoveFromCategory();
      this.ColorProperty = (SceneNodeProperty) null;
    }

    protected void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
