// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Properties.KeyframeData
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.ViewModel;
using System;

namespace Microsoft.Expression.DesignSurface.Properties
{
  internal class KeyframeData : Triplex<KeyFrameAnimationSceneNode, double, SceneNode>, IComparable
  {
    public KeyframeData(KeyFrameAnimationSceneNode node, double time, SceneNode element)
      : base(node, time, element)
    {
    }

    public static bool operator ==(KeyframeData x, KeyframeData y)
    {
      return object.Equals((object) x, (object) y);
    }

    public static bool operator !=(KeyframeData x, KeyframeData y)
    {
      return !object.Equals((object) x, (object) y);
    }

    public static bool operator >(KeyframeData x, KeyframeData y)
    {
      if (x != (KeyframeData) null)
        return x.CompareTo((object) y) > 0;
      return false;
    }

    public static bool operator <(KeyframeData x, KeyframeData y)
    {
      if (x != (KeyframeData) null)
        return x.CompareTo((object) y) < 0;
      return false;
    }

    public int CompareTo(object obj)
    {
      int num = 0;
      KeyframeData keyframeData = obj as KeyframeData;
      if (keyframeData != (KeyframeData) null && this.Second != keyframeData.Second)
        num = this.Second > keyframeData.Second ? 1 : -1;
      return num;
    }

    public override bool Equals(object obj)
    {
      KeyframeData keyframeData = obj as KeyframeData;
      if (keyframeData != (KeyframeData) null && this.First == keyframeData.First && this.Second == keyframeData.Second)
        return this.Third == keyframeData.Third;
      return false;
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }
  }
}
