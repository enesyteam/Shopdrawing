// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.TransformEditor.CoalescingSceneNodeProperty
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using Microsoft.Expression.Framework;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.TransformEditor
{
  public class CoalescingSceneNodeProperty : SceneNodeProperty
  {
    private SceneNodeProperty[] properties;

    public CoalescingSceneNodeProperty(SceneNodeObjectSet objectSet, AttributeCollection attributes, IList<SceneNodeProperty> properties)
      : base(objectSet, properties[0].Reference, attributes)
    {
      this.properties = new SceneNodeProperty[properties.Count];
      for (int index = 0; index < properties.Count; ++index)
        this.properties[index] = properties[index];
    }

    public override void Recache()
    {
      for (int index = 0; index < this.properties.Length; ++index)
        this.properties[index].Recache();
    }

    public override void SetValue(object value)
    {
      for (int index = 0; index < this.properties.Length; ++index)
        this.properties[index].SetValue(value);
    }

    public override object GetValue()
    {
      object objB = this.properties[0].GetValue();
      for (int index = 1; index < this.properties.Length; ++index)
      {
        object objA = this.properties[index].GetValue();
        if (!object.Equals(objA, objB))
          return MixedProperty.Mixed;
        objB = objA;
      }
      return objB;
    }
  }
}
