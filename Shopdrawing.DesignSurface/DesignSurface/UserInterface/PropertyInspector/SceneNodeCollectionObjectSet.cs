// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.SceneNodeCollectionObjectSet
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class SceneNodeCollectionObjectSet : SceneNodeObjectSetBase
  {
    private SceneNode[] objects;
    private SceneNodeObjectSet parent;
    private SceneNodeProperty parentProperty;
    private PropertyReference baseReference;
    private ObservableCollection<LocalResourceModel> localResources;
    private Dictionary<Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler, PropertyReference> registeredReferences;

    public override bool PropertyUpdatesLocked
    {
      get
      {
        return this.parent.PropertyUpdatesLocked;
      }
      set
      {
        this.parent.PropertyUpdatesLocked = value;
      }
    }

    public override bool ShouldWalkParentsForGetValue
    {
      get
      {
        return this.parent.ShouldWalkParentsForGetValue;
      }
    }

    public override bool ShouldAllowAnimation
    {
      get
      {
        return this.parent.ShouldAllowAnimation;
      }
    }

    public PropertyReference BaseReference
    {
      get
      {
        return this.baseReference;
      }
    }

    public override SceneDocument Document
    {
      get
      {
        return this.parent.Document;
      }
    }

    public override IDocumentContext DocumentContext
    {
      get
      {
        return this.parent.DocumentContext;
      }
    }

    public override IProjectContext ProjectContext
    {
      get
      {
        return this.parent.ProjectContext;
      }
    }

    public override SceneViewModel ViewModel
    {
      get
      {
        return this.parent.ViewModel;
      }
    }

    public override SceneNode[] Objects
    {
      get
      {
        return this.objects;
      }
    }

    public override bool CanSetBindingExpression
    {
      get
      {
        return this.parent.CanSetBindingExpression;
      }
    }

    public override bool CanSetDynamicExpression
    {
      get
      {
        return this.parent.CanSetDynamicExpression;
      }
    }

    public override bool IsHomogenous
    {
      get
      {
        if (this.objects == null || this.objects.Length <= 1)
          return true;
        IType type = this.objects[0].Type;
        for (int index = 1; index < this.objects.Length - 1; ++index)
        {
          if (!this.objects[index].Type.Equals((object) type))
            return false;
        }
        return true;
      }
    }

    public override bool IsValidForUpdate
    {
      get
      {
        SceneNode[] sceneNodeArray = this.objects;
        int index = 0;
        if (index < sceneNodeArray.Length)
          return sceneNodeArray[index].DesignerContext != null;
        return true;
      }
    }

    public override ObservableCollection<LocalResourceModel> LocalResources
    {
      get
      {
        return this.parent.LocalResources;
      }
    }

    private Dictionary<Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler, PropertyReference> RegisteredReferences
    {
      get
      {
        if (this.registeredReferences == null)
          this.registeredReferences = new Dictionary<Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler, PropertyReference>();
        return this.registeredReferences;
      }
    }

    public SceneNodeCollectionObjectSet(SceneNodeProperty parentProperty)
      : base(parentProperty.SceneNodeObjectSet.DesignerContext, parentProperty.SceneNodeObjectSet.TransactionContext)
    {
      this.parent = parentProperty.SceneNodeObjectSet;
      this.parent.PropertyChanged += new PropertyChangedEventHandler(this.parent_PropertyChanged);
      this.parent.SetChanged += new EventHandler(this.parent_SetChanged);
      this.parentProperty = parentProperty;
      this.RebuildObjects();
      this.UpdateLocalResources();
    }

    public override void Dispose()
    {
      base.Dispose();
      this.parent.PropertyChanged -= new PropertyChangedEventHandler(this.parent_PropertyChanged);
      this.parent.SetChanged -= new EventHandler(this.parent_SetChanged);
      if (this.localResources == null)
        return;
      this.localResources.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.LocalResources_CollectionChanged);
      this.localResources = (ObservableCollection<LocalResourceModel>) null;
    }

    public override SceneNodeProperty CreateSceneNodeProperty(PropertyReference propertyReference, AttributeCollection attributes)
    {
      SceneNodeCollectionProperty collectionProperty = new SceneNodeCollectionProperty(this, propertyReference, attributes, this.parentProperty.PropertyValue);
      collectionProperty.Recache();
      return (SceneNodeProperty) collectionProperty;
    }

    public override void RegisterPropertyChangedHandler(PropertyReference propertyReference, Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler handler)
    {
      SceneNodeObjectSet sceneNodeObjectSet = (SceneNodeObjectSet) this;
      for (SceneNodeCollectionObjectSet collectionObjectSet = this; collectionObjectSet != null; collectionObjectSet = sceneNodeObjectSet as SceneNodeCollectionObjectSet)
        sceneNodeObjectSet = collectionObjectSet.parent;
      PropertyReference propertyReference1 = this.baseReference.Append(propertyReference);
      this.RegisteredReferences[handler] = propertyReference1;
      sceneNodeObjectSet.RegisterPropertyChangedHandler(propertyReference1, handler);
    }

    public override void UnregisterPropertyChangedHandler(PropertyReference propertyReference, Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler handler)
    {
      SceneNodeObjectSet sceneNodeObjectSet = (SceneNodeObjectSet) this;
      for (SceneNodeCollectionObjectSet collectionObjectSet = this; collectionObjectSet != null; collectionObjectSet = sceneNodeObjectSet as SceneNodeCollectionObjectSet)
        sceneNodeObjectSet = collectionObjectSet.parent;
      PropertyReference propertyReference1;
      if (!this.RegisteredReferences.TryGetValue(handler, out propertyReference1))
        return;
      this.RegisteredReferences.Remove(handler);
      sceneNodeObjectSet.UnregisterPropertyChangedHandler(propertyReference1, handler);
    }

    public override object GetValue(PropertyReference propertyReference, PropertyReference.GetValueFlags getValueFlags)
    {
      try
      {
        if (this.objects.Length == 0)
          return this.parent.GetValue(this.baseReference.Append(propertyReference), getValueFlags);
        return base.GetValue(propertyReference, getValueFlags);
      }
      catch (IndexOutOfRangeException ex)
      {
      }
      catch (ArgumentOutOfRangeException ex)
      {
      }
      catch (TargetInvocationException ex)
      {
        if (ex.InnerException != null)
        {
          if (!(ex.InnerException is IndexOutOfRangeException))
          {
            if (!(ex.InnerException is ArgumentOutOfRangeException))
              throw;
          }
        }
      }
      return propertyReference.LastStep.GetDefaultValue(this.ObjectType);
    }

    protected override void ModifyValue(PropertyReferenceProperty property, object valueToSet, Modification modification, int index)
    {
      if (this.objects.Length == 0)
      {
        using (this.ViewModel.ForceDefaultSetValue())
          this.parent.SetValue((PropertyReferenceProperty) this.parentProperty, this.parent.GetValue(this.baseReference, PropertyReference.GetValueFlags.Computed));
        this.RebuildObjects();
      }
      bool treeModified = false;
      SceneEditTransaction sceneEditTransaction = (SceneEditTransaction) null;
      try
      {
        sceneEditTransaction = this.parent.PrepareTreeForModifyValue(this.parentProperty.Reference.Append(property.Reference), valueToSet, modification, out treeModified);
        if (treeModified)
          this.RebuildObjects();
        base.ModifyValue(property, valueToSet, modification, index);
      }
      finally
      {
        if (sceneEditTransaction != null)
        {
          sceneEditTransaction.Commit();
          sceneEditTransaction.Dispose();
        }
      }
    }

    private void UpdateLocalResources()
    {
      if (this.localResources != null)
        this.localResources.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.LocalResources_CollectionChanged);
      this.localResources = this.parent.LocalResources;
      if (this.localResources != null)
        this.localResources.CollectionChanged += new NotifyCollectionChangedEventHandler(this.LocalResources_CollectionChanged);
      this.InvalidateLocalResourcesCache(true);
    }

    private void parent_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "LocalResources"))
        return;
      this.UpdateLocalResources();
    }

    private void parent_SetChanged(object sender, EventArgs e)
    {
      this.RebuildObjects();
      this.OnSetChanged();
    }

    public void RebuildObjects()
    {
      List<SceneNode> list = new List<SceneNode>();
      int length = this.parent.Objects.Length;
      for (int index = 0; index < length; ++index)
      {
        using (this.parent.ViewModel.ForceBaseValue())
        {
          SceneNode valueAsSceneNode = this.parent.Objects[index].GetLocalValueAsSceneNode(this.parentProperty.Reference);
          if (valueAsSceneNode != null)
            list.Add(valueAsSceneNode);
        }
      }
      this.objects = list.ToArray();
      if (this.baseReference != null && this.baseReference.PlatformMetadata == this.parentProperty.Reference.PlatformMetadata)
        return;
      SceneNodeCollectionObjectSet collectionObjectSet = this.parentProperty.SceneNodeObjectSet as SceneNodeCollectionObjectSet;
      if (collectionObjectSet != null)
        this.baseReference = collectionObjectSet.BaseReference.Append(this.parentProperty.Reference);
      else
        this.baseReference = this.parentProperty.Reference;
    }

    private void LocalResources_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.InvalidateLocalResourcesCache(true);
    }
  }
}
