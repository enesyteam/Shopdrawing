// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.LocalValueObjectSet
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using System;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class LocalValueObjectSet : ObjectSetBase
  {
    private object localValue;
    private DesignerContext designerContext;
    private IProjectContext projectContext;

    public object LocalValue
    {
      get
      {
        return this.localValue;
      }
      set
      {
        this.localValue = value;
      }
    }

    public override IProjectContext ProjectContext
    {
      get
      {
        return this.projectContext;
      }
    }

    internal override DesignerContext DesignerContext
    {
      get
      {
        return this.designerContext;
      }
    }

    public override int Count
    {
      get
      {
        return this.localValue == null ? 0 : 1;
      }
    }

    public override Type ObjectType
    {
      get
      {
        if (this.localValue == null)
          return (Type) null;
        return this.localValue.GetType();
      }
    }

    public override bool IsHomogenous
    {
      get
      {
        throw new NotImplementedException(ExceptionStringTable.MethodOrOperationIsNotImplemented);
      }
    }

    internal LocalValueObjectSet(object value, DesignerContext designerContext, IProjectContext projectContext)
    {
      this.localValue = value;
      this.designerContext = designerContext;
      this.projectContext = projectContext;
    }

    public override object GetValue(IPropertyId propertyKey)
    {
      return this.GetValue(new PropertyReference((ReferenceStep) this.ProjectContext.ResolveProperty(propertyKey)), PropertyReference.GetValueFlags.Computed);
    }

    public override object GetValue(PropertyReference propertyReference, PropertyReference.GetValueFlags getValueFlags)
    {
      if ((getValueFlags & PropertyReference.GetValueFlags.Computed) != PropertyReference.GetValueFlags.Local)
        return propertyReference.GetCurrentValue(this.localValue);
      return propertyReference.GetBaseValue(this.localValue);
    }

    public override DocumentNode GetRawValue(IDocumentContext documentContext, PropertyReference propertyReference, PropertyReference.GetValueFlags getValueFlags)
    {
      object obj = this.GetValue(propertyReference, getValueFlags);
      if (obj != null)
        return documentContext.CreateNode(obj.GetType(), obj);
      return (DocumentNode) null;
    }

    protected override void ModifyValue(PropertyReferenceProperty property, object valueToSet, Modification modification, int index)
    {
      if (modification == Modification.InsertValue)
        property.Reference.Insert(this.localValue, index, valueToSet);
      else if (modification == Modification.ClearValue)
        property.Reference.ClearValue(this.localValue);
      else if (modification == Modification.RemoveValue)
      {
        property.Reference.RemoveAt(this.localValue, index);
      }
      else
      {
        if (modification != Modification.SetValue)
          return;
        property.Reference.SetValue(this.localValue, valueToSet);
      }
    }
  }
}
