// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.ObjectSetBase
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.Framework.PropertyInspector;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.ComponentModel;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public abstract class ObjectSetBase : ObjectSet
  {
    public abstract IProjectContext ProjectContext { get; }

    internal abstract DesignerContext DesignerContext { get; }

    public override string ObjectsName
    {
      get
      {
        throw new NotImplementedException(ExceptionStringTable.MethodOrOperationIsNotImplemented);
      }
    }

    public override ObjectSet.PropertiesIterator Properties
    {
      get
      {
        throw new NotImplementedException(ExceptionStringTable.MethodOrOperationIsNotImplemented);
      }
    }

    public override bool IsCategoryPresent(CategoryEntry category)
    {
      throw new NotImplementedException(ExceptionStringTable.MethodOrOperationIsNotImplemented);
    }

    public override bool IsPropertyPresent(PropertyEntry property)
    {
      throw new NotImplementedException(ExceptionStringTable.MethodOrOperationIsNotImplemented);
    }

    public virtual void RegisterPropertyChangedHandler(PropertyReference propertyReference, Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler handler)
    {
    }

    public virtual void UnregisterPropertyChangedHandler(PropertyReference propertyReference, Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler handler)
    {
    }

    public virtual PropertyReferenceProperty CreateProperty(PropertyReference propertyReference, AttributeCollection attributes)
    {
      return new PropertyReferenceProperty(this, propertyReference, attributes);
    }

    public abstract object GetValue(IPropertyId propertyKey);

    public abstract object GetValue(PropertyReference propertyReference, PropertyReference.GetValueFlags getValueFlags);

    public abstract DocumentNode GetRawValue(IDocumentContext documentContext, PropertyReference propertyReference, PropertyReference.GetValueFlags getValueFlags);

    protected abstract void ModifyValue(PropertyReferenceProperty property, object valueToSet, Modification modification, int index);

    public object GetValue(PropertyReferenceProperty property, PropertyReference.GetValueFlags getValueFlags)
    {
      return this.GetValue(property.Reference, getValueFlags);
    }

    public object GetValue(PropertyReferenceProperty property)
    {
      return this.GetValue(property, PropertyReference.GetValueFlags.Computed);
    }

    public void SetValue(PropertyReferenceProperty property, object value)
    {
      this.ModifyValue(property, value, Modification.SetValue, -1);
    }

    public void ClearValue(PropertyReferenceProperty property)
    {
      this.ModifyValue(property, null, Modification.ClearValue, -1);
    }

    public void AddValue(PropertyReferenceProperty property, object value)
    {
      this.ModifyValue(property, value, Modification.InsertValue, -1);
    }

    public void InsertValue(PropertyReferenceProperty property, int index, object value)
    {
      this.ModifyValue(property, value, Modification.InsertValue, index);
    }

    public void RemoveValueAt(PropertyReferenceProperty property, int index)
    {
      this.ModifyValue(property, null, Modification.RemoveValue, index);
    }
  }
}
