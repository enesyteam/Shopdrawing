// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.SceneNodeSearch.SearchPath
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.ViewModel.SceneNodeSearch
{
  public class SearchPath
  {
    private SearchStep[] steps;

    public SearchScope Scope
    {
      get
      {
        SearchScope searchScope = SearchScope.None;
        foreach (SearchStep searchStep in this.steps)
        {
          searchScope |= searchStep.Axis.Scope;
          if (searchStep.Predicate != null)
            searchScope |= searchStep.Predicate.AnalysisScope;
          if (searchScope == SearchScope.Unknown)
            break;
        }
        return searchScope;
      }
    }

    public int NumberOfSteps
    {
      get
      {
        return this.steps.Length;
      }
    }

    public SearchPath(params SearchStep[] steps)
    {
      this.steps = steps;
    }

    public IEnumerable<SceneNode> Query(SceneNode basisNode)
    {
      foreach (SceneNode sceneNode in this.EnumerateNodesWorker(0, basisNode))
        yield return sceneNode;
    }

    private IEnumerable<SceneNode> EnumerateNodesWorker(int currentStep, SceneNode basisNode)
    {
      foreach (SceneNode basisNode1 in this.steps[currentStep].Query(basisNode))
      {
        if (currentStep == this.steps.Length - 1)
        {
          yield return basisNode1;
        }
        else
        {
          foreach (SceneNode sceneNode in this.EnumerateNodesWorker(currentStep + 1, basisNode1))
            yield return sceneNode;
        }
      }
    }

    public SceneNode QueryFirst(SceneNode basisNode)
    {
      IEnumerator<SceneNode> enumerator = this.Query(basisNode).GetEnumerator();
      if (enumerator.MoveNext())
        return enumerator.Current;
      return (SceneNode) null;
    }

    public SearchStep Step(int index)
    {
      return this.steps[index];
    }
  }
}
