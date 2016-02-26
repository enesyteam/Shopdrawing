// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.ValueEditors.ConverterContext
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.Expression.Framework.ValueEditors
{
  public class ConverterContext : ITypeDescriptorContext, IServiceProvider
  {
    private IDictionary<Type, object> serviceTable = (IDictionary<Type, object>) new Dictionary<Type, object>();
    private object instance;

    public IContainer Container
    {
      get
      {
        return (IContainer) null;
      }
    }

    public object Instance
    {
      get
      {
        return this.instance;
      }
    }

    public PropertyDescriptor PropertyDescriptor
    {
      get
      {
        return (PropertyDescriptor) null;
      }
    }

    public ConverterContext(object instance)
    {
      this.instance = instance;
    }

    public void OnComponentChanged()
    {
    }

    public bool OnComponentChanging()
    {
      return false;
    }

    public object GetService(Type serviceType)
    {
      object obj = (object) null;
      this.serviceTable.TryGetValue(serviceType, out obj);
      return obj;
    }

    public void AddService(Type serviceType, object instance)
    {
      this.serviceTable.Add(serviceType, instance);
    }
  }
}
