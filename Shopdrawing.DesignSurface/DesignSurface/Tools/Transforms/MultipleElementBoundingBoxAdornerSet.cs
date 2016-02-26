﻿// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.MultipleElementBoundingBoxAdornerSet
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal class MultipleElementBoundingBoxAdornerSet : AdornerSet
  {
    public MultipleElementBoundingBoxAdornerSet(ToolBehaviorContext toolContext, AdornerElementSet adornedElementSet)
      : base(toolContext, adornedElementSet, AdornerSetOrderTokens.BoundingBoxPriority)
    {
    }

    protected override void CreateAdorners()
    {
      this.AddAdorner((Adorner) new MultipleElementBoundingBoxAdorner((AdornerSet) this));
    }
  }
}