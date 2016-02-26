// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.SkinEditing.BaseStateModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;

namespace Microsoft.Expression.DesignSurface.UserInterface.SkinEditing
{
  public class BaseStateModel : StateModelBase
  {
    public override string Name
    {
      get
      {
        return StringTable.BaseStateName;
      }
      set
      {
        throw new InvalidOperationException("Cannot set Name on Base state");
      }
    }

    public override bool IsStructureEditable
    {
      get
      {
        return false;
      }
    }

    protected override void Select()
    {
      this.Manager.SelectBaseState();
    }

    protected override void Pin()
    {
    }
  }
}
