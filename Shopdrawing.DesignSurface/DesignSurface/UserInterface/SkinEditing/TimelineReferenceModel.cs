// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.SkinEditing.TimelineReferenceModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.UserInterface.SkinEditing
{
  public sealed class TimelineReferenceModel
  {
    public StateGroupModel Group { get; private set; }

    public TimelineSceneNode.PropertyNodePair TargetElementAndProperty { get; private set; }

    public SceneNode StateOrTransition { get; private set; }

    public TimelineReferenceModel(StateGroupModel group, SceneNode stateOrTransition, TimelineSceneNode.PropertyNodePair propertyNode)
    {
      this.Group = group;
      this.TargetElementAndProperty = propertyNode;
      this.StateOrTransition = stateOrTransition;
    }
  }
}
