// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Timeline.DragDrop.BindingSceneInsertionPoint
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.Timeline.DragDrop
{
  public sealed class BindingSceneInsertionPoint : ISceneInsertionPoint
  {
    private SceneNode sceneNode;
    private IProperty property;

    public int InsertIndex { get; set; }

    public SceneNode SceneNode
    {
      get
      {
        return this.sceneNode;
      }
    }

    public SceneElement SceneElement
    {
      get
      {
        return this.SceneNode as SceneElement;
      }
    }

    public IProperty Property
    {
      get
      {
        return this.property;
      }
    }

    public BindingSceneInsertionPoint(SceneNode sceneNode, IProperty property, int insertIndex)
    {
      this.sceneNode = sceneNode;
      this.property = property;
      this.InsertIndex = insertIndex;
    }

    public void Insert(SceneNode nodeToInsert)
    {
      if (this.InsertIndex < 0)
        this.SceneElement.DefaultContent.Add(nodeToInsert);
      else
        this.SceneElement.DefaultContent.Insert(this.InsertIndex, nodeToInsert);
    }

    public bool CanInsert(ITypeId typeToInsert)
    {
      if (this.SceneElement != null)
      {
        IType typeToInsert1 = this.SceneNode.ProjectContext.ResolveType(typeToInsert);
        IProperty defaultContentProperty = this.SceneNode.DefaultContentProperty;
        if (typeToInsert1 != null && defaultContentProperty != null)
          return PropertySceneInsertionPoint.IsTypeCompatible(this.SceneNode, typeToInsert1, defaultContentProperty);
      }
      return false;
    }
  }
}
