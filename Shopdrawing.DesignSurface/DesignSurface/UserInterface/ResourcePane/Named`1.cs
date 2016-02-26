// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.ResourcePane.Named`1
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

namespace Microsoft.Expression.DesignSurface.UserInterface.ResourcePane
{
  public class Named<T>
  {
    public T Instance { get; private set; }

    public string Name { get; private set; }

    public string UniqueId { get; private set; }

    public Named(string name, T instance, string uniqueId)
    {
      this.Instance = instance;
      this.Name = name;
      this.UniqueId = uniqueId;
    }

    public override string ToString()
    {
      return this.Name;
    }
  }
}
