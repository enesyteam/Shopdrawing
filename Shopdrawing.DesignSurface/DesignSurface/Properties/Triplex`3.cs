// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Properties.Triplex`3
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

namespace Microsoft.Expression.DesignSurface.Properties
{
  internal class Triplex<T1, T2, T3>
  {
    private T1 first;
    private T2 second;
    private T3 third;

    public T1 First
    {
      get
      {
        return this.first;
      }
    }

    public T2 Second
    {
      get
      {
        return this.second;
      }
    }

    public T3 Third
    {
      get
      {
        return this.third;
      }
    }

    public Triplex(T1 first, T2 second, T3 third)
    {
      this.first = first;
      this.second = second;
      this.third = third;
    }
  }
}
