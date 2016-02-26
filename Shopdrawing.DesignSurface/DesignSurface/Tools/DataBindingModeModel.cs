// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.DataBindingModeModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal class DataBindingModeModel : NotifyingObject
  {
    private static DataBindingModeModel instance = new DataBindingModeModel();
    private DataBindingMode mode;

    public static DataBindingModeModel Instance
    {
      get
      {
        return DataBindingModeModel.instance;
      }
    }

    public DataBindingMode Mode
    {
      get
      {
        return this.mode;
      }
      private set
      {
        if (this.mode == value)
          return;
        this.mode = value;
        this.OnPropertyChanged("Mode");
      }
    }

    public DataBindingMode NormalizedMode
    {
      get
      {
        if (this.Mode == DataBindingMode.StickyDetails)
          return DataBindingMode.Details;
        return this.Mode;
      }
    }

    public void SetMode(DataBindingMode mode, bool force)
    {
      if (force)
      {
        this.Mode = mode;
      }
      else
      {
        if (this.Mode == DataBindingMode.StickyDetails)
          return;
        this.Mode = mode;
      }
    }
  }
}
