// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.View.SilverlightNameScope
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.Diagnostics;
using System.Collections.Generic;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignSurface.View
{
  [Prototype]
  public sealed class SilverlightNameScope : INameScope
  {
    private Dictionary<string, object> items = new Dictionary<string, object>();

    internal Dictionary<string, object> InternalDictionary
    {
      get
      {
        return this.items;
      }
    }

    public object FindName(string name)
    {
      object obj = (object) null;
      this.items.TryGetValue(name, out obj);
      return obj;
    }

    public void RegisterName(string name, object scopedElement)
    {
      this.items[name] = scopedElement;
    }

    public void UnregisterName(string name)
    {
      this.items.Remove(name);
    }
  }
}
