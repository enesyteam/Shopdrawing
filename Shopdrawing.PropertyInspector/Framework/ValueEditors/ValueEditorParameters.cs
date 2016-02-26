// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.ValueEditors.ValueEditorParameters
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.Expression.Framework.ValueEditors
{
  public class ValueEditorParameters
  {
    private static Dictionary<string, Type> keyToAttributeType = new Dictionary<string, Type>();
    private AttributeCollection attributes;
    private string subPath;

    public object this[string key]
    {
      get
      {
        if (this.attributes != null)
        {
          Type index = (Type) null;
          if (ValueEditorParameters.keyToAttributeType.TryGetValue(key, out index))
          {
            object obj = this.attributes[index];
            if (obj != null && this.subPath != null)
            {
              IIndexableAttribute iindexableAttribute = obj as IIndexableAttribute;
              if (iindexableAttribute != null)
                return (object) iindexableAttribute.get_Item(this.subPath);
            }
            return obj;
          }
        }
        return null;
      }
    }

    static ValueEditorParameters()
    {
      ValueEditorParameters.RegisterParameterName("NumberRanges", typeof (NumberRangesAttribute));
      ValueEditorParameters.RegisterParameterName("NumberIncrements", typeof (NumberIncrementsAttribute));
      ValueEditorParameters.RegisterParameterName("NumberFormat", typeof (NumberFormatAttribute));
    }

    public ValueEditorParameters(AttributeCollection attributes)
    {
      this.attributes = attributes;
    }

    public ValueEditorParameters(AttributeCollection attributes, string subPath)
    {
      this.attributes = attributes;
      this.subPath = subPath;
    }

    public static bool RegisterParameterName(string name, Type attributeType)
    {
      if (ValueEditorParameters.keyToAttributeType.ContainsKey(name))
        return false;
      ValueEditorParameters.keyToAttributeType.Add(name, attributeType);
      return true;
    }

    public static void OverrideValueEditorParameters(PropertyReferenceProperty baseProperty, PropertyReferenceProperty derivedProperty, string key)
    {
      SceneNodeProperty sceneNodeProperty = baseProperty as SceneNodeProperty;
      if (derivedProperty == null || sceneNodeProperty == null)
        return;
      ValueEditorParameters newParams = new ValueEditorParameters(sceneNodeProperty.Attributes, key);
      if (!(newParams["NumberRanges"] is NumberRangesAttribute))
      {
        NumberRangesAttribute numberRangesAttribute = derivedProperty.Attributes[typeof (NumberRangesAttribute)] as NumberRangesAttribute ?? new NumberRangesAttribute(new double?(), new double?(), new double?(), new double?(), new bool?(false));
        List<Attribute> list = new List<Attribute>();
        foreach (object obj in sceneNodeProperty.Attributes)
          list.Add((Attribute) obj);
        list.Add((Attribute) numberRangesAttribute);
        newParams = new ValueEditorParameters(new AttributeCollection(list.ToArray()), key);
      }
      derivedProperty.OverrideValueEditorParameters(newParams);
    }
  }
}
