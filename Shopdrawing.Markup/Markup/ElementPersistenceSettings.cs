// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.ElementPersistenceSettings
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using System;

namespace Microsoft.Expression.DesignModel.Markup
{
  internal sealed class ElementPersistenceSettings
  {
    private Type type;
    private int linesOutside;
    private int linesInside;

    public Type Type
    {
      get
      {
        return this.type;
      }
    }

    public int LinesOutside
    {
      get
      {
        return this.linesOutside;
      }
    }

    public int LinesInside
    {
      get
      {
        return this.linesInside;
      }
    }

    public ElementPersistenceSettings(Type type, int linesOutside, int linesInside)
    {
      this.type = type;
      this.linesOutside = linesOutside;
      this.linesInside = linesInside;
    }
  }
}
