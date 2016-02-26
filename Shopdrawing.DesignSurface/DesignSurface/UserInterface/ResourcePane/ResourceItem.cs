// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.ResourcePane.ResourceItem
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using System;

namespace Microsoft.Expression.DesignSurface.UserInterface.ResourcePane
{
  public abstract class ResourceItem : ResourceEntryBase
  {
    private ResourceManager resourceManager;
    private ResourceContainer resourceContainer;

    public string EffectiveTypeName
    {
      get
      {
        return this.EffectiveType.Name;
      }
    }

    public abstract Type EffectiveType { get; }

    public ResourceContainer Container
    {
      get
      {
        return this.resourceContainer;
      }
    }

    internal DesignerContext DesignerContext
    {
      get
      {
        return this.resourceManager.DesignerContext;
      }
    }

    protected override ResourceContainer DragDropTargetContainer
    {
      get
      {
        return this.Container;
      }
    }

    protected override ResourceManager ResourceManager
    {
      get
      {
        return this.resourceManager;
      }
    }

    protected ResourceItem(ResourceManager resourceManager, ResourceContainer resourceContainer)
    {
      this.resourceManager = resourceManager;
      this.resourceContainer = resourceContainer;
    }

    public abstract void InteractiveDelete();
  }
}
