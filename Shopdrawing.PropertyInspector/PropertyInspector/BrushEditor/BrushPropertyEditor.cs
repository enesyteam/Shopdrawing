// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.BrushEditor.BrushPropertyEditor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.BrushEditor
{
  internal abstract class BrushPropertyEditor : INotifyPropertyChanged
  {
    private Dictionary<SceneNodeProperty, PropertyChangedEventHandler> listeningProperties = new Dictionary<SceneNodeProperty, PropertyChangedEventHandler>();
    private BrushEditor brushEditor;
    private SceneNodeProperty basisProperty;

    protected BrushEditor BrushEditor
    {
      get
      {
        return this.brushEditor;
      }
    }

    protected SceneNodeProperty BasisProperty
    {
      get
      {
        return this.basisProperty;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected BrushPropertyEditor(BrushEditor brushEditor, SceneNodeProperty basisProperty)
    {
      this.brushEditor = brushEditor;
      this.basisProperty = basisProperty;
    }

    public virtual void Disassociate()
    {
      foreach (KeyValuePair<SceneNodeProperty, PropertyChangedEventHandler> keyValuePair in this.listeningProperties)
      {
        if (keyValuePair.Value != null)
          keyValuePair.Key.get_PropertyValue().remove_PropertyChanged(keyValuePair.Value);
        keyValuePair.Key.Associated = false;
        keyValuePair.Key.OnRemoveFromCategory();
      }
      this.listeningProperties.Clear();
    }

    protected SceneNodeProperty RequestUpdates(PropertyReference propertyReference, PropertyChangedEventHandler propertyChanged)
    {
      SceneNodeProperty sceneNodeProperty = this.basisProperty.SceneNodeObjectSet.CreateSceneNodeProperty(propertyReference, (AttributeCollection) null);
      sceneNodeProperty.get_PropertyValue().add_PropertyChanged(propertyChanged);
      this.listeningProperties.Add(sceneNodeProperty, propertyChanged);
      return sceneNodeProperty;
    }

    protected void RemovePropertyChangedListener(SceneNodeProperty property)
    {
      PropertyChangedEventHandler changedEventHandler = (PropertyChangedEventHandler) null;
      if (!this.listeningProperties.TryGetValue(property, out changedEventHandler))
        return;
      property.get_PropertyValue().remove_PropertyChanged(changedEventHandler);
      this.listeningProperties.Remove(property);
    }

    protected void OnPropertyChanged(string name)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(name));
    }
  }
}
