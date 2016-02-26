// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Properties.AttachedPropertyMetadata
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.Properties
{
  internal sealed class AttachedPropertyMetadata : IAttachedPropertyMetadata
  {
    private Type browsableTargetType = typeof (object);
    private MethodInfo runtimeGetMethodInfo;
    private MethodInfo referenceGetMethodInfo;
    private AttachedPropertyMetadata.BrowsableFlags browsableFlags;
    private Type browsableAttributePresentType;

    private MethodInfo GetMethodInfo
    {
      get
      {
        return this.referenceGetMethodInfo ?? this.runtimeGetMethodInfo;
      }
    }

    public Type OwnerType
    {
      get
      {
        return this.runtimeGetMethodInfo.DeclaringType;
      }
    }

    public Type PropertyType
    {
      get
      {
        return this.runtimeGetMethodInfo.ReturnType;
      }
    }

    public string Name
    {
      get
      {
        return this.runtimeGetMethodInfo.Name.Substring(3);
      }
    }

    private bool BrowsableForChildren
    {
      get
      {
        return (this.browsableFlags & AttachedPropertyMetadata.BrowsableFlags.Children) != AttachedPropertyMetadata.BrowsableFlags.None;
      }
    }

    private bool BrowsableForAllDescendants
    {
      get
      {
        return (this.browsableFlags & AttachedPropertyMetadata.BrowsableFlags.AllDescendants) != AttachedPropertyMetadata.BrowsableFlags.None;
      }
    }

    private Type BrowsableForType
    {
      get
      {
        return this.browsableTargetType;
      }
    }

    private bool BrowsableWhenAttributePresent
    {
      get
      {
        return (this.browsableFlags & AttachedPropertyMetadata.BrowsableFlags.AttributePresent) != AttachedPropertyMetadata.BrowsableFlags.None;
      }
    }

    private Type BrowsableWhenAttributePresentAttributeType
    {
      get
      {
        return this.browsableAttributePresentType;
      }
    }

    internal AttachedPropertyMetadata(MethodInfo runtimeGetMethodInfo, MethodInfo referenceGetMethodInfo)
    {
      this.runtimeGetMethodInfo = runtimeGetMethodInfo;
      this.referenceGetMethodInfo = referenceGetMethodInfo;
      this.browsableTargetType = runtimeGetMethodInfo.GetParameters()[0].ParameterType;
      this.LazyFillOutBrowsableInfo();
    }

    private void LazyFillOutBrowsableInfo()
    {
      foreach (CustomAttributeData customAttributeData in (IEnumerable<CustomAttributeData>) this.GetMethodInfo.GetCustomAttributesData())
      {
        if (customAttributeData.Constructor.DeclaringType.FullName.Equals(typeof (AttachedPropertyBrowsableForChildrenAttribute).FullName, StringComparison.Ordinal))
        {
          AttachedPropertyBrowsableForChildrenAttribute[] childrenAttributeArray = (AttachedPropertyBrowsableForChildrenAttribute[]) this.runtimeGetMethodInfo.GetCustomAttributes(typeof (AttachedPropertyBrowsableForChildrenAttribute), false);
          AttachedPropertyBrowsableForChildrenAttribute childrenAttribute = childrenAttributeArray.Length != 0 ? childrenAttributeArray[0] : (AttachedPropertyBrowsableForChildrenAttribute) null;
          if (childrenAttribute != null)
          {
            if (childrenAttribute.IncludeDescendants)
              this.browsableFlags = AttachedPropertyMetadata.BrowsableFlags.AllDescendants;
            else
              this.browsableFlags |= AttachedPropertyMetadata.BrowsableFlags.Children;
          }
        }
        else if (customAttributeData.Constructor.DeclaringType.FullName.Equals(typeof (AttachedPropertyBrowsableForTypeAttribute).FullName, StringComparison.Ordinal))
        {
          AttachedPropertyBrowsableForTypeAttribute[] forTypeAttributeArray = (AttachedPropertyBrowsableForTypeAttribute[]) this.runtimeGetMethodInfo.GetCustomAttributes(typeof (AttachedPropertyBrowsableForTypeAttribute), false);
          AttachedPropertyBrowsableForTypeAttribute forTypeAttribute = forTypeAttributeArray.Length != 0 ? forTypeAttributeArray[0] : (AttachedPropertyBrowsableForTypeAttribute) null;
          if (forTypeAttribute != null)
            this.browsableTargetType = forTypeAttribute.TargetType;
        }
        else if (customAttributeData.Constructor.DeclaringType.FullName.Equals(typeof (AttachedPropertyBrowsableWhenAttributePresentAttribute).FullName, StringComparison.Ordinal))
        {
          AttachedPropertyBrowsableWhenAttributePresentAttribute[] presentAttributeArray = (AttachedPropertyBrowsableWhenAttributePresentAttribute[]) this.runtimeGetMethodInfo.GetCustomAttributes(typeof (AttachedPropertyBrowsableWhenAttributePresentAttribute), false);
          AttachedPropertyBrowsableWhenAttributePresentAttribute presentAttribute = presentAttributeArray.Length != 0 ? presentAttributeArray[0] : (AttachedPropertyBrowsableWhenAttributePresentAttribute) null;
          if (presentAttribute != null)
          {
            this.browsableFlags |= AttachedPropertyMetadata.BrowsableFlags.AttributePresent;
            this.browsableAttributePresentType = presentAttribute.AttributeType;
          }
        }
      }
    }

    public bool IsBrowsable(Type typeOfTargetObject, IEnumerable<Type> ancestorChainTypes)
    {
      if (!this.BrowsableForType.IsAssignableFrom(typeOfTargetObject))
        return false;
      if (this.BrowsableForChildren)
      {
        Type c = Enumerable.FirstOrDefault<Type>(ancestorChainTypes);
        if (c == (Type) null || !this.OwnerType.IsAssignableFrom(c))
          return false;
      }
      if (this.BrowsableForAllDescendants)
      {
        bool flag = false;
        foreach (Type c in ancestorChainTypes)
        {
          if (this.OwnerType.IsAssignableFrom(c))
          {
            flag = true;
            break;
          }
        }
        if (!flag)
          return false;
      }
      return !this.BrowsableWhenAttributePresent || typeOfTargetObject.GetCustomAttributes(this.BrowsableWhenAttributePresentAttributeType, true).Length != 0;
    }

    private enum BrowsableFlags
    {
      None = 0,
      Children = 1,
      AllDescendants = 2,
      AttributePresent = 4,
    }
  }
}
