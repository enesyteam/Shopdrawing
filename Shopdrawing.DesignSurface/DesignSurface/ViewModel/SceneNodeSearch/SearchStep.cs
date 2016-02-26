// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.SceneNodeSearch.SearchStep
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.ViewModel.SceneNodeSearch
{
  public class SearchStep
  {
    private SearchAxis axis;
    private ISearchPredicate predicate;
    private ISearchPredicate continuePredicate;

    public ISearchPredicate Predicate
    {
      get
      {
        return this.predicate;
      }
    }

    public ISearchPredicate ContinuePredicate
    {
      get
      {
        return this.continuePredicate;
      }
    }

    public SearchAxis Axis
    {
      get
      {
        return this.axis;
      }
    }

    public SearchStep(SearchAxis axis)
      : this(axis, (ISearchPredicate) null)
    {
    }

    public SearchStep(SearchAxis axis, ISearchPredicate predicate)
      : this(axis, predicate, (ISearchPredicate) null)
    {
    }

    public SearchStep(SearchAxis axis, ISearchPredicate predicate, ISearchPredicate continuePredicate)
    {
      this.axis = axis;
      this.predicate = predicate;
      this.continuePredicate = continuePredicate;
    }

    public IEnumerable<SceneNode> Query(SceneNode basisNode)
    {
      foreach (SceneNode subject in this.axis.Enumerate(basisNode, this.continuePredicate))
      {
        if (this.predicate == null || this.predicate.Test(subject))
          yield return subject;
      }
    }
  }
}
