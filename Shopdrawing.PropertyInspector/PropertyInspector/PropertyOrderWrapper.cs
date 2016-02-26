// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.PropertyOrderWrapper
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Windows.Design;
using Microsoft.Windows.Design.PropertyEditing;
using System;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public sealed class PropertyOrderWrapper : IComparable, IComparable<PropertyOrder>
  {
    private PropertyOrder propertyOrder;

    public PropertyOrderWrapper(PropertyOrder propertyOrder)
    {
      this.propertyOrder = propertyOrder;
    }

    public static implicit operator PropertyOrder(PropertyOrderWrapper wrapper)
    {
      return wrapper.propertyOrder;
    }

    public static implicit operator PropertyOrderWrapper(PropertyOrder order)
    {
      return new PropertyOrderWrapper(order);
    }

    public static bool operator ==(PropertyOrderWrapper order1, PropertyOrderWrapper order2)
    {
      bool flag1 = order1 == null;
      bool flag2 = order2 == null;
      if (flag1 && flag2)
        return true;
      if (!flag1 && !flag2)
        return order1.CompareTo((PropertyOrder) order2) == 0;
      return false;
    }

    public static bool operator !=(PropertyOrderWrapper order1, PropertyOrderWrapper order2)
    {
      bool flag1 = order1 == null;
      bool flag2 = order2 == null;
      if (flag1 != flag2)
        return true;
      if (!flag1)
        return order1.CompareTo((PropertyOrder) order2) != 0;
      return false;
    }

    public static bool operator >(PropertyOrderWrapper order1, PropertyOrderWrapper order2)
    {
      if (order1 != (PropertyOrderWrapper) null)
        return order1.CompareTo((PropertyOrder) order2) > 0;
      return false;
    }

    public static bool operator <(PropertyOrderWrapper order1, PropertyOrderWrapper order2)
    {
      if (order1 != (PropertyOrderWrapper) null)
        return order1.CompareTo((PropertyOrder) order2) < 0;
      return false;
    }

    public override bool Equals(object obj)
    {
      return this.CompareTo(obj) == 0;
    }

    public override int GetHashCode()
    {
      return (this.propertyOrder).GetHashCode();
    }

    public int CompareTo(PropertyOrder other)
    {
      return ((OrderToken) this.propertyOrder).CompareTo((OrderToken) other);
    }

    public int CompareTo(object obj)
    {
      if (obj == null)
        return 1;
      PropertyOrderWrapper propertyOrderWrapper = obj as PropertyOrderWrapper;
      if (propertyOrderWrapper == (PropertyOrderWrapper) null)
        throw new ArgumentException("PropertyOrderWrapper.CompareTo must be called with a PropertyOrderWrapper");
      return this.CompareTo(propertyOrderWrapper.propertyOrder);
    }
  }
}
