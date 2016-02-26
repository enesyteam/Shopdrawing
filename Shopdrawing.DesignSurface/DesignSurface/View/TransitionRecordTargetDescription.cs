// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.View.TransitionRecordTargetDescription
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

namespace Microsoft.Expression.DesignSurface.View
{
  public class TransitionRecordTargetDescription : RecordTargetDescriptionBase
  {
    private string fromStateName;
    private string toStateName;

    public string FromStateName
    {
      get
      {
        return this.fromStateName;
      }
      set
      {
        this.fromStateName = value;
        this.SendPropertyChanged("FromStateName");
      }
    }

    public string ToStateName
    {
      get
      {
        return this.toStateName;
      }
      set
      {
        this.toStateName = value;
        this.SendPropertyChanged("ToStateName");
      }
    }
  }
}
