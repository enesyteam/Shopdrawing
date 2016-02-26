// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.View.RectangleHitTestResultTreeNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.Collections.Generic;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.View
{
  public class RectangleHitTestResultTreeNode
  {
    private List<RectangleHitTestResultTreeNode> children = new List<RectangleHitTestResultTreeNode>();
    private List<RectangleHitTestResultTreeLeaf> leaves = new List<RectangleHitTestResultTreeLeaf>();
    private DependencyObject target;
    private RectangleHitTestResultTreeNode parent;
    private bool sorted;

    public RectangleHitTestResultTreeNode Parent
    {
      get
      {
        return this.parent;
      }
    }

    public DependencyObject Target
    {
      get
      {
        return this.target;
      }
    }

    public RectangleHitTestResult ClosestHitTestResult
    {
      get
      {
        List<RectangleHitTestResult> hitTestResults = this.GetHitTestResults();
        if (hitTestResults.Count > 0)
          return hitTestResults[0];
        return (RectangleHitTestResult) null;
      }
    }

    public IEnumerable<RectangleHitTestResult> ClosestFirstHitTestResults
    {
      get
      {
        foreach (RectangleHitTestResult rectangleHitTestResult in this.GetHitTestResults())
          yield return rectangleHitTestResult;
      }
    }

    public RectangleHitTestResultTreeNode(RectangleHitTestResultTreeNode parent, DependencyObject target)
    {
      this.parent = parent;
      this.target = target;
    }

    public void AddChild(RectangleHitTestResultTreeNode child)
    {
      this.children.Add(child);
    }

    public void AddLeaf(RectangleHitTestResultTreeLeaf leaf)
    {
      this.leaves.Add(leaf);
    }

    private void Sort()
    {
      if (this.sorted)
        return;
      this.sorted = true;
    }

    private List<RectangleHitTestResult> GetHitTestResults()
    {
      this.Sort();
      List<RectangleHitTestResult> list = new List<RectangleHitTestResult>();
      foreach (RectangleHitTestResultTreeNode testResultTreeNode in this.children)
      {
        foreach (RectangleHitTestResult rectangleHitTestResult in testResultTreeNode.ClosestFirstHitTestResults)
          list.Add(rectangleHitTestResult);
      }
      foreach (RectangleHitTestResultTreeLeaf testResultTreeLeaf in this.leaves)
      {
        foreach (RectangleHitTestResult rectangleHitTestResult in testResultTreeLeaf.HitResults)
          list.Add(rectangleHitTestResult);
      }
      list.Sort((Comparison<RectangleHitTestResult>) ((result1, result2) =>
      {
        if (result1.DistanceToRectangleCenter < result2.DistanceToRectangleCenter)
          return -1;
        return result1.DistanceToRectangleCenter > result2.DistanceToRectangleCenter ? true : false;
      }));
      return list;
    }
  }
}
