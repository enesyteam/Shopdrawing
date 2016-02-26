// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.ZOrderComparer`1
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal sealed class ZOrderComparer<T> : IComparer<T> where T : SceneNode
  {
    private int depthMultiplier = 1;
    private int siblingMultiplier = 1;
    private SceneNode root;

    public ZOrderComparer(SceneNode root, bool markupOrder, bool depthFirst)
    {
      if (root == null)
        throw new ArgumentNullException("root");
      this.root = root;
      if (markupOrder)
        this.siblingMultiplier = -1;
      if (!depthFirst)
        return;
      this.depthMultiplier = -1;
    }

    public ZOrderComparer(SceneNode root)
      : this(root, false, false)
    {
    }

    public int Compare(T left, T right)
    {
      if ((object) left == (object) right)
        return 0;
      if (left.Parent == null)
        return this.depthMultiplier;
      if (right.Parent == null)
        return -this.depthMultiplier;
      if (left.Parent == right.Parent)
      {
        IProperty propertyForChild1 = left.Parent.GetPropertyForChild((SceneNode) left);
        IProperty propertyForChild2 = right.Parent.GetPropertyForChild((SceneNode) right);
        if (propertyForChild1 == propertyForChild2)
          return this.RelativeOrder((SceneNode) left, (SceneNode) right);
        return string.Compare(propertyForChild1.Name, propertyForChild2.Name, true, CultureInfo.CurrentCulture) * this.siblingMultiplier;
      }
      IList<SceneNode> pathTo1 = this.GetPathTo((SceneNode) left);
      IList<SceneNode> pathTo2 = this.GetPathTo((SceneNode) right);
      for (int index = 0; index < Math.Min(pathTo1.Count, pathTo2.Count); ++index)
      {
        if (pathTo1[index] != pathTo2[index])
          return this.RelativeOrder(pathTo1[index], pathTo2[index]);
      }
      return (pathTo1.Count - pathTo2.Count) * this.depthMultiplier;
    }

    private int RelativeOrder(SceneNode left, SceneNode right)
    {
      SceneNode parent = left.Parent;
      if (parent.GetPropertyForChild(left) != parent.GetPropertyForChild(right))
        return -1;
      ISceneNodeCollection<SceneNode> collectionForChild = parent.GetCollectionForChild(left);
      int num1 = collectionForChild.IndexOf(left);
      int num2 = collectionForChild.IndexOf(right);
      if (num1 < 0 || num2 < 0)
        throw new InvalidOperationException(ExceptionStringTable.ZOrderComparerBadLogicalTree);
      return (num1 - num2) * this.siblingMultiplier;
    }

    private IList<SceneNode> GetPathTo(SceneNode element)
    {
      List<SceneNode> list = new List<SceneNode>();
      for (; element != null && element != this.root; element = element.Parent)
        list.Insert(0, element);
      return (IList<SceneNode>) list;
    }
  }
}
