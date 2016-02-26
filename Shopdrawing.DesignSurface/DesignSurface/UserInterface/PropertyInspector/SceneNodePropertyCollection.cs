// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.SceneNodePropertyCollection
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.PropertyInspector;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class SceneNodePropertyCollection : PropertyEntryCollection, INotifyCollectionChanged, INotifyPropertyChanged
  {
    private List<SceneNodeProperty> subProperties = new List<SceneNodeProperty>();
    private SceneNodeProperty parentProperty;
    private SceneNodeCollectionObjectSet objectSet;

    public override PropertyEntry this[string propertyName]
    {
      get
      {
        foreach (PropertyEntry propertyEntry in this.subProperties)
        {
          if (propertyEntry.PropertyName == propertyName)
            return propertyEntry;
        }
        return (PropertyEntry) null;
      }
    }

    public override int Count
    {
      get
      {
        return this.subProperties.Count;
      }
    }

    public event NotifyCollectionChangedEventHandler CollectionChanged;

    public event PropertyChangedEventHandler PropertyChanged;

    public SceneNodePropertyCollection(SceneNodeProperty parentProperty, SceneNodePropertyValue parentValue)
      : base((PropertyValue) parentValue)
    {
      if (parentProperty == null)
        throw new ArgumentNullException("parentProperty");
      this.parentProperty = parentProperty;
      this.parentProperty.PropertyReferenceChanged += new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnPropertyReferenceChanged);
      this.objectSet = new SceneNodeCollectionObjectSet(this.parentProperty);
      this.Rebuild();
    }

    public override IEnumerator<PropertyEntry> GetEnumerator()
    {
      return (IEnumerator<PropertyEntry>) new SceneNodePropertyCollection.SubPropertyEnumerator((IEnumerator<SceneNodeProperty>) this.subProperties.GetEnumerator());
    }

    public void Rebuild()
    {
      foreach (PropertyBase propertyBase in this.subProperties)
        propertyBase.Associated = false;
      PropertyDescriptorCollection descriptorCollection = (PropertyDescriptorCollection) null;
      Type targetType = this.parentProperty.PropertyType;
      bool isMixed;
      DocumentNode valueAsDocumentNode = this.parentProperty.GetLocalValueAsDocumentNode(true, out isMixed);
      if (valueAsDocumentNode == null || !(valueAsDocumentNode is DocumentPrimitiveNode) && !valueAsDocumentNode.Type.IsBinding)
      {
        TypeConverter converterFromAttributes = MetadataStore.GetTypeConverterFromAttributes(targetType.Assembly, this.parentProperty.Attributes);
        if (converterFromAttributes != null)
        {
          object obj = this.parentProperty.SceneNodeObjectSet.GetValue(this.parentProperty.Reference, this.parentProperty.IsExpression ? PropertyReference.GetValueFlags.Computed : PropertyReference.GetValueFlags.Local);
          if (obj != null && obj != MixedProperty.Mixed)
          {
            descriptorCollection = converterFromAttributes.GetProperties(obj);
            targetType = obj.GetType();
          }
        }
        if (descriptorCollection == null)
        {
          object component = this.parentProperty.SceneNodeObjectSet.GetValue(this.parentProperty.Reference, this.parentProperty.IsExpression ? PropertyReference.GetValueFlags.Computed : PropertyReference.GetValueFlags.Local);
          if (component != null && component != MixedProperty.Mixed)
          {
            descriptorCollection = TypeUtilities.GetProperties(component);
            targetType = component.GetType();
          }
        }
      }
      this.objectSet.RebuildObjects();
      IProjectContext projectContext = this.parentProperty.SceneNodeObjectSet.ProjectContext;
      if (projectContext != null && descriptorCollection != null)
      {
        SceneNode[] objects = this.objectSet.Objects;
        foreach (PropertyDescriptor propertyDescriptor in descriptorCollection)
        {
          ReferenceStep referenceStep = PlatformTypeHelper.GetProperty((ITypeResolver) projectContext, targetType, propertyDescriptor) as ReferenceStep;
          if (referenceStep != null)
          {
            TargetedReferenceStep targetedReferenceStep = new TargetedReferenceStep(referenceStep, this.objectSet.ObjectTypeId);
            if (PropertyInspectorModel.IsPropertyBrowsable(objects, targetedReferenceStep) && PropertyInspectorModel.IsAttachedPropertyBrowsable(objects, this.objectSet.ObjectTypeId, targetedReferenceStep, (ITypeResolver) this.parentProperty.SceneNodeObjectSet.ProjectContext))
            {
              PropertyReference propertyReference = new PropertyReference(referenceStep);
              SceneNodeProperty property = this.FindProperty(propertyReference);
              if (property == null)
              {
                SceneNodeProperty sceneNodeProperty = this.objectSet.CreateProperty(propertyReference, referenceStep.Attributes) as SceneNodeProperty;
                int index = this.subProperties.BinarySearch(sceneNodeProperty, (IComparer<SceneNodeProperty>) new SceneNodePropertyCollection.PropertyNameComparer());
                if (index < 0)
                  index = ~index;
                this.subProperties.Insert(index, sceneNodeProperty);
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, (object) sceneNodeProperty, index));
              }
              else if (!property.UpdateAndRefresh(propertyReference, referenceStep.Attributes, (Type) null))
                property.Associated = true;
            }
          }
        }
      }
      for (int index = this.subProperties.Count - 1; index >= 0; --index)
      {
        if (!this.subProperties[index].Associated)
        {
          PropertyEntry propertyEntry = (PropertyEntry) this.subProperties[index];
          this.subProperties[index].OnRemoveFromCategory();
          this.subProperties.RemoveAt(index);
          this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, (object) propertyEntry, index));
        }
      }
      this.OnPropertyChanged("Item[]");
    }

    private SceneNodeProperty FindProperty(PropertyReference propertyReference)
    {
      foreach (SceneNodeProperty sceneNodeProperty in this.subProperties)
      {
        if (sceneNodeProperty.Reference.Path == propertyReference.Path)
          return sceneNodeProperty;
      }
      return (SceneNodeProperty) null;
    }

    private void OnPropertyReferenceChanged(object sender, PropertyReferenceChangedEventArgs e)
    {
      if (e.PropertyReference.Count > this.parentProperty.Reference.Count)
        return;
      this.Rebuild();
    }

    public void Unhook()
    {
      foreach (SceneNodeProperty sceneNodeProperty in this.subProperties)
      {
        sceneNodeProperty.Associated = false;
        sceneNodeProperty.OnRemoveFromCategory();
      }
      this.subProperties.Clear();
      if (this.objectSet == null)
        return;
      this.objectSet.Dispose();
      this.objectSet = (SceneNodeCollectionObjectSet) null;
    }

    protected void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
    {
      if (this.CollectionChanged == null)
        return;
      this.CollectionChanged((object) this, args);
    }

    protected void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    private class PropertyNameComparer : System.Collections.Generic.Comparer<SceneNodeProperty>
    {
      public override int Compare(SceneNodeProperty x, SceneNodeProperty y)
      {
        return string.Compare(x.PropertyName, y.PropertyName, StringComparison.Ordinal);
      }
    }

    private struct SubPropertyEnumerator : IEnumerator<PropertyEntry>, IDisposable, IEnumerator
    {
      private IEnumerator<SceneNodeProperty> current;

      public PropertyEntry Current
      {
        get
        {
          return (PropertyEntry) this.current.Current;
        }
      }

      object IEnumerator.Current
      {
        get
        {
          return (object) this.Current;
        }
      }

      public SubPropertyEnumerator(IEnumerator<SceneNodeProperty> current)
      {
        this.current = current;
      }

      public bool MoveNext()
      {
        return this.current.MoveNext();
      }

      public void Reset()
      {
        this.current.Reset();
      }

      void IDisposable.Dispose()
      {
        this.current.Dispose();
      }
    }
  }
}
