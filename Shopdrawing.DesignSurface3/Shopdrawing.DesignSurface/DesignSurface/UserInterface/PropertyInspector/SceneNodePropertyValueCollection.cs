// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.SceneNodePropertyValueCollection
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.PropertyInspector;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Controls;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class SceneNodePropertyValueCollection : PropertyValueCollection
  {
    private List<SceneNodeProperty> children;
    private SceneNodeProperty parentProperty;
    private SceneNodeCollectionObjectSet objectSet;
    private bool isValidSelection;

    public override PropertyValue this[int index]
    {
      get
      {
        return this.children[index].PropertyValue;
      }
    }

    public override int Count
    {
      get
      {
        return this.children.Count;
      }
    }

    public SceneNodePropertyValueCollection(SceneNodeProperty parentProperty, SceneNodePropertyValue parentValue)
      : base((PropertyValue) parentValue)
    {
      if (parentProperty == null)
        throw new ArgumentNullException("parentProperty");
      this.parentProperty = parentProperty;
      this.objectSet = new SceneNodeCollectionObjectSet(parentProperty);
      this.parentProperty.PropertyReferenceChanged += new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnPropertyReferenceChanged);
      this.children = new List<SceneNodeProperty>();
      this.Rebuild();
    }

    public override PropertyValue Add(object value)
    {
      return this.Insert(value, this.Count);
    }

    public override PropertyValue Insert(object value, int index)
    {
      SceneViewModel viewModel = this.parentProperty.SceneNodeObjectSet.ViewModel;
      if (!this.isValidSelection)
        return (PropertyValue) null;
      if (value != null && value is UserControl)
        value = (object) viewModel.CreateSceneNode(value.GetType()).DocumentNode;
      int count = this.Count;
      this.parentProperty.InsertValue(index, value);
      this.Rebuild();
      if (count + 1 == this.Count)
        return this[index];
      return (PropertyValue) null;
    }

    public override bool Remove(PropertyValue propertyValue)
    {
      for (int index = 0; index < this.children.Count; ++index)
      {
        if (this.children[index].PropertyValue == propertyValue)
        {
          this.RemoveAt(index);
          return true;
        }
      }
      return false;
    }

    public override void RemoveAt(int index)
    {
      this.parentProperty.RemoveValueAt(index);
      this.Rebuild();
    }

    public override void SetIndex(int currentIndex, int newIndex)
    {
      if (currentIndex < 0 || currentIndex >= this.Count)
        throw new ArgumentOutOfRangeException("currentIndex");
      if (newIndex < 0 || newIndex >= this.Count)
        throw new ArgumentOutOfRangeException("newIndex");
      bool isMixed;
      DocumentNode valueAsDocumentNode = ((SceneNodeProperty) this[currentIndex].ParentProperty).GetLocalValueAsDocumentNode(true, out isMixed);
      if (isMixed)
        return;
      this.parentProperty.RemoveValueAt(currentIndex);
      this.parentProperty.InsertValue(newIndex, (object) valueAsDocumentNode);
      this.Rebuild();
    }

    public override IEnumerator<PropertyValue> GetEnumerator()
    {
      foreach (PropertyEntry propertyEntry in this.children)
        yield return propertyEntry.PropertyValue;
    }

    public void Rebuild()
    {
      bool isMixed = false;
      DocumentCompositeNode valueAsDocumentNode = SceneNodePropertyValueCollection.GetLocalValueAsDocumentNode(this.parentProperty, false, out isMixed);
      int num;
      if (isMixed || valueAsDocumentNode != null && !this.parentProperty.PropertyType.IsAssignableFrom(valueAsDocumentNode.TargetType))
      {
        num = 0;
        this.isValidSelection = false;
      }
      else
      {
        this.isValidSelection = true;
        num = valueAsDocumentNode == null || !valueAsDocumentNode.SupportsChildren ? 0 : valueAsDocumentNode.Children.Count;
      }
      this.objectSet.RebuildObjects();
      foreach (PropertyBase propertyBase in this.children)
        propertyBase.OnRemoveFromCategory();
      this.children.Clear();
      for (int index = 0; index < num; ++index)
        this.children.Add(this.CreatePropertyForIndex(index));
      this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    private SceneNodeProperty CreatePropertyForIndex(int index)
    {
      return (SceneNodeProperty) this.objectSet.CreateProperty(new PropertyReference((ReferenceStep) IndexedClrPropertyReferenceStep.GetReferenceStep((ITypeResolver) this.objectSet.ViewModel.ProjectContext, this.parentProperty.PropertyType, index)), (AttributeCollection) null);
    }

    private void OnPropertyReferenceChanged(object sender, PropertyReferenceChangedEventArgs e)
    {
      if (e.PropertyReference.Count - 1 > this.parentProperty.Reference.Count)
        return;
      this.Rebuild();
    }

    public void Unhook()
    {
      if (this.children != null)
      {
        for (int index = 0; index < this.children.Count; ++index)
          this.children[index].OnRemoveFromCategory();
        this.children = (List<SceneNodeProperty>) null;
      }
      if (this.objectSet == null)
        return;
      this.objectSet.Dispose();
      this.objectSet = (SceneNodeCollectionObjectSet) null;
      this.parentProperty.PropertyReferenceChanged -= new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnPropertyReferenceChanged);
    }

    private static DocumentCompositeNode GetLocalValueAsDocumentNode(SceneNodeProperty parentProperty, bool resolved, out bool isMixed)
    {
      GetLocalValueFlags flags = resolved ? GetLocalValueFlags.Resolve : GetLocalValueFlags.None;
      return parentProperty.SceneNodeObjectSet.GetLocalValueAsDocumentNode(parentProperty, flags, out isMixed) as DocumentCompositeNode;
    }
  }
}
