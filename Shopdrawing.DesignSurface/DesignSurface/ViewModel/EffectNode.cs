// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.EffectNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public sealed class EffectNode : SceneNode
  {
    public static readonly EffectNode.ConcreteEffectNodeFactory Factory = new EffectNode.ConcreteEffectNodeFactory();

    public bool IsDisabled
    {
      get
      {
        return (bool) this.Parent.GetLocalOrDefaultValue(DesignTimeProperties.IsEffectDisabledProperty);
      }
      set
      {
        if (value)
          this.Parent.SetLocalValue(DesignTimeProperties.IsEffectDisabledProperty, (object) true);
        else
          this.Parent.ClearLocalValue(DesignTimeProperties.IsEffectDisabledProperty);
      }
    }

    public override bool ShouldClearAnimation
    {
      get
      {
        return true;
      }
    }

    public int CompareTo(object obj)
    {
      EffectNode effectNode = obj as EffectNode;
      if (effectNode != null)
        return SceneNode.MarkerCompare((SceneNode) this, (SceneNode) effectNode);
      return 1;
    }

    public class ConcreteEffectNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new EffectNode();
      }
    }
  }
}
