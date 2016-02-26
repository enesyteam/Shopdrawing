// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.TypedSceneNodeProperty
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.ComponentModel;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class TypedSceneNodeProperty : SceneNodeProperty
  {
    private Type valueType;
    private IType valueTypeId;

    public override Type PropertyType
    {
      get
      {
        return this.valueType;
      }
    }

    public override IType PropertyTypeId
    {
      get
      {
        return this.valueTypeId;
      }
    }

    public TypedSceneNodeProperty(SceneNodeObjectSet objectSet, PropertyReference propertyReference, AttributeCollection attributeCollection, Type valueType, ITypeResolver typeResolver)
      : base(objectSet, propertyReference, attributeCollection, (PropertyValue) null, valueType)
    {
      this.valueType = valueType;
      this.valueTypeId = typeResolver.GetType(valueType);
    }
  }
}
